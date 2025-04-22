using System.Collections;
using System.Linq;
using UnityEngine;

namespace AgentsExample
{
    public class AudioSpectrumProcessor
    {
        public float[] _raw;
        public float[] _processed;
        private float _maxObserved = 0f;
        private readonly int _resolution;

        public AudioSpectrumProcessor(int resolution)
        {
            Debug.Assert(resolution >= 64 && resolution <= 8192, "Resolution must be >= 64 and <= 8192");
            Debug.Assert((resolution & (resolution - 1)) == 0, "Resolution Must be a power of 2");

            _resolution = resolution;
            _raw = new float[resolution];
            _processed = new float[resolution];
        }

        public float[] Processed => _processed;

        public void UpdateFrom(AudioSource audioSource)
        {
            audioSource.GetSpectrumData(_raw, 0, FFTWindow.BlackmanHarris);
            Update();
        }

        private void Update()
        {
            _maxObserved = Mathf.Max(_raw.Max(), _maxObserved);
            if (_maxObserved == 0) return;

            for (int i = 0; i < _resolution; i++)
                _processed[i] = _raw[i] / _maxObserved;
        }
    }
}