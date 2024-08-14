using System.Collections.Generic;
using UnityEngine;

public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        name = "Swing The Sword";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 50f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.currentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] {TargetType.Enemy };
    }

    public override void SkillFunction(OnFieldCharacter character,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if (enemyTargets != null)
        {
            foreach (OnFieldCharacter target in enemyTargets)
            {
                float trueAttackDamage = character.CurrentAttack - target.CurrentArmor;

                if (trueAttackDamage > 0)
                    target.CurrentHealth -= trueAttackDamage;
            }
        }
    }
}
