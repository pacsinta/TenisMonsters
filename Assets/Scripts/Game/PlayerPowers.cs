public enum PowerEffects
{
    Gravitychange = 0,
    BallRotate = 1,
    SpeedIncrease = 2
}

/* 
 * If the effect is not active, the duration will be 0
 */
struct PlayerPowers
{
    public float GravityPowerDuration;
    public float BallRotationPowerDuration;
    public float SpeedIncreasePowerDuration;

    public float EffectDuration { set; private get; }
    public void SetPower(PowerEffects effect, float duration)
    {
        switch (effect)
        {
            case PowerEffects.Gravitychange:
                GravityPowerDuration = duration;
                break;
            case PowerEffects.BallRotate:
                BallRotationPowerDuration = duration;
                break;
            case PowerEffects.SpeedIncrease:
                SpeedIncreasePowerDuration = duration;
                break;
        }
    }
}

