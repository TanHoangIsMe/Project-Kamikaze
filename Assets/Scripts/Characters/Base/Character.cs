using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    #region Fields
    protected string fullName;
    protected string avatar;

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

    // Character's Skills 
    protected Skill[] skills;
    #endregion

    #region Properties
    // Get Character Info
    public string FullName { get { return fullName; } }
    public string Avatar { get { return avatar; } }
    public RoleType[] RoleTypes { get { return roleTypes; } }
    public ClassType[] ClassTypes { get { return classTypes; } }
    public ElementType[] ElementTypes { get { return elementTypes; } }
    public float Attack { get { return attack; } }
    public float Armor { get { return armor; } }
    public float Speed { get { return speed; } }
    public float Health { get { return health; } }
    public float MaxMana { get { return maxMana; } }
    public float MaxBurst { get { return maxBurst; } }  
    public Skill[] Skills { get { return skills; } }
    #endregion
}
