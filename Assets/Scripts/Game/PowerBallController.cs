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
    void Start()
    {
        powerEffects = (PowerEffects)Random.Range(0, 3);
    }

    private PowerEffects powerEffects;
    public PowerEffects PowerEffect { get { return powerEffects; } }
}
