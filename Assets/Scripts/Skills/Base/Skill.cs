using System.Collections.Generic;
using UnityEngine;

public abstract class Skill 
{
    #region Fields
    protected string name;
    protected string description;
    protected float manaCost;
    protected int numberOfTargets;

    protected SkillType[] skillTypes;
    protected ActivateType[] activateTypes;
    protected TargetType[] targetTypes;
    #endregion

    #region Properties
    public string Name { get { return name; } }
    public string Description {  get { return description; } }
    public float ManaCost { get {  return manaCost; } }
    public int NumberOfTargets { get { return numberOfTargets; } }
    public SkillType[] SkillTypes { get { return skillTypes; } }
    public ActivateType[] ActivateTypes { get { return activateTypes; } }
    public TargetType[] TargetTypes { get { return targetTypes; } }
    #endregion

    #region Methods
    public abstract void SkillFunction(GameObject character, List<GameObject> targets);
    #endregion
}