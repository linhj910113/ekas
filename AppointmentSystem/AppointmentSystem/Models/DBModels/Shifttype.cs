using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 班表類別資料表
/// </summary>
public partial class Shifttype
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string BeginTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public sbyte Sort { get; set; }
}
