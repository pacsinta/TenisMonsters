using Unity.Netcode;
using UnityEngine;

public partial class PlayerController
{
    [ServerRpc(RequireOwnership = false)]
    private void KickingServerRpc(NetworkObjectReference ballReference, Kick kickData)
    {
        if (ballReference.TryGet(out NetworkObject ball))
        {
            Kicking(ball, kickData, true);
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