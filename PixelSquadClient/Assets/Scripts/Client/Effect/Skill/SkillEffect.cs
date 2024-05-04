using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    Animator _animator;
    List<AnimationClip> _animationClips = new List<AnimationClip>();
    AudioSource _audioSource;

    int _userId;
    int _skillId;
    string _type;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _animator = GetComponent<Animator>();
        _animationClips.AddRange(_animator.runtimeAnimatorController.animationClips);
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetSkill(int userId, int skillId, string type, Vector2 lastDir)
    {
        if (_animator == null || _animationClips.Count == 0)
            Init();

        _userId = userId;
        _skillId = skillId;

        if (lastDir.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        float time = 0;
        foreach (var anim in _animationClips)
        {
            if (anim.name == $"{Managers.Data.SkillData[skillId].SkillName}")
                time = anim.length;
        }

        _animator.Play($"{Managers.Data.SkillData[skillId].SkillName}");

        StartCoroutine(CoDuration(time));
    }

    public void SetSkill(string name, Vector2 lastDir)
    {
        if (_animator == null || _animationClips.Count == 0)
            Init();

        _animator.Play($"{name}");

        float angle = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg + 180;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
        //this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public void SkillDamage()
    {
        if (!(_userId == Managers.Object.MyPlayer.Id))
            return;

        //이펙트 현재 위치랑 범위 따라서 서버로 데미지 패킷 전송
        CH_SkillDamage damagePacket = new CH_SkillDamage();
        damagePacket.SkillId = _skillId;
        damagePacket.PosX = transform.position.x;
        damagePacket.PosY = transform.position.y;
        Managers.Network.H_Send(damagePacket);
    }

    void MakeSound(string path)
    {
        Managers.Sound.Play(_audioSource, $"Sounds/SFX/{path}", Define.Sound.Effect, 1.0f);
    }

    IEnumerator CoDuration(float time)
    {
        yield return new WaitForSeconds(time);
        Managers.Resource.Destroy(this.gameObject);
    }
}
