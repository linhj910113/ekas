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

        public int? Sort { get; set; }

        //public string? Status { get; set; }
    }

    //public partial class TreatmentCreateVM
    //{
    //    [Display(Name = "療程名稱")]
    //    public string? TreatmentName { get; set; }

    //    [Display(Name = "療程介紹")]
    //    public string? Introduction { get; set; }

    //    [Display(Name = "療程圖片")]
    //    public FileData? TreatmentImage { get; set; }

    //    [Display(Name = "療程用時")]
    //    public sbyte? Time { get; set; }

    //    [Display(Name = "術後提醒訊息")]
    //    public string? AlertMessage { get; set; }

    //    [Display(Name = "是否隱藏")]
    //    public string? Hide { get; set; }

    //    [Display(Name = "排序")]
    //    public int? Sort { get; set; }

    //    [Display(Name = "備註")]
    //    public string? Memo { get; set; }

    //    public TreatmentCreateVM()
    //    {
    //        AlertMessage = "";
    //        Memo = "";
    //        TreatmentImage = new FileData();
    //    }
    //}

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
        public int? Time { get; set; }

        [Display(Name = "標籤列表")]
        public List<SelectListItem>? TimeSelectList { get; set; }

        [Display(Name = "術後提醒訊息")]
        public string? AlertMessage { get; set; }

        [Display(Name = "是否隱藏")]
        public string? Hide { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; }

        [Display(Name = "是否啟用")]
        public string? Status { get; set; }

        [Display(Name = "標籤列表")]
        public List<LableCheckboxList>? LableList { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }

        [Display(Name = "選取的標籤")]
        public string? SelectedLable { get; set; }

        public TreatmentEditVM()
        {
            Memo = "";
            TreatmentImage = new FileData();
        }
    }

}
