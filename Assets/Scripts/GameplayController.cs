using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] Canvas skillMenuCanvas;

    private string prefabPath; // path to character in Prefabs folder
    private int phase = 0; // combat turn
    private bool isFinishAction = false; // variable for check if champion finish it's action

    // place to hold all champion that exist on battle field
    private List<GameObject> aliveChampions;
    private GameObject whoTurn; // variable to know whose turn
    private CombatSkillMenu combatSkillMenu;

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

    private void Awake()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        whoTurn = null;
        aliveChampions = new List<GameObject>();
    }

    private void Start()
    {
        SpawnEnemiesAndHeroes();
        StartNewPhase();      
    }

    private void StartNewPhase()
    {
        phase++;
        skillMenuCanvas.enabled = true;
        SortChampionTurnBySpeed();
        whoTurn = aliveChampions[0];
        combatSkillMenu.Champion = whoTurn;
    }

    #region SpawnChampions
    private void SpawnEnemiesAndHeroes()
    {
        // spawn enemies
        foreach (KeyValuePair<Vector3, string> enemyChampion in enemyChampions)
        {
            CreateCharacter(enemyChampion.Key, enemyChampion.Value, 7);
        }

        // spawn player's champions
        foreach (KeyValuePair<Vector3, string> playerChampion in playerChampions)
        {
            CreateCharacter(playerChampion.Key, playerChampion.Value, 6);
        }
    }

    private void CreateCharacter(Vector3 spawnPosition, string characterName, int layer)
    {
        prefabPath = $"Prefabs/Characters/{characterName}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            // create champion
            GameObject champion = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // set up champion layer
            champion.layer = layer;

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
        // Sort aliveChampion List in increasing order of champion speed
        aliveChampions.Sort((champ1, champ2) 
            => champ1.GetComponent<OnFieldCharacter>().CurrentSpeed
            .CompareTo(champ2.GetComponent<OnFieldCharacter>().CurrentSpeed));

        // Reverse the list
        aliveChampions.Reverse();
    }
    #endregion
}
