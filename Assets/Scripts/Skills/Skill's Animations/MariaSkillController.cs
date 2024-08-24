using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariaSkillController : MonoBehaviour
{
    CombatSkillMenu combatSkillMenu;
    CalculateToPlayAnimation calculateToPlayAnimation;

    private Animator animator;

    private List<OnFieldCharacter> enemyTargets;
    public List<OnFieldCharacter> EnemyTargets { set { enemyTargets = value; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        calculateToPlayAnimation = GetComponent<CalculateToPlayAnimation>();
    }

    private void Start()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
    }

    public void PlayFirstSkillAnimation()
    {
        if (enemyTargets != null)
        {
            // turn off combat skill menu canvas
            combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.MoveToPointAndBack(gameObject.transform.position,
                enemyTargets[0].gameObject.transform.position, 5f, "Using First Skill", animator));
        }
    }

    public void PlaySecondSkillAnimation()
    {
        if (enemyTargets != null)
        {
            // turn off combat skill menu canvas
            combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.MoveToPointAndBack(gameObject.transform.position, 
                enemyTargets[0].gameObject.transform.position,3.5f,"Using Second Skill", animator));
        }
    }

    public void PlayBurstSkillAnimation()
    {
        if (enemyTargets != null)
        {
            // turn off combat skill menu canvas
            combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.UsingSkillAndBackToIdle("Using Burst Skill", 4.30f,animator));
        }
    }

    public void SendInfoFirstSkill()
    {
        if(combatSkillMenu != null)
        {
            combatSkillMenu.SendInfoToUsingFirstSkill();
        }
    }

    public void SendInfoSecondSkill()
    {
        if (combatSkillMenu != null)
        {
            combatSkillMenu.SendInfoToUsingSecondSkill();
        }
    }

    public void SendInfoBurstSkill()
    {
        if (combatSkillMenu != null)
        {
            combatSkillMenu.SendInfoToUsingBurstSkill();
        }
    }
}
