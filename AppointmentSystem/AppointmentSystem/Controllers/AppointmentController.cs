using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Security.Claims;

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

        public IActionResult Index()
        {
            var user = HttpContext.User.Claims.ToList();

            if (user == null)
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
                    var item = _appointmentService.GetCustomerDateByLineId(customer.FirstOrDefault(u => u.Type == "LineId").Value);

                    IndexVM indexVM = new IndexVM()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        LineId = item.LineId,
                        LineDisplayName = item.LineDisplayName,
                        LinePictureUrl = item.LinePictureUrl,
                        CellPhone = item.CellPhone,
                        NationalIdNumber = item.NationalIdNumber,
                        Gender = item.Gender,
                        Birthday = item.Birthday,
                        Email = item.Email,
                        Memo = item.Memo
                    };

                    return View(indexVM);
                }
            }
        }

        public IActionResult Fillin()
        {
            var user = HttpContext.User.Claims.ToList();

            if (user == null)
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
        public IActionResult Verify(string code)
        {
            string AppointmentId = _appointmentService.CheckVerificationCode(code);

            if (AppointmentId != "")
            {
                var user = HttpContext.User.Claims.ToList();

                Appointment item = new Appointment()
                {
                    Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    ModifyDate = DateTime.Now,

                    Status = "Y"
                };
                _appointmentService.SetAppointmentToOutpatient(AppointmentId, user.FirstOrDefault(u => u.Type == "UserId").Value, item);

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    Description = "Set appointment to outpatient Success, id='" + AppointmentId + "'."
                });

                return RedirectToAction("SuccessPage", "Appointment");
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
        public IActionResult getDateAndTime(string[] treatments, string doctor)
        {
            List<FillinDateTimeVM> results = _appointmentService.GetFillInDateTimeData(treatments, doctor);

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
        public IActionResult CreateAppointment(string[] treatments, string doctor, string date, string beginTime)
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
            Verificationcode vcode = new Verificationcode()
            {
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                SouceTable = "Appointment",
                ForeignKey = AppointmentId,
                HashCode = _functions.GetVerificationCode(),
                Otp = "",
                ExpireTime = DateTime.Now.AddSeconds(int.Parse(_functions.GetSystemParameter("VerificationCodeExpireTime")))
            };
            _appointmentService.CreateVerificationcode(vcode);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
                Description = "Add Appointment id='" + AppointmentId + "'."
            });

            return new JsonResult(AppointmentId);
        }





        public IActionResult NoticeVerification()
        {
            return View();
        }

        public IActionResult SuccessPage()
        {
            return View();
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Appointment");//導至登入頁
        }







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
