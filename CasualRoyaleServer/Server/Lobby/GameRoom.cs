using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameRoom
    {
        #region RoomInfo Info

        public RoomInfo Info { get; set; } = new RoomInfo();

        public int Id
        {
            get { return Info.RoomId; }
            set { Info.RoomId = value; }
        }

        public string Name
        {
            get { return Info.RoomName; }
            set { Info.RoomName = value; }
        }

        public string Password
        {
            get { return Info.Password; }
            set { Info.Password = value; }
        }

        public bool SecretRoom
        {
            get { return Info.SecretRoom; }
            set { Info.SecretRoom = value; }
        }

        public int CurMember
        {
            get { return Info.CurMember; }
            set { Info.CurMember = value; }
        }

        public int MaxMember
        {
            get { return Info.MaxMember; }
            set { Info.MaxMember = value; }
        }

        public int HostId
        {
            get { return Info.HostId; }
            set { Info.HostId = value; }
        }

        public string HostName
        {
            get { return Info.HostName; }
            set { Info.HostName = value; }
        }

        #endregion
    }
}
