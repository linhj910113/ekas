using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.BaseInfoModels
{

    public partial class DoctorIndexVM
    {
        public string? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public int? Sort { get; set; }

        public string? Status { get; set; }
    }

    public partial class DoctorEditVM
    {
        public string? DoctorId { get; set; }

        [Display(Name = "醫師姓名")]
        public string? DoctorName { get; set; }

        [Display(Name = "醫師英文姓名")]
        public string? DoctorNameEnglish { get; set; }

        [Display(Name = "醫師介紹")]
        public string? Introduction { get; set; }

        [Display(Name = "科別/職稱")]
        public string? DepartmentTitle { get; set; }

        [Display(Name = "醫師照片")]
        public FileData? DoctorImage { get; set; }

        [Display(Name = "療程列表")]
        public List<TreatmentCheckboxList>? TreatmentList { get; set; }

        [Display(Name = "選取的療程")]
        public string? SelectedTreatment { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; } = "";

        [Display(Name = "是否啟用")]
        public string Status { get; set; } = "";

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        public DoctorEditVM()
        {
            DoctorImage = new FileData();
            Memo = "";
        }
    }

    public partial class DoctorDataVM
    {
        public string? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public string? Introduction { get; set; }

        public string? DepartmentTitle { get; set; }

        public FileData? ImageFile { get; set; }

        public string? Image { get; set; }
    }
}
