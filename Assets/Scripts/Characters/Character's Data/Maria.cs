public class Maria : Character 
{
    public Maria()
    {
        fullName = "Maria";
        avatar = "Art/UI/Character Avatars/Maria Avatar"; ;
        roleTypes = new RoleType[] { RoleType.Attacker };
        classTypes = new ClassType[] { ClassType.Warrior };
        elementTypes = new ElementType[] { ElementType.Fire };
        attack = 1080f;
        armor = 50f;
        speed = 40f;
        health = 700f;
        maxMana = 80f;
        maxBurst = 100f;
        skills = new Skill[] {
            new SkyfallSlash(),
            new SwingTheSword(), 
            new HeavensEdge() 
        };
    }
}
