

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