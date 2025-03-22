using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack
{
    public float timeGet;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy")||collision.CompareTag("Boss"))
        {
            Debug.Log("gongji");
            collision.GetComponentInParent<Character>()?.TakeDamage(this);
            if(this.GetComponentInParent<PlayerController>().stopTimeCounter< this.GetComponentInParent<PlayerController>().stopTimeNeed)
                this.GetComponentInParent<PlayerController>().stopTimeCounter += timeGet;
        }
            
    }
}
