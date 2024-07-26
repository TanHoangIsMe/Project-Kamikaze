public class SwingTheSword : Skill
{
    SwingTheSword()
    {
        name = "Swing The Sword";
        description = "Maria spins around and swings her great sword at the enemy, dealing damage equal to 130% of her attack stat.";
        manaCost = 50f;
        skillType = new SkillType[] { SkillType.Attack };
        activateTypes = new ActivateType[] { ActivateType.Active };
    }

    public override void SkillFunction() 
    { }
}
