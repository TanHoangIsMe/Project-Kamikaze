public class UrielAPlotexia : Character
{
    public UrielAPlotexia()
    {
        fullName = "Uriel A Plotexia";
        roleTypes = new RoleType[] { RoleType.Tanker };
        classTypes = new ClassType[] { ClassType.Defender };
        elementTypes = new ElementType[] { ElementType.Light };
        attack = 20f;
        armor = 100f;
        speed = 20f;
        health = 1000f;
        maxMana = 80f;
        maxBurst = 100f;
    }

    public override void UsingFirstSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        
    }
    public override void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        
    }
    public override void UsingBurstSkill(OnFieldCharacter character, OnFieldCharacter[] targets)
    {
        
    }
}
