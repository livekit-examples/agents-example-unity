using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AgentsExample
{
    public class TokenService : MonoBehaviour
    {
        [SerializeField] private AuthConfig _config;

        private static readonly string SandboxUrl = "https://cloud-api.livekit.io/api/sandbox/connection-details";
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<ConnectionDetails> FetchConnectionDetails(string roomName, string participantName)
        {
            if (_config == null)
                throw new InvalidOperationException("Auth configuration was not provided");
            if (!_config.IsValid)
                throw new InvalidOperationException("Auth configuration is invalid");

            if (_config is SandboxAuth sandboxConfig)
                return await FetchConnectionDetailsFromSandbox(roomName, participantName, sandboxConfig.SandboxId);

            if (_config is HardcodedAuth hardcodedConfig)
                return new ConnectionDetails
                {
                    ServerUrl = hardcodedConfig.ServerUrl,
                    RoomName = roomName,
                    ParticipantName = participantName,
                    ParticipantToken = hardcodedConfig.Token
                };

            throw new InvalidOperationException("Unknown auth type");
        }

        private async Task<ConnectionDetails> FetchConnectionDetailsFromSandbox(string roomName, string participantName, string sandboxId)
        {
            var uriBuilder = new UriBuilder(SandboxUrl);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["roomName"] = roomName;
            query["participantName"] = participantName;
            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
            request.Headers.Add("X-Sandbox-ID", sandboxId);

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error from LiveKit Cloud sandbox: {response.StatusCode}, response: {response}");

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ConnectionDetails>(jsonContent);
        }
    }

    [Serializable]
    public struct ConnectionDetails
    {
        public string ServerUrl { get; init; }
        public string RoomName { get; init; }
        public string ParticipantName { get; init; }
        public string ParticipantToken { get; init; }
    }

    public abstract class AuthConfig : ScriptableObject
    {
        public abstract bool IsValid { get; }
    }

    [CreateAssetMenu(fileName = "TokenService", menuName = "LiveKit/Sandbox Auth")]
    public class SandboxAuth : AuthConfig
    {
        [SerializeField] private string _sandboxId;

        public string SandboxId => _sandboxId?.Trim('"');

        public override bool IsValid =>
            !string.IsNullOrEmpty(SandboxId);
    }

    [CreateAssetMenu(fileName = "TokenService", menuName = "LiveKit/Hardcoded Auth")]
    public class HardcodedAuth : AuthConfig
    {
        [SerializeField] private string _serverUrl;
        [SerializeField] private string _token;

        public string ServerUrl => _serverUrl;
        public string Token => _token;

        public override bool IsValid =>
            !string.IsNullOrEmpty(ServerUrl) && ServerUrl.StartsWith("ws") &&
            !string.IsNullOrEmpty(Token);
    }
}