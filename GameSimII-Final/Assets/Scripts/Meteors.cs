using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteors : MonoBehaviour
{
    public GameObject wall;
    public int scoreWorth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2.MoveTowards(this.transform.position, wall.transform.position, 20);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            if(GameObject.Find("Rocket") != null)
                ScoreManager.instance.IncrementScore(scoreWorth);

            Destroy(gameObject);
        }
    }

    
}
