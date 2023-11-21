using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWallChecker : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Wall"))
        {
            transform.parent.GetComponent<Monster>().TouchedWall(name.Split("_")[1], collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Wall"))
        {
            transform.parent.GetComponent<Monster>().StopTouchingWall(name.Split("_")[1]);
        }
        
    }
}
