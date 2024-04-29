using Google.Protobuf.Protocol;
using System.Collections;
using UnityEngine;

public class ProjectileController : BaseController
{
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        if (GetComponent<Poolable>() == null)
        {
            gameObject.AddComponent<Poolable>();
        }

        State = ActionState.Run;
    }

    protected override void Update()
    {
        UpdateMove();
    }

    protected void UpdateMove()
    {
        _destPos = Pos;

        if ((_destPos - (Vector2)transform.position).magnitude > 11f * Time.deltaTime)
        {
            transform.position += ((Vector3)_destPos - transform.position).normalized * 11f * Time.deltaTime;
        }
        else
        {
            transform.position = _destPos;
        }
    }

    public void Destroy()
    {
        StartCoroutine(CoDestroy());
    }

    public IEnumerator CoDestroy()
    {
        _destPos = Pos;
        yield return new WaitUntil(() => (_destPos - (Vector2)transform.position).magnitude < 0.5f);
        Managers.Object.Remove(Id);
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Environment")) 
    //    {
    //        target = collision.gameObject;
    //    }
    //}

    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    target = null;
    //}
}
