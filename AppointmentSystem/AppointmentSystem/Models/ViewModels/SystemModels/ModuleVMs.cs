using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{

    public partial class ModuleIndexVM
    {
        public string? ModuleId { get; set; }

        public string? ModuleName { get; set; }

        public int? Sort { get; set; }

        public string? Memo { get; set; }

        public string? Status { get; set; }

        public ModuleIndexVM()
        {
            Memo = "";
        }
    }

    public partial class ModuleCreateVM
    {
        [Display(Name = "模組名稱")]
        public string ModuleName { get; set; } = "";

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; } = "";

        public ModuleCreateVM()
        {
            Memo = "";
        }
    }

    public partial class ModuleEditVM
    {
        [Display(Name = "模組名稱")]
        public string? ModuleName { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; }

        [Display(Name = "是否啟用")]
        public string? Status { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        public ModuleEditVM()
        {
            Memo = "";
        }
    }

}
