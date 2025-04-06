using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Idle")]
public class EnemyIdleState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Idle;
    [SerializeField] protected float rangeDistance = 5f;
    
    private bool isWaiting = false;  // ����Ƿ����ڵȴ�
    private int DisapperCharge_Stage2_time = 0;

    public override void OnEnter()
    {
        FacePlayer();

        _PlayableDirector.Play(Idle);
        _PlayableDirector.extrapolationMode = isLoop;
        if (_StateMachineSystem == null)
        {
            InitState(_StateMachineSystem);
        }
        // �����ӳ���Ϊ�жϵ�Э��
        _StateMachineSystem.StartCoroutine(WaitBeforeDecision());
    }

    private IEnumerator WaitBeforeDecision()
    {
        isWaiting = true; // ���Ϊ�ȴ�״̬
        yield return new WaitForSeconds(2f); // �ȴ� 3.5 ��
        isWaiting = false; // �����ȴ�
    }

    public override void OnUpdate()
    {
        //���BOSS�ڱ���������������˴�������10������Ͷ����ָ���ִ������A����һ�׶Σ�Ȼ�����˴�����0
        if (_StateMachineSystem.hitCount >= 30 && _StateMachineSystem.dataSO.health >= 0)
        {
            _StateMachineSystem.hitCount = 0;
            string DisapperAnimation = "DisapperCharge_Stage1";

            _StateMachineSystem.StatesDictionary["DisapperCharge"].String = DisapperAnimation;
            _StateMachineSystem.BackLastState("DisapperCharge");
        }
        
        if (isWaiting) return; // ������ڵȴ�����ִ����Ϊ�ж�
        if (_StateMachineSystem.RefreshToughen) return; // �������ˢ��Ӳֱ����ִ����Ϊ�ж�
        Debug.Log("û���ֵ��ˣ�����");
        Debug.Log(_StateMachineSystem);
        //���BOSS��Ѫ����һ�ε���50%,�ͷ�����ִ������A����2�׶�
        if (DisapperCharge_Stage2_time <= 4 && isFirstStage && _StateMachineSystem.dataSO.health <= 0.5f * _StateMachineSystem.dataSO.maxHealth)
        {
            //time++,֪��Ϊ3ʱ��false
            _StateMachineSystem.fog.SetActive(true);
            DisapperCharge_Stage2_time += 1;
            
            if (DisapperCharge_Stage2_time <=3)
            {
                string DisapperAnimation = "DisapperCharge_Stage2";

                _StateMachineSystem.StatesDictionary["DisapperCharge"].String = DisapperAnimation;
                _StateMachineSystem.BackLastState("DisapperCharge");
                _StateMachineSystem.StartCoroutine(WaitBeforeDecision());
            }
            else if(DisapperCharge_Stage2_time == 4)
            {
                _StateMachineSystem.fog.SetActive(false);
                _StateMachineSystem.body.SetActive(true);
                isFirstStage = false;
            }


        }
        else
        {

            if (_StateMachineSystem.GetCurrentTarget() && _StateMachineSystem.GetCurrentTargetDistance() > rangeDistance)
            {

                string[] animations = {"RangeCombat_FlyPounce","Walk"};
                string RangeCombatAnimation = animations[Random.Range(0, animations.Length)];
                if (RangeCombatAnimation.Equals("RangeCombat_FlyPounce"))
                {
                    _StateMachineSystem.StatesDictionary["RangeCombat"].String = RangeCombatAnimation;
                    _StateMachineSystem.BackLastState("RangeCombat");
                }
                else
                {
                    _StateMachineSystem.StatesDictionary["Walk"].String = RangeCombatAnimation;
                    _StateMachineSystem.BackLastState("Walk");
                }
            }
            else if (_StateMachineSystem.GetCurrentTarget() && _StateMachineSystem.GetCurrentTargetDistance() <= rangeDistance)
            {
                //����BOSSѪ���ж���ƽA3�λ���ƽA4��
                //BOSSҲ�п���ִ��תȦȦ
                if (_StateMachineSystem.dataSO.health <= 0.5f * _StateMachineSystem.dataSO.maxHealth)
                {
                    string[] animations = { "RangeCombat_Circle", "CombatB" };
                    string CombatAnimation = animations[Random.Range(0, animations.Length)];
                    if (CombatAnimation.Equals("RangeCombat_Circle") || CombatAnimation.Equals("RangeCombat_FlyPounce"))
                    {
                        _StateMachineSystem.StatesDictionary["RangeCombat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("RangeCombat");
                    }
                    else
                    {
                        _StateMachineSystem.StatesDictionary["Combat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("Combat");
                    }
                }
                else
                {
                    string[] animations = { "RangeCombat_Circle", "CombatA"};
                    string CombatAnimation = animations[Random.Range(0, animations.Length)];

                    if (CombatAnimation.Equals("RangeCombat_Circle"))
                    {
                        _StateMachineSystem.StatesDictionary["RangeCombat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("RangeCombat");
                    }
                    else
                    {
                        _StateMachineSystem.StatesDictionary["Combat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("Combat");
                    }
                }
            }
        }
    }

    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"Idle", Idle},
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
        Idle = (PlayableAsset)data["Idle"];
    }
}
