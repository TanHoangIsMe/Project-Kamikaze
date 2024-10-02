using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrielAPlotexiaSkillController : MonoBehaviour, IAnimationPlayable
{
    private CombatSkillMenu combatSkillMenu;
    private SkillHandler skillHandler;
    private CalculateToPlayAnimation calculateToPlayAnimation;
    private PlayLastAnimation playLastAnimation;

    private Animator animator;

    private List<OnFieldCharacter> enemyTargets;

    private bool isAnimating = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        calculateToPlayAnimation = GetComponent<CalculateToPlayAnimation>();
        playLastAnimation = GetComponent<PlayLastAnimation>();
    }

    private void Start()
    {
        combatSkillMenu = FindObjectOfType<CombatSkillMenu>();
        skillHandler = FindObjectOfType<SkillHandler>();
    }

    public void SetEnemyTargets(List<OnFieldCharacter> targets)
    {
        enemyTargets = targets;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void SetIsAnimating(bool isAnimating)
    {
        this.isAnimating = isAnimating;
    }

    public void PlayFirstSkillAnimation()
    {
        if (enemyTargets != null && !isAnimating)
        {
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            isAnimating = true;
            StartCoroutine(calculateToPlayAnimation.MoveToPointAndBack(
                enemyTargets, 3f, "Using First Skill", animator));

            playLastAnimation.EnemyTargets = enemyTargets;
        }
    }

    public void PlaySecondSkillAnimation()
    {
        if (enemyTargets != null && !isAnimating)
        {
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            isAnimating = true;
            StartCoroutine(calculateToPlayAnimation.UsingSkillAndBackToIdle(enemyTargets, "Using Second Skill", 2.03f, animator));

            playLastAnimation.EnemyTargets = enemyTargets;
        }
    }

    public void PlayBurstSkillAnimation()
    {
        if (enemyTargets != null && !isAnimating)
        {
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            isAnimating = true;
            StartCoroutine(calculateToPlayAnimation.UsingSkillAndBackToIdle(enemyTargets, "Using Burst Skill", 2.26f, animator));

            playLastAnimation.EnemyTargets = enemyTargets;
        }
    }
}
