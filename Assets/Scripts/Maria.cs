public class Maria : Character 
{
    // Character's Skills
    SwingTheSword swingTheSword = new SwingTheSword();

    public Maria()
    {
        fullName = "Maria"; 
        roleTypes = new RoleType[] { RoleType.Attacker };
        classTypes = new ClassType[] { ClassType.Warrior };
        elementTypes = new ElementType[] { ElementType.Fire };
        attack = 80f;
        armor = 50f;
        speed = 40f;
        health = 700f;
        maxMana = 80f;
        maxBurst = 100f; 
    }
    
    public override void UsingFirstSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        swingTheSword.SkillFunction(character, targets);
    }
    public override void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        swingTheSword.SkillFunction(character,targets); 
    }
    public override void UsingBurstSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        swingTheSword.SkillFunction(character, targets);
    }
}
