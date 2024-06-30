using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

public partial class Positionpermission
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public long Index { get; set; }

    public string? PositionId { get; set; }

    public string? FunctionId { get; set; }

    public string? IsAllow { get; set; }

    public string Status { get; set; } = null!;

    public virtual Function? Function { get; set; }

    public virtual Position? Position { get; set; }
}
