// Add to a trigger at the end of a level
using UnityEngine;

namespace ImersiFOX.LevelBlocks
{
    [AddComponentMenu("ImersiFOX/LevelBlocks/EndTrigger")]
    public class LevelEndTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                LevelManager.EndLevel();
        }
    }
}
