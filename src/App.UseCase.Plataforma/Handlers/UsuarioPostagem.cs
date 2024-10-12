using app.plataforma.Identity;
using app.plataforma.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace app.plataforma.Handlers;

public class UsuarioPostagem : IUsuarioPostagem
{
    private readonly IFavoritosService _favoritosService;
    private readonly IPostagensService _postagensService;

    private UserManager<ApplicationUser> _userManager;

    private RoleManager<ApplicationRole> _roleManager;

    private List<Postagens> _postagens { get; }

    private List<Favoritos> _favoritos { get; }


    private readonly IHttpContextAccessor _httpContextAccessor;
    public UsuarioPostagem(IFavoritosService favoritosService,
        IPostagensService postagensService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IHttpContextAccessor httpContextAccessor)
    {
        favoritosService = _favoritosService;
        postagensService = _postagensService;
        this._userManager = userManager;
        this._roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// postagens
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Postagens>> ObterPostagemAsync()
    {
        return await _postagensService.ObterPostagens();
    }

    public async Task<IEnumerable<Postagens>> ObterPostagemPorIdUsuarioAsync(Object usuarioId)
    {
        return await _postagensService.ObterPostagens();
    }

    public async Task AdicionarPostagemAsync(Postagens obj)
    {
        await _postagensService.InserirAsync(obj);
    }

    public async Task AlterarPostagemAsync(Postagens obj)
    {
        await _postagensService.AlterarAsync(obj);
    }

    public async Task DeletarPostagemAsync(object id)
    {
        await _postagensService.DeletarAsync(id);
    }

    public async Task DeletarTodasPostagemPorIdUsuarioAsync()
    {
        await _postagensService.DeletarTodosPorIdUsuarioAsync(_userManager.GetUserId);
    }


    /// <summary>
    /// favoritos
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>

    public async Task<List<Favoritos>> ObterFavoritosPorIdUsuarioAsync()
    {
        //_favoritos = await _favoritosService.ObterListaPorIdUsuarioAsync(_userManager.GetUserId);
        return null;//_favoritos;
    }

    public async Task AdicionarFavoritoAsync(Favoritos obj)
    {
        await _favoritosService.ObterListaPorIdUsuarioAsync(obj);
    }

    public async Task DeletarFavoritoAsync(object id)
    {
        await _favoritosService.DeletarAsync(id);
    }

    public async Task DeletarTodosFavoritosPorIdUsuarioAsync()
    {
        await _favoritosService.DeletarTodosAsync(_userManager.GetUserId);
    }



    //public Task<UsuarioPostagem> CarregarDadosAsync(object IdUsuario)
    //{
    //    _usuario = 
    //    throw new NotImplementedException();
    //}        


}
