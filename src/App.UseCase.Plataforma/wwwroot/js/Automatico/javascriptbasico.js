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
    //playAudio("MyAudio", 'stop')
    //playAudio("AudioReceive", 'stop')
    //$('#callmodal').modal('hide');
};

const askUsername = () => {
 
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
    //playAudio("MyAudio", 'play')
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
        // Informe ao usuário que a funcionalidade não é suportada em seu navegador
        alert("funcionalidade não é suportada em seu navegador")
        return;
    } 

    try
    {      
        const stream = await navigator.mediaDevices.getUserMedia({
            audio: true,
            video: true,
        });
        otherAudio.srcObject = stream;
        localstream = stream;

        handleSuccess(stream);

    } catch (err) {
        // Se o usuário negar o acesso ao dispositivo de áudio/vídeo, mostre uma mensagem de erro adequada
        console.error('Não foi possível acessar o dispositivo de áudio/vídeo', err);
        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                audio: true,
                video: false,
            });
            otherAudio.srcObject = stream;
            localstream = stream;
        } catch (err) {
            // Se não for possível acessar o dispositivo de áudio, mostre uma mensagem de erro adequada
            console.error('Não foi possível acessar o dispositivo de áudio', err);
        }
    }
};

function handleSuccess(stream)
{
    recordButton.disabled = false;
    console.log('getUserMedia() got stream:', stream);
    window.stream = stream;

    const video = document.querySelector('video#localVideo');
    video.srcObject = stream;

    getSupportedMimeTypes().forEach(mimeType => {
        const option = document.createElement('option');
        option.value = mimeType;
        option.innerText = option.value;
        codecPreferences.appendChild(option);
    });
    codecPreferences.disabled = false;
}

function playAudio(id, task)
{
    var audio = $("#" + id);
    if (task == 'play') {
        audio.trigger('play');
    }
    if (task == 'stop') {
        audio.trigger('pause');
        audio.prop("currentTime", 0);
    }
}



recordButton.addEventListener('click', () => {

    if (recordButton.textContent === 'Iniciar') {
        startRecording();
    }
    else {
        stopRecording();
        recordButton.textContent = 'Iniciar';
        publicarButton.disabled = false;
        codecPreferences.disabled = false;
    }
});

const publicarButton = document.querySelector('button#publicar');


publicarButton.addEventListener('click', () => {

    const mimeType = "video/mp4";
    const blob = new Blob(recordedBlobs, { type: mimeType });
    let reader = new FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = function () {
        let base64String = reader.result;
        //    wsconn.invoke("publicar", base64String).catch(function (err) {
        //        alert(err.toString());
        //    });  
        wsconn.invoke("publicar", base64String).catch(function (err) {
            alert(err.toString());
        });

    };


    //const url = window.URL.createObjectURL(blob);
    //const a = document.createElement('a');
    //a.style.display = 'none';
    //a.href = url;
    //a.download = mimeType === 'video/mp4' ? 'test.mp4' : 'test.webm';
    //document.body.appendChild(a);
    //a.click();
    //setTimeout(() => {
    //  document.body.removeChild(a);
    //  window.URL.revokeObjectURL(url);
    //}, 100);
});

async function startRecording()
{
    const hasEchoCancellation = document.querySelector('#echoCancellation').checked;  

    recordedBlobs = [];
    const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value;
    const options = { mimeType };
    if (mimeType.split(';', 1)[0] === 'video/mp4') {
        // Adjust sampling rate to 48khz.
        const track = window.stream.getAudioTracks()[0];
        const { sampleRate } = track.getSettings();

        if (sampleRate != 48000) {
            track.stop();
            window.stream.removeTrack(track);
            const newStream = await navigator.mediaDevices.getUserMedia({ audio: { sampleRate: 48000 } });
            window.stream.addTrack(newStream.getTracks()[0]);
        }
    }
    try {
        mediaRecorder = new MediaRecorder(window.stream, options);
    } catch (e) {
        console.error('Exception while creating MediaRecorder:', e);
        errorMsgElement.innerHTML = `Exception while creating MediaRecorder: ${JSON.stringify(e)}`;
        return;
    }

    console.log('Created MediaRecorder', mediaRecorder, 'with options', options);
    recordButton.textContent = 'Parar';
    //playButton.disabled = true;
    publicarButton.disabled = true;
    codecPreferences.disabled = true;
    mediaRecorder.onstop = (event) => {
        console.log('Recorder stopped: ', event);
        console.log('Recorded Blobs: ', recordedBlobs);
    };
    mediaRecorder.ondataavailable = handleDataAvailable;
    mediaRecorder.start();
    console.log('MediaRecorder started', mediaRecorder);
}

function stopRecording() {
    mediaRecorder.stop();
}

function handleDataAvailable(event) {
    console.log('handleDataAvailable', event);
    if (event.data && event.data.size > 0) {
        recordedBlobs.push(event.data);
    }
}

function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=vp8,opus',
        'video/webm;codecs=h264,opus',
        'video/webm;codecs=av01,opus',
        'video/mp4;codecs=h264,aac',
        'video/mp4;codecs=avc1,mp4a.40.2',
        'video/mp4',
    ];
    return possibleTypes.filter(mimeType => {
        return MediaRecorder.isTypeSupported(mimeType);
    });
}