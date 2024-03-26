using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
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

        public override string ToString()
        {
            return Name + " - " + Score;
        }
    }
}