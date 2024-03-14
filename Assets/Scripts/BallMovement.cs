using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float initialUpForce = 15.0f;
    // Start is called before the first frame update

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private bool firstKick = true;
    void Update()
    {
        if(firstKick && Input.GetKeyDown(KeyCode.Space))
        {
            rb.useGravity = true;
            rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
            firstKick = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
