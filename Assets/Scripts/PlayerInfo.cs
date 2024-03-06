using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerInfo
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }

        public PlayerInfo()
        {
            LoadPlayerInfo();
        }

        public void StorePlayerInfo()
        {
            PlayerPrefs.SetString("PlayerName", PlayerName);
            PlayerPrefs.SetInt("Score", Score);
        }

        public void LoadPlayerInfo()
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
            Score = PlayerPrefs.GetInt("Score");
        }
    }
}
