using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 班表類別資料表
/// </summary>
public partial class Shifttype
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }

    public string BeginTime { get; set; }

    public string EndTime { get; set; }

    public sbyte Sort { get; set; }
}
