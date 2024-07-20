using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師門診資料表
/// </summary>
public partial class Doctoroutpatient
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string DoctorId { get; set; }

    public string ArrangeId { get; set; }

    public string Year { get; set; }

    public string Month { get; set; }

    public string Day { get; set; }

    public string BeginTime { get; set; }

    public string EndTime { get; set; }

    public string AppointmentId { get; set; }
}
