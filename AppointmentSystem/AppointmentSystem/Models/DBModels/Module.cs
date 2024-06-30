using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 系統模組資料表
/// </summary>
public partial class Module
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string? ModuleName { get; set; }

    public int? Sort { get; set; }

    public string? Memo { get; set; }

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
}
