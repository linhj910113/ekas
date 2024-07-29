using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 預約療程資料表
/// </summary>
public partial class Appointmenttreatment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string AppointmentId { get; set; } = null!;

    /// <summary>
    /// A:預約的療程，R:實際執行的療程
    /// </summary>
    public string Type { get; set; } = null!;

    public string TreatmentId { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Treatment Treatment { get; set; } = null!;
}
