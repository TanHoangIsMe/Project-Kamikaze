using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    private string[] playerChampions = { "Maria", "Maria", "Maria", "Maria", "Maria" };
    private string[] enemyChampions = { "Maria", "Maria", "Maria", "Maria", "Maria" };

    private string prefabPath;
 
    private void Start()
    {
        foreach (string enemyChampion in enemyChampions)
        {
            CreateCharacter(enemyChampion);
        }
        foreach (string playerChampion in playerChampions)
        {
            CreateCharacter(playerChampion);
        }
    }

    private void CreateCharacter(string characterName)
    {
        prefabPath = $"Prefabs/Characters/{characterName}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }
}
