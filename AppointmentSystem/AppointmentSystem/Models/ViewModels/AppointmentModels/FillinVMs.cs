using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.AppointmentModels
{
    //public partial class EditVM
    //{
    //    public string? AppointmentId { get; set; }

    //    public string? DoctorId { get; set; }

    //    public string? Date { get; set; }

    //    public string? BookingBeginTime { get; set; }

    //    public List<TreatmentDataVM>? Treatment { get; set; }

    //    public EditVM()
    //    {
    //        Treatment = new List<TreatmentDataVM>();
    //    }
    //}

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

    public partial class FillinDateTimeVM
    {
        public string? Date { get; set; }

        public List<OutpatientTimeData>? Outpatients { get; set; }

        public FillinDateTimeVM()
        {
            Outpatients = new List<OutpatientTimeData>();
        }
    }

    public partial class OutpatientTimeData
    {
        public string? BeginTime { get; set; }

        public string? EndTime { get; set; }

        public string? Enabled { get; set; }
    }


}
