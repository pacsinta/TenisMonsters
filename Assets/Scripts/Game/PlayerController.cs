using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerController : NetworkBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float speed = 5.0f;
    public float kickForce = 100.0f;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;

    private GameObject ball;
    private Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private Vector2 kickMouseStartPos = Vector2.zero;
    private float kickMouseStartFrame = 0;
    void Update()
    {
        if (!IsOwner) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q Pressed");
            kickBall(Vector2.left, kickForce);
        }
    }

    void PauseGame()
    {
        Application.Quit();
    }

    private bool firstkick = true;
    private void kickBall(Vector2 kickDirection, float kickForce)
    {
        animator.SetTrigger("Kick");
    }

    private bool isOnGround = false; // true if the player is on the ground otherwise false

    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.GetContact(0);
        var c = contact.point;
        
        GameObject collidedWithObject = collision.gameObject;
        if (collidedWithObject.CompareTag("Ground"))
        {
            isOnGround = true;
            Debug.Log("Gound Contact");
        }
        else if(collidedWithObject.CompareTag("Ball"))
        {
            BallController ballMovement = collidedWithObject.GetComponent<BallController>();
            ballMovement.ResetBounced();
            collidedWithObject.GetComponent<Rigidbody>().AddForce(0,300,800);
        }
        else
        {
            Debug.Log("Contact with: " + collidedWithObject.name);
        }

    }
} 
