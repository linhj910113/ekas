using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師請假資料表
/// </summary>
public partial class Doctordayoff
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string DoctorId { get; set; }

    public string Type { get; set; }

    public string Date { get; set; }

    public string BeginTime { get; set; }

    public string EndTime { get; set; }
}
