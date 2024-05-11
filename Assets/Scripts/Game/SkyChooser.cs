using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public enum SkyType
    {
        Sunny = 0,
        Cloudy = 1,
        CleanNight = 2,
        CosmicNight = 3,
        RedSun = 4
    }
    public class SkyChooser : NetworkBehaviour
    {
        readonly NetworkVariable<SkyType> skyType = new NetworkVariable<SkyType>(SkyType.Sunny);

        public List<Material> skyMaterials;

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                skyType.Value = (SkyType)PlayerPrefs.GetInt("skyType", 0);
            }
            var skyMaterial = skyMaterials[(int)skyType.Value];
            RenderSettings.skybox = skyMaterial;
        }
    }

}