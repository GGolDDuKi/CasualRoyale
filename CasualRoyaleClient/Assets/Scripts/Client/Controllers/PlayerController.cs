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

    //LookDir(ĳ���Ͱ� ���� ����) ������Ʈ�ϴ� �Լ�
    //Aim ���̽�ƽ�� Move ���̽�ƽ���� �켱���� ����
    protected virtual void UpdateCharacterDir()
    {
        UpdateDirX(LookDir);
        UpdateObjRotation(_head, LookDir);
        UpdateObjRotation(_hand.transform, LookDir);
    }

    //ĳ���� �ִϸ��̼��� ������Ʈ�ϴ� �Լ�
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

    //ĳ���͸� �������� �¿����
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

    //ĳ���� �¿����
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
