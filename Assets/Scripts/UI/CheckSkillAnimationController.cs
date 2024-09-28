using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckSkillAnimationController : MonoBehaviour
{
    public void GetSkillAnimationControllerForPlayAnimation(OnFieldCharacter champion, List<OnFieldCharacter> enemies, int whichSkill)
    {       
        Component skillAnimationController = GetCharacterSkillController(champion);

        if (skillAnimationController != null)
        {
            CheckWhoseAnimationControllerToPlayAnimation(enemies, skillAnimationController,whichSkill);
        }
    }

    public Component GetCharacterSkillController(OnFieldCharacter champion)
    {
        // get all components of character
        Component[] components =
                    champion.gameObject.GetComponents<Component>();

        // get 4th component (animation controller script)
        if (components.Length >= 5)
        {
            return components[4];
        }
        return null;
    }

    /// <summary>
    /// Check whose animation controller 
    /// </summary>
    private void CheckWhoseAnimationControllerToPlayAnimation(List<OnFieldCharacter> enemies, Component skillAnimationController, int whichSkill)
    {
        // Maria
        if (skillAnimationController is MariaSkillController mariaSkillAnimationController)
        {
            mariaSkillAnimationController.EnemyTargets = enemies;
            if(whichSkill == 0)
                mariaSkillAnimationController.PlayFirstSkillAnimation();
            else if(whichSkill == 1)
                mariaSkillAnimationController.PlaySecondSkillAnimation();
            else
                mariaSkillAnimationController.PlayBurstSkillAnimation();
        }

        // Uriel A Plotexia
        if (skillAnimationController is UrielAPlotexiaSkillController urielAPlotexiaSkillController)
        {
            urielAPlotexiaSkillController.EnemyTargets = enemies;
            if (whichSkill == 0)
                urielAPlotexiaSkillController.PlayFirstSkillAnimation();
            else if (whichSkill == 1)
                urielAPlotexiaSkillController.PlaySecondSkillAnimation();
            else
                urielAPlotexiaSkillController.PlayBurstSkillAnimation();
        }
    }

    public Animator CheckWhoseAnimationControllerToGetAnimator(Component skillAnimationController)
    {
        // Maria
        if (skillAnimationController is MariaSkillController mariaSkillAnimationController)
        {
            return mariaSkillAnimationController.Animator;
        }

        // Uriel A Plotexia
        if (skillAnimationController is UrielAPlotexiaSkillController urielAPlotexiaSkillController)
        {
            return urielAPlotexiaSkillController.Animator;
        }
        
        return null;
    }

    public void CheckWhoseAnimationControllerToResetIsAnimating(Component skillAnimationController)
    {
        // Maria
        if(skillAnimationController is MariaSkillController mariaSkillAnimationController)
            mariaSkillAnimationController.IsAnimating = false;

        // Uriel A Plotexia
        if (skillAnimationController is UrielAPlotexiaSkillController urielAPlotexiaSkillController)
            urielAPlotexiaSkillController.IsAnimating = false;
    }
}
