using Unity.Netcode;
using UnityEngine;

public class PowerBallController : NetworkBehaviour
{
    public PowerEffects type;
    public float powerBallLiveTime = -1;

    private void Update()
    {
        if (!IsHost) return;

        if (powerBallLiveTime != -1)
        {
            if (powerBallLiveTime == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                powerBallLiveTime -= Time.deltaTime;
            }
        }
    }
}