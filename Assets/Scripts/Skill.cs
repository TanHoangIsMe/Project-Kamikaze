public abstract class Skill 
{
    protected string name;
    protected string description;
    protected float manaCost;

    protected SkillType[] skillType;
    protected ActivateType[] activateTypes;

    public abstract void SkillFunction(OnFieldCharacter character, OnFieldCharacter[] targets);
}
