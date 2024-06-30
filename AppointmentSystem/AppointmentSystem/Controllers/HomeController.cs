using AppointmentSystem.Models;
using AppointmentSystem.Models.DBModels;
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

        [Authorize]
        public IActionResult Index()
        {
            //�P�_cookie�O���u�٬O�U��
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                return View();                
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }

            
        }

        public IActionResult Login()
        {
            //���n�J�N����쭺��
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

                    ViewBag.errMsg = "�ϥΪ̦W�٩αK�X���~�I";
                    return View();
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Home");//�ɦܵn�J��
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
