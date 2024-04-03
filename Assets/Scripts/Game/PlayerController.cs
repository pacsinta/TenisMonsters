using Assets.Scripts;
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
    public float initialSpeed = 5.0f;
    public float kickForce = 100.0f;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;
    public float powerDuration = 10.0f;

    public GameObject gameController { set; private get; }
    public AudioSource kickSound { set; private get; }

    private Rigidbody rb;
    private Animator animator;
    private PlayerPowers currentEffects = new();

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
        currSpeed = initialSpeed;
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


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            kickMouseStartPos = Input.mousePosition;
            kickMouseStartTime = Time.realtimeSinceStartup;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2 kickMouseEndPos = Input.mousePosition;
            float kickMouseEndTime = Time.realtimeSinceStartup;

            kickBall(kickMouseStartPos, kickMouseEndPos, kickMouseEndTime, kickMouseStartTime);
        }

        currSpeed = currentEffects.SpeedIncreasePowerDuration > 0 ? initialSpeed * 1.5f : initialSpeed;
    }


    struct Kick
    {
        public float Xdirection;
        public float force;
    }
    private bool kicked = false;
    private Kick kick = new();
    private void kickBall(Vector2 kickMouseStartPos, Vector2 kickMouseEndPos, float kickMouseEndTime, float kickMouseStartTime)
    {
        animator.SetTrigger("Kick");
        kicked = true;
        float mouseDistance = Vector2.Distance(kickMouseStartPos, kickMouseEndPos);
        float mouseTime = kickMouseEndTime - kickMouseStartTime;
        float mouseSpeed = mouseDistance / mouseTime;

        float force = mouseSpeed * kickForce;
        Vector2 kickDirection = kickMouseEndPos - kickMouseStartPos;
        kick = new Kick
        {
            force = force,
            Xdirection = kickDirection.x
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!IsOwner) return;

        GameObject collidedWithObject = collision.gameObject;
        if(kicked && collidedWithObject.CompareTag("Ball") && collision.GetContact(0).thisCollider.gameObject.name == "monster")
        {
            animator.enabled = false;
            collidedWithObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collidedWithObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            collidedWithObject.transform.position += new Vector3(0, 0, 0.5f * transform.forward.z); // move ball a bit forward to avoid animation clipping
            collidedWithObject.GetComponent<Rigidbody>().AddForce(kick.Xdirection / 100, 
                                                                  kick.force / 2, 
                                                                  kick.force, 
                                                                  ForceMode.Impulse);
            kicked = false;
            kick = new (); // reset kick

            BallController ballController = collidedWithObject.GetComponent<BallController>();
            ballController.Kicked(IsHost ? PlayerSide.Host : PlayerSide.Client);

            if (currentEffects.GravityPowerDuration > 0)
            {
                ballController.decreaseWeight();
            }
        }
        else if(collidedWithObject.CompareTag("PowerBall"))
        {
            var power = Variables.Object(collidedWithObject).Get("PowerEffect");
            currentEffects.SetPower((PowerEffects)power, powerDuration);
            Destroy(collidedWithObject);
        }
        else if(collidedWithObject.CompareTag("Lava"))
        {
            gameController.GetComponent<GameController>().EndTurn(IsHost ? PlayerSide.Host : PlayerSide.Client);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsOwner) return;

        GameObject collidedWithObject = collision.gameObject;
        if (collidedWithObject.CompareTag("Ball"))
        {
            animator.enabled = true;
        }
    }

    public void ResetObject(Vector3 position)
    {
        transform.position = position;
    }
} 
