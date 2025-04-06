using System.Collections;
using System.Collections.Generic;
using Assista.FSM;
using Animancer;
using UnityEngine;
using UnityEngine.Playables;

public abstract class EnemyStateBase : MonoBehaviour
{
    protected AnimancerComponent _Animancer;
    protected Transform _BOSS;

    protected PlayableDirector _PlayableDirector;
    protected CharacterController _CharacterController;
    protected EnemyStateMachine _StateMachineSystem;

    [SerializeField] protected DirectorWrapMode isLoop;
    public bool isFirstStage = true;
    //������
    public virtual string String { get; set; }
    public virtual AudioClip Clip { get; set; }

    //��ʼ����״̬����
    public void InitState(EnemyStateMachine stateMachineSystem)
    {
        _Animancer = stateMachineSystem.GetComponent<AnimancerComponent>();
        _PlayableDirector = stateMachineSystem.GetComponent<PlayableDirector>();
        _BOSS = stateMachineSystem.transform.parent.GetComponent<Transform>();
        _CharacterController = _BOSS.GetComponent<CharacterController>();
        _StateMachineSystem = stateMachineSystem;

    }

    public virtual Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            { "isLoop", isLoop},
        };
        return data;
    }

    public virtual void Paste(Dictionary<string, object> data)
    {
        isLoop = (DirectorWrapMode)data["isLoop"];

    }

    //�鷽��������������������д
    public virtual void OnEnter() { }
    //���󷽷����������������б����ṩ�÷����ľ���ʵ��
    public abstract void OnUpdate();
    public virtual void OnExit() { }

    protected void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �ҵ����
        if (player != null)
        {
            Vector3 direction = player.transform.position - _BOSS.position; // ���㳯������
            direction.y = 0; // ֻ��ˮƽ����ת����ֹ BOSS ��ת
            if (direction != Vector3.zero)
            {
                _BOSS.rotation = Quaternion.LookRotation(direction); // �� BOSS �������
            }
        }
    }
}
