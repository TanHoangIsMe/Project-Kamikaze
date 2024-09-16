using System.Collections.Generic;

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
        if (enemyTargets != null)
        {
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

                TemporaryShield temporaryShield = enemyTargets[0].GetComponent<TemporaryShield>();
                if (temporaryShield != null)
                    temporaryShield.RemainShield -= shieldLost; 
            }
            else // champ not have shield
            {
                enemyTargets[0].CurrentHealth -= trueAttackDamage;
            }
           
            damages.Add(trueAttackDamage);

            // send damage list to skill handler
            // for make pop up damage text 
            skillHandler.SkillValues = damages;
        }
    }
}
