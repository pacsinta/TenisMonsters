using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public Camera hostPlayerCamera;
    public Camera clientPlayerCamera;
    public Camera thirdCamera;

    private void Start()
    {
        thirdCamera.enabled = false;
        if (IsHost)
        {
            hostPlayerCamera.enabled = true;
            clientPlayerCamera.enabled = false;
        }
        else
        {
            hostPlayerCamera.enabled = false;
            clientPlayerCamera.enabled = true;
        }
    }

    private void Update()
    {
        thirdCamera.enabled = true;
        clientPlayerCamera.enabled = false;
        hostPlayerCamera.enabled = false;
    }
}