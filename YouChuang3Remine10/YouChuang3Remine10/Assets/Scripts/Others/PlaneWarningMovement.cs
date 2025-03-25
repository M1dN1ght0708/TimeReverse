using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlaneWarningMovement : MonoBehaviour
{
    public float interval = 1f;    // 移动间隔时间（a秒）
    public float distance = 2f;    // 每次移动距离（b单位）
    public float moveDir = 1;
    private Coroutine coroutine;
    void Start()
    {
        
    }
    void OnEnable()
    {
        this.transform.position=new Vector3(0,this.transform.position.y, this.transform.position.z);
        this.coroutine=StartCoroutine(MoveObjectRoutine());
    }
    void OnDisable()
    {
        StopCoroutine(coroutine);
    }
    IEnumerator MoveObjectRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            MoveObject();
        }
    }

    void MoveObject()
    {
        // 在世界坐标系中沿X轴移动
        transform.Translate(this.moveDir*Vector3.right * distance, Space.World);

        // 另一种实现方式：直接修改position
        // transform.position += new Vector3(distance, 0, 0);
    }
}
