var localstream;
var fileReader
var dataChannel;
var paused = true;
const otherAudio = document.getElementById('gum');
const partnerAudio = document.getElementById('remoteVideo');
const fileTable = document.getElementById('fileTable');
var peerConnectionConfig = { "iceServers": [{ "url": "stun:stun.l.google.com:19302" }] };

let mediaRecorder;
let recordedBlobs;

const codecPreferences = document.querySelector('#codecPreferences');

const errorMsgElement = document.querySelector('span#errorMsg');
const recordedVideo = document.querySelector('video#recorded');
const recordButton = document.querySelector('button#record');




recordButton.addEventListener('click', () => {
  
    if (recordButton.textContent === 'Iniciar')
    {
      startRecording();
    }
    else
    {
      stopRecording();
      recordButton.textContent = 'Iniciar';
      publicarButton.disabled = false;
      codecPreferences.disabled = false;
    }
});

const publicarButton = document.querySelector('button#publicar');


publicarButton.addEventListener('click', () =>
{ 
  
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

async function startRecording()
{ 
    //inicio da live
    const hasEchoCancellation = false;//document.querySelector('#echoCancellation').checked;

    const constraints = {
        audio: {
            echoCancellation: { exact: hasEchoCancellation }
        },
        video: {
            width: { min: 1024, ideal: 1280, max: 1920 },
            height: { min: 576, ideal: 720, max: 1080 },
        }
    };
    await init(constraints); 

      recordedBlobs = [];
      const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value;
      const options = {mimeType};
      if (mimeType.split(';', 1)[0] === 'video/mp4') {
        // Adjust sampling rate to 48khz.
        const track = window.stream.getAudioTracks()[0];
          const { sampleRate } = track.getSettings();

        if (sampleRate != 48000) {
          track.stop();
          window.stream.removeTrack(track);
          const newStream = await navigator.mediaDevices.getUserMedia({audio: {sampleRate: 48000}});
          window.stream.addTrack(newStream.getTracks()[0]);
        }
      }
      try 
      {
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



const userJoin = async () => {
    console.info('Joining...');

    await wsconn.invoke("SetUser").catch((err) => {
        console.error(err);
    });
    //$("#IdUser").text(username);
};

const initializeUserMedia = async () => {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        // Informe ao usuário que a funcionalidade não é suportada em seu navegador
        alert("funcionalidade não é suportada em seu navegador")
        return;
    }

    try {
        const hasEchoCancellation = false;//document.querySelector('#echoCancellation').checked;

        const constraints = {
            audio: {
                echoCancellation: { exact: hasEchoCancellation }
            },
            video: {
                width: { min: 1024, ideal: 1280, max: 1920 },
                height: { min: 576, ideal: 720, max: 1080 },
            }


        };
        const stream = await navigator.mediaDevices.getUserMedia(constraints);

        //const stream = await navigator.mediaDevices.getUserMedia({
        //    audio: true,
        //    video: true,
        //});
        otherAudio.srcObject = stream;
        localstream = stream;

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

function stopRecording() {
  mediaRecorder.stop();
}

function handleSuccess(stream) {
  recordButton.disabled = false;
  console.log('getUserMedia() got stream:', stream);
  window.stream = stream;

  const gumVideo = document.querySelector('video#gum');
  gumVideo.srcObject = stream;

  getSupportedMimeTypes().forEach(mimeType => {
    const option = document.createElement('option');
    option.value = mimeType;
    option.innerText = option.value;
    codecPreferences.appendChild(option);
  });
  codecPreferences.disabled = false;
}

async function init(constraints) {
  try {
    const stream = await navigator.mediaDevices.getUserMedia(constraints);
    handleSuccess(stream);
  } catch (e) {
    console.error('navigator.getUserMedia error:', e);
    errorMsgElement.innerHTML = `navigator.getUserMedia error:${e.toString()}`;
  }
}




