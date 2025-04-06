using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Enemy_Dle", menuName = "StateMachine/State/Enemy/Enemy_Dle")]
    public class Enemy_Dle : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Die;


        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {


        }

        public override void OnEnter()
        {
            _PlayableDirector.Play(Die);
            _PlayableDirector.extrapolationMode = isLoop;

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
            Debug.Log("���ˣ�");


        }
    }
}