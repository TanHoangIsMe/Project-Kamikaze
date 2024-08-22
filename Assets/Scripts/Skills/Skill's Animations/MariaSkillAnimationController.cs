using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariaSkillAnimationController : MonoBehaviour
{
    private Animator animator;

    private List<OnFieldCharacter> enemyTargets;
    public List<OnFieldCharacter> EnemyTargets { set { enemyTargets = value; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayFirstSkillAnimation()
    {

    }

    public void PlaySecondSkillAnimation()
    {
        if (enemyTargets != null)
        {
            Debug.Log(enemyTargets[0].gameObject.transform.position);
            StartCoroutine(MoveToPointAndBack(gameObject.transform.position, 
                enemyTargets[0].gameObject.transform.position,3.5f));
        }
    }

    public void PlayBurstSkillAnimation()
    {

    }

    //character move to target for attack and back to original position
    private IEnumerator MoveToPointAndBack(Vector3 startPosition, Vector3 endPosition, float distanceFromEnd)
    {
        // store champion start position
        Vector3 originalPosition = startPosition;

        // calculate the position in front of target position
        Vector3 directionToEnd = (endPosition - startPosition).normalized;
        Vector3 targetPosition = endPosition - directionToEnd * distanceFromEnd;

        // Run to end point
        yield return StartCoroutine(MoveToPoint(originalPosition, targetPosition));

        animator.SetTrigger("Using Second Skill");
        yield return new WaitForSeconds(2f);

        // Back to start point
        yield return StartCoroutine(MoveToPoint(targetPosition, originalPosition));

        // reset transform
        gameObject.transform.position = originalPosition;
        gameObject.transform.localEulerAngles = Vector3.zero;
    }


    // character move to target for attack
    private IEnumerator MoveToPoint(Vector3 startPosition, Vector3 endPosition)
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

        while (Vector3.Distance(gameObject.transform.position, endPosition) > 0.5f)
        {
            float distanceCovered = (Time.time - startTime) * 5f;
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
}
