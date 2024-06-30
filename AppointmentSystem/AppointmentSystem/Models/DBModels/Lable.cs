using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 標籤資料表
/// </summary>
public partial class Lable
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    /// <summary>
    /// Treatment:療程用的標籤
    /// </summary>
    public string Type { get; set; } = null!;

    public string LableName { get; set; } = null!;

    public virtual ICollection<Treatmentlable> Treatmentlables { get; set; } = new List<Treatmentlable>();
}
