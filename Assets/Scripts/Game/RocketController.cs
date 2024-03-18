using Unity.Netcode;
using UnityEngine;

public partial class RocketController : NetworkBehaviour
{
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!IsOwner) return;

        Debug.Log("Collision Detected");

        GameObject collidedWithObject = collision.gameObject;
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ballMovement = collidedWithObject.GetComponent<BallController>();
            ballMovement.ResetBounced();
            collidedWithObject.GetComponent<Rigidbody>().AddForce(0, 25, 25, ForceMode.Impulse);
        }
    }
}