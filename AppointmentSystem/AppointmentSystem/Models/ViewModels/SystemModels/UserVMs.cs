using AppointmentSystem.Models.DBModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{
    public partial class UserIndexVM
    {
        public string? UserId { get; set; }

        public string? UserAccount { get; set; }

        public string? UserName { get; set; }

        public string? Gender { get; set; }

        public string? UserEmail { get; set; }

        public string? Status { get; set; }

        public UserIndexVM()
        {
            UserAccount = "";
        }
    }

    public partial class UserCreateVM
    {
        public string UserName { get; set; } = "";

        public string? UserNameEnglish { get; set; }

        public string? Gender { get; set; }

        public string? Birthday { get; set; }

        public string? Address { get; set; }

        public string? UserEmail { get; set; }

        public string? Telphone { get; set; }

        public string? RoleId { get; set; }

        public string? IsAdmin { get; set; }

        public IEnumerable<SelectListItem>? GenderList { get; set; }

        public IEnumerable<SelectListItem>? RoleList { get; set; }

        public UserCreateVM()
        {
            UserNameEnglish = "";
            Gender = "";
            Birthday = "";
            Address = "";
            UserEmail = "";
            Telphone = "";
            RoleId = "";
        }
    }

    public partial class UserEditVM
    {
        public string UserName { get; set; } = "";

        public string? UserNameEnglish { get; set; }

        public string? Gender { get; set; }

        public string? Birthday { get; set; }

        public string? Address { get; set; }

        public string? UserEmail { get; set; }

        public string? Telphone { get; set; }

        public string? RoleId { get; set; }

        public string? IsAdmin { get; set; }

        public string? Status { get; set; }

        public IEnumerable<SelectListItem>? GenderList { get; set; }

        public IEnumerable<SelectListItem>? RoleList { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        public UserEditVM()
        {
            UserNameEnglish = "";
            Gender = "";
            Birthday = "";
            Address = "";
            UserEmail = "";
            Telphone = "";
            RoleId = "";
        }
    }

    public partial class UserDefaultPermissionVM
    {
        public string? UserId { get; set; }

        [Display(Name = "用戶名稱")]
        public string? UserName { get; set; }

        [Display(Name = "功能權限清單")]
        public List<FunctionPermissionVM>? Functions { get; set; }

        public UserDefaultPermissionVM()
        {
            Functions = new List<FunctionPermissionVM>();
        }
    }

    public partial class UserAccountVM
    {
        public string? Account { get; set; }

        public string? Password { get; set; }

        public string? Memo { get; set; }

        public UserAccountVM()
        {
            Memo = "";
        }
    }

}
