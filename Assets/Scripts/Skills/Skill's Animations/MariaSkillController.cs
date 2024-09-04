using System.Collections.Generic;
using UnityEngine;

public class MariaSkillController : MonoBehaviour
{
    CombatSkillMenu combatSkillMenu;
    SkillHandler skillHandler;
    CalculateToPlayAnimation calculateToPlayAnimation;

    private Animator animator;
    public Animator Animator {  get { return animator; } }

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
        skillHandler = FindObjectOfType<SkillHandler>();       
    }

    public void PlayFirstSkillAnimation()
    {
        if (enemyTargets != null)
        {
            if(combatSkillMenu != null)
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
            if (combatSkillMenu != null)
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
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.UsingSkillAndBackToIdle("Using Burst Skill", 4.30f,animator));
        }
    }

    public void SendInfoFirstSkill()
    {
        if(skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));

            skillHandler.SendInfoToUsingFirstSkill();

            calculateToPlayAnimation.PlayDeathAnimation();
        }
    }

    public void SendInfoSecondSkill()
    {
        if (skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));

            skillHandler.SendInfoToUsingFirstSkill();

            calculateToPlayAnimation.PlayDeathAnimation();
        }
    }

    public void SendInfoBurstSkill()
    {
        if (skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));

            skillHandler.SendInfoToUsingBurstSkill();
        }
    }
}
