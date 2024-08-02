using System.Collections.Generic;
using UnityEngine;

public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        name = "Swing The Sword";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 50f;
        numberOfTargets = 1;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] {TargetType.Enemy };
    }

    public override void SkillFunction(GameObject character, List<GameObject> targets)
    {
        foreach (GameObject target in targets)
        {
            target.GetComponent<OnFieldCharacter>().CurrentHealth -= 
                character.GetComponent<OnFieldCharacter>().CurrentAttack 
                - target.GetComponent<OnFieldCharacter>().CurrentArmor; 
        }
    }
}
