using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RoomPrefabInfo
{
    public int Id;
    public GameObject Prefab;
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<RoomPrefabInfo> roomPrefabs;

    private Dictionary<int, Room> rooms;
    private Room currentRoom;
    
    private static RoomManager _instance;
    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<RoomManager>();
                if (_instance == null)
                {
                    // TODO:
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Room room1 = new Room(0);
        Room room2 = new Room(1);
        Room room3 = new Room(2);
        Room room4 = new Room(3);
        Room room5 = new Room(4);
        Room room6 = new Room(5);
        Room room7 = new Room(6);
        Room room8 = new Room(7);
        
        room1.AddNeighbor(room2);
        room2.AddNeighbor(room3);
        room3.AddNeighbor(room4);
        room3.AddNeighbor(room5);
        room3.AddNeighbor(room6);
        room6.AddNeighbor(room7);
        room7.AddNeighbor(room8);

        rooms = new Dictionary<int, Room>
        {
            { 0, room1 },
            { 1, room2 },
            { 2, room3 },
            { 3, room4 },
            { 4, room5 },
            { 5, room6 },
            { 6, room7 },
            { 7, room8 }
        };
    }

    private void Start()
    {
        // 첫 번째 룸을 생성
        var startRoomId = 0;
        
        var prefab = GetRoomPrefab(startRoomId);
        if (prefab != null)
        {
            var intance = Instantiate(prefab);
            rooms[startRoomId].roomInstance = intance;
        }
    }

    // 특정 id 룸의 Neighbor Room을 생성하는 함수
    public void SetNeighborRoom(int id)
    {
        var room = rooms[id];
        if (room == null) return;

        foreach (var neighbor in room.Neighbors)
        {
            if (!neighbor.roomInstance)
            {
                var prefab = GetRoomPrefab(neighbor.Id);
                if (prefab != null)
                {
                    neighbor.roomInstance = Instantiate(prefab);
                }
            }
        }

        // 이전 룸의 Neighbor Room 제거
        if (currentRoom != null)
        {
            foreach (var neighbor in currentRoom.Neighbors)
            {
                if (neighbor != room && !room.Neighbors.Contains(neighbor) && neighbor.roomInstance)
                {
                    Destroy(neighbor.roomInstance);
                    neighbor.roomInstance = null;
                }
            }
        }
        
        currentRoom = room;
    }

    private GameObject GetRoomPrefab(int id)
    {
        var prefab = roomPrefabs.Find(x => x.Id == id);
        return prefab.Prefab;
    }
}
