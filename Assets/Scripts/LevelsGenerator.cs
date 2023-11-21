using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
    public GameObject basicRooms;
    public GameObject bossRooms;
    public GameObject floorLayouts;
    private GameObject currentFloorParent;
    public GameObject player;
    public GameObject floorGeneratingSpace;
    private int currentFloorNumber = 0;
    private bool isBossLvl = false;
    public GameObject allLoot;
    public GameObject allMonsters;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNextFloor()
    {
        var pastFloorParent = currentFloorParent;
        currentFloorParent = new GameObject();
        if (pastFloorParent != null)
        {
            Destroy(pastFloorParent);
        }
        currentFloorNumber++;
        if(currentFloorNumber % 3 == 0)
        {
            isBossLvl = true;
        }
        else
        {
            isBossLvl = false;
        }
        var floorLayoutIndex = GetRandomInt(0, floorLayouts.transform.childCount - 1);
        var floorLayout = Instantiate(floorLayouts.transform.GetChild(floorLayoutIndex).gameObject);
        floorLayout.transform.position = floorGeneratingSpace.transform.position;
        var roomSpawnPoints = floorLayout.transform.Find("RoomSpawnPoints").gameObject;
        for(var i = 0; i < roomSpawnPoints.transform.childCount; i++)
        {
            var roomSpawnPoint = roomSpawnPoints.transform.GetChild(i).gameObject;
            if(roomSpawnPoint.GetComponent<RoomSpawnPoint>().roomType == RoomSpawnPoint.RoomType.Basic)
            {
                if (isBossLvl && roomSpawnPoint.GetComponent<RoomSpawnPoint>().isLastBasicOnFloor)
                {
                    var bossRoomIndex = GetRandomInt(0, bossRooms.transform.childCount - 1);
                    var bossRoom = Instantiate(bossRooms.transform.GetChild(bossRoomIndex).gameObject);
                    bossRoom.transform.position = roomSpawnPoint.transform.position;
                    bossRoom.transform.parent = currentFloorParent.transform;

                    DisableUseless(roomSpawnPoint, bossRoom);
                    GenerateLoot(bossRoom);
                    GenerateMonsters(bossRoom);
                }
                else
                {
                    var basicRoomIndex = GetRandomInt(0, basicRooms.transform.childCount - 1);
                    var basicRoom = Instantiate(basicRooms.transform.GetChild(basicRoomIndex).gameObject);
                    basicRoom.transform.position = roomSpawnPoint.transform.position;
                    basicRoom.transform.parent = currentFloorParent.transform;

                    DisableUseless(roomSpawnPoint, basicRoom);
                    GenerateLoot(basicRoom);
                    GenerateMonsters(basicRoom);
                }
            }
            else if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().roomType == RoomSpawnPoint.RoomType.StartRoom)
            {
                var startRoom1 = Instantiate(startRoom);
                startRoom1.transform.position = roomSpawnPoint.transform.position;
                startRoom1.transform.parent = currentFloorParent.transform;
                MovePlayerTo(startRoom1.transform.position);

                DisableUseless(roomSpawnPoint, startRoom1);
            }
            else if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().roomType == RoomSpawnPoint.RoomType.EndRoom)
            {
                var endRoom1 = Instantiate(endRoom);
                endRoom1.transform.position = roomSpawnPoint.transform.position;
                endRoom1.transform.parent = currentFloorParent.transform;

                DisableUseless(roomSpawnPoint, endRoom1);
            }
        }

        Destroy(floorLayout);
    }

    private void GenerateLoot(GameObject room)
    {
        var chest = room.GetComponent<RoomController>().chest;
        var currLootInChestCount = 0;
        var resultLoot = new List<GameObject>();
        while(currLootInChestCount < 4)
        {
            var isWillBeWithLoot = GetRandomInt(0, 100);
            if (isWillBeWithLoot < (currentFloorNumber * 5 + 20))
            {
                var isWeapon = GetRandomInt(0, 100);
                GameObject loot;
                if (isWeapon > 10)
                {
                    var weaponLootIndex = GetRandomInt(0, allLoot.transform.Find("Weapons").childCount - 1);
                    loot = allLoot.transform.Find("Weapons").GetChild(weaponLootIndex).gameObject;
                }
                else
                {
                    var flaskLootIndex = GetRandomInt(0, allLoot.transform.Find("Flasks").childCount - 1);
                    loot = allLoot.transform.Find("Flasks").GetChild(flaskLootIndex).gameObject;
                }
                resultLoot.Add(loot);
                currLootInChestCount++;
            }
            else
            {
                break;
            }
        }
        if(resultLoot.Count != 0)
        {
            chest.GetComponent<Chest>().Loot = resultLoot.ToArray();
        }

        chest.GetComponent<Chest>().Prepare();
    }

    private void GenerateMonsters(GameObject room)
    {
        var monstersSpawnPoints = room.transform.Find("Monsters_SpawnPoints").gameObject;
        var notMonsters = new List<GameObject>();
        var monstersSpawnPointsCount = monstersSpawnPoints.transform.childCount;
        for (var i = 0; i < monstersSpawnPointsCount; i++)
        {
            var isWillBeMonster = GetRandomInt(0, 100);
            if(isWillBeMonster < (currentFloorNumber * 5 + 20))
            {
                var pos = monstersSpawnPoints.transform.GetChild(i).position;
                var monsterIndex = GetRandomInt(0, allMonsters.transform.Find("SimpleMonsters").childCount - 1);
                var monster = Instantiate(allMonsters.transform.Find("SimpleMonsters").GetChild(monsterIndex).gameObject, monstersSpawnPoints.transform);
                monster.transform.position = new Vector3(pos.x, pos.y, -1);
            }
            notMonsters.Add(monstersSpawnPoints.transform.GetChild(i).gameObject);
        }

        foreach(var e in notMonsters)
        {
            Destroy(e);
        }
    }

    private void DisableUseless(GameObject roomSpawnPoint, GameObject room)
    {
        if (!roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageDown)
        {
            room.transform.Find("Passage_Down").Find("Door_Instead_Wall").gameObject.SetActive(false);
        }
        if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageDown)
        {
            room.transform.Find("Passage_Down").Find("Wall_Instead_Door").gameObject.SetActive(false);
        }

        if (!roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageUp)
        {
            room.transform.Find("Passage_Up").Find("Door_Instead_Wall").gameObject.SetActive(false);
        }
        if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageUp)
        {
            room.transform.Find("Passage_Up").Find("Wall_Instead_Door").gameObject.SetActive(false);
        }

        if (!roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageLeft)
        {
            room.transform.Find("Passage_Left").Find("Door_Instead_Wall").gameObject.SetActive(false);
        }
        if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageLeft)
        {
            room.transform.Find("Passage_Left").Find("Wall_Instead_Door").gameObject.SetActive(false);
        }

        if (!roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageRight)
        {
            room.transform.Find("Passage_Right").Find("Door_Instead_Wall").gameObject.SetActive(false);
        }
        if (roomSpawnPoint.GetComponent<RoomSpawnPoint>().hasPassageRight)
        {
            room.transform.Find("Passage_Right").Find("Wall_Instead_Door").gameObject.SetActive(false);
        }
    }

    private int GetRandomInt(int from, int to)
    {
        if (from == to)
            return from;
        var rnd = (int)Random.Range((float)from, (float)(to + 1));
        if(rnd == to + 1)
        {
            rnd = to;
        }
        return rnd;
    }

    private void MovePlayerTo(Vector3 coor)
    {
        player.transform.position = new Vector3(coor.x, coor.y, player.transform.position.z);
    }

    public GameObject GetCurrentFloorParent()
    {
        return currentFloorParent;
    }
}
