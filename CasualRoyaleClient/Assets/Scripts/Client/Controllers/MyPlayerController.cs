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

    //LookDir(캐릭터가 보는 방향) 업데이트하는 함수
    //Aim 조이스틱이 Move 조이스틱보다 우선권이 높음
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

    //캐릭터 이동을 업데이트하는 함수
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

    //캐릭터 대쉬를 업데이트하는 함수
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

        //장전 애니메이션 있으면 재생
        //다른 모션 취하면 장전 취소되게 할까?

        yield return new WaitForSeconds(reloadSpeed);

        _weapon.CurBullets = _weapon.MaxBullets;
        WeaponState = lastState;
    }

    //총의 위치를 구하는 함수
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
