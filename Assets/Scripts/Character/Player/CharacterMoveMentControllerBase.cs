using System.Collections;
using System.Collections.Generic;
using Animancer;
using Assista.FSM;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public abstract class CharacterMoveMentControllerBase : MonoBehaviour
{
    public AnimancerComponent _Animancer;
    public CharacterController _Controller;
    protected PlayableDirector _PlayableDirector;
    
    
    protected PlayerControls _playerControls;
    [SerializeField, Tooltip("��ɫ�����ƶ�ʱ����ϰ���Ĳ㼶")] protected LayerMask whatIsObs;

    //���ڴ洢��ʷdeltaPosition�Ķ���
    private Queue<Vector3> deltaPositions = new Queue<Vector3>();

    private Vector3 averageDeltaPosition = new Vector3();

    //MoveDirection(�ƶ�������ˮƽ����ֱ)
    protected Vector3 movementDirection;
    protected Vector3 verticalDirection;

    [SerializeField, Header("��ɫ����")] public float characterGravity;
    public float currentGravity;
    private float tempGravity;
    //[SerializeField, Header("��ɫ��ǰ�ƶ��ٶ�")] protected float characterCurrentMoveSpeed;
    public float verticalSpeed;//��ǰ��ɫY���ٶ�
    public Vector3 inertia = Vector3.zero;
    public bool isInAirAttack = false; // ��־�������Ƿ��ڿ��й���״̬

    //��Ծ���
    private bool isReadyJump = false;

    //boss
    public GameObject BOSS;

    protected virtual void Start()
    {
        //_Animancer = GetComponent<AnimancerComponent>();
        _Controller = GetComponent<CharacterController>();
        if(_Controller == null)
        {
            _Controller = GetComponentInParent<CharacterController>();
        }
        _PlayableDirector = GetComponent<PlayableDirector>();
        _Animancer = GetComponent<AnimancerComponent>();
        //_Animancer.Playable.Speed = 2f;
        currentGravity = characterGravity;
        isInAirAttack = false;
    }

    protected virtual void Update()
    {
        GetAverageDeltaPosition();
        CaculateGravity();
    }


    protected void GetAverageDeltaPosition()
    {
        deltaPositions.Enqueue(_Animancer.Animator.deltaPosition);
        if (deltaPositions.Count > 10)
        {
            deltaPositions.Dequeue();
        }

        Vector3 virs = new Vector3();
        foreach (Vector3 vir in deltaPositions)
        {
            virs += vir;
        }
        averageDeltaPosition = virs / 10;

    }

    protected bool CanAnimationMotion()
    {
        Debug.DrawRay(transform.position + transform.up * 0.5f, averageDeltaPosition.normalized * 0.5f, Color.red);

        //���ǰ���Ƿ����ϰ���(characterAnimator.GetFloat(animationMoveID)�ٶȷ�ֹ�ڹ���ǰ��ʱ������ص�)
        return Physics.Raycast(transform.position + transform.up * 0.5f, averageDeltaPosition, out var hit, 0.5f, whatIsObs);

    }

    public Vector3 GetRelativeDiretion(Vector3 input)
    {
        Quaternion rot = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        Vector3 dir = rot * Vector3.forward * input.y + rot * Vector3.right * input.x;
        return new Vector3(dir.x, 0, dir.y).normalized;
    }


    public void SetJumpVelocityForUpper()
    {
        if(verticalSpeed < 1f)
        {
            verticalSpeed = 10f;
        }
        if(BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed < 1f)
        {
            //BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = 0f;
            BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 12f;
            
        }
    }

    public void SetJumpVelocityForDown()
    {

        if(_PlayableDirector.playableAsset.name.Contains("Jump") && verticalSpeed >= 0.1f)
        {
            //ȡ����ɫ�����ڿ��й�����������״̬
            isInAirAttack = false;
            currentGravity = characterGravity;
            //verticalSpeed = -5f;
        }
        else if (verticalSpeed > -10f)
        {
            //ȡ����ɫ�����ڿ��й�����������״̬
            isInAirAttack = false;
            currentGravity = characterGravity;
            verticalSpeed = -30f;

        }
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    public void SetJumpVelocityForDownBOSSS()
    {
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed -= 30f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    public void SetJumpFallVelocity()
    {
        isInAirAttack = false;
        currentGravity = characterGravity;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    /// <summary>
    /// ��ɫ����
    /// </summary>
    void CaculateGravity()
    {
        //Debug.Log(isInAirAttack);
        if (CharacterInputSystem.Instance.playerJump && _Controller.isGrounded && _PlayableDirector.playableAsset.name.Contains("_Jump"))
        {
            {
                verticalSpeed = 8f; // ����ɫһ����ʼ���ϵ��ٶ�
            }
        }
        //��ɫ�Ƿ�վ�ڵ��棨��ĳ����ײ�壩��
        else if (_Controller.isGrounded)
        {
            
            isInAirAttack = false; // һ���ŵأ��˳����й���״̬
            //����ֱ�ٶȹ���
            //verticalSpeed = 0;
            //�ڵ���ʱ��ֹ�ٶ������½�
            if (verticalSpeed < 0.0f)
            {
                verticalSpeed = -5f;
            }
        }
        else
        {
            // �����Ұ��¹������Ҳ��ڿ��й���״̬���������й���״̬
            if (CharacterInputSystem.Instance.playerLAtk && !isInAirAttack)
            {
                isInAirAttack = true;
                verticalSpeed = 0f; // �ڿ��й���ʱ���ִ�ֱ�ٶ�Ϊ��
                inertia = Vector3.zero;
            }
            if (isInAirAttack)
            {
                // ������ڿ��й���״̬����������Ӱ��
                verticalSpeed = 0f;

                inertia = Vector3.zero;
            }
            else
            {
                //����V = gt  (Time.deltaTime��ʾ����һ֡�뵱ǰ֡������������ȡ�һ֡��ʱ�䡱)
                if (verticalSpeed <= 0)
                {
                    //������½��׶Σ���ʱ�ļ��ٶ��������׶ε�1.3��
                    verticalSpeed += currentGravity * 5f * Time.deltaTime;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed += currentGravity * 5f * Time.deltaTime;


                }
                if (verticalSpeed < -30)
                {
                    verticalSpeed = -30;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30;

                }
                else
                {
                    verticalSpeed += currentGravity * Time.deltaTime;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed += currentGravity * Time.deltaTime;
                }

            }
        }


        //Debug.Log(verticalSpeed);
    }

    public virtual void AirNotAttack()
    {
        isInAirAttack = false;
    }
    public virtual void FreeVelocity()
    {
        float tempGravity = 0f;
        currentGravity = tempGravity;
        verticalSpeed = 0f; // �ڿ��й���ʱ���ִ�ֱ�ٶ�Ϊ��
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 0f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = tempGravity;
    }

    /// <summary>
    /// �ƶ��ӿ�
    /// </summary>
    /// <param name="moveDirection">�ƶ�����</param>
    /// <param name="moveSpeed">�ƶ��ٶ�</param>
    /// /// <param name="useGravity">�Ƿ�ʹ������</param>
    public virtual void CharacterMoveInterface(Vector3 moveDirection, float moveSpeed, bool useGravity)
    {
        //����ƶ������ǰ��û���ϰ���
        if (!CanAnimationMotion())
        {
            //�ƶ������׼��
            movementDirection = moveDirection.normalized;

            //�Ե�ǰ�ƶ���������¶ȼ��
            //movementDirection = ResetMoveDirectionOnSlop(movementDirection);

            //���ʹ������
            if (useGravity)
            {
                //����ֱ����Y�ḳֵ
                //verticalDirection.Set(0.0f, verticalSpeed, 0.0f);
            }
            else
            {
                //����
                verticalDirection = Vector3.zero;
            }

            //�ƶ�
            _Controller.Move((moveSpeed * Time.deltaTime) * movementDirection + Time.deltaTime * verticalDirection);
        }
    }





}
