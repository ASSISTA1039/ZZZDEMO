using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "StateMachine/State/Player/New IdleState")]
    public class Player_IdleState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Idle;
        [SerializeField, Header("��������CD")] public float time;
        private float _time;
        private bool StartTheTimer = false;
        
        [SerializeField] protected PlayableAsset[] IdleAnimations;

        //public StateBaseSO EvadeState;

        //public StateBaseSO MoveState;
        //public StateBaseSO CombatState;


        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            StartTheTimer = true;

            _PlayableDirector.Play(Idle);
            _PlayableDirector.extrapolationMode = isLoop;

        }

        public override void OnEnter()
        {
            base.OnEnter();
            _StateMachineSystem.iswudi = false;

            _PlayableDirector.Play(Idle);
            _PlayableDirector.extrapolationMode = isLoop;
            _time = time;

            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            StartTheTimer = true;
            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            if (StartTheTimer)
            {
                _time -= Time.deltaTime;

            }
            if(_time < 0 && IdleAnimations.Length != 0)
            {
                playableAssetPlay(IdleAnimations[Random.Range(0, IdleAnimations.Length)]);
                StartTheTimer = false;
                _time = time;
            }
            //�㰴��shift��������Ҽ�
            if(CharacterInputSystem.Instance.playerSlide || CharacterInputSystem.Instance.playerDefen)
            {
                _StateMachineSystem.BackLastState("Evade");
            }
            if (CharacterInputSystem.Instance.playerJump)
            {
                _StateMachineSystem.BackLastState("Jump");
            }
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                _StateMachineSystem.BackLastState("Move");
            }

            if (CharacterInputSystem.Instance.playerLAtk)
            {
                Debug.Log(CharacterInputSystem.Instance.playerLAtk);
                _StateMachineSystem.BackLastState("Combat");
            }
            if (CharacterInputSystem.Instance.Combat_E && _StateMachineSystem.energy >= 20f)
            {
                _StateMachineSystem.energy -= 20f;
                _StateMachineSystem.enegySlider.fillAmount = _StateMachineSystem.energy / _StateMachineSystem.MaxEnergy;
                _StateMachineSystem.BackLastState("Combat_E");
            }
            if (CharacterInputSystem.Instance.Combat_F)
            {
                _StateMachineSystem.energy -= 20f;
                _StateMachineSystem.enegySlider.fillAmount = _StateMachineSystem.energy / _StateMachineSystem.MaxEnergy;
                _StateMachineSystem.BackLastState("Combat_F");
            }
            if (CharacterInputSystem.Instance.Combat_Q && _StateMachineSystem.energy >= 50f)
            {
                if (!_StateMachineSystem.gameObject.name.Equals("S"))
                {
                    _StateMachineSystem.energy -= 50f;
                    _StateMachineSystem.enegySlider.fillAmount = _StateMachineSystem.energy / _StateMachineSystem.MaxEnergy;
                }
                _StateMachineSystem.BackLastState("Combat_Q");
            }
            //if (_StateMachineSystem.GetComponent<PlayerStateMachine>().health < currentHealth)
            //{
            //    _StateMachineSystem.BackLastState("Hit");
            //}
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }

        private void playableAssetPlay(PlayableAsset playableAsset)
        {
            _PlayableDirector.Play(playableAsset);
            _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

        }



    }
}

