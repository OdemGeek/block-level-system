// Script storing information about blocks for the level
using UnityEngine;

namespace ImersiFOX.LevelBlocks
{
    [CreateAssetMenu(fileName = "Parts", menuName = "LevelBlocks/Parts", order = 1)]
    public class Parts : ScriptableObject
    {
        [SerializeField] private GameObject[] _objects;
        public GameObject[] objects { get => _objects; }

        [SerializeField] private GameObject _endBlock;
        public GameObject endBlock { get => _endBlock; }
    }
}
