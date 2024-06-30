namespace AppointmentSystem.Models.ViewModels
{

    public partial class SelectListModels
    {
        public class ModuleDropdown
        {
            public string? ModuleId { get; set; }

            public string? ModuleName { get; set; }
        }

        //public class FunctionData
        //{
        //    public string? FunctionId { get; set; }

        //    public string? FunctionName { get; set; }

        //    public string? IsAllow { get; set; }

        //    public IEnumerable<SelectListItem>? IsAllowList { get; set; }
        //}

        public class RoleDropdown
        {
            public string? RoleId { get; set; }

            public string? RoleName { get; set; }
        }

        public class TreatmentCheckboxList
        {
            public string? TreatmentId { get; set; }

            public string? TreatmentName { get; set; }

            public string? IsChecked { get; set; }
        }

        public class DoctorCheckboxList
        {
            public string? DoctorId { get; set; }

            public string? DoctorName { get; set; }

            public string? IsChecked { get; set; }
        }

        public class LableCheckboxList
        {
            public string? LableId { get; set; }

            public string? LableName { get; set; }

            public string? IsChecked { get; set; }
        }
    }
}
