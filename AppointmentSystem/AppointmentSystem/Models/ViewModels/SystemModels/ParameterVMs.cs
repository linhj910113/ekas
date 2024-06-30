using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{

    public partial class ParameterIndexVM
    {
        public long? Index { get; set; }

        public string? ParameterName { get; set; }

        public string? Memo { get; set; }
    }

    public partial class ParameterEditVM
    {
        public long? Index { get; set; }

        [Display(Name = "參數名稱")]
        public string? ParameterName { get; set; }

        [Display(Name = "備註")]
        public string? Memo { get; set; }

        [Display(Name = "種類")]
        public string? Type { get; set; }

        [Display(Name = "值")]
        public string? Value { get; set; }

        [Display(Name = "是否鎖定")]
        public string? Locked { get; set; }

        [Display(Name = "圖片")]
        public FileData? ImageFile { get; set; }

        public ParameterEditVM()
        {
            ImageFile = new FileData();
        }
    }

}
