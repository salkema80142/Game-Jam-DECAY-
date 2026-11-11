using UnityEngine;
using System.Collections.Generic;

public class EndlessBackgroundGenerator : MonoBehaviour
{
    [Header("Background Settings")]
    public GameObject backgroundPrefab;

    [Header("Tile Size")]
    public float tileWidth = 20f;
    public float tileHeight = 12f;
    public float backgroundY = 0f;

    [Header("Generation Settings")]
    public float spawnAheadDistance = 30f;
    public float destroyBehindDistance = 20f;

    private Camera mainCamera;
    private float nextSpawnX;

    private List<GameObject> backgroundTiles =
        new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;

        nextSpawnX = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        ).x;

        GenerateBackground();
    }

    void Update()
    {
        GenerateBackground();
        DestroyOldBackground();
    }

    void GenerateBackground()
    {
        float cameraRight = mainCamera.ViewportToWorldPoint(
            new Vector3(1, 0, 0)
        ).x;

        float spawnLimit = cameraRight + spawnAheadDistance;

        while (nextSpawnX < spawnLimit)
        {
            Vector3 position = new Vector3(
                nextSpawnX + tileWidth / 2f,
                backgroundY,
                5f
            );

            GameObject tile = Instantiate(
                backgroundPrefab,
                position,
                Quaternion.identity
            );

            tile.transform.localScale = new Vector3(
                tileWidth,
                tileHeight,
                1f
            );

            backgroundTiles.Add(tile);

            nextSpawnX += tileWidth;
        }
    }

    void DestroyOldBackground()
    {
        float cameraLeft = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        ).x;

        for (int i = backgroundTiles.Count - 1; i >= 0; i--)
        {
            if (backgroundTiles[i] == null)
            {
                backgroundTiles.RemoveAt(i);
                continue;
            }

            if (backgroundTiles[i].transform.position.x <
                cameraLeft - destroyBehindDistance)
            {
                Destroy(backgroundTiles[i]);
                backgroundTiles.RemoveAt(i);
            }
        }
    }
}
