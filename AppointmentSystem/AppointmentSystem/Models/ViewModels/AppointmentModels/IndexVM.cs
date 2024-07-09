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

    public partial class CustomerData()
    {
        public string? Id { get; set; }

        public string? LineId { get; set; }

        public string? LineDiaplayName { get; set; }

        public string? LinePictureUrl { get; set; }

        public string? CellPhone { get; set; }

        public string? NationalIdNumber { get; set; }

        public string? Gender { get; set; }

        public string? Birthday { get; set; }

        public string? Email { get; set; }


    }

}