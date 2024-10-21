using System.Threading.Channels;

namespace app.plataforma.Hubs;

public interface IConnectionHubTransmissao
{
    Task AttUsuariosOnline(List<UserAutomatic> userList);
    Task LigaCaoAceita(UserAutomatic acceptingUser);
    Task LidacaoNegada(UserAutomatic decliningUser, string reason);
    Task ChamadaRecebida(UserAutomatic callingUser);
    Task ReceiveSignal(UserAutomatic signalingUser, string signal);
    Task UploadStream(ChannelReader<string> stream);
    Task LigacaoDesligada(UserAutomatic signalingUser, string signal);
    Task ReceiveDataMsg(UserAutomatic callingUser, string data);
    Task ReceberArquivo(string arquivo, string nomeArquivo);
}
