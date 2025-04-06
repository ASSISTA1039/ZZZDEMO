using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Enemy_Hit", menuName = "StateMachine/State/Enemy/Enemy_Hit")]
    public class Enemy_Hit : StateBaseSO
    {
        //[SerializeField] protected PlayableAsset Idle;
        [SerializeField] protected PlayableAsset Hit_D_Up;
        [SerializeField] protected PlayableAsset Hit_Right_Left;
        [SerializeField] protected PlayableAsset Hit_Left_Right;
        [SerializeField] protected PlayableAsset Hit_Inplace;

        //[SerializeField] protected StateBaseSO IdleState;

        //public ExposedReference<AudioSource> _AudioSource;
        //public AudioSource _AudioSource1;



        public AudioClip HitAudio => Clip;

        [HideInInspector] public string HitName => String;

        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");
        }

        public override void OnEnter()
        {
            switch (HitName)
            {
                case "Hit_D_Up":
                    _PlayableDirector.Play(Hit_D_Up);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Right_Left":
                    _PlayableDirector.Play(Hit_Right_Left);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Left_Right":
                    _PlayableDirector.Play(Hit_Left_Right);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Inplace":
                    _PlayableDirector.Play(Hit_Inplace);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                default:
                    Debug.Log("û�ҵ����˶���");
                    break;

            }
            _StateMachineSystem.AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
            _StateMachineSystem.AudioSourcesDictionary["Masa_FX"].Play();

            //AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
            //AudioSourcesDictionary["Masa_FX"].Play();

            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }






        }

        public override void OnExit()
        {
            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            //Debug.Log("������");


        }


    }
}

