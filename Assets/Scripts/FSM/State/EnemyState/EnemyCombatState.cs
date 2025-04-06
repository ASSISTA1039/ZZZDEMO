using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Combat", menuName = "StateMachine/State/Enemy/New Enemy_Combat")]
public class EnemyCombatState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset CombatA;
    [SerializeField] protected PlayableAsset CombatB;

    public AudioClip CombatAudio => Clip;
    [HideInInspector] public string CombatName => String;

    //timeline�������ʱ���õķ���
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }
    public override void OnEnter()
    {
        FacePlayer();
        switch (CombatName)
        {
            case "CombatA":
                _PlayableDirector.Play(CombatA);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            case "CombatB":
                _PlayableDirector.Play(CombatB);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            default:
                Debug.Log("û�ҵ���ս����");
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
    }

    // �� BOSS ��������ƶ�
    //private void MoveTowardsPlayer()
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player"); // ��ȡ���
    //    if (player == null) return;

    //    Vector3 direction = (player.transform.position - _BOSS.position).normalized; // ���㷽��
    //    float distance = Vector3.Distance(_BOSS.position, player.transform.position); // �������

    //    float minDistance = 3f; // ������С��ȫ���룬��ֹ BOSS ����
    //    float moveSpeed = 5f;   // �趨�ƶ��ٶ�

    //    if (distance > minDistance)
    //    {
    //        _CharacterController.Move(direction * moveSpeed * Time.deltaTime); // �� BOSS �ƶ�
    //    }
    //}



    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"CombatA", CombatA},
            {"CombatB", CombatB},
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
        CombatA = (PlayableAsset)data["CombatA"];
        CombatB = (PlayableAsset)data["CombatB"];

    }

}
