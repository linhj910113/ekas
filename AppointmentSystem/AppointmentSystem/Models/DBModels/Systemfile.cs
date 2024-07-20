using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 上傳檔案資料表
/// </summary>
public partial class Systemfile
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string FileName { get; set; }

    public string FileExtension { get; set; }

    public long? FileSize { get; set; }

    public string Path { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
