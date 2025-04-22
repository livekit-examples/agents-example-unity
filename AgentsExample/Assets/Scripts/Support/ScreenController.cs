using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AgentsExample
{
    [RequireComponent(typeof(UIDocument), typeof(Animator))]
    public class ScreenController : MonoBehaviour
    {
        private Animator _animator;
        private AudioSource _audioSource;
        private AudioSpectrumProcessor _spectrumProcessor;

        private Label _transcriptionField;
        private ScrollView _transcriptionScroll;
        private AudioVisualizer _audioVisualizer;

        public IEnumerator Boot()
        {
            yield return AnimateToState("Boot");
        }

        public IEnumerator OpenWindow()
        {
            yield return AnimateToState("WindowOpen");
        }

        public IEnumerator CloseWindow()
        {
            yield return AnimateToState("WindowClose");
        }

        public bool WindowOpen {
            get => _animator.GetBool("WindowOpen");
            set => _animator.SetBool("WindowOpen", value);
        }

        public AudioSource VisualizerAudioSource {
            get => _audioSource;
            set => _audioSource = value;
        }

        void Start()
        {
            _animator = GetComponent<Animator>();
            _spectrumProcessor = new AudioSpectrumProcessor(128);
        }

        public void Update()
        {
            if (_audioSource == null) return;
            _spectrumProcessor.UpdateFrom(_audioSource);
            _audioVisualizer.Update(_spectrumProcessor.Processed);
        }

        public void AppendTranscription(string transcription)
        {
            _transcriptionField.text += transcription;
            _transcriptionScroll.scrollOffset = new Vector2(0, _transcriptionScroll.contentContainer.layout.height);
            // TODO: fix issue where text is momentarily clipped on the bottom
        }

        public void ClearTranscription()
        {
            _transcriptionField.text = "";
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _transcriptionField = root.Q<Label>("TranscriptionField");
            _transcriptionScroll = root.Q<ScrollView>("TranscriptionScroll");
            _audioVisualizer = root.Q<AudioVisualizer>();
        }

        private IEnumerator AnimateToState(string stateName)
        {
            int stateHash = Animator.StringToHash(stateName);
            _animator.CrossFade(stateHash, 0.25f);

            while (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != stateHash)
                yield return null;
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;
        }
    }
}