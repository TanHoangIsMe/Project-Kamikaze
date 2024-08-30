using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateToPlayAnimation : MonoBehaviour
{
    private GameplayController gameplayController;
    private CheckSkillAnimationController checkSkillAnimationController;

    private void Awake()
    {
        gameplayController = FindObjectOfType<GameplayController>();
    }

    private void Start()
    {
        checkSkillAnimationController = FindObjectOfType<CheckSkillAnimationController>();
    }

    //character move to target for attack and back to original position
    public IEnumerator MoveToPointAndBack(Vector3 startPosition, Vector3 endPosition,
        float distanceFromEnd, string triggerName, Animator animator)
    {
        // store champion start position
        Vector3 originalPosition = startPosition;

        // calculate the position in front of target position
        Vector3 directionToEnd = (endPosition - startPosition).normalized;
        Vector3 targetPosition = endPosition - directionToEnd * distanceFromEnd;

        // Run to end point
        yield return StartCoroutine(MoveToPoint(originalPosition, targetPosition,animator));

        animator.SetTrigger(triggerName);
        yield return new WaitForSeconds(2f);

        // Back to start point
        yield return StartCoroutine(MoveToPoint(targetPosition, originalPosition, animator));

        // reset transform
        gameObject.transform.position = originalPosition;
        gameObject.transform.localEulerAngles = Vector3.zero;

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

        // start new turn
        gameplayController.StartTurn();
    }

    public IEnumerator BeingAttackedAndBackToIdle(string parameterName, float animationDuration, List<OnFieldCharacter> enemyTargets)
    {
        // enemy targets list will be clear when finish using skill
        // so make a clone list to not encounter bug
        List<OnFieldCharacter> targets = new List<OnFieldCharacter>(enemyTargets);

        if (checkSkillAnimationController != null)
        {
            // List to hold the animators of the targets
            List<Animator> animators = new List<Animator>();

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
                animator.SetBool(parameterName, true); // Play being attacked animation
            }

            // Wait for the duration of the animation
            yield return new WaitForSeconds(animationDuration);

            // Stop animations on all animators
            foreach (var animator in animators)
            {
                animator.SetBool(parameterName, false); // Play idle animation
            }

            // Optionally, wait a short period for idle animation to transition
            yield return new WaitForSeconds(0.5f);
        }
    }
}
