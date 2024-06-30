using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 療程資料表
/// </summary>
public partial class Treatment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string? TreatmentName { get; set; }

    public string? Introduction { get; set; }

    public string? ImageFileId { get; set; }

    public int? Time { get; set; }

    public string? AlertMessage { get; set; }

    public string? Hide { get; set; }

    public int? Sort { get; set; }

    public string? Memo { get; set; }

    public virtual ICollection<Appointmenttreatment> Appointmenttreatments { get; set; } = new List<Appointmenttreatment>();

    public virtual ICollection<Doctortreatment> Doctortreatments { get; set; } = new List<Doctortreatment>();

    public virtual Systemfile? ImageFile { get; set; }

    public virtual ICollection<Treatmentlable> Treatmentlables { get; set; } = new List<Treatmentlable>();
}
