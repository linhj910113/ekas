using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 使用者帳號資料表
/// </summary>
public partial class Account
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Account1 { get; set; }

    public string? Password { get; set; }

    public string? Memo { get; set; }

    public virtual User User { get; set; } = null!;
}
