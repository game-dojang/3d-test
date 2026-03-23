using UnityEngine;
using System.Collections.Generic;

public class Room
{
    public int Id { get; private set; }                     // 룸의 ID
    public List<Room> Neighbors { get; private set; }       // 주변 룸
    public GameObject roomInstance;                         // 씬에 생성된 룸 모델

    public Room(int id)
    {
        roomInstance = null;
        Id = id;
        Neighbors = new List<Room>();
    }

    public void AddNeighbor(Room room)
    {
        if (!Neighbors.Contains(room))
        {
            Neighbors.Add(room);
            room.AddNeighbor(this);
        }
    }
    
    public void RemoveNeighbor(Room room)
    {
        if (Neighbors.Contains(room))
        {
            Neighbors.Remove(room);
            room.Neighbors.Remove(this);
        }
    }
}
