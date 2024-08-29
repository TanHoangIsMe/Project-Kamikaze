using System.Collections.Generic;
using UnityEngine;

public class AutoFindTargets : MonoBehaviour
{
    private List<OnFieldCharacter> enemyTargets; // list to store enemy targets
    public List<OnFieldCharacter> EnemyTargets { get { return enemyTargets; } set { enemyTargets = value; } }

    private List<OnFieldCharacter> allyTargets; // list to store ally targets
    public List<OnFieldCharacter> AllyTargets { get { return allyTargets; } set { enemyTargets = value; } }

    private OnFieldCharacter selfTarget; // value to store self target
    public OnFieldCharacter SelfTarget { get { return selfTarget; } set { selfTarget = value; } }

    private void Awake()
    {
        enemyTargets = new List<OnFieldCharacter>();
        allyTargets = new List<OnFieldCharacter>();

        selfTarget = null;
    }

    public void AutoFindTargetsBasedOnPriority(int numberOfTargets, int layer, StatType priorityStat, bool isLow)
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
                    case StatType.CurrentAttack:
                        compareResult = champ1.CurrentAttack.CompareTo(champ2.CurrentAttack);
                        break;
                    case StatType.CurrentArmor:
                        compareResult = champ1.CurrentArmor.CompareTo(champ2.CurrentArmor);
                        break;
                    case StatType.CurrentSpeed:
                        compareResult = champ1.CurrentSpeed.CompareTo(champ2.CurrentSpeed);
                        break;
                    case StatType.CurrentHealth:
                        compareResult = champ1.CurrentHealth.CompareTo(champ2.CurrentHealth);
                        break;
                    case StatType.CurrentMana:
                        compareResult = champ1.CurrentMana.CompareTo(champ2.CurrentMana);
                        break;
                    case StatType.CurrentBurst:
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
        if (layer == 6) allyTargets.Clear(); // ally
        else enemyTargets.Clear(); // layer = 7 - enemy

        // find all On-field character by searching for its script 
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if (character.gameObject.layer == layer)
            {
                if (layer == 6) // ally
                {
                    allyTargets.Add(character);
                }
                else // layer == 7 -> enemy
                {
                    enemyTargets.Add(character);
                }
            }
        }

        if (enemyTargets.Count > 0)
        {
            return enemyTargets;
        }
        else if (allyTargets.Count > 0)
        {
            return allyTargets;
        }
        else
        {
            return null;
        }
    }

    #region Targets Selected UI
    public void TurnOnShowTargets()
    {
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            if (enemyTargets.Contains(character)) // enemy target
            {
                // active enemy selected ring
                ActiveOrDeActiveSelectedRing(character, "Enemy Selected Ring", true);
            }
            else if (allyTargets.Contains(character))
            {
                // active ally selected ring
                ActiveOrDeActiveSelectedRing(character, "Ally Selected Ring", true);
            }
            else if(selfTarget == character)
            {
                // active self selected ring
                ActiveOrDeActiveSelectedRing(character, "Self Selected Ring", true);
            }
        }
    }

    public void TurnOffShowTargets()
    {
        foreach (var character in FindObjectsOfType<OnFieldCharacter>())
        {
            ActiveOrDeActiveSelectedRing(character, "Enemy Selected Ring", false);
            ActiveOrDeActiveSelectedRing(character, "Ally Selected Ring", false);
            ActiveOrDeActiveSelectedRing(character, "Self Selected Ring", false);
        }
    }

    private void ActiveOrDeActiveSelectedRing(OnFieldCharacter character,string ringName,bool turnOn)
    {        
        foreach (Transform child in character.gameObject.transform)
        {
            if (child.name == "Selected Rings")
            {
                foreach (Transform grandchild in child)
                {
                    if (grandchild.name == ringName)
                    {
                        grandchild.gameObject.SetActive(turnOn);
                    }
                }
            }
        }
            
    }
    #endregion
}
