using Assista.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using static SwitchCharacter;

public class PlayerMoveController : CharacterMoveMentControllerBase
{
    public Transform _Player;
    private float finalSpeed;
    [Header("Acceleration/Deceleration")]
    public float acceleration = 5f;
    public float deceleration = 7f;
    public float targetSpeed = 0f;
    private Vector3 moveDirection;
    //private Vector3 currentVelocity = Vector3.zero;

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public SwicthCharacterInfo character;
    //玩家状态机
    public PlayerStateMachine _StateMachine;


    //控制角色运动状态下的公转
    public new Transform camera;
    

    private void Awake()
    {
    }
    protected override void Start()
    {
        base.Start();
        _StateMachine = GetComponent<PlayerStateMachine>();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnAnimatorMove()
    {
        if (_StateMachine.canAnimMotion)
        {
            if (!CanAnimationMotion())
            {
                if (!_PlayableDirector.playableAsset.name.Contains("Jump"))
                {
                    //Debug.Log(_PlayableDirector.playableAsset.name);
                    inertia = _Animancer.Animator.deltaPosition;
                    _Controller.Move(_Animancer.Animator.deltaPosition + Time.deltaTime * new Vector3(0.0f, verticalSpeed, 0.0f));


                }
                else
                {
                    moveDirection.x = CharacterInputSystem.Instance.playerMovement.x;
                    moveDirection.z = CharacterInputSystem.Instance.playerMovement.y;

                    _Controller.Move((0.5f * inertia) + Time.deltaTime * new Vector3(0f, verticalSpeed, 0f));
                }
            }
        }
    }

    public override void AirNotAttack()
    {
        base.AirNotAttack();
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
