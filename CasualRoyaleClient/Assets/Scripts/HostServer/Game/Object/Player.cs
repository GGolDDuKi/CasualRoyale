using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

		public override void OnDamaged(GameObject attacker, float damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}
	}
}
