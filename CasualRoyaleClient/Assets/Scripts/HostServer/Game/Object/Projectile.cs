using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
	public class Projectile : GameObject
	{
		public GameObject Owner { get; set; }

		public int SkillId { get; set; }

        public override BoxCollider2D Collider
        {
            get
            {
                return new BoxCollider2D(PosInfo.PosX, PosInfo.PosY, Managers.Data.SkillData[SkillId].Width, Managers.Data.SkillData[SkillId].Height);
            }
        }

        public Vector2 StartPos { get; set; }
        public Vector2 DestPos { get; set; }
        public float _counter = 0;

        long _nextMoveTick = 0;

        public Projectile()
		{
			ObjectType = GameObjectType.Projectile;
            Speed = 4.0f;
		}

        public override void Update()
        {
            if (Owner == null || Room == null)
                return;

            if (_nextMoveTick >= Environment.TickCount)
                return;

            long tick = (long)(1000 / Speed / 5);
            _nextMoveTick = Environment.TickCount + tick;

            Vector2 nextPos = GetFrontPos(DestPos, Speed / 5);
            UpdatePosition(nextPos);
            CheckCollision();

            if (Pos == DestPos)
                Room.Push(Room.LeaveGame, Id);
        }

        public void MoveAndCheckCollision(Vector2 DestPos, float step = 0.2f)
        {
            bool isCollision = false;
            for (float t = 0; t <= 1; t += step)
            {
                Vector2 newPos = Vector2.Lerp(Pos, DestPos, t);
                UpdatePosition(newPos);
                isCollision = CheckCollision();
                if (isCollision)
                    break;
            }
        }

        private void UpdatePosition(Vector2 newPos)
        {
            Pos = newPos;
            HC_Move mp = new HC_Move();
            mp.ObjectId = Id;
            PositionInfo posInfo = new PositionInfo();
            mp.PosInfo = posInfo;
            mp.PosInfo = PosInfo;
            Room.Broadcast(mp);
        }

        private bool CheckCollision()
        {
            BoxCollider2D newCol = new BoxCollider2D(Pos.x, Pos.y, Collider.width, Collider.height);

            foreach (Player p in Room.Players.Values)
            {
                if (BoxCollider2D.Collision(newCol, p.Collider) && p != Owner)
                {
                    p.OnDamaged(this, Owner.StatInfo.Damage * Managers.Data.SkillData[SkillId].DmgRatio);
                    Room.Push(Room.LeaveGame, Id);
                    return true;
                }
            }
            return false;
        }
    }
}