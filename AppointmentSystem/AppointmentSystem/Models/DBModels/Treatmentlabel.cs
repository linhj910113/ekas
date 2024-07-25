using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 療程標籤資料表
/// </summary>
public partial class Treatmentlabel
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string TreatmentId { get; set; }

    public string LabelId { get; set; }

    public virtual Label Label { get; set; }

    public virtual Treatment Treatment { get; set; }
}
