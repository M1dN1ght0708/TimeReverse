using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightRandomRotate : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("最大旋转角度（相对于初始方向）")]
    public float maxAngle = 30f;

    [Tooltip("旋转速度（度/秒）")]
    public float rotationSpeed = 30f;

    [Tooltip("旋转轴（使用本地坐标系）")]
    public Vector3 rotationAxis = Vector3.up;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // 记录初始旋转状态
        initialRotation = transform.localRotation;
        SetNewTargetRotation();
    }

    void Update()
    {
        // 平滑转向目标方向
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // 当接近目标时设置新目标
        if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
        {
            SetNewTargetRotation();
        }
    }

    private void SetNewTargetRotation()
    {
        // 生成随机旋转角度
        float randomAngle = Random.Range(-maxAngle, maxAngle);

        // 创建目标旋转（基于初始方向）
        targetRotation = initialRotation * Quaternion.AngleAxis(randomAngle, rotationAxis);
    }

    // 可选：在编辑器中可视化旋转范围
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

// 辅助类用于获取正确的宽高比
public static class AspectRatioHelper
{
    public static float GetAspectRatio(Camera cam)
    {
        if (cam == null) return 16f / 9f;
        return cam.aspect;
    }
}
