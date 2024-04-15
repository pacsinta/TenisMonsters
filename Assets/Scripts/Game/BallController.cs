using Assets.Scripts;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float initialUpForce = 15.0f;
    public float initialMass = 1.0f;
    public uint groundCollisinMaxTime = 10;
    public GameController gameController;


    private Rigidbody rb;
    private SphereCollider colldider;
    private Vector3 startLocation;
    private PlayerSide currentSide;
    private PlayerSide? playerOfLastKick = null;

    void Start()
    {
        colldider = GetComponent<SphereCollider>();
        colldider.enabled = false;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        startLocation = gameController.PlayerStartPosition;
        startLocation.z = -startLocation.z + 1.2f;
        startLocation.x += 3.0f;
        ResetObject();
    }

    private bool firstKick = true;
    private float time = 0;
    void Update()
    {
        if (firstKick && Input.GetKeyDown(KeyCode.Space))
        {
            colldider.enabled = true;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.up * initialUpForce, ForceMode.Impulse);
            firstKick = false;
            gameController.StartGame();
        }

        if (!IsHost) return;

        var newSide = CurrentSide;
        if (newSide != currentSide)
        {
            currentSide = newSide;
            bounced = false;
        }
        time += Time.deltaTime;

        int updateTime = 2;
        /*if(rotationKick && time % updateTime == 0) 
        {
            float force = time % (updateTime*2) == 0 ? 1 : -1;
            Vector3 direction = new Vector3(force, 0, 0);

            rb.AddForce(direction, ForceMode.Impulse);
        }*/
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
    private bool rotationKick = false;
    public void Kicked(PlayerSide player, bool rotationKick = false)
    {
        ResetBounced();
        resetWeight();
        /*if(playerOfLastKick == player && playerOfLastKick != null && firstKick == false)
        {
            gameController.EndTurn(player == PlayerSide.Host ? PlayerSide.Client : PlayerSide.Client);
        }*/
        playerOfLastKick = player;
        this.rotationKick = rotationKick;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (bounced)
            {
                GroundEnd();
            }
            else
            {
                bounced = true;
            }
        }
        else if (collision.gameObject.CompareTag("Lava"))
        {
            CollisionWithLava();
        }
    }
    private void CollisionWithLava()
    {
        var winner = playerOfLastKick == PlayerSide.Host ? PlayerSide.Client : PlayerSide.Host;
        gameController.EndTurn(winner);
    }
    private void GroundEnd()
    {
        bool clientWon = (currentSide == PlayerSide.Host && playerOfLastKick == PlayerSide.Host) ||
                       (currentSide == PlayerSide.Host && playerOfLastKick == PlayerSide.Client);

        gameController.EndTurn(clientWon ? PlayerSide.Client : PlayerSide.Host);
    }
    public void decreaseWeight()
    {
        rb.mass = initialMass * 0.7f;
    }
    public void resetWeight()
    {
        rb.mass = initialMass;
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
            GroundEnd();
        }
        if(collision.gameObject.CompareTag("Player"))
        {
            colldider.enabled = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            collisionTimeCount = 0;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            colldider.enabled = true;
        }
    }

    /// <summary>
    /// Gets the current side of the ball based on its position.
    /// </summary>
    /// <returns>The current side of the ball.</returns>
    private PlayerSide CurrentSide
    {
        get { return transform.position.z > 0 ? PlayerSide.Client : PlayerSide.Host; }
    }

    public void ResetObject()
    {
        transform.position = startLocation;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        colldider.enabled = false;
        ResetBounced();
        firstKick = true;
        collisionTimeCount = 0;
        playerOfLastKick = null;
    }
}
