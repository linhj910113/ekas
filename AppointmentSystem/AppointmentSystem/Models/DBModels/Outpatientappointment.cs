using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 門診對應預約資料表
/// </summary>
public partial class Outpatientappointment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string? OutpatientId { get; set; }

    public string? Type { get; set; }

    public string? AppointmentId { get; set; }
}
