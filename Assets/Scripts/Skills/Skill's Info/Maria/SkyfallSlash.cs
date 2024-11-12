using System.Collections.Generic;
using UnityEngine;

public class SkyfallSlash: Skill
{
    public SkyfallSlash()
    {
        skillName = "Skyfall Slash";
        avatar = "Art/UI/Skill Avatars/Maria/Skyfall Slash Avatar";
        description = "Not yet";
        manaCost = 0f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy};
    }

    public override void SkillFunction()
    {
        if (enemyTargets == null) Debug.Log("Something's wrong");

        if (enemyTargets != null)
        {
            calculateSkillEnergy.ReduceCharacterMana(character, manaCost);

            List<float> trueAttackDamages =
                calculateSkillDamage.CalculateOutputDamage
                (character, enemyTargets, skillHandler, 1f);

            calculateSkillEnergy.IncreaseBurstBaseOnDamage(character,
                enemyTargets, trueAttackDamages);
        }
    }
}
