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

        [Display(Name = "醫師代表顏色")]
        public string? ColorHEX { get; set; }

        [Display(Name = "醫師照片")]
        public FileData? DoctorImage { get; set; }

        public string? Image { get; set; }

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

        public string? ColorHEX { get; set; }

        public FileData? ImageFile { get; set; }

        public string? Image { get; set; }
    }

    #region -- 醫師請假 -- (DoctorDayOff)

    public partial class DoctorDayOffIndexVM
    {
        public long? DayOffIndex { get; set; }

        public string? DoctorName { get; set; }

        public string? Type { get; set; }

        public string? BeginDate { get; set; }

        public string? BeginTime { get; set; }

        public string? EndDate { get; set; }

        public string? EndTime { get; set; }

        public string? Status { get; set; }
    }

    public partial class DoctorDayOffCreateVM
    {
        [Display(Name = "醫師ID")]
        public string? DoctorId { get; set; }

        public IEnumerable<SelectListItem>? DoctorList { get; set; }

        [Display(Name = "假別")]
        public string? Type { get; set; }

        public IEnumerable<SelectListItem>? TypeList { get; set; }

        [Display(Name = "開始日期")]
        public string? BeginDate { get; set; }

        [Display(Name = "開始時間")]
        public string? BeginTime { get; set; }

        public IEnumerable<SelectListItem>? BeginTimeList { get; set; }

        [Display(Name = "結束日期")]
        public string? EndDate { get; set; }

        [Display(Name = "結束時間")]
        public string? EndTime { get; set; }

        public IEnumerable<SelectListItem>? EndTimeList { get; set; }
    }

    public partial class DoctorDayOffEditVM
    {
        [Display(Name = "醫師ID")]
        public string? DoctorId { get; set; }

        public IEnumerable<SelectListItem>? DoctorList { get; set; }

        [Display(Name = "假別")]
        public string? Type { get; set; }

        public IEnumerable<SelectListItem>? TypeList { get; set; }

        [Display(Name = "開始日期")]
        public string? BeginDate { get; set; }

        [Display(Name = "開始時間")]
        public string? BeginTime { get; set; }

        public IEnumerable<SelectListItem>? BeginTimeList { get; set; }

        [Display(Name = "結束日期")]
        public string? EndDate { get; set; }

        [Display(Name = "結束時間")]
        public string? EndTime { get; set; }

        public IEnumerable<SelectListItem>? EndTimeList { get; set; }
    }

    #endregion
}
