using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.HomeModels
{
    public partial class IndexVM
    {
        public List<LableListWithTreatment>? LableList { get; set; }

        public List<TreatmentCheckboxList>? TreatmentList { get; set; }

        public IndexVM()
        {
            LableList = new List<LableListWithTreatment>();
            TreatmentList = new List<TreatmentCheckboxList>();
        }
    }

    public partial class IndexAppointmentVM
    {
        public List<DoctorDataVM>? DoctorDatas { get; set; }

        public List<OutpatientTimeData>? OutpatientTimeDatas { get; set; }


        public IndexAppointmentVM()
        {
            DoctorDatas = new List<DoctorDataVM>();
            OutpatientTimeDatas = new List<OutpatientTimeData>();
        }
    }

    public partial class LableListWithTreatment
    {
        public string? LableId { get; set; }

        public string? LableName { get; set; }

        public string? TreatmentIdList { get; set; }

        //public List<string>? TreatmentIdList { get; set; }

        //public LableListWithTreatment()
        //{
        //    TreatmentIdList = new List<string>();
        //}
    }
}