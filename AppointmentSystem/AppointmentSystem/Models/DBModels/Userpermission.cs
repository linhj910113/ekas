using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 使用者權限資料表
/// </summary>
public partial class Userpermission
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long Index { get; set; }

    public string? UserId { get; set; }

    public string? FunctionId { get; set; }

    public string? IsAllow { get; set; }

    public virtual Function? Function { get; set; }

    public virtual User? User { get; set; }
}
