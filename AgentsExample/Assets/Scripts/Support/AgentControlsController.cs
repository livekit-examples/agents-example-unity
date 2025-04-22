using System;
using UnityEngine.UIElements;

namespace AgentsExample
{
    public class AgentControlsController : OverlayViewController
    {
        public override string Name => "controls";

        public event Action MuteRequested;
        public event Action ExitRequested;

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