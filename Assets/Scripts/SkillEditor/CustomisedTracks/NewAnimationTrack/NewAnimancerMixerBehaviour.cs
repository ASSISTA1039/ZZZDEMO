using System;
using UnityEngine;
using Animancer;
using UnityEngine.Playables;

public class NewAnimancerMixerBehaviour : PlayableBehaviour
{

    public AnimancerComponent _Animancer;
    //public ClipTransition _AnimationClip;
    private float _time;


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //��ͣ����
        base.OnBehaviourPause(playable, info);
        Debug.Log("1111111111111");
        
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //��ʼ����
        base.OnBehaviourPlay(playable, info);
        Debug.Log("11333333333333331");
    }

    public override void OnGraphStart(Playable playable)
    {
        //��ʼ����
        base.OnGraphStart(playable);
        Debug.Log("1444444444444444441");
    }

    public override void OnGraphStop(Playable playable)
    {
        //��ͣ����
        base.OnGraphStop(playable);
        Debug.Log("55555555555555555551");
    }

    public override void OnPlayableCreate(Playable playable)
    {
        //����timeline�༭����
        base.OnPlayableCreate(playable);
        Debug.Log("16666666666666666611");
        
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        //�˳�timeline�༭����
        base.OnPlayableDestroy(playable);
        Debug.Log("77777777777777777777");
        Debug.Log(_Animancer);
        _time = (float)playable.GetTime();
        if (_Animancer.States.Current != null)
        {
            _Animancer.States.Current.Time = _time;
        }

    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        _Animancer = playerData as AnimancerComponent;
        //��ȡ�������clip������
        int inputCount = playable.GetInputCount();

        for(int i = 1; i < inputCount; i++)
        {
            //ScriptPlayable<>
        }


        //�������ÿִ֡��
        base.ProcessFrame(playable, info, playerData);
        Debug.Log("8888888888888888888888");
    }





}
