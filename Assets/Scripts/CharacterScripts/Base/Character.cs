public abstract class Character
{
    protected string fullName;

    // Character's Attribute
    protected RoleType[] roleTypes;
    protected ClassType[] classTypes;
    protected ElementType[] elementTypes;

    // Character's Basic Stats 
    protected float attack;
    protected float armor;
    protected float speed;
    protected float health;
    protected float maxMana;
    protected float maxBurst;

    // Get Character Info
    public string FullName { get { return fullName; } }
    public RoleType[] RoleTypes { get { return roleTypes; } }
    public ClassType[] ClassTypes { get { return classTypes; } }
    public ElementType[] ElementTypes { get { return elementTypes; } }
    public float Attack { get { return attack; } }
    public float Armor { get { return armor; } }
    public float Speed { get { return speed; } }
    public float Health { get { return health; } }
    public float MaxMana { get { return maxMana; } }
    public float MaxBurst { get { return maxBurst; } }

    // Skill List
    public abstract void UsingFirstSkill(OnFieldCharacter character, OnFieldCharacter[] targets);
    public abstract void UsingSecondSkill(OnFieldCharacter character, OnFieldCharacter[] targets);
    public abstract void UsingBurstSkill(OnFieldCharacter character, OnFieldCharacter[] targets);

}
