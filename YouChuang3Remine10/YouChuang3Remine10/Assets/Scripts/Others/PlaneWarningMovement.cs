using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWarningMovement : MonoBehaviour
{
    public float interval = 1f;    // �ƶ����ʱ�䣨a�룩
    public float distance = 2f;    // ÿ���ƶ����루b��λ��

    void Start()
    {
        StartCoroutine(MoveObjectRoutine());
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
        transform.Translate(Vector3.right * distance, Space.World);

        // ��һ��ʵ�ַ�ʽ��ֱ���޸�position
        // transform.position += new Vector3(distance, 0, 0);
    }
}
