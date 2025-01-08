using System;
using System.Collections.Generic;

namespace Project5.Models;

public partial class Member
{
    public Guid MemberId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public DateTime? Dob { get; set; }

    public string? Gender { get; set; }

    public string HashPassword { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Memberaddress> Memberaddresses { get; set; } = new List<Memberaddress>();

    public virtual ICollection<Sessionlog> Sessionlogs { get; set; } = new List<Sessionlog>();
}
