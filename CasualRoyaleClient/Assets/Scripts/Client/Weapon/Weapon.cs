using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Fields

    [SerializeField] MyPlayerController _player;
    [SerializeField] GameObject _bullet;

    //�ִ� źâ�� ���� źâ�� ���� ������ ǥ��Ǵ� HUD�󿡼� �����Ͽ� ���� ź���� ǥ����
    //�ִ� źâ ��
    public int MaxBullets { get; set; } = 30;

    //���� źâ ��
    public int CurBullets { get; set; } = 30;

    //������ �ð�
    public float ReloadSpeed { get; set; } = 1.5f;

    //��ݰ� ��� ������ �ð� ����
    public float ShootDelay { get; set; } = 1f;
    public bool canShoot = true;

    //�Ѿ��� ���ư��� �ӵ��� �Ÿ�
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
