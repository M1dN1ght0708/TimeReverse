using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    private bool hasDead;
    protected override void OnEnable()
    {
        base.OnEnable();
        EventCenter.Instance.TriggerEvent("HpChange", this);
    }
    public override void TakeDamage(Attack attacker, bool attackType=false)
    {
        if (isInvincible)
            return;
        if (currentHp - attacker.damage > 0)
        {
            currentHp -= attacker.damage;
            this.TriggerInvincible();
            hurtEvent?.Invoke(attacker.gameObject.transform,attackType);
        }
        else
        {
            currentHp = 0;
            if (!hasDead)
            {
                hasDead = true;
                deadEvent?.Invoke();
            }
            
        }

        EventCenter.Instance.TriggerEvent("HpChange", this);
        EventCenter.Instance.TriggerEvent("CameraShake", null);
    }
}
