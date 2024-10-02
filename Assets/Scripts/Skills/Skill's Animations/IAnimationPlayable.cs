using System.Collections.Generic;
using UnityEngine;

public interface IAnimationPlayable 
{
    public void SetEnemyTargets(List<OnFieldCharacter> targets);

    public Animator GetAnimator();

    public void SetIsAnimating(bool isAnimating);

    public void PlayFirstSkillAnimation();

    public void PlaySecondSkillAnimation();

    public void PlayBurstSkillAnimation();
}
