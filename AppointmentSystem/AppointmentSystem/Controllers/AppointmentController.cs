using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Services;
using Azure.Core;
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
            var user = HttpContext.User.Claims.ToList();
            var customerData = _appointmentService.GetCustomerDateById(AppointmentData.CustomerId);

            if (customerData.LineId != "")
                ViewBag.showBindBtn = "false";
            else
                ViewBag.showBindBtn = "true";

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

        public async Task<IActionResult> CellphoneLogin(string cellphone, string hashCode, string otp)
        {
            //驗證手機及驗證碼
            string result = "";

            if (cellphone == "")
                result = "手機號碼資料異常，請聯絡系統管理者!";
            else if (hashCode == "" || otp == "")
                result = "驗證碼資料異常，請聯絡系統管理者!";
            else
                result = _appointmentService.VerifyLoginInfo(cellphone, hashCode, otp);

            if (result == "OK")
            {
                if (_appointmentService.CheckCustomerExistByCellphone(cellphone))
                {
                    var user = _appointmentService.GetCustomerDateByCellphone(cellphone);

                    var claims = new List<Claim>
                    {
                        new Claim("LoginType", "Customer"),
                        new Claim("UserId", user.Id),
                        new Claim("LoginBy", "Cellphone"),
                        new Claim("LineId", user.LineId),
                        new Claim("Name", user.DisplayName),
                        new Claim("Phone", user.CellPhone),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = user.Id,
                        UserAccount = user.Id,
                        Description = "Customer '" + user.Id + "' login."
                    });
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
                        Name = cellphone,
                        LineId = "",
                        DisplayName = cellphone,
                        LinePictureUrl = "",
                        CellPhone = cellphone,
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
                        new Claim("LoginBy", "Cellphone"),
                        new Claim("LineId", ""),
                        new Claim("Name", cellphone),
                        new Claim("Phone", cellphone),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = CustomerId,
                        UserAccount = CustomerId,
                        Description = "Customer '" + CustomerId + "' login."
                    });
                }

                return RedirectToAction("Index", "Appointment");
            }
            else
            {
                ViewBag.ErrorMsg = result;

                return View();
            }
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

                        LinePictureUrl = pictureUrl,
                    };
                    _appointmentService.UpdateCustomerLineInformation(item, lineUserId);

                    var claims = new List<Claim>
                    {
                        new Claim("LoginType", "Customer"),
                        new Claim("UserId", user.Id),
                        new Claim("LoginBy", "Line"),
                        new Claim("LineId", lineUserId),
                        new Claim("Name", displayName),
                        new Claim("Phone", user.CellPhone),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = user.Id,
                        UserAccount = user.Id,
                        Description = "Customer '" + user.Id + "' login."
                    });

                    //Line Token
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
                        DisplayName = displayName,
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
                        new Claim("LoginBy", "Line"),
                        new Claim("LineId", lineUserId),
                        new Claim("Name", displayName),
                        new Claim("Phone", ""),
                        new Claim("IsAdmin", "N")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = CustomerId,
                        UserAccount = CustomerId,
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
        public async Task<IActionResult> BindLineAccount(string customerId, string code, string state)
        {
            string LineLoginChannelId = _functions.GetSystemParameter("LineLoginChannelId");
            string LineLoginChannelSecret = _functions.GetSystemParameter("LineLoginChannelSecret");
            string LineLoginCallbackURL = _functions.GetSystemParameter("SystemDomainName") + "/Appointment/BindLineAccount?customerId=" + customerId;

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

                //將LineId設定到顧客資料表裡面
                Customer item = new Customer()
                {
                    Modifier = customerId,
                    ModifyDate = DateTime.Now,

                    LineId = lineUserId,
                    LinePictureUrl = pictureUrl,
                };
                _appointmentService.BindCustomerLineId(item, customerId);

                _functions.SaveSystemLog(new Systemlog()
                {
                    CreateDate = DateTime.Now,
                    Creator = customerId,
                    UserAccount = customerId,
                    Description = "Customer bind line account success id='" + customerId + "'."
                });

                var user = _appointmentService.GetCustomerDateById(customerId);
                var claims = new List<Claim>
                {
                    new Claim("LoginType", "Customer"),
                    new Claim("UserId", user.Id),
                    new Claim("LoginBy", "Line"),
                    new Claim("LineId", lineUserId),
                    new Claim("Name", displayName),
                    new Claim("Phone", user.CellPhone),
                    new Claim("IsAdmin", "N")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                _functions.SaveSystemLog(new Systemlog()
                {
                    CreateDate = DateTime.Now,
                    Creator = user.Id,
                    UserAccount = user.Id,
                    Description = "Customer '" + user.Id + "' login."
                });

                //if (_appointmentService.CheckCustomerExist(lineUserId))
                //{
                //    var user = _appointmentService.GetCustomerDateByLineId(lineUserId);

                //    Customer item = new Customer()
                //    {
                //        Modifier = "Default",
                //        ModifyDate = DateTime.Now,
                //        Status = "Y",

                //        LinePictureUrl = pictureUrl,
                //    };
                //    _appointmentService.UpdateCustomerLineInformation(item, lineUserId);

                //    var claims = new List<Claim>
                //    {
                //        new Claim("LoginType", "Customer"),
                //        new Claim("UserId", user.Id),
                //        new Claim("LoginBy", "Line"),
                //        new Claim("LineId", lineUserId),
                //        new Claim("Name", displayName),
                //        new Claim("Phone", user.CellPhone),
                //        new Claim("IsAdmin", "N")
                //    };

                //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                //    _functions.SaveSystemLog(new Systemlog()
                //    {
                //        CreateDate = DateTime.Now,
                //        Creator = user.Id,
                //        UserAccount = user.Id,
                //        Description = "Customer '" + user.Id + "' login."
                //    });

                //    //Line Token
                //    var token = new Customertoken
                //    {
                //        Creator = "Default",
                //        Modifier = "Default",
                //        CreateDate = DateTime.Now,
                //        ModifyDate = DateTime.Now,
                //        Status = "Y",

                //        CustomerId = user.Id,
                //        AccessToken = accessToken,
                //        RefreshToken = refreshToken,
                //        ExpiresIn = expiresIn
                //    };

                //    _appointmentService.CreateCustomerToken(token);
                //}

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

        //[HttpGet]
        //public async Task<IActionResult> Verify(string code)
        //{
        //    Verificationcode vc = _appointmentService.CheckVerificationCode(code);

        //    if (vc is not null)
        //    {
        //        string AppointmentId = vc.ForeignKey;
        //        var user = _appointmentService.GetAppointmentCustomerData(AppointmentId);

        //        Appointment item = new Appointment()
        //        {
        //            Modifier = user.Id!,
        //            ModifyDate = DateTime.Now,

        //            Status = "Y"
        //        };
        //        _appointmentService.SetAppointmentToOutpatient(AppointmentId, user.Id, item);

        //        _functions.SaveSystemLog(new Systemlog
        //        {
        //            CreateDate = DateTime.Now,
        //            Creator = user.Id,
        //            UserAccount = user.Id,
        //            Description = "Set appointment to outpatient Success, id='" + AppointmentId + "'."
        //        });

        //        //傳送LINE訊息
        //        //string message = _functions.GetSystemParameter("AppointmentVerifySuccess");
        //        string Url = _functions.GetSystemParameter("SystemDomainName") + "/Appointment/SuccessPage/" + AppointmentId;
        //        string message = "預約驗證成功，詳細資料如下網址\n" + Url;

        //        if (vc.LoginBy == "Line")
        //            await _functions.SendLineMessageAsync(user.LineId, message);
        //        else if (vc.LoginBy == "Cellphone")
        //            await _functions.SendSmsAsync(user.CellPhone, message);

        //        return RedirectToAction("SuccessPage", "Appointment", new { id = AppointmentId });
        //    }
        //    else
        //        return RedirectToAction("FailedPage", "Appointment");
        //}

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

            string customerMedicalRecordNumber = _appointmentService.getCustomerMedicalRecordNumber(birthday);

            Customer item = new Customer()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                ModifyDate = DateTime.Now,

                MedicalRecordNumber = customerMedicalRecordNumber,
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
                Status = "Y",

                Id = AppointmentId,
                Type = "Appointment",
                CustomerId = user.FirstOrDefault(u => u.Type == "UserId").Value,
                DoctorId = doctor,
                Date = date,
                BookingBeginTime = beginTime,
                BookingEndTime = BookingEndTime,
                CheckIn = "N",
                CheckInTime = ""
            };
            _appointmentService.CreateAppointment(item);
            _appointmentService.SetAppointmentToOutpatient(AppointmentId, user.FirstOrDefault(u => u.Type == "UserId").Value);

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

            //傳送LINE訊息
            string Url = _functions.GetSystemParameter("SystemDomainName") + "/Appointment/SuccessPage/" + AppointmentId;
            string message = "已為您預約EK美學門診時間，詳細預約資訊如下列網址\n" + Url;

            if (user.FirstOrDefault(u => u.Type == "LoginBy").Value == "Line")
                await _functions.SendLineMessageAsync(user.FirstOrDefault(u => u.Type == "LineId").Value, message);
            else if (user.FirstOrDefault(u => u.Type == "LoginBy").Value == "Cellphone")
                await _functions.SendSmsAsync(user.FirstOrDefault(u => u.Type == "Phone").Value, message);

            return new JsonResult(AppointmentId);
            //return RedirectToAction("SuccessPage", "Appointment", new { id = AppointmentId });

            //建立驗證資料
            //string code = _functions.GetVerificationCode();

            //Verificationcode vcode = new Verificationcode()
            //{
            //    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
            //    Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
            //    CreateDate = DateTime.Now,
            //    ModifyDate = DateTime.Now,
            //    Status = "Y",

            //    SouceTable = "Appointment",
            //    ForeignKey = AppointmentId,
            //    LoginBy = user.FirstOrDefault(u => u.Type == "LoginBy").Value,
            //    HashCode = code,
            //    Otp = "",
            //    ExpireTime = DateTime.Now.AddSeconds(int.Parse(_functions.GetSystemParameter("VerificationCodeExpireTime")))
            //};
            //_appointmentService.CreateVerificationcode(vcode);

            ////傳送line驗證碼
            //string VerifyUrl = _functions.GetSystemParameter("SystemDomainName") + "/Appointment/Verify?code=" + code;
            //string message = "預約驗證網址如下，請於5分鐘內進行驗證\n" + VerifyUrl;

            //if (user.FirstOrDefault(u => u.Type == "LoginBy").Value == "Line")
            //    await _functions.SendLineMessageAsync(user.FirstOrDefault(u => u.Type == "LineId").Value, message);
            //else if (user.FirstOrDefault(u => u.Type == "LoginBy").Value == "Cellphone")
            //    await _functions.SendSmsAsync(user.FirstOrDefault(u => u.Type == "Phone").Value, message);

            //_functions.SaveSystemLog(new Systemlog
            //{
            //    CreateDate = DateTime.Now,
            //    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
            //    UserAccount = user.FirstOrDefault(u => u.Type == "UserId").Value,
            //    Description = "Add Appointment id='" + AppointmentId + "'."
            //});

            //return new JsonResult(AppointmentId);
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

        [HttpPost]
        public async Task<IActionResult> sendVerificationCode(string cellphone, string hashCode)
        {
            //確認是否有生效中的驗證碼，如果有就註銷(手機查一次，hashCode查一次)
            _appointmentService.CheckActiveVerificationCode(cellphone, hashCode);

            //產生驗證碼並發送手機簡訊
            Random random = new Random();

            string code = _functions.GetVerificationCode();
            string randomNumber = (random.Next(1, 10) * 100000 + random.Next(0, 100000)).ToString("D6");

            Verificationcode vcode = new Verificationcode()
            {
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                SouceTable = "Customer",
                ForeignKey = cellphone,
                LoginBy = "Cellphone",
                HashCode = code,
                Otp = randomNumber,
                ExpireTime = DateTime.Now.AddSeconds(int.Parse(_functions.GetSystemParameter("VerificationCodeExpireTime")))
            };
            _appointmentService.CreateVerificationcode(vcode);


            string message = "EK美學診所手機登入驗證碼如下，請於5分鐘內進行驗證：" + randomNumber;
            await _functions.SendSmsAsync(cellphone, message);

            return new JsonResult(code);
        }

        [HttpPost]
        public IActionResult checkCellphone(string cellphone)
        {
            var user = HttpContext.User.Claims.ToList();

            return new JsonResult(_appointmentService.checkCellphone(cellphone, user.FirstOrDefault(u => u.Type == "UserId").Value));
        }

        [HttpPost]
        public IActionResult checkNationalIdNumber(string nationalIdNumber)
        {
            var user = HttpContext.User.Claims.ToList();

            return new JsonResult(_appointmentService.checkNationalIdNumber(nationalIdNumber, user.FirstOrDefault(u => u.Type == "UserId").Value));
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
