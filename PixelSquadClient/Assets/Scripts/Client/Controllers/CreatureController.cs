using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureController : BaseController
{
    #region Fields

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
            _stat.FirstSkillId = value.FirstSkillId;
            _stat.SecondSkillId = value.SecondSkillId;
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

    public int FirstSkillId
    {
        get { return StatInfo.FirstSkillId; }
        set { StatInfo.FirstSkillId = value; }
    }

    public int SecondSkillId
    {
        get { return StatInfo.SecondSkillId; }
        set { StatInfo.SecondSkillId = value; }
    }

    #endregion

    protected bool _flipX = false;  //false면 Left, true면 Right
    protected Animator _animator;
    protected List<AnimationClip> _animationClips = new List<AnimationClip>();
    protected AudioSource _audioSource;
    public string _skillName;
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

    protected Coroutine _coroutine;

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
        _audioSource = GetComponent<AudioSource>();
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

    public virtual bool Attack()
    {
        _coroutine = StartCoroutine(CoAttack());

        return true;
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
        _coroutine = null;
    }

    public void Hit(float hp)
    {
        _coroutine = StartCoroutine(CoHit(hp));
        StartCoroutine(CoRedBody());
    }

    protected virtual IEnumerator CoHit(float hp)
    {
        if (State == ActionState.Dead)
            yield break;

        Hp = hp;
        _hpBar.SetHpBar(Hp, MaxHp);

        float time = 0;
        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{_directionY.ToString()}Hit")
                time = anim.length;
        }

        if(State == ActionState.Idle || State == ActionState.Run)
            State = ActionState.Hit;

        yield return new WaitForSeconds(time);
        
        if(State != ActionState.Dead)
        {
            State = ActionState.Idle;
            _coroutine = null;
        }
    }

    IEnumerator CoRedBody()
    {
        float time = 0, timer = 0;

        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{_directionY.ToString()}Hit")
                time = anim.length;
        }

        while (timer <= time)
        {
            Color color = new Color(1, timer / time, timer / time, 1);
            _sprite.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        _sprite.color = Color.white;
    }

    public virtual void Die(HC_Die diePacket)
    {
        StartCoroutine(CoDie(diePacket));
    }

    protected virtual IEnumerator CoDie(HC_Die diePacket)
    {
        _coroutine = null;
        State = ActionState.Dead;
        yield return new WaitForSeconds(3.0f);
        Clear();
    }

    public virtual bool UsingSkill(int skillId)
    {
        _coroutine = StartCoroutine(CoSkill(skillId));

        return true;
    }

    protected virtual IEnumerator CoSkill(int skillId)
    {
        if (!(State == ActionState.Idle || State == ActionState.Run))
            yield break;

        if (State == ActionState.Dead)
            yield break;

        _skillName = Managers.Data.SkillData[skillId].SkillName;
        float time = 0;
        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{_directionY.ToString()}{_skillName}")
                time = anim.length;
        }

        State = ActionState.Skill;

        yield return new WaitForSeconds(time);

        State = ActionState.Idle;
        yield break;
    }

    void SkillEffect(int skillId)
    {
        Vector2 skillPos;

        switch (Managers.Data.SkillData[skillId].SkillType)
        {
            case "Immediate":
                skillPos = (Vector2)transform.position + LastDir * Managers.Data.SkillData[skillId].Range;
                GameObject go = Managers.Resource.Instantiate("Effect/Skill");
                go.transform.position = skillPos;
                go.GetComponent<SkillEffect>().SetSkill(Id, skillId, Managers.Data.SkillData[skillId].SkillType, LastDir);
                break;

            case "Projectile":
                if(GetComponent<MyPlayerController>() != null)
                {
                    skillPos = (Vector2)transform.position + LastDir * 1;

                    CH_SkillEffect effectPacket = new CH_SkillEffect();
                    PositionInfo posInfo = new PositionInfo();
                    effectPacket.SkillId = skillId;
                    effectPacket.PosInfo = posInfo;
                    effectPacket.PosInfo.PosX = skillPos.x;
                    effectPacket.PosInfo.PosY = skillPos.y;
                    effectPacket.PosInfo.LastDirX = LastDir.x;
                    effectPacket.PosInfo.LastDirY = LastDir.y;
                    Managers.Network.H_Send(effectPacket);
                }
                break;
        }
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

    void MakeSound(string path)
    {
        Managers.Sound.Play(_audioSource, $"Sounds/SFX/{path}", Define.Sound.Effect, 1.0f);
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
            case ActionState.Skill:
                _animator.Play($"{_directionY.ToString()}{_skillName}");
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
}
