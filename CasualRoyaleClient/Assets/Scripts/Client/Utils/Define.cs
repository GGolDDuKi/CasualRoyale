using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game
    }

    public enum Sound
    {
        Bgm,
        Effect
    }

    public enum UIEvent
    {
        Click,
        Drag
    }

    public class User
    {
        public UserInfo Info;
    }
}
