using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly WebFunctions _functions;
        private readonly AppointmentService _appointmentService;

        public AppointmentController(EkasContext context)
        {
            _functions = new WebFunctions(context);
            _appointmentService = new AppointmentService(context);
        }

        public IActionResult Index()
        {
            var user = HttpContext.User.Claims.ToList();

            if (user.Count() == 0)
                return RedirectToAction("Login", "Appointment");
            else
            {
                if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
                {
                    return RedirectToAction("Login", "Appointment");
                }
                else
                {
                    var customer = HttpContext.User.Claims.ToList();
                    IndexVM indexVM = _appointmentService.GetCustomerAppointmentData(customer.FirstOrDefault(u => u.Type == "UserId").Value);
                    ViewBag.Count = indexVM.AppointmentData.Count;

                    return View(indexVM);
                }
            }
        }

        public IActionResult Fillin()
        {
            var user = HttpContext.User.Claims.ToList();

            if (user.Count() == 0)
                return RedirectToAction("Login", "Appointment");
            else
            {
                if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
                {
                    return RedirectToAction("Login", "Appointment");
                }
                else
                {
                    int AppointmentCount = _appointmentService.GetCustomerAppointmentCount(user.FirstOrDefault(u => u.Type == "UserId").Value);

                    ViewBag.AppointmentCount = AppointmentCount;
                    //var customer = HttpContext.User.Claims.ToList();
                    //var item = _appointmentService.GetCustomerDateByLineId(customer.FirstOrDefault(u => u.Type == "LineId").Value);

                    //IndexVM indexVM = new IndexVM()
                    //{
                    //    Id = item.Id,
                    //    Name = item.Name,
                    //    LineId = item.LineId,
                    //    LineDisplayName = item.LineDisplayName,
                    //    LinePictureUrl = item.LinePictureUrl,
                    //    CellPhone = item.CellPhone,
                    //    NationalIdNumber = item.NationalIdNumber,
                    //    Gender = item.Gender,
                    //    Birthday = item.Birthday,
                    //    Email = item.Email,
                    //    Memo = item.Memo
                    //};

                    return View();
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = HttpContext.User.Claims.ToList();

            if (user.Count() == 0)
                return RedirectToAction("Login", "Appointment");
            else
            {
                if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
                {
                    return RedirectToAction("Login", "Appointment");
                }
                else
                {
                    EditVM AppointmentData = _appointmentService.GetAppointmentDataById(id);

                    return View(AppointmentData);
                }
            }
        }

        public IActionResult NoticeVerification()
        {
            return View();
        }

        public IActionResult SuccessPage(string id)
        {
            SuccessPageVM AppointmentData = _appointmentService.GetAppointmentDataByIdForSuccessPage(id);

            return View(AppointmentData);
        }

        public IActionResult FailedPage()
        {
            return View();
        }

        public IActionResult Login()
        {
            //有登入就跳轉到首頁
            var user = HttpContext.User.Claims.ToList();

            if (user.Count == 0)
                return View();
            else
            {
                if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "Customer")
                    return RedirectToAction("Index", "Appointment");
            }

            return View();
        }


        #region ---AJAX---

        [HttpGet]
        public async Task<IActionResult> Redirect(string code, string state)
        {
            string LineLoginChannelId = _functions.GetSystemParameter("LineLoginChannelId");
            string LineLoginChannelSecret = _functions.GetSystemParameter("LineLoginChannelSecret");
            string LineLoginCallbackURL = _functions.GetSystemParameter("LineLoginCallbackURL");

            using (var client = new HttpClient())
            {
                var tokenResponse = await client.PostAsync("https://api.line.me/oauth2/v2.1/token", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", LineLoginCallbackURL),
                    new KeyValuePair<string, string>("client_id", LineLoginChannelId),
                    new KeyValuePair<string, string>("client_secret", LineLoginChannelSecret)
                }));

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    return BadRequest("登入失敗");
                }

                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(tokenContent);
                var accessToken = tokenData["access_token"].ToString();
                var refreshToken = tokenData["refresh_token"]?.ToString();
                var expiresIn = (int)tokenData["expires_in"];

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var profileResponse = await client.GetAsync("https://api.line.me/v2/profile");
                var profileContent = await profileResponse.Content.ReadAsStringAsync();
                var profileData = JObject.Parse(profileContent);

                var lineUserId = profileData["userId"].ToString();
                var displayName = profileData["displayName"].ToString();
                var pictureUrl = profileData["pictureUrl"]?.ToString();

                if (_appointmentService.CheckCustomerExist(lineUserId))
                {
                    var user = _appointmentService.GetCustomerDateByLineId(lineUserId);

                    Customer item = new Customer()
                    {
                        Modifier = "Default",
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        LineDisplayName = displayName,
                        LinePictureUrl = pictureUrl,
                    };
                    _appointmentService.UpdateCustomerLineInformation(item, lineUserId);

                    var claims = new List<Claim>
                    {
                        new Claim("LoginType", "Customer"),
                        new Claim("UserId", user.Id),
                        new Claim("LineId", lineUserId),
                        new Claim("Name", displayName),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = user.Id,
                        UserAccount = lineUserId,
                        Description = "Customer '" + user.Id + "' login."
                    });


                    var token = new Customertoken
                    {
                        Creator = "Default",
                        Modifier = "Default",
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        CustomerId = user.Id,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresIn = expiresIn
                    };

                    _appointmentService.CreateCustomerToken(token);
                }
                else
                {
                    //新顧客
                    string CustomerId = _functions.GetGuid();

                    while (_appointmentService.CheckCustomerId(CustomerId))
                        CustomerId = _functions.GetGuid();

                    Customer item = new Customer()
                    {
                        Creator = "Default",
                        Modifier = "Default",
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        Id = CustomerId,
                        Name = displayName,
                        LineId = lineUserId,
                        LineDisplayName = displayName,
                        LinePictureUrl = pictureUrl,
                        CellPhone = "",
                        NationalIdNumber = "",
                        Gender = "",
                        Birthday = "",
                        Email = "",
                        Memo = ""
                    };
                    _appointmentService.CreateCustomer(item);

                    _functions.SaveSystemLog(new Systemlog
                    {
                        CreateDate = DateTime.Now,
                        Creator = "Default",
                        UserAccount = "Default",
                        Description = "Add customer id='" + CustomerId + "'."
                    });

                    var claims = new List<Claim>
                    {
                        new Claim("LoginType", "Customer"),
                        new Claim("UserId", CustomerId),
                        new Claim("LineId", lineUserId),
                        new Claim("Name", displayName),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = CustomerId,
                        UserAccount = lineUserId,
                        Description = "Customer '" + CustomerId + "' login."
                    });

                    var token = new Customertoken
                    {
                        Creator = "Default",
                        Modifier = "Default",
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        CustomerId = CustomerId,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresIn = expiresIn
                    };

                    _appointmentService.CreateCustomerToken(token);
                }

                return RedirectToAction("Index", "Appointment");
            }
        }

        [HttpGet]
        public IActionResult GetCustomerAppointmentData()
        {
            var customer = HttpContext.User.Claims.ToList();
            IndexVM indexVM = _appointmentService.GetCustomerAppointmentData(customer.FirstOrDefault(u => u.Type == "UserId").Value);

            return new JsonResult(indexVM.AppointmentData);
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string code)
        {
            string AppointmentId = _appointmentService.CheckVerificationCode(code);

            if (AppointmentId != "")
            {
                var user = _appointmentService.GetAppointmentCustomerData(AppointmentId);

                Appointment item = new Appointment()
                {
                    Modifier = user.Id!,
                    ModifyDate = DateTime.Now,

                    Status = "Y"
                };
                _appointmentService.SetAppointmentToOutpatient(AppointmentId, user.Id, item);

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.Id,
                    UserAccount = user.Id,
                    Description = "Set appointment to outpatient Success, id='" + AppointmentId + "'."
                });

                //傳送LINE訊息
                string message = _functions.GetSystemParameter("AppointmentVerifySuccess");
                await _functions.SendLineMessageAsync(user.LineId, message);

                return RedirectToAction("SuccessPage", "Appointment", new { id = AppointmentId });
            }
            else
                return RedirectToAction("FailedPage", "Appointment");
        }

        [HttpGet]
        public IActionResult setAppointmentIntroductionText()
        {
            string text = _functions.GetSystemParameter("AppointmentIntroductionText");

            text = text.Replace("\r\n", "<br />");
            text = text.Replace("\n", "<br />");

            return new JsonResult(text);
        }

        [HttpGet]
        public IActionResult setTreatmentList()
        {
            List<TreatmentDataVM> data = _appointmentService.GetTreatmentList();

            foreach (var item in data)
            {
                item.Image = "data:image/" + item.ImageFile.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(item.ImageFile.Path);
            }

            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult setDoctorList(string[] treatments)
        {
            List<DoctorDataVM> data = _appointmentService.GetDoctorList(treatments);

            foreach (var item in data)
            {
                item.Image = "data:image/" + item.ImageFile.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(item.ImageFile.Path);
            }

            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult getDateAndTime(string[] treatments, string doctor, string appointmentId)
        {
            string user = HttpContext.User.Claims.ToList().FirstOrDefault(u => u.Type == "UserId").Value;

            if (appointmentId is not null && appointmentId != "")
                user = _appointmentService.GetAppointmentCustomerData(appointmentId).Id!;

            List<FillinDateTimeVM> results = _appointmentService.GetFillInDateTimeData(treatments, doctor, user, appointmentId);

            return new JsonResult(results);
        }

        [HttpPost]
        public IActionResult UpdateCustomerInfomation(string customerName, string nationalIdNumber, string cellphone, string gender, string birthday, string email)
        {
            var user = HttpContext.User.Claims.ToList();

            Customer item = new Customer()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                ModifyDate = DateTime.Now,

                Name = customerName,
                NationalIdNumber = nationalIdNumber,
                CellPhone = cellphone,
                Gender = gender,
                Birthday = birthday,
                Email = email
            };
            string result = _appointmentService.UpdateCustomer(user.FirstOrDefault(u => u.Type == "UserId").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Description = "Update customer id='" + user.FirstOrDefault(u => u.Type == "UserId").Value + "'."
            });


            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(string[] treatments, string doctor, string date, string beginTime)
        {
            var user = HttpContext.User.Claims.ToList();
            string AppointmentId = _functions.GetGuid();

            while (_appointmentService.CheckAppointmentId(AppointmentId))
                AppointmentId = _functions.GetGuid();

            string BookingEndTime = TimeSpan.Parse(beginTime).Add(new TimeSpan(0, _appointmentService.GetTreatmentListMaxTime(treatments), 0)).ToString();

            BookingEndTime = DateTime.ParseExact(BookingEndTime, "HH:mm:ss", null).ToString("HH:mm");

            Appointment item = new Appointment()
            {
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "N",

                Id = AppointmentId,
                CustomerId = user.FirstOrDefault(u => u.Type == "UserId").Value,
                DoctorId = doctor,
                Date = date,
                BookingBeginTime = beginTime,
                BookingEndTime = BookingEndTime,
                CheckIn = "N",
                CheckInTime = ""
            };
            _appointmentService.CreateAppointment(item);

            //新增預約的療程
            foreach (var treatment in treatments)
            {
                Appointmenttreatment at = new Appointmenttreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    AppointmentId = AppointmentId,
                    Type = "A",
                    TreatmentId = treatment
                };
                _appointmentService.CreateAppointmenttreatment(at);
            }

            //建立驗證資料
            string code = _functions.GetVerificationCode();

            Verificationcode vcode = new Verificationcode()
            {
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                SouceTable = "Appointment",
                ForeignKey = AppointmentId,
                HashCode = code,
                Otp = "",
                ExpireTime = DateTime.Now.AddSeconds(int.Parse(_functions.GetSystemParameter("VerificationCodeExpireTime")))
            };
            _appointmentService.CreateVerificationcode(vcode);

            //傳送line驗證碼
            string VerifyUrl = "https://localhost:7146/Appointment/Verify?code=" + code;
            string message = "預約驗證網址如下，請於5分鐘內進行驗證\n" + VerifyUrl;

            await _functions.SendLineMessageAsync(user.FirstOrDefault(u => u.Type == "LineId").Value, message);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Description = "Add Appointment id='" + AppointmentId + "'."
            });

            return new JsonResult(AppointmentId);
        }

        [HttpPost]
        public IActionResult UpdateAppointment(string AppointmentId, string[] treatments, string doctor, string date, string beginTime)
        {
            var user = HttpContext.User.Claims.ToList();

            string BookingEndTime = TimeSpan.Parse(beginTime).Add(new TimeSpan(0, _appointmentService.GetTreatmentListMaxTime(treatments), 0)).ToString();

            BookingEndTime = DateTime.ParseExact(BookingEndTime, "HH:mm:ss", null).ToString("HH:mm");

            Appointment item = new Appointment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                ModifyDate = DateTime.Now,

                DoctorId = doctor,
                Date = date,
                BookingBeginTime = beginTime,
                BookingEndTime = BookingEndTime
            };
            _appointmentService.UpdateAppointment(item, AppointmentId);

            //刪除原本的療程後新增
            _appointmentService.RemoveAppointmenttreatment(AppointmentId);

            foreach (var treatment in treatments)
            {
                Appointmenttreatment at = new Appointmenttreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    AppointmentId = AppointmentId,
                    Type = "A",
                    TreatmentId = treatment
                };
                _appointmentService.CreateAppointmenttreatment(at);
            }

            //更新門診時刻表
            _appointmentService.UpdateAppointmentToOutpatient(AppointmentId, user.FirstOrDefault(u => u.Type == "UserId").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Description = "Add Appointment id='" + AppointmentId + "'."
            });

            return new JsonResult(AppointmentId);
        }

        [HttpPost]
        public IActionResult CancelAppointment(string AppointmentId)
        {
            var user = HttpContext.User.Claims.ToList();

            //設定appointment狀態為C(cancel)
            Appointment item = new Appointment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                ModifyDate = DateTime.Now,

                Status = "C"
            };
            _appointmentService.CancelAppointmentAndOutpatient(AppointmentId, user.FirstOrDefault(u => u.Type == "UserId").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Description = "Cancel appointment success id='" + AppointmentId + "'."
            });


            return new JsonResult(AppointmentId);
        }

        [HttpGet]
        public IActionResult setAppointmentDesktopImage()
        {
            string ImageId = _functions.GetSystemParameter("AppointmentPageDesktopImage");
            Systemfile file = _appointmentService.GetFileData(ImageId);

            string result = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path);

            return new JsonResult(result);
        }

        [HttpGet]
        public IActionResult setAppointmentMobileImage()
        {
            string ImageId = _functions.GetSystemParameter("AppointmentPageMobileImage");
            Systemfile file = _appointmentService.GetFileData(ImageId);

            string result = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path);

            return new JsonResult(result);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Appointment");//導至登入頁
        }

        #endregion



        //發送訊息測試
        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> SendTestMessage(string id)
        //{
        //    var result = await _appointmentService.SendLineMessageAsync(id, "傳送訊息功能測試~~~");
        //    if (!result)
        //    {
        //        return BadRequest("訊息傳送失敗");
        //    }

        //    return Ok("訊息已傳送");

        //}



    }
}
