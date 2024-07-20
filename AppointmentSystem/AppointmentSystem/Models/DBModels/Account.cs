using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 使用者帳號資料表
/// </summary>
public partial class Account
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string UserId { get; set; }

    public string Account1 { get; set; }

    public string Password { get; set; }

    public string Memo { get; set; }

    public virtual User User { get; set; }
}
