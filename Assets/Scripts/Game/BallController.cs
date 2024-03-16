using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialUpForce = 15.0f;
    public GameController gameController;
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionFrameCount = 0;
        }
    }

    private PlayerSide CurrentSide
    {
        get { return transform.position.z > 0 ? PlayerSide.Left : PlayerSide.Right; }
    }
}
