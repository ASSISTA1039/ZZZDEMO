using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_RangeCombat", menuName = "StateMachine/State/Enemy/New Enemy_RangeCombat")]
public class EnemyRangeCombatState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset RangeCombat_Circle;
    [SerializeField] protected PlayableAsset RangeCombat_FlyPounce;

    public AudioClip RangeCombatAudio => Clip;
    [HideInInspector] public string RangeCombatName => String;

    //timeline�������ʱ���õķ���
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }
    public override void OnEnter()
    {
        FacePlayer();
        switch (RangeCombatName)
        {
            case "RangeCombat_Circle":
                _PlayableDirector.Play(RangeCombat_Circle);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            case "RangeCombat_FlyPounce":
                _PlayableDirector.Play(RangeCombat_FlyPounce);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            default:
                Debug.Log("û�ҵ�Զ�̶���");
                break;

        }
        //_StateMachineSystem.AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
        //_StateMachineSystem.AudioSourcesDictionary["Masa_FX"].Play();

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
        //MoveTowardsPlayer(); // ÿ֡����ʱ��������ƶ�
        _StateMachineSystem.SeekThePlayer();
        FacePlayer();
    }

    // �� BOSS ��������ƶ�
    private void MoveTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // ��ȡ���
        if (player == null) return;

        Vector3 direction = (player.transform.position - _BOSS.position).normalized; // ���㷽��
        float distance = Vector3.Distance(_BOSS.position, player.transform.position); // �������

        float minDistance = 3.5f; // ������С��ȫ���룬��ֹ BOSS ����
        float moveSpeed = 4f;   // �趨�ƶ��ٶ�

        if (distance > minDistance)
        {
            _CharacterController.Move(direction * moveSpeed * Time.deltaTime); // �� BOSS �ƶ�
        }
    }



    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"RangeCombat_Circle", RangeCombat_Circle},
            {"RangeCombat_FlyPounce", RangeCombat_FlyPounce},
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
        RangeCombat_Circle = (PlayableAsset)data["RangeCombat_Circle"];
        RangeCombat_FlyPounce = (PlayableAsset)data["RangeCombat_FlyPounce"];

    }

}
