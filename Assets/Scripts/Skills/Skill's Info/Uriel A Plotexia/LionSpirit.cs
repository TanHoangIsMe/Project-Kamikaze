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
        isGroupEnemy = false;
        isGroupAlly = false;
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
        calculateSkillEnergy.ReduceCharacterMana(character, manaCost);

        // set up temporary shield effect and add to champion effect list
        TemporaryShield temporaryShield = new TemporaryShield();
        temporaryShield.Champion = character;
        temporaryShield.EffectValue = 200f;
        temporaryShield.EffectFunction();

        if (gameplayController != null)
        {
            temporaryShield.StartTurn = gameplayController.Phase;
            temporaryShield.EndTurn = gameplayController.Phase + 2;
        }

        character.Effects.Add(temporaryShield);
        character.UpdateEffectIcon();
    }
}
