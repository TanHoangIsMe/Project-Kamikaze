using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetUpTurnList : NetworkBehaviour
{
    // place to hold all champion that exist on battle field
    private List<OnFieldCharacter> turnList = new List<OnFieldCharacter>();
    public List<OnFieldCharacter> TurnList { get { return turnList; } set { turnList = value; } }

    public void StartNewPhase()
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
        CreateTurnListClientRpc(); // Create new turn list 
        SortChampionTurnBySpeed(); // Sort turn list to who faster speed go first
        //StartTurn(); // Start character turn
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
            StartNewPhase();
            return;
        }

        //whoTurn = turnList[0];
        //turnList.RemoveAt(0);

        //if (whoTurn.gameObject.layer == 6) // ally turn
        //{
        //    skillMenuCanvas.SetActive(true);

        //    combatSkillMenu.Champion = whoTurn;
        //    combatSkillMenu.SetUpSkillAvatar();
        //    combatSkillMenu.SetUpBarsUI();
        //    combatSkillMenu.StartAllyTurn();
        //}
        //else // enemy turn
        //{
        //    enemyAI.Champion = whoTurn;
        //    enemyAI.StartEnemyTurn();
        //    //StartTurn(); // for debug
        //}
    }

    [ClientRpc]
    private void CreateTurnListClientRpc()
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
