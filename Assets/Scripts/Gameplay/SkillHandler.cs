using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    [SerializeField] private GameObject skillMenu;
    [SerializeField] private GameObject popUpDamageText;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private bool isCombatSkillMenu;
    public bool IsCombatSkillMenu { set { isCombatSkillMenu = value; } }

    private CheckNumberOfTargets checkNumberOfTargets;
    private AutoFindTargets autoFindTargets;
    private CheckSkillAnimationController checkSkillAnimationController;

    private List<float> skillValues;
    public List<float> SkillValues { set { skillValues = value; } }

    private void Awake()
    {
        checkNumberOfTargets = GetComponent<CheckNumberOfTargets>();
        autoFindTargets = GetComponent<AutoFindTargets>();
        checkSkillAnimationController = GetComponent<CheckSkillAnimationController>();

        skillValues = new List<float>();
    }

    private void ChangeLayerToSelf()
    {
        if (champion != null)
            champion.gameObject.layer = 8;
        else
            Debug.Log("Something wrong in change layer to self");
    }

    private void SetUpToAutoFindTargets(int whichSkill)
    {
        // change champion layer = 8 - self
        ChangeLayerToSelf();

        // set up information need to auto find targets 
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = whichSkill;
        checkNumberOfTargets.CheckInfoToAutoFindTargets(isCombatSkillMenu);
        
        // if player champion using skill show UI
        if (isCombatSkillMenu) 
        {
            // clear all selected ring and turn on based on targets
            autoFindTargets.TurnOffShowTargets();
            autoFindTargets.TurnOnShowTargets();
        }
    }

    private void ResetThings()
    {
        // turn off can select target
        checkNumberOfTargets.IsFinish = false;
        checkNumberOfTargets.CanSelectTarget = false;

        // reset target lists
        ClearTargetsList();

        // turn off show targets
        autoFindTargets.TurnOffShowTargets();

        if (isCombatSkillMenu)
        {
            // set champion layer back
            champion.gameObject.layer = 6;           
        }
        else
        {
            // set champion layer back
            champion.gameObject.layer = 7;
        }
    }

    private void ClearTargetsList()
    {
        autoFindTargets.AllyTargets.Clear();
        autoFindTargets.EnemyTargets.Clear();
        autoFindTargets.SelfTarget = null;
    }

    private void ResetCheckNumberOfTargetsFlags()
    {
        checkNumberOfTargets.ChoosePriorityPanel.gameObject.SetActive(false);
        checkNumberOfTargets.IsFinish = false;
        checkNumberOfTargets.IsChoosePriorityOpen = false;
        checkNumberOfTargets.CanSelectTarget = false;
    }

    #region Using Skill
    public void UsingSkill1Click()
    {
        UsingSkill1();
    }

    public void UsingSkill2Click()
    {
        UsingSkill2();
    }

    public void UsingSkillBurstClick()
    {
        UsingSkillBurst();
    }

    public bool UsingSkill1()
    {
        if (champion.CurrentMana > champion.Skills[0].ManaCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlags();
            ClearTargetsList();

            SetUpToAutoFindTargets(0);
            return true;
        }
        else
        {
            Debug.Log("Not enough mana to use skill");
            return false;
        }
    }

    public bool UsingSkill2()
    {
        if (champion.CurrentMana > champion.Skills[1].ManaCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlags();
            ClearTargetsList();

            SetUpToAutoFindTargets(1);
            return true;
        }
        else
        {
            Debug.Log("Not enough mana to use skill");
            return false;
        }
    }

    public bool UsingSkillBurst()
    {
        if (champion.CurrentBurst == champion.Skills[2].BurstCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlags();
            ClearTargetsList();

            SetUpToAutoFindTargets(2);
            return true;
        }
        else
        {
            Debug.Log("Not enough burst to use skill");
            return false;
        }
    }

    public void AttackConfirm()
    {
        if (autoFindTargets.EnemyTargets.Count() > 0 ||
            autoFindTargets.AllyTargets.Count() > 0 ||
            autoFindTargets.SelfTarget != null)
        {
            if(isCombatSkillMenu)
                // turn off skill menu
                skillMenu.SetActive(false);

            List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;

            if (checkNumberOfTargets.WhichSkill == 0) // using skill 1
            {
                // play animation
                checkSkillAnimationController.
                    GetSkillAnimationControllerForPlayAnimation(champion, enemies, 0);

            }
            else if (checkNumberOfTargets.WhichSkill == 1) // using skill 2
            {
                // play animation
                checkSkillAnimationController.
                    GetSkillAnimationControllerForPlayAnimation(champion, enemies, 1);
            }
            else // using burst
            {
                // play animation
                checkSkillAnimationController.
                    GetSkillAnimationControllerForPlayAnimation(champion, enemies, 2);

            }
        }
        else
        {
            Debug.Log("Please choose a skill");
        }       
    }

    public void SendInfoToUsingFirstSkill()
    {
        List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
        List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;

        if (enemies.Count() > 0 && allies.Count() > 0)
            champion.UsingFirstSkill(this, enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingFirstSkill(this, enemyTargets: enemies);
        else
            champion.UsingFirstSkill(this, allyTargets: allies);

        // play health bar reduce or increase animation
        // when champion current health change
        PlayHealthBarEffect(enemies, allies);

        ResetThings();
    }

    public void SendInfoToUsingSecondSkill()
    {
        List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
        List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;

        if (enemies.Count() > 0 && allies.Count() > 0)
            champion.UsingSecondSkill(this, enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingSecondSkill(this, enemyTargets: enemies);
        else
            champion.UsingSecondSkill(this, allyTargets: allies);

        // play health bar reduce or increase animation
        // when champion current health change
        PlayHealthBarEffect(enemies, allies);

        ResetThings();
    }

    public void SendInfoToUsingBurstSkill()
    {
        List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
        List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;

        if (enemies.Count() > 0 && allies.Count() > 0)
            champion.UsingBurstSkill(this, enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingBurstSkill(this, enemyTargets: enemies);
        else
            champion.UsingBurstSkill(this, allyTargets: allies);

        // play health bar reduce or increase animation
        // when champion current health change
        PlayHealthBarEffect(enemies, allies);

        ResetThings();
    }
    #endregion

    #region OverHead Champion UI
    private void PlayHealthBarEffect(List<OnFieldCharacter> enemies, List<OnFieldCharacter> allies)
    {
        // play health bar fill animation on enemies
        int totalEnemies = enemies.Count();
        for (int i = 0; i < totalEnemies; i++)
        {
            PlayPopUpDamageText(enemies, i);

            // Play Update Health Bar Animation
            enemies[i].gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
        }

        // play health bar fill animation on allies
        foreach (var ally in allies)
        {
            ally.gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
        }
    }

    private void PlayPopUpDamageText(List<OnFieldCharacter> enemies, int i)
    {
        // get over head position of enemy 
        Vector3 overHeadEnemyPosition = enemies[i].gameObject.transform.position + Vector3.up * 3.2f;
        Vector3 randomOverHeadPosition = overHeadEnemyPosition + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);

        // spawn damage text
        GameObject popUp = Instantiate(popUpDamageText, randomOverHeadPosition, Quaternion.identity);
        TextMeshProUGUI popUpText = popUp.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        popUpText.text = skillValues[i].ToString();

        ChangeTextColorBasedOnElement(i, popUpText);
    }

    private void ChangeTextColorBasedOnElement(int i, TextMeshProUGUI popUpText)
    {
        // get character elements info
        ElementType[] elementTypes = champion.CurrentCharacter.ElementTypes;
        ElementType elementType = new ElementType();
        int numberOfElements = elementTypes.Length;

        if (numberOfElements == 1) // character has 1 element
        {
            elementType = elementTypes[0];
        }
        else // character has > 1 elements
        {
            GetRandomElement(elementTypes, elementType, numberOfElements);
        }

        ChangeTextColor(popUpText, elementType);
    }

    private static void ChangeTextColor(TextMeshProUGUI popUpText, ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                popUpText.color = Color.red;
                break;
            case ElementType.Water:
                popUpText.color = Color.blue;
                break;
            case ElementType.Nature:
                popUpText.color = Color.green;
                break;
            case ElementType.Light:
                popUpText.color = Color.yellow;
                break;
            case ElementType.Dark: // purple color
                popUpText.color = new Color(180f / 255f, 0f / 255f, 255f / 255f);
                break;
            case ElementType.Mystic:
                popUpText.color = Color.magenta;
                break;
        }
    }

    private void GetRandomElement(ElementType[] elementTypes, ElementType elementType, int numberOfElements)
    {
        // create random object
        System.Random random = new System.Random();
        // create random number based on number of element
        int index = random.Next(numberOfElements);
        // get random element
        elementType = elementTypes[index];
    }
    #endregion
}
