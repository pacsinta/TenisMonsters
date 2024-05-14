using Cinemachine;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Controllers.Player
{
    public partial class PlayerController : NetworkBehaviour
    {
        private float horizontalInput;
        private float verticalInput;
        public float initialSpeed = 5.0f;
        public float ballDistance = 2.0f;
        public float jumpForce = 5.0f;
        public float powerDuration = 30.0f;

        public GameObject Environment { set; private get; }
        public AudioSource kickSource;
        public AudioSource effectSource;
        public Material racketMaterial;
        public GameObject racket;

        private Scrollbar gravityTime;
        private Scrollbar speedTime;
        private Scrollbar rotationTime;

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
                kickSource.volume = PlayerPrefs.GetFloat("volume", 1);
                effectSource.volume = PlayerPrefs.GetFloat("volume", 1);
            }
            else
            {
                vcam.Priority = 0;
                audioListener.enabled = false;
            }

            if ((IsHost && IsOwner) || (!IsHost && !IsOwner))
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
            Debug.Assert(minForce < maxForce, "minForce must be less than maxForce");
            animator = GetComponentInChildren<Animator>();
            currSpeed = initialSpeed;
        }

        private Vector2 kickMouseStartPos = Vector2.zero;
        private float kickMouseStartTime = 0;
        private float currSpeed;
        void Update()
        {
            if (!IsOwner && IsSpawned) return;

            MovePlayer();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Store the mouse position and time when the kick started
                kickMouseStartPos = Input.mousePosition;
                kickMouseStartTime = Time.realtimeSinceStartup;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                StartKick();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                // Update the kick force and color of the racket, while the mouse is held down
                Vector2 kickMouseEndPos = Input.mousePosition;
                float kickMouseEndTime = Time.realtimeSinceStartup;

                DetermineKickForce(kickMouseStartPos, kickMouseEndPos, kickMouseEndTime, kickMouseStartTime);
                SetRacketColorByKickForce();
            }
            if (wasInKickState && animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAnimation"))
            {
                // If the kick animation has ended, but the ball was not kicked, reset the kick
                EndKick();
            }

            if (GetPowerBallTimerInstances())
            {
                UpdatePowerBallTimers();
            }

            currSpeed = currentEffects.SpeedIncreasePowerDuration > 0 ? initialSpeed * 1.5f : initialSpeed;
            currentEffects.DecreaseTime(Time.deltaTime);

            wasInKickState = animator.GetCurrentAnimatorStateInfo(0).IsName("KickAnimation");
        }

        private bool GetPowerBallTimerInstances()
        {
            if (gravityTime == null)
            {
                try { gravityTime = GameObject.Find("GravityTime").GetComponent<Scrollbar>(); }
                catch (NullReferenceException) { return false; }
                if (gravityTime != null) gravityTime.size = 0;
                else return false;
            }
            if (speedTime == null)
            {
                try { speedTime = GameObject.Find("SpeedTime").GetComponent<Scrollbar>(); }
                catch (NullReferenceException) { return false; }
                if (speedTime != null) speedTime.size = 0;
                else return false;
            }
            if (rotationTime == null)
            {
                try { rotationTime = GameObject.Find("RotationTime").GetComponent<Scrollbar>(); }
                catch (NullReferenceException) { return false; }
                if (rotationTime != null) rotationTime.size = 0;
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
                transform.Translate(currSpeed * Time.deltaTime * verticalInput * Vector3.forward);
                transform.Translate(currSpeed * horizontalInput * Time.deltaTime * Vector3.right);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }

        private void PowerBallCollision(GameObject PowerBall)
        {
            var power = PowerBall.GetComponent<PowerBallController>().type;
            currentEffects.SetPower(power, powerDuration);
            effectSource.Play();

            Utils.RunOnTarget((powerBall) => { Destroy(powerBall); },
                              (powerBall) => { DestroyServerRpc(powerBall); },
                              PowerBall, IsHost);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsOwner) return;

            GameObject collidedWithObject = collision.gameObject;
            if (collidedWithObject.CompareTag("PowerBall"))
            {
                PowerBallCollision(collidedWithObject);
            }
            else if (collidedWithObject.CompareTag("Lava"))
            {
                Environment.GetComponent<GameController>().EndTurn(IsHost ? EPlayerSide.Client : EPlayerSide.Host);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner) return;

            GameObject collidedWithObject = other.gameObject;
            if (collidedWithObject.CompareTag("Ball"))
            {
                print("Kick with force: " + kick.force + " and direction: " + kick.XdirectionForce);
                kickSource.Play();

                Kicking(collidedWithObject.GetComponent<NetworkObject>(), kick);
                EndKick();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsOwner) return;

            GameObject collidedWithObject = other.gameObject;
            if (collidedWithObject.CompareTag("Ball"))
            {
                animator.enabled = true;
            }
        }

        public void ResetObject(Vector3 position)
        {
            if (!IsHost) return;

            Utils.RunOnTarget((pos) => { transform.position = pos; }, ResetObejctClientRpc, position, IsOwner);
        }
    }
}
