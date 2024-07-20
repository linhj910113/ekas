using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 系統功能資料表
/// </summary>
public partial class Function
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public string Id { get; set; }

    public string FunctionName { get; set; }

    public string ModuleId { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public int? Sort { get; set; }

    public string Memo { get; set; }

    public virtual Module Module { get; set; }

    public virtual ICollection<Rolepermission> Rolepermissions { get; set; } = new List<Rolepermission>();

    public virtual ICollection<Userpermission> Userpermissions { get; set; } = new List<Userpermission>();
}
