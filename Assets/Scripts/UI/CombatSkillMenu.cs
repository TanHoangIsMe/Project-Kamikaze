using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
    private GameObject champion;
    public GameObject Champion { set { champion = value; } }

    private List<OnFieldCharacter> enemyTargets; // list to store enemy targets
    private List<OnFieldCharacter> allyTargets; // list to store ally targets

    private GameplayController gameplayController;
    int choseSkill; // value to show which skill user chose
    private void Awake()
    {
        enemyTargets = new List<OnFieldCharacter>();
        allyTargets = new List<OnFieldCharacter>();
        gameplayController = FindObjectOfType<GameplayController>();
        choseSkill = 0;
    }

    #region UsingSkill
    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        AutoFindTargetsBasedOnSkill(1, true);
        choseSkill = 1;
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        AutoFindTargetsBasedOnSkill(2, true);
        choseSkill = 2;
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        AutoFindTargetsBasedOnSkill(3, true);
        choseSkill = 3;
    }

    public void AttackConfirm()
    {
        //if (choseSkill != 0)
        //    ActivateSkill(choseSkill,)
    }

    // Using Skill Details
    // WhichSkill = 1 -> Using skill 1
    //            = 2 -> Using skill 2
    //            = 3 -> Burst
    private void AutoFindTargetsBasedOnSkill(int whichSkill, bool isLow)
    {
        OnFieldCharacter onFieldCharacter = champion.GetComponent<OnFieldCharacter>();

        if (onFieldCharacter != null)
        {
            Skill skill = onFieldCharacter.currentCharacter
                .Skills[whichSkill - 1]; // skill in skills[] start with 0

            if (skill.TargetTypes.Count() > 1)
            {
                AutoFindTargets(skill.NumberOfAllyTargets, 6, skill.PriorityStat, isLow);
                AutoFindTargets(skill.NumberOfEnemyTargets, 7, skill.PriorityStat, isLow);
            }
            else
            {
                if ((int)skill.TargetTypes[0] == 6)
                {
                    AutoFindTargets(skill.NumberOfAllyTargets, 6, skill.PriorityStat, isLow);
                }
                else
                {
                    AutoFindTargets(skill.NumberOfEnemyTargets, 7, skill.PriorityStat, isLow);
                }
            }
        }

        gameplayController.IsFinishAction = true;
        gameObject.SetActive(false);
    }

    // Choosing skill base on target type (ally, enemy or both)
    private void ActivateSkill(int whichSkill, int targetType, OnFieldCharacter onFieldCharacter)
    {
        if (targetType == 1)
            if (whichSkill == 1)
                onFieldCharacter.UsingFirstSkill(enemyTargets: enemyTargets,
                allyTargets: allyTargets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingSecondSkill(enemyTargets: enemyTargets,
                allyTargets: allyTargets);
            else
                onFieldCharacter.UsingBurstSkill(enemyTargets: enemyTargets,
                allyTargets: allyTargets);
        else if (targetType == 2)
            if (whichSkill == 1)
                onFieldCharacter.UsingFirstSkill(allyTargets: allyTargets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingSecondSkill(allyTargets: allyTargets);
            else
                onFieldCharacter.UsingBurstSkill(allyTargets: allyTargets);
        else
            if (whichSkill == 1)
                onFieldCharacter.UsingBurstSkill(enemyTargets: enemyTargets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingBurstSkill(enemyTargets: enemyTargets);
            else
                onFieldCharacter.UsingBurstSkill(enemyTargets: enemyTargets);
    }
    #endregion

    #region AutoFindTargets
    private void AutoFindTargets(int numberOfTargets, int layer, StatType priorityStat, bool isLow)
    {
        CreateTargetsList(layer);

        List<OnFieldCharacter> targetsToSort = null;

        if (layer == 6 && allyTargets != null)
        {
            targetsToSort = allyTargets;
        }
        else if (layer == 7 && enemyTargets != null)
        {
            targetsToSort = enemyTargets;
        }

        if (targetsToSort != null)
        {
            // Sort the target list based on the priorityStat
            targetsToSort.Sort((champ1, champ2) =>
            {
                int compareResult = 0;

                switch (priorityStat)
                {
                    case StatType.currentAttack:
                        compareResult = champ1.CurrentAttack.CompareTo(champ2.CurrentAttack);
                        break;
                    case StatType.currentArmor:
                        compareResult = champ1.CurrentArmor.CompareTo(champ2.CurrentArmor);
                        break;
                    case StatType.currentSpeed:
                        compareResult = champ1.CurrentSpeed.CompareTo(champ2.CurrentSpeed);
                        break;
                    case StatType.currentHealth:
                        compareResult = champ1.CurrentHealth.CompareTo(champ2.CurrentHealth);
                        break;
                    case StatType.currentMana:
                        compareResult = champ1.CurrentMana.CompareTo(champ2.CurrentMana);
                        break;
                    case StatType.currentBurst:
                        compareResult = champ1.CurrentBurst.CompareTo(champ2.CurrentBurst);
                        break;
                }

                return isLow ? compareResult : -compareResult; // If isLow is true, sort ascending, otherwise descending
            });

            // Ensure the list size is within the required number of targets
            if (targetsToSort.Count > numberOfTargets)
            {
                targetsToSort.RemoveRange(numberOfTargets, targetsToSort.Count - numberOfTargets);
            }
        }
    }

    private List<OnFieldCharacter> CreateTargetsList(int layer)
    {
        if(layer == 6) allyTargets.Clear(); // ally
        else enemyTargets.Clear(); // layer = 7 - enemy

        // find all On-field character by searching for its script 
        OnFieldCharacter[] onFieldCharacters = FindObjectsOfType<OnFieldCharacter>();

        foreach (OnFieldCharacter onFieldCharacter in onFieldCharacters)
        {
            if (onFieldCharacter.gameObject.layer == layer)
            {
                if (layer == 6) // ally
                {
                    allyTargets.Add(onFieldCharacter);
                }
                else // layer == 7 -> enemy
                {
                    enemyTargets.Add(onFieldCharacter);
                }
            }
        }

        if (enemyTargets.Count > 0)
        { 
            return enemyTargets;
        }
        else if(allyTargets.Count > 0)
        {
            return allyTargets;
        }
        else
        {
            return null;
        }
    }
    #endregion
}
