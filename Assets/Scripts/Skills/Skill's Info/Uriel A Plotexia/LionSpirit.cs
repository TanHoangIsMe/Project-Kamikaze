using System.Collections.Generic;

public class LionSpirit : Skill
{
    public LionSpirit()
    {
        skillName = "Lion's Spirit";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/Lion's Spirit Avatar";
        description = "Uriel A Plotexia gain a shield for himself equal to 30% of his lost health.";
        manaCost = 50f;
        burstCost = 0f;
        numberOfEnemyTargets = 0;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 1;
        numberOfSelfOrAllyTarget = 0;
        priorityStat = StatType.CurrentHealth;
        skillTypes = new SkillType[] { SkillType.Shield };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.Self};
    }

    public override void SkillFunction(OnFieldCharacter character,
        SkillHandler skillHandler,
        List<OnFieldCharacter> enemyTargets = null,
        List<OnFieldCharacter> allyTargets = null)
    {
        // decrease champion mana
        character.CurrentMana -= manaCost;
        if (character.CurrentMana < 0)
        { 
            character.CurrentMana = 0;
        }

        Effect temporaryShield = new TemporaryShield();
        temporaryShield.Duration = 2;
        temporaryShield.EffectFunction(character);
        character.Effects.Add(temporaryShield);
    }
}
