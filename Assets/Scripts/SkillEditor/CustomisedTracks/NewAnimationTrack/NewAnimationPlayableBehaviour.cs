using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.Playables;

public class NewAnimationPlayableBehaviour : PlayableBehaviour
{
    public AnimancerComponent _Animancer;
    public ClipTransition _AnimationClip;
    private float _time;

    public override void OnGraphStart(Playable playable)
    {
        //Debug.Log("������ͼ�ο�ʼ����ʱ����");
        if (_Animancer.States.Current != null)
        {
            //_Animancer.States.Current.Time = _time;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        //Debug.Log("��ͣ");

        //_Animancer = null;

        if (_Animancer && _Animancer.States.Current != null)
        {
            //_Animancer.States.Current.Time = _time;
        }

    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("ʱ�������ClipƬ��");
        //��ǰ����״̬Ϊ�ջ��ߵ�ǰ���ŵĶ�����_AnimationClip.Clip��ͬʱ������_AnimationClip
        if (_Animancer.States.Current == null || _Animancer.States.Current.Clip != _AnimationClip.Clip)
        {
            //_Animancer.Playable.Speed = 1f;
            //_AnimationClip.Speed = 1f;
            
            _Animancer.Play(_AnimationClip);
        }
        if (_Animancer.States.Current != null)
        {
            _Animancer.States.Current.Time = _time;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //Debug.Log("ʱ�����뿪ClipƬ��");
        
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if (Application.isEditor)
        {
            base.PrepareFrame(playable, info);
            _Animancer.States.Current.Speed = 0f;
            _time = (float)playable.GetTime();
            _Animancer.States.Current.MoveTime(_time, false);
            //�༭����ʱԤ������λ��
            //OnValidate();

        }
    }

    public override void OnPlayableCreate(Playable playable)
    {
        //Debug.Log("����Ƭ�δ���ʱ����" + playable.GetDuration());
        
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        //Debug.Log("����Ƭ��ɾ��ʱ����" + playable.GetDuration());
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

    }

    public override void PrepareData(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
    }

    private void OnValidate()
    {
        //AnimancerUtilities.EditModeSampleAnimation(_AnimationClip.Clip, _Animancer, _time * _AnimationClip.Clip.length);
        //_AnimationClip.Clip.EditModeSampleAnimation(_Animancer, _time * _AnimationClip.Clip.length);
        _AnimationClip.Clip.EditModeSampleAnimation(_Animancer, _time);
    }


}