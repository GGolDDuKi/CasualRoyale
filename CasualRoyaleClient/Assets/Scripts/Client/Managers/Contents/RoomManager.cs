﻿using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager
{
    public Dictionary<int, RoomInfo> RoomInfo { get; set; } = new Dictionary<int, RoomInfo>();
    public Dictionary<int, Room> Room { get; set; } = new Dictionary<int, Room>();

    public Room _selected {  get; set; }

    public void UpdateRoomList()
    {
        Transform parent = UnityEngine.GameObject.Find("Content").transform;
        
        if (parent == null)
            return;

        foreach(Room room in Room.Values)
        {
            if (!RoomInfo.ContainsKey(room.RoomId))
            {
                Managers.Resource.Destroy(room.gameObject);
            }
        }

        foreach (RoomInfo room in RoomInfo.Values)
        {
            if (!Room.ContainsKey(room.RoomId))
            {
                UnityEngine.GameObject go = Managers.Resource.Instantiate("UI/Room", parent);
                go.GetComponent<Room>().Init(room);
                Room.Add(room.RoomId, go.GetComponent<Room>());
            }
        }
    }
}
