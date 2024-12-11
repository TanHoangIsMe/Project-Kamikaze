using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour 
{
    #region Fields
    protected string skillName;
    protected string avatar;
    protected string description;
    protected float manaCost;
    protected float burstCost;
    protected int numberOfEnemyTargets;
    protected int numberOfAllyTargets;
    protected bool isGroupEnemy;
    protected bool isGroupAlly;

    protected StatType priorityStat;

    protected SkillType[] skillTypes;
    protected ActivateType[] activateTypes;
    protected TargetType[] targetTypes;

    protected CalculateSkillEnergy calculateSkillEnergy;
    protected CalculateSkillDamage calculateSkillDamage;
    protected SetUpTurnList setUpTurnList;
    protected SkillHandler skillHandler;

    protected OnFieldCharacter character;
    protected List<OnFieldCharacter> enemyTargets;
    protected List<OnFieldCharacter> allyTargets;
    #endregion

    #region Properties
    public string SkillName { get { return skillName; } }
    public string Avatar { get { return avatar; } }
    public string Description {  get { return description; } }
    public float ManaCost { get {  return manaCost; } }
    public float BurstCost { get { return burstCost; } }
    public int NumberOfEnemyTargets { get { return numberOfEnemyTargets; } }
    public int NumberOfAllyTargets { get { return numberOfAllyTargets; } }
    public bool IsGroupEnemy { get { return isGroupEnemy; } }
    public bool IsGroupAlly { get { return isGroupAlly; } }
    public StatType PriorityStat { get { return priorityStat; } }
    public SkillType[] SkillTypes { get { return skillTypes; } }
    public ActivateType[] ActivateTypes { get { return activateTypes; } }
    public TargetType[] TargetTypes { get { return targetTypes; } }

    public OnFieldCharacter Character { set { character = value; } }
    public List<OnFieldCharacter> EnemyTargets { set { enemyTargets = value; } }
    public List<OnFieldCharacter> AllyTargets { set { allyTargets = value; } }

    protected Skill()
    {
        calculateSkillEnergy = new CalculateSkillEnergy();
        calculateSkillDamage = new CalculateSkillDamage();
        setUpTurnList = FindObjectOfType<SetUpTurnList>();
        skillHandler = FindObjectOfType<SkillHandler>();
    }
    #endregion

    #region Methods
    public abstract void SkillFunction();
    #endregion
}
