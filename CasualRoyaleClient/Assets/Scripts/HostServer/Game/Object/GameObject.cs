using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
	public class GameObject
	{
		public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;

		public GameRoom Room { get; set; }

		public BoxCollider2D Collider
		{
			get
			{
				switch (ObjectType)
				{
					case GameObjectType.Projectile:
						return new BoxCollider2D(PosInfo.PosX, PosInfo.PosY, 0.1f, 0.1f);
					case GameObjectType.Player:
						return new BoxCollider2D(PosInfo.PosX, PosInfo.PosY, 0.3f, 0.4f);
					default:
						return new BoxCollider2D(PosInfo.PosX, PosInfo.PosY, 0.1f, 0.2f);
				}
			}
		}

		#region ObjectInfo Info

		public ObjectInfo Info { get; set; } = new ObjectInfo();

		public int Id
		{
			get { return Info.ObjectId; }
			set { Info.ObjectId = value; }
		}

		public string Name
		{
			get { return Info.Name; }
			set { Info.Name = value; }
		}

		public WeaponType WeaponType
		{
			get { return Info.WeaponType; }
			set { Info.WeaponType = value; }
		}

		#endregion

		#region PositionInfo PosInfo

		public PositionInfo PosInfo { get; private set; } = new PositionInfo();

		public ActionState State
		{
			get { return PosInfo.State; }
			set { PosInfo.State = value; }
		}

		public WeaponState WeaponState
		{
			get { return PosInfo.WeaponState; }
			private set { PosInfo.WeaponState = value; }
		}

		public DirX Dir
		{
			get { return PosInfo.Dir; }
			set { PosInfo.Dir = value; }
		}

		public Vector2 Pos
		{
			get
			{
				return new Vector2(PosInfo.PosX, PosInfo.PosY);
			}

			set
			{
				PosInfo.PosX = value.x;
				PosInfo.PosY = value.y;
			}
		}

		public Vector2 WeaponPos
		{
			get { return new Vector2(PosInfo.WeaponPosX, PosInfo.WeaponPosY); }
			set
			{
				if (PosInfo.WeaponPosX == value.x && PosInfo.WeaponPosY == value.y)
					return;

				PosInfo.WeaponPosX = value.x;
				PosInfo.WeaponPosY = value.y;
			}
		}

		public Vector2 LookDir
		{
			get
			{
				return new Vector2(PosInfo.DirX, PosInfo.DirY);
			}

			set
			{
				PosInfo.DirX = value.x;
				PosInfo.DirY = value.y;
			}
		}

		#endregion

		#region StatInfo StatInfo

		public StatInfo StatInfo { get; private set; } = new StatInfo();

		public float Hp
		{
			get { return StatInfo.Hp; }
			set { StatInfo.Hp = Math.Clamp(value, 0, StatInfo.MaxHp); }
		}

		public float Speed
		{
			get { return StatInfo.Speed; }
			set { StatInfo.Speed = value; }
		}

		public float Damage
		{
			get { return StatInfo.Damage; }
			set { StatInfo.Damage = value; }
		}

		#endregion

		public GameObject()
		{
			Info.PosInfo = PosInfo;
			Info.StatInfo = StatInfo;
		}

		public virtual void Update()
		{

		}

		public Vector2 GetFrontPos()
		{
			return GetFrontPos(LookDir);
		}

		public Vector2 GetFrontPos(Vector2 dir)
		{
			Vector2 pos = Pos;

			if (ObjectType == GameObjectType.Projectile)
			{
				if (WeaponType == WeaponType.Hg)
				{
					float newX = pos.x + dir.x * 2.5f;
					float newY = pos.y + dir.y * 2.5f;

					return new Vector2(newX, newY);
				}
			}
			return pos;
		}

		public virtual void OnDamaged(GameObject attacker, float damage)
		{
			if (Room == null)
				return;

			StatInfo.Hp = Math.Max(StatInfo.Hp - damage, 0);

			HC_ChangeHp changePacket = new HC_ChangeHp();
			changePacket.ObjectId = Id;
			changePacket.Hp = StatInfo.Hp;
			Room.Broadcast(changePacket);



			if (StatInfo.Hp <= 0)
			{
				OnDead(attacker);
			}
		}

		public virtual void OnDead(GameObject attacker)
		{
			if (Room == null)
				return;

			HC_Die diePacket = new HC_Die();
			diePacket.ObjectId = Id;
			diePacket.AttackerId = attacker.Id;
			Room.Broadcast(diePacket);

			GameRoom room = Room;
			room.LeaveGame(Id);

			StatInfo.Hp = StatInfo.MaxHp;
			PosInfo.State = ActionState.Idle;
			PosInfo.PosX = 0;
			PosInfo.PosY = 0;

			room.EnterGame(this);
		}
	}
}
