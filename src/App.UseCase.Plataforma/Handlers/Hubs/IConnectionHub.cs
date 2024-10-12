﻿using System.Threading.Channels;

namespace app.plataforma.Handlers.Hubs;


public interface IConnectionHub
{
    Task AttUsuariosOnline(List<User> userList);
    Task LigaCaoAceita(User acceptingUser);
    Task LidacaoNegada(User decliningUser, string reason);
    Task ChamadaRecebida(User callingUser);
    Task ReceiveSignal(User signalingUser, string signal);
    Task UploadStream(ChannelReader<string> stream);
    Task LigacaoDesligada(User signalingUser, string signal);
    Task ReceiveDataMsg(User callingUser, string data);
    Task ReceberArquivo(string arquivo, string nomeArquivo);

}