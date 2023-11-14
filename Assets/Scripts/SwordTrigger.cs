using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    public GameObject Sword;
    private List<GameObject> touchingEnemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Monster") && !Sword.GetComponent<Sword>().isMonsterWeapon)
        {
            if (!touchingEnemies.Contains(collision.gameObject))
            {
                touchingEnemies.Add(collision.gameObject);
            }
        }

        if (collision.tag.Equals("Player") && Sword.GetComponent<Sword>().isMonsterWeapon)
        {
            if (!touchingEnemies.Contains(collision.gameObject))
            {
                touchingEnemies.Add(collision.gameObject);
            }
        }
    }

    public List<GameObject> GetList()
    {
        return touchingEnemies;
    }

    public void ClearList()
    {
        touchingEnemies.Clear();
    }
}
