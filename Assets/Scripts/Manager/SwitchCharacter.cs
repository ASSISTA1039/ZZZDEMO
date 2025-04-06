using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assista.FSM;
using UnityEngine.TextCore.Text;
using Cinemachine;

public class SwitchCharacter : MonoBehaviour
{
    [System.Serializable]
    public class SwicthCharacterInfo
    {
        public string characterName; // ��ɫ������
        public Vector3 newTransfrom; // �½�ɫλ��
        public Transform lookAtPos; // �ӽ�λ��
        public Transform followAtPos; // �ӽ�λ��
        public GameObject character; // ��ɫ����

        [HideInInspector] public Animator animator; // �����ʼ�������㲥��ÿ��������ɫ�Ķ���
        //[HideInInspector] public CharacterInputSystem characterInputSystem;
        [HideInInspector] public CharacterController Controller;
        [HideInInspector] public PlayerStateMachine StateMachine;
    }

    [SerializeField, Header("���н�ɫ")] private List<SwicthCharacterInfo> swicthCharacterInfos = new List<SwicthCharacterInfo>(); // ���н�ɫ����Ϣ�б�

    private Queue<SwicthCharacterInfo> Characters = new Queue<SwicthCharacterInfo>();

    private bool canSwichInput; // �Ƿ�����л���ɫ
    //[SerializeField, Header("�л���ɫ�Ļ���ʱ��")] private float applyNextSwitchTime;
    [SerializeField, Header("�˳���ɫ�Ĵ���ʱ��")] private float switchOutCharacterTime;

    private SwicthCharacterInfo currentCharacter;//��ǰ��ɫ
    private SwicthCharacterInfo oldCharacter;// �ɽ�ɫ

    [SerializeField, Header("���")] private CinemachineVirtualCamera Camera; // ����������������Ŀ���

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public LayerMask groundLayer;


    //��ɫ1������ʱ�л���ɫ2��������ɫ2����
    public bool isCombat_Q = false;

    // ��Awake�����г�ʼ����������ͽ�ɫ��Ϣ
    private void Awake()
    {
        InitCharacter();
    }

    // ��InitCharacter�����л�ȡ���н�ɫ�Ķ���������������ϵͳ���ܻ��ű�
    private void InitCharacter()
    {
        for (int i = 0; i < swicthCharacterInfos.Count; i++)
        {
            swicthCharacterInfos[i].animator = swicthCharacterInfos[i].character.transform.GetComponentInChildren<Animator>();

            //swicthCharacterInfos[i].characterInputSystem = swicthCharacterInfos[i].character.transform.GetComponent<CharacterInputSystem>();

            swicthCharacterInfos[i].Controller = swicthCharacterInfos[i].character.transform.GetComponent<CharacterController>();

            swicthCharacterInfos[i].StateMachine = swicthCharacterInfos[i].character.transform.GetComponentInChildren<PlayerStateMachine>();
            //swicthCharacterInfos[i].StateMachine.Awake();
            //swicthCharacterInfos[i].StateMachine.Start();

            Characters.Enqueue(swicthCharacterInfos[i]);

            swicthCharacterInfos[i].character.SetActive(false);
        }

    }

    // ��Start���������ó�ʼ�Ľ�ɫ
    private void Start()
    {
        canSwichInput = true;
        //��1��λ��ɫ���Ӳ�����
        currentCharacter = Characters.Dequeue();
        currentCharacter.character.SetActive(true);
        //���������
        Camera.LookAt = currentCharacter.lookAtPos;
        Camera.Follow = currentCharacter.followAtPos;

    }

    // ��Update�����м���Ƿ���Ҫ�л���ɫ
    private void Update()
    {
         SwtichInput();
    }

    private void SwtichInput()
    {
        // ��������л���ɫ��ֱ�ӷ���
        if (!canSwichInput) return;
        // �л���ɫ������
        switch (CharacterInputSystem.Instance.playerSwitchType)
        {
            case 1:
                // ��ֹ��ɫ�л�
                canSwichInput = false;

                oldCharacter = currentCharacter;
                //oldCharacter.characterInputSystem.enabled = false;

                oldCharacter.StateMachine.BackLastState("Exit");


                CharacterSwtich(oldCharacter);
                Characters.Enqueue(oldCharacter);
                oldCharacter.character.SetActive(false);
                // ��ʼЭ��
                StartCoroutine(MyCoroutine(oldCharacter));
                break;
            case 2:
                // ��ֹ��ɫ�л�
                canSwichInput = false;

                oldCharacter = currentCharacter;
                //oldCharacter.characterInputSystem.enabled = false;
                oldCharacter.StateMachine.BackLastState("Exit");

                CharacterSwtich(oldCharacter);

                Characters.Enqueue(oldCharacter);
                oldCharacter.character.SetActive(false);
                // ��ʼЭ��
                StartCoroutine(MyCoroutine(oldCharacter));
                break;
            default:
                //Debug.Log("δ֪����");
                break;

        }
    }

    //Э�̣�һ��ʱ��󽫾ɽ�ɫ��Ӳ�����
    IEnumerator MyCoroutine(SwicthCharacterInfo Character)
    {
        yield return new WaitForSeconds(switchOutCharacterTime);
        canSwichInput = true;


    }

    private void CharacterSwtich(SwicthCharacterInfo Character)
    {
        currentCharacter = Characters.Dequeue();

        currentCharacter.character.SetActive(true);
        //�������
        //currentCharacter.characterInputSystem.enabled = true;
        //���������
        Camera.LookAt = currentCharacter.lookAtPos;
        Camera.Follow = currentCharacter.followAtPos;
        currentCharacter.character.GetComponent<CharacterMoveMentControllerBase>().isInAirAttack = false;
        //���ɽ�ɫ���ͷ�����У������½�ɫ�Ĵ�������Ҳ���˵�ʱ��
        if (isCombat_Q)
        {
            currentCharacter.StateMachine.BackLastState("Combat_Q");
            isCombat_Q = false;
        }
        else
        {
            //�������������
            if (CheckGrounded(Character))
            {
                currentCharacter.StateMachine.BackLastState("Admission");
            }
            else
            {
                currentCharacter.StateMachine.BackLastState("Jump");
            }
        }



        // �½�ɫλ��
        Vector3 oldPosition = Character.character.transform.position;
        Vector3 offset = Character.character.transform.forward * currentCharacter.newTransfrom.z +
                         Character.character.transform.right * currentCharacter.newTransfrom.x;

        // ����ɽ�ɫ�ڿ��У����½�ɫ��λ������Ϊͬһ�߶�
        Vector3 newPosition = oldPosition + offset;
        if (!Character.Controller.isGrounded)
        {
            newPosition.y = oldPosition.y; // �����½�ɫ�ĸ߶���ɽ�ɫһ��
        }
        currentCharacter.Controller.Move(newPosition - currentCharacter.character.transform.position);
        //�½�ɫ��ת
        currentCharacter.character.transform.rotation = Character.character.transform.rotation;

        //// ���ɽ�ɫ�Ĵ�ֱ�ٶȴ��ݸ��½�ɫ
        //var oldVerticalSpeed = Character.Controller.velocity.y;
        //var newRigidbody = currentCharacter.character.GetComponent<Rigidbody>();
        //if (newRigidbody != null)
        //{
        //    newRigidbody.velocity = new Vector3(newRigidbody.velocity.x, oldVerticalSpeed, newRigidbody.velocity.z);
        //}
    }



    private bool CheckGrounded(SwicthCharacterInfo Character)
    {
        return Physics.Raycast(Character.character.transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

}
