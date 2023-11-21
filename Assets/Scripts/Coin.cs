using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float velocity = 5;
    private bool isFollowing = false;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing)
        {
            var dir = (player.transform.position - transform.position) / 5;
            SetVelocity(dir);
        }
    }

    public void FollowPlayer()
    {
        isFollowing = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && isFollowing)
        {
            player.GetComponent<Player>().ChangeCoinsCount(1);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && isFollowing)
        {
            player.GetComponent<Player>().ChangeCoinsCount(1);
            Destroy(this.gameObject);
        }
    }

    private void SetVelocity(Vector2 dir)
    {
        var c = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
        var coef = velocity / c;
        GetComponent<Rigidbody2D>().velocity = dir * coef;
        //info.GetComponent<Text>().text = "c: " + c + "  coef: " + coef + "  vel: " + GetComponent<Rigidbody2D>().velocity.ToString();

    }
}
