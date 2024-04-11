using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Fields

    [SerializeField] MyPlayerController _player;
    [SerializeField] GameObject _bullet;

    //최대 탄창과 현재 탄창은 게임 내에서 표기되는 HUD상에서 참고하여 남은 탄약을 표시함
    //최대 탄창 수
    public int MaxBullets { get; set; } = 30;

    //현재 탄창 수
    public int CurBullets { get; set; } = 30;

    //재장전 시간
    public float ReloadSpeed { get; set; } = 1.5f;

    //사격과 사격 사이의 시간 간격
    public float ShootDelay { get; set; } = 1f;
    public bool canShoot = true;

    //총알이 날아가는 속도와 거리
    public float ShootPower { get; set; } = 10f;

    #endregion

    void Start()
    {
        Init();
    }

    void Init()
    {
        _player = transform.parent.parent.GetComponent<MyPlayerController>();
        _bullet = Managers.Resource.Load<GameObject>($"Prefabs/Bullets/{gameObject.name}Bullet");
    }

    void Update()
    {

    }

    //public IEnumerator SendShootPacket()
    //{
    //    if (canShoot == false)
    //    {
    //        yield break;
    //    }

    //    if (CurBullets <= 0)
    //    {
    //        StartCoroutine(_player.Reload(ReloadSpeed));
    //        yield break;
    //    }

    //    CH_Shoot shoot = new CH_Shoot()
    //    {
    //        PosInfo = new PositionInfo(),
    //    };
    //    shoot.PosInfo = _player.PosInfo;
    //    shoot.WeaponType = _player.WeaponType;
    //    Managers.Network.H_Send(shoot);

    //    CurBullets--;

    //    StartCoroutine(ShootCooldown(ShootDelay));
    //}

    public IEnumerator ShootCooldown(float cooldown)
    {
        canShoot = false;

        yield return new WaitForSeconds(cooldown);

        canShoot = true;
    }

    
}
