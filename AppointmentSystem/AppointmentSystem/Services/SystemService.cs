using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.SystemModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Services
{
    public class SystemService
    {
        private readonly EkasContext _db;
        private readonly WebFunctions _functions;

        public SystemService(EkasContext db)
        {
            _db = db;
            _functions = new WebFunctions(db);
        }

        #region -- 系統模組 -- (Module)

        public bool CheckModuleId(string ModuleId)
        {
            if (_db.Modules.Where(x => x.Id == ModuleId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<ModuleIndexVM> GetModuleListForIndex()
        {
            var items = _db.Modules.OrderBy(x => x.Sort);

            List<ModuleIndexVM> modules = new List<ModuleIndexVM>();

            foreach (var item in items)
            {
                modules.Add(new ModuleIndexVM
                {
                    ModuleId = item.Id,
                    ModuleName = item.ModuleName,
                    Sort = item.Sort,
                    Memo = item.Memo,
                    Status = item.Status
                });
            }

            return modules;
        }

        public List<ModuleDropdown> GetModuleListForDropdown()
        {
            var items = _db.Modules.Where(x => x.Status == "Y").OrderBy(x => x.Sort);

            List<ModuleDropdown> modules = new List<ModuleDropdown>();

            foreach (var item in items)
            {
                modules.Add(new ModuleDropdown
                {
                    ModuleId = item.Id,
                    ModuleName = item.ModuleName,
                });
            }

            return modules;
        }

        public ModuleEditVM GetModuleById(string id)
        {
            var item = _db.Modules.Where(x => x.Id == id).FirstOrDefault();

            ModuleEditVM module = new ModuleEditVM();

            module.ModuleName = item.ModuleName;
            module.Sort = item.Sort;
            module.Memo = item.Memo;
            module.Status = item.Status;

            return module;
        }

        public void CreateModule(Module value)
        {
            _db.Modules.Add(value);
            _db.SaveChanges();
        }

        public void UpdateModule(string id, string account, Module value)
        {
            _db.Modules.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Modules.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Modules.FirstOrDefault(x => x.Id == id).ModuleName = value.ModuleName;
            _db.Modules.FirstOrDefault(x => x.Id == id).Sort = value.Sort;
            _db.Modules.FirstOrDefault(x => x.Id == id).Memo = value.Memo;
            _db.Modules.FirstOrDefault(x => x.Id == id).Status = value.Status;

            _db.SaveChanges();
        }

        #endregion

        #region -- 系統功能 -- (Function)

        public bool CheckFunctionId(string FunctionId)
        {
            if (_db.Functions.Where(x => x.Id == FunctionId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<FunctionIndexVM> GetFunctionListForIndex()
        {
            var items = from f in _db.Functions
                        join m in _db.Modules on f.ModuleId equals m.Id
                        orderby m.Sort, f.Sort
                        select new
                        {
                            f.Id,
                            f.FunctionName,
                            m.ModuleName,
                            f.Sort,
                            f.Memo,
                            f.Status
                        };

            List<FunctionIndexVM> functions = new List<FunctionIndexVM>();

            foreach (var item in items)
            {
                functions.Add(new FunctionIndexVM
                {
                    FunctionId = item.Id,
                    ModuleName = item.ModuleName,
                    FunctionName = item.FunctionName,
                    Sort = item.Sort,
                    Memo = item.Memo,
                    Status = item.Status
                });
            }

            return functions;
        }

        public FunctionEditVM GetFunctionById(string id)
        {
            var item = _db.Functions.Where(x => x.Id == id).FirstOrDefault();

            FunctionEditVM function = new FunctionEditVM();

            function.FunctionName = item.FunctionName;
            function.ModuleId = item.ModuleId;
            function.Controller = item.Controller;
            function.Action = item.Action;
            function.Sort = item.Sort;
            function.Memo = item.Memo;
            function.Status = item.Status;

            return function;
        }

        public void CreateFunction(Function value)
        {
            _db.Functions.Add(value);
            _db.SaveChanges();
        }

        public void UpdateFunction(string id, string account, Function value)
        {
            _db.Functions.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Functions.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Functions.FirstOrDefault(x => x.Id == id).FunctionName = value.FunctionName;
            _db.Functions.FirstOrDefault(x => x.Id == id).ModuleId = value.ModuleId;
            _db.Functions.FirstOrDefault(x => x.Id == id).Controller = value.Controller;
            _db.Functions.FirstOrDefault(x => x.Id == id).Action = value.Action;
            _db.Functions.FirstOrDefault(x => x.Id == id).Sort = value.Sort;
            _db.Functions.FirstOrDefault(x => x.Id == id).Memo = value.Memo;
            _db.Functions.FirstOrDefault(x => x.Id == id).Status = value.Status;

            _db.SaveChanges();
        }

        #endregion

        #region -- 職位 -- (Role)

        public bool CheckRoleId(string RoleId)
        {
            if (_db.Roles.Where(x => x.Id == RoleId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<RoleIndexVM> GetRoleListForIndex()
        {
            var items = _db.Roles.OrderBy(x => x.CreateDate);

            List<RoleIndexVM> Roles = new List<RoleIndexVM>();

            foreach (var item in items)
            {
                Roles.Add(new RoleIndexVM
                {
                    RoleId = item.Id,
                    RoleName = item.RoleName,
                    Status = item.Status
                });
            }

            return Roles;
        }

        public List<RoleDropdown> GetRoleListForDropdown()
        {
            var items = _db.Roles.Where(x => x.Status == "Y");

            List<RoleDropdown> Roles = new List<RoleDropdown>();

            foreach (var item in items)
            {
                Roles.Add(new RoleDropdown
                {
                    RoleId = item.Id,
                    RoleName = item.RoleName,
                });
            }

            return Roles;
        }

        public RoleEditVM GetRoleById(string id)
        {
            var item = _db.Roles.Where(x => x.Id == id).FirstOrDefault();

            RoleEditVM Role = new RoleEditVM();

            Role.RoleName = item.RoleName;
            Role.Status = item.Status;

            return Role;
        }

        public void CreateRole(Role value)
        {
            _db.Roles.Add(value);
            _db.SaveChanges();
        }

        public void UpdateRole(string id, string account, Role value)
        {
            _db.Roles.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Roles.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Roles.FirstOrDefault(x => x.Id == id).RoleName = value.RoleName;
            _db.Roles.FirstOrDefault(x => x.Id == id).Status = value.Status;

            _db.SaveChanges();
        }

        #region -- 職位預設權限 -- (RoleDefaultPermission)

        //確認資料是否存在
        public bool CheckRoleDefaultPermission(string RoleId, string FunctionId)
        {
            if (_db.Rolepermissions.Where(x => x.RoleId == RoleId && x.FunctionId == FunctionId).Count() == 0)
                return false;
            else
                return true;
        }

        //取得職位預設權限
        public RoleDefaultPermissionVM GetRoleDefaultPermission(string RoleId, string user, string IsAdmin)
        {
            var Role = _db.Roles.Where(x => x.Id == RoleId).FirstOrDefault();

            var item = from f in _db.Functions.ToList()
                       join m in _db.Modules on f.ModuleId equals m.Id
                       orderby m.Sort, f.Sort
                       where f.Status == "Y"
                       select new
                       {
                           f.Id,
                           f.FunctionName,
                           m.ModuleName,
                           f.Sort,
                           f.Memo,
                           f.Status
                       };

            RoleDefaultPermissionVM permission = new RoleDefaultPermissionVM();

            permission.RoleId = RoleId;
            permission.RoleName = Role.RoleName;

            foreach (var obj in item)
            {
                var d = _db.Rolepermissions.Where(x => x.Status == "Y" && x.RoleId == RoleId && x.FunctionId == obj.Id).FirstOrDefault();
                FunctionPermissionVM data = new FunctionPermissionVM();
                List<SelectListItem> IsAllowList = _functions.CreateSelectList("AllowList", true);

                data.FunctionId = obj.Id;
                data.FunctionName = obj.FunctionName;
                data.IsAllow = "N";
                data.IsAllowDropdown = IsAllowList;

                if (d != null && d.IsAllow == "Y")
                    data.IsAllow = "Y";

                if (IsAdmin == "Y")
                    permission.Functions.Add(data);
                else
                {
                    var permissiondata = _db.Userpermissions.Where(x => x.Status == "Y" && x.UserId == user && x.FunctionId == obj.Id && x.IsAllow == "Y").FirstOrDefault();

                    if (permissiondata != null)
                        permission.Functions.Add(data);
                }

            }

            return permission;
        }

        //新增職位預設權限資料表
        public void CreateRoleDefaultPermission(string RoleId, string FunctionId, string IsAllow, string user)
        {
            //long max;

            //if (_db.Rolepermissions.Count() == 0)
            //    max = 0;
            //else
            //    max = _db.Rolepermissions.Max(x => x.Index);

            Rolepermission item = new Rolepermission()
            {
                Creator = user,
                Modifier = user,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                //Index = max + 1,
                RoleId = RoleId,
                FunctionId = FunctionId,
                IsAllow = IsAllow
            };

            _db.Rolepermissions.Add(item);
            _db.SaveChanges();
        }

        //更新職位預設權限資料表
        public void UpdateRoleDefaultPermission(string RoleId, string FunctionId, string IsAllow, string user)
        {
            _db.Rolepermissions.FirstOrDefault(x => x.RoleId == RoleId && x.FunctionId == FunctionId).Modifier = user;
            _db.Rolepermissions.FirstOrDefault(x => x.RoleId == RoleId && x.FunctionId == FunctionId).ModifyDate = DateTime.Now;

            _db.Rolepermissions.FirstOrDefault(x => x.RoleId == RoleId && x.FunctionId == FunctionId).IsAllow = IsAllow;

            _db.SaveChanges();
        }

        #endregion

        #endregion

        #region -- 使用者 -- (User)

        public bool CheckUserId(string UserId)
        {
            if (_db.Users.Where(x => x.Id == UserId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<UserIndexVM> GetUserListForIndex()
        {
            var items = _db.Users.OrderBy(x => x.CreateDate).ToList();

            List<UserIndexVM> users = new List<UserIndexVM>();

            foreach (var item in items)
            {
                string account = "";
                var data = _db.Accounts.FirstOrDefault(x => x.UserId == item.Id);

                if (data != null)
                    account = data.Account1!;

                users.Add(new UserIndexVM
                {
                    UserId = item.Id,
                    UserName = item.UserName,
                    UserAccount = account,
                    UserEmail = item.UserEmail,
                    Status = item.Status
                });
            }

            return users;
        }

        public UserEditVM GetUserById(string id)
        {
            var item = _db.Users.Where(x => x.Id == id).FirstOrDefault();

            UserEditVM Role = new UserEditVM();

            Role.UserName = item.UserName;
            Role.UserNameEnglish = item.UserNameEnglish;
            Role.Gender = item.Gender;
            Role.Birthday = item.Birthday;
            Role.Address = item.Address;
            Role.UserEmail = item.UserEmail;
            Role.Telphone = item.Telphone;
            Role.RoleId = item.RoleId;
            Role.IsAdmin = item.IsAdmin;
            Role.Status = item.Status;

            return Role;
        }

        public void CreateUser(User value)
        {
            _db.Users.Add(value);
            _db.SaveChanges();
        }

        public void UpdateUser(string id, string account, User value)
        {
            _db.Users.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Users.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Users.FirstOrDefault(x => x.Id == id).UserName = value.UserName;
            _db.Users.FirstOrDefault(x => x.Id == id).UserNameEnglish = value.UserNameEnglish;
            _db.Users.FirstOrDefault(x => x.Id == id).Gender = value.Gender;
            _db.Users.FirstOrDefault(x => x.Id == id).Birthday = value.Birthday;
            _db.Users.FirstOrDefault(x => x.Id == id).Address = value.Address;
            _db.Users.FirstOrDefault(x => x.Id == id).UserEmail = value.UserEmail;
            _db.Users.FirstOrDefault(x => x.Id == id).Telphone = value.Telphone;
            _db.Users.FirstOrDefault(x => x.Id == id).RoleId = value.RoleId;
            _db.Users.FirstOrDefault(x => x.Id == id).IsAdmin = value.IsAdmin;
            _db.Users.FirstOrDefault(x => x.Id == id).Status = value.Status;

            _db.SaveChanges();
        }

        #region -- 使用者預設權限 -- (UserDefaultPermission)

        public bool CheckUserDefaultPermission(string UserId, string FunctionId)
        {
            if (_db.Userpermissions.Where(x => x.UserId == UserId && x.FunctionId == FunctionId).Count() == 0)
                return false;
            else
                return true;
        }

        public UserDefaultPermissionVM GetUserDefaultPermission(string UserId, string user, string IsAdmin)
        {
            var u = _db.Users.Where(x => x.Id == UserId).FirstOrDefault();
            var item = from f in _db.Functions.ToList()
                       join m in _db.Modules on f.ModuleId equals m.Id
                       orderby m.Sort, f.Sort
                       where f.Status == "Y"
                       select new
                       {
                           f.Id,
                           f.FunctionName,
                           m.ModuleName,
                           f.Sort,
                           f.Memo,
                           f.Status
                       };

            UserDefaultPermissionVM permission = new UserDefaultPermissionVM();

            permission.UserId = UserId;
            permission.UserName = u.UserName;

            foreach (var obj in item)
            {
                var d = _db.Userpermissions.Where(x => x.Status == "Y" && x.UserId == UserId && x.FunctionId == obj.Id).FirstOrDefault();
                FunctionPermissionVM data = new FunctionPermissionVM();
                List<SelectListItem> IsAllowList = _functions.CreateSelectList("AllowList", true);

                data.FunctionId = obj.Id;
                data.FunctionName = obj.FunctionName;
                data.IsAllow = "N";
                data.IsAllowDropdown = IsAllowList;

                if (d != null && d.IsAllow == "Y")
                    data.IsAllow = "Y";

                if (IsAdmin == "Y")
                    permission.Functions.Add(data);
                else
                {
                    var permissiondata = _db.Userpermissions.Where(x => x.Status == "Y" && x.UserId == user && x.FunctionId == obj.Id && x.IsAllow == "Y").FirstOrDefault();

                    if (permissiondata != null)
                        permission.Functions.Add(data);
                }

            }

            return permission;
        }

        public void CreateUserDefaultPermission(string UserId, string FunctionId, string IsAllow, string user)
        {
            //long max;

            //if (_db.Userpermissions.Count() == 0)
            //    max = 0;
            //else
            //    max = _db.Userpermissions.Max(x => x.Index);

            Userpermission item = new Userpermission()
            {
                Creator = user,
                Modifier = user,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                //Index = max + 1,
                UserId = UserId,
                FunctionId = FunctionId,
                IsAllow = IsAllow
            };

            _db.Userpermissions.Add(item);
            _db.SaveChanges();
        }

        public void UpdateUserDefaultPermission(string UserId, string FunctionId, string IsAllow, string user)
        {
            _db.Userpermissions.FirstOrDefault(x => x.UserId == UserId && x.FunctionId == FunctionId).Modifier = user;
            _db.Userpermissions.FirstOrDefault(x => x.UserId == UserId && x.FunctionId == FunctionId).ModifyDate = DateTime.Now;

            _db.Userpermissions.FirstOrDefault(x => x.UserId == UserId && x.FunctionId == FunctionId).IsAllow = IsAllow;

            _db.SaveChanges();
        }

        public void CopyRolePermissionToUser(string UserId, string RoleId, string user)
        {
            var defaultpermission = _db.Rolepermissions.Where(x => x.Status == "Y" && x.RoleId == RoleId);
            //int i = 0;

            foreach (var item in defaultpermission)
            {
                //i++;
                //long max;

                //if (_db.Userpermissions.Count() == 0)
                //    max = 0;
                //else
                //    max = _db.Userpermissions.Max(x => x.Index);

                Userpermission permission = new Userpermission()
                {
                    Creator = user,
                    Modifier = user,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    //Index = max + i,
                    UserId = UserId,
                    FunctionId = item.FunctionId,
                    IsAllow = item.IsAllow
                };

                _db.Userpermissions.Add(permission);
            }

            _db.SaveChanges();
        }

        #endregion

        #region -- 使用者帳號 -- (Account)

        //建立Account資料時確認ID是否重複
        public bool CheckAccountId(string AccountId)
        {
            if (_db.Accounts.Where(x => x.Id == AccountId).Count() == 0)
                return false;
            else
                return true;
        }

        //用以確認帳號是否重複(前端AJX呼叫)，如重複前端會有提示訊息
        public bool CheckAccountExist(string? account)
        {
            if (_db.Accounts.Where(x => x.Account1 == account).Count() == 0)
                return false;
            else
                return true;
        }

        //用以確認使用者是否已建立過資料
        public bool CheckAccountByUserId(string? userId)
        {
            if (_db.Accounts.Where(x => x.UserId == userId).Count() == 0)
                return false;
            else
                return true;
        }

        public UserAccountVM GetUserAccountByUserId(string id)
        {
            var item = _db.Accounts.Where(x => x.UserId == id).FirstOrDefault();

            if (item == null)
                return null;
            else
            {
                return new UserAccountVM()
                {
                    Account = item.Account1,
                    Memo = item.Memo
                };
            }
        }

        public void CreateUserAccount(Account value)
        {
            _db.Accounts.Add(value);
            _db.SaveChanges();
        }

        public void UpdateUserAccount(string userId, string account, Account value)
        {
            _db.Accounts.FirstOrDefault(x => x.UserId == userId).Modifier = account;
            _db.Accounts.FirstOrDefault(x => x.UserId == userId).ModifyDate = DateTime.Now;

            _db.Accounts.FirstOrDefault(x => x.UserId == userId).Account1 = value.Account1;
            _db.Accounts.FirstOrDefault(x => x.UserId == userId).Password = value.Password;
            _db.Accounts.FirstOrDefault(x => x.UserId == userId).Memo = value.Memo;
            _db.Accounts.FirstOrDefault(x => x.UserId == userId).Status = value.Status;

            _db.SaveChanges();
        }

        #endregion

        #endregion

        #region -- 標籤管理 -- (Label)

        public bool CheckLabelId(string LabelId)
        {
            if (_db.Labels.Where(x => x.Id == LabelId).Count() == 0)
                return false;
            else
                return true;
        }

        public int GetLabelCount()
        {
            return _db.Labels.Count();
        }

        public List<LabelIndexVM> GetLabelListForIndex()
        {
            var items = _db.Labels.OrderBy(x => x.Sort);

            List<LabelIndexVM> Labels = new List<LabelIndexVM>();

            foreach (var item in items)
            {
                Labels.Add(new LabelIndexVM
                {
                    LabelId = item.Id,
                    Type = item.Type,
                    LabelName = item.LabelName,
                    Sort = item.Sort,
                    Status = item.Status
                });
            }

            return Labels;
        }

        public LabelEditVM GetLabelById(string id)
        {
            var item = _db.Labels.Where(x => x.Id == id).FirstOrDefault();

            LabelEditVM Label = new LabelEditVM();

            Label.Type = item.Type;
            Label.LabelName = item.LabelName;
            Label.Sort = item.Sort;
            Label.Status = item.Status;

            return Label;
        }

        public void CreateLabel(Label value)
        {
            _db.Labels.Add(value);
            _db.SaveChanges();
        }

        public void UpdateLabel(string id, string account, Label value)
        {
            _db.Labels.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Labels.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Labels.FirstOrDefault(x => x.Id == id).Type = value.Type;
            _db.Labels.FirstOrDefault(x => x.Id == id).LabelName = value.LabelName;
            _db.Labels.FirstOrDefault(x => x.Id == id).Sort = value.Sort;
            _db.Labels.FirstOrDefault(x => x.Id == id).Status = value.Status;

            _db.SaveChanges();
        }

        public string DeleteLabel(string id)
        {
            //先刪除有使用到此標籤的療程單身資料
            _db.Treatmentlabels.RemoveRange(_db.Treatmentlabels.Where(x => x.LabelId == id));
            //此刪除此標籤
            _db.Labels.RemoveRange(_db.Labels.Where(x => x.Id == id));
            _db.SaveChanges();

            return "success";
        }

        #endregion

        #region -- 系統參數 -- (SystemParameter)

        public string GetParameterImageIdByIndex(long? index)
        {
            var item = _db.Systemparameters.FirstOrDefault(x => x.Index == index);

            if (item != null)
                return item.Value;
            else
                return "";
        }

        public List<ParameterIndexVM> GetParameterListForIndex()
        {
            var items = _db.Systemparameters.OrderBy(x => x.Name);

            List<ParameterIndexVM> parameters = new List<ParameterIndexVM>();

            foreach (var item in items)
            {
                parameters.Add(new ParameterIndexVM
                {
                    Index = item.Index,
                    ParameterName = item.Name,
                    Memo = item.Memo
                });
            }

            return parameters;
        }

        public ParameterEditVM GetParameterByIndex(long index)
        {
            var item = _db.Systemparameters.Where(x => x.Index == index).FirstOrDefault();

            if (item != null)
            {
                if (item.Type == "Text" || item.Type == "Time(minutes)" || item.Type == "Time(seconds)")
                {

                    ParameterEditVM parameter = new ParameterEditVM()
                    {
                        ParameterName = item.Name,
                        Memo = item.Memo,
                        Type = item.Type,
                        Value = item.Value,
                        Locked = item.Locked
                    };

                    return parameter;
                }
                else if (item.Type == "Image")
                {
                    var file = _db.Systemfiles.FirstOrDefault(x => x.Id == item.Value);
                    FileData fd = new FileData();

                    if (file != null)
                    {
                        fd = new FileData()
                        {
                            FileID = file.Id,
                            FileName = file.FileName,
                            FileExtension = file.FileExtension,
                            FileSize = file.FileSize,
                            Path = file.Path
                        };
                    }

                    ParameterEditVM parameter = new ParameterEditVM()
                    {
                        ParameterName = item.Name,
                        Memo = item.Memo,
                        Type = item.Type,
                        Locked = item.Locked,
                        ImageFile = fd
                    };

                    return parameter;
                }
            }

            return new ParameterEditVM();
        }

        public void UpdateParameter(long index, string account, Systemparameter value)
        {
            _db.Systemparameters.FirstOrDefault(x => x.Index == index).Modifier = account;
            _db.Systemparameters.FirstOrDefault(x => x.Index == index).ModifyDate = DateTime.Now;

            if (value.Memo != null)
                _db.Systemparameters.FirstOrDefault(x => x.Index == index).Memo = value.Memo;
            if (value.Value != null)
                _db.Systemparameters.FirstOrDefault(x => x.Index == index).Value = value.Value;

            _db.SaveChanges();
        }

        #endregion

    }
}
