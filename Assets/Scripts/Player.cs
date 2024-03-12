using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerMovement : NetworkBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float speed = 5.0f;
    public float kickForce = 0.01f;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;
    private GameObject ball;

    private Rigidbody rb;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        Debug.Log("Player spawned");
        rb = GetComponent<Rigidbody>();
    }

    private Vector2 kickMouseStartPos = Vector2.zero;
    private float kickMouseStartFrame = 0;
    void Update()
    {
        if (!IsOwner) return;
        if (ball == null)
        {
            Debug.Log("Trying to initialize the ball");
            ball = GameObject.FindWithTag("Ball");
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            kickMouseStartPos = Input.mousePosition;
            kickMouseStartFrame = Time.realtimeSinceStartup;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2 kickMouseEndPos = Input.mousePosition;
            float kickMouseEndFrame = Time.realtimeSinceStartup;

            kickBall(kickMouseEndPos-kickMouseStartPos, (kickMouseEndFrame-kickMouseStartFrame) * kickForce);
        }
    }

    void PauseGame()
    {
        Application.Quit();
    }

    private bool firstkick = true;
    private void kickBall(Vector2 kickDirection, float kickForce)
    {
        float distance = Vector3.Distance(transform.position, ball.transform.position);
        Debug.Log("Distance: " + distance);
        if (distance < ballDistance)
        {
            Vector3 direction = ball.transform.position - transform.position;
            direction = new Vector3(direction.x + kickDirection.x, direction.y, direction.z + kickDirection.y);
            ball.GetComponent<Rigidbody>().AddForce(direction * kickForce, ForceMode.Impulse);
        }
    }

    private bool isOnGround = false; // true if the player is on the ground otherwise false

    protected void Jump()
    {
        if (isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
} 
