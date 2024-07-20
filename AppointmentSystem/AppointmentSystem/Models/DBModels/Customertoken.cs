using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 客戶Toekn資料表
/// </summary>
public partial class Customertoken
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string CustomerId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public int? ExpiresIn { get; set; }

    public virtual Customer Customer { get; set; }
}
