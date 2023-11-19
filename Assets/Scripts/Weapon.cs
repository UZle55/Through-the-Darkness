using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Weapon : MonoBehaviour
    {
        public float damage;
        public bool isMonsterWeapon = false;
        public float attackSpeed;
        public int attackManaCost;
        public Vector2 position;
        public virtual bool Attack()
        {
            return false;
        }

        public virtual void SetAngle(float angle)
        {

        }
    }
}
