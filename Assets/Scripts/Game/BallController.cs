using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float initialUpForce = 15.0f;
    public GameController gameController;
    // Start is called before the first frame update

    private Rigidbody rb;
    private Vector3 startLocation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        startLocation = transform.position;
    }

    private bool firstKick = true;
    void Update()
    {
        if(firstKick && Input.GetKeyDown(KeyCode.Space))
        {
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
        get { return transform.position.z > 0 ? PlayerSide.Left : PlayerSide.Right; }
    }

    public void Reset()
    {
        transform.position = startLocation;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        ResetBounced();
        firstKick = true;
        collisionFrameCount = 0;
    }
}
