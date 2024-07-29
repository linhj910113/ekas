using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 療程標籤資料表
/// </summary>
public partial class Treatmentlabel
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string TreatmentId { get; set; } = null!;

    public string LabelId { get; set; } = null!;

    public virtual Label Label { get; set; } = null!;

    public virtual Treatment Treatment { get; set; } = null!;
}
