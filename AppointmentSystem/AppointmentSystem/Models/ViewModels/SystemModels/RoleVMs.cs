using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{
    public partial class RoleIndexVM
    {
        public string? RoleId { get; set; }

        public string? RoleName { get; set; }

        public string? Status { get; set; }
    }

    public partial class RoleCreateVM
    {
        public string? RoleName { get; set; }

        public string? Status { get; set; }
    }

    public partial class RoleEditVM
    {
        [Display(Name = "角色名稱")]
        public string? RoleName { get; set; }

        [Display(Name = "是否啟用")]
        public string? Status { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }
    }

    public partial class RoleDefaultPermissionVM
    {
        public string? RoleId { get; set; }

        [Display(Name = "角色名稱")]
        public string? RoleName { get; set; }

        [Display(Name = "功能名稱")]
        public List<FunctionPermissionVM>? Functions { get; set; }

        public RoleDefaultPermissionVM()
        {
            Functions = new List<FunctionPermissionVM>();
        }
    }

}
