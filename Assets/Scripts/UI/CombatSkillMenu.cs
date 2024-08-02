using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
    private GameObject champion;
    private List<GameObject> targets = new List<GameObject>();
    private int layer;
    public GameObject Champion { set { champion = value; } }

    #region UsingSkill
    // Function for pressing Skill 1 button
    public void UsingSkill1() 
    {
        OnFieldCharacter onFieldCharacter = champion.GetComponent<OnFieldCharacter>();
        if(onFieldCharacter != null)
        {
            Skill skill = onFieldCharacter.currentCharacter.Skills[0];

            AutoFindTarget(skill.NumberOfTargets,
                (int)skill.TargetTypes[0],
                skill.TargetTypes.Count());

            onFieldCharacter.UsingFirstSkill(champion,targets);
        }
        gameObject.SetActive(false);
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        Debug.Log("using skill 2");
        gameObject.SetActive(false);
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        Debug.Log("using skill burst");
        gameObject.SetActive(false);
    }
    #endregion

    #region AutoFindTarget
    private void AutoFindTarget(int numberOfTargets, int layer, int targetTypes)
    {
       CreateTargetList(layer,targetTypes);

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
    }

    // Create a list of all target
    private List<GameObject> CreateTargetList(int layer, int targetTypes)
    {
        if (targetTypes == 1)
        {
            return GetAllChampionWithSameLayer(layer);
        }
        else
        {
            GetAllChampionWithSameLayer(6);
            return GetAllChampionWithSameLayer(7);
        }
    }

    private List<GameObject> GetAllChampionWithSameLayer(int layer)
    {
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
