using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType
    {
        StartRoom,
        Basic,
        BossRoom,
        EndRoom
    }
    public GameObject[] Doors;
    public GameObject[] ProjectileDestroyers;
    private bool isRoomStarted = false;
    private bool isRoomFinished = false;
    public GameObject monstersParent;
    public GameObject bossesParent;
    public GameObject player;
    public GameObject chest;
    public RoomType roomType = RoomType.Basic;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRoomStarted)
        {
            if(monstersParent.transform.childCount == 0 
                && bossesParent.transform.childCount == 0
                && !isRoomFinished)
            {
                FinishRoom();
            }
            
        }
    }

    public void StartRoom(GameObject player)
    {
        if (!isRoomStarted)
        {
            this.player = player;
            this.player.GetComponent<Player>().isClearingRoom = true;
            isRoomStarted = true;
            CloseDoors();

            for(var i = 0; i < monstersParent.transform.childCount; i++)
            {
                monstersParent.transform.GetChild(i).gameObject.GetComponent<Monster>().isActive = true;
            }

            for (var i = 0; i < bossesParent.transform.childCount; i++)
            {
                bossesParent.transform.GetChild(i).gameObject.GetComponent<Monster>().isActive = true;
            }
        }
        
    }

    private void CloseDoors()
    {
        foreach(var door in Doors)
        {
            door.GetComponent<BoxCollider2D>().isTrigger = false;
            door.GetComponent<SpriteRenderer>().color = Color.red;
        }

        foreach (var destroyer in ProjectileDestroyers)
        {
            destroyer.tag = "Untagged";
        }
    }

    private void FinishRoom()
    {
        isRoomFinished = true;
        player.GetComponent<Player>().isClearingRoom = false;
        OpenDoors();
        chest.SetActive(true);
    }

    private void OpenDoors()                
    {
        foreach (var door in Doors)
        {
            door.GetComponent<BoxCollider2D>().isTrigger = true;
            door.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
