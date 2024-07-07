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

        public string? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public List<TreatmentDataVM>? TreatmentData { get; set; }

        public AppointmentData()
        {
            TreatmentData = new List<TreatmentDataVM>();
        }
    }
}