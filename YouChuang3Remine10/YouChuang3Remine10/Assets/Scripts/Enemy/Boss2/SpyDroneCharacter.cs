using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyDroneCharacter : Character
{
    public override void TakeDamage(Attack attacker, bool attackType)
    {
        base.TakeDamage(attacker, attackType);
        this.GetComponentInChildren<EnmeyHp>().EnmeyHpChange(this);
    }
}
