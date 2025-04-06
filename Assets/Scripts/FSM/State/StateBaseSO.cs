using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Assista.FSM
{
    public class StateBaseSO : ScriptableObject
    {
        protected AnimancerComponent _Animancer;
        protected Transform _Player;
        public float _TargetSpeed;
        protected PlayableDirector _PlayableDirector;
        protected CharacterController _CharacterController;
        protected StateMachineBase _StateMachineSystem;
        [SerializeField] protected DirectorWrapMode isLoop;


        //��¼��ɫ�ĵ�ǰѪ��
        public float currentHealth;

        //������
        public virtual string String { get; set; }
        public virtual AudioClip Clip { get; set; }
        public virtual bool isDelay_E { get; set; }
        public virtual bool isDelay_F { get; set; }
        public virtual float SetTime { get; set; }
        protected Vector3 movementDirection;
        //��ʼ����״̬����
        public void InitState(StateMachineBase stateMachineSystem)
        {
            _Animancer = stateMachineSystem.GetComponent<AnimancerComponent>();
            _PlayableDirector = stateMachineSystem.GetComponent<PlayableDirector>();
            _CharacterController = stateMachineSystem.GetComponent<CharacterController>();
            _Player = stateMachineSystem.transform.GetComponent<Transform>();
            _StateMachineSystem = stateMachineSystem;
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }
        
        public float GetTargetSpeed()
        {
            return _TargetSpeed;
        }
        //�鷽��������������������д
        public virtual void OnEnter() {
            AddInputActionCallBacks();
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }
        //���󷽷����������������б����ṩ�÷����ľ���ʵ��
        public virtual void OnUpdate() 
        {
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                //��ɫ�ڵ��棬�����벻Ϊ�㣬�ҿ����ƶ��������ƶ��ٶ�
                movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                //��¼���������ת�Ƕ�
                float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

                _Player.rotation = Quaternion.Slerp(_Player.rotation, Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0), Time.deltaTime * 8f);
                //_Player.rotation = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

            }
        }
        public virtual void OnExit() {
            RemoveInputActionCallBacks();
        }

        //�޲������캯��
        //public StateBaseSO() { }

        #region ����ص�
        protected void AddInputActionCallBacks()
        {
            //��ɫwalkί��  
            if (CharacterInputSystem.Instance == null || CharacterInputSystem.Instance._playerControls == null)
            {
                //Debug.LogError("CharacterInputSystem or PlayerControls is not initialized!");
                return;
            }

            // ȷ�� Movement ��Ϊ null ��ע��ص�
            var playerInput = CharacterInputSystem.Instance._playerControls.PlayerInput;
            if (playerInput.Movement != null)
            {
                playerInput.Movement.canceled += OnMovementCanceled;
                playerInput.Movement.performed += OnMovementPerformed;
            }
            else
            {
                Debug.LogError("PlayerInput.Movement is not initialized!");
            }
        }



        protected void RemoveInputActionCallBacks()
        {
            CharacterInputSystem.Instance._playerControls.PlayerInput.Movement.canceled -= OnMovementCanceled;
            CharacterInputSystem.Instance._playerControls.PlayerInput.Movement.performed -= OnMovementPerformed;
            //CharacterInputSystem.MainInstance.inputActions.Player.CameraLook.started -= OnMouseMovementStarted;
        }
        #endregion

        #region �����ˮƽ����
        public void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            if (movementInput == Vector2.zero) { return; }
            //�����Ұ�סW��Ҳȡ��ˮƽ����
            if (movementInput == Vector2.up)
            {
                DisableCameraRecentering();
                return;
            }
            //��������Ĵ�ֱ�������þ��е��ٶ�
            //������ҵ�����İ����������Ƿ�������
            //�õ�����Ĵ�ֱ�Ƕȣ����½ǣ�
            float cameraVerticalAngle = Camera.main.transform.localEulerAngles.x;
            //ŷ���Ƿ��صĶ���һ������-90=>270�����Լ�ȥ
            if (cameraVerticalAngle > 270f)
            {
                cameraVerticalAngle -= 360f;
            }
            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

            if (movementInput == Vector2.down)
            {
                SetCameraRecentering(cameraVerticalAngle, _StateMachineSystem.BackWardsCameraRecenteringData);
                return;
            }
            //ִ�е�������Ǵ���ˮƽ����������ˣ�
            SetCameraRecentering(cameraVerticalAngle, _StateMachineSystem.SidewaysCameraRecenteringData);
        }

        protected void SetCameraRecentering(float cameraVerticalAngle, List<PlayerCameraRecenteringData> playerCameraRecenteringDates)
        {

            foreach (PlayerCameraRecenteringData recenteringData in playerCameraRecenteringDates)
            {
                if (!recenteringData.IsWithInAngle(cameraVerticalAngle))
                {
                    //ֱ���˳����Ԫ�أ�������һ��Ԫ��
                    continue;
                }
                //����ڷ�Χ�ڣ�
                EnableCameraRecentering(recenteringData.waitingTime, recenteringData.recenteringTime);
                return;
            }

            //���ѭ������û��ƥ��ķ�Χ�͹ر�ˮƽ����
            DisableCameraRecentering();
        }

        public void EnableCameraRecentering(float waitTime = -1f, float recenteringTime = -1)
        {
            _StateMachineSystem.playerCameraUtility.EnableRecentering(waitTime, recenteringTime);
        }
        public void DisableCameraRecentering()
        {
            _StateMachineSystem.playerCameraUtility.DisableRecentering();
        }
        protected void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //����ֹͣ����þ��У�ˮƽ����ֱ��
            DisableCameraRecentering();
        }
        ////���������������ƶ������ʱ����
        //private void OnMouseMovementStarted(InputAction.CallbackContext context)
        //{
        //    UpdateCameraRecenteringState(GetPlayerMovementInputDirection());
        //}

        protected void OnMovementPerformed(InputAction.CallbackContext context)
        {
            //����ֱ�����¼��õ�������ֵ
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }

        #endregion
    }
}

