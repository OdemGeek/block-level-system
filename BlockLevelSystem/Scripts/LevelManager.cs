using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ImersiFOX.LevelBlocks
{
    [AddComponentMenu("ImersiFOX/LevelBlocks/Manager")]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [SerializeField] private Transform startPosition;
        [SerializeField] private Parts parts;

        [Space(10)]
        public UnityEvent OnFail = new UnityEvent();
        public UnityEvent OnWin = new UnityEvent();

        private List<LevelBlock> spawnedBlocks = new List<LevelBlock>();
        private int currentLevel = 0;
        private int currentSceneIndex = 0;
        private int blocksOnThisLevel = 1;

        private const string SAVE_NAME = "LevelIndex";

        private GameObject[] objectsToPlaceNow;

        private void Awake()
        {
            instance = this;
        }

        private IEnumerator Start()
        {
            // Wait until the player spawns and only after we spawn the level
            while (PlayerInstance.instance == null) yield return null;
            SpawnLevel();
        }

        public void SpawnLevel()
        {
            if (spawnedBlocks.Count > 0) return; // If the level has already been created, then do not create it again

            currentLevel = PlayerPrefs.GetInt(SAVE_NAME, 0);
            blocksOnThisLevel = Mathf.FloorToInt((currentLevel + 1) / 3f + 1f); // Add +1 block every 3 levels
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // We save the value of which scene is currently enabled to load exactly this scene later

            objectsToPlaceNow = new GameObject[parts.objects.Length];
            parts.objects.CopyTo(objectsToPlaceNow, 0);
            var rng = new System.Random(1531 * currentLevel);
            rng.Shuffle(objectsToPlaceNow);
            for (int i = 0; i < blocksOnThisLevel + 1; i++)
            {
                GameObject newBlock = null;

                if (i % objectsToPlaceNow.Length == 0)
                {
                    rng = new System.Random(1531 * currentLevel + i);
                    rng.Shuffle(objectsToPlaceNow);
                }

                if (spawnedBlocks.Count == blocksOnThisLevel)
                    newBlock = Instantiate(parts.endBlock);
                else
                    newBlock = Instantiate(objectsToPlaceNow[i % objectsToPlaceNow.Length]);

                if (spawnedBlocks.Count > 0)
                    newBlock.transform.position = spawnedBlocks[spawnedBlocks.Count - 1].endCoordinate.position;
                else
                    newBlock.transform.position = startPosition.position;

                spawnedBlocks.Add(newBlock.GetComponent<LevelBlock>());
            }
            StartCoroutine(nameof(CheckPlayerUpdate));
        }

        public static void EndLevel()
        {
            PlayerPrefs.SetInt(SAVE_NAME, PlayerPrefs.GetInt(SAVE_NAME, 0) + 1);
            PlayerPrefs.Save();
            instance.OnWin.Invoke();
            //SceneManager.LoadScene(currentSceneIndex);
        }

        public static void LoadLevel()
        {
            SceneManager.LoadScene(instance.currentSceneIndex);
        }

        // Checking the player every half second
        IEnumerator CheckPlayerUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                CheckPlayerAlive();
            }
        }

        // Check if the player is below the active block
        private void CheckPlayerAlive()
        {
            LevelBlock closestBlock = GetClosestBlock(PlayerInstance.instance.transform.position);
            float minPositionY = Mathf.Min(closestBlock.transform.position.y, closestBlock.endCoordinate.position.y);

            if (PlayerInstance.instance.transform.position.y < minPositionY - 200f)
            {
                OnFail.Invoke();
                //SceneManager.LoadScene(currentSceneIndex);
            }
        }

        // Returns approximately the nearest block to the player
        private LevelBlock GetClosestBlock(Vector3 position)
        {
            LevelBlock closestBlock = null;
            float wasDistanceSqr = float.MaxValue;

            for (int i = 0; i < spawnedBlocks.Count; i++)
            {
                float distanceSqr = (position - spawnedBlocks[i].transform.position).sqrMagnitude;
                if (distanceSqr < wasDistanceSqr)
                {
                    wasDistanceSqr = distanceSqr;
                    closestBlock = spawnedBlocks[i];
                }
                else
                {
                    break;
                }
            }

            return closestBlock;
        }
    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this System.Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
