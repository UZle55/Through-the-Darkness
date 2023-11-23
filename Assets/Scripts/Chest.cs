using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public int coinsCount = 7;
    public GameObject coinExample;
    private Tuple<GameObject, Vector3>[] coins;
    private bool isMovingCoins = false;
    private float movingTimes = 0;
    private float movingTimeBetween = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        movingTimeBetween += Time.deltaTime;
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

        if (isMovingCoins && movingTimeBetween > 0.25f)
        {
            if(movingTimes < 1)
            {
                MoveCoins();
            }
            if(movingTimes >= 1)
            {
                isMovingCoins = false;
                foreach (var coin in coins)
                {
                    coin.Item1.GetComponent<Coin>().FollowPlayer();
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
                isLootDropping = false;
            }
        }

        if (isLootDropped)
        {
            Destroy(GetComponent<BoxCollider2D>());
            Player.GetComponent<Player>().DeleteChestToOpen();
            isLootDropped = false;
        }
    }

    public void Prepare()
    {
        lootPointsToMove[0] = new Vector3(transform.position.x, transform.position.y + 1, -0.5f);
        lootPointsToMove[1] = new Vector3(transform.position.x + 1, transform.position.y, -0.5f);
        lootPointsToMove[2] = new Vector3(transform.position.x - 1, transform.position.y, -0.5f);
        lootPointsToMove[3] = new Vector3(transform.position.x, transform.position.y - 1, -0.5f);
        for (var i = 0; i < Loot.Length; i++)
        {
            Loot[i] = Instantiate(Loot[i], null);
            Loot[i].transform.position = new Vector3(1000, 1000, -0.5f);
        }

        gameObject.SetActive(false);
    }

    public void Open()
    {
        Player.GetComponent<Player>().DeleteChestToOpen();
        PlayAni();
    }

    private void DropLoot()
    {
        isLootDropping = true;
        SpawnCoins();

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

    private void SpawnCoins()
    {
        coins = new Tuple<GameObject, Vector3>[coinsCount];
        for(var i = 0; i < coinsCount; i++)
        {
            var coin = Instantiate(coinExample, null);
            coin.transform.position = new Vector3(transform.position.x, transform.position.y, -3);
            var throwVector = new Vector3(coin.transform.position.x + GetRandomInt(-75, 75), coin.transform.position.y + 25, coin.transform.position.z);
            var dir = throwVector - transform.position;
            coins[i] = Tuple.Create(coin, dir);
        }
        isMovingCoins = true;
        movingTimeBetween = 0;
        MoveCoins();
    }

    private void MoveCoins()
    {
        for (var i = 0; i < coinsCount; i++)
        {
            var dir = coins[i].Item2;
            var c = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            var coef = 500 / c;
            coins[i].Item1.GetComponent<Rigidbody2D>().AddForce(dir * coef);
        }
        movingTimeBetween = 0;
        movingTimes++;
    }

    private int GetRandomInt(int from, int to)
    {
        if (from == to)
            return from;
        var rnd = (int)Random.Range((float)from, (float)(to + 1));
        rnd = (int)Random.Range((float)from, (float)(to + 1));
        rnd = (int)Random.Range((float)from, (float)(to + 1));
        if (rnd == to + 1)
        {
            rnd = to;
        }
        return rnd;
    }
}
