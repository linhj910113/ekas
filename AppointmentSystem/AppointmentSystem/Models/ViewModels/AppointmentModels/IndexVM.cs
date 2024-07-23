using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.AppointmentModels
{
    public partial class IndexVM
    {
        public List<AppointmentData>? AppointmentData { get; set; }

        public IndexVM()
        {
            AppointmentData = new List<AppointmentData>();
        }
    }

    public partial class AppointmentData
    {
        public string? AppointmentId { get; set; }

        public string? Date { get; set; }

        public string? BookingBeginTime { get; set; }

        public string? BookingEndTime { get; set; }

        public DoctorDataVM DoctorData { get; set; }

        public string? CheckIn { get; set; }

        public string? CheckInTime { get; set; }

        public double? TimeUnitCount { get; set; }

        public CustomerData? customerData { get; set; }

        public List<TreatmentDataVM>? TreatmentData { get; set; }

        public List<TreatmentCheckboxList>? ActualTreatmentData { get; set; }

        public AppointmentData()
        {
            DoctorData = new DoctorDataVM();
            customerData = new CustomerData();
            TreatmentData = new List<TreatmentDataVM>();
            ActualTreatmentData = new List<TreatmentCheckboxList>();
        }
    }

    public partial class CustomerData()
    {
        public string? Id { get; set; }

        public string? LineId { get; set; }

        public string? DisplayName { get; set; }

        public string? LinePictureUrl { get; set; }

        public string? MedicalRecordNumber { get; set; }

        public string? Name { get; set; }

        public string? CellPhone { get; set; }

        public string? NationalIdNumber { get; set; }

        public string? Gender { get; set; }

        public string? Birthday { get; set; }

        public int? Age { get; set; }

        public int? missed { get; set; }

        public string? Email { get; set; }


    }

}