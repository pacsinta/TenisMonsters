using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class ConnectionHandler : NetworkBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            HandleNotConnected();
        }

        private void HandleNotConnected()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogWarning("Netcode is not initialized");
                SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true);
            }
            else if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogWarning("Player is not connected to the server!");
                SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true);
            }
            else if (IsHost && NetworkManager.Singleton.ConnectedClients.Count != 2 && SceneLoader.GetCurrentScene() == SceneLoader.Scene.GameScene)
            {
                Debug.LogWarning("No opponent is connected");
                SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true);
            }
        }
    }
}