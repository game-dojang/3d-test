using System;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private int id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager roomManager = RoomManager.Instance;
            if (roomManager != null)
            {
                roomManager.SetNeighborRoom(id);
            }
        }
    }
}
