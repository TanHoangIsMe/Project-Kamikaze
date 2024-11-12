using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetUpTurnList : NetworkBehaviour
{
    private CombatSkillMenu combatSkillMenu;
    private SkillHandler skillHandler;

    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList;
    public List<OnFieldCharacter> TurnList { get { return turnList; } set { turnList = value; } }

    private OnFieldCharacter whoTurn; // variable to know whose turn

    private void Awake()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        skillHandler = FindObjectOfType<SkillHandler>();
        turnList = new List<OnFieldCharacter>();
        whoTurn = null;

        //if(combatSkillMenu != null) // turn off skill menu
        //    combatSkillMenu.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void StartNewPhaseClientRpc()
    {
        //phase++; // Next phase
        //phaseText.text = $"Phase: {phase.ToString()}"; // Display Turn 
        //phaseCanvas.SetActive(true);
        //turnList.Clear();
        //foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        //{
        //    if (character.Effects.Count > 0)
        //        for (int i = character.Effects.Count - 1; i >= 0; i--)
        //        {
        //            character.Effects[i].UpdateEffect();
        //        }

        //    character.UpdateEffectIcon();
        //}
        CreateTurnList(); // Create new turn list 
        SortChampionTurnBySpeed(); // Sort turn list to who faster speed go first
        StartTurn(); // Start character turn
    }

    public void StartTurn()
    {
        //// check if game over
        //Check1SideAllDead(out bool enemyAllDead, out bool allyAllDead);
        //if (enemyAllDead || allyAllDead)
        //{
        //    skillMenuCanvas.SetActive(false);
        //    phaseCanvas.SetActive(false);
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

        whoTurn = turnList[0];Debug.Log(whoTurn.name);
        turnList.RemoveAt(0);
        Debug.Log(whoTurn.name);
        combatSkillMenu.gameObject.SetActive(true);
        combatSkillMenu.Champion = whoTurn;
        combatSkillMenu.SetUpSkillAvatar();
        combatSkillMenu.SetUpBarsUI();
        combatSkillMenu.StartAllyTurn();
    }
    
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
}
