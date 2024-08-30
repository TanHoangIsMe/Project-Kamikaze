using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CombatSkillMenu : MonoBehaviour
{
    [SerializeField] private GameObject chooseTargetText;
    [SerializeField] private Image skill1Avatar;
    [SerializeField] private Image skill2Avatar;
    [SerializeField] private Image skillBurstAvatar;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image manaBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI burstText;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private GameplayController gameplayController;
    private CheckNumberOfTargets checkNumberOfTargets;
    private AutoFindTargets autoFindTargets;
    private CheckSkillAnimationController checkSkillAnimationController;

    private int championLayer;

    private void Awake()
    {
        gameplayController = FindObjectOfType<GameplayController>();
        checkNumberOfTargets = GetComponent<CheckNumberOfTargets>();
        autoFindTargets = GetComponent<AutoFindTargets>();
        checkSkillAnimationController = GetComponent<CheckSkillAnimationController>();
        
        championLayer = 0;
    }

    private void ChangeLayerToSelf()
    {
        if (champion != null)
        { 
            championLayer = champion.gameObject.layer;
            champion.gameObject.layer = 8;
        }
        else
        {
            Debug.Log("champion is null");
        }
    }

    public void SetUpBarsUI()
    {
        /// <summary>
        /// Set up character choosing skill canvas UI
        /// </summary>

        // load character avatar image
        Sprite avatarSprite = Resources.Load<Sprite>(champion.CurrentCharacter.Avatar);

        if (avatarSprite != null)
        {
            characterAvatar.sprite = avatarSprite;
        }

        healthBarFill.fillAmount = champion.CurrentHealth / champion.CurrentCharacter.Health;
        manaBarFill.fillAmount = champion.CurrentMana / champion.CurrentCharacter.MaxMana;

        healthText.text = $"{champion.CurrentHealth} / {champion.CurrentCharacter.Health}";
        manaText.text = $"{champion.CurrentMana} / {champion.CurrentCharacter.MaxMana}";
        burstText.text = $"Burst: {champion.CurrentBurst / champion.CurrentCharacter.MaxBurst * 100}%";
    }

    public void SetUpSkillAvatar()
    {
        // load skill avatars
        Sprite skill1Sprite = Resources.Load<Sprite>(champion.Skills[0].Avatar);
        Sprite skill2Sprite = Resources.Load<Sprite>(champion.Skills[1].Avatar);
        Sprite skillBurstSprite = Resources.Load<Sprite>(champion.Skills[2].Avatar);

        // set avatar to skill button

        if(skill1Sprite != null)
        {
            skill1Avatar.sprite = skill1Sprite;
        }

        if (skill2Sprite != null)
        {
            skill2Avatar.sprite = skill2Sprite;
        }

        if (skillBurstSprite != null)
        {
            skillBurstAvatar.sprite = skillBurstSprite;
        }
    }

    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        if (champion.CurrentMana > champion.Skills[0].ManaCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlag();
            ClearTargetsList();

            SetUpToAutoFindTargets(0);
        }
        else
        {
            Debug.Log("Not enough mana to use skill");
        }
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        if (champion.CurrentMana > champion.Skills[1].ManaCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlag();
            ClearTargetsList();

            SetUpToAutoFindTargets(1);
        }
        else
        {
            Debug.Log("Not enough mana to use skill");
        }
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        if (champion.CurrentBurst == champion.Skills[2].BurstCost)
        {
            // Clear targets list and reset flag properties
            ResetCheckNumberOfTargetsFlag();
            ClearTargetsList();

            SetUpToAutoFindTargets(2);
        }
        else
        {
            Debug.Log("Not enough burst to use skill");
        }
    }

    private void ResetCheckNumberOfTargetsFlag()
    {
        checkNumberOfTargets.ChoosePriorityPanel.gameObject.SetActive(false);
        checkNumberOfTargets.IsFinish = false;
        checkNumberOfTargets.IsChoosePriorityOpen = false;
        checkNumberOfTargets.CanSelectTarget = false;
    }

    private void SetUpToAutoFindTargets(int whichSkill)
    {
        ChangeLayerToSelf();
        chooseTargetText.SetActive(true);
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = whichSkill;
        checkNumberOfTargets.CheckInfoToAutoFindTargets();

        // clear all selected ring and turn on based on targets
        autoFindTargets.TurnOffShowTargets();
        autoFindTargets.TurnOnShowTargets();
    }

    public void AttackConfirm()
    {
        if ( autoFindTargets.EnemyTargets.Count() > 0 || 
            autoFindTargets.AllyTargets.Count() > 0 || 
            autoFindTargets.SelfTarget != null )
        {
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
            Debug.Log(autoFindTargets.EnemyTargets.Count() + "-"
            + autoFindTargets.AllyTargets.Count() + "-"
            + autoFindTargets.SelfTarget);
        }
    }

    public void SendInfoToUsingFirstSkill()
    {
        List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
        List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;

        if (enemies.Count() > 0 && allies.Count() > 0)
            champion.UsingFirstSkill(enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingFirstSkill(enemyTargets: enemies);
        else
            champion.UsingFirstSkill(allyTargets: allies);

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
            champion.UsingSecondSkill(enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingSecondSkill(enemyTargets: enemies);
        else
            champion.UsingSecondSkill(allyTargets: allies);

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
            champion.UsingBurstSkill(enemyTargets: enemies, allyTargets: allies);
        else if (enemies.Count() > 0 && allies.Count() == 0)
            champion.UsingBurstSkill(enemyTargets: enemies);
        else
            champion.UsingBurstSkill(allyTargets: allies);

        // play health bar reduce or increase animation
        // when champion current health change
        PlayHealthBarEffect(enemies, allies);

        ResetThings();
    }

    private void ResetThings()
    {
        // turn off can select target
        checkNumberOfTargets.IsFinish = false;
        checkNumberOfTargets.CanSelectTarget = false;

        // reset target lists
        ClearTargetsList();

        // set champion layer back
        champion.gameObject.layer = championLayer;

        // turn off show targets
        autoFindTargets.TurnOffShowTargets();

        // turn off choose targets text
        chooseTargetText.SetActive(false);
    }

    private void ClearTargetsList()
    {
        autoFindTargets.AllyTargets.Clear();
        autoFindTargets.EnemyTargets.Clear();
        autoFindTargets.SelfTarget = null;
    }

    private void PlayHealthBarEffect(List<OnFieldCharacter> enemies, List<OnFieldCharacter> allies)
    {
        foreach (var enemy in enemies)
        {
            enemy.gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
        }

        foreach (var ally in allies)
        {
            ally.gameObject.GetComponent<OverHealthBar>().UpdateHealthFill();
        }
    }
}
