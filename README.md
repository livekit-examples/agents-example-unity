# Agents Example Unity

This example demonstrates building a frontend for a voice  agent built with [LiveKit Agents](https://docs.livekit.io/agents/overview/) using the [LiveKit Unity SDK](https://github.com/livekit/client-sdk-unity).

## Quick start

1. **Clone repository**:

```sh
git clone https://github.com/livekit-examples/agents-example-unity.git
```

2. **Add project to Unity Hub**: From the Unity Hub, click "Add" and select "Add project from disk." Navigate to the cloned repository, and choose the "AgentsExample" subdirectory (this is the Unity project root).

3. **Set configuration**: With the project open, locate [`Configuration.asset`](/AgentsExample/Assets/Configuration.asset) in the Assets directory. In the Inspector panel, modify the values of `ServerUrl` and `Token` according to your configuration.

4. **Enter Play Mode**: Click the play button at the top of the Unity Editor to start the application.

## Development

This project is configured to work out-of-the-box in Visual Studio Code for development, providing IntelliSense and the ability to attach to the Unity debugger. To use this configuration, open the "AgentsExample" directory in Visual Studio Code. Install the recommended extensions when prompted.

### Debugging

To debug, ensure Unity is running in Play mode, then use the "Attach to Unity" debug configuration.