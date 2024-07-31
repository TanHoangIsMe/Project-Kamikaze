public class SwingTheSword : Skill
{
    public SwingTheSword()
    {
        name = "Swing The Sword";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 50f;
        skillType = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
    }

    public override void SkillFunction(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        foreach (OnFieldCharacter target in targets)
        {
            target.CurrentHealth -= character.CurrentAttack - target.CurrentArmor; 
        }
    }
}
