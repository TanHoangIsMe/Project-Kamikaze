using System.Collections.Generic;
using Unity.VisualScripting;

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
        //character.CurrentMana -= manaCost;
        //if (character.CurrentMana < 0)
        //{ 
        //    character.CurrentMana = 0;
        //}

        character.AddComponent<TemporaryShield>();
        TemporaryShield temporaryShield = character.GetComponent<TemporaryShield>();
        if (temporaryShield != null)
        {
            temporaryShield.EffectValue = 200f;
            temporaryShield.EffectFunction();

            GameplayController gameplayController = FindAnyObjectByType<GameplayController>();
            if (gameplayController != null)
            {
                temporaryShield.StartTurn = gameplayController.Phase;
                temporaryShield.EndTurn = temporaryShield.StartTurn + 1;
            }

            character.Effects.Add("Temporary Shield",temporaryShield.EffectAvatar);
        }
    }
}
