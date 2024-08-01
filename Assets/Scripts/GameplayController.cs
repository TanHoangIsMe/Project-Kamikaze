using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    private string prefabPath; // path to character in Prefabs folder
    private int phase = 0;
    private bool isFinishAction = false; // variable for check if champion finish it's action

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
    }

    private void StartNewPhase()
    {
        phase++;
        SortChampionTurnBySpeed();
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
        // Sort aliveChampion List in increasing order of champion speed
        aliveChampions.Sort((champ1, champ2) 
            => champ1.GetComponent<OnFieldCharacter>().CurrentSpeed
            .CompareTo(champ2.GetComponent<OnFieldCharacter>().CurrentSpeed));

        // Reverse the list
        aliveChampions.Reverse();
        foreach (GameObject a in aliveChampions)
        {
            float b = a.GetComponent<OnFieldCharacter>().CurrentSpeed;
            Debug.Log(a + "--" + b);
        }
    }
    #endregion
}
