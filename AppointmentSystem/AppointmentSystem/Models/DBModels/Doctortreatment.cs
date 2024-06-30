using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師負責療程資料表
/// </summary>
public partial class Doctortreatment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string DoctorId { get; set; } = null!;

    public string TreatmentId { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Treatment Treatment { get; set; } = null!;
}
