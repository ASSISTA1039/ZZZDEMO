
using UnityEngine;
using UnityEngine.InputSystem;
using Assista.UnitTools;
using System.Collections;


public class CharacterInputSystem : SingletonBase<CharacterInputSystem>
{
    //public Button button;
    public PlayerControls _playerControls;
    //����
    public Vector2 playerMovement
    {
        //WASD
        get => _playerControls.PlayerInput.Movement.ReadValue<Vector2>();
        
    }

    public float CameraDistance
    {
        //������
        get => Mouse.current.scroll.y.ReadValue();

    }

    public Vector2 cameraLook
    {
        //����ƶ�
        get => _playerControls.PlayerInput.CameraLook.ReadValue<Vector2>();

    }

    public bool playerRun
    {
        //???
        get => _playerControls.PlayerInput.Run.phase == InputActionPhase.Performed;

    }

    private bool _playerDefen;
    public bool playerDefen
    {
        //�㰴����Ҽ�
        get => _playerControls.PlayerInput.Defen.triggered || _playerDefen;
    }
    public void Set_playerDefen()
    {
        _playerDefen = true;
        //����Э�̣�һ��ʱ����޸�_Combat_E��ֵ
        StartCoroutine(_playerDefenCoroutine());
    }
    //Э��
    IEnumerator _playerDefenCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _playerDefen = false;
    }

    //public bool playerDefenLong
    //{
    //    //��������Ҽ�
    //    get => _playerControls.PlayerInput.DefenLong.phase == InputActionPhase.Performed;
    //}

    private bool _playerJump;
    public bool playerJump
    {
        //�㰴�ո�
        get => _playerControls.PlayerInput.Jump.triggered || _playerJump;

    }
    public void SetplayerJump()
    {
        _playerJump = true;
        //StartCoroutine(playerJumpCoroutine());
    }

    //Э��
    IEnumerator playerJumpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _playerJump = false;
    }

    //public bool playerWeaponAnimation
    //{
    //    //�㰴R
    //    get => _playerControls.PlayerInput.WeaponAnimation.triggered;

    //}

    public bool Combat_E_Long
    {
        get => _playerControls.PlayerInput.Combat_E_Long.phase == InputActionPhase.Performed;
    }

    private bool _Combat_E;
    public bool Combat_E
    {
        //�㰴E
        get => _playerControls.PlayerInput.Combat_E.triggered || _Combat_E;

    }
    public void SetCombat_E()
    {
        _Combat_E = true;
        //����Э�̣�һ��ʱ����޸�_Combat_E��ֵ
        StartCoroutine(Combat_ECoroutine());
    }
    //Э��
    IEnumerator Combat_ECoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_E = false;
    }

    private float combatFHoldTime = 0f; // ��������ʱ��
    private float requiredHoldTime = 0.5f; // ��������ʱ��

    public bool Combat_F_Long
    {
        get => _playerControls.PlayerInput.Combat_F_Long.phase == InputActionPhase.Performed;
    }

    //public bool Combat_F_Long
    //{
    //    get => combatFHoldTime >= requiredHoldTime;
    //}

    private bool _Combat_F;
    public bool Combat_F
    {
        //�㰴F
        get => _playerControls.PlayerInput.Combat_F.triggered || _Combat_F;

    }
    public void SetCombat_F()
    {
        _Combat_F = true;
        //����Э�̣�һ��ʱ����޸�_Combat_F��ֵ
        StartCoroutine(Combat_FCoroutine());
    }
    //Э��
    IEnumerator Combat_FCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_F = false;
    }

    private bool _Combat_Q;
    public bool Combat_Q
    {
        //�㰴E
        get => _playerControls.PlayerInput.Combat_Q.triggered || _Combat_Q;

    }

    public bool Combat_Q_Long
    {
        get => _playerControls.PlayerInput.Combat_Q_Long.phase == InputActionPhase.Performed;
    }

    public void SetCombat_Q()
    {
        _Combat_Q = true;
        //����Э�̣�һ��ʱ����޸�_Combat_E��ֵ
        StartCoroutine(Combat_QCoroutine());
    }
    //Э��
    IEnumerator Combat_QCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _Combat_Q = false;
    }


    public int playerSwitchType
    {
        //�������1 2 3 4
        get => (int)_playerControls.PlayerInput.SwitchCharacter.ReadValue<float>();

    }

    public bool playerSlide
    {
        //�㰴��shift
        get => _playerControls.PlayerInput.Slide.triggered;

    }

    //public bool LeftAltLong
    //{
    //    //������alt
    //    get => _playerControls.PlayerInput.Left_Alt_Long.phase == InputActionPhase.Performed;
    //}


    public bool playerLAtkLong
    {
        get => _playerControls.PlayerInput.LAtk_Long.phase == InputActionPhase.Performed;
    }


    private bool _playerLAtk;
    private bool disablePlayerLAtk;
    public bool playerLAtk
    {
        //�㰴������
        get => (_playerControls.PlayerInput.LAtk.triggered ) || _playerLAtk;
        //&& disablePlayerLAtk
    }
    /// <summary>
    /// ��������������
    /// </summary>
    public void DisablePlayerLAtk() => disablePlayerLAtk = false;
    /// <summary>
    /// ��������������
    /// </summary>
    public void EnablePlayerLAtk() => disablePlayerLAtk = true;


    public void SetPlayerLAtk()
    {
        _playerLAtk = true;
        //����Э�̣�һ��ʱ����޸�_Combat_E��ֵ
        StartCoroutine(PlayerLAtkCoroutine());
    }
    //Э��
    IEnumerator PlayerLAtkCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _playerLAtk = false;
    }



    //�ڲ�����
    private void Awake()
    {
        if (_playerControls == null)
            _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void OnDisable()
    {
        _playerControls.Disable();
    }
    private void Update()
    {
        if (_playerControls.PlayerInput.Combat_F.ReadValue<float>() > 0) // �������������
        {
            combatFHoldTime += Time.deltaTime; // �ۼӳ���ʱ��
        }
        else
        {
            combatFHoldTime = 0f; // �����ɿ�ʱ���ü�ʱ
        }
    }
}
