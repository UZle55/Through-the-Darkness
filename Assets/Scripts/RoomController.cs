using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject[] Doors;
    public GameObject[] ProjectileDestroyers;
    private bool isRoomStarted = false;
    private bool isRoomFinished = false;
    public GameObject MeleeMonstersParent;
    public GameObject RangeMonstersParent;
    public GameObject BossesParent;
    public GameObject Player;
    public GameObject Chest;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRoomStarted)
        {
            if(MeleeMonstersParent.transform.childCount == 0 
                && RangeMonstersParent.transform.childCount == 0
                && BossesParent.transform.childCount == 0
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
            Player = player;
            Player.GetComponent<Player>().isClearingRoom = true;
            isRoomStarted = true;
            CloseDoors();

            for(var i = 0; i < MeleeMonstersParent.transform.childCount; i++)
            {
                MeleeMonstersParent.transform.GetChild(i).gameObject.GetComponent<Monster>().isActive = true;
            }

            for (var i = 0; i < RangeMonstersParent.transform.childCount; i++)
            {
                RangeMonstersParent.transform.GetChild(i).gameObject.GetComponent<Monster>().isActive = true;
            }

            for (var i = 0; i < BossesParent.transform.childCount; i++)
            {
                BossesParent.transform.GetChild(i).gameObject.GetComponent<Monster>().isActive = true;
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
        Player.GetComponent<Player>().isClearingRoom = false;
        OpenDoors();
        Chest.SetActive(true);
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
