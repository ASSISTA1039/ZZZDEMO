using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "MoveState", menuName = "StateMachine/State/Player/New MoveState")]
    public class Player_MoveState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Move;
        [SerializeField] protected PlayableAsset Move_Clone;
        [SerializeField] protected PlayableAsset Run;
        [SerializeField] protected PlayableAsset Run_Clone;
  
        [SerializeField] protected PlayableAsset TurnBack;
        [SerializeField] private float playbackSpeed = 1.5f;
        //public StateBaseSO RunEndState;
        //public StateBaseSO EvadeState;

        private bool door = true;
        private bool isTurnBack;
        private bool isTurnBackJustNow = false;
        [HideInInspector] public  bool isRun = false;
        private float angle;
        private float playerMovementInputTime = 0.05f;

        Quaternion quaternion;

        private Vector3 movementDirection;

        public float targetAngle;
        public float rotationTime;

        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            door = !door;
            isTurnBack = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _StateMachineSystem.iswudi = false;
            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
            isRun = false;
            if(_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {

            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                if (isRun)
                {
                    _TargetSpeed = 6f;
                    if (!isTurnBack) { CharacterRotation(CharacterInputSystem.Instance.playerMovement); }
                    PlayerTurnBack();
                    AlternatePlayPlayableAsset(Run, Run_Clone);
                    if (_PlayableDirector.playableGraph.IsValid())
                        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.8f * playbackSpeed);
                }
                else
                {
                    _TargetSpeed = 4f;
                    AlternatePlayPlayableAsset(Move, Move_Clone);
                    if(_PlayableDirector.playableGraph.IsValid())
                        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
                }
                //���ƽ�ɫ�Ĺ�ת
                //CharacterRotation(CharacterInputSystem.Instance.playerMovement);
                //�㰴��shift��������Ҽ�
                if (CharacterInputSystem.Instance.playerSlide || CharacterInputSystem.Instance.playerDefen)
                {
                    _StateMachineSystem.BackLastState("Evade");
                }

                if (!isTurnBack){ CharacterRotation(CharacterInputSystem.Instance.playerMovement); }

                playerMovementInputTime = 0.05f;

            }
            else
            {
                playerMovementInputTime -= Time.deltaTime;
                if (playerMovementInputTime < 0)
                {
                    _StateMachineSystem.BackLastState("RunEnd");
                }
                
            }
            if (CharacterInputSystem.Instance.Combat_Q)
            {
                _StateMachineSystem.BackLastState("Combat_Q");
            }
            if (CharacterInputSystem.Instance.playerJump)
            {
                _StateMachineSystem.BackLastState("Jump");
            }
            if (CharacterInputSystem.Instance.playerLAtk)
            {
                _StateMachineSystem.BackLastState("Combat");
            }
            if (CharacterInputSystem.Instance.Combat_E)
            {
                _StateMachineSystem.BackLastState("Combat_E");
            }
            if (CharacterInputSystem.Instance.Combat_F)
            {
                _StateMachineSystem.BackLastState("Combat_F");
            }
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }

        float currentVelocity = 0;
        protected void CharacterRotation(Vector2 movementDirection)
        {
            // ���û��������룬����ת
            if (movementDirection == Vector2.zero) { return; }

            // ����Ŀ��Ƕȣ���������� Y ����ת
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            // ƽ�����ɵ�Ŀ��Ƕ�
            float smoothedAngle = Mathf.SmoothDampAngle(_Player.eulerAngles.y, targetAngle, ref currentVelocity, rotationTime);

            // Ӧ����ת
            _Player.rotation = Quaternion.Lerp(_Player.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 20);
        }

        private void PlayerTurnBack()
        {
            ////��ɫת����
            ////Time.timeScale = 0.3f;
            //Vector3 vector = new Vector3();
            //vector.x = CharacterInputSystem.Instance.playerMovement.x;
            //vector.z = CharacterInputSystem.Instance.playerMovement.y;
            //angle = Vector3.Angle(_Player.forward, (Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * vector).normalized);

            ////����ɫforward�������ƶ�Ŀ�귽��Ƕȴ���120ʱ����ת����
            //if (angle > 120)
            //{
            //    if (_PlayableDirector.playableAsset != TurnBack)
            //    {
            //        isTurnBack = true;
            //        _PlayableDirector.Play(TurnBack);
            //        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
            //        _PlayableDirector.extrapolationMode = DirectorWrapMode.None;
            //        isTurnBackJustNow = true;

            //    }

            //}

        }

        private void PlayerTurn()
        {
            ////��ɫ�ڵ��棬�����벻Ϊ�㣬�ҿ����ƶ��������ƶ��ٶ�
            //movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
            //movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
            ////��¼���������ת�Ƕ�
            //float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

            //quaternion = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);
            //if (!isTurnBackJustNow)
            //{
            //    //_Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, Time.deltaTime * 8f);
            //}
            //else
            //{
            //    _Player.rotation = quaternion;
            //   isTurnBackJustNow = false;
            //}

        }

        private void PlayerTurn1()
        {
            // ȷ�����뷽��Ϊ��
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude < Mathf.Epsilon)
                return;

            // 1. ��ȡ������뷽�򣨽���������תΪ3D��
            movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
            movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;

            // 2. ����������Ƕȣ������뷽��ת������������ϵ
            float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 worldDirection = Quaternion.Euler(0, angleOfRotaion, 0) * movementDirection;

            // 3. ����Ŀ�곯�����Ԫ��
            quaternion = Quaternion.LookRotation(worldDirection, _Player.up);

            // 4. ƽ��ת�򣨵�����ת�ٶȣ�0.3f ����ƽ��Ч����
            _Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, Time.deltaTime * 8f);
        }


        //���沥������һ����timeline
        private void AlternatePlayPlayableAsset(PlayableAsset asset1, PlayableAsset asset2)
        {
            if (_PlayableDirector.playableAsset != asset1 && door && !isTurnBack)
            {
                _PlayableDirector.Play(asset1);
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

            }
            if (_PlayableDirector.playableAsset != asset2 && !door && !isTurnBack)
            {
                _PlayableDirector.Play(asset2);
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

            }
        }




    }
}

