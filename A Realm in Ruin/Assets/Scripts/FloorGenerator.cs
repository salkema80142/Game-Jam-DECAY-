using UnityEngine;
using System.Collections.Generic;

public class FloorGenerator : MonoBehaviour
{
    [Header("Starting Position")]
    public Vector2 startingPosition = new Vector2(0, 0);

    [Header("Platform Settings")]
    public GameObject groundPrefab;
    public float platformWidth = 1f;
    public float gapBetweenPlatforms = 0f;

    [Header("Generation Settings")]
    public float spawnAheadDistance = 15f;
    public float destroyBehindDistance = 5f;

    private Camera mainCamera;
    private float nextSpawnX;

    private List<GameObject> platforms =
        new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;

        // Start at your chosen X position
        nextSpawnX = startingPosition.x;

        GeneratePlatforms();
    }

    void Update()
    {
        GeneratePlatforms();
        DestroyOldPlatforms();
    }

    void GeneratePlatforms()
    {
        float cameraRight = mainCamera.ViewportToWorldPoint(
            new Vector3(1, 0, 0)
        ).x;

        float spawnLimit = cameraRight + spawnAheadDistance;

        while (nextSpawnX < spawnLimit)
        {
            Vector3 spawnPosition = new Vector3(
                nextSpawnX + platformWidth / 2f,
                startingPosition.y,
                0
            );

            GameObject platform = Instantiate(
                groundPrefab,
                spawnPosition,
                Quaternion.identity
            );

            platforms.Add(platform);

            nextSpawnX += platformWidth + gapBetweenPlatforms;
        }
    }

    void DestroyOldPlatforms()
    {
        float cameraLeft = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        ).x;

        for (int i = platforms.Count - 1; i >= 0; i--)
        {
            if (platforms[i] == null)
            {
                platforms.RemoveAt(i);
                continue;
            }

            if (platforms[i].transform.position.x <
                cameraLeft - destroyBehindDistance)
            {
                Destroy(platforms[i]);
                platforms.RemoveAt(i);
            }
        }
    }
}
