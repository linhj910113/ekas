using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 預約資料表
/// </summary>
public partial class Appointment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public string? Date { get; set; }

    public string? BookingBeginTime { get; set; }

    public string? BookingEndTime { get; set; }

    public string? CheckIn { get; set; }

    public string? CheckInTime { get; set; }

    public virtual ICollection<Appointmenttreatment> Appointmenttreatments { get; set; } = new List<Appointmenttreatment>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
