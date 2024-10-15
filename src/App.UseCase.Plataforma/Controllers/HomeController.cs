using app.plataforma.Handlers.Hubs;
using app.plataforma.Identity;
using app.plataforma.Interfaces;
using App.UseCase.Plataforma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace app.plataforma.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private UserManager<ApplicationUser> userManager;

    private RoleManager<ApplicationRole> roleManager;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly IPostagensService _postagensService;
    public HomeController(IHttpContextAccessor httpContextAccessor,
        IPostagensService postagensService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<HomeController> logger,
        IHubContext<LiveHub, IConnectionHub> hubContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        this.userManager = userManager;
        this.roleManager = roleManager;
        _postagensService = postagensService;
    }

    public async Task<IActionResult> ObterUsuario()
    {
        var Name = (string)(User.Identity).Name;
        return new OkObjectResult(Name);

        //var userId = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
        //var user = await userManager.FindByIdAsync(userId);
        //User usuario = new User();
        //usuario.Username = user.Email;
        //usuario.Email = user.Email;
        //return new OkObjectResult(usuario);
    }



    //[ResponseCache(NoStore = true, Duration = 3)]
    public async Task<IActionResult> Index()
    {
        var model = await _postagensService.ObterPostagens();
        //await Task.Run(async () => _postagensService.ObterPostagens().Result);
        return View(model);
    }


    [HttpGet("/stream/id")]
    public async Task<IActionResult> Stream(object Id)
    {
        var model = await _postagensService.ObterPostagens();
        //await Task.Run(async () => _postagensService.ObterPostagens().Result);
        return View(model);
    }



    //public async Task<FileStreamResult> ResultStream(ObjectId id)
    //{
    //    var model = await _postagensService.GetFile(id);
    //    return model;
    //}



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