using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Character : Character
{
    public override void TakeDamage(Attack attacker, bool attackType = false)
    {
        base.TakeDamage(attacker, attackType);
        EventCenter.Instance.TriggerEvent("Boss2HpChange", this);
    }
}
