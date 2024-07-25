using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 角色預設權限資料表
/// </summary>
public partial class Rolepermission
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string RoleId { get; set; }

    public string FunctionId { get; set; }

    public string IsAllow { get; set; }

    public virtual Function Function { get; set; }

    public virtual Role Role { get; set; }
}
