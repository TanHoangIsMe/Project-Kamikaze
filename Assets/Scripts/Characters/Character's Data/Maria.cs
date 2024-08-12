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
}
