using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(1, 8), SerializeField, Header("Ĭ�ϵľ���")] private float defaultDistance;
    [Range(0, 8), SerializeField, Header("��С�ľ���")] private float lookMinDistance;
    [Range(1, 8), SerializeField, Header("���ľ���")] private float lookMaxDistance;
    [SerializeField] private float zoomSensitivity = 1;
    [SerializeField] private float zoomSpeed = 4;
    public float ExternalSpeedVariable = 1;


    private CinemachineFramingTransposer CinemachineFramingTransposer;
    private CinemachineInputProvider CinemachineInputProvider;
    [SerializeField] public float currentDistance;
    private void Awake()
    {
        CinemachineFramingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        CinemachineInputProvider = GetComponent<CinemachineInputProvider>();
        currentDistance = defaultDistance;
    }
    private void Update()
    {
        UpdateInput();
    }
    private void UpdateInput()
    {
        float inputZoomValue = CinemachineInputProvider.GetAxisValue(2) * zoomSensitivity;
        UpdateZoom(inputZoomValue);
    }

    private void UpdateZoom(float inputZoomValue)
    {

        currentDistance = Mathf.Clamp(currentDistance + inputZoomValue, lookMinDistance, lookMaxDistance);

        float realDistance = CinemachineFramingTransposer.m_CameraDistance;

        realDistance = Mathf.Lerp(realDistance, currentDistance, zoomSpeed * Time.deltaTime);

        CinemachineFramingTransposer.m_CameraDistance = realDistance;

        if (realDistance == currentDistance)
        { return; }
    }
    public void SetZoom(float distance, float speed)
    {
        currentDistance = distance;
        ExternalSpeedVariable = speed;
    }
    ////[SerializeField] private CharacterInputSystem _inputSystem;
    //[SerializeField] public Transform LookAttarGet;
    //private Transform playerCamera;

    //[Range(0.1f, 1f), SerializeField, Header("���������")] public float RotateSpeed;
    //[Range(0.1f, 0.5f), SerializeField, Header("�����תƽ����")] public float rotationSmoothTime = 0.2f;
    //[Range(0.1f, 1f), SerializeField, Header("����������")] private float scrollSpeed;
    ////����ͷ���ɫ�ľ���
    //[SerializeField, Header("�������")] private float _cameraDistance;
    //[SerializeField] private Vector2 _cameraDistanceMinMax = new Vector2(0.01f, 3f);
    //[SerializeField] private float distancePlayerOffset;
    //[SerializeField, Header("���������")] private Vector2 ClmpCameraRang = new Vector2(-65f, 65f);
    //[SerializeField, Header("�������")] private Vector3 lookAtPlayerLerpTime;
    //[SerializeField, Header("����")] private bool isLockOn;
    //[SerializeField, Header("��ײ���")] public LayerMask collisionLayer;
    //[SerializeField] private float colliderMotionLerpTime;

    //private Vector3 rotationSmoothVelocity;
    //private Vector3 currentRotation;
    //private Vector3 _camDirection;

    ////����ͷ������ײʱ�������ɫ�ľ���
    //private float _cameraDistanceScrollWheel;
    //private float yaw;
    //private float pitch;
    //private bool start = false;

    //private void Awake()
    //{
    //    playerCamera = Camera.main.transform;
    //    //_inputSystem = GetComponent<CharacterInputSystem>();
    //}
    //private void Start()
    //{
    //    _camDirection = new Vector3(0f, 0f, -1f);
    //    _cameraDistance = _cameraDistanceMinMax.y;
    //}

    //private void Update()
    //{
    //    UpdateCursor();
    //    //GetCameraControllerInput();
    //    ZoomView();

    //}
    ////LateUpdate��Update֮��ִ��
    //private void LateUpdate()
    //{
    //    //ControllerCamera();
    //    //CheckCameraOcclusionAndCollision(playerCamera);
    //}

    ////�������
    //private void UpdateCursor()
    //{

    //}

    ////��ȡ������루�������һ�����
    //private void GetCameraControllerInput()
    //{
    //    if (isLockOn) return;
    //    yaw += CharacterInputSystem.Instance.cameraLook.x * RotateSpeed;
    //    pitch -= CharacterInputSystem.Instance.cameraLook.y * RotateSpeed;
    //    //Mathf.Clamp(a,b,c):�޶�a��b~c֮��
    //    pitch = Mathf.Clamp(pitch, ClmpCameraRang.x, ClmpCameraRang.y);
    //}

    ////����ͷ��ת���ƶ�
    //private void ControllerCamera()
    //{
    //    //��������ת
    //    if (!isLockOn)
    //    {
    //        //��currentRotationƽ���Ĺ��ɵ�Vector3(pitch, yaw)
    //        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
    //        //��������ת
    //        transform.eulerAngles = currentRotation;
    //    }
    //    Vector3 fanlePos = LookAttarGet.position - transform.forward * distancePlayerOffset;

    //    Vector3 positionX = new Vector3(transform.position.x, 0, 0);
    //    Vector3 positionY = new Vector3(0, transform.position.y, 0);
    //    Vector3 positionZ = new Vector3(0, 0, transform.position.z);

    //    positionX = Vector3.Lerp(positionX, fanlePos, lookAtPlayerLerpTime.x * Time.deltaTime);
    //    positionY = Vector3.Lerp(positionY, fanlePos, lookAtPlayerLerpTime.y * Time.deltaTime);
    //    positionZ = Vector3.Lerp(positionZ, fanlePos, lookAtPlayerLerpTime.z * Time.deltaTime);

    //    transform.position = new Vector3(positionX.x, positionY.y, positionZ.z);
    //}

    ////�����ײ���
    //private void CheckCameraOcclusionAndCollision(Transform camera)
    //{
    //    //TransformPoint:���������ת��Ϊ��������
    //    Vector3 desiredCamPosition = transform.TransformPoint(_camDirection * (_cameraDistanceMinMax.y + 0.1f));

    //    //Physics.Linecast(Vector3 start, Vector3 end, out hit, int Layer) start:������㣻end:�����յ㣻hit:������ײ�����Ϣ��Layer:���Ĳ㼶
    //    if (Physics.Linecast(transform.position, desiredCamPosition, out var hit, collisionLayer))
    //    {
    //        //����⵽���ڵ�����ײ�壩ʱ����������ͷ���ɫ��������
    //        _cameraDistance = Mathf.Clamp(hit.distance, _cameraDistanceMinMax.x, _cameraDistanceScrollWheel);

    //        start = true;
    //    }
    //    else
    //    {
    //        //�����ڵ���Ϊ���ڵ�ʱ����ǰ�滺�������ͷ���ɫ�ľ���ʵ����
    //        if (start)
    //        {
    //            _cameraDistance = _cameraDistanceScrollWheel;
    //            start = false;
    //        }
    //        //��������ͷ���ɫ�ľ���
    //        _cameraDistanceScrollWheel = _cameraDistance;

    //    }
    //    //�������ߣ����ڹ۲�
    //    Debug.DrawRay(transform.position, (desiredCamPosition - transform.position) * (_cameraDistance) / (_cameraDistanceMinMax.y + 0.1f), Color.red);

    //    camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, _camDirection * (_cameraDistance - 0.1f), colliderMotionLerpTime * Time.deltaTime);

    //}

    ////��ͷԶ��
    //void ZoomView()
    //{
    //    _cameraDistance -= CharacterInputSystem.Instance.CameraDistance * scrollSpeed * 0.01f;
    //    //Զ����Χ
    //    _cameraDistance = Mathf.Clamp(_cameraDistance, _cameraDistanceMinMax.x, _cameraDistanceMinMax.y);
    //    _cameraDistanceScrollWheel -= CharacterInputSystem.Instance.CameraDistance * scrollSpeed * 0.01f;
    //    _cameraDistanceScrollWheel = Mathf.Clamp(_cameraDistanceScrollWheel, _cameraDistanceMinMax.x, _cameraDistanceMinMax.y);

    //}

}
