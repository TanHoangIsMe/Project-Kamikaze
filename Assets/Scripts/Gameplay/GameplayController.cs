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
    [SerializeField] GameObject settingCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] TextMeshProUGUI phaseText;
    [SerializeField] GameObject resultIcon;

    private string prefabPath; // path to character in Prefabs folder
    private int phase; // combat turn
    public int Phase {  get { return phase; } }

    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList;
    public List<OnFieldCharacter> TurnList { get { return turnList; } set { turnList = value; } }

    private OnFieldCharacter whoTurn; // variable to know whose turn
    private CombatSkillMenu combatSkillMenu;
    private EnemyAI enemyAI;

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
        // spawn champion from champs selected list
        SpawnChampion spawnChampion = GetComponent<SpawnChampion>();
        if (spawnChampion != null)
            spawnChampion.SpawnEnemiesAndHeroes(false);

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
        phaseCanvas.SetActive(true);
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
            phaseCanvas.SetActive(false);
            settingCanvas.SetActive(false);
            gameOverCanvas.SetActive(true);
        }

        Image resultIconImage = resultIcon.GetComponent<Image>();

        if (enemyAllDead && resultIconImage != null)
        {
            resultIconImage.rectTransform.sizeDelta = new Vector2(1000f, 800f);
            resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Victory Icon");
        }
        else
        {
            resultIconImage.rectTransform.sizeDelta = new Vector2(800f, 800f);
            resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Defeat Icon");
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
