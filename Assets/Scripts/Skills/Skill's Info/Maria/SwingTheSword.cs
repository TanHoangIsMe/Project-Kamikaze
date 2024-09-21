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

            List<float> damages = new List<float>();

            // calculate the real damage deal to enemy 
            float trueAttackDamage = character.CurrentAttack
                - enemyTargets[0].CurrentArmor;
            if (trueAttackDamage < 0) // make sure damage deal not negative
                trueAttackDamage = 0;

            // reduce champion shield before health
            if (enemyTargets[0].CurrentShield > 0) // champ have shield
            {
                float damageAfterShield = trueAttackDamage - enemyTargets[0].CurrentShield;
                float shieldLost;

                if (damageAfterShield <= 0) 
                {
                    enemyTargets[0].CurrentShield -= trueAttackDamage;
                    shieldLost = trueAttackDamage;
                }
                else
                {
                    shieldLost = enemyTargets[0].CurrentShield;
                    enemyTargets[0].CurrentShield = 0;
                    enemyTargets[0].CurrentHealth -= damageAfterShield;
                }

                // check if champion have temporary shield effect 
                // then update shield value remain
                UpdateRemainShield(enemyTargets[0], shieldLost);
            }
            else // champ not have shield
            {
                enemyTargets[0].CurrentHealth -= trueAttackDamage;
            }
           
            damages.Add(trueAttackDamage);

            // send damage list to skill handler
            // for make pop up damage text 
            skillHandler.SkillValues = damages;

            calculateSkillEnergy.IncreaseBurstBaseOnDamage(character, enemyTargets, trueAttackDamage);
        }
    }

    private void UpdateRemainShield(OnFieldCharacter enemy, float shieldLost)
    {
        for (int i = 0; i < enemy.effects.Count; i++)
            if (enemy.effects[i] is TemporaryShield)
            {
                TemporaryShield temporaryShield = enemy.effects[i] as TemporaryShield;

                temporaryShield.RemainShield -= shieldLost;

                // if shield is break then remove shield effect from champion
                if (temporaryShield.RemainShield <= 0)
                {
                    // update shield lost
                    shieldLost = -temporaryShield.RemainShield;

                    temporaryShield.RemoveEffect();
                    enemy.UpdateEffectIcon();

                    // run this method again to find next shield
                    UpdateRemainShield(enemy, shieldLost);
                }
            }
    }
}
