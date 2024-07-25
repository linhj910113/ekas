using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 標籤資料表
/// </summary>
public partial class Label
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    /// <summary>
    /// Treatment:療程用的標籤
    /// </summary>
    public string Type { get; set; }

    public string LabelName { get; set; }

    public int? Sort { get; set; }

    public virtual ICollection<Treatmentlabel> Treatmentlabels { get; set; } = new List<Treatmentlabel>();
}
