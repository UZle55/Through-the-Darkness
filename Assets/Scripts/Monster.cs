using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public enum Direction
    {
        up,
        down,
        left,
        right,
        none
    }

    public enum AttackType
    {
        melee,
        distance
    }

    public float expRewardOnKill;
    public float flaskChargesRewardOnKill = 3;
    public bool isChasing = false;
    private bool isStopMoving = false;
    public int moveSpeed;
    private Vector2 moveVector = new Vector2(0, 0);
    public GameObject player;
    public float chasingDistance = 3f;
    private float distanceToPlayer = 0;
    private Direction wallTouchDir;
    private GameObject wall;
    private bool isTouchingWall = false;
    private Vector2 firstDir;
    private float timeToContinue = 0.25f;
    private float timeEndedTouchingWall = 5f;
    private float timeCanGoAfterWall = 0.25f;
    public GameObject info;
    public AttackType attackType;
    public GameObject Weapon;

    public GameObject Rot;
    public GameObject Direction1;
    public GameObject Direction2;
    private bool isGoingAround = false;
    private float timeGoingAround = 0;
    private GameObject chosenDirection = null;
    private float attackDelayTime = 0;
    private float attackTime = 0;

    public float HP;
    public Slider HealthBar;
    private float maxHP;
    private float currHP;
    private float Damage;
    private float AttackSpeed;
    public float closeStopDistance = 1.25f;
    public float attackDistance = 2f;

    public bool isSeePlayer = false;
    public bool isActive = false;

    private bool isKnockingBack = false;
    private float durationKnockingBack = 0;
    public bool canKnockingBack = true;

    private bool isGoingStraight = false;
    private float timeGoingStraight = 0;
    private bool[] isGoingAroundArray = new bool[3];
    private float[] goingAroundTimers = new float[3];

    public GameObject damageMarker;
    // Start is called before the first frame update
    void Start()
    {
        AttackSpeed = Weapon.GetComponent<Weapon>().attackSpeed;
        maxHP = HP;
        currHP = HP;
        attackDelayTime = 1 / AttackSpeed;
        Damage = Weapon.GetComponent<Weapon>().damage;
        Weapon.GetComponent<Weapon>().isMonsterWeapon = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isActive && !isKnockingBack)
        {
            attackTime += Time.deltaTime;
            timeEndedTouchingWall += Time.deltaTime;

            if (isGoingAround)
            {
                timeGoingAround += Time.deltaTime;
                if (timeGoingAround > 0.5f)
                {
                    isGoingAround = false;
                    chosenDirection = null;
                    timeGoingAround = 0;
                    var isAnyFalse = false;
                    for (var i = 0; i < isGoingAroundArray.Length; i++)
                    {

                        if (!isGoingAroundArray[i])
                        {
                            isAnyFalse = true;
                            isGoingAroundArray[i] = true;
                            break;
                        }
                    }
                    if (!isAnyFalse)
                    {
                        var max = -1f;
                        var indexToUpdate = -1;
                        for (var q = 0; q < goingAroundTimers.Length; q++)
                        {
                            if (goingAroundTimers[q] > max)
                            {
                                indexToUpdate = q;
                            }
                        }
                        goingAroundTimers[indexToUpdate] = 0;
                    }
                }
            }

            isGoingStraight = false;
            var count = 0;
            for (var i = 0; i < isGoingAroundArray.Length; i++)
            {
                if (isGoingAroundArray[i])
                {
                    count++;
                    goingAroundTimers[i] += Time.deltaTime;
                    if (goingAroundTimers[i] > 5f)
                    {
                        goingAroundTimers[i] = 0;
                        isGoingAroundArray[i] = false;
                        count--;
                    }
                }
            }
            if (count == 3)
            {
                isGoingStraight = true;
            }

            if (isGoingStraight)
            {
                timeGoingStraight += Time.deltaTime;
                var dir = (player.transform.position - transform.position) / 5;
                SetVelocity(dir);
            }


            distanceToPlayer = GetDistance(transform.position, player.transform.position);

            if (!isChasing)
            {
                CheckPlayer();
            }
            else if (distanceToPlayer > closeStopDistance || !isSeePlayer)
            {
                FollowPlayer();
                isStopMoving = false;
            }
            else
            {
                isStopMoving = true;
                StopMoving();
            }

            if (isChasing && distanceToPlayer < attackDistance && attackTime > attackDelayTime)
            {
                isSeePlayer = false;
                var dir = (player.transform.position - transform.position) / 5;
                var hits = Physics2D.RaycastAll(transform.position, dir, 20);
                foreach (var hit in hits)
                {
                    if (hit.collider.tag.Equals("Wall"))
                    {
                        isSeePlayer = false;
                        break;
                    }
                    if (hit.collider.tag.Equals("Player"))
                    {
                        isSeePlayer = true;
                        break;
                    }
                }

                if (isSeePlayer)
                {
                    Weapon.GetComponent<Weapon>().Attack();
                    attackTime = 0;
                }

            }

            RotateRot();

        }
        else if (isKnockingBack && isActive)
        {
            durationKnockingBack += Time.deltaTime;
            if (durationKnockingBack > 0.25f)
            {
                durationKnockingBack = 0;
                isKnockingBack = false;
            }
        }
    }

    private void Attack()
    {

    }

    public void GetDamage(int damage, int avgDamage, bool isCrit)
    {
        currHP -= damage;
        damageMarker.GetComponent<DamageMarker>().ShowDamage(damage, avgDamage, isCrit);
        if(currHP <= 0)
        {
            Die();
        }
        else
        {
            var value = currHP / maxHP;
            HealthBar.value = value;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        player.GetComponent<Player>().GetExp(expRewardOnKill);
        player.GetComponent<Player>().AddCharges(flaskChargesRewardOnKill);
    }

    private void RotateRot()
    {
        var x1 = player.transform.position.x;
        var y1 = player.transform.position.y;
        var x2 = transform.position.x;
        var y2 = transform.position.y;
        var len = Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        var dx = x2 - x1;
        var dy = y2 - y1;
        var arcsin = Mathf.Asin(dx / len);
        var angle = arcsin * 180 / Mathf.PI;
        if(angle > 0)
        {
            if(dy > 0)
            {
                angle = 180 - angle;
            }
        }
        else
        {
            if(dy > 0)
            {
                angle = -180 - angle;
            }
        }
        if(angle == float.NaN)
        {
            var a = 0;
        }
        Rot.transform.localEulerAngles = new Vector3(0, 0, angle);
        Weapon.GetComponent<Weapon>().SetAngle(angle);
    }

    private void StopMoving()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2();
    }

    private void SetVelocity(Vector2 dir)
    {
        var c = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
        var coef = moveSpeed / c;
        GetComponent<Rigidbody2D>().AddForce(dir * coef);
        //info.GetComponent<Text>().text = "c: " + c + "  coef: " + coef + "  vel: " + GetComponent<Rigidbody2D>().velocity.ToString();

    }

    private void CheckPlayer()
    {
        var dir = (player.transform.position - transform.position) / 5;
        //Debug.DrawRay(transform.position, dir);
        var hits = Physics2D.RaycastAll(transform.position, dir, chasingDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                break;
            }
            if (hit.collider.tag.Equals("Player"))
            {
                isChasing = true;
                break;
            }
        }
    }

    private void FollowPlayer()
    {
        var pos1 = transform.position - new Vector3(0.45f, 0, 0);
        var pos2 = transform.position - new Vector3(-0.45f, 0, 0);
        var pos3 = transform.position - new Vector3(0, 0.45f, 0);
        var pos4 = transform.position - new Vector3(0, -0.45f, 0);
        var dir1 = (player.transform.position - pos1) / 5;
        var dir2 = (player.transform.position - pos2) / 5;
        var dir3 = (player.transform.position - pos3) / 5;
        var dir4 = (player.transform.position - pos4) / 5;
        var arrPos = new Vector3[] { pos1, pos2, pos3, pos4 };
        var arrDir = new Vector3[] { dir1, dir2, dir3, dir4 };
        isSeePlayer = true;
        var isMonsterAhead = false;
        var monsterAheadPos = new Vector2(0, 0);
        if (!isGoingAround)
        {
            for (var i = 0; i < 4; i++)
            {
                var hits = Physics2D.RaycastAll(arrPos[i], arrDir[i], chasingDistance);
                foreach (var hit in hits)
                {
                    if (isSeePlayer && hit.collider.tag.Equals("Wall"))
                    {
                        isSeePlayer = false;
                        break;
                    }
                    if (!isMonsterAhead && hit.collider.tag.Equals("Monster") && !hit.collider.gameObject.Equals(gameObject))
                    {
                        var distance = GetDistance(hit.collider.transform.position, transform.position);
                        if (distance < 1.25f)
                        {
                            isMonsterAhead = true;
                            monsterAheadPos = hit.transform.position;
                            break;
                        }
                    }
                    if (hit.collider.tag.Equals("Player"))
                    {
                        break;
                    }
                }
            }  
        }
        var dir = (player.transform.position - transform.position) / 5;
        if (!isGoingStraight)
        {
            if (!isTouchingWall && !isMonsterAhead && !isGoingAround && timeEndedTouchingWall > timeCanGoAfterWall)
            {
                SetVelocity(dir);
            }
            else if ((!isTouchingWall && isMonsterAhead) || isGoingAround)
            {

                isGoingAround = true;
                SetVelocity(GetDirAround(monsterAheadPos));
            }
            else if (!isGoingAround && (isTouchingWall || timeEndedTouchingWall < timeCanGoAfterWall))
            {
                SetVelocity(FindDirection());
            }
        }
    }

    private Vector2 FindDirection()
    {
        if(wallTouchDir == Direction.up || wallTouchDir == Direction.down)
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
        if(timeEndedTouchingWall > timeCanGoAfterWall)
        {
            firstDir = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
            isTouchingWall = true;
            this.wall = wall;
            if (dir.Equals("Up"))
            {
                wallTouchDir = Direction.up;
            }
            if (dir.Equals("Down"))
            {
                wallTouchDir = Direction.down;
            }
            if (dir.Equals("Left"))
            {
                wallTouchDir = Direction.left;
            }
            if (dir.Equals("Right"))
            {
                wallTouchDir = Direction.right;
            }
        }
        

        //info.GetComponent<Text>().text += dir;
    }

    public void StopTouchingWall(string dir)
    {
        if ((dir.Equals("Up") && wallTouchDir == Direction.up) || (dir.Equals("Down") && wallTouchDir == Direction.down)
            || (dir.Equals("Left") && wallTouchDir == Direction.left) || (dir.Equals("Right") && wallTouchDir == Direction.right))
        {
            timeEndedTouchingWall = 0;
            isTouchingWall = false;
            wall = null;
            //info.GetComponent<Text>().text += "none";
        }
    }

    private Vector2 GetDirAround(Vector2 monsterAheadPos)
    {
        var dir = new Vector3();
        if (chosenDirection == null)
        {
            var resX1 = Direction1.transform.position.x;
            var resX2 = Direction2.transform.position.x;
            var resY1 = Direction1.transform.position.y;
            var resY2 = Direction2.transform.position.y;

            var lenToMonster1 = GetDistance(Direction1.transform.position, monsterAheadPos);
            var lenToMonster2 = GetDistance(Direction2.transform.position, monsterAheadPos);
            
            if (lenToMonster1 > lenToMonster2)
            {
                dir = (new Vector3(resX1, resY1, transform.position.z) - transform.position) / 5;
                chosenDirection = Direction1;
                return dir;
            }
            dir = (new Vector3(resX2, resY2, transform.position.z) - transform.position) / 5;
            chosenDirection = Direction2;
            return dir;
        }
        dir = (new Vector3(chosenDirection.transform.position.x, chosenDirection.transform.position.y, transform.position.z) - transform.position) / 5;
        return dir;
    }

    public void KnockBack(float force)
    {
        if (canKnockingBack)
        {
            isKnockingBack = true;
            var dir = (transform.position - player.transform.position);
            var c = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            var coef = force / c;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            GetComponent<Rigidbody2D>().AddForce(dir * coef);

        }
    }

    public static float GetDistance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y));
    }
}
