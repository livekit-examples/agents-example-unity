using System;
using UnityEngine.UIElements;

namespace AgentsExample
{
    public class MainMenuController : OverlayViewController
    {
        public event Action TalkRequested;
        public override string Name => "main-menu";

        protected override void Configure()
        {
            RootElement.Q<Button>("Talk").clicked += () => TalkRequested?.Invoke();
        }
    }
}