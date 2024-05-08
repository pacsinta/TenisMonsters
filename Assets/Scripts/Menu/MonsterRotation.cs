using UnityEngine;

public class MonsterRotation : MonoBehaviour
{
    public GameObject monster;
    public float rotationSpeed = 10.0f;
    public float rotationAngleLimit = 15.0f;

    private bool rotateRight = true;
    void Update()
    {
        if (rotateRight)
        {
            monster.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
            if (monster.transform.rotation.eulerAngles.y - 180 >= rotationAngleLimit)
            {
                rotateRight = false;
            }
        }
        else
        {
            monster.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.down);
            if (monster.transform.rotation.eulerAngles.y - 180 < -rotationAngleLimit)
            {
                rotateRight = true;
            }
        }
    }
}
