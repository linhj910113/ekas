using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 使用者資料表
/// </summary>
public partial class User
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string UserName { get; set; }

    public string UserNameEnglish { get; set; }

    public string Gender { get; set; }

    public string Birthday { get; set; }

    public string Address { get; set; }

    public string UserEmail { get; set; }

    public string Telphone { get; set; }

    public string RoleId { get; set; }

    public string IsAdmin { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Role Role { get; set; }

    public virtual ICollection<Userpermission> Userpermissions { get; set; } = new List<Userpermission>();
}
