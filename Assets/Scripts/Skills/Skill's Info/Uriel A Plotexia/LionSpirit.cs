using System.Collections.Generic;

public class LionSpirit : Skill
{
    public LionSpirit()
    {
        skillName = "Lion's Spirit";
        avatar = "Art/UI/Skill Avatars/Uriel A Plotexia/Lion's Spirit Avatar";
        description = "Uriel A Plotexia increase his or an ally's armor by 30";
        manaCost = 50f;
        burstCost = 0f;
        numberOfEnemyTargets = 0;
        numberOfAllyTargets = 0;
        numberOfSelfTarget = 0;
        numberOfSelfOrAllyTarget = 1;
        priorityStat = StatType.CurrentArmor;
        skillTypes = new SkillType[] { SkillType.Defend };
        activateTypes = new ActivateType[] { ActivateType.Active };
        targetTypes = new TargetType[] { TargetType.SelfOrAlly};
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
    }
}