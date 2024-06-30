using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{

    public partial class FunctionIndexVM
    {
        public string? FunctionId { get; set; }

        public string? FunctionName { get; set; }

        public string? ModuleName { get; set; }

        public int? Sort { get; set; }

        public string? Memo { get; set; }

        public string? Status { get; set; }

        public FunctionIndexVM()
        {
            Memo = "";
        }
    }

    public partial class FunctionCreateVM
    {

        [Display(Name = "功能名稱")]
        public string? FunctionName { get; set; }

        [Display(Name = "模組ID")]
        public string? ModuleId { get; set; }

        public IEnumerable<SelectListItem>? ModuleList { get; set; }

        [Display(Name = "Controller")]
        public string? Controller { get; set; }

        [Display(Name = "Action")]
        public string? Action { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; } = "";

        public FunctionCreateVM()
        {
            FunctionName = "";
            ModuleId = "";
            Controller = "";
            Action = "";
            Memo = "";
        }
    }

    public partial class FunctionEditVM
    {
        [Display(Name = "功能名稱")]
        public string? FunctionName { get; set; }

        [Display(Name = "模組ID")]
        public string? ModuleId { get; set; }

        public IEnumerable<SelectListItem>? ModuleList { get; set; }

        [Display(Name = "Controller")]
        public string? Controller { get; set; }

        [Display(Name = "Action")]
        public string? Action { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; }

        [Display(Name = "是否啟用")]
        public string? Status { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        public FunctionEditVM()
        {
            FunctionName = "";
            ModuleId = "";
            Controller = "";
            Action = "";
            Memo = "";
        }
    }

    public partial class FunctionPermissionVM
    {
        public string? FunctionId { get; set; }

        public string? FunctionName { get; set; }

        public string? IsAllow { get; set; }

        public IEnumerable<SelectListItem>? IsAllowDropdown { get; set; }
    }

}
