using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
    private GameObject champion;
    public GameObject Champion { set { champion = value; } }

    private List<GameObject> targets;
    private GameplayController gameplayController;
    int choseSkill; // value to show which skill user chose
    private void Awake()
    {
        targets = new List<GameObject>();
        gameplayController = FindObjectOfType<GameplayController>();
        choseSkill = 0;
    }

    #region UsingSkill
    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        AutoFindTargetsBasedOnSkill(1);
        choseSkill = 1;
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        AutoFindTargetsBasedOnSkill(2);
        choseSkill = 2;
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        AutoFindTargetsBasedOnSkill(3);
        choseSkill = 3;
    }

    public void AttackConfirm()
    {
        //if(choseSkill != 0)
        //    ActivateSkill(choseSkill,)
    }

    // Using Skill Details
    // WhichSkill = 1 -> Using skill 1
    //            = 2 -> Using skill 2
    //            = 3 -> Burst
    private void AutoFindTargetsBasedOnSkill(int whichSkill)
    {
        OnFieldCharacter onFieldCharacter = champion.GetComponent<OnFieldCharacter>();

        if (onFieldCharacter != null)
        {
            Skill skill = onFieldCharacter.currentCharacter
                .Skills[whichSkill - 1]; // skill in skills[] start with 0

            if (skill.TargetTypes.Count() > 1)
            {
                AutoFindTargets(skill.NumberOfAllyTargets, 6);
                List<GameObject> cloneTargets = new List<GameObject>(targets);
                AutoFindTargets(skill.NumberOfEnemyTargets, 7);
                //ChoosingSkill(whichSkill, 1, onFieldCharacter, cloneTargets);
            }
            else
            {
                if ((int)skill.TargetTypes[0] == 6)
                {
                    AutoFindTargets(skill.NumberOfAllyTargets, 6);
                    //ChoosingSkill(whichSkill, 2, onFieldCharacter);
                }
                else
                {
                    AutoFindTargets(skill.NumberOfEnemyTargets, 7);
                    //ChoosingSkill(whichSkill, 3, onFieldCharacter);
                }
            }
        }

        gameplayController.IsFinishAction = true;
        gameObject.SetActive(false);
    }

    // Choosing skill base on target type (ally, enemy or both)
    private void ActivateSkill(int whichSkill, int targetType, 
        OnFieldCharacter onFieldCharacter, List<GameObject> cloneTargets = null)
    {
        if (targetType == 1)
            if (whichSkill == 1)
                onFieldCharacter.UsingFirstSkill(enemyTargets: targets,
                allyTargets: cloneTargets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingSecondSkill(enemyTargets: targets,
                allyTargets: cloneTargets);
            else
                onFieldCharacter.UsingBurstSkill(enemyTargets: targets,
                allyTargets: cloneTargets);
        else if (targetType == 2)
            if (whichSkill == 1)
                onFieldCharacter.UsingFirstSkill(allyTargets: cloneTargets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingSecondSkill(allyTargets: cloneTargets);
            else
                onFieldCharacter.UsingBurstSkill(allyTargets: cloneTargets);
        else
            if (whichSkill == 1)
                onFieldCharacter.UsingBurstSkill(enemyTargets: targets);
            else if (whichSkill == 2)
                onFieldCharacter.UsingBurstSkill(enemyTargets: targets);
            else
                onFieldCharacter.UsingBurstSkill(enemyTargets: targets);

        string a = "";
        foreach (GameObject gameObject in targets)
        {
            a += gameObject.GetComponent<OnFieldCharacter>().CurrentHealth + "-";
        }
        Debug.Log(a);
    }
    #endregion

    #region AutoFindTargets
    private void AutoFindTargets(int numberOfTargets, int layer)
    {
       CreateTargetsList(layer);

        if(targets != null)
        {
            // sort target list by increasing of champion health
            targets.Sort((champ1, champ2)
            => champ1.GetComponent<OnFieldCharacter>().CurrentHealth
            .CompareTo(champ2.GetComponent<OnFieldCharacter>().CurrentHealth));

            // check for not encounter bug that number of targets in list < targets needed
            if (targets.Count > numberOfTargets)
                // reduce target list to number of target that skill can impact to
                targets.RemoveRange(numberOfTargets, targets.Count - numberOfTargets);
        }

        string a = "";
        foreach(GameObject gameObject in targets)
        {
            a += gameObject.name;
        }
        Debug.Log(a);
    }


    private List<GameObject> CreateTargetsList(int layer)
    {
        targets.Clear(); // clear targets list to create new one

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.layer == layer) targets.Add(gameObject);
        }
        
        if (targets.Count == 0)
        {
            return null;
        }
        return targets; 
    }
    #endregion
}
