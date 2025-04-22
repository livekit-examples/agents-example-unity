using UnityEngine;
using System.Collections;
using System;

namespace AgentsExample
{
    public class PermissionManager : MonoBehaviour
    {
        private bool _hasMicrophonePermission {
            get => _hasMicrophonePermission;
            set {
                _hasMicrophonePermission = value;
                PermissionsStateChanged?.Invoke(value);
            }
        }

        public bool PermissionsGranted => _hasMicrophonePermission;
        public Action<bool> PermissionsStateChanged;

        public void ObtainPermissions()
        {
            StartCoroutine(ObtainMicrophonePermission());
        }

        private IEnumerator ObtainMicrophonePermission()
        {
            var level = UserAuthorization.Microphone;
            yield return Application.RequestUserAuthorization(level);
            if (!Application.HasUserAuthorization(level))
            {
                Debug.LogError("Unable to obtain microphone permission");
                _hasMicrophonePermission = false;
                yield break;
            }
            _hasMicrophonePermission = true;
        }
    }
}
