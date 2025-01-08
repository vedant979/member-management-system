using System;
using System.Collections.Generic;

namespace Project5.Models;

public partial class Credential
{
    public Guid CredentialId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashPassword { get; set; } = null!;
}
