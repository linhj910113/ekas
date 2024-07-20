using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

public partial class Systemparameter
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public string Value { get; set; }

    public string Locked { get; set; }

    public string Memo { get; set; }
}
