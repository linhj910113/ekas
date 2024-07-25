using System;
using System.Collections.Generic;

namespace AppointmentSystem.Models.DBModels;

/// <summary>
/// 驗證碼資料表
/// </summary>
public partial class Verificationcode
{
    public DateTime CreateDate { get; set; }

    public string Creator { get; set; }

    public DateTime ModifyDate { get; set; }

    public string Modifier { get; set; }

    public string Status { get; set; }

    public long Index { get; set; }

    public string SouceTable { get; set; }

    public string ForeignKey { get; set; }

    public string LoginBy { get; set; }

    public string HashCode { get; set; }

    public string Otp { get; set; }

    public DateTime ExpireTime { get; set; }
}
