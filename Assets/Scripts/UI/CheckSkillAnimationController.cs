using System.Collections.Generic;
using UnityEngine;

public class CheckSkillAnimationController : MonoBehaviour
{
    public void GetSkillAnimationControllerForPlayAnimation(OnFieldCharacter champion, List<OnFieldCharacter> enemies, int whichSkill)
    {
        // get all components of character
        UnityEngine.Component[] components =
                    champion.gameObject.GetComponents<UnityEngine.Component>();

        // get 4th component (animation controller script)
        if (components.Length >= 4)
        {
            UnityEngine.Component skillAnimationController = components[3];
            CheckWhoseAnimationController(enemies, skillAnimationController,whichSkill);
        }
    }

    private void CheckWhoseAnimationController(List<OnFieldCharacter> enemies, Component skillAnimationController, int whichSkill)
    {
        // check whose skill animation controller
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
    }
}
