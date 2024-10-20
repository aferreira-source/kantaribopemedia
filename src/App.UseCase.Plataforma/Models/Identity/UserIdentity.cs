using System.ComponentModel.DataAnnotations;

namespace app.plataforma.Identity;

public class UserIdentity
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
