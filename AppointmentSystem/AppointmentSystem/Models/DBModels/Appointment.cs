using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 預約資料表
/// </summary>
public partial class Appointment
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string CustomerId { get; set; }

    public string DoctorId { get; set; }

    public string Date { get; set; }

    public string BookingBeginTime { get; set; }

    public string BookingEndTime { get; set; }

    public string CheckIn { get; set; }

    public string CheckInTime { get; set; }

    public virtual ICollection<Appointmenttreatment> Appointmenttreatments { get; set; } = new List<Appointmenttreatment>();

    public virtual Customer Customer { get; set; }

    public virtual Doctor Doctor { get; set; }
}
