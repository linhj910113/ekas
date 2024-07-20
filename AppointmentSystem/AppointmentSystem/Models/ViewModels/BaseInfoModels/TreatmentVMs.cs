using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.BaseInfoModels
{

    public partial class TreatmentIndexVM
    {
        public string? TreatmentId { get; set; }

        [Display(Name = "療程名稱")]
        public string? TreatmentName { get; set; }

        public string? Hide { get; set; }

        public int? Sort { get; set; }

        //public string? Status { get; set; }
    }

    public partial class TreatmentCreateVM
    {
        [Display(Name = "時間列表")]
        public List<SelectListItem>? TimeSelectList { get; set; }

        [Display(Name = "標籤列表")]
        public List<LabelCheckboxList>? LabelList { get; set; }
    }

    public partial class TreatmentEditVM
    {
        public string? TreatmentId { get; set; }

        [Display(Name = "療程名稱")]
        public string? TreatmentName { get; set; }

        [Display(Name = "療程介紹")]
        public string? Introduction { get; set; }

        [Display(Name = "療程圖片")]
        public FileData? TreatmentImage { get; set; }

        [Display(Name = "療程用時")]
        public int Time { get; set; }

        [Display(Name = "時間列表")]
        public List<SelectListItem>? TimeSelectList { get; set; }

        [Display(Name = "術後提醒訊息")]
        public string? AlertMessage { get; set; }

        [Display(Name = "隱藏")]
        public string? Hide { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; }

        [Display(Name = "是否啟用")]
        public string? Status { get; set; }

        [Display(Name = "標籤列表")]
        public List<LabelCheckboxList>? LabelList { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        [Display(Name = "選取的標籤")]
        public string? SelectedLabel { get; set; }

        public TreatmentEditVM()
        {
            Memo = "";
            TreatmentImage = new FileData();
        }
    }

    public partial class TreatmentDataVM
    {
        public string? TreatmentId { get; set; }

        public string? TreatmentName { get; set; }

        public string? Introduction { get; set; }

        public FileData? ImageFile { get; set; }

        public string? Image { get; set; }

        public int? Time { get; set; }
    }

}
