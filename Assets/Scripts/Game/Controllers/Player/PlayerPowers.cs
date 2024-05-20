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
        private float garvityPowerDuration;
        public float GravityPowerDuration 
        {
            readonly get { return garvityPowerDuration; }
            private set { garvityPowerDuration = value < 0 ? 0 : value; }
        }

        private float ballRotationPowerDuration;
        public float BallRotationPowerDuration
        {
            readonly get { return ballRotationPowerDuration; }
            private set { ballRotationPowerDuration = value < 0 ? 0 : value; }
        }

        private float speedIncreasePowerDuration;
        public float SpeedIncreasePowerDuration
        {
            readonly get { return speedIncreasePowerDuration; }
            private set { speedIncreasePowerDuration = value < 0 ? 0 : value; }
        }

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
