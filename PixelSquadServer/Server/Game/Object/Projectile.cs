using Google.Protobuf.Protocol;
using Server.Data;
using Server.Define;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Object
{
	public class Projectile : GameObject
	{
		public GameObject Owner { get; set; }

		public int SkillId { get; set; }

        public override BoxCollider2D Collider
        {
            get
            {
                return new BoxCollider2D(PosInfo.PosX, PosInfo.PosY, DataManager.Instance.GetSkillData(SkillId).Width, DataManager.Instance.GetSkillData(SkillId).Height);
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
            S_Move mp = new S_Move();
            mp.ObjectId = Id;
            PositionInfo posInfo = new PositionInfo();
            mp.PosInfo = posInfo;
            mp.PosInfo = PosInfo;
            Room.Broadcast(mp);
        }

        private bool CheckCollision()
        {
            if (Room.GameStart == false)
                return false;

            BoxCollider2D newCol = new BoxCollider2D(Pos.x, Pos.y, Collider.width, Collider.height);

            foreach (Player p in Room.Players.Values)
            {
                if (BoxCollider2D.Collision(newCol, p.Collider) && p != Owner)
                {
                    p.OnDamaged(this, Owner.StatInfo.Damage * DataManager.Instance.GetSkillData(SkillId).DmgRatio);
                    Room.Push(Room.LeaveGame, Id);
                    return true;
                }
            }
            return false;
        }
    }
}