using UnityEngine;
using System.Collections;

namespace AgentsExample
{
    /// <summary>
    /// Simple state machine for the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Dependencies

        [SerializeField] private AgentController _agent;
        [SerializeField] private MainMenuController _mainMenu;
        [SerializeField] private AgentControlsController _controls;
        [SerializeField] private ScreenController _screen;
        [SerializeField] private CameraController _camera;

        #endregion

        #region State management

        private GameState _currentState;
        public enum GameState { Initial, MainMenu, Talk }

        public void Start()
        {
            StartCoroutine(TransitionToState(GameState.MainMenu));
        }

        private IEnumerator TransitionToState(GameState newState)
        {
            Debug.Log($"Transitioning to {newState}");
            if (_currentState == newState)
            {
                Debug.Log($"Already in state {newState}; skipping transition.");
                yield break;
            }

            if (_currentState != GameState.Initial)
                yield return StartCoroutine(ExitState(_currentState));

            _currentState = newState;
            yield return StartCoroutine(EnterState(_currentState));
        }

        private IEnumerator EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu: yield return EnterMainMenu(); break;
                case GameState.Talk: yield return EnterTalk(); break;
            }
        }

        private IEnumerator ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu: yield return ExitMainMenu(); break;
                case GameState.Talk: yield return ExitTalk(); break;
            }
        }
        #endregion

        #region Main menu state

        private IEnumerator EnterMainMenu()
        {
            _mainMenu.TalkRequested += OnTalkRequested;
            _mainMenu.Present();
            yield break;
        }

        private IEnumerator ExitMainMenu()
        {
            _mainMenu.TalkRequested -= OnTalkRequested;
            _mainMenu.Dismiss();
            yield break;
        }

        private void OnTalkRequested()
        {
            StartCoroutine(TransitionToState(GameState.Talk));
        }
        #endregion

        #region Talk state

        private IEnumerator EnterTalk()
        {
            _agent.OnTranscription += OnAgentTranscriptionReceived;

            yield return _camera.ZoomIn();
            yield return _agent.Connect();

            if (!_agent.IsConnected)
            {
                yield return StartCoroutine(TransitionToState(GameState.MainMenu));
                yield break;
            }
            _screen.AgentVoiceSource = _agent.VoiceAudioSource;
            yield return _screen.OpenWindow();

            _controls.ExitRequested += OnExitRequested;
            _controls.MuteRequested += OnMuteRequested;
            _controls.IsMicrophoneMuted = _agent.IsMicrophoneMuted;
            _controls.Present();
        }

        private IEnumerator ExitTalk()
        {
            _agent.OnTranscription -= OnAgentTranscriptionReceived;
            _agent.Disconnect();

            yield return _screen.CloseWindow();
            _screen.AgentVoiceSource = null;

            _controls.ExitRequested -= OnExitRequested;
            _controls.MuteRequested -= OnMuteRequested;
            _controls.Dismiss();

            yield return _camera.ZoomOut();
        }

        private void OnExitRequested()
        {
            StartCoroutine(TransitionToState(GameState.MainMenu));
        }

        private void OnMuteRequested()
        {
            _agent.IsMicrophoneMuted = !_agent.IsMicrophoneMuted;
            _controls.IsMicrophoneMuted = _agent.IsMicrophoneMuted;
        }

        private void OnAgentStateChanged(AgentController.State state)
        {
            // TODO: Handle agent disconnect
        }

        private void OnAgentTranscriptionReceived(AgentController.Transcription transcription)
        {
            if (transcription.IsNewStream)
                _screen.ClearTranscription();
            _screen.AppendTranscription(transcription.Text);
        }

        #endregion
    }
}