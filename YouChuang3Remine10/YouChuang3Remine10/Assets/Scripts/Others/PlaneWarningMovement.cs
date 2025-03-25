using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlaneWarningMovement : MonoBehaviour
{
    public float interval = 1f;    // �ƶ����ʱ�䣨a�룩
    public float distance = 2f;    // ÿ���ƶ����루b��λ��
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
        // ����������ϵ����X���ƶ�
        transform.Translate(this.moveDir*Vector3.right * distance, Space.World);

        // ��һ��ʵ�ַ�ʽ��ֱ���޸�position
        // transform.position += new Vector3(distance, 0, 0);
    }
}
