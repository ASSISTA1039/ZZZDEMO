using System.Collections;
using System.Collections.Generic;
using Assista.FSM;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.SkillEditor
{
    public class AttackDetectionPlayableBehaviour : PlayableBehaviour
    {
        public PlayerStateMachine stateMachine;
        //public CharacterInputSystem InputSystem;
        public CharacterMoveMentControllerBase MoveController;
        private Vector3 movementDirection;
        public Transform _Player;
        //����
        //public Transform currentTarget;

        public Transform attackDetectionCenter;
        //public GameObject gameObject;
        public float attackDetectionRang;
        public LayerMask enemyLayer;
        public GameObject Player;
        public DetectionType DetectionType;
        public DetectionShape DetectionShape;
        public Vector3 CubeSize;
        public Vector3 Position;
        public DrawGizmos gizmos;

        public string HitAnimationName;
        public GameObject HitVFX;
        public AudioClip[] HitAudios;

        public bool AttackAdsorption;
        private float t = 0.0f;
        //ת��ʱ�䣨���ڵ���ת���ٶȣ�
        public float RotationTime;

        //�������⹥������ص�����ײ�壬�����4��
        Collider[] attackDetectionTargets = new Collider[4];

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //Debug.Log("ʱ�������ClipƬ��");

            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    //gizmos��ͼ������
                    gizmos.transform.localPosition = Position;
                    switch (DetectionShape)
                    {
                        case DetectionShape.spherical:
                            gizmos.DetectionShape = DetectionShape.spherical;
                            break;
                        case DetectionShape.rectangle:
                            gizmos.DetectionShape = DetectionShape.rectangle;
                            break;
                    }
                    //Debug.Log(AttackDetectionType());
                    //�������
                    //Debug.Log("��⵽����ײ��ĸ���:" + AttackDetectionType());
                    if (AttackDetectionType() > 0)
                    {
                        for (int i = 0; i < AttackDetectionType(); i++)
                        {
                            //����ײ������������������г��Ի�ȡ�ӿ�
                            if(enemyLayer.Equals("Player"))
                            {
                                if (attackDetectionTargets[i].GetComponent<PlayerStateMachine>().TryGetComponent(out IDamagar damagar))
                                {
                                    //Debug.Log("����:" + attackDetectionTargets[i].GetComponentInChildren<PlayerStateMachine>());
                                    //Debug.Log("���˽ӿ�:" + damagar);

                                    //damagar.TakeDamager(500f, HitAnimationName, Player.transform, HitAudios[Random.Range(0, HitAudios.Length)], HitVFX);
                                    damagar.TakeDamager_NoSound(500f, HitAnimationName, Player.transform.parent);
                                }
                            }
                            else
                            {
                                if (attackDetectionTargets[i].GetComponentInChildren<EnemyStateMachine>().TryGetComponent(out IDamagar damagar))
                                {
                                    //Debug.Log("����:" + attackDetectionTargets[i].GetComponentInChildren<PlayerStateMachine>());
                                    //Debug.Log("���˽ӿ�:" + damagar);

                                    //damagar.TakeDamager(500f, HitAnimationName, Player.transform, HitAudios[Random.Range(0, HitAudios.Length)], HitVFX);
                                    damagar.TakeDamager_NoSound(20f, HitAnimationName, Player.transform);
                                }
                            }

                        }
                    }

                    break;
            }

        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    gizmos.enabled = true;
                    gizmos.detectionRang = attackDetectionRang;
                    gizmos.CubeSize = CubeSize;

                    break;
                case DetectionType.SearchEnemies:

                    float deltaTime = Time.deltaTime;
                    t += deltaTime / RotationTime;
                    //��ֹt����1
                    t = Mathf.Clamp01(t);
                    if (stateMachine.currentTarget == null)
                    {
                        if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                        {
                            //��ɫ�ڵ��棬�����벻Ϊ�㣬�ҿ����ƶ��������ƶ��ٶ�
                            movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                            movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                            //��¼���������ת�Ƕ�
                            float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

                            Quaternion quaternion = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

                            _Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, t);

                        }
                    }
                    else
                    {
                        EnemyStateMachine enemystate = Player.GetComponent<EnemyStateMachine>();
                        Debug.Log(enemystate);
                        Debug.Log(Player);

                        if (enemystate != null)
                        {
                            Debug.Log("here is player");
                            _Player.parent.rotation = _Player.parent.LockOnTarget(Player.GetComponent<EnemyStateMachine>().currentTarget, _Player.parent.transform, 50f);
                        }
                        else
                        {
                            _Player.rotation = _Player.LockOnTarget(stateMachine.currentTarget, _Player.transform, 50f);
                        }
                    }

                    if ((stateMachine.currentTarget != null) && AttackAdsorption)
                    {
                        //Vector3 targetDirection;
                        //ȷ������
                        //targetDirection = currentTarget.position - transform.root.position;

                        {
                            Vector3 targetDirection = stateMachine.currentTarget.position - _Player.position;
                            MoveController.CharacterMoveInterface(new Vector3(stateMachine.currentTarget.position.x - _Player.position.x, 0, stateMachine.currentTarget.position.z - _Player.position.z), 30f, true);
                        }
                    }


                    
                    break;
            }   


        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Debug.Log("ʱ�����뿪ClipƬ��");
            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    gizmos.enabled = false;
                    attackDetectionCenter.localPosition = new Vector3(0, 0, 0);
                    gizmos.transform.position = attackDetectionCenter.position;

                    break;
            }

        }


        /// <summary>
        /// ���ؼ�⵽����ײ��ĸ���
        /// </summary>
        /// <returns></returns>
        private int AttackDetectionType()
        {
            switch (DetectionShape)
            {
                case DetectionShape.spherical:
                    return Physics.OverlapSphereNonAlloc(gizmos.transform.position, attackDetectionRang, attackDetectionTargets, enemyLayer);

                case DetectionShape.rectangle:
                    return Physics.OverlapBoxNonAlloc(gizmos.transform.position, CubeSize, attackDetectionTargets, gizmos.transform.rotation, enemyLayer);

                default:
                    return 0;
            }
        }





    }
}

