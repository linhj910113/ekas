using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師負責療程資料表
/// </summary>
public partial class Doctortreatment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string DoctorId { get; set; }

    public string TreatmentId { get; set; }

    public virtual Doctor Doctor { get; set; }

    public virtual Treatment Treatment { get; set; }
}
