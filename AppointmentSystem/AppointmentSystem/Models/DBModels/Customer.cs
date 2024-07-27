using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 客戶資料表
/// </summary>
public partial class Customer
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string MedicalRecordNumber { get; set; } = null!;

    public string? Name { get; set; }

    public string? LineId { get; set; }

    public string? DisplayName { get; set; }

    public string? LinePictureUrl { get; set; }

    public string? CellPhone { get; set; }

    public string? NationalIdNumber { get; set; }

    public string? Gender { get; set; }

    public string? Birthday { get; set; }

    public string? Email { get; set; }

    public string? Memo { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Customertoken> Customertokens { get; set; } = new List<Customertoken>();
}
