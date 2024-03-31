﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }

    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	public Dictionary<int, GameObject> Objects { get { return _objects; } }
	public Dictionary<int, BulletController> Bullets {
		get 
		{
			Dictionary<int, BulletController> _bullets = new Dictionary<int, BulletController>();
			foreach(var obj in _objects.Values)
            {
				if (obj.GetComponent<BulletController>() != null)
					_bullets.Add(obj.GetComponent<BulletController>().Id, obj.GetComponent<BulletController>());
            }
			return _bullets;
		} 
	}
	
	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;
		return (GameObjectType)type;
	}

	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);

		if (objectType == GameObjectType.Player)
		{
			if (myPlayer)
			{
				GameObject go = Managers.Resource.Instantiate("Creatures/MyPlayer");
				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.StatInfo = info.StatInfo;
                MyPlayer.Sync();
            }
			else
			{
				GameObject go = Managers.Resource.Instantiate("Creatures/Player");
				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.StatInfo = info.StatInfo;
                pc.Sync();
            }
		}
        else if (objectType == GameObjectType.Projectile)
        {
            switch (info.WeaponType)
            {
				case WeaponType.Hg:
					GameObject go = Managers.Resource.Instantiate("Bullets/HandgunBullet");
					go.name = "HandgunBullet";

					BulletController bc = go.GetComponent<BulletController>();
					bc.Id = info.ObjectId;
					bc.PosInfo = info.PosInfo;
					bc.StatInfo = info.StatInfo;

					_objects.Add(info.ObjectId, go);
					bc.Sync();
                    break;
            }
        }
    }

	public void Remove(int id)
	{
		GameObject go = FindById(id);
		if (go == null)
			return;

		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public GameObject FindById(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		return go;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		foreach (GameObject obj in _objects.Values)
			Managers.Resource.Destroy(obj);
		_objects.Clear();
        MyPlayer = null;
    }
}
