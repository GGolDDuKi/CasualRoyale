using Google.Protobuf.Protocol;
using System.Collections;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    #region Fields

    [SerializeField] VirtualJoystick _moveJoystick;

    private bool canDash = true;
    private bool canAttack = true;

    #endregion

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _moveJoystick = GameObject.Find("MoveJoystick").GetComponent<VirtualJoystick>();
    }

    protected override void Update()
    {
        UpdateMove();
        UpdateAnimation();
    }

    protected override void UpdateMove()
    {
        if (!(State == ActionState.Idle || State == ActionState.Run))
            return;

        if (_moveJoystick._handleDir == Vector2.zero)
        {
            State = ActionState.Idle;
            CheckUpdatedFlag();
            return;
        }

        _destPos = (Vector2)transform.position;

        if(_moveJoystick._handleDir != Vector2.zero)
        {
            State = ActionState.Run;
            _destPos += _moveJoystick._handleDir * 1.5f * Time.deltaTime;
            LastDir = _moveJoystick._handleDir;
        }

        UpdateDir(_destPos);
        transform.position = _destPos;
        Pos = _destPos;

        SetLayer(_sprite);

        CheckUpdatedFlag();
    }

    protected override IEnumerator CoAttack()
    {
        if (!(State == ActionState.Idle || State == ActionState.Run))
            yield break;

        if (canAttack == false || LastDir == Vector2.zero)
            yield break;

        canAttack = false;
        float time = 0;

        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{_directionY.ToString()}Attack")
                time = anim.length;
        }

        State = ActionState.Attack;

        CH_Attack attackPacket = new CH_Attack();
        attackPacket.AttackType = AttackType.Normal;
        Managers.Network.H_Send(attackPacket);

        yield return new WaitForSeconds(time);
        
        State = ActionState.Idle;
        CheckUpdatedFlag();
        StartCoroutine(CoAttackCooldown());
    }

    public IEnumerator CoDash()
    {
        if (!(State == ActionState.Idle || State == ActionState.Run))
            yield break;

        if ((_moveJoystick._handleDir == Vector2.zero && LastDir == Vector2.zero) || canDash == false)
            yield break;

        canDash = false;
        Vector2 dir;

        if (_moveJoystick._handleDir == Vector2.zero)
            dir = LastDir;
        else
            dir = _moveJoystick._handleDir;

        State = ActionState.Dash;
        float dashTime = 0.5f;

        while(dashTime > 0)
        {
            _destPos = (Vector2)transform.position;
            _destPos += dir * 10f * Time.deltaTime;
            transform.position = _destPos;
            Pos = _destPos;
            CheckUpdatedFlag();
            dashTime -= Time.deltaTime;
            yield return null;
        }

        State = ActionState.Idle;
        CheckUpdatedFlag();
        StartCoroutine(CoDashCooldown());
    }

    protected override IEnumerator CoDie(HC_Die diePacket)
    {
        Camera.main.transform.SetParent(null);
        Managers.Game.Rank = diePacket.Rank;
        Managers.Object.RemovePlayer(this.gameObject);

        GameObject go = Managers.UI.GenerateUI("UI/GameEnd");

        if (diePacket.Rank == 1)
            go.GetComponent<GameEnd>().Exit();
        else
            go.GetComponent<GameEnd>().Monitor();

        State = ActionState.Dead;
        yield return new WaitForSeconds(3.0f);

        GameObject controlUI = GameObject.Find("ControlUI");
        controlUI.SetActive(false);

        Clear();
    }

    public void EndGame()
    {
        GameObject go = Managers.UI.GenerateUI("UI/GameEnd");
        go.GetComponent<GameEnd>().Exit();
    }

    IEnumerator CoAttackCooldown(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }

    IEnumerator CoDashCooldown(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
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
