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

    public string? DoctorId { get; set; }

    public string? ArrangeId { get; set; }

    public string? Year { get; set; }

    public string? Month { get; set; }

    public string? Day { get; set; }

    public string? BeginTime { get; set; }

    public string? EndTime { get; set; }

    public string? BppointmentId { get; set; }
}
