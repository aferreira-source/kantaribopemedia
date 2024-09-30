using App.UseCase.Plataforma.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace App.UseCase.Plataforma.Controllers
{
    public class MensagemController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public MensagemController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CadastroModal()
        {
            return PartialView();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
