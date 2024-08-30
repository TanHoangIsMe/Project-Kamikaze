using System.Collections.Generic;
using UnityEngine;

public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        skillName = "Swing The Sword";
        avatar = "Art/UI/Skill Avatars/Maria/Swing The Sword Avatar";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 50f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy };
    }

    public override void SkillFunction(OnFieldCharacter character,
        CombatSkillMenu combatSkillMenu,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if (enemyTargets != null)
        {
            List<float> damages = new List<float>();

            float trueAttackDamage = character.CurrentAttack 
                - enemyTargets[0].CurrentArmor;

            if (trueAttackDamage > 0)
                enemyTargets[0].CurrentHealth -= trueAttackDamage;

            damages.Add(trueAttackDamage);

            combatSkillMenu.SkillValues = damages;
        }
    }
}
