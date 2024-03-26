using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float initialUpForce = 15.0f;
    public float initialMass = 1.0f;
    public GameController gameController;


    private Rigidbody rb;
    private Vector3 startLocation;
    private bool IsHostTurn = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        startLocation = gameController.PlayerStartPosition;
        startLocation.z = -startLocation.z + 1.2f;
        startLocation.x += 3.0f;
        ResetObject();
    }

    private bool firstKick = true;
    void Update()
    {
        if(firstKick && Input.GetKeyDown(KeyCode.Space))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
            firstKick = false;
        }
    }

    private bool bounced = false; // true if the ball has already bounced on the current turn

    /*
     * This function resets the bounce flag to false
     * Should be called when the other player kicks the ball
     */
    public void ResetBounced()
    {
        bounced = false;
    }
    public void Kicked(bool hostKick)
    {
        IsHostTurn = !hostKick;
        ResetBounced();
        resetWeight();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            if(bounced)
            {
                gameController.EndTurn(false);
            }
            else
            {
                bounced = true;
            }
        }
    }

    public void decreaseWeight()
    {
        rb.mass = rb.mass * 0.7f;
    }
    public void resetWeight()
    {
        rb.mass = initialMass;
    }

    private int collisionFrameCount = 0;
    private void OnCollisionStay(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionFrameCount++;
        }
        if(collisionFrameCount > 10)
        {
            gameController.EndTurn(false);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionFrameCount = 0;
        }
    }

    /// <summary>
    /// Gets the current side of the ball based on its position.
    /// </summary>
    /// <returns>The current side of the ball.</returns>
    private PlayerSide CurrentSide
    {
        get { return transform.position.z > 0 ? PlayerSide.Host : PlayerSide.Client; }
    }

    public void ResetObject()
    {
        transform.position = startLocation;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.isKinematic = true;
        ResetBounced();
        firstKick = true;
        collisionFrameCount = 0;
    }

    private bool GetWinner()
    {
        if(CurrentSide == PlayerSide.Host)
        {
            
        }
        else
        {
            if(bounced)
            {
                
            }
        }

        return false;
    }
}
