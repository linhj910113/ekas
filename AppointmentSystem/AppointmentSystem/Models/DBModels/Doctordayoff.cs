using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師請假資料表
/// </summary>
public partial class Doctordayoff
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string DoctorId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Date { get; set; } = null!;

    public string BeginTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;
}
