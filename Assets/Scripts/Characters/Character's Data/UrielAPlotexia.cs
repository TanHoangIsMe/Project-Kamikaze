public class UrielAPlotexia : Character
{
    public UrielAPlotexia()
    {
        fullName = "Uriel A Plotexia";
        avatar = "Art/UI/Character Avatars/Uriel A Plotexia Avatar"; ;
        roleTypes = new RoleType[] { RoleType.Tanker };
        classTypes = new ClassType[] { ClassType.Defender };
        elementTypes = new ElementType[] { ElementType.Light };
        attack = 100f;
        armor = 200f;
        speed = 40f;
        health = 1000f;
        maxMana = 80f;
        maxBurst = 100f;
        skills = new Skill[] {
            new HitFirstTalkLater(),
            new LionSpirit(),
            new AllEyesOnMe()
        };
    }
}
