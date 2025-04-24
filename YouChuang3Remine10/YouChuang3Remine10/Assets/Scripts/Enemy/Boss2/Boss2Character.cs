using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Character : Character
{
    public static bool isStageTwo;
    public static bool hasStageTwo;
    protected override void Update()
    {
        base.Update();
        //if(!isStageTwo&&this.currentHp<=this.maxHp/2)
        //    isStageTwo = true;
    }
    public override void TakeDamage(Attack attacker, bool attackType = false)
    {
        base.TakeDamage(attacker, attackType);
        EventCenter.Instance.TriggerEvent("Boss2HpChange", this);
    }
}
