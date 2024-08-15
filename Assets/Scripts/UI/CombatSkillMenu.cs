using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
    [SerializeField] Transform choosePriorityPanel;

    private TextMeshProUGUI choosePriorityText;  
    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { set { champion = value; } }

    private List<OnFieldCharacter> enemyTargets; // list to store enemy targets
    private List<OnFieldCharacter> allyTargets; // list to store ally targets

    private GameplayController gameplayController;

    private TargetType[] targetType;
    private StatType priorityStat;
    private int numberOfTargetType;
    private int numberOfTargets;
    private int layer;

    private int choseSkill; // value to show which skill user chose

    // value to control when to open choose dialog when need to open dialog 2 times
    private bool isFinish;
    private bool canSelectTarget; // value to know when player can select target
    // value to know what type of target can select
    // 1 -> Enemy / 2 -> Ally / 3 -> Both
    private int selectType; 

    private void Awake()
    {
        enemyTargets = new List<OnFieldCharacter>();
        allyTargets = new List<OnFieldCharacter>();

        gameplayController = FindObjectOfType<GameplayController>();

        choosePriorityPanel.gameObject.SetActive(false);
        choosePriorityText = choosePriorityPanel.GetChild(0)
            .gameObject.GetComponent<TextMeshProUGUI>();

        choseSkill = 0;
        isFinish = false;
        canSelectTarget = false;
    }

    private void Update()
    {
        if (canSelectTarget)
        {
            // check if player click something
            if (Input.GetMouseButtonDown(0))
            {
                // Create raycast based on mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Check if raycast hit something
                if (Physics.Raycast(ray, out hit))
                {
                    // Get object that ray hit
                    GameObject clickedObject = hit.collider.gameObject;

                    // can select enemy 
                    if((selectType == 1 || selectType == 3) && clickedObject.layer == 7)
                    {
                        enemyTargets.Clear();   
                        enemyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                    }

                    // can select ally
                    if((selectType == 2 || selectType == 3) && clickedObject.layer == 6)
                    {
                        allyTargets.Clear();
                        allyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                    }
                }
                test(allyTargets);
                test(enemyTargets);
            }
        }
    }

    #region UsingSkill
    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        CheckInfoToAutoFindTargets(0);
        choseSkill = 0;
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        CheckInfoToAutoFindTargets(1);
        choseSkill = 1;
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        CheckInfoToAutoFindTargets(2);
        choseSkill = 2;
    }

    public void AttackConfirm()
    {
        if (enemyTargets.Count > 0 || allyTargets.Count() > 0)
        {
            if (targetType.Count() > 1)
            {
                if (choseSkill == 0)
                    champion.UsingFirstSkill(enemyTargets: enemyTargets
                        , allyTargets: allyTargets);
                else if (choseSkill == 1)
                    champion.UsingSecondSkill(enemyTargets: enemyTargets
                        , allyTargets: allyTargets);
                else
                    champion.UsingBurstSkill(enemyTargets: enemyTargets
                        , allyTargets: allyTargets);
            }
            else
            {
                if (targetType[0] == TargetType.Enemy)
                {
                    if (choseSkill == 0)
                        champion.UsingFirstSkill(enemyTargets: enemyTargets);
                    else if (choseSkill == 1)
                        champion.UsingSecondSkill(enemyTargets: enemyTargets);
                    else
                        champion.UsingBurstSkill(enemyTargets: enemyTargets);
                }
                else
                {
                    if (choseSkill == 0)
                        champion.UsingFirstSkill(enemyTargets: allyTargets);
                    else if (choseSkill == 1)
                        champion.UsingSecondSkill(enemyTargets: allyTargets);
                    else
                        champion.UsingBurstSkill(enemyTargets: allyTargets);
                }
            }

            gameplayController.IsFinishAction = true;
            gameObject.SetActive(false);
            isFinish = false;
            canSelectTarget = false;
        }
        else
        {
            Debug.Log("Please choose a skill");
        }
    }

    public void ChoosingLowestPriority()
    {
        AutoFindTargets(numberOfTargets, layer, priorityStat, true);
        choosePriorityPanel.gameObject.SetActive(false);
        isFinish = true;
    }

    public void ChoosingHighestPriority()
    {
        AutoFindTargets(numberOfTargets, layer, priorityStat, false);
        choosePriorityPanel.gameObject.SetActive(false);
        isFinish = true;
    }
    #endregion

    public void test(List<OnFieldCharacter> list)
    {
        string a = "";
        string b = "";
        foreach (OnFieldCharacter c in list)
        {
            a += c.name + "-";
        }
        foreach (OnFieldCharacter c in list)
        {
            b += c.CurrentHealth + "-";
        }
        Debug.Log(a);
        Debug.Log(b);
    }

    private void CheckInfoToAutoFindTargets(int whichSkill)
    {
        // count how many type of target that skill can affect to
        numberOfTargetType = champion.currentCharacter
            .Skills[whichSkill].TargetTypes.Count();
        
        // priority stat for auto find
        priorityStat = champion.currentCharacter
            .Skills[whichSkill].PriorityStat;

        // count how many enemies
        int numberOfEnemyTargets = champion.currentCharacter
            .Skills[whichSkill].NumberOfEnemyTargets;

        // count how many enemies
        int numberOfAllyTargets = champion.currentCharacter
            .Skills[whichSkill].NumberOfAllyTargets;

        // which type of target skill affected to
        targetType = champion.currentCharacter.Skills[whichSkill].TargetTypes;

        if (numberOfTargetType == 1)
        {
            if (targetType[0] == TargetType.Enemy)
            {
                if(numberOfEnemyTargets == 1)
                {
                    // auto find 1 target which low priority
                    // 1 target - enemy layer - priority stat - lowest stat
                    AutoFindTargets(1, 7, priorityStat, true);
                    canSelectTarget = true;
                    selectType = 1;
                }
                else
                {
                    // open choose priority dialog
                    OpenChoosePriorityDialog(priorityStat, numberOfEnemyTargets, 7);
                }
            }
            else // target type = ally
            {
                if (numberOfAllyTargets == 1)
                {
                    // auto find 1 target which low priority
                    AutoFindTargets(1, 6, priorityStat, true);
                    canSelectTarget = true;
                    selectType = 2;
                }
                else
                {
                    // open choose priority dialog
                    OpenChoosePriorityDialog(priorityStat, numberOfAllyTargets, 6);
                }
            }
        }
        else // number of target type = 2
        {
            // skill affect 1 ally and 1 enemy
            if(numberOfAllyTargets == numberOfEnemyTargets && numberOfAllyTargets == 1)
            {
                // auto find 1 enemy 1 ally which low priority
                AutoFindTargets(1, 6, priorityStat, true);
                AutoFindTargets(1, 7, priorityStat, true);
                canSelectTarget = true;
                selectType = 3;

            }
            else if(numberOfEnemyTargets == 1 &&  numberOfAllyTargets > 1)
            {
                // auto find 1 enemy
                AutoFindTargets(1, 7, priorityStat, true);
                canSelectTarget = true;
                selectType = 1;
                // open choose priority dialog
                OpenChoosePriorityDialog(priorityStat, numberOfAllyTargets, 6);
            }
            else if(numberOfAllyTargets == 1 && numberOfEnemyTargets > 1)
            {
                // auto find 1 ally
                AutoFindTargets(1, 6, priorityStat, true);
                canSelectTarget = true;
                selectType = 2;
                // open choose priority dialog
                OpenChoosePriorityDialog(priorityStat, numberOfEnemyTargets, 7);
            }
            else // number of enemy > 1 and number of ally > 1
            {
                // open choose priority dialog 2 times
                StartCoroutine(OpenDialog2Times(numberOfEnemyTargets, 
                    numberOfAllyTargets));
            }
        }
    }

    private void OpenChoosePriorityDialog(StatType priorityStat, int numberOfTargets, int layer)
    {
        string targets = "";
        if (layer == 6) targets = "allies";
        else targets = "enemies";

        choosePriorityPanel.gameObject.SetActive(true);
        choosePriorityText.text = $"Do you want to choose the {targets} with the lowest or highest {priorityStat} points?";
        this.priorityStat = priorityStat;
        this.numberOfTargets = numberOfTargets;
        this.layer = layer;
    }

    private IEnumerator OpenDialog2Times(int numberOfEnemyTargets, int numberOfAllyTargets)
    {
        OpenChoosePriorityDialog(priorityStat, numberOfEnemyTargets, 7);
        yield return StartCoroutine(AwaitFinishChoosing());
        OpenChoosePriorityDialog(priorityStat, numberOfAllyTargets, 6);
    }

    private IEnumerator AwaitFinishChoosing()
    {
        while (!isFinish)
        {
            yield return null;
        }
    }

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
