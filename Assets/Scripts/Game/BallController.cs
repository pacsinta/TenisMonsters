using Assets.Scripts;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    struct KickData
    {
        private PlayerSide player;
        public PlayerSide Player
        {
            get => player;
            set
            {
                player = value;
                bounced = false;
            }
        }
        public bool bounced; // true if the ball has already bounced on the current turn
    }

    public float initialUpForce = 15.0f;
    public float initialMass = 1.0f;
    public uint groundCollisinMaxTime = 1;
    public GameController gameController;


    private Rigidbody rb;
    private SphereCollider colldider;
    private Vector3 startLocation;
    private KickData kickData;


    void Start()
    {
        colldider = GetComponent<SphereCollider>();
        colldider.enabled = false;

        rb = GetComponent<Rigidbody>();

        startLocation = gameController.PlayerStartPosition;
        startLocation.z = -startLocation.z + 1.2f;
        startLocation.x += 3.0f;

        ResetObject();
    }

    private bool gameStarted = false;
    private float time = 0;
    void Update()
    {
        if(!IsHost) return;

        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        time += Time.deltaTime;
        UpdateBallRotationForces(time);
    }
    private void StartGame()
    {
        colldider.enabled = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
        gameStarted = true;
        kickData.Player = PlayerSide.Host;
        gameController.StartGame();
    }
    public void Kicked(PlayerSide player, bool rotationKick = false)
    {
        kickData.Player = player;
        ResetWeight();
        this.rotationKick = rotationKick;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            print(transform.position);
            CourtSquare location = CourtData.GetCurrentCourtSquare(transform.position);
            CollisionWithGround(location);
        }
        else if (collision.gameObject.CompareTag("Lava"))
        {
            CollisionWithLava();
        }
    }
    private void CollisionWithGround(CourtSquare location)
    {
        if (location == CourtSquare.Out) 
            gameController.EndTurn(~kickData.Player);
        
        if(kickData.Player == PlayerSide.Host && CourtData.IsHostSide(location) ||
           kickData.Player == PlayerSide.Client && CourtData.IsClientSide(location)) 
            gameController.EndTurn(~kickData.Player);

        if (kickData.bounced) 
            gameController.EndTurn(kickData.Player); // If the ball has already bounced, the player who kicked the ball wins

        kickData.bounced = true;
    }
    private void CollisionWithLava()
    {
        gameController.EndTurn(~kickData.Player);
    }

    private float collisionTimeCount = 0;
    private void OnCollisionStay(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionTimeCount += Time.deltaTime;
        }
        if (collisionTimeCount > groundCollisinMaxTime)
        {
            CourtSquare location = CourtData.GetCurrentCourtSquare(transform.position);
            CollisionWithGround(location);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionTimeCount = 0;
        }
    }

    public void DecreaseWeight()
    {
        rb.mass = initialMass * 0.7f;
    }
    public void ResetWeight()
    {
        rb.mass = initialMass;
    }
    private bool rotationKick = false;
    public void UpdateBallRotationForces(float time)
    {
        int updateTime = 2;
        if (rotationKick && time % updateTime == 0)
        {
            float force = time % (updateTime * 2) == 0 ? 1 : -1;
            Vector3 direction = new Vector3(force, 0, 0);

            rb.AddForce(direction, ForceMode.Impulse);
        }
    }

    public void ResetObject()
    {
        transform.position = startLocation;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        colldider.enabled = false;
        gameStarted = false;
        collisionTimeCount = 0;
    }
}
