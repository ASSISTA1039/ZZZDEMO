using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assista.FSM
{
    public abstract class StateMachineBase : MonoBehaviour
    {
        //��ҿ�����
        //[SerializeField, Header("��ҿ�����")] public CharacterMoveMentControllerBase _Controller;
        public float boxCombatF_DistanceUpDown;
        public bool iswudi;
        //״̬����ǰ״̬
        [SerializeField, Header("��ǰ״̬")] public StateBaseSO CurrentState;

        [SerializeField, Header("����״̬")] private List<CharacterStates> States = new List<CharacterStates>();

        //���ڴ洢����״̬���ֵ�
        public Dictionary<string, StateBaseSO> StatesDictionary = new Dictionary<string, StateBaseSO>();
        //���ڴ洢������Ч�������
        public Dictionary<string, AudioSource> AudioSourcesDictionary = new Dictionary<string, AudioSource>();
        public Transform Camera { get; set; }
        //����
        [SerializeField] public Transform currentTarget { get; set; }

        //����
        [SerializeField, Header("���˼��")] public float detectionRang;
        [SerializeField] protected LayerMask enemyLayer;
        //�����⵽�ĵ���Ŀ��
        [SerializeField] protected Collider[] detectionedTarget = new Collider[5];

        //�̶�����BOSS
        [SerializeField] public Collider BOSSTarget;

        //�����ת
        [SerializeField] public PlayerCameraUtility playerCameraUtility;
        [field: SerializeField] public List<PlayerCameraRecenteringData> SidewaysCameraRecenteringData { get; private set; }

        [field: SerializeField] public List<PlayerCameraRecenteringData> BackWardsCameraRecenteringData { get; private set; }


        //����Q���ܴ��У���Ҫ��BOSS���־�ֹ
        public bool isBOSSStaticStop = false;



        //��ɫ��ֵϵͳ����ʱд����
        //Ѫ������
        public float maxHealth;
        //��ǰѪ��
        public float health;

        public float MaxEnergy = 100f;
        public float energy = 0;
        public Image hpSlider; // ��� Ѫ��
        public Image enegySlider; // ��� ʱ��������

        public float attack = 500f;

        protected void Awake()
        {
            InitCharacterStates();
            playerCameraUtility.Init();
            Transform parent = transform.parent.GetComponent<Transform>();
            //��ȡ�����е���Ч��������洢���ֵ���
            if (parent != null)
            {
                //���������������������
                foreach (Transform child in parent)
                {
                    //������ǰGameObject����
                    if (child != transform)
                    {
                        //�����ֵ����������������
                        foreach (Transform subChild in child)
                        {
                            AudioSource audioSource = subChild.GetComponent<AudioSource>();
                            if (audioSource != null)
                            {
                                //����б��һ��Ԫ�ص������Ƿ��Ѿ��ڳ��������ˣ�û�еĻ��ʹ���һ��
                                if (!AudioSourcesDictionary.ContainsKey(audioSource.name))
                                {
                                    AudioSourcesDictionary.Add(audioSource.name, audioSource);
                                }
                                //Debug.Log("name:" + audioSource.name);
                            }
                        }
                    }
                }
            }
        }

        void Start()
        {
            MaxEnergy = 100f;
            energy = 0f;
        }

        void Update()
        {

        }

        //��ʼ��״̬��
        private void InitCharacterStates()
        {
            if (States.Count == 0) return;

            //���ȱ����������õ���Դ
            for (int i = 0; i < States.Count; i++)
            {
                //����б��һ��Ԫ�ص������Ƿ��Ѿ��ڳ��������ˣ�û�еĻ��ʹ���һ��
                if (!StatesDictionary.ContainsKey(States[i].StateName))
                {
                    StatesDictionary.Add(States[i].StateName, States[i].State);

                    States[i].State.InitState(this);

                }
            }
        }

        /// <summary>
        /// ״̬ת��
        /// </summary>
        /// <param name="toState"></param>
        public void BackLastState(String toState)
        {
            if (!StatesDictionary.ContainsKey(toState))
            {
                Debug.Log("û���ҵ���" + toState + "��״̬��");
                return;
            }

            //ִ�о�״̬�Ľ����¼�
            CurrentState?.OnExit();
            //�滻��״̬
            CurrentState = StatesDictionary[toState];
            //ִ����״̬�Ŀ�ʼ�¼�
            CurrentState?.OnEnter();
        }



        /// <summary>
        /// �������������һ������
        /// </summary>
        public void SeekTheEnemy()
        {
            //������巶Χ�ڵ�Ŀ��
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
            //Debug.Log("������巶Χ�ڵ�Ŀ��:" + targetCount);


            if (targetCount > 0)
            {
                currentTarget = searchNearestTarget(targetCount).transform;
                //Debug.Log(detectionedTarget[0].transform.position);
            }
            else
            {
                currentTarget = null;
            }
        }


        /// <summary>
        /// �ж��Ƿ����ڹ�������
        /// </summary>
        public bool AnyEnemyInRange()
        {
            //������巶Χ�ڵ�Ŀ��
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
            if (targetCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ������⵽�ĵ����о��������һ������
        /// </summary>
        /// <returns></returns>
        private Collider searchNearestTarget(int targetCount)
        {
            //��ʼ��һ����������롱Ϊ�����,��һ�����ڴ������Ŀ��ı���
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            //�������飬���Ը������������Ŀ��
            for (int i = 0; i < targetCount; i++)
            {
                //Debug.Log("��ײ�����ĵ�: " + detectionedTarget[0].bounds.center);

                float distance = Vector3.Distance(detectionedTarget[i].transform.position, transform.position);
                //����������С��֮ǰ�ҵ����������
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = detectionedTarget[i];
                    //Debug.Log(closestCollider.transform.position);
                }
            }
            return closestCollider;
        }




        [Serializable]
        private class CharacterStates
        {
            public string StateName;
            public StateBaseSO State;
        }


    }




}

