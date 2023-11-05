using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private enum direction
    {
        up,
        down,
        left,
        right,
        none
    }

    public bool isChasing = false;
    public int moveSpeed;
    private Vector2 moveVector = new Vector2(0, 0);
    public GameObject player;
    public float chasingDistance = 3f;
    private float distanceToPlayer = 0;
    private direction wallTouchDir;
    private GameObject wall;
    private bool isTouchingWall = false;
    private Vector2 firstDir;
    private float timeToContinue = 0.25f;
    private float t = 0f;
    public GameObject info;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        distanceToPlayer = Mathf.Sqrt(Mathf.Pow((transform.position.x - player.transform.position.x), 2) + Mathf.Pow((transform.position.y - player.transform.position.y), 2));

        if (!isChasing)
        {
            CheckPlayer();
        }
        else if(distanceToPlayer <= chasingDistance && distanceToPlayer > 1.5)
        {
            FollowPlayer();
        }
        else
        {
            isChasing = false;
            StopMoving();
        }
        
    }

    private void StopMoving()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2();
    }

    private void SetVelocity(Vector2 dir)
    {
        var c = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
        var coef = moveSpeed / c;
        GetComponent<Rigidbody2D>().velocity = dir * coef;
    }

    private void CheckPlayer()
    {
        var dir = (player.transform.position - transform.position) / 5;
        //Debug.DrawRay(transform.position, dir);
        var hit = Physics2D.Raycast(transform.position, dir, chasingDistance);
        if (hit.collider == null)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        else if (hit.collider.tag.Equals("Player"))
        {
            isChasing = true;
        }
    }

    private void FollowPlayer()
    {
        var dir = (player.transform.position - transform.position) / 5;
        var hit = Physics2D.Raycast(transform.position, dir, chasingDistance);
        if (timeToContinue < t && !isTouchingWall)
        {
            SetVelocity(dir);
        }
        else
        {
            SetVelocity(FindDirection());
        }
    }

    private Vector2 FindDirection()
    {
        if(wallTouchDir == direction.up || wallTouchDir == direction.down)
        {
            if(firstDir.x > 0)
            {
                return new Vector2(1, 0);
            }
            return new Vector2(-1, 0);
        }
        else
        {
            if(firstDir.y > 0)
            {
                return new Vector2(0, 1);
            }
            return new Vector2(0, -1);
        }
    }

    public void TouchedWall(string dir, GameObject wall)
    {
        firstDir = (player.transform.position - transform.position) / 5;
        isTouchingWall = true;
        this.wall = wall;
        if (dir.Equals("Up"))
        {
            wallTouchDir = direction.up;
        }
        if (dir.Equals("Down"))
        {
            wallTouchDir = direction.down;
        }
        if (dir.Equals("Left"))
        {
            wallTouchDir = direction.left;
        }
        if (dir.Equals("Right"))
        {
            wallTouchDir = direction.right;
        }

        //info.GetComponent<Text>().text += dir;
    }

    public void StopTouchingWall(string dir)
    {
        if ((dir.Equals("Up") && wallTouchDir == direction.up) || (dir.Equals("Down") && wallTouchDir == direction.down)
            || (dir.Equals("Left") && wallTouchDir == direction.left) || (dir.Equals("Right") && wallTouchDir == direction.right))
        {
            t = 0;
            isTouchingWall = false;
            wall = null;
            //info.GetComponent<Text>().text += "none";
        }
    }
}
