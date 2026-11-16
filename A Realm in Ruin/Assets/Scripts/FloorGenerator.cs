using UnityEngine;
using System.Collections.Generic;

public class FloorGenerator : MonoBehaviour
{
    [Header("Ground")]
    public GameObject groundPrefab;
    public float groundWidth = 5f;
    public float groundY = 0f;

    [Header("Parkour Platforms")]
    public GameObject platformPrefab;
    public float platformWidth = 3f;
    public float platformHeight = 1.2f;

    [Header("Slide Obstacles")]
    public GameObject slideObstaclePrefab;
    public float obstacleY = 1.5f;

    [Header("Course Settings")]
    public int sectionLength = 5;
    public float gapSize = 5f;
    public float spawnAheadDistance = 35f;
    public float destroyBehindDistance = 20f;

    [Header("Starting Position")]
    public Vector2 startingPosition = Vector2.zero;

    private Camera mainCamera;
    private float nextSpawnX;
    private int lastSection = -1;

    private List<GameObject> spawnedObjects =
        new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null || groundPrefab == null)
        {
            Debug.LogError("Assign the ground prefab and Main Camera!");
            enabled = false;
            return;
        }

        nextSpawnX = startingPosition.x;

        // Create a safe starting platform.
        for (int i = 0; i < 4; i++)
        {
            SpawnGround(nextSpawnX);
            nextSpawnX += groundWidth;
        }

        // Start generating parkour after the starting platform.
        GenerateCourse();
    }


    void Update()
    {
        GenerateCourse();
        DestroyOldObjects();
    }

    void GenerateCourse()
    {
        float cameraRight = mainCamera.ViewportToWorldPoint(
            new Vector3(1, 0, 0)
        ).x;

        float spawnLimit = cameraRight + spawnAheadDistance;

        while (nextSpawnX < spawnLimit)
        {
            // Pick a new course section.
            int section = Random.Range(0, 4);

            // Avoid repeating the same section.
            if (section == lastSection)
                section = (section + 1) % 4;

            lastSection = section;

            switch (section)
            {
                case 0:
                    GenerateNormalSection();
                    break;

                case 1:
                    GenerateJumpSection();
                    break;

                case 2:
                    GeneratePlatformSection();
                    break;

                case 3:
                    GenerateSlideSection();
                    break;
            }
        }
    }

    // NORMAL RUNNING SECTION
    void GenerateNormalSection()
    {
        for (int i = 0; i < sectionLength; i++)
        {
            SpawnGround(nextSpawnX);
            nextSpawnX += groundWidth;
        }
    }

    // JUMP OVER A GAP
    void GenerateJumpSection()
    {
        SpawnGround(nextSpawnX);

        nextSpawnX += groundWidth;

        // Leave a gap with no ground.
        nextSpawnX += gapSize;

        // Spawn landing ground.
        for (int i = 0; i < 3; i++)
        {
            SpawnGround(nextSpawnX);
            nextSpawnX += groundWidth;
        }
    }

    // RAISED PARKOUR PLATFORMS
    void GeneratePlatformSection()
    {
        SpawnGround(nextSpawnX);

        nextSpawnX += groundWidth;

        for (int i = 0; i < 4; i++)
        {
            float height = groundY +
                platformHeight * (i % 2 == 0 ? 1 : 2);

            SpawnPlatform(
                nextSpawnX + platformWidth / 2f,
                height
            );

            nextSpawnX += platformWidth + 1f;
        }

        // Safe landing ground.
        SpawnGround(nextSpawnX);
        nextSpawnX += groundWidth;
    }

    // SLIDE UNDER OBSTACLES
    void GenerateSlideSection()
    {
        for (int i = 0; i < sectionLength; i++)
        {
            SpawnGround(nextSpawnX);

            if (i == 2 && slideObstaclePrefab != null)
            {
                SpawnObstacle(
                    nextSpawnX + groundWidth / 2f
                );
            }

            nextSpawnX += groundWidth;
        }
    }

    void SpawnGround(float x)
    {
        Vector3 position = new Vector3(
            x + groundWidth / 2f,
            groundY,
            0f
        );

        GameObject ground = Instantiate(
            groundPrefab,
            position,
            Quaternion.identity
        );

        spawnedObjects.Add(ground);
    }

    void SpawnPlatform(float x, float y)
    {
        if (platformPrefab == null)
            return;

        Vector3 position = new Vector3(x, y, 0f);

        GameObject platform = Instantiate(
            platformPrefab,
            position,
            Quaternion.identity
        );

        spawnedObjects.Add(platform);
    }

    void SpawnObstacle(float x)
    {
        Vector3 position = new Vector3(
            x,
            obstacleY,
            0f
        );

        GameObject obstacle = Instantiate(
            slideObstaclePrefab,
            position,
            Quaternion.identity
        );

        spawnedObjects.Add(obstacle);
    }

    void DestroyOldObjects()
    {
        float cameraLeft = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        ).x;

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            if (spawnedObjects[i].transform.position.x <
                cameraLeft - destroyBehindDistance)
            {
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
