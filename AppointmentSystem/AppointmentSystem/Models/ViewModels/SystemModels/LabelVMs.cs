using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModels.SystemModels
{

    public partial class LabelIndexVM
    {
        public string? LabelId { get; set; }

        public string? Type { get; set; }

        public string? LabelName { get; set; }

        public int? Sort { get; set; }

        public string? Status { get; set; }
    }

    public partial class LabelCreateVM
    {
        //標籤類別(暫時沒有用到)
        public string? Type { get; set; }

        public string LabelName { get; set; } = "";

        public int? Sort { get; set; }
    }

    public partial class LabelEditVM
    {
        //標籤類別(暫時沒有用到)
        public string? Type { get; set; }

        public string LabelName { get; set; } = "";

        public int? Sort { get; set; }

        public string? Status { get; set; }

        public IEnumerable<SelectListItem>? StatusList { get; set; }
    }

}
