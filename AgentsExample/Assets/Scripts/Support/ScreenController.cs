using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AgentsExample
{
    /// <summary>
    /// Controls the on-screen UI.
    /// </summary>
    [RequireComponent(typeof(UIDocument), typeof(Animator))]
    public class ScreenController : MonoBehaviour
    {
        private AudioSource _agentVoiceSource;
        private AudioSpectrumProcessor _spectrumProcessor;

        private Label _transcriptionField;
        private ScrollView _transcriptionScroll;
        private AudioVisualizer _audioVisualizer;

        private Animator _animator;
        private bool _windowOpen = false;

        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;

        /// <summary>
        /// Opens the on-screen window, revealing the agent UI.
        /// </summary>
        public IEnumerator OpenWindow()
        {
            if (_windowOpen) yield break;
            yield return AnimateToState("WindowOpen");
            AudioSource.PlayClipAtPoint(_openSound, transform.position);
            _windowOpen = true;
        }

        /// <summary>
        /// Closes the on-screen window.
        /// </summary>
        public IEnumerator CloseWindow()
        {
            if (!_windowOpen) yield break;
            AudioSource.PlayClipAtPoint(_closeSound, transform.position);
            yield return AnimateToState("WindowClose");
            _windowOpen = false;
        }

        /// <summary>
        /// The audio source to be visualized (the agent's voice).
        /// </summary>
        public AudioSource AgentVoiceSource {
            get => _agentVoiceSource;
            set => _agentVoiceSource = value;
        }

        /// <summary>
        /// Appends a new transcription text to the transcription field.
        /// </summary>
        public void AppendTranscription(string transcription)
        {
            _transcriptionField.text += transcription;
            _transcriptionScroll.scrollOffset = new Vector2(0, _transcriptionScroll.contentContainer.layout.height);
            // TODO: fix issue where text is momentarily clipped on the bottom
        }

        /// <summary>
        /// Clears the transcription field.
        /// </summary>
        public void ClearTranscription()
        {
            _transcriptionField.text = "";
        }

        public void Update()
        {
            if (_agentVoiceSource == null) return;
            _spectrumProcessor.UpdateFrom(_agentVoiceSource);
            _audioVisualizer.Update(_spectrumProcessor.Processed);
        }

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _spectrumProcessor = new AudioSpectrumProcessor(128);

            var root = GetComponent<UIDocument>().rootVisualElement;
            _transcriptionField = root.Q<Label>("TranscriptionField");
            _transcriptionScroll = root.Q<ScrollView>("TranscriptionScroll");
            _audioVisualizer = root.Q<AudioVisualizer>();
        }

        private IEnumerator AnimateToState(string stateName)
        {
            int stateHash = Animator.StringToHash(stateName);
            _animator.CrossFade(stateHash, 0.0f);

            while (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != stateHash)
                yield return null;
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;
        }
    }
}