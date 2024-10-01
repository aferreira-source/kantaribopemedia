using app.plataforma.Interfaces;
using App.UseCase.Plataforma.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace App.UseCase.Plataforma.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDocumentosService _DocumentosService;        

        public HomeController(ILogger<HomeController> logger, IDocumentosService DocumentosService)
        {
            _logger = logger;
            _DocumentosService = DocumentosService;
        }

        public IActionResult Index()
        {
            return View();
        }

         public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
