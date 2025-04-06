using Assista.SkillEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ProBuilder.Shapes;
public class EnemyWalkState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Walk;
    [SerializeField] protected PlayableAsset Walk_Clone;

    private bool door = true; //������·�������沥�ŵ��滻�ж�

    private float distanceChangeCachedTime = 0.05f; //����״̬

    //timeline�������ʱ���õķ���
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        door = !door;
    }
    public override void OnEnter()
    {
        if (_PlayableDirector != null)
        {
            //����״̬ʱע���¼�
            _PlayableDirector.stopped += OnTimelineFinished;
        }
    }
    public override void OnUpdate()
    {
        if (_StateMachineSystem.GetCurrentTarget() && _StateMachineSystem.GetCurrentTargetDistance() > 2f)
        {
            RotateTowardsPlayer();
            AlternatePlayPlayableAsset(Walk, Walk_Clone);
            distanceChangeCachedTime = 0.05f;

        }
        else
        {
            distanceChangeCachedTime -= Time.deltaTime;
            if (distanceChangeCachedTime < 0)
            {
                _StateMachineSystem.BackLastState("Idle");
            }

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

    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"Walk", Walk},
            {"Walk_Clone", Walk_Clone},
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
        Walk = (PlayableAsset)data["Walk"];
        Walk_Clone = (PlayableAsset)data["Walk_Clone"];
        Debug.Log(Walk);
    }
    float currentVelocity = 0f;
    protected void RotateTowardsPlayer()
    {
        // ȷ����Ҷ������
        if (!_StateMachineSystem.GetCurrentTarget()) return;
        // ����Ŀ�귽�����λ�� - ����λ��
        Vector3 targetDirection = _StateMachineSystem.GetCurrentTarget().position - _BOSS.position;
        targetDirection.y = 0; // ���� Y �ᣬ��ֹ�����ڴ�ֱ������ת

        // ����Ŀ��Ƕ�
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        // ƽ�����ɵ�Ŀ��Ƕ�
        float smoothedAngle = Mathf.SmoothDampAngle(_BOSS.eulerAngles.y, targetAngle, ref currentVelocity, 0.1f);

        // Ӧ����ת
        _BOSS.rotation = Quaternion.Lerp(_BOSS.rotation, Quaternion.Euler(0, smoothedAngle, 0), Time.deltaTime * 50);
    }


    //���沥������һ����timeline
    private void AlternatePlayPlayableAsset(PlayableAsset asset1, PlayableAsset asset2)
    {
        if (_PlayableDirector.playableAsset != asset1 && door)
        {
            _PlayableDirector.Play(asset1);
            _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

        }
        if (_PlayableDirector.playableAsset != asset2 && !door)
        {
            _PlayableDirector.Play(asset2);
            _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

        }
    }
}
