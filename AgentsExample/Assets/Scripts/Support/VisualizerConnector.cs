using UnityEngine;
using UnityEngine.UIElements;

namespace AgentsExample
{
    [RequireComponent(typeof(AudioSource))]
    public class VisualizerConnector : MonoBehaviour
    {
        private AudioSource _audioSource;

        [SerializeField]
        private VisualElement _visualizer;

        private float[] _rawSamples = new float[64];
        public float[] _binnedSamples = new float[32];

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (!_audioSource.isPlaying)
                return;

            _audioSource.GetSpectrumData(_rawSamples, 0, FFTWindow.BlackmanHarris);
            BinSamples();
        }

        private void BinSamples()
        {
            // Calculate how many raw samples will be averaged into each bin
            float samplesPerBin = (float)_rawSamples.Length / _binnedSamples.Length;

            // Process each bin
            for (int binIndex = 0; binIndex < _binnedSamples.Length; binIndex++)
            {
                // Calculate the start and end indices for this bin
                int startSample = Mathf.FloorToInt(binIndex * samplesPerBin);
                int endSample = Mathf.FloorToInt((binIndex + 1) * samplesPerBin);

                // Ensure we don't go out of bounds
                endSample = Mathf.Min(endSample, _rawSamples.Length);

                // Calculate the average value for this bin
                float sum = 0f;
                for (int i = startSample; i < endSample; i++)
                    sum += _rawSamples[i];

                // Store the average in the binned samples array
                if (endSample > startSample)
                    _binnedSamples[binIndex] = sum / (endSample - startSample);
            }
        }
    }
}