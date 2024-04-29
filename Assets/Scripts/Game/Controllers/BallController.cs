using Assets.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class BallController : NetworkBehaviour
{
    public float initialUpForce;
    public float initialMass;
    public uint groundCollisinMaxTime;
    public GameController gameController;

    private Rigidbody rb;
    private SphereCollider colldider;
    private Vector3 startLocation;
    private KickData kickData;
    private readonly NetworkVariable<PlayerSide> serveSide = new (PlayerSide.Host);

    readonly NetworkVariable<bool> colliderEnabled = new (false);
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
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space) && Utils.IsMyPlayer(serveSide.Value, IsHost))
        {
            StartGame();
        }

        colldider.enabled = colliderEnabled.Value;

        if (!IsHost) return;

        time += Time.deltaTime;
        UpdateBallRotationForces(time);
    }
    private void StartGame()
    {
        if(IsHost)
        {
            colliderEnabled.Value = true;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
            gameStarted = true;
            kickData.Player = serveSide.Value;
            serveSide.Value = Utils.Swap(serveSide.Value);
            gameController.StartGame();
        }
        else
        {
            StartGameServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        StartGame();
    }
    public void Kicked(PlayerSide player, bool rotationKick = false)
    {
        kickData.firstKickSuccess = true;
        kickData.Player = player;
        ResetWeight();
        rotationKickData.rotationKick = rotationKick;
        print("Roation kick: " + rotationKick);
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
        {
            print("End: out");
            gameController.EndTurn(kickData.bounced ? kickData.Player : Utils.Swap(kickData.Player));
        }
        else if(kickData.Player == PlayerSide.Host && CourtData.IsHostSide(location) ||
                kickData.Player == PlayerSide.Client && CourtData.IsClientSide(location))
        {
            print("End: same side");
            gameController.EndTurn(Utils.Swap(kickData.Player)); // If the player can't kick the ball to the other side, the other player wins
        }
        else if (kickData.bounced)
        {
            print("End: bounced");
            gameController.EndTurn(kickData.Player); // If the ball has already bounced, the player who kicked the ball wins
        }

        kickData.bounced = true;
    }
    private void CollisionWithLava()
    {
        print("End: lava");
        gameController.EndTurn(kickData.bounced ? kickData.Player : Utils.Swap(kickData.Player));
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
        print("Ball weight decreased");
        rb.mass = initialMass * 0.7f;
    }
    private void ResetWeight()
    {
        print("Ball weight reset");
        rb.mass = initialMass;
    }
    private RotationKick rotationKickData;
    public void UpdateBallRotationForces(float time)
    {
        float updateTime = 1.0f;
        if (rotationKickData.rotationKick && time - rotationKickData.rotationKickTime >= updateTime)
        {
            rotationKickData.rotationKickTime = time;
            float force = rotationKickData.rotationKickDirection ? 2 : -2;
            rotationKickData.rotationKickDirection = !rotationKickData.rotationKickDirection;
            Vector3 direction = new (force, 0, 0);

            rb.AddForce(direction, ForceMode.Impulse);
        }
    }

    public void ResetObject()
    {
        transform.position = startLocation;
        if(PlayerSide.Client == serveSide.Value)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, -transform.position.z);
        }
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        gameStarted = false;
        collisionTimeCount = 0;
        kickData.firstKickSuccess = false;
        kickData.Player = serveSide.Value;

        if(IsHost)
        {
            colliderEnabled.Value = false;
        }
    }
}
