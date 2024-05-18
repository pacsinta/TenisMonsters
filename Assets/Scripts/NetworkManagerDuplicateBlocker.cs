using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class NetworkManagerDuplicateBlocker : MonoBehaviour
    {
        private static NetworkManagerDuplicateBlocker instance;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}