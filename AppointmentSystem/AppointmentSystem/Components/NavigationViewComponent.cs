using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AppointmentSystem.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly WebFunctions _functions;

        public NavigationViewComponent(EkasContext context)
        {
            _functions = new WebFunctions(context);
        }

        [Authorize]
        public IViewComponentResult Invoke()
        {
            List<ModuleVM> modules = new List<ModuleVM>();
            var user = HttpContext.User.Claims.ToList();

            if (user.Count != 0)
            {
                if (user.FirstOrDefault(u => u.Type == "IsAdmin").Value == "Y")
                {
                    List<Module> moduleitems = _functions.GetAllModules();

                    foreach (Module item in moduleitems)
                    {
                        List<Function> functionitems = _functions.GetALLFunctionByModule(item.Id);
                        ModuleVM module = new ModuleVM();

                        module.ModuleName = item.ModuleName;
                        module.IsActive = true;

                        foreach (Function function in functionitems)
                        {
                            module.Functions.Add(new FunctionVM()
                            {
                                FunctionName = function.FunctionName,
                                Controller = function.Controller,
                                Action = function.Action,
                                IsActive = true
                            });
                        }

                        modules.Add(module);
                    }
                }
                else
                {
                    List<Module> moduleitems = _functions.GetMoudlesByUserId(user.FirstOrDefault(u => u.Type == "UserId").Value);

                    foreach (Module item in moduleitems)
                    {
                        List<Function> functionitems = _functions.GetFunctionByModule(item.Id, user.FirstOrDefault(u => u.Type == "UserId").Value);
                        ModuleVM module = new ModuleVM();

                        module.ModuleName = item.ModuleName;
                        module.IsActive = true;

                        foreach (Function function in functionitems)
                        {
                            module.Functions.Add(new FunctionVM()
                            {
                                FunctionName = function.FunctionName,
                                Controller = function.Controller,
                                Action = function.Action,
                                IsActive = true
                            });
                        }

                        modules.Add(module);
                    }
                }
            }

            return View(modules);
        }
    }
}