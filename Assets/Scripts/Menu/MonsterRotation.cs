using UnityEngine;

public class MonsterRotation : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public float rotationAngleLimit = 15.0f;

    private bool rotateRight = true;
    void Update()
    {
        if (rotateRight)
        {
            gameObject.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
            if (gameObject.transform.rotation.eulerAngles.y - 180 > rotationAngleLimit)
            {
                rotateRight = false;
            }
        }
        else
        {
            gameObject.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.down);
            if (gameObject.transform.rotation.eulerAngles.y - 180 < -rotationAngleLimit)
            {
                rotateRight = true;
            }
        }
    }
}
