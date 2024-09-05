using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrielAPlotexiaSkillController : MonoBehaviour
{
    private CombatSkillMenu combatSkillMenu;
    private SkillHandler skillHandler;
    private CalculateToPlayAnimation calculateToPlayAnimation;
    private PlayLastAnimation playLastAnimation;

    private Animator animator;
    public Animator Animator { get { return animator; } }

    private List<OnFieldCharacter> enemyTargets;
    public List<OnFieldCharacter> EnemyTargets { set { enemyTargets = value; } }

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

    public void PlayFirstSkillAnimation()
    {
        if (enemyTargets != null)
        {
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.MoveToPointAndBack(gameObject.transform.position,
                enemyTargets[0].gameObject.transform.position, 5f, "Using First Skill", animator));

            playLastAnimation.EnemyTargets = enemyTargets;
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
                enemyTargets[0].gameObject.transform.position, 3.5f, "Using Second Skill", animator));

            playLastAnimation.EnemyTargets = enemyTargets;
        }
    }

    public void PlayBurstSkillAnimation()
    {
        if (enemyTargets != null)
        {
            if (combatSkillMenu != null)
                // turn off combat skill menu canvas
                combatSkillMenu.gameObject.SetActive(false);

            StartCoroutine(calculateToPlayAnimation.UsingSkillAndBackToIdle("Using Burst Skill", 4.30f, animator));

            playLastAnimation.EnemyTargets = enemyTargets;
        }
    }
}
