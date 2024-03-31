using Google.Protobuf.Protocol;
using System.Collections;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    #region Fields

    [SerializeField] VirtualJoystick _moveJoystick;
    [SerializeField] VirtualJoystick _aimJoystick;

    private bool canDash = true;

    #endregion

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _moveJoystick = GameObject.Find("MoveJoystick").GetComponent<VirtualJoystick>();
        _aimJoystick = GameObject.Find("AimJoystick").GetComponent<VirtualJoystick>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAttack();
    }

    //LookDir(ĳ���Ͱ� ���� ����) ������Ʈ�ϴ� �Լ�
    //Aim ���̽�ƽ�� Move ���̽�ƽ���� �켱���� ����
    protected override void UpdateCharacterDir()
    {
        if (_aimJoystick._handleDir == Vector2.zero && _moveJoystick._handleDir == Vector2.zero)
        {
            LookDir = Vector2.zero;
            WeaponPos = GetWeaponPos();
            CheckUpdatedFlag();
            return;
        }

        if (_aimJoystick._handleDir == Vector2.zero)
        {
            UpdateDirX(_moveJoystick._handleDir);
            UpdateObjRotation(_head, _moveJoystick._handleDir);
            UpdateObjRotation(_hand.transform, _moveJoystick._handleDir);
            LookDir = _moveJoystick._handleDir;
            WeaponPos = GetWeaponPos();
        }
        else
        {
            UpdateDirX(_aimJoystick._handleDir);
            UpdateObjRotation(_head, _aimJoystick._handleDir);
            UpdateObjRotation(_hand.transform, _aimJoystick._handleDir);
            LookDir = _aimJoystick._handleDir;
            WeaponPos = GetWeaponPos();
        }

        CheckUpdatedFlag();
    }

    //ĳ���� �̵��� ������Ʈ�ϴ� �Լ�
    protected override void UpdateMove()
    {
        if (State == ActionState.Dash)
            return;

        if (_moveJoystick._handleDir == Vector2.zero)
        {
            State = ActionState.Idle;
            _head.rotation = Quaternion.Euler(0, 0, 0);
            _hand.rotation = Quaternion.Euler(0, 0, 0);
            CheckUpdatedFlag();
            return;
        }

        _destPos = transform.position;

        if(_moveJoystick._handleDir != Vector2.zero)
        {
            State = ActionState.Run;
            _destPos += _moveJoystick._handleDir * 1.5f * Time.deltaTime;
        }

        transform.position = _destPos;
        Pos = _destPos;
        CheckUpdatedFlag();
    }

    void UpdateAttack()
    {
        if (_aimJoystick._handleDir == Vector2.zero)
            return;

        StartCoroutine(_weapon.SendShootPacket());
    }

    //ĳ���� �뽬�� ������Ʈ�ϴ� �Լ�
    public IEnumerator CoDash()
    {
        if (_moveJoystick._handleDir == Vector2.zero || canDash == false)
            yield break;

        canDash = false;
        Vector2 dir = _moveJoystick._handleDir;

        State = ActionState.Dash;
        Vector2 destPos = transform.position + (new Vector3(dir.x, dir.y, 0) * 5);

        while((destPos - (Vector2)transform.position).magnitude > 10f * Time.deltaTime)
        {
            _destPos = transform.position;
            _destPos += dir * 10f * Time.deltaTime;
            transform.position = _destPos;
            Pos = _destPos;
            //CheckUpdatedFlag();
            yield return null;
        }

        _destPos = destPos;
        transform.position = _destPos;
        Pos = _destPos;
        State = ActionState.Idle;

        StartCoroutine(DashCooldown());
        CheckUpdatedFlag();
    }

    public IEnumerator Reload(float reloadSpeed)
    {
        WeaponState lastState = WeaponState;
        WeaponState = WeaponState.Reload;

        //���� �ִϸ��̼� ������ ���
        //�ٸ� ��� ���ϸ� ���� ��ҵǰ� �ұ�?

        yield return new WaitForSeconds(reloadSpeed);

        _weapon.CurBullets = _weapon.MaxBullets;
        WeaponState = lastState;
    }

    //���� ��ġ�� ���ϴ� �Լ�
    public Vector2 GetWeaponPos()
    {
        return _weapon.transform.position;
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(3.0f);
        canDash = true;
    }

    void CheckUpdatedFlag()
    {
        if (_updated)
        {
            CH_Move movePacket = new CH_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.H_Send(movePacket);
            _updated = false;
        }
    }
}
