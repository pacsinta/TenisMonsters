using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    //public Camera FPVCamera;
    public Camera PlayerBack;
    
    private int currentCameraIndex = 0;
    private void Start()
    {
        //FPVCamera.enabled = true;
        PlayerBack.enabled = true;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Switching camera: " + currentCameraIndex);
            if(currentCameraIndex == 0)
            {
                //FPVCamera.enabled = false;
                PlayerBack.enabled = true;
                currentCameraIndex = 1;
            }else if(currentCameraIndex == 1)
            {
                //FPVCamera.enabled = true;
                PlayerBack.enabled = false;
                currentCameraIndex = 0;
            }
        }
    }
}
