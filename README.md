# Agents Example Unity

This example showcases how to integrate a voice agent built with [LiveKit Agents](https://docs.livekit.io/agents/overview/) into a Unity project using the [LiveKit Unity SDK](https://github.com/livekit/client-sdk-unity), attaching the agent's voice and transcriptions to a world object.

## Quick start

1. **Setup your agent**: You can follow the [Voice AI Quickstart](https://docs.livekit.io/agents/start/voice-ai/) to build a simple voice agent in less than 10 minutes.

2. **Clone repository**:

```sh
git clone https://github.com/livekit-examples/agents-example-unity.git
```

3. **Add Project to Unity Hub**: From the Unity Hub, click "Add" and select "Add project from disk." Navigate to the cloned repository, and choose the "AgentsExample" subdirectory (this is the Unity project root).

> [!NOTE]
> When opening the project for the first time, it may take a few minutes to resolve the LiveKit SDK package.

4. **Configuration**: With the project open, locate [`Configuration.asset`](/AgentsExample/Assets/Configuration.asset) in the Assets directory. In the Inspector panel, modify the values of `ServerUrl` and `Token` according to your configuration.

5. **Enter Play Mode**: Click the play button at the top of the Unity Editor to start the application.

## Development

This project is configured to work out-of-the-box in Visual Studio Code for development, providing IntelliSense and the ability to attach to the Unity debugger. To use this configuration, open the "AgentsExample" directory in Visual Studio Code. Install the recommended extensions when prompted.

### Debugging

To debug, ensure Unity is running in Play mode, then use the "Attach to Unity" debug configuration.