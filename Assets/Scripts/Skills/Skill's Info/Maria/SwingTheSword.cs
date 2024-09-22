using System.Collections.Generic;
using UnityEngine;

public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        skillName = "Swing The Sword";
        avatar = "Art/UI/Skill Avatars/Maria/Swing The Sword Avatar";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 10f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy };
    }

    public override void SkillFunction(OnFieldCharacter character,
        SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        if (enemyTargets == null) Debug.Log("Something's wrong");

        if (enemyTargets != null)
        {
            calculateSkillEnergy.ReduceCharacterMana(character, manaCost);

            List<float> trueAttackDamages = 
                calculateSkillDamage.CalculateOutputDamage
                (character, enemyTargets, skillHandler);

            calculateSkillEnergy.IncreaseBurstBaseOnDamage(character, 
                enemyTargets, trueAttackDamages);
        }
    }
}
