using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    private string prefabPath; // path to character in Prefabs folder
    private int phase = 0;

    // place to hold all champion that exist on battle field
    private List<GameObject> aliveChampions = new List<GameObject>(); 

    private Dictionary<Vector3, string> enemyChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,-4f) ,"Maria" },
        { new Vector3(-1.6f,0f,-2.6f) ,"UrielAPlotexia" },
        { new Vector3(0f,0f,-4f) ,"UrielAPlotexia" },
        { new Vector3(1.6f,0f,-2.6f) ,"Maria" },
        { new Vector3(2.4f,0f,-4f) ,"Maria" }
    };

    private Dictionary<Vector3, string> playerChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,4f) ,"UrielAPlotexia" },
        { new Vector3(-1.6f,0f,2.6f) ,"Maria" },
        { new Vector3(0f,0f,4f) ,"Maria" },
        { new Vector3(1.6f,0f,2.6f) ,"UrielAPlotexia" },
        { new Vector3(2.4f,0f,4f) ,"UrielAPlotexia" }
    };

    private void Start()
    {
        SpawnEnemiesAndHeroes();
        StartNewPhase();
        SortChampionTurnBySpeed();
    }

    private void StartNewPhase()
    {
        phase++;
    }

    #region SpawnChampions
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
            // create champion
            GameObject champion = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // add to aliveChampion list
            aliveChampions.Add(champion);
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }
    #endregion

    #region SortTheChampionTurn

    private void SortChampionTurnBySpeed()
    {
        foreach (GameObject a in aliveChampions)
        {
            float b = a.GetComponent<OnFieldCharacter>().CurrentSpeed;
            Debug.Log(a + "--" + b);
        }
        Debug.Log("----------------");
        QuickSort(aliveChampions, 0, aliveChampions.Count - 1);
        foreach (GameObject a in aliveChampions)
        {
            float b = a.GetComponent<OnFieldCharacter>().CurrentSpeed;
            Debug.Log(a + "--" + b);
        }
    }

    static void QuickSort(List<GameObject> aliveChampions, int low, int high)
    {
        if (low < high)
        {
            // Partition the list and get the pivot index
            int pivotIndex = Partition(aliveChampions, low, high);

            // Recursively sort elements before and after partition
            QuickSort(aliveChampions, low, pivotIndex - 1);
            QuickSort(aliveChampions, pivotIndex + 1, high);
        }
    }

    static int Partition(List<GameObject> aliveChampions, int low, int high)
    {
        float pivot = aliveChampions[high].GetComponent<OnFieldCharacter>().CurrentSpeed;
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            // Change the comparison for descending order
            if (aliveChampions[j].GetComponent<OnFieldCharacter>().CurrentSpeed > pivot) 
            {
                i++;
                Swap(aliveChampions, i, j);
            }
        }
        Swap(aliveChampions, i + 1, high);
        return i + 1;
    }

    static void Swap(List<GameObject> aliveChampions, int i, int j)
    {
        GameObject temp = aliveChampions[i];
        aliveChampions[i] = aliveChampions[j];
        aliveChampions[j] = temp;
    }
    #endregion
}
