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
    public int damage = 0;
    public int avgDamage = 0;
    public float maxDistance = 0;
    private Vector2 startPoint;
    public float criticalMultiplayer;
    public bool isCritical;

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

    public void GoInDirection(Vector2 dir, bool isMonsterSphere, float velocity, float damage, float maxDistance, bool isCrit, float critMulti)
    {
        this.dir = dir;
        this.isMonsterSphere = isMonsterSphere;
        this.velocity = velocity;
        avgDamage = Mathf.RoundToInt(damage);
        this.damage = avgDamage + GetRandomInt(-Mathf.RoundToInt(avgDamage * 0.2f), Mathf.RoundToInt(avgDamage * 0.2f));
        isGoing = true;
        startPoint = transform.position;
        this.maxDistance = maxDistance;
        isCritical = isCrit;
        criticalMultiplayer = critMulti;
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
            if (isCritical)
            {
                collision.gameObject.GetComponent<Monster>().GetDamage(Mathf.RoundToInt(damage * criticalMultiplayer), avgDamage, isCritical);
            }
            else
            {
                collision.gameObject.GetComponent<Monster>().GetDamage(damage, avgDamage, isCritical);
            }
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
        else if (collision.tag.Equals("Sword") && collision.gameObject != null 
            && isMonsterSphere != collision.transform.parent.parent.parent.gameObject.GetComponent<Weapon>().isMonsterWeapon)
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag.Equals("ProjectileDestroyer"))
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

    public int GetRandomInt(int from, int to)
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
