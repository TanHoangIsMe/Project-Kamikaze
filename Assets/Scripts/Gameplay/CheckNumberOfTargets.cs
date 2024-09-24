using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class CheckNumberOfTargets : MonoBehaviour
{
    [SerializeField] private Transform choosePriorityPanel;
    public Transform ChoosePriorityPanel { get { return choosePriorityPanel; } }

    private TextMeshProUGUI choosePriorityText;

    private AutoFindTargets autoFindTargets;

    private OnFieldCharacter champion; // who using skill
    public OnFieldCharacter Champion { get { return champion; } set {  champion = value; } }

    private int whichSkill; // which skill
    public int WhichSkill { get { return whichSkill; } set { whichSkill = value; } }

    private bool isChoosePriorityOpen; // value to know if choose priority panel open
    public bool IsChoosePriorityOpen { get { return isChoosePriorityOpen; } set { isChoosePriorityOpen = value; } }

    // skill info
    private TargetType[] targetType;
    private StatType priorityStat;
    private int numberOfTargetType;
    private int numberOfAllyTargets;
    private int numberOfEnemyTargets;
    private bool isGroupEnemy;
    private bool isGroupAlly;
    private int layer;

    // value to know what type of target can select
    // 1 -> Enemy / 2 -> Ally / 3 -> Both / 4 -> Self or Ally
    private int selectType;

    // value to control when to open choose dialog when need to open dialog 2 times
    private bool isFinish;
    public bool IsFinish { set { isFinish = value; } }

    // value to know when player can select target
    private bool canSelectTarget;
    public bool CanSelectTarget { set { canSelectTarget = value; } }

    private void Awake()
    {
        autoFindTargets = FindObjectOfType<AutoFindTargets>();

        choosePriorityPanel.gameObject.SetActive(false);

        choosePriorityText = choosePriorityPanel.GetChild(0)
            .gameObject.GetComponent<TextMeshProUGUI>();

        isFinish = false;
        canSelectTarget = false;
        isChoosePriorityOpen = false;
    }

    private void Update()
    {
        SelectSingleTarget();
    }

    private void SelectSingleTarget()
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
                    UpdateTargetListBasedOnSelect(clickedObject);
                    autoFindTargets.TurnOnShowTargets();
                }
            }
        }
    }

    public void ChoosingLowestPriority()
    {
        AutoFindTargetsBasedOnPriority(true);
    }

    public void ChoosingHighestPriority()
    {
        AutoFindTargetsBasedOnPriority(false);
    }

    private void AutoFindTargetsBasedOnPriority(bool isLow)
    {
        if (layer == 6)
            autoFindTargets.AutoFindTargetsBasedOnPriority
                (numberOfAllyTargets, 6, priorityStat, isLow);
        else
            autoFindTargets.AutoFindTargetsBasedOnPriority
                (numberOfEnemyTargets, 7, priorityStat, isLow);

        choosePriorityPanel.gameObject.SetActive(false);
        isFinish = true;
        autoFindTargets.TurnOnShowTargets();
        isChoosePriorityOpen = false;
    }

    private void UpdateTargetListBasedOnSelect(GameObject clickedObject)
    {
        // can select enemy 
        if ((selectType == 1) && clickedObject.layer == 7)
        {
            autoFindTargets.TurnOffShowTargets();
            autoFindTargets.EnemyTargets.Clear();
            autoFindTargets.EnemyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
            if (isGroupEnemy)
                autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfEnemyTargets, 7, priorityStat);
        }

        // can select ally
        if ((selectType == 2) && clickedObject.layer == 6)
        {
            autoFindTargets.TurnOffShowTargets();
            autoFindTargets.AllyTargets.Clear();
            autoFindTargets.AllyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
            if (isGroupAlly)
                autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfAllyTargets, 6, priorityStat);

        }

        // can select ally or enemy
        if (selectType == 3)
        { 
            if(clickedObject.layer == 7)
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.EnemyTargets.Clear();
                autoFindTargets.EnemyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                if (isGroupEnemy)
                    autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfEnemyTargets, 7, priorityStat);
            }

            if(clickedObject.layer == 6)
            {
                autoFindTargets.AllyTargets.Clear();
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.AllyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                if (isGroupAlly)
                    autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfAllyTargets, 6, priorityStat);
            }
        }

        // can select self or ally
        if (selectType == 4)
        {
            if (clickedObject.layer == 8) // select self
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.SelfTarget = champion;
                autoFindTargets.AllyTargets.Clear();
            }

            if (clickedObject.layer == 6) // select ally
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.AllyTargets.Clear();
                autoFindTargets.AllyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                autoFindTargets.SelfTarget = null;
            }
        }

        // can select self or ally and enemy
        if(selectType == 5)
        {
            if (clickedObject.layer == 8) // select self
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.SelfTarget = champion;
                autoFindTargets.AllyTargets.Clear();
            }

            if (clickedObject.layer == 6) // select ally
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.AllyTargets.Clear();
                autoFindTargets.AllyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
                autoFindTargets.SelfTarget = null;
            }

            if (clickedObject.layer == 7)
            {
                autoFindTargets.TurnOffShowTargets();
                autoFindTargets.EnemyTargets.Clear();
                autoFindTargets.EnemyTargets.Add(clickedObject.GetComponent<OnFieldCharacter>());
            }
        }
    }

    private void GetSkillInfo()
    {
        // count how many type of target that skill can affect to
        numberOfTargetType = champion.Skills[whichSkill].TargetTypes.Count();

        // priority stat for auto find
        priorityStat = champion.Skills[whichSkill].PriorityStat;

        // count how many enemies
        numberOfEnemyTargets = champion.Skills[whichSkill].NumberOfEnemyTargets;

        // count how many ally
        numberOfAllyTargets = champion.Skills[whichSkill].NumberOfAllyTargets;

        // check group enemy
        isGroupEnemy = champion.Skills[whichSkill].IsGroupEnemy;

        // check group ally
        isGroupAlly = champion.Skills[whichSkill].IsGroupAlly;

        // which type of target skill affected to
        targetType = champion.CurrentCharacter.Skills[whichSkill].TargetTypes;
    }

    public void CheckInfoToAutoFindTargets(bool isCombatSkillMenu, bool isTaunted, OnFieldCharacter taunter)
    {
        GetSkillInfo();
        
        if (numberOfTargetType == 1)
        {
            if (targetType[0] == TargetType.Self) // skill just affected self
            {
                autoFindTargets.SelfTarget = champion;
                autoFindTargets.TurnOnShowTargets();
            }
            else if (targetType[0] == TargetType.SelfOrAlly) // skill affected self or ally
                // auto find 1 enemy but can select self
                // layer should be 6 or 7
                // but i set 0 cause self don't need layer
                AutoFind1EnemyOrAllyOrGroup(true, 0, 4, true, false);

            else if (targetType[0] == TargetType.Enemy)
            {
                if (numberOfEnemyTargets == 1)
                    // auto find 1 target which low priority
                    // 1 target - enemy layer - priority stat - lowest stat
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 1, true, false);
                else
                {
                    if(!isGroupEnemy) // enemies not next to others
                        // auto find enemies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);
                    else // enemies next to others
                        AutoFind1EnemyOrAllyOrGroup(false,7,1,true, true);
                }

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, false, 0);
            }
            else // target type = ally
            {
                if (numberOfAllyTargets == 1)
                    // auto find 1 target which low priority
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 2, true, false);
                else
                {
                    if(!isGroupAlly)
                        // auto find allies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);
                    else
                        AutoFind1EnemyOrAllyOrGroup(false, 6, 2, true, true);
                }
            }
        }
        else if (numberOfTargetType == 2) // number of target type = 2
        {
            // skill just affected ally and enemy not self
            if (targetType.Contains(TargetType.Ally) && targetType.Contains(TargetType.Enemy))
            {
                // skill affect 1 ally and 1 enemy
                if (numberOfAllyTargets == numberOfEnemyTargets && numberOfAllyTargets == 1)
                {
                    // auto find 1 enemy 1 ally which low priority
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 3, false, false);
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 3, true, false);

                    // replace target list with taunter (cannot select enemy)
                    AutoFindTauntTargets(isTaunted, taunter, true, 2);
                }
                else if (numberOfEnemyTargets == 1 && numberOfAllyTargets > 1)
                {
                    // auto find 1 enemy
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 1, false, false);

                    if(!isGroupAlly)
                        // auto find allies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);
                    else
                        // auto find group allies
                        AutoFind1EnemyOrAllyOrGroup(false, 6, 3, true, true);

                    // replace target list with taunter (cannot select enemy)
                    AutoFindTauntTargets(isTaunted, taunter, true, 2);
                }
                else if (numberOfAllyTargets == 1 && numberOfEnemyTargets > 1)
                {
                    // auto find 1 ally
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 2, false, false);

                    if(!isGroupEnemy)
                        // auto find enemies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);
                    else
                        AutoFind1EnemyOrAllyOrGroup(false, 7, 3, true, true);

                    // replace target list with taunter (cannot select enemy)
                    AutoFindTauntTargets(isTaunted, taunter, true, 2);
                }
                else // number of enemy > 1 and number of ally > 1
                {
                    if (!isGroupEnemy && !isGroupAlly)
                    {
                        // auto find targets (allies and enemies)
                        AutoFindOver1EnemyAndAlly(isCombatSkillMenu);

                        // replace target list with taunter (cannot select enemy)
                        AutoFindTauntTargets(isTaunted, taunter, false, 0);
                    }
                    else if (!isGroupEnemy && isGroupAlly)
                    {
                        AutoFind1EnemyOrAllyOrGroup(false, 6, 2, false, true);
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);

                        // replace target list with taunter (cannot select enemy)
                        AutoFindTauntTargets(isTaunted, taunter, true, 2);
                    }
                    else if (isGroupEnemy && !isGroupAlly)
                    {
                        AutoFind1EnemyOrAllyOrGroup(false, 7, 1, false, true);
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);

                        // replace target list with taunter (cannot select enemy)
                        AutoFindTauntTargets(isTaunted, taunter, false, 0);
                    }
                    else // group enemies and allies
                    {
                        AutoFind1EnemyOrAllyOrGroup(false, 7, 3, false, true);
                        AutoFind1EnemyOrAllyOrGroup(false, 6, 3, true, true);

                        // replace target list with taunter (cannot select enemy)
                        AutoFindTauntTargets(isTaunted, taunter, true, 2);
                    }
                }
            }
            //skill affected self and enemy
            else if (targetType.Contains(TargetType.Enemy) && targetType.Contains(TargetType.Self))
            {
                autoFindTargets.SelfTarget = champion;

                if (numberOfEnemyTargets == 1) // skill affected self and 1 enemy
                    // auto find 1 enemy
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 1, true, false);
                else // skill affected self and >1 enemy
                {
                    if (!isGroupEnemy) // enemies not next to others
                        // auto find enemies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);
                    else // enemies next to others
                        AutoFind1EnemyOrAllyOrGroup(false, 7, 1, true, true);
                }

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, false, 0);
            }
            // skill affected self and ally
            else if (targetType.Contains(TargetType.Enemy) && targetType.Contains(TargetType.Self)) 
            {
                autoFindTargets.SelfTarget = champion;

                if (numberOfAllyTargets == 1)
                    // auto find 1 target which low priority
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 2, true, false);
                else
                {
                    if (!isGroupAlly)
                        // auto find allies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);
                    else
                        AutoFind1EnemyOrAllyOrGroup(false, 6, 2, true, true);
                }
            }
            else // skill affected self or ally and enemy
            {
                autoFindTargets.SelfTarget = champion;

                if (numberOfEnemyTargets == 1) // skill affected self and 1 enemy
                    // auto find 1 enemy
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 5, true, false);
                else // skill affected self and >1 enemy
                {
                    if (!isGroupEnemy) // enemies not next to others
                        // auto find enemies
                        AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);
                    else // enemies next to others
                        AutoFind1EnemyOrAllyOrGroup(false, 7, 5, true, true);
                }

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, true, 4);
            }
        }
        else // number of target = 3
        {
            // skill affected self, 1 ally, 1 enemy
            if (numberOfAllyTargets == numberOfEnemyTargets && numberOfAllyTargets == 1)
            {
                autoFindTargets.SelfTarget = champion;

                // auto find 1 enemy 1 ally which low priority
                AutoFind1EnemyOrAllyOrGroup(false, 6, 3, false, false);
                AutoFind1EnemyOrAllyOrGroup(false, 7, 3, true, false);

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, true, 2);
            }
            // skill affected self, > 1 ally, 1 enemy
            else if (numberOfEnemyTargets == 1 && numberOfAllyTargets > 1)
            {
                autoFindTargets.SelfTarget = champion;

                // auto find 1 enemy
                AutoFind1EnemyOrAllyOrGroup(false, 7, 1, false, false);

                // auto find allies
                AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, false, 0);
            }
            // skill affected self, 1 ally, > 1 enemy
            else if (numberOfAllyTargets == 1 && numberOfEnemyTargets > 1)
            {
                autoFindTargets.SelfTarget = champion;

                // auto find 1 ally
                AutoFind1EnemyOrAllyOrGroup(false, 6, 2, false, false);

                // auto find enemies
                AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);

                // replace target list with taunter
                AutoFindTauntTargets(isTaunted, taunter, true, 2);
            }
            else // number of enemy > 1 and number of ally > 1 and self
            {
                autoFindTargets.SelfTarget = champion;

                if (!isGroupEnemy && !isGroupAlly)
                {
                    // auto find targets (allies and enemies)
                    AutoFindOver1EnemyAndAlly(isCombatSkillMenu);

                    // replace target list with taunter
                    AutoFindTauntTargets(isTaunted, taunter, false, 0);
                }
                else if (!isGroupEnemy && isGroupAlly)
                {
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 2, false, true);
                    AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 7);

                    // replace target list with taunter
                    AutoFindTauntTargets(isTaunted, taunter, true, 2);
                }
                else if (isGroupEnemy && !isGroupAlly)
                {
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 1, false, true);
                    AutoFindOver1EnemyOrAlly(isCombatSkillMenu, 6);

                    // replace target list with taunter
                    AutoFindTauntTargets(isTaunted, taunter, false, 0);
                }
                else // group enemies and allies
                {
                    AutoFind1EnemyOrAllyOrGroup(false, 7, 3, false, true);
                    AutoFind1EnemyOrAllyOrGroup(false, 6, 3, true, true);

                    // replace target list with taunter
                    AutoFindTauntTargets(isTaunted, taunter, true, 2);
                }
            }
        }
    }

    #region Reduce Code For Check Info For Auto Find Targets

    // auto find 1 target (Enemy or Ally) with lowest priority stat
    private void AutoFind1EnemyOrAllyOrGroup(bool isSelf,int layer, int selectType, bool canTurnOnShowTargets,bool isGroup)
    {
        // check if target is self or other
        if (isSelf)
            autoFindTargets.SelfTarget = champion;
        else
            autoFindTargets.AutoFindTargetsBasedOnPriority(1, layer, priorityStat, true);

        // check if targets next to each other
        if (isGroup)
            if(layer == 7)
                autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfEnemyTargets, 7, priorityStat);
            else
                autoFindTargets.AutoFindGroupTargetsBasedOnPriority(numberOfAllyTargets, 6, priorityStat);

        // check if can select target and which type of target can select
        canSelectTarget = true;
        this.selectType = selectType;

        // check if need to show targets found UI 
        if (canTurnOnShowTargets)
            autoFindTargets.TurnOnShowTargets();
    }


    private void AutoFindOver1EnemyOrAlly(bool isCombatSkillMenu, int layer)
    {
        if (isCombatSkillMenu)
            // open choose priority dialog
            OpenChoosePriorityDialog(layer);
        else
        {
            this.layer = layer;
            ChoosingLowestPriority();
        }
    }  
    
    private void AutoFindOver1EnemyAndAlly(bool isCombatSkillMenu)
    {
        if (isCombatSkillMenu)
            // open choose priority dialog 2 times
            StartCoroutine(OpenDialog2Times());
        else
        {
            layer = 7;
            ChoosingLowestPriority();
            layer = 6;
            ChoosingLowestPriority();
        }
    }

    private void AutoFindTauntTargets(bool isTaunted, OnFieldCharacter taunter, bool canSelect, int selectType)
    {
        if (isTaunted)
        {
            // replace enemy target list with taunter
            for (int i = 0; i < numberOfEnemyTargets; i++)
                autoFindTargets.EnemyTargets[i] = taunter;

            // update select type
            if (!canSelect)
                canSelectTarget = false;
            else
                this.selectType = selectType;

            // reset show targets UI
            autoFindTargets.TurnOffShowTargets();
            autoFindTargets.TurnOnShowTargets();
        }
    }
    #endregion

    #region Open Priority Dialog 2 times
    private void OpenChoosePriorityDialog(int layer)
    {
        isChoosePriorityOpen = true;
        this.layer = layer;

        string targets = "";
        if (layer == 6) targets = "allies";
        else targets = "enemies";

        choosePriorityPanel.gameObject.SetActive(true);
        choosePriorityText.text = $"Do you want to choose the {targets} with the lowest or highest {priorityStat} points?";
    }

    private IEnumerator OpenDialog2Times()
    {
        OpenChoosePriorityDialog(7);
        yield return StartCoroutine(AwaitFinishChoosing());
        OpenChoosePriorityDialog(6);
    }

    private IEnumerator AwaitFinishChoosing()
    {
        while (!isFinish)
        {
            yield return null;
        }
    }
    #endregion
}
