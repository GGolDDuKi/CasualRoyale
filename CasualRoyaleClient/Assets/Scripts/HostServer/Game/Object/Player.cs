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
			if (Room == null)
				return;

			State = ActionState.Dead;

			HC_Die diePacket = new HC_Die();
			diePacket.ObjectId = Id;
			diePacket.Rank = Room.GetRank() - 1;
			diePacket.AttackerId = attacker.Id;
			Room.Broadcast(diePacket);
		}
	}
}
