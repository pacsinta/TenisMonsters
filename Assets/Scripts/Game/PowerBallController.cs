using Unity.Netcode;

public class PowerBallController : NetworkBehaviour
{
    public int powerBallLiveTime = -1;

    private void Update()
    {
        if (!IsHost) return;

        if(powerBallLiveTime != -1)
        {
            if(powerBallLiveTime == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                powerBallLiveTime--;
            }
        }
    }
}