using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameObject player;
    public GameObject levelsGenerator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPortalClick()
    {
        levelsGenerator.GetComponent<LevelsGenerator>().GenerateNextFloor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(player == null)
            {
                player = collision.gameObject;
            }
            player.GetComponent<Player>().TouchedPortal(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {

            player.GetComponent<Player>().StoppedTouchingPortal();
        }
    }
}
