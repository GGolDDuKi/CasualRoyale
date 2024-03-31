using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    #region Fields

    [SerializeField] protected List<SpriteRenderer> _bodies;
    [SerializeField] protected Transform _head;
    [SerializeField] protected Transform _hand;
    [SerializeField] protected Weapon _weapon;

    #endregion

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
            {
                _bodies.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            }

            if (transform.GetChild(i).gameObject.name == "Head")
            {
                _head = transform.GetChild(i);
            }

            if (transform.GetChild(i).gameObject.name == "Hand")
            {
                _hand = transform.GetChild(i);
                _weapon = _hand.GetChild(0).GetComponent<Weapon>();
                _bodies.Add(_weapon.GetComponent<SpriteRenderer>());
            }
        }
        _animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAnimation();
        UpdateCharacterDir();
    }

    //LookDir(캐릭터가 보는 방향) 업데이트하는 함수
    //Aim 조이스틱이 Move 조이스틱보다 우선권이 높음
    protected virtual void UpdateCharacterDir()
    {
        UpdateDirX(LookDir);
        UpdateObjRotation(_head, LookDir);
        UpdateObjRotation(_hand.transform, LookDir);
    }

    //캐릭터 애니메이션을 업데이트하는 함수
    protected override void UpdateAnimation()
    {
        switch (Dir)
        {
            case DirX.Left:
                Xflip(true);
                break;
            case DirX.Right:
                Xflip(false);
                break;
        }

        switch (State)
        {
            case ActionState.Idle:
                _animator.Play("Idle");
                break;
            case ActionState.Run:
                _animator.Play("Run");
                break;
        }
    }

    //캐릭터를 기준으로 좌우반전
    void PosInversion(Transform obj, bool flip = false)
    {
        if (flip == false)
        {
            if (obj.localPosition.x < 0)
            {
                obj.localPosition = new Vector3(-obj.localPosition.x, 0, 0);
            }
        }
        else
        {
            if (obj.localPosition.x > 0)
            {
                obj.localPosition = new Vector3(-obj.localPosition.x, 0, 0);
            }
        }
    }

    //캐릭터 좌우반전
    void Xflip(bool flip = false)
    {
        foreach (SpriteRenderer body in _bodies)
        {
            body.flipX = flip;
        }
        _flipX = flip;
        PosInversion(_weapon.transform, _flipX);
    }
}
