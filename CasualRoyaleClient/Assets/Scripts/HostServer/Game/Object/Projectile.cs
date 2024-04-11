using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
	public class Projectile : GameObject
	{
		public GameObject Owner { get; set; }

		public Projectile()
		{
			ObjectType = GameObjectType.Projectile;
		}

		//public void CollisionCheck()
		//{
		//	if (Managers.Object.Objects[Id].GetComponent<BulletController>().target != null)
		//	{
		//		int targetId = Managers.Object.Objects[Id].GetComponent<BulletController>().target.GetComponent<CreatureController>().Id;
		//	}
		//}
	}
}
