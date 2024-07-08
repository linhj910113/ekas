namespace AppointmentSystem.Models.ViewModels.AppointmentModels
{
    public partial class SuccessPageVM
    {
        public string? Date { get; set; }

        public string? BookingBeginTime { get; set; }

        public DoctorDataVM SelectedDoctor { get; set; }

        public List<TreatmentDataVM>? SelectedTreatment { get; set; }

        public string? NotifyMessage { get; set; }

        public SuccessPageVM()
        {
            SelectedDoctor = new DoctorDataVM();
            SelectedTreatment = new List<TreatmentDataVM>();
        }
    }
}
