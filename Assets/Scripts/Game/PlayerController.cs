using Cinemachine;
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

    public GameObject gameController { set; private get; }

    private Rigidbody rb;
    private Animator animator;
    private PowerEffects powerEffect;

    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera vcam = transform.Find("Virtual Camera").gameObject.GetComponent<CinemachineVirtualCamera>();
        AudioListener audioListener = transform.Find("Camera").gameObject.GetComponent<AudioListener>();
        if (IsOwner)
        {
            vcam.Priority = 1;
            audioListener.enabled = true;
        }
        else
        {
            vcam.Priority = 0;
            audioListener.enabled = false;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        currSpeed = speed;
    }

    private Vector2 kickMouseStartPos = Vector2.zero;
    private float kickMouseStartTime = 0;
    private float currSpeed;
    void Update()
    {
        if (!IsOwner) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * currSpeed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * currSpeed * horizontalInput);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            kickMouseStartPos = Input.mousePosition;
            kickMouseStartTime = Time.realtimeSinceStartup;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2 kickMouseEndPos = Input.mousePosition;
            float kickMouseEndTime = Time.realtimeSinceStartup;

            kickBall(kickMouseEndPos-kickMouseStartPos, (kickMouseEndTime-kickMouseStartTime) * kickForce);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q Pressed");
            kickBall(Vector2.left, kickForce);
        }


        currSpeed = powerEffect == PowerEffects.SpeedIncrease ? speed * 1.5f : speed;

    }

    void ExitGame()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
    }

    private bool kicked = false;
    private void kickBall(Vector2 kickDirection, float kickForce)
    {
        //rb.isKinematic = true;
        animator.SetTrigger("Kick");
        kicked = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedWithObject = collision.gameObject;
        if(kicked && collidedWithObject.CompareTag("Ball") && collision.GetContact(0).thisCollider.gameObject.name == "monster")
        {
            Debug.Log("Contact with: " + collidedWithObject.name);
            animator.enabled = false;
            collidedWithObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collidedWithObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            collidedWithObject.GetComponent<Rigidbody>().AddForce(0, 4, 15, ForceMode.Impulse);
            //collidedWithObject.GetComponent<BallController>().Kicked(IsHost);

            kicked = false;
        }
        else if(collidedWithObject.CompareTag("PowerBall"))
        {
            Destroy(collidedWithObject);
        }
        else if(collidedWithObject.CompareTag("Lava"))
        {
            gameController.GetComponent<GameController>().EndTurn(IsHost);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject collidedWithObject = collision.gameObject;
        if (collidedWithObject.CompareTag("Ball"))
        {
            var distance = Vector3.Distance(transform.position, GetRocketColliderFrontPosition());
            print("Distance: " + distance);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject collidedWithObject = collision.gameObject;
        if (collidedWithObject.CompareTag("Ball"))
        {
            animator.enabled = true;
        }
    }

    private Vector3 GetRocketColliderFrontPosition()
    {
        var monster = transform.Find("monster");
        var rocketCollider = monster.GetComponent<BoxCollider>();

        var rocketCenter = transform.position + monster.position + rocketCollider.center;
        var rocketFront = rocketCenter + monster.forward * rocketCollider.size.z / 2;
        return rocketFront;
    }

    public void ResetObject(Vector3 position)
    {
        transform.position = position;
    }
} 
