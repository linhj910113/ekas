using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Models.ViewModels.HomeModels
{
    public partial class IndexVM
    {
        public List<LabelListWithTreatment>? LabelList { get; set; }

        public List<TreatmentCheckboxList>? TreatmentList { get; set; }

        public IndexVM()
        {
            LabelList = new List<LabelListWithTreatment>();
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

    public partial class UsernameAndRoleNameVM
    {
        public string? Username { get; set; }

        public string? Rolename { get; set; }
    }

    public partial class LabelListWithTreatment
    {
        public string? LabelId { get; set; }

        public string? LabelName { get; set; }

        public string? TreatmentIdList { get; set; }

        //public List<string>? TreatmentIdList { get; set; }

        //public LabelListWithTreatment()
        //{
        //    TreatmentIdList = new List<string>();
        //}
    }
}