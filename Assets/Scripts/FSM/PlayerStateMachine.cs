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

        //E/Q技能生成“时空节点”以及返回最近的“时空节点”
        public SphereManager sphereManager;
        //private Queue<GameObject> activeSpheres = new Queue<GameObject>(); // 已经激活的球体
        //public  GameObjectPoolSystem GameObjectPool;


        //判断敌人是否攻击了
        public bool isEnemyAttacked = false;
        //设置完美闪避无敌帧的开关；
        public bool isInvincible=false;
        //设置无敌期间的破盾伤害更高
        public bool isRecordForPODUN = false;
        //完美闪避时间内的碰撞体检测范围增大！
        public float evade_Radius=2f;

        //----------------------------------------
        //为事件提供判断是否可以通过动画rootmotion移动
        public bool canAnimMotion = true;

        //角色切换管理器
        public SwitchCharacter switchCharacter;


        public GameObject BOSS;
        private bool canTeleportInvincible;
        
        //记录boss目前是否处于“异常”状态
        public Color bossCurrentColor;

        //记录角色是否处于完美特殊技状态
        public bool isQ_FireState = false;
        public bool isQ_OVERLOADING_FireState = false;
        //记录角色是否刚刚瞬移到敌人背后
        public bool isTransportJUSTNOW = false;

        // 需要不受 Time.timeScale 影响的音频
        public AudioSource[] audioSources; 


        public void Awake()
        {
            //初始化血量
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
                    //设置角色材质颜色恢复为常态颜色
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

            //检测是否有敌人对你造成了伤害
            if (isEnemyAttacked)
            {
                //如果在冲刺状态下，不扣血
                if (CurrentState.name.Contains("Evade")| isInvincible | iswudi)
                {
                    //如果在完美闪避时间内
                    if(isInvincible)
                    {
                        //canTeleportInvincible = true;
                        // 触发时间减缓效果
                        StartCoroutine(SlowMotionEffect());
                    }
                }
                //否则扣血
                else
                {
                    health -= 20;
                    
                }
            }
            isEnemyAttacked = false;
        }


        #region 设置无敌状态
        public void SetWudi(bool wudi)
        {
            iswudi = wudi;
        }
        #endregion

        #region 设置BOSS时停（动画播放或暂停）
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

        #region 设置将BOSS砸向地面
        public void PushBOSS()
        {
            //StartCoroutine(PushBackCoroutine());
            BOSS.GetComponent<EnemyStateMachine>().BOSS被砸到地面 = true;
        }
        #endregion

        #region 设置玩家结束了完美闪避可以高额破盾的时间
        public void SetRecordForPODUN()
        {
            isRecordForPODUN = false;
        }
        #endregion

        #region 完美闪避以及特殊角色完美闪避的瞬移
        //完美闪避无敌帧触发判定
        public void SetInvincibility(bool state)
        {
            isInvincible = state;
            StartCoroutine(EndInvincibility());
            Debug.Log(state ? "无敌状态开启" : "无敌状态关闭");
        }

        public void TeleportInvincible()
        {

            if (canTeleportInvincible)
            {
                // 瞬移到BOSS背后
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
            Time.timeScale = 0.2f; // 慢动作效果
            //角色S触发完美闪避，并且敌人被标记为红色的“熔火”状态时，可以触发“瞬移到距离最近的，符合条件的敌人背后”
            if (gameObject.name == "S" && bossCurrentColor.Equals(Color.red))
            {
                body.SetActive(false);
                isTransportJUSTNOW = true;
                canAnimMotion = false;
                StartCoroutine(LongPressWarp());
            }
            yield return new WaitForSecondsRealtime(0.5f); // 慢动作持续时间
            Time.timeScale = 1f; // 恢复正常时间
            isRecordForPODUN = false;
        }
        #endregion

        #region 强制切换至跳跃状态
        public void SwitchToJumpState()
        {
            BackLastState("Jump");
        }
        #endregion

        #region 设置角色当前材质颜色（BUFF状态下），设置角色Q技能提供的60%熔火能量，同时标记角色当前为熔火状态
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
            //设置角色Q技能提供的60%熔火能量
            energy += 0.6f * MaxEnergy;
            if (energy > MaxEnergy)
            {
                energy = MaxEnergy;
            }
            BOSS.GetComponent<EnemyStateMachine>().exceptionTime = 0f;
            isQ_FireState = true;
        }
        #endregion

        //#region 设置角色Q技能提供的60%熔火能量,同时标记角色当前为熔火状态
        //public void SetPlayerQEnergyANDState()
        //{
        //    //设置角色Q技能提供的60%熔火能量
        //    energy += 0.6f * MaxEnergy;
        //    if(energy > MaxEnergy)
        //    {
        //        energy = MaxEnergy;
        //    }
        //    isQ_FireState = true;
        //}
        //#endregion
        //跳跃状态的位移攻击

        #region F技能（地面上长按F键 / 瞬移到BOSS头顶 / 从BOSS头顶返回地面)
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

        #region E技能 传送瞬移并制造时空节点 
        public void Attack_CombatE()
        {
            canAnimMotion = false;
            body.SetActive(false);
            StartCoroutine(Teleportation());
        }
        #endregion

        #region 瞬移到不同位置的各种协程
        private IEnumerator LongPressWarp()
        {
            if (BOSSTarget)
            {
                // 获取BOSS的朝向和位置
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS的朝向
                Vector3 bossRight = bossTransform.right; // BOSS的右侧方向

                // 根据朝向计算传送点
                float forwardOffset = -2f; // 前方的偏移量
                float verticalOffset = bossPosition.y; // 垂直方向偏移量
                float lateralOffset = 0f; // 横向偏移量，可自行调整

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // 调整Y轴高度
                // 使用 DOMove 方法移动角色，持续 0.1 秒
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(() => {
                    Vector3 targetDirection = (BOSSTarget.transform.position).normalized;
                    targetDirection.y = 0f;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;
                }); // 等待完成

            }
        }

        private IEnumerator UpWarp()
        {
            if (BOSSTarget)
            {
                //// 获取BOSS位置并调整角色偏移
                //Vector3 bossPosition = BOSSTarget.transform.position;
                //Vector3 newPosition = new Vector3(
                //    bossPosition.x,
                //    bossPosition.y + boxCombatF_DistanceUpDown, // 添加Y轴偏移
                //    bossPosition.z
                //);
                // 获取BOSS的朝向和位置
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS的朝向
                Vector3 bossRight = bossTransform.right; // BOSS的右侧方向

                // 根据朝向计算传送点
                float forwardOffset = 1f; // 前方的偏移量
                float verticalOffset = bossTransform.position.y + boxCombatF_DistanceUpDown; // 垂直方向偏移量
                float lateralOffset = 0f; // 横向偏移量，可自行调整

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // 调整Y轴高度
                // 使用 DOMove 方法移动角色，持续 0.1 秒
                yield return transform.DOMove(newPosition,0.1f).OnComplete(() => {
                    Vector3 targetDirection = (BOSSTarget.transform.position).normalized;
                    targetDirection.y = 0f;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;
                }); // 等待完成

            }
        }

        private IEnumerator DownWarp()
        {
            if (BOSSTarget)
            {
                // 获取BOSS的朝向和位置
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS的朝向
                Vector3 bossRight = bossTransform.right; // BOSS的右侧方向

                // 根据朝向计算传送点
                float forwardOffset = 1f; // 前方的偏移量
                float verticalOffset = -1.35f; // 垂直方向偏移量
                float lateralOffset = 0f; // 横向偏移量，可自行调整

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // 调整Y轴高度
                // 使用 DOMove 方法移动角色，持续 0.1 秒
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(()=> { body.SetActive(true); canAnimMotion = true; }); // 等待完成
                
                //Vector3 targetDirection = BOSSTarget.transform.position - transform.position;
                //Controller.Move(new Vector3(BOSSTarget.transform.position.x - transform.position.x, BOSSTarget.transform.position.y - transform.position.y + boxCombatF_DistanceUpDown, BOSSTarget.transform.position.z - transform.position.z).normalized * 60f * Time.deltaTime);
            }
        }

        private IEnumerator Teleportation()
        {
            if (BOSSTarget)
            {
                // 获取BOSS的朝向和位置
                Transform bossTransform = BOSSTarget.transform;
                Vector3 bossPosition = bossTransform.position;
                Vector3 bossForward = bossTransform.forward; // BOSS的朝向
                Vector3 bossRight = bossTransform.right; // BOSS的右侧方向

                // 根据朝向计算传送点
                float forwardOffset = 1f; // 前方的偏移量
                float verticalOffset = -1.35f; // 垂直方向偏移量
                float lateralOffset = 0f; // 横向偏移量，可自行调整

                Vector3 newPosition = bossPosition + bossForward * forwardOffset + bossRight * lateralOffset;
                newPosition.y = verticalOffset; // 调整Y轴高度

                // 使用 DOMove 方法移动角色，持续 0.1 秒
                yield return transform.DOMove(newPosition, 0.1f).OnComplete(() =>
                {
                    // 让角色面向 BOSS
                    Vector3 targetDirection = (bossPosition - transform.position).normalized;
                    targetDirection.y = 0f; // 忽略Y轴差异
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                    body.SetActive(true);
                    canAnimMotion = true;

                    // 生成 Sphere
                    sphereManager.SpawnSphere(transform.position);

                    // 激活角色主体
                    body.SetActive(true);
                });
            }
        }
        #endregion

        #region Q技能触发

        public void Attack_CombatQ()
        {
            canAnimMotion = false;
            body.SetActive(false);
            MovePlayerToNearestSphere();
        }

        // 找到最近的球体并将玩家移动过去
        private void MovePlayerToNearestSphere()
        {
            if (sphereManager.activeSpheres.Count == 0)
            {
                Debug.LogWarning("没有可用的球体！");
                canAnimMotion = true;
                body.SetActive(true);
                return;
            }

            GameObject nearestSphere = null;
            float shortestDistance = float.MaxValue;
            //检测是否有BOSS进入“时空节点”
            foreach(var sphere in sphereManager.activeSpheres)
            {
                if (sphere.activeInHierarchy) // 确保球体是激活状态
                {
                    int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
                    if (targetCount > 0)
                    {
                        StartCoroutine(SmoothMove(sphere.transform.position));
                    }
                }
            }

            // 遍历所有激活的球体，找到最近的一个
            foreach (var sphere in sphereManager.activeSpheres)
            {
                if (sphere.activeInHierarchy) // 确保球体是激活状态
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
                // 移动玩家到最近的球体
                StartCoroutine(SmoothMove(nearestSphere.transform.position));
            }
        }

        // 平滑移动协程
        private IEnumerator SmoothMove(Vector3 target)
        {
            // 获取 BOSS 的朝向和位置
            Transform bossTransform = BOSSTarget.transform;
            Vector3 bossPosition = bossTransform.position;

            // 计算从圆心到 BOSS 的矢量
            Vector3 bossToTargetDir = (target - bossPosition).normalized;

            // 计算目标位置，使其距离 BOSS 一定距离
            float desiredDistance = 2.0f; // 你想要的间距
            Vector3 finalPosition = target + bossToTargetDir * desiredDistance;

            yield return transform.DOMove(finalPosition, 0.1f).OnComplete(() =>
            {
                // 让角色面向 BOSS
                Vector3 targetDirection = (bossPosition - transform.position).normalized;
                targetDirection.y = 0f; // 忽略Y轴差异
                transform.rotation = Quaternion.LookRotation(targetDirection);
        
                canAnimMotion = true;
                body.SetActive(true);
            }); // 等待完成
        }
        public void Q_Backward()
        {
            canAnimMotion = false;
            StartCoroutine(MoveBackWard());
        }

        private IEnumerator MoveBackWard()
        {
            float backwardOffset = -5f; // 后方的偏移量
            float verticalOffset = -1.35f; // 垂直方向偏移量
            float lateralOffset = 0f; // 横向偏移量，可自行调整
            Vector3 directionToBoss = (BOSSTarget.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToBoss * backwardOffset + transform.right * lateralOffset;
            newPosition.y = verticalOffset; // 调整Y轴高度

            // 使用 DOMove 方法移动角色，持续 0.1 秒
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
            // 根据朝向计算传送点
            float forwardOffset = 7f; // 前方的偏移量
            float verticalOffset = -1.35f; // 垂直方向偏移量
            float lateralOffset = 0f; // 横向偏移量，可自行调整
            Vector3 directionToBoss = (BOSSTarget.transform.position - transform.position).normalized;

            Vector3 newPosition = transform.position + directionToBoss * forwardOffset + transform.right * lateralOffset;
            newPosition.y = verticalOffset; // 调整Y轴高度

            // 使用 DOMove 方法移动角色，持续 0.1 秒
            yield return transform.DOMove(newPosition, 0.1f).OnComplete(() =>
            {
                canAnimMotion = true;

                // 激活角色主体
                body.SetActive(true);
            });

        }
        #endregion

        #region Q技能切换角色2开大
        public void SetCombat_Q_SwitchCharacter()
        {
            switchCharacter.isCombat_Q = true;
        }
        #endregion

        #region 冻结帧
        public void FreezeFrames(int frameCount)
        {
            StartCoroutine(DoFreezeFrames(frameCount));
        }

        private IEnumerator DoFreezeFrames(int frameCount)
        {
            // 1. 调整音频不受 Time.timeScale 影响
            foreach (var audioSource in audioSources)
            {
                audioSource.ignoreListenerPause = true; // 防止音频暂停
            }

            // 2. 冻结游戏逻辑
            Time.timeScale = 0f;

            // 3. 使用 unscaledDeltaTime 等待（不受 Time.timeScale 影响）
            float realTimeToWait = frameCount * Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(realTimeToWait);

            // 4. 恢复游戏
            Time.timeScale = 1f;

            // 5. 恢复音频设置（可选）
            foreach (var audioSource in audioSources)
            {
                audioSource.ignoreListenerPause = false;
            }
        }
        #endregion

        #region 受伤接口
        public void TakeDamager_NoSound(float damager, string hitAnimation, Transform attacker)
        {
            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            //扣血

            //如果在冲刺状态下，不扣血
            if (CurrentState.name.Contains("Evade") | isInvincible | iswudi)
            {
                //如果在完美闪避时间内
                if (isInvincible)
                {
                    // 触发时间减缓效果
                    StartCoroutine(SlowMotionEffect());
                }
            }
            //否则扣血
            else
            {
                health -= 20;
                Debug.Log("玩家被打了");
                BackLastState("Hit");
            }


            //GameObjectPoolSystem.Instance.TakeGameObject("damagarNumericalValue", damagarNumericalValueTransform).GetComponent<damagarNumericalValue>().Create(damagar * UnityEngine.Random.Range(1, 2), mainCamera);

            //BOSS血量为0 死亡
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

        #region 检测攻击判定发范围（编辑器）
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5);
        }
        #endregion
    }
}


