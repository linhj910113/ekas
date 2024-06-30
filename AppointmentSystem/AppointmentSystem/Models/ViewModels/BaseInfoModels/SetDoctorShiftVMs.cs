using AppointmentSystem.Models.ViewModels.AppointmentModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.BaseInfoModels
{

    public partial class SetDoctorShiftVM
    {
        public List<ShiftTypeData>? shiftTypeData { get; set; } = null;

        public List<DoctorCheckboxList>? doctorData { get; set; } = null;

        public List<ArrangeDoctorShiftData>? arrangeDoctorShiftData { get; set; } = null;

        public string? Locked { get; set; }

        public SetDoctorShiftVM()
        {
            shiftTypeData = new List<ShiftTypeData>();
            doctorData = new List<DoctorCheckboxList>();
            arrangeDoctorShiftData = new List<ArrangeDoctorShiftData>();
        }
    }

    public partial class ShiftTypeData
    {
        public string? ShiftId { get; set; }

        public string? ShiftName { get; set; }

        public string? BeginTime { get; set; }

        public string? EndTime { get; set; }
    }

    public partial class ArrangeDoctorShiftData
    {
        public string? Id { get; set; }

        public string? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public string? Year { get; set; }

        public string? Month { get; set; }

        public string? Day { get; set; }

        public string? ShiftTypeId { get; set; }
    }

    public partial class PostArrangeShiftData
    {
        public string? ShiftTypeId { get; set; }

        public string? Year { get; set; }

        public string? Month { get; set; }

        public string? Day { get; set; }

        public string? DoctorIdList { get; set; }
    }

}
