using System.Collections.Generic;
using UnityEngine;

public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        skillName = "Swing The Sword";
        avatar = "Art/UI/Skill Avatars/Maria/Swing The Sword Avatar";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat and reduce enemy armor by 20%";
        manaCost = 10f;
        burstCost = 0f;
        numberOfEnemyTargets = 1;
        numberOfAllyTargets = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Enemy };
    }

    public override void SkillFunction()
    {
        if (enemyTargets == null) Debug.Log("Something's wrong");

        if (enemyTargets != null)
        {
            calculateSkillEnergy.ReduceCharacterMana(character, manaCost);

            List<float> trueAttackDamages = 
                calculateSkillDamage.CalculateOutputDamage
                (character, enemyTargets, skillHandler, 1.3f);

            calculateSkillEnergy.IncreaseBurstBaseOnDamage(character, 
                enemyTargets, trueAttackDamages);

            ArmorBreak armorBreak = new ArmorBreak();
            armorBreak.Champion = enemyTargets[0];
            armorBreak.EffectValue = enemyTargets[0].CurrentArmor * 0.2f;
            armorBreak.EffectFunction();

            if (gameplayController != null)
            {
                armorBreak.StartTurn = gameplayController.Phase;
                armorBreak.EndTurn = gameplayController.Phase + 2;
            }

            enemyTargets[0].Effects.Add(armorBreak);
            enemyTargets[0].UpdateEffectIcon();
        }
    }
}
