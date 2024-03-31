using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
    public class Map
    {
        public float MinX { get; set; } = -50;
        public float MaxX { get; set; } = 50;
        public float MinY { get; set; } = -50;
        public float MaxY { get; set; } = 50;

        public float SizeX { get { return MaxX - MinX + 1; } }
        public float SizeY { get { return MaxY - MinY + 1; } }

        Dictionary<GameObject, Vector2> _objects = new Dictionary<GameObject, Vector2>();
        public Dictionary<GameObject, Vector2> Objects { get { return _objects; } }

        public bool CanGo(Vector2 pos, bool checkObjects = true)
        {
            if (pos.x < MinX || pos.x > MaxX)
                return false;
            if (pos.y < MinY || pos.y > MaxY)
                return false;

            return true;
        }

        //public GameObject Find(Vector2 pos)
        //{
        //    if (pos.x < MinX || pos.x > MaxX)
        //        return null;
        //    if (pos.y < MinY || pos.y > MaxY)
        //        return null;

        //    foreach (var obj in _objects)
        //    {
        //        if (obj.Value == pos)
        //        {
        //            return obj.Key;
        //        }
        //    }

        //    return null;
        //}

        public bool ApplyLeave(GameObject gameObject)
        {
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.PosInfo;
            if (posInfo.PosX < MinX || posInfo.PosX > MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY > MaxY)
                return false;

            //float x = posInfo.PosX;
            //float y = posInfo.PosY;
            //if (_objects.ContainsKey(gameObject))
            //    _objects.Remove(gameObject);

            return true;
        }

        public bool ApplyMove(GameObject gameObject, Vector2 dest)
        {
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.PosInfo;
            if (CanGo(dest, true) == false)
                return false;

            //_objects[gameObject] = dest;

            // 실제 좌표 이동
            posInfo.PosX = dest.x;
            posInfo.PosY = dest.y;
            return true;
        }
    }
}
