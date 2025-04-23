using System;
using UnityEngine.UIElements;

namespace AgentsExample
{
    public class AgentControlsController : OverlayViewController
    {
        public override string Name => "controls";

        public event Action MuteRequested;
        public event Action ExitRequested;

        public bool IsMicrophoneMuted
        {
            get => _muteButton.ClassListContains("muted");
            set
            {
                if (value)
                    _muteButton.AddToClassList("muted");
                else
                    _muteButton.RemoveFromClassList("muted");
            }
        }

        private Button _muteButton;
        private Button _exitButton;

        protected override void Configure()
        {
            _muteButton = RootElement.Q<Button>("Mute");
            _exitButton = RootElement.Q<Button>("Exit");

            _muteButton.clicked += () => MuteRequested?.Invoke();
            _exitButton.clicked += () => ExitRequested?.Invoke();
        }
    }
}