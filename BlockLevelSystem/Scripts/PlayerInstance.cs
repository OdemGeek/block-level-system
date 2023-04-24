// The script is on the player, this script helps the manager knows where the player is
using UnityEngine;

namespace ImersiFOX.LevelBlocks
{
    [AddComponentMenu("ImersiFOX/LevelBlocks/PlayerInstance")]
    public class PlayerInstance : MonoBehaviour
    {
        public static PlayerInstance instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
