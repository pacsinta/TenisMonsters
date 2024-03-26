using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerEffects
{
    Gravitychange,
    SpeedIncrease,
    BallRotate
}
public class PowerBallController : MonoBehaviour
{
    public void init(bool gravityBallEnabled, bool speedBallEnabled, bool rotateKickBallEnabled)
    {
        var listOfEffects = new List<PowerEffects>();
        if (gravityBallEnabled) listOfEffects.Add(PowerEffects.Gravitychange);
        if (speedBallEnabled) listOfEffects.Add(PowerEffects.SpeedIncrease);
        if (rotateKickBallEnabled) listOfEffects.Add(PowerEffects.BallRotate);

        powerEffects = listOfEffects[Random.Range(0, listOfEffects.Count)];
    }
    private PowerEffects powerEffects;
    public PowerEffects PowerEffect { get { return powerEffects; } }
}
