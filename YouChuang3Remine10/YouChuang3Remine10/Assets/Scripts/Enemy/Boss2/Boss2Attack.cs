using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Attack : Attack
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Character>()?.TakeDamage(this);
        }

    }
}
