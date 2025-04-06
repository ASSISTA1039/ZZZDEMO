using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerLockOn : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform lockTarget;
    public float lockOnRadius = 5f; // 自动锁定范围

    [Header("Camera Settings")]
    public float rotationSpeed = 2f;
    public float lockOnSmoothTime = 0.3f;
    public float maxLockOnAngle = 45f; // 最大锁定偏移角度

    private CinemachineVirtualCamera vcam;
    private CinemachinePOV pov;
    private bool isLockedOn;
    private float currentHorizontalOffset;
    private float currentVerticalOffset;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }

    void Update()
    {
        ApplyCameraRotation();
    }


    Transform FindNearestTarget()
    {
        Collider[] targets = Physics.OverlapSphere(player.position, lockOnRadius, LayerMask.GetMask("Enemy"));
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var target in targets)
        {
            float dist = Vector3.Distance(player.position, target.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = target.transform;
            }
        }
        return closest;
    }

    void ApplyCameraRotation()
    {
        // 计算目标方向
        Vector3 targetDir = (lockTarget.position - player.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // 计算偏移量（限制范围）
        currentHorizontalOffset = Mathf.Clamp(
            currentHorizontalOffset + mouseX,
            -maxLockOnAngle,
            maxLockOnAngle
        );

        currentVerticalOffset = Mathf.Clamp(
            currentVerticalOffset + mouseY,
            -maxLockOnAngle * 0.5f,
            maxLockOnAngle * 0.5f
        );

        // 混合锁定目标和手动偏移
        Quaternion finalRot = targetRot * Quaternion.Euler(-currentVerticalOffset, currentHorizontalOffset, 0);

        // 平滑过渡
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRot,
            Time.deltaTime / lockOnSmoothTime
        );

        // 重置POV输入（避免冲突）
        pov.m_HorizontalAxis.m_InputAxisValue = 0;
        pov.m_VerticalAxis.m_InputAxisValue = 0;
    }
}