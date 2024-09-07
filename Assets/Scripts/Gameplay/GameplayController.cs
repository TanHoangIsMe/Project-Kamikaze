using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] GameObject skillMenuCanvas;
    [SerializeField] TextMeshProUGUI phaseText;

    private string prefabPath; // path to character in Prefabs folder
    private int phase; // combat turn

    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList;
    private OnFieldCharacter whoTurn; // variable to know whose turn
    private CombatSkillMenu combatSkillMenu;
    private EnemyAI enemyAI;

    private Dictionary<Vector3, string> playerChampions = new Dictionary<Vector3, string>
    {
        //{ new Vector3(-7f,0f,-2f) ,"Maria" },
        //{ new Vector3(-4f,0f,-5f) ,"UrielAPlotexia" },
        { new Vector3(0f,0f,-2f) ,"UrielAPlotexia" },//UrielAPlotexia
        { new Vector3(4f,0f,-5f) ,"Maria" },
        //{ new Vector3(7f,0f,-2f) ,"Maria" },
    };

    private Dictionary<Vector3, string> enemyChampions = new Dictionary<Vector3, string>
    {
        //{ new Vector3(-7f,0f,10f) ,"Maria" },
        //{ new Vector3(-4f,0f,13f) ,"Maria" },
        //{ new Vector3(0f,0f,10f) ,"Maria" },
        { new Vector3(4f,0f,13f) ,"Maria" },
        { new Vector3(7f,0f,10f) ,"Maria" },
    };

    private void Awake()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        enemyAI = GetComponent<EnemyAI>();
        turnList = new List<OnFieldCharacter>();
        skillMenuCanvas.SetActive(false);
        whoTurn = null;
        phase = 0;
    }

    #region Turn
    private void Start()
    {
        SpawnEnemiesAndHeroes();
        StartNewPhase();
    }

    public void StartTurn()
    {
        // remove turn list of dead champion       
        for(int i = 0; i < turnList.Count; i++)
            if (turnList[i].CurrentHealth < 0)
                turnList.Remove(turnList[i]);

        if (turnList.Count == 0)
        {
            StartNewPhase();
            return;
        }

        whoTurn = turnList[0];
        turnList.RemoveAt(0);

        if (whoTurn.gameObject.layer == 6) // ally turn
        {
            skillMenuCanvas.SetActive(true);

            combatSkillMenu.Champion = whoTurn;
            combatSkillMenu.SetUpSkillAvatar();
            combatSkillMenu.SetUpBarsUI();
            combatSkillMenu.StartAllyTurn();
        }
        else // enemy turn
        {
            enemyAI.Champion = whoTurn;
            enemyAI.StartEnemyTurn();
            //StartTurn(); // for debug
        }
    }

    private void StartNewPhase()
    {
        phase++; // Next phase
        phaseText.text = $"Phase: {phase.ToString()}"; // Display Turn 
        turnList.Clear();
        CreateTurnList(); // Create new turn list 
        SortChampionTurnBySpeed(); // Sort turn list to who faster speed go first
        StartTurn(); // Start character turn
    }

    private void CreateTurnList()
    {
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if(character.CurrentHealth > 0)
                turnList.Add(character);
        }
    }
    #endregion

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

            if(layer == 7)
            {
                champion.transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x, 
                    180f, 
                    transform.eulerAngles.z);
            }

            // disable selected ring
            foreach (Transform child in champion.transform)
            {
                if (child.name == "Selected Rings")
                {
                    foreach (Transform grandchild in child.transform)
                    {
                        grandchild.gameObject.SetActive(false);
                    }
                }
            }
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
