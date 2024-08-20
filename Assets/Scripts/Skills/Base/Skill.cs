using System.Collections.Generic;
using UnityEngine;

public abstract class Skill 
{
    #region Fields
    protected string name;
    protected string description;
    protected float manaCost;
    protected float burstCost;
    protected int numberOfEnemyTargets;
    protected int numberOfAllyTargets;
    protected int numberOfSelfTarget;
    protected int numberOfSelfOrAllyTarget;

    protected StatType priorityStat;

    protected SkillType[] skillTypes;
    protected ActivateType[] activateTypes;
    protected TargetType[] targetTypes;
    #endregion

    #region Properties
    public string Name { get { return name; } }
    public string Description {  get { return description; } }
    public float ManaCost { get {  return manaCost; } }
    public float BurstCost { get { return burstCost; } }
    public int NumberOfEnemyTargets { get { return numberOfEnemyTargets; } }
    public int NumberOfAllyTargets { get { return numberOfAllyTargets; } }
    public int NumberOfSelfTarget { get { return numberOfSelfTarget; } }
    public int NumberOfSelforAllyTarget { get { return numberOfSelfTarget; } }
    public StatType PriorityStat { get { return priorityStat; } }
    public SkillType[] SkillTypes { get { return skillTypes; } }
    public ActivateType[] ActivateTypes { get { return activateTypes; } }
    public TargetType[] TargetTypes { get { return targetTypes; } }
    #endregion

    #region Methods
    public abstract void SkillFunction(OnFieldCharacter character, 
        List<OnFieldCharacter> enemyTargets = null, 
        List<OnFieldCharacter> allyTargets = null);
    #endregion
}
