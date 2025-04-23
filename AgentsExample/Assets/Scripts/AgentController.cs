using UnityEngine;
using System.Collections;
using LiveKit;
using LiveKit.Proto;
using System;
using Debug = UnityEngine.Debug;

namespace AgentsExample
{
    public class AgentController : MonoBehaviour
    {
        public enum State
        {
            Disconnected,
            Connected,
            Error
        }

        public struct Transcription
        {
            public string Text;
            public bool IsNewStream;
        }

        public Action<State> OnStateChanged;
        public Action<Transcription> OnTranscription;
        public bool IsConnected => _room != null;
        public AudioSource VoiceAudioSource => _agentVoiceOutput;

        /// <summary>
        /// The scriptable object that contains the configuration for connecting to
        /// the LiveKit server.
        /// </summary>
        [SerializeField]
        private Configuration _configuration;

        /// <summary>
        /// The audio source that will play the agent's voice in the scene.
        /// </summary>
        [SerializeField]
        private AudioSource _agentVoiceOutput;

        private Room _room;
        //private State _state = State.Disconnected;

        private AudioSource _microphoneSource;
        private RtcAudioSource _agentVoiceRtcSource;
        private RtcAudioSource _microphoneRtcSource;

        // private State CurrentState
        // {
        //     get => _state;
        //     set
        //     {
        //         _state = value;
        //         OnStateChanged?.Invoke(_state);
        //     }
        // }

        public IEnumerator Connect()
        {
            if (!HasValidConfiguration())
            {
                Debug.LogError("Server URL and token must be set");
                //CurrentState = State.Error;
                yield break;
            }

            Disconnect();
            CreateRoom();
            yield return OpenConnection();
        }

        public void Disconnect()
        {
            if (_room == null) return;
            _room.Disconnect();
            _room = null;
        }

        private void CreateRoom()
        {
            _room = new Room();
            _room.ParticipantConnected += OnParticipantConnected;
            _room.ParticipantMetadataChanged += OnParticipantMetadataChanged;
            _room.TrackSubscribed += OnTrackSubscribed;
            _room.TrackUnsubscribed += OnTrackUnsubscribed;

            _room.RegisterTextStreamHandler(CHAT_TOPIC, OnChatStreamOpened);
            _room.RegisterTextStreamHandler(TRANSCRIPTION_TOPIC, OnTranscriptionStreamOpened);
        }

        private IEnumerator OpenConnection()
        {
            var options = new LiveKit.RoomOptions();
            // Optionally set addition room options before connecting

            Debug.Log($"Connecting to '{_configuration.ServerUrl}'");
            var connect = _room.Connect(_configuration.ServerUrl, _configuration.Token, options);
            yield return connect;

            if (connect.IsError)
            {
                Debug.LogError($"Failed to connect to room");
                //CurrentState = State.Error;
                _room = null;
                yield break;
            }
            //CurrentState = State.Connected;

            yield return PublishMicrophone();
        }

        public IEnumerator PublishMicrophone()
        {
            if (_microphoneSource == null)
                 _microphoneSource = gameObject.AddComponent<AudioSource>();

            var rtcSource = new MicrophoneSource(_microphoneSource);
            rtcSource.Configure(Microphone.devices[0], true, 2, (int)RtcAudioSource.DefaultMirophoneSampleRate);

            var track = LocalAudioTrack.CreateAudioTrack("microphone", rtcSource, _room);

            var options = new TrackPublishOptions();
            options.AudioEncoding = new AudioEncoding();
            options.AudioEncoding.MaxBitrate = 64000;
            options.Source = TrackSource.SourceMicrophone;

            var publish = _room.LocalParticipant.PublishTrack(track, options);
            yield return publish;

            if (publish.IsError)
            {
                Debug.LogError("Failed to published microphone track");
                yield break;
            }

            _microphoneRtcSource = rtcSource;
            yield return rtcSource.PrepareAndStart();
        }

        /// <summary>
        /// Whether the microphone is muted.
        /// </summary>
        public bool IsMicrophoneMuted
        {
            get => _microphoneSource?.mute ?? false;
            set
            {
                if (_microphoneSource == null) return;
                _microphoneSource.mute = value;
            }
        }

        private void OnParticipantConnected(Participant participant)
        {
            if (!IsAgent(participant)) return;
            Debug.Log($"Agent connected");
        }

        private void OnParticipantMetadataChanged(Participant participant)
        {
            if (!IsAgent(participant)) return;
            if (participant.Attributes.ContainsKey(AGENT_STATE_KEY))
            {
                string state = participant.Attributes[AGENT_STATE_KEY];
                Debug.Log($"Agent state: {state}");
                return;
            }
        }

        private void OnTrackSubscribed(IRemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
        {
            Debug.Log($"OnTrackSubscribed: {track.Name} from {participant.Identity}");
            if (!(track is RemoteAudioTrack audioTrack)) return;

            if (_agentVoiceRtcSource != null)
                Debug.LogError("Already subscribed to agent voice");

            if (_agentVoiceOutput == null)
            {
                Debug.LogError("Agent voice output is not set");
                return;
            }
            // TODO: How to reject the track if the audio source is not set?
            var stream = new AudioStream(audioTrack, _agentVoiceOutput);
            _agentVoiceRtcSource = stream.AudioSource;
        }

        private void OnTrackUnsubscribed(IRemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
        {
            Debug.Log($"OnTrackUnsubscribed: {track.Name} from {participant.Identity}");
            if (!(track is RemoteAudioTrack)) return;
            if (_agentVoiceRtcSource == null)
                Debug.LogError("Not subscribed to agent voice");

            _agentVoiceRtcSource.Stop();
            _agentVoiceRtcSource = null;
        }

        private void OnTranscriptionStreamOpened(TextStreamReader reader, string Identity)
        {
            Debug.Log($"Transcription stream opened: {Identity}");
            StartCoroutine(HandleTranscriptionStream(reader));
        }

        private void OnChatStreamOpened(TextStreamReader reader, string Identity)
        {
            Debug.Log($"Chat stream opened: {Identity}");
            StartCoroutine(HandleChatStream(reader));
        }

        private IEnumerator HandleTranscriptionStream(TextStreamReader reader)
        {
            var readIncremental = reader.ReadIncremental();
            var isNewStream = true;

            while (!readIncremental.IsEos)
            {
                readIncremental.Reset();
                yield return readIncremental;

                var transcription = new Transcription
                {
                    Text = readIncremental.Text,
                    IsNewStream = isNewStream
                };
                Debug.Log($"Agent: '{transcription.Text}'");
                OnTranscription?.Invoke(transcription);
                isNewStream = false;
            }
        }

        private IEnumerator HandleChatStream(TextStreamReader reader)
        {
            var readIncremental = reader.ReadIncremental();
            while (!readIncremental.IsEos)
            {
                readIncremental.Reset();
                yield return readIncremental;
                Debug.Log($"User: '{readIncremental.Text}'");
            }
        }

        private static bool IsAgent(Participant participant)
        {
            // TODO: Use participant.Kind once added
            return participant.Name.StartsWith("agent-");
        }

        private bool HasValidConfiguration()
        {
            return _configuration != null &&
                   !string.IsNullOrEmpty(_configuration.ServerUrl) &&
                   !string.IsNullOrEmpty(_configuration.Token);
        }

        private const string AGENT_STATE_KEY = "lk.agent.state";
        private const string CHAT_TOPIC = "lk.chat";
        private const string TRANSCRIPTION_TOPIC = "lk.transcription";
    }
}