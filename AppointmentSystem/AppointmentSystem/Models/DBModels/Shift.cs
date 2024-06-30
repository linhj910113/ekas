using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師班表資料表
/// </summary>
public partial class Shift
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public string ShiftTypeId { get; set; } = null!;
}
