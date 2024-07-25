using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 預排醫師班表資料表
/// </summary>
public partial class Arrangedoctorshift
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string DoctorId { get; set; }

    public string Year { get; set; }

    public string Month { get; set; }

    public string Day { get; set; }

    public string ShiftTypeId { get; set; }
}
