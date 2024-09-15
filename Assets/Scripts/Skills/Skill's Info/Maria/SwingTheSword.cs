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
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 0;
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
            if (enemyTargets[0].CurrentShield > 0)
            {
                float damageAfterShield = character.CurrentShield - trueAttackDamage;
                float shieldLost;

                if (damageAfterShield >= 0) 
                {
                    character.CurrentShield -= trueAttackDamage;
                    shieldLost = trueAttackDamage;
                }
                else
                {
                    shieldLost = character.CurrentShield;
                    character.CurrentShield = 0;
                    character.CurrentHealth += damageAfterShield;
                }

                TemporaryShield temporaryShield = character.GetComponent<TemporaryShield>();
                if (temporaryShield != null)
                    temporaryShield.RemainShield -= shieldLost; 
            }

            enemyTargets[0].CurrentHealth -= trueAttackDamage;
            
            damages.Add(trueAttackDamage);

            // send damage list to skill handler
            // for make pop up damage text 
            skillHandler.SkillValues = damages;
        }
    }
}
