using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnPoint : MonoBehaviour
{
    public enum RoomType
    {
        StartRoom,
        Basic,
        EndRoom
    }
    public bool hasPassageUp = true;
    public bool hasPassageDown = true;
    public bool hasPassageLeft = true;
    public bool hasPassageRight = true;
    public RoomType roomType = RoomType.Basic;
    public bool isLastBasicOnFloor = false;
    public int lineIndex;
    public int rowIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
