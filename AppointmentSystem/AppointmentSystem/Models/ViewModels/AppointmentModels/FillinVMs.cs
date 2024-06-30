using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.AppointmentModels
{

    public partial class TreatmentDataVM
    {
        public string? TreatmentId { get; set; }

        public string? TreatmentName { get; set; }

        public string? Introduction { get; set; }

        public FileData? ImageFile { get; set; }

        public string? Image { get; set; }

        public int? Time { get; set; }
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
