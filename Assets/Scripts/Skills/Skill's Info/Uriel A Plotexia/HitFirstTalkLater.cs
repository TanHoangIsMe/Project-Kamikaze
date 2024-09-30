using System.Collections.Generic;
using UnityEngine;

public class HitFirstTalkLater : Skill
{
    public HitFirstTalkLater()
    {
        skillName = "Hit First, Talk Later";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/Hit First, Talk Later Avatar";
        description = "Not yet";
        manaCost = 0f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack, SkillType.Heal };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy, TargetType.Self };
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
                (character, enemyTargets, skillHandler, 0.8f);

            calculateSkillEnergy.IncreaseBurstBaseOnDamage(character,
                enemyTargets, trueAttackDamages);

            // heal character by 70% damage deal
            float healAmount = trueAttackDamages[0] * 0.7f;
            float loseHealth = character.CurrentCharacter.Health
                - character.CurrentHealth;
            float needHealAmount = healAmount - loseHealth;

            if (needHealAmount <= 0)
                character.CurrentHealth += healAmount;
            else
            {
                character.CurrentHealth += loseHealth;
                character.CurrentShield += needHealAmount;
            }
        }
    }
}
