using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Attack : Attack
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")|| collision.CompareTag("CanAttackPlatform"))
        {           
            collision.GetComponentInParent<Character>()?.TakeDamage(this);
        }
        /*if (collision.CompareTag("CanAttackPlatform"))
        {

            int temp = collision.GetComponentInParent<Boss2PlatformCharacter>().attackCount;
            if (temp > 0)
                return;
            else
                collision.GetComponentInParent<Boss2PlatformCharacter>().attackCount++;
            print("击中箱子"+this.gameObject.name+" "+collision.gameObject.name);
            print(collision.GetComponentInParent<Boss2PlatformCharacter>().attackCount);
            collision.GetComponentInParent<Character>()?.TakeDamage(this);
        }*/
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CanAttackPlatform"))
        {
         collision.GetComponentInParent<Boss2PlatformCharacter>().attackCount = 0;
            print("离开箱子");
        }
           
    }*/
}
