using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 醫師資料表
/// </summary>
public partial class Doctor
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string DoctorName { get; set; }

    public string DoctorNameEnglish { get; set; }

    public string Introduction { get; set; }

    public string DepartmentTitle { get; set; }

    public string ColorHex { get; set; }

    public string ImageFileId { get; set; }

    public int? Sort { get; set; }

    public string Memo { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctortreatment> Doctortreatments { get; set; } = new List<Doctortreatment>();

    public virtual Systemfile ImageFile { get; set; }
}
