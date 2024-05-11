namespace Assets.Scripts.Game.Controllers.Player
{
    public enum EPowerEffects
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
        public void SetPower(EPowerEffects effect, float duration)
        {
            switch (effect)
            {
                case EPowerEffects.Gravitychange:
                    GravityPowerDuration = duration;
                    break;
                case EPowerEffects.BallRotate:
                    BallRotationPowerDuration = duration;
                    break;
                case EPowerEffects.SpeedIncrease:
                    SpeedIncreasePowerDuration = duration;
                    break;
            }
        }

        public void DecreaseTime(float deltaTime)
        {
            GravityPowerDuration -= deltaTime;
            BallRotationPowerDuration -= deltaTime;
            SpeedIncreasePowerDuration -= deltaTime;
        }
    }

}
