using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    private float t = 0;
    
    private float attackCooldown;
    public int startAngle;
    public int endAngle;
    private float aniDuration;
    private float angle;
    private int direction = 1;
    private bool isPlayingAni = false;
    public float aniDownTime = 0.2f;
    public float aniUpTime = 0.3f;
    private bool isSwordGoingDown = false;
    private bool isSwordGoingUp = false;
    private int hittedEnemiesCount = 0;
    private List<GameObject> touchingEnemies = new List<GameObject>();
    public float knockBackForce = 0;

    public GameObject RotatingPoint;
    public GameObject RotatingAniPoint;
    public GameObject SwordTrigger;
    // Start is called before the first frame update
    void Start()
    {
        aniDuration = aniDownTime + aniUpTime;
        attackCooldown = 1 / attackSpeed;
        RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, startAngle);

        touchingEnemies = SwordTrigger.GetComponent<SwordTrigger>().GetList();
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        RotateWeapon();

        if (isPlayingAni)
        {
            PlayAni();
        }

        if (!isMonsterWeapon)
        {
            position = SwordTrigger.transform.position;
        }


        if (isSwordGoingDown && touchingEnemies.Count > hittedEnemiesCount)
        {
            for(var i = hittedEnemiesCount; i < touchingEnemies.Count; i++)
            {
                HitEnemy(touchingEnemies[hittedEnemiesCount]);
                hittedEnemiesCount++;
            }
        }
        if (!isSwordGoingDown && touchingEnemies.Count != 0)
        {
            SwordTrigger.GetComponent<SwordTrigger>().ClearList();
            hittedEnemiesCount = 0;
        }
    }

    public override bool Attack()
    {
        if(t > attackCooldown)
        {
            t = 0;
            PlayAni();
            return true;
        }
        return false;
    }

    public override void SetAngle(float angle)
    {
        if (!isPlayingAni)
        {
            this.angle = angle;
            if (angle < 0 && direction == -1)
            {
                direction = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (angle > 0 && direction == 1)
            {
                direction = -1;
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        
    }

    private void RotateWeapon()
    {
        RotatingPoint.transform.localEulerAngles = new Vector3(0, 0, angle * direction);
        
        //RotatingPoint.transform.localEulerAngles = new Vector3(0, 0, angle);

    }

    private void PlayAni()
    {
        if (!isPlayingAni)
        {
            isPlayingAni = true;
        }
        if(!isSwordGoingDown && t < aniDownTime)
        {
            isSwordGoingDown = true;
            SwordTrigger.tag = "Sword";
        }
        if (isSwordGoingDown)
        {
            var k = t / aniDownTime;
            var a = ((endAngle - startAngle) * k) + startAngle;
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if(t >= aniDownTime)
            {
                isSwordGoingUp = true;
                isSwordGoingDown = false;
                SwordTrigger.tag = "Untagged";
            }
        }
        if (isSwordGoingUp)
        {
            var k = (t - aniDownTime) / aniUpTime;
            var a = endAngle - ((endAngle - startAngle) * k);
            RotatingAniPoint.transform.localEulerAngles = new Vector3(0, 0, a);
            if(t >= aniDownTime + aniUpTime)
            {
                isSwordGoingUp = false;
                isPlayingAni = false;
            }
        }

    }

    public void HitEnemy(GameObject enemy)
    {
        if (isSwordGoingDown)
        {
            if (!isMonsterWeapon)
            {
                var dir = (enemy.transform.position - transform.parent.position) / 5;
                var hits = Physics2D.RaycastAll(transform.parent.position, dir, 5);
                foreach (var hit in hits)
                {
                    if (hit.collider.tag.Equals("Wall"))
                    {
                        break;
                    }
                    if (hit.collider.tag.Equals("Monster"))
                    {
                        enemy.GetComponent<Monster>().GetDamage(damage);
                        if(knockBackForce != 0)
                        {
                            enemy.GetComponent<Monster>().KnockBack(knockBackForce);
                        }
                        
                        break;
                    }
                }
            }
            else if (isMonsterWeapon)
            {
                enemy.GetComponent<Player>().GetDamage(damage);
            }
        }
    }
}
