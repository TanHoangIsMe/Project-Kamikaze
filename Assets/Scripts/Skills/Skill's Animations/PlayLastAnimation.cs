using System.Collections.Generic;
using UnityEngine;

public class PlayLastAnimation : MonoBehaviour
{
    private SkillHandler skillHandler;
    private CalculateToPlayAnimation calculateToPlayAnimation;

    private List<OnFieldCharacter> enemyTargets;
    public List<OnFieldCharacter> EnemyTargets { set { enemyTargets = value; } }

    private void Awake()
    {
        calculateToPlayAnimation = GetComponent<CalculateToPlayAnimation>();
    }

    private void Start()
    {
        skillHandler = FindObjectOfType<SkillHandler>();
    }

    public void SendInfoFirstSkill()
    {
        if (skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));

            skillHandler.SendInfoToUsingFirstSkill();

            calculateToPlayAnimation.PlayDeathAnimation(0, skillHandler);
        }
    }

    public void SendInfoSecondSkill()
    {
        if (skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));
            
            skillHandler.SendInfoToUsingSecondSkill();

            calculateToPlayAnimation.PlayDeathAnimation(1, skillHandler);
        }
    }

    public void SendInfoBurstSkill()
    {
        if (skillHandler != null)
        {
            // play target being attacked animation
            StartCoroutine(calculateToPlayAnimation.BeingAttackedAndBackToIdle(1f, enemyTargets));

            skillHandler.SendInfoToUsingBurstSkill();

            calculateToPlayAnimation.PlayDeathAnimation(2, skillHandler);
        }
    }
}
