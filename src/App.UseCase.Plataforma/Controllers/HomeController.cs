﻿using app.plataforma.Handlers.Hubs;
using App.UseCase.Plataforma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace app.plataforma.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
      
        private readonly IHubContext<MyHub, IConnectionHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<MyHub, IConnectionHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            TempData["success"] = "Sucesso";
            return View("IndexWebRTC");
        }

        //public async Task<IActionResult> Hub(string TargetId,IFormFile arquivo)
        //{
        //   await _hubContext.Clients.Client(TargetId).ReceberArquivo("teste");
        //    return Json(new {sucesso = "suycesso" });
        //}

        public IActionResult IndexAutomatica(string ConectId)
        {
            ViewBag.ConectId = ConectId != null ? ConectId : null; 
            
            
            return View();
        }

        public IActionResult IndexWebRTC(string ConectId)
        {
            ViewBag.ConectId = ConectId != null ? ConectId : null;


            return View();
        }

        public IActionResult IndexSemSocket(string ConectId)
        {
            ViewBag.ConectId = ConectId != null ? ConectId : null;

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