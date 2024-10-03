using app.plataforma.Identity;
using app.plataforma.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace app.plataforma.Handlers
{
    public class UsuarioPostagem: IUsuarioPostagem
    {        
        private readonly IFavoritosService _favoritosService;
        private readonly IPostagensService _postagensService;

        private UserManager<ApplicationUser> _userManager;

        private RoleManager<ApplicationRole> _roleManager;

        public List<Postagens> _postagens{ get; private set; }

        public List<Favoritos> _favoritos { get; private set; }

        public UsuarioPostagem(IFavoritosService favoritosService, IPostagensService postagensService, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            favoritosService = _favoritosService;
            postagensService = _postagensService;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        /// <summary>
        /// postagens
        /// </summary>
        /// <returns></returns>
        public async Task<List<Postagens>> ObterPostagemAsync()
        {
            return null; 
            //_postagens = await _postagensService.ObterTodosPorIdUsuarioAsync(_usuario.Id);
            //return _postagens;
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
            _favoritos = await _favoritosService.ObterListaPorIdUsuarioAsync(_userManager.GetUserId);
            return _favoritos;
        }

        public async Task AdicionarFavoritoAsync(Favoritos obj)
        {
            await _favoritosService.ObterListaPorIdUsuarioAsync(obj);
        }

        public async Task DeletarFavoritoAsync(object id)
        {
           await  _favoritosService.DeletarAsync(id);
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
}
