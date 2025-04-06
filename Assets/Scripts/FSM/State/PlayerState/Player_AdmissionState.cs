using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "AdmissionState", menuName = "StateMachine/State/Player/New AdmissionState")]
    public class Player_AdmissionState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Admission;

        //[SerializeField, Header("���˼��")] private float detectionRang;
        //[SerializeField] protected LayerMask enemyLayer;

        //�����⵽�ĵ���Ŀ��
        //private Collider[] detectionedTarget = new Collider[1];


        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");

        }
        public override void OnEnter()
        {
            _PlayableDirector.Play(Admission);
            _PlayableDirector.extrapolationMode = isLoop;

            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            _StateMachineSystem.currentTarget = null;

            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }

        }

        public override void OnUpdate()
        {
            /*//�㰴��shift��������Ҽ�
            if (_InputSystem.playerSlide || _InputSystem.playerDefen)
            {
                _StateMachineSystem.BackLastState("Evade");
            }
            if (_InputSystem.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                _StateMachineSystem.BackLastState("Move");
            }
            if (_InputSystem.playerLAtk)
            {
                _StateMachineSystem.BackLastState("Combat");
            }
            if (_InputSystem.Combat_E)
            {
                _StateMachineSystem.BackLastState("Combat_E");
            }
            if (_InputSystem.Combat_Q)
            {
                _StateMachineSystem.BackLastState("Combat_Q");
            }*/


        }



    }
}

