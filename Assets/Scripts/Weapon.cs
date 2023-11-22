using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Weapon : MonoBehaviour
    {
        public enum Type
        {
            Sword,
            Staff,
            Bow
        }
        public float damage;
        public bool isMonsterWeapon = false;
        public float attackSpeed;
        public int attackManaCost;
        public Vector2 position;
        public string[] stats;
        public string weaponName;
        public float criticalChance = 5;
        public float criticalMultiplayer = 2;
        public Type type;
        public virtual bool Attack()
        {
            return false;
        }

        public virtual void SetAngle(float angle)
        {

        }

        public string GetStatsText()
        {
            var result = "";
            for(var i = 0; i < stats.Length; i++)
            {
                result += stats[i];
                if(i != stats.Length - 1)
                {
                    result += "\n";
                }
                
            }
            return result;
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
}
