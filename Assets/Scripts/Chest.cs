using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedChestSprite;
    public Sprite[] openingChestSprites;
    public bool isPlayerCanOpen;
    private GameObject Player;
    private bool isPlayingAni = false;
    private float t = 0;
    private int aniSpriteIndex = 0;
    private bool isLootDropped = false;
    private bool isLootDropping = false;
    public GameObject[] Loot; //максимум 4 предмета
    private Vector3[] lootPointsToMove = new Vector3[4];
    // Start is called before the first frame update
    void Start()
    {
        lootPointsToMove[0] = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        lootPointsToMove[1] = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        lootPointsToMove[2] = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        lootPointsToMove[3] = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        for (var i = 0; i < Loot.Length; i++)
        {
            Loot[i] = Instantiate(Loot[i], null);
            Loot[i].transform.position = new Vector3(1000, 1000, -1);
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (isPlayingAni)
        {
            if(aniSpriteIndex < openingChestSprites.Length)
            {
                if(t > 0.1f)
                {
                    t = 0;
                    GetComponent<SpriteRenderer>().sprite = openingChestSprites[aniSpriteIndex];
                    aniSpriteIndex++;
                    if(aniSpriteIndex == openingChestSprites.Length)
                    {
                        DropLoot();
                    }
                }
            }
        }

        if (isLootDropping)
        {
            var isAllDropped = true;
            for(var i = 0; i < Loot.Length; i++)
            {
                Loot[i].transform.position = Vector3.Lerp(transform.position, lootPointsToMove[i], t);
                if (Mathf.Abs(Loot[i].transform.position.x - lootPointsToMove[i].x) > 0.001f 
                    || Mathf.Abs(Loot[i].transform.position.y - lootPointsToMove[i].y) > 0.001f)
                {
                    isAllDropped = false;
                }
            }
            if (isAllDropped)
            {
                for (var i = 0; i < Loot.Length; i++)
                {
                    if (Loot[i].tag.Equals("Weapon"))
                    {
                        Loot[i].tag = "WeaponOnGround";
                    }
                    if (Loot[i].tag.Equals("Flask"))
                    {
                        Loot[i].tag = "FlaskOnGround";
                    }
                }
                isLootDropped = true;
            }
        }

        if (isLootDropped)
        {
            Destroy(this.gameObject);
        }
    }

    public void Open()
    {
        Player.GetComponent<Player>().DeleteChestToOpen();
        PlayAni();
    }

    private void DropLoot()
    {
        isLootDropping = true;

    }

    private void PlayAni()
    {
        isPlayingAni = true;
        t = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && !isPlayingAni)
        {
            Player = collision.gameObject;
            Player.GetComponent<Player>().SetChestToOpen(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && !isPlayingAni)
        {
            Player.GetComponent<Player>().DeleteChestToOpen();
        }
    }
}
