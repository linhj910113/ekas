using AppointmentSystem.Models.ViewModels.BaseInfoModels;

namespace AppointmentSystem.Models.ViewModels.AppointmentModels
{
    public partial class EditVM
    {
        public string? AppointmentId { get; set; }

        public string? Date { get; set; }

        public string? BookingBeginTime { get; set; }

        public string? DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public List<DoctorDataVM> DoctorList { get; set; }

        public List<TreatmentDataVM>? TreatmentList { get; set; }

        public List<string>? SelectedTreatment { get; set; }

        public List<OutpatientTimeData>? Outpatients { get; set; }

        public EditVM()
        {
            DoctorList = new List<DoctorDataVM>();
            TreatmentList = new List<TreatmentDataVM>();
            SelectedTreatment = new List<string>();
            Outpatients = new List<OutpatientTimeData>();
            //SelectedTreatment = new List<TreatmentDataVM>();
        }
    }
}
