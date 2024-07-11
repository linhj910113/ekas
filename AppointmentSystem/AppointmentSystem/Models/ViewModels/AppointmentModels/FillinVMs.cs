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
