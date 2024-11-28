using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetUpTurnList : NetworkBehaviour
{
    private CombatSkillMenu combatSkillMenu;
    private SkillHandler skillHandler;
    private Setting setting;
    private GameObject gameOverCanvas;
    private GameObject resultIcon;
    private GameObject phaseText;
    private TextMeshProUGUI thisPhase;

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
        setting = FindObjectOfType<Setting>(); 

        gameOverCanvas = GameObject.FindGameObjectWithTag("GameOver");
        if(gameOverCanvas != null ) 
            resultIcon = gameOverCanvas.transform.GetChild(1).gameObject;

        phaseText = GameObject.FindGameObjectWithTag("Phase");
        if(phaseText != null )
            thisPhase = phaseText.GetComponent<TextMeshProUGUI>();

        turnList = new List<OnFieldCharacter>();
        whoTurn = null;

        // turn off ui canvas
        if (combatSkillMenu != null) // turn off skill menu
            combatSkillMenu.gameObject.SetActive(false);

        if (thisPhase != null)
            thisPhase.text = "";

        if(gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    #region Server Rpc
    [ServerRpc (RequireOwnership = false)]
    public void StartNewPhaseServerRpc()
    {
        StartNewPhaseClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTurnServerRpc()
    {
        StartTurnClientRpc();
    }
    #endregion

    #region Client Rpc
    [ClientRpc]
    public void StartNewPhaseClientRpc()
    {
        phase++; // Next phase
       
        if (thisPhase != null) // Display Turn
            thisPhase.text = $"Phase: {phase.ToString()}";

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
        //// check if game over
        //Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);
        //if (enemyAllDead || allyAllDead)
        //{
        //    combatSkillMenu.gameObject.SetActive(false);
        //    thisPhase.text = "";
        //    return;
        //}

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
        Debug.Log(turnList.Count + whoTurn.name);
        if (combatSkillMenu != null) // set up menu skill UI
        {
            if (whoTurn.gameObject.layer == 6 && IsHost)
                combatSkillMenu.gameObject.SetActive(true);
            else if (whoTurn.gameObject.layer == 7 && !IsHost)
            {
                Debug.Log(whoTurn.gameObject.layer+whoTurn.name);
                combatSkillMenu.gameObject.SetActive(true);
            }

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
    public void CheckGameOver()
    {
        Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);

        if (allyAllDead || enemyAllDead)
        {
            Time.timeScale = 0;
            thisPhase.text = "";
            setting.gameObject.SetActive(false);
            gameOverCanvas.SetActive(true);
        }

        Image resultIconImage = resultIcon.GetComponent<Image>();

        if (enemyAllDead && resultIconImage != null)
        {
            if(IsHost)
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Victory Icon");
            else
                resultIconImage.sprite = Resources.Load<Sprite>("Art/UI/In Game/Defeat Icon");
        }
        else
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
