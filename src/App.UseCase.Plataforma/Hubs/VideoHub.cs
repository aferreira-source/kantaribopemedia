using app.plataforma.Identity;
using app.plataforma.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.Security.Claims;
using System.Threading.Channels;

namespace app.plataforma.Hubs;

public class VideoHub : Hub<IConnectionHub>
{
    private readonly List<User> _users;
    private readonly List<Connection> _connections;
    private readonly List<Call> _calls;
    protected readonly IPostagensService _postagensService;

    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;


    public VideoHub(List<User> users,
                   List<Connection> connections,
                   List<Call> calls,
                   IPostagensService postagensService,
                   UserManager<ApplicationUser> userManager,
                   IHttpContextAccessor httpContextAccessor
     )
    {
        _users = users;
        _connections = connections;
        _calls = calls;
        _postagensService = postagensService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task Join(string username)
    {
        var currentUserName = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var ConnectionId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var email = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;

        _users.Add(new User
        {
            Username = currentUserName,
            ConnectionId = Context.ConnectionId,
            Email = email,
        });

        await UpdateOnlineUsers();
    }


    public async Task SetUser()
    {
        var currentUserName = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var ConnectionId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var email = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;


        _users.Add(new User
        {
            Username = currentUserName,
            ConnectionId = Context.ConnectionId,
            Email = email,
        });

        await UpdateOnlineUsers();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await HangUp();

        _users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

        await UpdateOnlineUsers();

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Call(User targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);

        if (targetUser == null)
        {
            await Clients.Caller.LidacaoNegada(targetConnectionId, "Usuario nao existe.");
            return;
        }

        // Check connection
        if (GetConnection(targetUser.ConnectionId) != null)
        {
            await Clients.Caller.LidacaoNegada(targetConnectionId, string.Format("{0} Pessoa ja está em uma ligação.", targetUser.Username));
            return;
        }

        await Clients.Client(targetConnectionId.ConnectionId).ChamadaRecebida(callingUser);

        _calls.Add(new Call
        {
            From = callingUser,
            To = targetUser,
            CallStartTime = DateTime.Now
        });
    }


    public async Task GetUsuario(User targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);




        if (targetUser == null)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, "Usuario Desligou");
            return;
        }



        var callCount = _calls.RemoveAll(c => c.To.ConnectionId == callingUser.ConnectionId && c.From.ConnectionId == targetUser.ConnectionId);
        if (callCount < 1)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, $"{targetUser.Username} Ligacao desligada.");
            return;
        }

        // Check if user is in another call
        if (GetConnection(targetUser.ConnectionId) != null)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, $"{targetUser.Username} Em outra ligação.");
            return;
        }

        // Remove all the other offers for the call initiator, in case they have multiple calls out
        _calls.RemoveAll(c => c.From.ConnectionId == targetUser.ConnectionId);

        _connections.Add(new Connection
        {
            Users = new List<User> { callingUser, targetUser }
        });
        await Clients.Caller.LigaCaoAceita(targetUser);
        await Clients.Client(targetConnectionId.ConnectionId).LigaCaoAceita(callingUser);

        await UpdateOnlineUsers();
    }


    public async Task AnswerCall(bool acceptCall, User targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);




        if (targetUser == null)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, "Usuario Desligou");
            return;
        }

        if (!acceptCall)
        {

            await Clients.Client(targetConnectionId.ConnectionId).LidacaoNegada(callingUser, $"{callingUser.Username} Não pode aceitar a sua ligação.");
            return;
        }

        var callCount = _calls.RemoveAll(c => c.To.ConnectionId == callingUser.ConnectionId && c.From.ConnectionId == targetUser.ConnectionId);
        if (callCount < 1)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, $"{targetUser.Username} Ligacao desligada.");
            return;
        }

        // Check if user is in another call
        if (GetConnection(targetUser.ConnectionId) != null)
        {
            await Clients.Caller.LigacaoDesligada(targetConnectionId, $"{targetUser.Username} Em outra ligação.");
            return;
        }

        // Remove all the other offers for the call initiator, in case they have multiple calls out
        _calls.RemoveAll(c => c.From.ConnectionId == targetUser.ConnectionId);

        _connections.Add(new Connection
        {
            Users = new List<User> { callingUser, targetUser }
        });
        await Clients.Caller.LigaCaoAceita(targetUser);
        await Clients.Client(targetConnectionId.ConnectionId).LigaCaoAceita(callingUser);

        await UpdateOnlineUsers();
    }

    public async Task HangUp()
    {

        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

        if (callingUser == null)
        {
            return;
        }

        var currentCall = GetConnection(callingUser.ConnectionId);

        // Send a hang up message to each user in the call, if there is one
        if (currentCall != null)
        {
            foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
            {
                await Clients.Caller.LigacaoDesligada(callingUser, $"{callingUser.Username} Desligando ai machoo");
                await Clients.Client(user.ConnectionId).LigacaoDesligada(callingUser, $"{callingUser.Username} has hung up.");
            }

            currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
            if (currentCall.Users.Count < 2)
            {
                _connections.Remove(currentCall);
            }
        }

        _calls.RemoveAll(c => c.From.ConnectionId == callingUser.ConnectionId);

        await UpdateOnlineUsers();
    }

    public async Task sendSignal(string data, string targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

        if (callingUser == null || targetUser == null)
        {
            return;
        }


        //Check the connection 
        var userCall = GetConnection(callingUser.ConnectionId);
        if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
        {
            await Clients.Client(targetUser.ConnectionId).ReceiveSignal(callingUser, data);
            // await Clients.Caller.ReceiveData(callingUser, data);
        }
    }

    public async Task SendDataMsg(string data, string targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

        if (callingUser == null || targetUser == null)
        {
            return;
        }


        //Check the connection 
        var userCall = GetConnection(callingUser.ConnectionId);
        if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
        {
            await Clients.Client(targetConnectionId).ReceiveDataMsg(callingUser, data);
        }
    }

    public async Task UploadStream(ChannelReader<string> stream, string targetConnectionId)
    {
        var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

        while (await stream.WaitToReadAsync())
        {
            while (stream.TryRead(out var item))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var dataStream = item.Split('|');
                    if (!string.IsNullOrEmpty(dataStream[0]))
                    {
                        var connectionId = dataStream[0].Trim().TrimStart('\b');
                        var targetUser = _users.SingleOrDefault(u => u.ConnectionId == connectionId);
                        if (targetUser != null)
                        {
                            await Clients.Client(targetUser.ConnectionId).ReceiveSignal(callingUser, dataStream[1]);
                        }
                    }

                }


            }
        }
    }

    public ChannelReader<string> DownloadStream(int delay, CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();
        _ = WriteItemsAsync(channel.Writer, DateTime.Now.Millisecond.ToString(), delay, cancellationToken);

        return channel.Reader;
    }

    private static async Task WriteItemsAsync(ChannelWriter<string> writer, string data, int delay, CancellationToken cancellationToken)
    {
        Exception localException = null;
        try
        {
            await writer.WriteAsync(data, cancellationToken);
            await Task.Delay(delay, cancellationToken);

        }
        catch (Exception ex)
        {
            localException = ex;
        }

        writer.Complete(localException);
    }

    private async Task UpdateOnlineUsers()
    {
        _users.ForEach(u => u.InCall = GetConnection(u.ConnectionId) != null);
        await Clients.All.AttUsuariosOnline(_users);
    }

    private Connection GetConnection(string connectionId)
    {
        var matchingCall = _connections.FirstOrDefault(uc => uc.Users.FirstOrDefault(u => u.ConnectionId == connectionId) != null);
        return matchingCall;
    }
    [HubMethodName("enviararquivo")]
    public async Task EnviarArquivo(byte[] fileData)
    {

    }
    public async Task Publicar(object video)
    {
        //var totalCaracters = video.Length;
        var x = video;


        var postagem = new Postagens()
        {
            usuarioid = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,

            usuario = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value,
            titulo = "Primeiro video",
            descricao = "Descrição bablablbalbalbalblalbaaaaaaaaaaalballllllllllllllll",
            fileBlob = video,
            dtHora_Publicacao = DateTime.Now,
            tipo = 1
        };

        await _postagensService.InserirAsync(postagem);
    }
}
