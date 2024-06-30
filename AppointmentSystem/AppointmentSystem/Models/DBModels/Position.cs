using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

public partial class Position
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string? PositionName { get; set; }

    public string Memo { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Positionpermission> Positionpermissions { get; set; } = new List<Positionpermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
