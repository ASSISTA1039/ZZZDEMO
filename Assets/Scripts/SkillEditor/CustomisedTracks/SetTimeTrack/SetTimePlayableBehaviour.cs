using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using UnityEngine.Playables;
using Assista.FSM;
using static UnityEngine.UI.GridLayoutGroup;

namespace Assista.SkillEditor
{
    public class SetTimePlayableBehaviour : PlayableBehaviour
    {
        public AnimancerComponent Animancer;
        //public CharacterInputSystem InputSystem;
        public PlayableDirector Director;
        public inputSystemEnum inputEnum;

        public SkipType SkipType;
        public RotationType RotationType;

        public StateMachineBase StateMachine;

        private Vector3 movementDirection;
        public Transform _Player;
        public float SetAtkTime;
        public float SetTime;
        public bool isEditor;

        private float t = 0.0f;
        //ת��ʱ�䣨���ڵ���ת���ٶȣ�
        public float RotationTime;


        public override void PrepareFrame(Playable playable, FrameData info)
        {
            //if (isEditor) Director.time = SetTime;

            float deltaTime = Time.deltaTime;
            t += deltaTime / RotationTime;
            //��ֹt����1
            t = Mathf.Clamp01(t);

            switch (SkipType)
            {
                case SkipType.SkipTime:
                    SkipTimeInputType(playable);

                    break;
                case SkipType.SkipState:
                    SkipStateInputType(playable);

                    break;
                case SkipType.PlayerRotation:
                    PlayerRotationType();

                    break;
            }

            base.PrepareFrame(playable, info);

        }

        private void SkipTimeInputType(Playable playable)
        {
            switch (inputEnum)
            {
                case inputSystemEnum.Latk:
                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop(); Director.time = SetTime; Director.Evaluate(); Director.Play();
                    }
                    break;
                case inputSystemEnum.LatkLong:

                    if (CharacterInputSystem.Instance.playerLAtkLong)
                    {
                        Director.Stop(); 
                        Director.time = SetTime; Director.Evaluate(); Director.Play();
                    }
                    break;
                case inputSystemEnum.Latk_Air:
                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop(); Director.time = SetTime; Director.Evaluate(); Director.Play();
                    }
                    break;
            }
        }


        private void PlayerRotationType()
        {
            switch (RotationType)
            {
                case RotationType.MoveRotation:
                    if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                    {
                        //��ɫ�ڵ��棬�����벻Ϊ�㣬�ҿ����ƶ��������ƶ��ٶ�
                        movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                        movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                        //��¼���������ת�Ƕ�
                        float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

                        Quaternion quaternion = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

                        _Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, t);
                    
                    }

                    break;
                case RotationType.AinmationRotation:

                    _Player.rotation *= Animancer.Animator.deltaRotation;
                    //_Player.rotation = Quaternion.Slerp(_Player.rotation, _Player.rotation * Quaternion.Euler(0, 90, 0), 0.5f);
                    break;



            }


        
        }



        private void SkipStateInputType(Playable playable)
        {
            switch (inputEnum)
            {
                case inputSystemEnum.Move://������ҡ�����ڼ�
                    //�ƶ�
                    if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Move");
                    }

                    break;
                case inputSystemEnum.Evade:
                    //�㰴��shift��������Ҽ�����
                    if (CharacterInputSystem.Instance.playerSlide || CharacterInputSystem.Instance.playerDefen)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Evade");
                    }
                    break;
                case inputSystemEnum.None:
                    //ֱ�������޲���ת����
                    if(playable.GetDuration() - playable.GetTime() <= 0.05f)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Idle");
                    }

                    break;
                case inputSystemEnum.Latk:
                
                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat");
                    }

                    break;
                case inputSystemEnum.LatkLong:

                    if (CharacterInputSystem.Instance.playerLAtkLong && StateMachine)//�жϴ�ʱ����Ƿ��������١�2��������
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat");
                    }
                    break;
                case inputSystemEnum.SetAtk:

                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop();
                        //��ת��ָ��ʱ��
                        StateMachine.StatesDictionary["Combat"].SetTime = SetAtkTime;
                        StateMachine.BackLastState("Combat");
                    }

                    break;
                case inputSystemEnum.Latk_Air:

                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("AirCombat");
                    }

                    break;
                case inputSystemEnum.SetAtk_Air:

                    if (CharacterInputSystem.Instance.playerLAtk)
                    {
                        Director.Stop();
                        //��ת��ָ��ʱ��
                        StateMachine.StatesDictionary["AirCombat"].SetTime = SetAtkTime;
                        StateMachine.BackLastState("AirCombat");
                    }

                    break;
                case inputSystemEnum.E_Long:

                    /*if (CharacterInputSystem.Instance.Combat_E)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_E");
                    }*/

                    break;
                case inputSystemEnum.E:

                    if (CharacterInputSystem.Instance.Combat_E)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_E");
                    }

                    break;
                case inputSystemEnum.NoDelay_E:

                    if (CharacterInputSystem.Instance.Combat_E)
                    {
                        Director.Stop();
                        //��ǰҡE
                        StateMachine.StatesDictionary["Combat_E"].isDelay_E = false;
                        StateMachine.BackLastState("Combat_E");
                    }

                    break;
                case inputSystemEnum.F:

                    if (CharacterInputSystem.Instance.Combat_F)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_F");
                    }

                    break;
                case inputSystemEnum.F_Long:

                    if (CharacterInputSystem.Instance.Combat_F_Long)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_F");
                    }

                    break;                
                case inputSystemEnum.Q_Long:

                    if (CharacterInputSystem.Instance.Combat_Q_Long)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_Q");
                    }

                    break;
                case inputSystemEnum.Q:

                    if (CharacterInputSystem.Instance.Combat_Q)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Combat_Q");
                    }

                    break;
                case inputSystemEnum.Jump:

                    if (CharacterInputSystem.Instance.playerJump)
                    {
                        Director.Stop();
                        StateMachine.BackLastState("Jump");
                    }

                    break;
            }

        }
    }

}
