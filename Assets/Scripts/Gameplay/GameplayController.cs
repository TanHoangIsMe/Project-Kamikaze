using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour
{
    [SerializeField] GameObject skillMenuCanvas;
    [SerializeField] GameObject phaseCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] TextMeshProUGUI phaseText;
    [SerializeField] GameObject resultIcon;

    private string prefabPath; // path to character in Prefabs folder
    private int phase; // combat turn
    public int Phase {  get { return phase; } }

    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList;
    private OnFieldCharacter whoTurn; // variable to know whose turn
    private CombatSkillMenu combatSkillMenu;
    private EnemyAI enemyAI;

    private Dictionary<int, string> playerChampions = new Dictionary<int, string>
    {
        //{ 6 ,"Maria" },
        //{ 7 ,"Maria" },
        //{ 8 ,"UrielAPlotexia" },//UrielAPlotexia
        { 9 ,"UrielAPlotexia" },
        //{ 10 ,"Maria" },
    };

    private Dictionary<int, string> enemyChampions = new Dictionary<int, string>
    {
        { 0 ,"Maria" },
        //{ 1 ,"UrielAPlotexia" },
        { 2 ,"Maria" },
        //{ 3 ,"UrielAPlotexia" },
        { 4 ,"Maria" },
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
        // await a little bit to start combat 
        Invoke("StartNewPhase", 2f); 
    }

    public void StartTurn()
    {
        // check if game over
        Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);
        if (enemyAllDead || allyAllDead)
        {
            skillMenuCanvas.SetActive(false);
            phaseCanvas.SetActive(false);
            return;
        }
            
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
        foreach(var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if (character.Effects.Count > 0)
                for (int i = character.Effects.Count - 1; i >= 0; i--)
                {
                    character.Effects[i].UpdateEffect();
                }
            character.UpdateEffectIcon();
        }
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
        // spawn player's champions
        foreach (KeyValuePair<int, string> playerChampion in playerChampions)
        {
            CreateCharacter(playerChampion.Key, playerChampion.Value, 6);
        }

        // spawn enemies
        foreach (KeyValuePair<int, string> enemyChampion in enemyChampions)
        {
            CreateCharacter(enemyChampion.Key, enemyChampion.Value, 7);
        }
    }

    private void CreateCharacter(int position, string characterName, int layer)
    {
        prefabPath = $"Prefabs/Characters/{characterName}";

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab != null)
        {
            // create champion
            GameObject champion = Instantiate(prefab, GetPosition(position), Quaternion.identity);

            // set up champion layer
            champion.layer = layer;

            // set on field character position
            champion.GetComponent<OnFieldCharacter>().Position = position;

            if(layer == 7)
            {
                champion.transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x, 
                    180f, 
                    transform.eulerAngles.z);

                GameObject overheadBars = champion.transform.Find("Health Bar Canvas").gameObject;
                if (overheadBars != null)
                {
                    RectTransform overheadBarsTransform = overheadBars.GetComponent<RectTransform>();
                    if (overheadBarsTransform != null)
                        overheadBarsTransform.localScale *= 1.5f;                  
                }
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

    private Vector3 GetPosition(int position)
    {
        switch (position)
        {
            // enemy positions
            case 0: return new Vector3(18f, 0f, 14f);
            case 1: return new Vector3(21.5f, 0f, 16f);
            case 2: return new Vector3(25f, 0f, 14f);
            case 3: return new Vector3(28.5f, 0f, 16f);
            case 4: return new Vector3(32f, 0f, 14f);

            // ally positions
            case 6: return new Vector3(18f, 0f, 4f);
            case 7: return new Vector3(21.5f, 0f, 2f);
            case 8: return new Vector3(25f, 0f, 4f);
            case 9: return new Vector3(28.5f, 0f, 2f);
            case 10: return new Vector3(32f, 0f, 4f);

            default: return Vector3.zero;
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

    #region GameOver
    public void CheckGameOver()
    {
        Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);

        if (allyAllDead || enemyAllDead)
        {
            Time.timeScale = 0;
            gameOverCanvas.SetActive(true);
        }

        Image resultIconImage = resultIcon.GetComponent<Image>();

        if (enemyAllDead && resultIconImage != null)
            resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/GameOver/Victory Icon");
        else
        {
            resultIconImage.rectTransform.sizeDelta = new Vector2(500f, 450f);
            resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/GameOver/Defeat Icon");
        }
    }

    private void Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead)
    {
        enemyAllDead = true;
        allyAllDead = true;

        foreach (var champ in FindObjectsOfType<OnFieldCharacter>())
        {
            if (new[] { 6, 7, 8, 9, 10 }.Contains(champ.Position) && champ.CurrentHealth > 0)
                allyAllDead = false;
            else if (new[] { 0, 1, 2, 3, 4 }.Contains(champ.Position) && champ.CurrentHealth > 0)
                enemyAllDead = false;
        }           
    }
    #endregion
}
