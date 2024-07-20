using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 系統紀錄資料表
/// </summary>
public partial class Systemlog
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public long Index { get; set; }

    public string UserAccount { get; set; }

    public string Description { get; set; }
}
