using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    // path to character in Prefabs folder
    private string prefabPath;

    private Dictionary<Vector3, string> enemyChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,-4f) ,"Maria" },
        { new Vector3(-1.6f,0f,-2.6f) ,"Maria" },
        { new Vector3(0f,0f,-4f) ,"Maria" },
        { new Vector3(1.6f,0f,-2.6f) ,"Maria" },
        { new Vector3(2.4f,0f,-4f) ,"Maria" }
    };

    private Dictionary<Vector3, string> playerChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,4f) ,"Maria" },
        { new Vector3(-1.6f,0f,2.6f) ,"Maria" },
        { new Vector3(0f,0f,4f) ,"Maria" },
        { new Vector3(1.6f,0f,2.6f) ,"Maria" },
        { new Vector3(2.4f,0f,4f) ,"Maria" }
    };

    private void Start()
    {
        SpawnEnemiesAndHeroes();
    }

    private void SpawnEnemiesAndHeroes()
    {
        // spawn enemies
        foreach (KeyValuePair<Vector3, string> enemyChampion in enemyChampions)
        {
            CreateCharacter(enemyChampion.Key, enemyChampion.Value);
        }

        // spawn player's champions
        foreach (KeyValuePair<Vector3, string> playerChampion in playerChampions)
        {
            CreateCharacter(playerChampion.Key, playerChampion.Value);
        }
    }

    private void CreateCharacter(Vector3 spawnPosition, string characterName)
    {
        prefabPath = $"Prefabs/Characters/{characterName}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }
}
