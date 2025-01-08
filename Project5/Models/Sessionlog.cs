using System;
using System.Collections.Generic;

namespace Project5.Models;

public partial class Sessionlog
{
    public Guid SessionlogId { get; set; }

    public sbyte? SessionDuration { get; set; }

    public string Token { get; set; } = null!;

    public Guid MemberId { get; set; }

    public string? IsValid { get; set; }

    public virtual Member Member { get; set; } = null!;
}
