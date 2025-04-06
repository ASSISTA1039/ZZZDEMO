using Cinemachine;
using System.Collections;
using UnityEngine;

public class DollyController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // ������������
    //[SerializeField] private float moveDuration = 2f; // ��·���ƶ�ʱ��
    [SerializeField] private Vector3 fixedRotation; // �̶���ʼ��ת�ĽǶ�

    private CinemachineTrackedDolly trackedDolly; // ���ٵ� Dolly
    private float elapsedTime;

    private void Start()
    {
        // ��ȡ Tracked Dolly ���
        trackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void StartPathMovement(float _moveduration)
    {
        elapsedTime = 0f;
        // �̶���ʼ��ת
        if (virtualCamera != null)
        {
            //virtualCamera.transform.rotation = Quaternion.Euler(fixedRotation);
        }

        StartCoroutine(MoveAlongPath(_moveduration));
    }

    private IEnumerator MoveAlongPath(float moveDuration)
    {
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            // ��̬���� Path Position
            if (trackedDolly != null)
            {
                trackedDolly.m_PathPosition = Mathf.Lerp(0f, 1f, elapsedTime / moveDuration);
            }

            yield return null;
        }
    }
}
