using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerLockOn : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform lockTarget;
    public float lockOnRadius = 5f; // �Զ�������Χ

    [Header("Camera Settings")]
    public float rotationSpeed = 2f;
    public float lockOnSmoothTime = 0.3f;
    public float maxLockOnAngle = 45f; // �������ƫ�ƽǶ�

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
        // ����Ŀ�귽��
        Vector3 targetDir = (lockTarget.position - player.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);

        // ��ȡ�������
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // ����ƫ���������Ʒ�Χ��
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

        // �������Ŀ����ֶ�ƫ��
        Quaternion finalRot = targetRot * Quaternion.Euler(-currentVerticalOffset, currentHorizontalOffset, 0);

        // ƽ������
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRot,
            Time.deltaTime / lockOnSmoothTime
        );

        // ����POV���루�����ͻ��
        pov.m_HorizontalAxis.m_InputAxisValue = 0;
        pov.m_VerticalAxis.m_InputAxisValue = 0;
    }
}