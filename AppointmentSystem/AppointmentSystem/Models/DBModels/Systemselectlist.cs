using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 下拉式選單資料表
/// </summary>
public partial class Systemselectlist
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Id { get; set; }

    public string? GroupName { get; set; }

    public string? SelectName { get; set; }

    public string? SelectValue { get; set; }

    public string? Memo { get; set; }
}
