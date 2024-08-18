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
        else if (allyTargets.Count > 0)
        {
            return allyTargets;
        }
        else
        {
            return null;
        }
    }
}
