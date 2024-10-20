namespace app.plataforma;

public class Favoritos : EntityBase
{
    public object? documentoId { get; set; }
    public object? usuarioId { get; set; }
    public string? dtCadastro { get; set; }
}
