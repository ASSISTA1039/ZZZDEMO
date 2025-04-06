using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "EvadeState", menuName = "StateMachine/State/Player/New EvadeState")]
    public class Player_EvadeState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset EvadeFront;
        [SerializeField] protected PlayableAsset EvadeBack;
        [SerializeField] protected PlayableAsset EvadeCombat;
        [SerializeField] protected PlayableAsset EvadeCombat_Normal;
        [SerializeField] protected PlayableAsset EvadeAir;

        [SerializeField] public float groundCheckDistance = 0.1f;
        [SerializeField] public LayerMask groundLayer;

        private Vector3 movementDirection;
        private bool CanEvadeCombat;

        public Player_MoveState MoveState;

        //��ҹ�������ʱ
        public float tempattack;

        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().isTransportJUSTNOW = false;
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");
            
        }

        public override void OnEnter()
        {
            if (CheckGrounded())
            {
                if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                {
                    _PlayableDirector.Play(EvadeFront);
                    _PlayableDirector.extrapolationMode = isLoop;
                    MoveState.isRun = true;
                }
                else
                {
                    _PlayableDirector.Play(EvadeBack);
                    _PlayableDirector.extrapolationMode = isLoop;
                }
            }
            else
            {
                _PlayableDirector.Play(EvadeAir);
                //_PlayableDirector.extrapolationMode = isLoop;
            }
            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
            CanEvadeCombat = true;


        }

        public override void OnExit()
        {
            //MoveState.isRun = false;
            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            _StateMachineSystem.SeekTheEnemy();
            //_StateMachineSystem.BackLastState(CombatState);

            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon && _PlayableDirector.playableAsset == EvadeFront)
            {
                //��ɫ�ڵ��棬�����벻Ϊ�㣬�ҿ����ƶ��������ƶ��ٶ�
                movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                //��¼���������ת�Ƕ�
                float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;
                
                _Player.rotation = Quaternion.Slerp(_Player.rotation, Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0), Time.deltaTime * 8f);
                //_Player.rotation = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

            }
            if(CheckGrounded())
            {
                if (CharacterInputSystem.Instance.playerLAtk && CanEvadeCombat)
                {
                    Debug.Log(_StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().bossCurrentColor);
                    //�����ɫ������������״̬+���˱����Ϊ��ɫ������Q���ܽ����ۻ�״̬��
                    if ((_StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().isTransportJUSTNOW
                        && _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().bossCurrentColor.Equals(Color.red))
                        || _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().isQ_FireState)
                    {
                        _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().energy += 
                            0.2f * _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().MaxEnergy;
                        _PlayableDirector.Play(EvadeCombat);
                    }
                    else
                    {
                        _PlayableDirector.Play(EvadeCombat_Normal);
                    }
                    _StateMachineSystem.gameObject.GetComponent<PlayerStateMachine>().isTransportJUSTNOW = false;
                    _PlayableDirector.extrapolationMode = isLoop;
                    CanEvadeCombat = false;
                }
            }

        }

        private bool CheckGrounded()
        {
            return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
    }
}
