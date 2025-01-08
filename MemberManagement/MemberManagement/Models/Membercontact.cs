using System;
using System.Collections.Generic;

namespace Project5.Models;

public partial class Membercontact
{
    public Guid MemberCredentialId { get; set; }

    public string? ContactType { get; set; }

    public Guid MemberId { get; set; }

    public Guid ContactId { get; set; }

    public virtual Contact Contact { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
