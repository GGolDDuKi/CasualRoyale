using Google.Protobuf.Protocol;
using UnityEngine;

public class BulletController : BaseController
{
    public GameObject target;

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        if (GetComponent<Poolable>() == null)
        {
            gameObject.AddComponent<Poolable>();
        }

        _sprite = GetComponent<SpriteRenderer>();
        State = ActionState.Run;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateMove()
    {
        _destPos = Pos;

        switch (WeaponType)
        {
            case WeaponType.Hg:
                if ((_destPos - (Vector2)transform.position).magnitude > 20f * Time.deltaTime)
                {
                    transform.position += ((Vector3)_destPos - transform.position).normalized * 20f * Time.deltaTime;
                }
                else
                {
                    transform.position = _destPos;
                }
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Environment")) 
        {
            target = collision.gameObject;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        target = null;
    }
}
