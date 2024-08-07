using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 客戶Toekn資料表
/// </summary>
public partial class Customertoken
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string? CustomerId { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public int? ExpiresIn { get; set; }

    public virtual Customerlineaccount? Customer { get; set; }
}
