using AppointmentSystem.Models;
using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Models.ViewModels.HomeModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebFunctions _functions;
        private readonly HomeService _homeService;

        public HomeController(ILogger<HomeController> logger, EkasContext context)
        {
            _logger = logger;
            _functions = new WebFunctions(context);
            _homeService = new HomeService(context);
        }

        public IActionResult Login()
        {
            //有登入就跳轉到首頁
            var user = HttpContext.User.Claims.ToList();

            if (user.Count == 0)
                return View();
            else
                return RedirectToAction("index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM value)
        {
            if (ModelState.IsValid)
            {
                if (_homeService.UserLogin(value.Account, _functions.SHA256Hash(value.Password)))
                {
                    var user = _homeService.GetUserDate(value.Account);

                    var claims = new List<Claim>
                    {
                        new Claim("LoginType", "EkUser"),
                        new Claim("UserId", user.Id),
                        new Claim("Account", value.Account),
                        new Claim("Name", user.UserName),
                        new Claim("IsAdmin", user.IsAdmin)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = value.Account,
                        UserAccount = value.Account,
                        Description = "User '" + value.Account + "' login."
                    });

                    //return View();
                    return RedirectToAction("index", "Home");
                }
                else
                {
                    _functions.SaveSystemLog(new Systemlog()
                    {
                        CreateDate = DateTime.Now,
                        Creator = value.Account,
                        //Index = _functions.GetMaxLogIndex() + 1,
                        Description = "User '" + value.Account + "' login failed."
                    });

                    ViewBag.errMsg = "使用者名稱或密碼錯誤！";
                    return View();
                }
            }

            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                Models.ViewModels.HomeModels.IndexVM indexVM = _homeService.GetIndexVMData();

                return View(indexVM);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult AppointmentEdit(string id)
        {
            var user = HttpContext.User.Claims.ToList();

            EditVM AppointmentData = _homeService.GetAppointmentDataById(id);

            return View(AppointmentData);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Home");//導至登入頁
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        public IActionResult setHeaderUserName()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();
            UsernameAndRoleNameVM result = new UsernameAndRoleNameVM();

            result.Username = user.FirstOrDefault(u => u.Type == "Name").Value;
            result.Rolename = _homeService.GetUserRoleName(user.FirstOrDefault(u => u.Type == "UserId").Value);

            return new JsonResult(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SetAppointmentTable(string currentYear, string currentMonth, string currentDay)
        {
            IndexAppointmentVM indexVM = _homeService.GetIndexAppointmentVMData(currentYear, currentMonth, currentDay);

            return new JsonResult(indexVM);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SetAppointmentData(string currentYear, string currentMonth, string currentDay)
        {
            List<AppointmentData> result = _homeService.GetAppointmentData(currentYear, currentMonth, currentDay);

            return new JsonResult(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SetAppointmentDetail(string appointmentId)
        {
            AppointmentData result = _homeService.GetAppointmentDetail(appointmentId);

            return new JsonResult(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult appointmentCheckIn(string appointmentId)
        {
            var user = HttpContext.User.Claims.ToList();

            Appointment item = new Appointment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,
                CheckIn = "Y",
                CheckInTime = DateTime.Now.ToString()
            };

            _homeService.AppointmentCheckIn(appointmentId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Appointment check in success id='" + appointmentId + "'."
            });

            return new JsonResult("success.");
        }

        [HttpPost]
        [Authorize]
        public IActionResult appointmentCancel(string appointmentId)
        {
            var user = HttpContext.User.Claims.ToList();

            Appointment item = new Appointment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,
                Status = "C"
            };

            _homeService.AppointmentCancel(appointmentId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Appointment check in success id='" + appointmentId + "'."
            });

            return new JsonResult("success.");
        }

        [HttpPost]
        [Authorize]
        public IActionResult saveSelectedTreatment(string appointmentId, string[] treatments)
        {
            var user = HttpContext.User.Claims.ToList();

            //先刪除再新增
            _homeService.RemoveActualAppointmentTreatment(appointmentId);
            foreach (var treatment in treatments)
            {
                Appointmenttreatment at = new Appointmenttreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "UserId").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    AppointmentId = appointmentId,
                    Type = "T",
                    TreatmentId = treatment
                };
                _homeService.CreateAppointmenttreatment(at);
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Appointment check in success id='" + appointmentId + "'."
            });

            return new JsonResult("success.");
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateAppointment(string AppointmentId, string[] treatments, string doctor, string date, string beginTime)
        {
            var user = HttpContext.User.Claims.ToList();

            string BookingEndTime = TimeSpan.Parse(beginTime).Add(new TimeSpan(0, _homeService.GetTreatmentListMaxTime(treatments), 0)).ToString();
            BookingEndTime = DateTime.ParseExact(BookingEndTime, "HH:mm:ss", null).ToString("HH:mm");

            Appointment item = new Appointment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,

                DoctorId = doctor,
                Date = date,
                BookingBeginTime = beginTime,
                BookingEndTime = BookingEndTime
            };
            _homeService.UpdateAppointment(item, AppointmentId);

            //刪除原本的療程後新增
            _homeService.RemoveAppointmenttreatment(AppointmentId);

            foreach (var treatment in treatments)
            {
                Appointmenttreatment at = new Appointmenttreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    AppointmentId = AppointmentId,
                    Type = "A",
                    TreatmentId = treatment
                };
                _homeService.CreateAppointmenttreatment(at);
            }

            //更新門診時刻表
            _homeService.UpdateAppointmentToOutpatient(AppointmentId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add Appointment id='" + AppointmentId + "'."
            });

            return new JsonResult(AppointmentId);
        }

        [HttpPost]
        [Authorize]
        public IActionResult editCustomerInfo(string AppointmentId, string customerName, string customerCellPhone, string customerBirth, string customerEmail)
        {
            string result = _homeService.EditCustomerInfo(AppointmentId, customerName, customerCellPhone, customerBirth, customerEmail);

            return new JsonResult(result);
        }

    }
}
