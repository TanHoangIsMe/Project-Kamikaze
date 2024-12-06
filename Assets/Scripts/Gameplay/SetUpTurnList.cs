using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetUpTurnList : NetworkBehaviour
{
    private CheckNumberOfTargets checkNumberOfTargets;
    private CombatSkillMenu combatSkillMenu;
    private SkillHandler skillHandler;
    private GameObject gameOverCanvas;
    private GameObject resultIcon;
    private TextMeshProUGUI phaseText;
    private TextMeshProUGUI whoTurnText;

    private int phase; // combat turn
    public int Phase { get { return phase; } }

    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList;
    public List<OnFieldCharacter> TurnList { get { return turnList; } set { turnList = value; } }

    private OnFieldCharacter whoTurn; // variable to know whose turn

    private void Start()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        skillHandler = FindObjectOfType<SkillHandler>(); 
        checkNumberOfTargets = FindObjectOfType<CheckNumberOfTargets>();
        phaseText = GameObject.Find("Phase Text").GetComponent<TextMeshProUGUI>();
        whoTurnText = GameObject.Find("Who Turn Text").GetComponent<TextMeshProUGUI>();
        gameOverCanvas = GameObject.Find("GameOver Canvas");

        turnList = new List<OnFieldCharacter>();
        whoTurn = null;

        // turn off ui canvas
        if (combatSkillMenu != null) // turn off skill menu
            combatSkillMenu.gameObject.SetActive(false);

        if (phaseText != null)
            phaseText.text = "";

        if (whoTurnText != null)
            whoTurnText.text = "";

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        if (checkNumberOfTargets != null && combatSkillMenu != null)
        {
            Transform choosePriority = combatSkillMenu.transform.GetChild(1);
            checkNumberOfTargets.ChoosePriorityPanel = choosePriority;
            checkNumberOfTargets.ChoosePriorityText = 
                choosePriority.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            choosePriority.gameObject.SetActive(false);
        }

        if (gameOverCanvas != null)
            resultIcon = gameOverCanvas.transform.GetChild(1).gameObject;
    }

    #region Server Rpc
    [ServerRpc (RequireOwnership = false)]
    public void StartNewPhaseServerRpc()
    {
        StartNewPhaseClientRpc();
    }
    #endregion

    #region Client Rpc
    [ClientRpc]
    public void StartNewPhaseClientRpc()
    {
        phase++; // Next phase
       
        if (phaseText != null) // Display Turn
            phaseText.text = $"Phase: {phase.ToString()}";

        turnList.Clear(); // reset turn list

        // update champ effect
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
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
        StartTurnClientRpc(); // Start character turn
    }

    [ClientRpc]
    public void StartTurnClientRpc()
    {
        // check if game over
        Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);
        if (enemyAllDead || allyAllDead)
        {
            combatSkillMenu.gameObject.SetActive(false);
            phaseText.text = "";
            return;
        }

        // remove turn list of dead champion       
        for (int i = 0; i < turnList.Count; i++)
            if (turnList[i].CurrentHealth < 0)
                turnList.Remove(turnList[i]);

        if (turnList.Count == 0)
        {
            StartNewPhaseClientRpc();
            return;
        }

        whoTurn = turnList[0];
        turnList.RemoveAt(0);

        if (combatSkillMenu != null) // set up menu skill UI
        {
            if (whoTurn.gameObject.layer == 6)
                DisplaySkillMenu(IsHost);
            else
                DisplaySkillMenu(!IsHost);

            combatSkillMenu.Champion = whoTurn;
            combatSkillMenu.SetUpSkillAvatar();
            combatSkillMenu.SetUpBarsUI();
        }

        if (skillHandler != null) // set up skill handler
        {
            skillHandler.Champion = whoTurn;
            skillHandler.IsPlayer = true;

            if (whoTurn.gameObject.layer == 7)
                skillHandler.SwapChampionsLayer();
        }
    }

    private void DisplaySkillMenu(bool isHost)
    {
        if (isHost)
        {
            whoTurnText.text = "Your Turn";
            combatSkillMenu.gameObject.SetActive(true);
        }
        else
            whoTurnText.text = "Enemy's Turn";
    }
    #endregion

    #region Create Turn    
    private void CreateTurnList()
    {
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if (character.CurrentHealth > 0)
                turnList.Add(character);
        }
    }

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
    public void CheckGameOver(bool isPlayer1Quit, bool isPlayer2Quit)
    {
        Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);

        if (allyAllDead || enemyAllDead || isPlayer1Quit || isPlayer2Quit)
        {
            Time.timeScale = 0;
            phaseText.text = "";
            whoTurnText.text = "";
            combatSkillMenu.gameObject.SetActive(false);
            gameOverCanvas.SetActive(true);
            NetworkManager.Singleton.Shutdown();
        }

        Image resultIconImage = resultIcon.GetComponent<Image>();

        if (enemyAllDead && resultIconImage != null 
            || isPlayer2Quit && resultIconImage != null)
        {
            if(IsHost)
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Victory Icon");
            else
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Defeat Icon");
        }
        else if (allyAllDead && resultIconImage != null
                || isPlayer1Quit && resultIconImage != null)
        {
            if(IsHost)
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Defeat Icon");
            else
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Victory Icon");
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
