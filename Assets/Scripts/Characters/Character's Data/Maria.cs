using System.Collections.Generic;
using UnityEngine;

public class Maria : Character 
{
    public Maria()
    {
        fullName = "Maria"; 
        roleTypes = new RoleType[] { RoleType.Attacker };
        classTypes = new ClassType[] { ClassType.Warrior };
        elementTypes = new ElementType[] { ElementType.Fire };
        attack = 80f;
        armor = 50f;
        speed = 40f;
        health = 700f;
        maxMana = 80f;
        maxBurst = 100f;
        skills = new Skill[] { 
            new SwingTheSword(), 
            new SwingTheSword(), 
            new SwingTheSword() 
        };
    }
    
    public override void UsingFirstSkill(GameObject character, List<GameObject> targets)
    {
        skills[0].SkillFunction(character, targets);
    }
    public override void UsingSecondSkill(GameObject character, List<GameObject> targets)
    {
        skills[1].SkillFunction(character,targets); 
    }
    public override void UsingBurstSkill(GameObject character, List<GameObject> targets)
    {
        skills[2].SkillFunction(character, targets);
    }
}
