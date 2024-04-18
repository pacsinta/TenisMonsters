using Assets.Scripts;
using Cinemachine;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController : NetworkBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float initialSpeed = 5.0f;
    public float kickForce;
    public float maxForce;
    public float minForce;
    public float sideKickForce;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;
    public float powerDuration = 30.0f;

    public GameObject Environment { set; private get; }
    public AudioSource kickSource;

    private Scrollbar gravityTime;
    private Scrollbar speedTime;
    private Scrollbar rotationTime;

    private Rigidbody rb;
    private Animator animator;
    private PlayerPowers currentEffects = new();

    struct Kick
    {
        public float XdirectionForce;
        public float force;
    }
    private Kick kick = new();

    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera vcam = transform.Find("Virtual Camera").gameObject.GetComponent<CinemachineVirtualCamera>();
        AudioListener audioListener = transform.Find("Camera").gameObject.GetComponent<AudioListener>();
        if (IsOwner)
        {
            vcam.Priority = 1;
            audioListener.enabled = true;
            kickSource.volume = PlayerPrefs.GetFloat("volume", 1);
        }
        else
        {
            vcam.Priority = 0;
            audioListener.enabled = false;
        }

        if((IsHost && IsOwner) || (!IsHost && !IsOwner))
        {
            gameObject.name = "HostPlayer";
        }
        else if ((IsHost && !IsOwner) || (!IsHost && IsOwner))
        {
            gameObject.name = "ClientPlayer";
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

        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            kickMouseStartPos = Input.mousePosition;
            kickMouseStartTime = Time.realtimeSinceStartup;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2 kickMouseEndPos = Input.mousePosition;
            float kickMouseEndTime = Time.realtimeSinceStartup;

            DetermineKickForce(kickMouseStartPos, kickMouseEndPos, kickMouseEndTime, kickMouseStartTime);
        }

        if (GetPowerBallTimerInstances())
        {
            UpdatePowerBallTimers();
        }

        currSpeed = currentEffects.SpeedIncreasePowerDuration > 0 ? initialSpeed * 1.5f : initialSpeed;
        currentEffects.DecreaseTime(Time.deltaTime);
    }

    private bool GetPowerBallTimerInstances()
    {
        if (gravityTime == null)
        {
            gravityTime = GameObject.Find("GravityTime")?.GetComponent<Scrollbar>();
            if(gravityTime != null) gravityTime.size = 0;
            else return false;
        }
        if (speedTime == null)
        {
            speedTime = GameObject.Find("SpeedTime")?.GetComponent<Scrollbar>();
            if(speedTime != null) speedTime.size = 0;
            else return false;
        }
        if (rotationTime == null)
        {
            rotationTime = GameObject.Find("RotationTime")?.GetComponent<Scrollbar>();
            if(rotationTime != null) rotationTime.size = 0;
            else return false;
        }
        return true;
    }
    private void UpdatePowerBallTimers()
    {
        speedTime.size = currentEffects.SpeedIncreasePowerDuration / powerDuration;
        gravityTime.size = currentEffects.GravityPowerDuration / powerDuration;
        rotationTime.size = currentEffects.BallRotationPowerDuration / powerDuration;
    }
    private void MovePlayer()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("Running", true);
            transform.Translate(Vector3.forward * Time.deltaTime * currSpeed * verticalInput);
            transform.Translate(Vector3.right * Time.deltaTime * currSpeed * horizontalInput);
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }

    private void DetermineKickForce(Vector2 kickMouseStartPos, Vector2 kickMouseEndPos, float kickMouseEndTime, float kickMouseStartTime)
    {
        animator.SetTrigger("Kick");
        float mouseTime = kickMouseEndTime - kickMouseStartTime;

        Vector2 kickDirection = kickMouseEndPos - kickMouseStartPos;
        kickDirection.Normalize();

        kick = new Kick
        {
            force = Math.Clamp(mouseTime * kickForce, minForce, maxForce),
            XdirectionForce = kickDirection.x * sideKickForce
        };
        print("Kick with force: " + kick.force + " and direction: " + kick.XdirectionForce);
    }
    private void PowerBallCollision(GameObject PowerBall)
    {
        var power = PowerBall.GetComponent<PowerBallController>().type;
        currentEffects.SetPower(power, powerDuration);
        if (IsHost)
        {
            Destroy(PowerBall);
        }
        else
        {
            DestroyServerRpc(PowerBall);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
         if (!IsOwner) return;

        GameObject collidedWithObject = collision.gameObject;
        if (collidedWithObject.CompareTag("Ball") && collision.GetContact(0).thisCollider.gameObject.name == "monster")
        {
            print("Kicked the ball");
            animator.enabled = false;
            kickSource.Play();

            collidedWithObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collidedWithObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            collidedWithObject.transform.position += new Vector3(0, 0, 0.5f * transform.forward.z); // move ball a bit forward to avoid animation clipping
            collidedWithObject.GetComponent<Rigidbody>().AddForce(kick.XdirectionForce / 100,
                                                                  kick.force / 2,
                                                                  kick.force,
                                                                  ForceMode.Impulse);
            kick = new(); // reset kick

            BallController ballController = collidedWithObject.GetComponent<BallController>();
            ballController.Kicked(IsHost ? PlayerSide.Host : PlayerSide.Client);

            if (currentEffects.GravityPowerDuration > 0)
            {
                ballController.DecreaseWeight();
            }
        }
        else if (collidedWithObject.CompareTag("PowerBall"))
        {
            PowerBallCollision(collidedWithObject);
        }
        else if (collidedWithObject.CompareTag("Lava"))
        {
            Environment.GetComponent<GameController>().EndTurn(IsHost ? PlayerSide.Client : PlayerSide.Host);
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
        if(!IsHost) return;
        if(IsOwner)
        {
            transform.position = position;
        }
        else
        {
            ResetObejctClientRpc(position);
        }
    }

    [ClientRpc]
    void ResetObejctClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyServerRpc(NetworkObjectReference objectReference)
    {
        if (!objectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("error");
        }
        Destroy(networkObject);
    }
}
