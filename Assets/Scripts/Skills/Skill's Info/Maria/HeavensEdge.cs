using System.Collections.Generic;
using UnityEngine;

public class HeavensEdge: Skill
{
    public HeavensEdge()
    {
        skillName = "Heaven's Edge";
        avatar = "Art/UI/Skill Avatars/Maria/Heaven's Edge Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 0f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack};
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
            // decrease champion burst
            calculateSkillEnergy.ReduceCharacterBurst(character);

            List<float> trueAttackDamages =
                calculateSkillDamage.CalculateOutputDamage
                (character, enemyTargets, skillHandler, 2f);

            if (enemyTargets[0].CurrentHealth <= 0) // reset skill if enemy die
            {
                character.CurrentBurst = burstCost;
                skillHandler.CanReuse = true;
            }
            else
                calculateSkillEnergy.IncreaseBurstBaseOnDamage(character,
                    enemyTargets, trueAttackDamages);
        }
    }
}
