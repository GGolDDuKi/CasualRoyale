using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureController : BaseController
{
    #region Fields

    public int Id { get; set; }

    public string Name { get; set; }

    public JobType JobType { get; set; }

    #region PositionInfo PosInfo

    PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            State = value.State;
            Pos = new Vector2(value.PosX, value.PosY);
            LastDir = new Vector2(value.LastDirX, value.LastDirY);
        }
    }

    //캐릭터 상태
    public ActionState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            _updated = true;
        }
    }

    //캐릭터 위치
    public Vector2 Pos
    {
        get { return new Vector2(PosInfo.PosX, PosInfo.PosY); }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }

    //캐릭터가 마지막으로 바라본 방향
    public Vector2 LastDir
    {
        get { return new Vector2(PosInfo.LastDirX, PosInfo.LastDirY); }
        set
        {
            if (PosInfo.LastDirX == value.x && PosInfo.LastDirY == value.y)
                return;

            PosInfo.LastDirX = value.x;
            PosInfo.LastDirY = value.y;
            _updated = true;
        }
    }

    #endregion

    #region StatInfo StatInfo

    StatInfo _stat = new StatInfo();
    public virtual StatInfo StatInfo
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }

    public float Hp
    {
        get { return StatInfo.Hp; }
        set
        {
            StatInfo.Hp = value;
        }
    }

    public float MaxHp
    {
        get { return StatInfo.MaxHp; }
        set
        {
            StatInfo.MaxHp = value;
        }
    }

    public float Speed
    {
        get { return StatInfo.Speed; }
        set { StatInfo.Speed = value; }
    }

    #endregion

    protected bool _flipX = false;  //false면 Left, true면 Right
    protected Vector2 _destPos = Vector2.zero;
    protected bool _updated = false;
    protected Animator _animator;
    protected List<AnimationClip> _animationClips = new List<AnimationClip>();
    public HpBar _hpBar;
    public Ghost _ghost;

    public enum DirectionX
    {
        Left,
        Right
    }
    protected DirectionX _directionX ;

    public enum DirectionY
    {
        Front,
        Back
    }
    protected DirectionY _directionY;


    #endregion

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _directionX = DirectionX.Left;
        _directionY = DirectionY.Front;
        _animator = GetComponent<Animator>();
        _animationClips.AddRange(_animator.runtimeAnimatorController.animationClips);
        _hpBar = Managers.UI.GenerateHpBar(this);
        _hpBar.SetHpBar(Hp, MaxHp);
        _ghost = GetComponent<Ghost>();
    }

    protected override void Update()
    {
        UpdateMove();
        UpdateAnimation();
        UpdateHpBar();
    }

    public void Attack()
    {
        StartCoroutine(CoAttack());
    }

    protected virtual IEnumerator CoAttack()
    {
        if (State == ActionState.Dead)
            yield break;

        float time = 0;
        foreach(var anim in _animationClips)
        {
            if(anim.name == $"{_directionY.ToString()}Attack")
                time = anim.length;
        }
        State = ActionState.Attack;
        yield return new WaitForSeconds(time);
        State = ActionState.Idle;
    }

    public void Hit(float hp)
    {
        StartCoroutine(CoHit(hp));
    }

    protected virtual IEnumerator CoHit(float hp)
    {
        if (State == ActionState.Dead)
            yield break;

            float time = 0;
        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{_directionY.ToString()}Hit")
                time = anim.length;
        }
        State = ActionState.Hit;
        Hp = hp;
        _hpBar.SetHpBar(Hp, MaxHp);
        yield return new WaitForSeconds(time);
        
        if(State != ActionState.Dead)
            State = ActionState.Idle;
    }

    public virtual void Die(HC_Die diePacket)
    {
        StartCoroutine(CoDie(diePacket));
    }

    protected virtual IEnumerator CoDie(HC_Die diePacket)
    {
        State = ActionState.Dead;
        yield return new WaitForSeconds(3.0f);
        Clear();
    }

    protected virtual void UpdateMove()
    {
        _destPos = Pos;
        UpdateDir(_destPos);

        switch (State)
        {
            case ActionState.Run:
                if ((_destPos - (Vector2)transform.position).magnitude > 1.5f * Time.deltaTime)
                {
                    transform.position += ((Vector3)_destPos - transform.position).normalized * 1.5f * Time.deltaTime;
                    SetLayer(_sprite);
                }
                else
                {
                    transform.position = _destPos;
                    SetLayer(_sprite);
                }
                break;
            case ActionState.Dash:
                if ((_destPos - (Vector2)transform.position).magnitude > 10.0f * Time.deltaTime)
                {
                    transform.position += ((Vector3)_destPos - transform.position).normalized * 10.0f * Time.deltaTime;
                    SetLayer(_sprite);
                }
                else
                {
                    transform.position = _destPos;
                    SetLayer(_sprite);
                }
                break;
        }
    }

    protected virtual void UpdateAnimation()
    {
        switch (State)
        {
            case ActionState.Idle:
                _animator.Play($"{_directionY.ToString()}Idle");
                break;
            case ActionState.Run:
                _animator.Play($"{_directionY.ToString()}Run");
                break;
            case ActionState.Dash:
                _animator.Play($"{_directionY.ToString()}Run");
                break;
            case ActionState.Dead:
                _animator.Play($"{_directionY.ToString()}Die");
                break;
            case ActionState.Attack:
                _animator.Play($"{_directionY.ToString()}Attack");
                break;
            case ActionState.Hit:
                _animator.Play($"{_directionY.ToString()}Hit");
                break;
        }

        switch (State)
        {
            case ActionState.Dash:
                _ghost._makeGhost = true;
                break;
            default:
                _ghost._makeGhost = false;
                break;
        }
    }

    protected virtual void UpdateHpBar()
    {
        _hpBar.transform.localPosition = Vector3.zero;
    }

    protected virtual void Clear()
    {
        Managers.Resource.Destroy(_hpBar.gameObject);
        Managers.Object.Remove(Id);
    }

    //캐릭터가 바라보는 방향 설정(좌, 우, 상, 하)
    protected void UpdateDir(Vector2 _destPos)
    {
        if ((Vector2)transform.position == _destPos)
            return;

        Vector2 dir = (_destPos - (Vector2)transform.position).normalized;

        if (dir.x < 0)
        {
            _directionX = DirectionX.Left;
            Xflip(false);
        }
        else
        {
            _directionX = DirectionX.Right;
            Xflip(true);
        }

        if (dir.y < 0)
            _directionY = DirectionY.Front;
        else
            _directionY = DirectionY.Back;
    }

    //캐릭터 좌우반전 - false == Left / true == Right
    protected void Xflip(bool flip = false)
    {
        if (_flipX == flip)
            return;

        if (flip)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _flipX = true;
        }
        else
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _flipX = false;
        }
    }

    public void Sync()
    {
        Vector3 _destPos = Pos;
        transform.position = _destPos;
    }
}
