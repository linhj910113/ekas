using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.SystemModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Controllers
{
    public class SystemController : Controller
    {
        private readonly SystemService _systemService;
        private readonly WebFunctions _functions;

        public SystemController(EkasContext context)
        {
            _systemService = new SystemService(context);
            _functions = new WebFunctions(context);
        }

        #region -- 系統模組 -- (Module)

        [Authorize]
        public IActionResult ModuleIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<ModuleIndexVM> modules = _systemService.GetModuleListForIndex();
                ViewBag.Count = modules.Count;

                return View(modules);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult ModuleCreate()
        {
            //判斷cookie是員工還是顧客
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

        [HttpPost]
        [Authorize]
        public IActionResult ModuleCreate(ModuleCreateVM value)
        {
            string ModuleId = _functions.GetGuid();

            while (_systemService.CheckModuleId(ModuleId))
                ModuleId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            Module item = new Module()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = ModuleId,
                ModuleName = value.ModuleName,
                Sort = value.Sort,
                Memo = value.Memo,
            };
            _systemService.CreateModule(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add module id='" + ModuleId + "'."
            });

            return RedirectToAction("ModuleIndex");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ModuleEdit(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> StatusList = _functions.CreateSelectList("StatusList", false);
                ModuleEditVM item = _systemService.GetModuleById(id);

                ModuleEditVM model = new ModuleEditVM() //上面的 Model
                {
                    StatusList = StatusList,
                    Status = item.Status,
                    ModuleName = item.ModuleName,
                    Sort = item.Sort,
                    Memo = item.Memo,
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult ModuleEdit(string id, ModuleEditVM value)
        {
            string ModuleId = id;

            var user = HttpContext.User.Claims.ToList();

            Module item = new Module()
            {
                ModifyDate = DateTime.Now,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,

                Id = ModuleId,
                ModuleName = value.ModuleName,
                Memo = value.Memo,
                Status = value.Status!,
                Sort = value.Sort
            };

            _systemService.UpdateModule(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update module id='" + ModuleId + "'."
            });

            return RedirectToAction("ModuleIndex");
        }

        #endregion

        #region -- 系統功能 -- (Function)

        [Authorize]
        public IActionResult FunctionIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<FunctionIndexVM> functions = _systemService.GetFunctionListForIndex();
                ViewBag.Count = functions.Count;

                return View(functions);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult FunctionCreate()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> ModuleList = CreateModuleList();

                FunctionCreateVM model = new FunctionCreateVM() //上面的 Model
                {
                    ModuleList = ModuleList
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult FunctionCreate(FunctionCreateVM value)
        {
            string FunctionId = _functions.GetGuid();

            while (_systemService.CheckFunctionId(FunctionId))
                FunctionId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            Function item = new Function()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = FunctionId,
                FunctionName = value.FunctionName,
                ModuleId = value.ModuleId,
                Controller = value.Controller,
                Action = value.Action,
                Sort = value.Sort,
                Memo = value.Memo,
            };
            _systemService.CreateFunction(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add function id='" + FunctionId + "'."
            });

            return RedirectToAction("FunctionIndex");
        }

        [HttpGet]
        [Authorize]
        public IActionResult FunctionEdit(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> ModuleList = CreateModuleList();
                List<SelectListItem> StatusList = _functions.CreateSelectList("StatusList", false);
                FunctionEditVM item = _systemService.GetFunctionById(id);

                FunctionEditVM model = new FunctionEditVM() //上面的 Model
                {
                    ModuleList = ModuleList,
                    StatusList = StatusList,
                    Status = item.Status,

                    FunctionName = item.FunctionName,
                    ModuleId = item.ModuleId,
                    Controller = item.Controller,
                    Action = item.Action,
                    Sort = item.Sort,
                    Memo = item.Memo,
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult FunctionEdit(string id, FunctionEditVM value)
        {
            string FunctionId = id;

            var user = HttpContext.User.Claims.ToList();

            Function item = new Function()
            {
                Id = FunctionId,
                FunctionName = value.FunctionName,
                ModuleId = value.ModuleId,
                Controller = value.Controller,
                Action = value.Action,
                Sort = value.Sort,
                Memo = value.Memo,
                Status = value.Status!
            };

            _systemService.UpdateFunction(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update function id='" + FunctionId + "'."
            });

            return RedirectToAction("FunctionIndex");
        }

        #endregion

        #region -- 職位 -- (Role)

        [Authorize]
        public IActionResult RoleIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<RoleIndexVM> Roles = _systemService.GetRoleListForIndex();
                ViewBag.Count = Roles.Count;

                return View(Roles);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult RoleCreate()
        {
            //判斷cookie是員工還是顧客
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

        [HttpPost]
        [Authorize]
        public IActionResult RoleCreate(RoleCreateVM value)
        {
            string RoleId = _functions.GetGuid();

            while (_systemService.CheckRoleId(RoleId))
                RoleId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            Role item = new Role()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = RoleId,
                RoleName = value.RoleName
            };
            _systemService.CreateRole(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add Role id='" + RoleId + "'."
            });

            return RedirectToAction("RoleIndex");
        }

        [HttpGet]
        [Authorize]
        public IActionResult RoleEdit(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> StatusList = _functions.CreateSelectList("StatusList", false);
                RoleEditVM item = _systemService.GetRoleById(id);

                RoleEditVM model = new RoleEditVM() //上面的 Model
                {
                    StatusList = StatusList,
                    Status = item.Status,
                    RoleName = item.RoleName
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult RoleEdit(string id, RoleEditVM value)
        {
            string RoleId = id;

            var user = HttpContext.User.Claims.ToList();

            Role item = new Role()
            {
                Id = RoleId,
                RoleName = value.RoleName,
                Status = value.Status!,
            };

            _systemService.UpdateRole(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update Role id='" + RoleId + "'."
            });

            return RedirectToAction("RoleIndex");
        }

        #region -- 職位預設權限 -- (RoleDefaultPermission)

        [HttpGet]
        [Authorize]
        public IActionResult RoleDefaultPermission(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                RoleDefaultPermissionVM item = _systemService.GetRoleDefaultPermission(id, user.FirstOrDefault(u => u.Type == "UserId").Value, user.FirstOrDefault(u => u.Type == "IsAdmin").Value);

                return View(item);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult SaveRoleDefaultPermission(string RoleId, string FunctionId, string IsAllow)
        {
            var item = HttpContext.User.Claims.ToList();
            string user = item.FirstOrDefault(u => u.Type == "Account").Value;

            //儲存至角色預設權限
            if (_systemService.CheckRoleDefaultPermission(RoleId, FunctionId))
                _systemService.UpdateRoleDefaultPermission(RoleId, FunctionId, IsAllow, user);
            else
            {
                _systemService.CreateRoleDefaultPermission(RoleId, FunctionId, IsAllow, user);

                //如為新功能或新身分，則為所有該身分的使用者新增權限
                //_systemService.SaveUserPermission_WithNewMenuItem(RoleId, FunctionId, IsAllow, user);
            }


            return new JsonResult("finish");
        }

        #endregion

        #endregion

        #region -- 使用者 -- (User)

        [Authorize]
        public IActionResult UserIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<UserIndexVM> users = _systemService.GetUserListForIndex();
                ViewBag.Count = users.Count;

                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult UserCreate()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> GenderList = _functions.CreateSelectList("Gender", false);
                List<SelectListItem> RoleList = CreateRoleList();

                UserCreateVM model = new UserCreateVM() //上面的 Model
                {
                    GenderList = GenderList,
                    RoleList = RoleList
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult UserCreate(UserCreateVM value)
        {
            string UserId = _functions.GetGuid();

            while (_systemService.CheckUserId(UserId))
                UserId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            User item = new User()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = UserId,
                UserName = value.UserName,
                UserNameEnglish = value.UserNameEnglish,
                Gender = value.Gender,
                Birthday = value.Birthday,
                Address = value.Address,
                UserEmail = value.UserEmail,
                Telphone = value.Telphone,
                RoleId = value.RoleId,
                IsAdmin = value.IsAdmin!
            };
            _systemService.CreateUser(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add user id='" + UserId + "'."
            });

            //依身分新增用戶預設權限
            _systemService.CopyRolePermissionToUser(UserId, value.RoleId, user.FirstOrDefault(u => u.Type == "Account").Value);

            return RedirectToAction("UserIndex");
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserEdit(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> RoleList = CreateRoleList();
                List<SelectListItem> GenderList = _functions.CreateSelectList("Gender", false);
                List<SelectListItem> StatusList = _functions.CreateSelectList("StatusList", false);

                UserEditVM item = _systemService.GetUserById(id);

                UserEditVM model = new UserEditVM() //上面的 Model
                {
                    GenderList = GenderList,
                    RoleList = RoleList,
                    StatusList = StatusList,

                    UserName = item.UserName,
                    UserNameEnglish = item.UserNameEnglish,
                    Gender = item.Gender,
                    Birthday = item.Birthday,
                    Address = item.Address,
                    UserEmail = item.UserEmail,
                    Telphone = item.Telphone,
                    RoleId = item.RoleId,
                    IsAdmin = item.IsAdmin,
                    Status = item.Status
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult UserEdit(string id, UserEditVM value)
        {
            string UserId = id;

            var user = HttpContext.User.Claims.ToList();
            UserEditVM userInfo = _systemService.GetUserById(id);
            User item;

            item = new User()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,
                Status = value.Status!,

                Id = UserId,
                UserName = value.UserName,
                UserNameEnglish = value.UserNameEnglish,
                Gender = value.Gender,
                Birthday = value.Birthday,
                Address = value.Address,
                UserEmail = value.UserEmail,
                Telphone = value.Telphone,
                RoleId = value.RoleId,
                IsAdmin = value.IsAdmin!
            };

            _systemService.UpdateUser(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update user id='" + UserId + "'."
            });

            return RedirectToAction("UserIndex");
        }

        #region -- 使用者預設權限 -- (UserDefaultPermission)

        [HttpGet]
        [Authorize]
        public IActionResult UserDefaultPermission(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                UserDefaultPermissionVM item = _systemService.GetUserDefaultPermission(id, user.FirstOrDefault(u => u.Type == "UserId").Value, user.FirstOrDefault(u => u.Type == "IsAdmin").Value);

                return View(item);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult SaveUserDefaultPermission(string UserId, string FunctionId, string IsAllow)
        {
            var item = HttpContext.User.Claims.ToList();
            string user = item.FirstOrDefault(u => u.Type == "Account").Value;

            //儲存至角色預設權限
            if (_systemService.CheckUserDefaultPermission(UserId, FunctionId))
                _systemService.UpdateUserDefaultPermission(UserId, FunctionId, IsAllow, user);
            else
            {
                _systemService.CreateUserDefaultPermission(UserId, FunctionId, IsAllow, user);

                //如為新功能或新身分，則為所有該身分的使用者新增權限
                //_systemService.SaveUserPermission_WithNewMenuItem(RoleId, FunctionId, IsAllow, user);
            }

            return new JsonResult("finish");
        }

        #endregion

        #region -- 使用者帳號 -- (Account)

        [HttpGet]
        [Authorize]
        public IActionResult UserAccountEdit(string id)
        {
            UserAccountVM item = _systemService.GetUserAccountByUserId(id);

            return View(item);
        }

        [HttpPost]
        [Authorize]
        public IActionResult UserAccountEdit(string id, UserAccountVM value)
        {
            //用userId找查詢資料庫是否有資料
            //如有則已建立過帳號資料，使用update的方式更新資料
            //如沒有則尚未建立帳號資料，使用create的方式建立資料

            if (_systemService.CheckAccountByUserId(id))
            {
                var user = HttpContext.User.Claims.ToList();

                _systemService.UpdateUserAccount(id, user.FirstOrDefault(u => u.Type == "Account").Value, new Account()
                {
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    ModifyDate = DateTime.Now,

                    Account1 = value.Account!,
                    Password = _functions.SHA256Hash(value.Password),
                    Memo = value.Memo,
                    Status = "Y"
                });

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Description = "Create user account='" + id + "'."
                });
            }
            else
            {
                var user = HttpContext.User.Claims.ToList();
                string AccountId = _functions.GetGuid();

                while (_systemService.CheckAccountId(AccountId))
                    AccountId = _functions.GetGuid();

                _systemService.CreateUserAccount(new Account()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,

                    Id = AccountId,
                    UserId = id,
                    Account1 = value.Account!,
                    Password = _functions.SHA256Hash(value.Password),
                    Memo = value.Memo,
                    Status = "Y"
                });

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Description = "Create user account='" + id + "'."
                });
            }

            return RedirectToAction("UserIndex");
        }

        //TODO:確認帳號是否存在(AJAX)

        #endregion

        #endregion

        #region -- 標籤管理 -- (Label)

        [Authorize]
        public IActionResult LabelIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<LabelIndexVM> modules = _systemService.GetLabelListForIndex();
                ViewBag.Count = modules.Count;

                return View(modules);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult LabelCreate()
        {
            //判斷cookie是員工還是顧客
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

        [HttpPost]
        [Authorize]
        public IActionResult LabelCreate(LabelCreateVM value)
        {
            string LabelId = _functions.GetGuid();

            while (_systemService.CheckLabelId(LabelId))
                LabelId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            Label item = new Label()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = LabelId,
                Type = "Treatment",
                LabelName = value.LabelName,
                Sort = _systemService.GetLabelCount() + 1,
            };
            _systemService.CreateLabel(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add Label id='" + LabelId + "'."
            });

            return RedirectToAction("LabelIndex");
        }

        [HttpGet]
        [Authorize]
        public IActionResult LabelEdit(string id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> StatusList = _functions.CreateSelectList("StatusList", false);
                LabelEditVM item = _systemService.GetLabelById(id);

                LabelEditVM model = new LabelEditVM() //上面的 Model
                {
                    StatusList = StatusList,
                    Status = item.Status,
                    Type = item.Type,
                    LabelName = item.LabelName,
                    Sort = item.Sort,
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult LabelEdit(string id, LabelEditVM value)
        {
            string LabelId = id;

            var user = HttpContext.User.Claims.ToList();

            Label item = new Label()
            {
                ModifyDate = DateTime.Now,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,

                Id = LabelId,
                Type = "Treatment",
                LabelName = value.LabelName,
                Status = value.Status!,
                Sort = value.Sort
            };

            _systemService.UpdateLabel(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update label id='" + LabelId + "'."
            });

            return RedirectToAction("LabelIndex");
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteLabel(string Id)
        {
            string result = _systemService.DeleteLabel(Id);

            return new JsonResult(result);
        }

        #endregion

        #region -- 系統參數 -- (SystemParameter)

        [Authorize]
        public IActionResult ParameterIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<ParameterIndexVM> parameters = _systemService.GetParameterListForIndex();
                ViewBag.Count = parameters.Count;

                return View(parameters);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ParameterEdit(long id)
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                ParameterEditVM model = _systemService.GetParameterByIndex(id);

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult ParameterEdit(long id, [FromForm] ParameterEditVM data)
        {
            var user = HttpContext.User.Claims.ToList();
            Systemparameter item;

            if (data.Type == "Text" || data.Type == "Time(minutes)" || data.Type == "Time(seconds)")
            {
                item = new Systemparameter()
                {
                    ModifyDate = DateTime.Now,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,

                    Memo = data.Memo,
                    Value = data.Value,
                };

                _systemService.UpdateParameter(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);
            }
            else if (data.Type == "Image")
            {
                string FileId = "";

                if (data.ImageFile.File != null)
                    FileId = _functions.SaveFile(data.ImageFile.File, user.FirstOrDefault(u => u.Type == "Account").Value);
                else
                    FileId = _systemService.GetParameterImageIdByIndex(id);


                item = new Systemparameter()
                {
                    ModifyDate = DateTime.Now,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,

                    Memo = data.Memo,
                    Value = FileId,
                };

                _systemService.UpdateParameter(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update system parameter index='" + id + "'."
            });

            return RedirectToAction("ParameterIndex");
        }

        #endregion




        [Authorize]
        private List<SelectListItem> CreateModuleList()
        {
            List<ModuleDropdown> ModuleList = _systemService.GetModuleListForDropdown();
            List<SelectListItem> SelectItemList = new List<SelectListItem>();

            SelectItemList.Add(new SelectListItem()
            {
                Text = "請選擇...",
                Value = "",
                Selected = true
            });

            foreach (var item in ModuleList)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = item.ModuleName,
                    Value = item.ModuleId,
                    Selected = false
                });
            }

            return SelectItemList;
        }

        [Authorize]
        private List<SelectListItem> CreateRoleList()
        {
            List<RoleDropdown> JobList = _systemService.GetRoleListForDropdown();
            List<SelectListItem> SelectItemList = new List<SelectListItem>();

            SelectItemList.Add(new SelectListItem()
            {
                Text = "請選擇...",
                Value = "",
                Selected = true
            });

            foreach (var item in JobList)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = item.RoleName,
                    Value = item.RoleId,
                    Selected = false
                });
            }

            return SelectItemList;
        }


    }
}
