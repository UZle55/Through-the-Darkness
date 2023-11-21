using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private GameObject[,] rooms = new GameObject[5, 5];
    private GameObject[,] roomIcons = new GameObject[5, 5];
    public GameObject roomsParent;
    public GameObject passagesParent;
    public GameObject currentRoomMarker;
    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < roomsParent.transform.childCount; i++)
        {
            var nameParts = roomsParent.transform.GetChild(i).gameObject.name.Split();
            roomIcons[int.Parse(nameParts[1]) - 1, int.Parse(nameParts[2]) - 1] = roomsParent.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRooms(GameObject[,] roomsArr)
    {
        this.rooms = roomsArr;
        foreach(var roomIcon in roomIcons)
        {
            roomIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        for(var i = 0; i < rooms.GetLength(0); i++)
        {
            for(var q = 0; q < rooms.GetLength(1); q++)
            {
                if(rooms[i, q] != null)
                {
                    roomIcons[i, q].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
    }

    public void SetPassages(List<string> numbers)
    {
        for(var i = 0; i < passagesParent.transform.childCount; i++)
        {
            var number = passagesParent.transform.GetChild(i).gameObject.name.Split(" ", 2)[1];
            var isContain = false;
            foreach (var num in numbers)
            {
                if (num.Equals(number))
                {
                    isContain = true;
                    break;
                }
            }
            if (!isContain)
            {
                passagesParent.transform.GetChild(i).GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 0);
            }
            else
            {
                passagesParent.transform.GetChild(i).GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 1);
            }
        }
    }

    public void InsideRoom(GameObject room)
    {
        for (var i = 0; i < rooms.GetLength(0); i++)
        {
            for (var q = 0; q < rooms.GetLength(1); q++)
            {
                if (rooms[i, q] != null && rooms[i, q].Equals(room))
                {
                    currentRoomMarker.transform.position = roomIcons[i, q].transform.position;
                }
            }
        }
    }

    public void OutsideRooms()
    {
        currentRoomMarker.transform.position = new Vector3(100000, 100000, 0);
    }
}
