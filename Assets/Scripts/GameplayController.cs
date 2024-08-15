using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] GameObject skillMenuCanvas;
    [SerializeField] TextMeshProUGUI phaseText;

    private string prefabPath; // path to character in Prefabs folder
    private int phase; // combat turn
    private bool isFinishAction; // variable for check if champion finish it's action
    public bool IsFinishAction { set { isFinishAction = value; } }

    // place to hold all champion that exist on battle field
    private List<GameObject> aliveChampions;
    private List<GameObject> turnList;
    private GameObject whoTurn; // variable to know whose turn
    private CombatSkillMenu combatSkillMenu;

    private Dictionary<Vector3, string> enemyChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,-4f) ,"Maria" },
        { new Vector3(-1.6f,0f,-2.6f) ,"UrielAPlotexia" },
        { new Vector3(-1.6f,0f,-1.6f) ,"UrielAPlotexia" },
    };

    private Dictionary<Vector3, string> playerChampions = new Dictionary<Vector3, string>
    {
        { new Vector3(-2.4f,0f,4f) ,"Maria" },
        { new Vector3(-1.6f,0f,2.6f) ,"Maria" },
        
    };

    private void Awake()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        aliveChampions = new List<GameObject>();
        whoTurn = null;
        phase = 0;
        isFinishAction = false;
    }

    private void Start()
    {
        SpawnEnemiesAndHeroes();
        StartNewPhase();
    }

    private void Update()
    {
        CombatPhase();
    }

    private void CombatPhase()
    {
        if (turnList.Count > 0) // Check if every champions finish their action on this turn
        {
            if (!isFinishAction)  // Check if first champion on the list finish action
            { 
                whoTurn = turnList[0];
                if (whoTurn.layer == 6) // Check champion is ally or enemy
                {
                    skillMenuCanvas.SetActive(true);
                    combatSkillMenu.Champion = whoTurn.GetComponent<OnFieldCharacter>();
                    //Debug.Log(whoTurn + " turn");
                }
                else
                {
                    //Debug.Log(whoTurn + " turn");
                    isFinishAction = true;
                }
            }
            else // if first champion of the list finish action then remove it from turn list
            {
                turnList.RemoveAt(0);
                isFinishAction = false;
            }
        }
        else // if every champion finish their action
        {
            StartNewPhase();
        }
    }

    private void StartNewPhase()
    {
        phase++; // Next phase
        phaseText.text = $"Phase: {phase.ToString()}"; // Display Turn 
        turnList = new List<GameObject>(aliveChampions);
        SortChampionTurnBySpeed(); // Create new turn list  
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
        turnList.Sort((champ1, champ2) 
            => champ1.GetComponent<OnFieldCharacter>().CurrentSpeed
            .CompareTo(champ2.GetComponent<OnFieldCharacter>().CurrentSpeed));

        // Reverse the list
        turnList.Reverse();
    }
    #endregion
}
