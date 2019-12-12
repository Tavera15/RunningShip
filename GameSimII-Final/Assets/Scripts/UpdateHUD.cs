using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHUD : MonoBehaviour
{
    public Text ScoreText;
    public Image timeImage;
    public GameObject wall;

    private WallOfDoom wallComp;

    // Start is called before the first frame update
    void Start()
    {
        wallComp = wall.GetComponent<WallOfDoom>();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = "Score: " + ScoreManager.instance.score;
        timeImage.fillAmount = (wallComp.timeToUpdateWorld - wallComp.wallTimer) / wallComp.timeToUpdateWorld;
    }
}
