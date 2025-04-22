using UnityEngine;

namespace AgentsExample
{
    [CreateAssetMenu(fileName = "Configuration")]
    public class Configuration : ScriptableObject
    {
        public string ServerUrl;
        public string Token;
    }
}