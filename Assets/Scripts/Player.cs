using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerMovement : NetworkBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    public float speed = 5.0f;
    public float kickForce = 15.0f;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;
    public GameObject ball;

    private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            CickBall();
        }
    }

    void PauseGame()
    {
        Application.Quit();
    }

    protected void CickBall()
    {
        float distance = Vector3.Distance(transform.position, ball.transform.position);
        if (distance < ballDistance)
        {
            Vector3 direction = ball.transform.position - transform.position;
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
