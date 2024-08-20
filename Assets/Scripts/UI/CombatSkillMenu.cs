using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
    [SerializeField] private GameObject chooseTargetText;

    private OnFieldCharacter champion;
    public OnFieldCharacter Champion { get { return champion; } set { champion = value; } }

    private GameplayController gameplayController;
    private CheckNumberOfTargets checkNumberOfTargets;
    private AutoFindTargets autoFindTargets;

    private int championLayer;

    private void Awake()
    {
        gameplayController = FindObjectOfType<GameplayController>();
        checkNumberOfTargets = FindObjectOfType<CheckNumberOfTargets>();
        autoFindTargets = FindObjectOfType<AutoFindTargets>();
        
        championLayer = 0;
    }

    private void Start()
    {
        chooseTargetText.SetActive(false);
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

    // Function for pressing Skill 1 button
    public void UsingSkill1()
    {
        if (champion.CurrentMana > champion.Skills[0].ManaCost)
        {
            ChangeLayerToSelf();
            chooseTargetText.SetActive(true);
            checkNumberOfTargets.Champion = champion;
            checkNumberOfTargets.WhichSkill = 0;
            checkNumberOfTargets.CheckInfoToAutoFindTargets();
            autoFindTargets.TurnOnShowTargets();
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
            ChangeLayerToSelf();
            chooseTargetText.SetActive(true);
            checkNumberOfTargets.Champion = champion;
            checkNumberOfTargets.WhichSkill = 1;
            checkNumberOfTargets.CheckInfoToAutoFindTargets();
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
            ChangeLayerToSelf();
            chooseTargetText.SetActive(true);
            checkNumberOfTargets.Champion = champion;
            checkNumberOfTargets.WhichSkill = 2;
            checkNumberOfTargets.CheckInfoToAutoFindTargets();
        }
        else
        {
            Debug.Log("Not enough burst to use skill");
        }
    }

    public void AttackConfirm()
    {
        Debug.Log(autoFindTargets.EnemyTargets.Count() + "-"
        + autoFindTargets.AllyTargets.Count()+ "-" + autoFindTargets.SelfTarget);
        if ( autoFindTargets.EnemyTargets.Count() > 0 || 
            autoFindTargets.AllyTargets.Count() > 0 || 
            autoFindTargets.SelfTarget != null )
        {
            List<OnFieldCharacter> enemies = autoFindTargets.EnemyTargets;
            List<OnFieldCharacter> allies = autoFindTargets.AllyTargets;

            if (checkNumberOfTargets.WhichSkill == 0) // using skill 1
            {
                if (enemies.Count() > 0 && allies.Count() > 0)
                    champion.UsingFirstSkill(enemyTargets: enemies, allyTargets: allies);
                else if (enemies.Count() > 0 && allies.Count() == 0)
                    champion.UsingFirstSkill(enemyTargets: enemies);
                else
                    champion.UsingFirstSkill(allyTargets: allies);
            }
            else if (checkNumberOfTargets.WhichSkill == 1) // using skill 2
            {
                if (enemies.Count() > 0 && allies.Count() > 0)
                    champion.UsingSecondSkill(enemyTargets: enemies, allyTargets: allies);
                else if (enemies.Count() > 0 && allies.Count() == 0)
                    champion.UsingSecondSkill(enemyTargets: enemies);
                else
                    champion.UsingSecondSkill(allyTargets: allies);
            }
            else // using burst
            {
                if (enemies.Count() > 0 && allies.Count() > 0)
                    champion.UsingBurstSkill(enemyTargets: enemies, allyTargets: allies);
                else if (enemies.Count() > 0 && allies.Count() == 0)
                    champion.UsingBurstSkill(enemyTargets: enemies);
                else
                    champion.UsingBurstSkill(allyTargets: allies);
            }

            ResetThings();
            foreach (var character in FindObjectsOfType<OnFieldCharacter>())
            {
                Debug.Log(character.gameObject.name + " - "
                    + character.gameObject.layer + " - "
                    + character.CurrentHealth + " - "
                    + character.CurrentArmor + " - ");
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

    private void ResetThings()
    {
        // turn off using skill panel
        gameObject.SetActive(false);

        // turn off can select target
        checkNumberOfTargets.IsFinish = false;
        checkNumberOfTargets.CanSelectTarget = false;

        // reset target lists
        autoFindTargets.AllyTargets.Clear();
        autoFindTargets.EnemyTargets.Clear();
        autoFindTargets.SelfTarget = null;

        // set champion layer back
        champion.gameObject.layer = championLayer;

        // turn off show targets
        autoFindTargets.TurnOffShowTargets();

        // turn off choose targets text
        chooseTargetText.SetActive(false);

        // start next character turn
        gameplayController.StartTurn();
    }
}
