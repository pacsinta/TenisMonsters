using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAbilites : MonoBehaviour
{
    public float kickForce = 15.0f;
    public float ballDistance = 2.0f;
    public float jumpForce = 5.0f;
    public GameObject ball;

    private Rigidbody rb;

    private void Start()
    {
        if (ball == null)
        {
            ball = GameObject.Find("Ball");
        }

        rb = GetComponent<Rigidbody>();
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
