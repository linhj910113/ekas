using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師門診資料表
/// </summary>
public partial class Doctoroutpatient
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public string ArrangeId { get; set; } = null!;

    public string Year { get; set; } = null!;

    public string Month { get; set; } = null!;

    public string Day { get; set; } = null!;

    public string BeginTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public string AppointmentId { get; set; } = null!;
}
