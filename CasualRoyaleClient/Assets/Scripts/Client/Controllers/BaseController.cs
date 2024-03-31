using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    #region Fields

    public int Id { get; set; }

    public int Name { get; set; }

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
            WeaponState = value.WeaponState;
            Dir = value.Dir;
            Pos = new Vector2(value.PosX, value.PosY);
            WeaponPos = new Vector2(value.WeaponPosX, value.WeaponPosY);
            LookDir = new Vector2(value.DirX, value.DirY);
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

    //무기 위치
    public Vector2 WeaponPos
    {
        get { return new Vector2(PosInfo.WeaponPosX, PosInfo.WeaponPosY); }
        set
        {
            if (PosInfo.WeaponPosX == value.x && PosInfo.WeaponPosY == value.y)
                return;

            PosInfo.WeaponPosX = value.x;
            PosInfo.WeaponPosY = value.y;
            _updated = true;
        }
    }

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

    public WeaponState WeaponState
    {
        get { return PosInfo.WeaponState; }
        set
        {
            if (PosInfo.WeaponState == value)
                return;

            PosInfo.WeaponState = value;
            _updated = true;
        }
    }

    public DirX Dir
    {
        get { return PosInfo.Dir; }
        set
        {
            if (PosInfo.Dir == value)
                return;

            PosInfo.Dir = value;
            _updated = true;
        }
    }

    public Vector2 LookDir
    {
        get { return new Vector2(PosInfo.DirX, PosInfo.DirY); }
        set
        {
            if (PosInfo.DirX == value.x && PosInfo.DirY == value.y)
                return;

            PosInfo.DirX = value.x;
            PosInfo.DirY = value.y;
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

    public WeaponType WeaponType = WeaponType.Hg;

    protected bool _flipX = false;
    protected Vector2 _destPos = Vector2.zero;
    protected bool _updated = false;
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    #endregion

    protected virtual void Init()
    {

    }

    protected virtual void Update()
    {
        UpdateMove();
    }

    //캐릭터가 바라보는 방향 설정(좌, 우)
    protected void UpdateDirX(Vector2 dir)
    {
        if (dir.x < 0)
        {
            Dir = DirX.Left;
        }
        else if (dir.x > 0)
        {
            Dir = DirX.Right;
        }
    }

    protected virtual void UpdateMove()
    {
        _destPos = Pos;

        switch (State)
        {
            case ActionState.Run:
                if ((_destPos - (Vector2)transform.position).magnitude > 1.5f * Time.deltaTime)
                {
                    transform.position += ((Vector3)_destPos - transform.position).normalized * 1.5f * Time.deltaTime;
                }
                else
                {
                    transform.position = _destPos;
                }
                break;
            case ActionState.Dash:
                if ((_destPos - (Vector2)transform.position).magnitude > 10.0f * Time.deltaTime)
                {
                    transform.position += ((Vector3)_destPos - transform.position).normalized * 10.0f * Time.deltaTime;
                }
                else
                {
                    transform.position = _destPos;
                }
                break;
        }
    }

    protected virtual void UpdateAnimation()
    {

    }

    //오브젝트가 바라보는 각도 설정
    protected void UpdateObjRotation(Transform obj, Vector2 lookDir)
    {
        float angle;

        if (_flipX == false)
        {
            angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        }
        else
        {
            angle = Mathf.Atan2(-lookDir.y, -lookDir.x) * Mathf.Rad2Deg;
        }

        obj.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Sync()
    {
        Vector3 destPos = Pos;
        transform.position = destPos;
        UpdateObjRotation(transform, LookDir);
    }
}
