using System.Collections;
//using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.ProBuilder.Shapes;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.UI;
using System.Linq;

namespace Assista.FSM
{
    public class PlayerStateMachine : StateMachineBase, IDamagar
    {
        public Transform TP_Camera => Camera;

        public Transform _Enemy => currentTarget;

        private CharacterController Controller;

        public GameObject body;

        private Camera mainCamera;
        [SerializeField] private Transform damagarNumericalValueTransform;

        public Transform Hit;

        //E/Q�������ɡ�ʱ�սڵ㡱�Լ���������ġ�ʱ�սڵ㡱
        public SphereManager sphereManager;
        //private Queue<GameObject> activeSpheres = new Queue<GameObject>(); // �Ѿ����������
        //public  GameObjectPoolSystem GameObjectPool;


        //�жϵ����Ƿ񹥻���
        public bool isEnemyAttacked = false;
        //�������������޵�֡�Ŀ��أ�
        public bool isInvincible=false;
        //�����޵��ڼ���ƶ��˺�����
        public bool isRecordForPODUN = false;
        //��������ʱ���ڵ���ײ���ⷶΧ����
        public float evade_Radius=2f;

        //----------------------------------------
        //Ϊ�¼��ṩ�ж��Ƿ����ͨ������rootmotion�ƶ�
        public bool canAnimMotion = true;

        //��ɫ�л�������
        public SwitchCharacter switchCharacter;


        public GameObject BOSS;
        private bool canTeleportInvincible;
        
        //��¼bossĿǰ�Ƿ��ڡ��쳣��״̬
        public Color bossCurrentColor;

        //��¼��ɫ�Ƿ����������⼼״̬
        public bool isQ_FireState = false;
        public bool isQ_OVERLOADING_FireState = false;
        //��¼��ɫ�Ƿ�ո�˲�Ƶ����˱���
        public bool isTransportJUSTNOW = false;

        // ��Ҫ���� Time.timeScale Ӱ�����Ƶ
        public AudioSource[] audioSources; 


        public void Awake()
        {
            //��ʼ��Ѫ��
            maxHealth = 500f;
            health = maxHealth;
            MaxEnergy = 100f;
            canTeleportInvincible = false;
            base.Awake();

            CurrentState?.OnEnter();

        }

        public void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            Controller = GetComponent<CharacterController>();
            energy = 0f;
            //BackLastState("Idle");

        }

        void Update()
        {
            if(isQ_OVERLOADING_FireState)
            {
                health -= 0.01f * maxHealth * Time.deltaTime;
            }

            if(isQ_FireState && energy > 0)
            {
                energy -= 0.1f * MaxEnergy * Time.deltaTime;
                if (energy <= 0)
                {
                    isQ_FireState = false;
                    //���ý�ɫ������ɫ�ָ�Ϊ��̬��ɫ
                    SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                    {
                        if (renderer.materials.Length > 0)
                        {
                            Material[] materials = renderer.materials;
                            materials[0].color = Color.white;
                            renderer.materials = materials;
                            isQ_OVERLOADING_FireState = false;
                        }
                    }
                }
            }

            bossCurrentColor = BOSS.GetComponent<EnemyStateMachine>().body.GetComponent<SkinnedMeshRenderer>().materials[0].color;
            CurrentState?.OnUpdate();

            if(isInvincible)
            {
                gameObject.GetComponent<CharacterController>().radius = evade_Radius;
            }
            else
            {
                gameObject.GetComponent<CharacterController>().radius = 0.2f;
            }

            hpSlider.fillAmount = health / maxHealth;
            enegySlider.fillAmount = energy / MaxEnergy;

            //����Ƿ��е��˶���������˺�
            if (isEnemyAttacked)
            {
                //����ڳ��״̬�£�����Ѫ
                if (CurrentState.name.Contains("Evade")| isInvincible | iswudi)
                {
                    //�������������ʱ����
                    if(isInvincible)
                    {
                        //canTeleportInvincible = true;
                        // ����ʱ�����Ч��
                        StartCoroutine(SlowMotionEffect());
                    }
                }
                //�����Ѫ
                else
                {
                    health -= 20;
                    
                }
            }
            isEnemyAttacked = false;
        }


        #region �����޵�״̬
        public void SetWudi(bool wudi)
        {
            iswudi = wudi;
        }
        #endregion

        #region ����BOSSʱͣ���������Ż���ͣ��
        public void SetBOSSStopAnima()
        {
            //currentTarget.gameObject.GetComponentInChildren<PlayableDirector>().Pause();
            BOSS.GetComponent<PlayableDirector>().Pause();
        }

        public void SetBOSSStartAnima()
        {
            BOSS.GetComponent<PlayableDirector>().Play();
        }
        #endregion

        #region ���ý�BOSS�������
        public void PushBOSS()
        {
            //StartCoroutine(PushBackCoroutine());
            BOSS.GetComponent<EnemyStateMachine>().BOSS���ҵ����� = true;
        }
        #endregion

        #region ������ҽ������������ܿ��Ը߶��ƶܵ�ʱ��
        public void SetRecordForPODUN()
        {
            isRecordForPODUN = false;
        }
        #endregion

        #region ���������Լ������ɫ�������ܵ�˲��
        //���������޵�֡�����ж�
        public void SetInvincibility(bool state)
        {
            isInvincible = state;
            StartCoroutine(EndInvincibility());
            Debug.Log(state ? "�޵�״̬����" : "�޵�״̬�ر�");
        }

        public void TeleportInvincible()
        {

            if (canTeleportInvincible)
            {
                // ˲�Ƶ�BOSS����
                canTeleportInvincible = false;

            }
        }

        private IEnumerator EndInvincibility()
        {

            yield return new WaitForSeconds(0.3f);
            isInvincible = false;
        }
        private IEnumerator SlowMotionEffect()
        {
            isRecordForPODUN = true;
            Time.timeScale = 0.2f; // ������Ч��
            //��ɫS�����������ܣ����ҵ��˱����Ϊ��ɫ�ġ��ۻ�״̬ʱ�����Դ�����˲�Ƶ���������ģ����������ĵ��˱���
            if (gameObject.name == "S" && bossCurrentColor.Equals(Color.red))
            {
                body.SetActive(false);
                isTransportJUSTNOW = true;
                canAnimMotion = false;
                StartCoroutine(LongPressWarp());
            }
            yield return new WaitForSecondsRealtime(0.5f); // ����������ʱ��
            Time.timeScale = 1f; // �ָ�����ʱ��
            isRecordForPODUN = false;
        }
        #endregion

        #region ǿ���л�����Ծ״̬
        public void SwitchToJumpState()
        {
            BackLastState("Jump");
        }
        #endregion

        #region ���ý�ɫ��ǰ������ɫ��BUFF״̬�£������ý�ɫQ�����ṩ��60%�ۻ�������ͬʱ��ǽ�ɫ��ǰΪ�ۻ�״̬
        public void SetPlayerColor()
        {
            SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                if (renderer.materials.Length > 0)
                {
                    Material[] materials = renderer.materials;
                    materials[0].color = Color.red;
                    renderer.materials = materials;
                }
            }
            //���ý�ɫQ�����ṩ��60%�ۻ�����
            energy += 0.6f * MaxEnergy;
            if (energy > MaxEnergy)
            {
                energy = MaxEnergy;
            }
            BOSS.GetComponent<EnemyStateMachine>().exceptionTime = 0f;
            isQ_FireState = true;
        }
        #endregion

        //#region ���ý�ɫQ�����ṩ��60%�ۻ�����,ͬʱ��ǽ�ɫ��ǰΪ�ۻ�״̬
        //public void SetPlayerQEnergyANDState()
        //{
        //    //���ý�ɫQ�����ṩ��60%�ۻ�����
        //    energy += 0.6f * MaxEnergy;
        //    if(energy > MaxEnergy)
        //    {
        //        energy = MaxEnergy;
        //    }
        //    isQ_FireState = true;
        //}
        //#endregion
        //��Ծ״̬��λ�ƹ���

        #region F���ܣ������ϳ���F�� / ˲�Ƶ�BOSSͷ�� / ��BOSSͷ�����ص���)
        public void AttackUpDown_Combat_Ground_LongPress()
        {
            canAnimMotion = false;
            StartCoroutine(LongPressWarp());

        }
        public void AttackUpDown_CombatF()
        {
            canAnimMotion = false;
            body.SetActive(false);
            StartCoroutine(UpWarp());

        }
        public void AttackUpDown_CombatF_BackToGround()
        {
            canAnimMotion = false;
            body.SetActive(false);
            StartCoroutine(DownWarp());
            Vector3 targetDirection = (BOSSTarget.transform.position).normalized;
            targetDirection.y = 0f;
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
        #endregion

        #region E���� ����˲�Ʋ�����ʱ�սڵ� 
        public void Attack_CombatE()
        {
            canAnimMotion = false;
            body.SetActive(false);
            StartCoroutine(Teleportation());
        }
        #endregion

        #region ˲�Ƶ���ͬλ�õĸ���Э��
        private IEnumerator LongPressWarp()
        {
            if (BOSSTarget)
            {
                // ��ȡBOSS�ĳ����λ��
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS�ĳ���
                Vector3 bossRight = bossTransform.right; // BOSS���Ҳ෽��

                // ���ݳ�����㴫�͵�
                float forwardOffset = -2f; // ǰ����ƫ����
                float verticalOffset = bossPosition.y; // ��ֱ����ƫ����
                float lateralOffset = 0f; // ����ƫ�����������е���

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // ����Y��߶�
                // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(() => {
                    Vector3 targetDirection = (BOSSTarget.transform.position).normalized;
                    targetDirection.y = 0f;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;
                }); // �ȴ����

            }
        }

        private IEnumerator UpWarp()
        {
            if (BOSSTarget)
            {
                //// ��ȡBOSSλ�ò�������ɫƫ��
                //Vector3 bossPosition = BOSSTarget.transform.position;
                //Vector3 newPosition = new Vector3(
                //    bossPosition.x,
                //    bossPosition.y + boxCombatF_DistanceUpDown, // ���Y��ƫ��
                //    bossPosition.z
                //);
                // ��ȡBOSS�ĳ����λ��
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS�ĳ���
                Vector3 bossRight = bossTransform.right; // BOSS���Ҳ෽��

                // ���ݳ�����㴫�͵�
                float forwardOffset = 1f; // ǰ����ƫ����
                float verticalOffset = bossTransform.position.y + boxCombatF_DistanceUpDown; // ��ֱ����ƫ����
                float lateralOffset = 0f; // ����ƫ�����������е���

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // ����Y��߶�
                // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
                yield return transform.DOMove(newPosition,0.1f).OnComplete(() => {
                    Vector3 targetDirection = (BOSSTarget.transform.position).normalized;
                    targetDirection.y = 0f;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;
                }); // �ȴ����

            }
        }

        private IEnumerator DownWarp()
        {
            if (BOSSTarget)
            {
                // ��ȡBOSS�ĳ����λ��
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS�ĳ���
                Vector3 bossRight = bossTransform.right; // BOSS���Ҳ෽��

                // ���ݳ�����㴫�͵�
                float forwardOffset = 1f; // ǰ����ƫ����
                float verticalOffset = -1.35f; // ��ֱ����ƫ����
                float lateralOffset = 0f; // ����ƫ�����������е���

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // ����Y��߶�
                // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(()=> { body.SetActive(true); canAnimMotion = true; }); // �ȴ����
                
                //Vector3 targetDirection = BOSSTarget.transform.position - transform.position;
                //Controller.Move(new Vector3(BOSSTarget.transform.position.x - transform.position.x, BOSSTarget.transform.position.y - transform.position.y + boxCombatF_DistanceUpDown, BOSSTarget.transform.position.z - transform.position.z).normalized * 60f * Time.deltaTime);
            }
        }

        private IEnumerator Teleportation()
        {
            if (BOSSTarget)
            {
                // ��ȡBOSS�ĳ����λ��
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS�ĳ���
                Vector3 bossRight = bossTransform.right; // BOSS���Ҳ෽��

                // ���ݳ�����㴫�͵�
                float forwardOffset = 1f; // ǰ����ƫ����
                float verticalOffset = -1.35f; // ��ֱ����ƫ����
                float lateralOffset = 0f; // ����ƫ�����������е���

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // ����Y��߶�

                // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(() =>
                {
                    // �ý�ɫ���� BOSS
                    Vector3 targetDirection = (bossPosition - transform.position).normalized;
                    targetDirection.y = 0f; // ����Y�����
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;

                    // ���� Sphere
                    sphereManager.SpawnSphere(transform.position);

                    // �����ɫ����
                    body.SetActive(true);
                });
            }
        }
        #endregion

        #region Q���ܴ���

        public void Attack_CombatQ()
        {
            canAnimMotion = false;
            body.SetActive(false);
            MovePlayerToNearestSphere();
        }

        // �ҵ���������岢������ƶ���ȥ
        private void MovePlayerToNearestSphere()
        {
            if (sphereManager.activeSpheres.Count == 0)
            {
                Debug.LogWarning("û�п��õ����壡");
                canAnimMotion = true;
                body.SetActive(true);
                return;
            }

            GameObject nearestSphere = null;
            float shortestDistance = float.MaxValue;
            //����Ƿ���BOSS���롰ʱ�սڵ㡱
            foreach(var sphere in sphereManager.activeSpheres)
            {
                if (sphere.activeInHierarchy) // ȷ�������Ǽ���״̬
                {
                    int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
                    if (targetCount > 0)
                    {
                        StartCoroutine(SmoothMove(sphere.transform.position));
                    }
                }
            }

            // �������м�������壬�ҵ������һ��
            foreach (var sphere in sphereManager.activeSpheres)
            {
                if (sphere.activeInHierarchy) // ȷ�������Ǽ���״̬
                {
                    float distance = Vector3.Distance(transform.position, sphere.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestSphere = sphere;
                    }
                }
            }

            if (nearestSphere != null)
            {
                // �ƶ���ҵ����������
                StartCoroutine(SmoothMove(nearestSphere.transform.position));
            }
        }

        // ƽ���ƶ�Э��
        private IEnumerator SmoothMove(Vector3 target)
        {
            // ��ȡ BOSS �ĳ����λ��
            Transform bossTransform = BOSSTarget.transform;
            Vector3 bossPosition = bossTransform.position;

            // �����Բ�ĵ� BOSS ��ʸ��
            Vector3 bossToTargetDir = (target - bossPosition).normalized;

            // ����Ŀ��λ�ã�ʹ����� BOSS һ������
            float desiredDistance = 2.0f; // ����Ҫ�ļ��
            Vector3 finalPosition = target + bossToTargetDir * desiredDistance;

            yield return transform.DOMove(finalPosition, 0.1f).OnComplete(() =>
            {
                // �ý�ɫ���� BOSS
                Vector3 targetDirection = (bossPosition - transform.position).normalized;
                targetDirection.y = 0f; // ����Y�����
                transform.rotation = Quaternion.LookRotation(targetDirection);
        
                canAnimMotion = true;
                body.SetActive(true);
            }); // �ȴ����
        }
        public void Q_Backward()
        {
            canAnimMotion = false;
            StartCoroutine(MoveBackWard());
        }

        private IEnumerator MoveBackWard()
        {
            float backwardOffset = -5f; // �󷽵�ƫ����
            float verticalOffset = -1.35f; // ��ֱ����ƫ����
            float lateralOffset = 0f; // ����ƫ�����������е���
            Vector3 directionToBoss = (BOSSTarget.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToBoss * backwardOffset + transform.right * lateralOffset;
            newPosition.y = verticalOffset; // ����Y��߶�

            // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
            yield return transform.DOMove(newPosition, 0.1f).OnComplete(() =>
            {
                canAnimMotion = true;
            });
        }
        public void Q2_MoveForward()
        {
            canAnimMotion = false;
            body.SetActive(false);
            StartCoroutine(MoveForward());
        }

        private IEnumerator MoveForward()
        {
            // ���ݳ�����㴫�͵�
            float forwardOffset = 7f; // ǰ����ƫ����
            float verticalOffset = -1.35f; // ��ֱ����ƫ����
            float lateralOffset = 0f; // ����ƫ�����������е���
            Vector3 directionToBoss = (BOSSTarget.transform.position - transform.position).normalized;

            Vector3 newPosition = transform.position + directionToBoss * forwardOffset + transform.right * lateralOffset;
            newPosition.y = verticalOffset; // ����Y��߶�

            // ʹ�� DOMove �����ƶ���ɫ������ 0.1 ��
            yield return transform.DOMove(newPosition, 0.1f).OnComplete(() =>
            {
                canAnimMotion = true;

                // �����ɫ����
                body.SetActive(true);
            });

        }
        #endregion

        #region Q�����л���ɫ2����
        public void SetCombat_Q_SwitchCharacter()
        {
            switchCharacter.isCombat_Q = true;
        }
        #endregion

        #region ����֡
        public void FreezeFrames(int frameCount)
        {
            StartCoroutine(DoFreezeFrames(frameCount));
        }

        private IEnumerator DoFreezeFrames(int frameCount)
        {
            // 1. ������Ƶ���� Time.timeScale Ӱ��
            foreach (var audioSource in audioSources)
            {
                audioSource.ignoreListenerPause = true; // ��ֹ��Ƶ��ͣ
            }

            // 2. ������Ϸ�߼�
            Time.timeScale = 0f;

            // 3. ʹ�� unscaledDeltaTime �ȴ������� Time.timeScale Ӱ�죩
            float realTimeToWait = frameCount * Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(realTimeToWait);

            // 4. �ָ���Ϸ
            Time.timeScale = 1f;

            // 5. �ָ���Ƶ���ã���ѡ��
            foreach (var audioSource in audioSources)
            {
                audioSource.ignoreListenerPause = false;
            }
        }
        #endregion

        #region ���˽ӿ�
        public void TakeDamager_NoSound(float damager, string hitAnimation, Transform attacker)
        {
            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            //��Ѫ

            //����ڳ��״̬�£�����Ѫ
            if (CurrentState.name.Contains("Evade") | isInvincible | iswudi)
            {
                //�������������ʱ����
                if (isInvincible)
                {
                    // ����ʱ�����Ч��
                    StartCoroutine(SlowMotionEffect());
                }
            }
            //�����Ѫ
            else
            {
                health -= 20;
                Debug.Log("��ұ�����");
                BackLastState("Hit");
            }


            //GameObjectPoolSystem.Instance.TakeGameObject("damagarNumericalValue", damagarNumericalValueTransform).GetComponent<damagarNumericalValue>().Create(damagar * UnityEngine.Random.Range(1, 2), mainCamera);

            //BOSSѪ��Ϊ0 ����
            //if (dataSO.health <= 0)
            //{
            //    BackLastState("Die");
            //}
        }


        public void TakeDamager(float damagar, string hitAnimationName, Transform attacker, AudioClip audioClip, GameObject gameObject)
        {

            StatesDictionary["Hit"].String = hitAnimationName;
            StatesDictionary["Hit"].Clip = audioClip;
            BackLastState("Hit");
            if (health <= 0)
            {
                BackLastState("Die");
                Controller.enabled = false;
            }
        }
        #endregion

        #region ��⹥���ж�����Χ���༭����
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5);
        }
        #endregion
    }
}


