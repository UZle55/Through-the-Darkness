using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSphere : MonoBehaviour
{
    public bool isMonsterSphere = false;
    private bool isGoing = false;
    private Vector2 dir;
    public float velocity = 0;
    public float damage = 0;
    public float maxDistance = 0;
    private Vector2 startPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGoing)
        {
            var dis = Monster.GetDistance(startPoint, transform.position);
            if(dis > maxDistance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void GoInDirection(Vector2 dir, bool isMonsterSphere, float velocity, float damage, float maxDistance)
    {
        this.dir = dir;
        this.isMonsterSphere = isMonsterSphere;
        this.velocity = velocity;
        this.damage = damage;
        isGoing = true;
        startPoint = transform.position;
        this.maxDistance = maxDistance;
        SetVelocity(dir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player") && isMonsterSphere)
        {
            collision.gameObject.GetComponent<Player>().GetDamage(damage);
            Destroy(this.gameObject);
        }
        else if(collision.tag.Equals("Monster") && !isMonsterSphere)
        {
            collision.gameObject.GetComponent<Monster>().GetDamage(damage);
            Destroy(this.gameObject);
        }
        else if (collision.tag.Equals("Wall"))
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag.Equals("OutsideWall"))
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag.Equals("ProjectileDestroyer") && isMonsterSphere != collision.gameObject.GetComponent<Weapon>().isMonsterWeapon)
        {
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
