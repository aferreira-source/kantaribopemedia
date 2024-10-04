/// <reference path="funcoeshub.js" />
/// <reference path="funcoeshub.js" />
const acceptCall = () => {
    var callingUserName = $('#callmodal').attr('data-cid');
   wsconn.invoke('AnswerCall', true, caller).catch(err => console.error(err));
    $('#divChat').show()
    initiateOffer(caller.connectionId, localstream)
    caller = null;
    $('#callmodal').modal('hide');


};

const declineCall = () => {
    var callingUserName = $('#callmodal').attr('data-cid');
   wsconn.invoke('AnswerCall', false, caller).catch(err => console.error(err));
    caller = null;
    playAudio("MyAudio", 'stop')
    playAudio("AudioReceive", 'stop')

    $('#callmodal').modal('hide');
};

const askUsername = () => {    

    alertify.success("Conex�o realizada com sucesso");
    user = generateId();
    userJoin(user);
    return;

    alertify.prompt('Selecione seu nome', 'Qual seu nome?', '', (evt, Username) => {
        if (Username !== '') {
            user = Username
            userJoin(Username);
        }

        else {
            user = generateId();
            userJoin(user);
        }
         

    }, () => {
        user = generateId();
        userJoin(user);
    });


};


const userJoin = (username) => {
    console.info('Joining...');
   wsconn.invoke("Join", username).catch((err) => {
        console.error(err);
    })
    $("#IdUser").text(username);
};
const limparChat = () => {
    $('#Chatmesseger div.chatHub').remove();
    $('#divChat').hide()

}
const callUser = (connectionId) => {
    /* caller = { "connectionId": connectionId }*/
    playAudio("MyAudio", 'play')
   wsconn.invoke('call', { "connectionId": connectionId });
};
const endCall = (connectionId) => {
   
  
    wsconn.invoke('hangUp');
};

const closeConnection = (partnerClientId) => {
    console.log("WebRTC: called closeConnection ");
    var connection = connections[partnerClientId];

    if (connection) {
        // Let the user know which streams are leaving
        // todo: foreach connection.remoteStreams -> onStreamRemoved(stream.id)
        partnerAudio.srcObject =null

        // Close the connection
        connection.close();
        delete connections[partnerClientId]; // Remove the property
    }
}



const initializeUserMedia = async () => {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        // Informe ao usu�rio que a funcionalidade n�o � suportada em seu navegador
        alert("funcionalidade n�o � suportada em seu navegador")
        return;
    }

    try {
        const stream = await navigator.mediaDevices.getUserMedia({
            audio: true,
            video: true,
        });
        otherAudio.srcObject = stream;
        localstream = stream;

    } catch (err) {
        // Se o usu�rio negar o acesso ao dispositivo de �udio/v�deo, mostre uma mensagem de erro adequada
        console.error('N�o foi poss�vel acessar o dispositivo de �udio/v�deo', err);
        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                audio: true,
                video: false,
            });
            otherAudio.srcObject = stream;
            localstream = stream;
        } catch (err) {
            // Se n�o for poss�vel acessar o dispositivo de �udio, mostre uma mensagem de erro adequada
            console.error('N�o foi poss�vel acessar o dispositivo de �udio', err);
        }
    }
};

function playAudio(id, task) {
    var audio = $("#" + id);
    if (task == 'play') {
        audio.trigger('play');
    }
    if (task == 'stop') {
        audio.trigger('pause');
        audio.prop("currentTime", 0);
    }
}


