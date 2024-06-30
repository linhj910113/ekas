namespace AppointmentSystem.Models.ViewModels
{
    public partial class ModuleVM
    {
        public string? ModuleName { get; set; }

        public List<FunctionVM>? Functions { get; set; }

        public bool IsActive { get; set; }

        public ModuleVM()
        {
            Functions = new List<FunctionVM>();
        }
    }

    public partial class FunctionVM
    {
        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? FunctionName { get; set; }

        public bool IsActive { get; set; }
    }

    public partial class FileData
    {
        public IFormFile? File { get; set; }

        public string? FileID { get; set; } = "";

        public string? FileName { get; set; } = "";

        public string? FileExtension { get; set; } = "";

        public long? FileSize { get; set; }

        public string? Path { get; set; } = "";
    }

}
