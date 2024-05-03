using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    #region Fields
    
    public int Id { get; set; }

    public string Name { get; set; }

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

    protected bool _updated = false;
    public SpriteRenderer _sprite;
    protected int layer;
    protected Vector2 _destPos = Vector2.zero;

    #endregion

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        SetLayer(_sprite);
    }

    protected virtual void Update()
    {
        
    }

    protected void SetLayer(SpriteRenderer sprite)
    {
        YPosToLayer();
        sprite.sortingOrder = layer;
    }

    protected void SetLayer(List<SpriteRenderer> sprites)
    {
        YPosToLayer();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = layer;
        }
    }

    void YPosToLayer()
    {
        layer = -(int)Math.Round(transform.position.y);
    }

    public void Sync()
    {
        Vector3 _destPos = Pos;
        transform.position = _destPos;
    }
}
