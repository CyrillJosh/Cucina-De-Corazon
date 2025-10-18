using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int PersonId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Person Person { get; set; } = null!;
}
