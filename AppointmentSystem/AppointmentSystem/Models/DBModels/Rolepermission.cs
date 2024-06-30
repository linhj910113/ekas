using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 角色預設權限資料表
/// </summary>
public partial class Rolepermission
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string RoleId { get; set; } = null!;

    public string FunctionId { get; set; } = null!;

    public string? IsAllow { get; set; }

    public virtual Function Function { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
