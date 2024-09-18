using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateToPlayAnimation : MonoBehaviour
{
    private GameplayController gameplayController;
    private CheckSkillAnimationController checkSkillAnimationController;
    private List<OnFieldCharacter> targets;
    private List<Animator> animators;
    private Vector3 characterOriginalPosition;
    private Vector3 characterOriginalRotation;

    private void Awake()
    {
        gameplayController = FindObjectOfType<GameplayController>();

        // enemy targets list will be clear when finish using skill
        // so make a clone list to not encounter bug
        targets = new List<OnFieldCharacter>();

        // List to hold the animators of the targets
        animators = new List<Animator>();
    }

    private void Start()
    {
        checkSkillAnimationController = FindObjectOfType<CheckSkillAnimationController>();

        // value to store character position
        characterOriginalPosition = gameObject.transform.position;

        // value to store character rotation
        characterOriginalRotation = gameObject.transform.eulerAngles;
    }

    //character move to target for attack and back to original position
    public IEnumerator MoveToPointAndBack(Vector3 endPosition,
        float distanceFromEnd, string triggerName, Animator animator)
    {
        // calculate the position in front of target position
        Vector3 directionToEnd = (endPosition - characterOriginalPosition).normalized;
        Vector3 targetPosition = endPosition - directionToEnd * distanceFromEnd;

        // Run to end point
        yield return StartCoroutine(MoveToPoint(characterOriginalPosition, targetPosition,animator));

        animator.SetTrigger(triggerName);
        yield return new WaitForSeconds(2f);

        // Back to start point
        yield return StartCoroutine(MoveToPoint(targetPosition, characterOriginalPosition, animator));

        ResetChampionTransform();

        // start new turn
        gameplayController.StartTurn();
    }

    // character move to target for attack
    private IEnumerator MoveToPoint(Vector3 startPosition, Vector3 endPosition, Animator animator)
    {
        // set start position
        gameObject.transform.position = startPosition;

        // play run to target animation
        if (animator != null)
        {
            animator.SetBool("Running", true);
        }

        // calculate distance
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        // move to end point while distance > 0.5f
        while (Vector3.Distance(gameObject.transform.position, endPosition) > 0.5f)
        {
            float distanceCovered = (Time.time - startTime) * 9f;
            float fractionOfJourney = distanceCovered / journeyLength;

            gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            gameObject.transform.LookAt(endPosition);

            yield return null;
        }

        // set end position
        gameObject.transform.position = endPosition;

        // stop animation
        if (animator != null)
        {
            animator.SetBool("Running", false);
        }
    }

    public IEnumerator UsingSkillAndBackToIdle(string parameterName, float animationDuration, Animator animator)
    {
        animator.SetBool(parameterName, true); // play skill animation
        yield return new WaitForSeconds(animationDuration);

        animator.SetBool(parameterName, false); // play idle animation
        yield return new WaitForSeconds(0.5f);

        // reset champion transform
        ResetChampionTransform();

        // start new turn
        gameplayController.StartTurn();
    }

    public IEnumerator BeingAttackedAndBackToIdle(float animationDuration, List<OnFieldCharacter> enemyTargets)
    {
        targets = new List<OnFieldCharacter>(enemyTargets);

        if (checkSkillAnimationController != null)
        {
            animators = new List<Animator>();

            foreach (var target in targets)
            {
                // get target skill controller script and animator controller
                Component targetSkillController = checkSkillAnimationController.GetCharacterSkillController(target);
                Animator targetAnimator = checkSkillAnimationController.CheckWhoseAnimationControllerToGetAnimator(targetSkillController);

                if (targetSkillController != null && targetAnimator != null)
                {
                    animators.Add(targetAnimator);
                }
            }

            // Start animations on all animators
            foreach (var animator in animators)
            {
                animator.SetBool("Being Attacked", true); // Play being attacked animation
            }

            // Wait for the duration of the animation
            yield return new WaitForSeconds(animationDuration);

            // Stop animations on all animators
            foreach (var animator in animators)
            {
                animator.SetBool("Being Attacked", false); // Play idle animation
            }

            // reset enemies position.y and y rotation
            foreach (var target in targets)
            {
                ResetEnemiesYValue(target.gameObject);
            }
        }
    }

    public void PlayDeathAnimation()
    {
        // check enemy death to play death animation
        foreach (var target in targets)
            if (target.CurrentHealth <= 0)
                foreach (var animator in animators)
                    animator.SetTrigger("Death");
    }

    private void ResetChampionTransform()
    {
        // reset champion position
        gameObject.transform.position = characterOriginalPosition;

        // reset champion y rotation
        gameObject.transform.eulerAngles = characterOriginalRotation;
    }

    private void ResetEnemiesYValue(GameObject target)
    {
        // reset y position
        target.transform.position =
            new Vector3(
                target.transform.position.x,
                0,
                target.transform.position.z);

        // reset y rotation
        if (target.layer == 7)
            target.transform.eulerAngles =
                new Vector3(0, 180f, 0);
        else
            target.transform.eulerAngles = Vector3.zero;
    }
}
