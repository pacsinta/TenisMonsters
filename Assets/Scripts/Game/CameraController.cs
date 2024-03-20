using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{

    public CinemachineVirtualCamera vcam;
    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Instantiate(NetworkObject player)
    {   
        var target = player.transform.Find("CameraTarget");
        vcam.Follow = target;
    }
}