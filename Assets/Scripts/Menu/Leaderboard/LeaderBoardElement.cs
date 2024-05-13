namespace Assets.Scripts.Menu.Leaderboard
{
    public struct LeaderBoardElement
    {
        public string Name;
        public int Score;

        public LeaderBoardElement(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public override readonly string ToString()
        {
            return Name + " - " + Score;
        }
    }
}