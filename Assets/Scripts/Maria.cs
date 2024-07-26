public class Maria : Character 
{
    public Maria()
    {
        name = "Maria"; 
        roleTypes = new RoleType[] { RoleType.Attacker };
        classTypes = new ClassType[] { ClassType.Warrior };
        elementTypes = new ElementType[] { ElementType.Fire };
        attack = 80f;
        armor = 50f;
        speed = 40f;
        health = 700f;
        mana = 80f;
        burst = 10f; 
    }

    public override void UsingFirstSkill()
    {

    }
    public override void UsingSecondSkill()
    {

    }
    public override void UsingBurstSkill()
    {

    }
}
