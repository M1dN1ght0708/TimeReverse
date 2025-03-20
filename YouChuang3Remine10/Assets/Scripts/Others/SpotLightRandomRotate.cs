using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightRandomRotate : MonoBehaviour
{
    [Header("��ת����")]
    [Tooltip("�����ת�Ƕȣ�����ڳ�ʼ����")]
    public float maxAngle = 30f;

    [Tooltip("��ת�ٶȣ���/�룩")]
    public float rotationSpeed = 30f;

    [Tooltip("��ת�ᣨʹ�ñ�������ϵ��")]
    public Vector3 rotationAxis = Vector3.up;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // ��¼��ʼ��ת״̬
        initialRotation = transform.localRotation;
        SetNewTargetRotation();
    }

    void Update()
    {
        // ƽ��ת��Ŀ�귽��
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // ���ӽ�Ŀ��ʱ������Ŀ��
        if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
        {
            SetNewTargetRotation();
        }
    }

    private void SetNewTargetRotation()
    {
        // ���������ת�Ƕ�
        float randomAngle = Random.Range(-maxAngle, maxAngle);

        // ����Ŀ����ת�����ڳ�ʼ����
        targetRotation = initialRotation * Quaternion.AngleAxis(randomAngle, rotationAxis);
    }

    // ��ѡ���ڱ༭���п��ӻ���ת��Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(
            Vector3.zero,
            maxAngle,
            1f,
            0f,
            AspectRatioHelper.GetAspectRatio(Camera.current)
        );
    }
}

// ���������ڻ�ȡ��ȷ�Ŀ�߱�
public static class AspectRatioHelper
{
    public static float GetAspectRatio(Camera cam)
    {
        if (cam == null) return 16f / 9f;
        return cam.aspect;
    }
}
