using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HostServer.Game
{
	public class Bullet : Projectile
	{
        float timer = 0;

        long _nextMoveTick = 0;

        public override void Update()
		{
			if (Owner == null || Room == null)
				return;

            if (_nextMoveTick >= Environment.TickCount)
                return;

            long tick = (long)(1000 / Speed);
            _nextMoveTick = Environment.TickCount + tick;
            timer++;

            //TODO
            //Vector2 destPos = GetFrontPos(new Vector2(PosInfo.DirX, PosInfo.DirY));

			if (timer < 5)
			{
				//for (float t = 0; t <= 1; t += 0.1f)
				//{
				//	Vector2 newPos = Vector2.Lerp(Pos, destPos, t);
				//	BoxCollider2D newCol = new BoxCollider2D(newPos.x, newPos.y, Collider.width, Collider.height);

				//	Pos = newPos;
				//	HC_Move mp = new HC_Move();
				//	mp.ObjectId = Id;
				//	mp.PosInfo = PosInfo;
				//	Console.WriteLine($"Bullet[{Id}] 이동 - [{(mp.PosInfo.PosX).ToString("0.00")}, {mp.PosInfo.PosY.ToString("0.00")}]");
				//	Room.Broadcast(mp);

				//	foreach (Player p in Room.Players.Values)
				//	{
				//		if (BoxCollider2D.Collision(newCol, p.Collider))
				//		{
				//			if (p != Owner)
				//			{
				//				GameObject target = p;
				//				target.OnDamaged(this, Owner.StatInfo.Damage);
				//				Console.WriteLine($"{Owner.Id}가 {p.Id}를 공격!");
				//				Room.Push(Room.LeaveGame, Id);
				//			}
				//		}
				//	}
				//}

				//Pos = destPos;
				HC_Move movePacket = new HC_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;

				Debug.Log($"Bullet[{Id}] 이동 - [{(movePacket.PosInfo.PosX).ToString("0.00")}, {movePacket.PosInfo.PosY.ToString("0.00")}]");

				Room.Broadcast(movePacket);
			}
			else
			{
				// 소멸
				Room.Push(Room.LeaveGame, Id);
			}
		}
	}
}
