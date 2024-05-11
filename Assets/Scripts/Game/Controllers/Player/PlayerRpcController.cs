using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Controllers.Player
{
    public partial class PlayerController
    {
        [ServerRpc(RequireOwnership = false)]
        private void KickingServerRpc(NetworkObjectReference ballReference, Kick kickData, bool rotationKick, bool gravityChange)
        {
            if (ballReference.TryGet(out NetworkObject ball))
            {
                Kicking(ball, kickData, true, rotationKick, gravityChange);
            }
        }

        [ClientRpc]
        void ResetObejctClientRpc(Vector3 position)
        {
            transform.position = position;
        }

        [ServerRpc(RequireOwnership = false)]
        void DestroyServerRpc(NetworkObjectReference objectReference)
        {
            if (!objectReference.TryGet(out NetworkObject networkObject))
            {
                Debug.Log("error");
            }
            Destroy(networkObject);
        }
    }
}