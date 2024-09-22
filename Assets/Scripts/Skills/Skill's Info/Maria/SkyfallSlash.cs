using System.Collections.Generic;
using UnityEngine;

public class SkyfallSlash: Skill
{
    public SkyfallSlash()
    {
        skillName = "Skyfall Slash";
        avatar = "Art/UI/Skill Avatars/Maria/Skyfall Slash Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 50f;
        numberOfEnemyTargets = 3;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy};
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
