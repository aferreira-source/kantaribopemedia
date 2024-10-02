using app.plataforma.Handlers;
using app.plataforma.Models;
using app.plataforma.Services;
using App.UseCase.Plataforma.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace App.UseCase.Plataforma.Controllers
{
    public class PostagensController : Controller
    {
        private readonly ILogger<PostagensController> _logger;
        private readonly IPostagensService _postagensService;
        private readonly IUsuarioPostagem _usuarioPostagem;
        

        public PostagensController(ILogger<PostagensController> logger, IPostagensService postagensService, IUsuarioPostagem usuarioPostagem)
        {
            _logger = logger;
            _postagensService = postagensService;
            _usuarioPostagem = usuarioPostagem;

        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult CadastroModal()
        //{
        //    return PartialView();
        //}

        public IActionResult Indexx()
        {
            return PartialView();
        }


        [HttpGet("edit/{IdCliente?}")]
        public async Task<IActionResult> Edit(int IdUsuario)
        {
            var model = new Postagens();
            //if (IdUsuario > 0)
            //{
            //    var entity = await _postagensService.GetAllAsync(IdCliente);       
            //}
            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
