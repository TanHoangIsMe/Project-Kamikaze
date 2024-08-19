using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatSkillMenu : MonoBehaviour
{
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
        ChangeLayerToSelf();
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = 0;
        checkNumberOfTargets.CheckInfoToAutoFindTargets();
    }

    // Function for pressing Skill 2 button
    public void UsingSkill2()
    {
        ChangeLayerToSelf();
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = 1;
        checkNumberOfTargets.CheckInfoToAutoFindTargets();
    }

    // Function for pressing Skill Burst button
    public void UsingSkillBurst()
    {
        ChangeLayerToSelf();
        checkNumberOfTargets.Champion = champion;
        checkNumberOfTargets.WhichSkill = 2;
        checkNumberOfTargets.CheckInfoToAutoFindTargets();
    }

    public void AttackConfirm()
    {
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
            else if(checkNumberOfTargets.WhichSkill == 1) // using skill 2
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

            gameObject.SetActive(false);

            // turn off can select target
            checkNumberOfTargets.IsFinish = false;
            checkNumberOfTargets.CanSelectTarget = false;

            // set champion layer back
            champion.gameObject.layer = championLayer;

            // start next character turn
            gameplayController.StartTurn();
            //string a = "";
            //OnFieldCharacter[] b = FindObjectsOfType<OnFieldCharacter>();
            //foreach (OnFieldCharacter c in b)
            //{
            //    a += c.name + "-" + c.CurrentHealth + "-" + c.CurrentArmor;
            //}
            //Debug.Log(a);
        }
        else
        {
            Debug.Log("Please choose a skill");
            Debug.Log(autoFindTargets.EnemyTargets.Count() + "-"
            + autoFindTargets.AllyTargets.Count() + "-"
            + autoFindTargets.SelfTarget);
        }
    }
}
