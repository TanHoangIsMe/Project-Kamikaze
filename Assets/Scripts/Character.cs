public abstract class Character
{
    protected string name;

    // Character's Attribute
    protected RoleType[] roleTypes;
    protected ClassType[] classTypes;
    protected ElementType[] elementTypes;

    // Character's Basic Stats 
    protected float attack;
    protected float armor;
    protected float speed;
    protected float health;
    protected float mana;
    protected float burst;

    // Get Character Info
    public string Name { get { return name; } }
    public RoleType[] RoleTypes { get { return roleTypes; } }
    public ClassType[] ClassTypes { get { return classTypes; } }
    public ElementType[] ElementTypes { get { return elementTypes; } }
    public float Attack { get { return attack; } }
    public float Armor { get { return armor; } }
    public float Speed { get { return speed; } }
    public float Health { get { return health; } }
    public float Mana { get { return mana; } }
    public float Burst { get { return burst; } }

    // Skill List
    public abstract void UsingFirstSkill();
    public abstract void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets);
    public abstract void UsingBurstSkill();

}
