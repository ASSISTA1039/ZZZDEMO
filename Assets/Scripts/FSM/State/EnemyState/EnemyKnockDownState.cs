using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Hit")]
public class EnemyKnockDownState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Knockdown;


    //timeline�������ʱ���õķ���
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }
    public override void OnEnter()
    {
        _PlayableDirector.Play(Knockdown);
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
        Debug.Log("������");

    }


    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"KnockDown", Knockdown},
        };
        foreach (var kvp in base.Copy())
        {
            data[kvp.Key] = kvp.Value;
        }

        return data;
    }
    public override void Paste(Dictionary<string, object> data)
    {
        base.Paste(data);
        Knockdown = (PlayableAsset)data["KnockDown"];

    }

}
