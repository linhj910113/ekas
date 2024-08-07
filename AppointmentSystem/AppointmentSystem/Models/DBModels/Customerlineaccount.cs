using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 客戶Line資料表
/// </summary>
public partial class Customerlineaccount
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string? LineId { get; set; }

    public string? DisplayName { get; set; }

    public string? LinePictureUrl { get; set; }

    public string? NationalIdNumber { get; set; }

    public string? Cellphone { get; set; }

    public virtual ICollection<Customertoken> Customertokens { get; set; } = new List<Customertoken>();
}
