using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using DG.Tweening;
using Animancer;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;
using Cinemachine.Utility;

namespace Assista.FSM
{
    public class EnemyStateMachine : MonoBehaviour, IDamagar
    {
        //״̬����ǰ״̬
        [SerializeField, Header("��ǰ״̬")] public EnemyStateBase CurrentStatePrefab;
        private EnemyStateBase CurrentState;

        [SerializeField, Header("����״̬")] private List<CharacterStates> States = new List<CharacterStates>();

        private EnemyIdleState idleState;
        private EnemyDieState DieState;
        private EnemyHitState HitState;
        private EnemyWalkState WalkState;
        private EnemyCombatState CombatState;
        private EnemyRangeCombatState RangeCombatState;
        private EnemyDisapperChargeState DisapperChargeState;
        private EnemyKnockDownState KnockDownState;
        private EnemyAirHitState AirHitState;


        //BOSS���׶ο�ʼʱ��������
        public GameObject fog;


        //���ڴ洢����״̬���ֵ�
        public Dictionary<string, EnemyStateBase> StatesDictionary = new Dictionary<string, EnemyStateBase>();
        //���ڴ洢������Ч�������
        public Dictionary<string, AudioSource> AudioSourcesDictionary = new Dictionary<string, AudioSource>();
        public Transform Camera { get; set; }


        //����
        [SerializeField] private Transform detectionCenter;
        [SerializeField, Header("���˼��")] private float detectionRang;
        //���
        [SerializeField] protected LayerMask whatisPlayer;
        [SerializeField] protected LayerMask whatisGround; //�ϰ���
        //�����⵽�ĵ���Ŀ��
        [SerializeField] private Collider[] detectionedTarget = new Collider[1];
        [SerializeField, Header("Ŀ��")] public Transform currentTarget;
        public BOSSDataSO dataSO;

        //��ɫ��ֵϵͳ����ʱд����
        //Ѫ������
        public float maxHealth;
        //��ǰѪ��
        public float health;

        public Image hpSlider; // Boss Ѫ��
        public Image staggerSlider; // Boss �Ͷ���

        private CharacterController Controller;
        private Camera mainCamera;
        [SerializeField] private Transform damagarNumericalValueTransform;
        public Transform Hit;
        //public float BossToughen = 100f;
        public bool RefreshToughen = false;
        private Coroutine toughenRecoveryCoroutine;

        //BOSS�����еĴ���
        public int hitCount = 0;
        public bool �Ѿ����� = false;

        public bool BOSS���ҵ����� = false;
        //���ֵ���ұ�����ʱ����BOSS        //BOSS�Ĳ���mesh
        public GameObject body;
        public Material targetMaterial;
        public Material originMaterial;
        public bool isDisapperCharge;
        public GameObject player;


        //���Q���ܿ���BOSS�Ŀ�����
        public bool isopen;

        //���˵�Ѫǰ�Ĳ�����ɫ
        public Texture originalTexture;

        //��¼��ɫʹ�û����Թ���
        public bool isFireAttack = false;

        //���˱��������ı��,���ڼ�¼�����Ƿ��ڡ��쳣��״̬
        public bool isAttackedByPlayer = false;
        public float exceptionTime = 0f;

        protected void Awake()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            dataSO = GetComponent<BOSSDataSO>();
            dataSO.Unbalance = dataSO.MaxUnbalance;
            RefreshToughen = false;
            �Ѿ����� = false;
            isDisapperCharge = false;

            idleState = new EnemyIdleState();
            HitState = new EnemyHitState();
            DieState = new EnemyDieState();
            WalkState = new EnemyWalkState();
            CombatState = new EnemyCombatState();
            RangeCombatState = new EnemyRangeCombatState();
            DisapperChargeState = new EnemyDisapperChargeState();
            KnockDownState = new EnemyKnockDownState();
            AirHitState = new EnemyAirHitState();
            AIView();
            InitCharacterStates();

            //InitCharacteraudioSource();

            /*//��ʼ��Ѫ��
            maxHealth = 50000f;
            health = maxHealth;*/
            
            CurrentState = CurrentStatePrefab.GetComponent<EnemyStateBase>();

            idleState.Paste(CurrentState.Copy());
            idleState?.OnEnter();

            //hpSlider.fillAmount = dataSO.maxHealth;
            //staggerSlider.fillAmount = dataSO.MaxUnbalance;

            isopen = false;
        }

        void Start()
        {
            originalTexture = body.GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture;
            Controller = transform.GetComponentInParent<CharacterController>();
            BackLastState("Idle");
        }

        void Update()
        {
            //����BOSS�Ĳ�����ɫ���Ƿ����쳣״̬��,����15s

            if (isAttackedByPlayer && exceptionTime < 15f && isFireAttack)
            {
                body.GetComponent<SkinnedMeshRenderer>().materials[0].color = Color.red;
                exceptionTime += Time.deltaTime;
                Debug.LogWarning(isFireAttack);
                Debug.LogWarning(exceptionTime);
                Debug.LogWarning(isAttackedByPlayer);
            }
            else
            {
                isFireAttack = false;
                body.GetComponent<SkinnedMeshRenderer>().materials[0].color = Color.white;
                exceptionTime = 0f;
                isAttackedByPlayer = false;
            }

            if (dataSO.health <= 0)
            {
                BackLastState("idle");
            }
            AIView();
            CurrentState?.OnUpdate();

            hpSlider.fillAmount = dataSO.health/dataSO.maxHealth;
            staggerSlider.fillAmount = dataSO.Unbalance/dataSO.MaxUnbalance;

            // �����Ҫ�ָ����ԣ�����Э����δ���У��������ָ�Э��
            if (RefreshToughen && toughenRecoveryCoroutine == null)
            {
                toughenRecoveryCoroutine = StartCoroutine(RecoverToughen());
            }

        }

        #region �����Թ���
        public void FireAttack()
        {
            isFireAttack = true;
        }

        #endregion

        // ���Իָ���Э��
        private IEnumerator RecoverToughen()
        {
            float recoverSpeed = 20f; // ÿ��ָ���������
            float maxToughen = dataSO.MaxUnbalance;  // �趨�������ֵ

            while (dataSO.Unbalance < maxToughen && RefreshToughen)
            {
                dataSO.Unbalance += recoverSpeed * Time.deltaTime;
                dataSO.Unbalance = Mathf.Min(dataSO.Unbalance, maxToughen); // �������ֵ
                yield return null; // �ȴ���һ֡
            }

            // �ָ���ɺ����Э�̱���
            toughenRecoveryCoroutine = null;
            RefreshToughen = false;
            �Ѿ����� = false;
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
                    if (Type.GetType(States[i].State.name) == idleState.GetType())
                    {
                        idleState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, idleState);
                        idleState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == DieState.GetType())
                    {
                        DieState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, DieState);
                        DieState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == HitState.GetType())
                    {
                        HitState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, HitState);
                        HitState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == WalkState.GetType())
                    {
                        WalkState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, WalkState);
                        WalkState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == RangeCombatState.GetType())
                    {
                        RangeCombatState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, RangeCombatState);
                        RangeCombatState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == CombatState.GetType())
                    {
                        CombatState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, CombatState);
                        CombatState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == DisapperChargeState.GetType())
                    {
                        DisapperChargeState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, DisapperChargeState);
                        DisapperChargeState.InitState(this);
                    }                    
                    if (Type.GetType(States[i].State.name) == KnockDownState.GetType())
                    {
                        KnockDownState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, KnockDownState);
                        KnockDownState.InitState(this);
                    }                    
                    if (Type.GetType(States[i].State.name) == AirHitState.GetType())
                    {
                        AirHitState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, AirHitState);
                        AirHitState.InitState(this);
                    }
                }
            }
        }

        private void InitCharacteraudioSource()
        {
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



        /// <summary>
        /// ״̬ת��
        /// </summary>
        /// <param name="toState"></param>
        public void BackLastState(string toState)
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
        /// ������⵽�ĵ����о��������һ�����
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

        /// <summary>
        /// �������������һ�����
        /// </summary>
        public void SeekThePlayer()
        {
            //������巶Χ�ڵ�Ŀ��
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, whatisPlayer);
            //Debug.Log("������巶Χ�ڵ�Ŀ��:" + targetCount);


            if (targetCount > 0)
            {
                currentTarget = searchNearestTarget(targetCount).transform;
            }
            else
            {
                //currentTarget = null;
            }
        }

        /// <summary>
        /// �����������Ƿ������Ұ��Χ
        /// </summary>
        /// <returns></returns>
        public void AIView()
        {
            // ��ȡ��⵽��Ŀ������
            int targetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRang, detectionedTarget, whatisPlayer);

            if (targetCount > 0)
            {
                Transform player = detectionedTarget[0].transform;
                Vector3 enemyPos = transform.parent.position;
                Vector3 toPlayer = (player.position - enemyPos).normalized;

                // �����������˵ľ���
                float distanceToPlayer = Vector3.Distance(enemyPos, player.position);

                // ����������˵ľ���С�� 2f��ʹ�� CheckSphere �����
                if (distanceToPlayer < 2f)
                {
                    // �������˼���Բ�η�Χ����ɫ��
                    Debug.DrawRay(enemyPos, toPlayer * 2f, Color.green);

                    // ʹ�� CheckSphere ����Բ�η�Χ���
                    Collider[] detected = Physics.OverlapSphere(enemyPos, 2f, whatisPlayer);
                    if (detected.Length > 0)
                    {
                        currentTarget = player;
                        Debug.Log("�����Բ�η�Χ�ڣ�");
                    }
                }
                else
                {
                    // �����ӵ��˵���ҵ����ߣ���ɫ��
                    Debug.DrawRay(enemyPos + transform.parent.up * 0.5f, toPlayer * detectionRang, Color.red);

                    // ����ϰ���
                    if (!Physics.Raycast(enemyPos + transform.parent.up * 0.5f, toPlayer, out var hit, detectionRang, whatisGround))
                    {
                        float dot = Vector3.Dot(toPlayer, transform.parent.forward);
                        //Debug.Log($"�������˷�����: {dot}");

                        // �������˵�ǰ������ɫ��
                        Debug.DrawRay(enemyPos, transform.parent.forward * 2f, Color.blue);

                        if (dot > 0.25f)
                        {
                            currentTarget = player;
                            Debug.Log("��ҽ�����Ұ��Χ��");
                        }
                    }
                }
            }
        }

        public void DisapperCharge_Combat_Ground()
        {
            body.SetActive(false);
            isDisapperCharge = true;
            //StartCoroutine(LongPressWarp());
            float forwardOffset = -4f; // ǰ����ƫ����
            Vector3 newPosition = player.transform.position - player.transform.right * forwardOffset;
            newPosition.y = transform.parent.position.y; // ����Y��߶�
            Debug.Log(newPosition);
            transform.parent.position = newPosition;
            // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
            body.SetActive(true);
        }

        public void DisapperCharge_Combat_Ground_2stage()
        {
            //StartCoroutine(LongPressWarp());
            body.SetActive(true);

            Vector3 newPosition = player.transform.position - player.transform.forward * 5f;
            newPosition.y = transform.parent.position.y; // ����Y��߶�
            transform.parent.position = newPosition;
            // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
        }

        public void DisapperCharge_Combat_Ground_2stage_End()
        {
            //StartCoroutine(LongPressWarp());
            body.SetActive(false);
            Vector3 newPosition = player.transform.position - player.transform.forward * 20f;
            newPosition.y = transform.parent.position.y; // ����Y��߶�
            transform.parent.position = newPosition;
            // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
        }

        public void SetBoolDisapperCharge()
        {
            isDisapperCharge = false;
        }

        public void FacePlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // �ҵ����
            if (player != null)
            {
                Vector3 direction = player.transform.position - gameObject.transform.parent.position; // ���㳯������
                direction.y = 0; // ֻ��ˮƽ����ת����ֹ BOSS ��ת
                if (direction != Vector3.zero)
                {
                    gameObject.transform.parent.rotation = Quaternion.LookRotation(direction); // �� BOSS �������
                }
            }
        }

        public void TakeDamager(float damagar, string hitAnimationName, Transform attacker, AudioClip audioClip, GameObject gameObject)
        {
            StatesDictionary["Hit"].String = hitAnimationName;
            StatesDictionary["Hit"].Clip = audioClip;
            BackLastState("Hit");
            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            Debug.Log(damagar);
            //�ܻ�ʱ���򹥻���
            transform.parent.rotation = transform.parent.LockOnTarget(attacker, transform.parent, 100f);
            //��Ѫ
            dataSO.health -= damagar * UnityEngine.Random.Range(1, 2);

            //GameObjectPoolSystem.Instance.TakeGameObject("damagarNumericalValue", damagarNumericalValueTransform).GetComponent<damagarNumericalValue>().Create(damagar * UnityEngine.Random.Range(1, 2), mainCamera);
            if (dataSO.health <= 0)
            {
                BackLastState("Die");

            }
        }

        public void TakeDamager_NoSound(float damager, string hitAnimation, Transform attacker)
        {
            hitCount++;
            isAttackedByPlayer = true;
            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            if (isDisapperCharge) { return; }
            Debug.Log(damager);
            //�ܻ�ʱ���򹥻���
            if (RefreshToughen && �Ѿ����� && gameObject.GetComponent<CharacterMoveMentControllerBase>()._Controller.isGrounded)
            { }
            else
            {
                transform.parent.rotation = transform.parent.LockOnTarget(attacker, transform.parent, 200f);
            }

            //��Ѫ
            if(!RefreshToughen)
            {
                if (attacker.GetComponent<PlayerStateMachine>().isRecordForPODUN)
                {
                    dataSO.Unbalance -= 30f;
                    attacker.GetComponent<PlayerStateMachine>().energy += 30f;
                }
                else
                {
                    dataSO.Unbalance -= 5f;
                    attacker.GetComponent<PlayerStateMachine>().energy += 1f;
                }
                attacker.GetComponent<PlayerStateMachine>().isRecordForPODUN = false;
                if(attacker.GetComponent<PlayerStateMachine>().isQ_OVERLOADING_FireState)
                {
                    dataSO.health -= 12 * damager * UnityEngine.Random.Range(1, 2);
                }
                else
                {
                    dataSO.health -= 4 * damager * UnityEngine.Random.Range(1, 2);
                }
            }
            //������ʻ����ʾ��Ѫ
            body.GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = null;
            StartCoroutine(FlashWhite());

            if (dataSO.Unbalance <= 0f && !RefreshToughen)
            {
                attacker.GetComponent<PlayerStateMachine>().energy += 20f;
            }

            if (dataSO.Unbalance <= 0.1f && !RefreshToughen)
            {
                if (attacker.GetComponent<PlayerStateMachine>().isQ_OVERLOADING_FireState)
                {
                    dataSO.health -= 120 * damager * UnityEngine.Random.Range(1, 2);
                }
                else
                {
                    dataSO.health -= 70 * damager * UnityEngine.Random.Range(1, 2);
                }
                StatesDictionary["Hit"].String = hitAnimation;
                //��BOSS���ʱ��
                //body.GetComponent<SkinnedMeshRenderer>().material = targetMaterial;
                BackLastState("Hit");
                //body.GetComponent<SkinnedMeshRenderer>().material = originMaterial;
                //BOSS�Ͷȿ�ʼ�ָ�
                RefreshToughen = true;
            }
            if (RefreshToughen && �Ѿ�����)
            {
                if(!gameObject.GetComponent<CharacterMoveMentControllerBase>()._Controller.isGrounded)
                {
                    if (attacker.GetComponent<PlayerStateMachine>().isQ_OVERLOADING_FireState)
                    {
                        dataSO.health -= 140 * damager * UnityEngine.Random.Range(1, 2);
                    }
                    else
                    {
                        dataSO.health -= 90 * damager * UnityEngine.Random.Range(1, 2);
                    }
                    if (BOSS���ҵ�����)
                    {
                        BOSS���ҵ����� = false;
                        Debug.Log("BOSS���ҵ�����");
                        gameObject.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30f;
                        gameObject.GetComponent<CharacterMoveMentControllerBase>().currentGravity = -10f;
                    }
                    StatesDictionary["AirHit"].String = "AirHit";
                    BackLastState("AirHit");
                }
                else
                {
                    if (attacker.GetComponent<PlayerStateMachine>().isQ_OVERLOADING_FireState)
                    {
                        dataSO.health -= 100 * damager * UnityEngine.Random.Range(1, 2);
                    }
                    else
                    {
                        dataSO.health -= 50 * damager * UnityEngine.Random.Range(1, 2);
                    }
                    StatesDictionary["KnockDown"].String = hitAnimation;
                    BackLastState("KnockDown");
                }
            }

            
            //BOSSѪ��Ϊ0 ����
            //if (dataSO.health <= 0)
            //{
            //    BackLastState("Die");
            //}
        }

        public void BOSSAirHitDown()
        {
            dataSO.health -= 40f * 20f * UnityEngine.Random.Range(1, 2);

            StatesDictionary["AirHit"].String = "AirHitDown";
            BackLastState("AirHit");
        }

        IEnumerator FlashWhite()
        {
            yield return new WaitForSeconds(0.05f);
            body.GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = originalTexture;

        }

        public Transform GetCurrentTarget()
        {
            if(currentTarget == null)
            {
                currentTarget = GameObject.FindWithTag("Player").transform;
            }
            return currentTarget;
        }

        public float GetCurrentTargetDistance() => Vector3.Distance(currentTarget.position, transform.parent.position);

        [System.Serializable]
        private class CharacterStates
        {
            public string StateName;
            public EnemyStateBase State;
        }
    }
}

