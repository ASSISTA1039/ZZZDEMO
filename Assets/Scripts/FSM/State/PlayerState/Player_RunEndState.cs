using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "RunEndState", menuName = "StateMachine/State/Player/New RunEndState")]
    public class Player_RunEndState : StateBaseSO
    {
        [SerializeField] private PlayableAsset RunEnd;

        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }


        public override void OnUpdate()
        {
            if (CharacterInputSystem.Instance.playerJump)
            {
                _StateMachineSystem.BackLastState("Jump");
            }
            if (_PlayableDirector.playableAsset != RunEnd)
            {
                _PlayableDirector.Play(RunEnd);
                _PlayableDirector.extrapolationMode = isLoop;
            }
            if (CharacterInputSystem.Instance.Combat_Q)
            {
                _StateMachineSystem.BackLastState("Combat_Q");
            }
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                _StateMachineSystem.BackLastState("Move");
            }
            if (CharacterInputSystem.Instance.playerLAtk)
            {
                _StateMachineSystem.BackLastState("Combat");
            }
            //�㰴��shift��������Ҽ�
            if (CharacterInputSystem.Instance.playerSlide || CharacterInputSystem.Instance.playerDefen)
            {
                _StateMachineSystem.BackLastState("Evade");
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
    }
}