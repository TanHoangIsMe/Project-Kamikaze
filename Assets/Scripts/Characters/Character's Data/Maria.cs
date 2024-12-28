public class Maria : Character 
{
    public Maria()
    {
        fullName = "Maria";
        avatar = "Art/UI/Character Avatars/Maria Avatar"; ;
        roleTypes = new RoleType[] { RoleType.Attacker };
        classTypes = new ClassType[] { ClassType.Warrior };
        elementTypes = new ElementType[] { ElementType.Fire };
        attack = 250f;
        armor = 80f;
        speed = 70f;
        health = 700f;
        maxMana = 80f;
        maxBurst = 80f;
        skills = new Skill[] {
            new SkyfallSlash(),
            new SwingTheSword(), 
            new HeavensEdge() 
        };
    }
}
