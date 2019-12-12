using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WallInfo
{
    public Vector3 position;
    public float mWidth;
    public float mHeight;
    public Vector2 mGravity;
    public Vector3 mPlayerRotation;
    public Vector3 mWallScale;

    public WallInfo(Vector3 pos, float width, float height, Vector2 gravity, Vector3 playerRotation)
    {
        mWidth = width;
        mHeight = height;
        mWallScale = new Vector3(width, height, 1f);

        position = pos;
        mGravity = gravity;
        mPlayerRotation = playerRotation;
    }

    public bool matches(WallInfo other)
    {
        return (position.x == other.position.x && position.y == other.position.y && position.z == other.position.z);
    }
}

public class WallOfDoom : MonoBehaviour
{   
    public Camera cam;
    public GameObject rocket;
    public float wallThickness;
    public float gravity;
    public float durationBetweenMeteorSpawns = 2;
    public float timeToUpdateWorld = 10;
    public GameObject[] meteors;
    public float gravityLerp = 50;
    public float scaleLerp = 5;
    public float lerpTime = 0;
    public Canvas GameOverCanvas;

    WallInfo currentWall;

    List<WallInfo> possibleWalls = new List<WallInfo>();
    WallInfo top;
    WallInfo bottom;
    WallInfo left;
    WallInfo right;

    private float meteorTimer = 0;
    public float wallTimer = 0;
    private bool isUpdatingWorld = false;

    // Start is called before the first frame update
    void Start()
    {
        var topWall = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, cam.pixelHeight, 1));
        var bottomWall = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, 1));
        var leftWall = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight / 2, 1));
        var rightWall = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, 1));

        float wallWidth = 2.81f;      // (float)(cam.pixelWidth / 9.46);
        float wallHeight = 2.1f;      // (float)(cam.pixelHeight / 9.57);

        top = new WallInfo(topWall, wallWidth, wallThickness, new Vector2(0, gravity), new Vector3(0, 0, -180));
        bottom = new WallInfo(bottomWall, wallWidth, wallThickness, new Vector2(0, -gravity), new Vector3(0, 0, 0));
        left = new WallInfo(leftWall, wallThickness, wallHeight, new Vector2(-gravity, 0), new Vector3(0, 0, -90));
        right = new WallInfo(rightWall, wallThickness, wallHeight, new Vector2(gravity, 0), new Vector3(0, 0, 90));

        possibleWalls.Add(top);
        possibleWalls.Add(bottom);
        possibleWalls.Add(left);
        possibleWalls.Add(right);

        Physics2D.gravity = new Vector2(0, 0);

        currentWall = left;
        transform.localPosition = currentWall.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        rocket.transform.rotation = Quaternion.Euler(currentWall.mPlayerRotation);

        isUpdatingWorld = true;
    }

    // Update is called once per frame
    void Update()
    {
        meteorTimer += Time.deltaTime;

        if (!isUpdatingWorld && rocket)
        {
            wallTimer += Time.deltaTime;
        }

        if(meteorTimer >= durationBetweenMeteorSpawns && !isUpdatingWorld)
        {
            var spawnPos = new Vector3(0,0,0);

            if (currentWall.matches(top))
                spawnPos = new Vector3(Random.Range(-10, 10), bottom.position.y, 0);
            else if (currentWall.matches(bottom))
                spawnPos = new Vector3(Random.Range(-10, 10), top.position.y, 0);
            else if (currentWall.matches(left))
                spawnPos = new Vector3(right.position.x, Random.Range(-5, 5), 0);
            else
                spawnPos = new Vector3(left.position.x, Random.Range(-5, 5), 0);

            int randomMeteor = Random.Range(0, meteors.Length);

            Instantiate(meteors[randomMeteor], spawnPos, Quaternion.Euler(new Vector3(0,0,0)));
            meteorTimer = 0;
        }

        if(wallTimer >= timeToUpdateWorld && !isUpdatingWorld && rocket)
        {
            if (isDoneDisplaying(new Vector2(0, 0), new Vector3(0, 0, 0)))
            {
                UpdateTheWall();
                isUpdatingWorld = true;
                lerpTime = 0;
            }
        }

        if(isUpdatingWorld && rocket)
        {
            if(isDoneDisplaying(currentWall.mGravity, currentWall.mWallScale))
            {
                wallTimer = 0;
                meteorTimer = 0;
                isUpdatingWorld = false;
                lerpTime = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != rocket) { return; }

        Destroy(collision.gameObject);
        GameOverCanvas.enabled = true;
    }

    WallInfo getRandomWallPosition()
    {
        int r = Random.Range(0, possibleWalls.Count);

        return possibleWalls[r];
    }

    void UpdateTheWall()
    {
        if (!rocket) { return; }

        currentWall = getRandomWallPosition();

        transform.localPosition = currentWall.position;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,0));

        rocket.transform.rotation = Quaternion.Euler(currentWall.mPlayerRotation);
    }

    bool isDoneDisplaying(Vector2 gravityGoal, Vector3 localScaleGoal)
    {
        lerpTime += (float)(Time.deltaTime);

        Physics2D.gravity = Vector2.Lerp(Physics2D.gravity, gravityGoal, gravityLerp * lerpTime);
        transform.localScale = Vector3.Lerp(transform.localScale, localScaleGoal, scaleLerp * lerpTime);

        return (lerpTime > 1.5);
    }
}
