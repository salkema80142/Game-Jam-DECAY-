using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class EndlessBackgroundGenerator : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.3f;

    [Header("Position Settings")]
    public float yOffset = 0f;
    public float zPosition = 5f;

    private Camera cam;
    private Sprite sprite;
    private float tileWidth;
    private float tileHeight;

    private List<Transform> tiles = new List<Transform>();

    void Start()
    {
        cam = Camera.main;
        sprite = GetComponent<SpriteRenderer>().sprite;

        if (cam == null || sprite == null)
        {
            Debug.LogError("Camera or background sprite is missing!");
            enabled = false;
            return;
        }

        CreateTiles();
    }

    void CreateTiles()
    {
        float cameraHeight = cam.orthographicSize * 2f;

        // Fit image to camera height while keeping its proportions.
        float scale = cameraHeight / sprite.bounds.size.y;

        tileWidth = sprite.bounds.size.x * scale;
        tileHeight = cameraHeight;

        float cameraWidth = cameraHeight * cam.aspect;

        // Create enough copies to cover the camera.
        int tileCount = Mathf.CeilToInt(
            cameraWidth / tileWidth
        ) + 3;

        for (int i = 0; i < tileCount; i++)
        {
            GameObject tile = new GameObject("Background Tile");

            SpriteRenderer renderer =
                tile.AddComponent<SpriteRenderer>();

            renderer.sprite = sprite;
            renderer.sortingLayerName = "Background";
            renderer.sortingOrder = -10;

            tile.transform.localScale = new Vector3(
                scale, scale, 1f
            );

            tiles.Add(tile.transform);
        }
    }

    void LateUpdate()
    {
        float cameraX = cam.transform.position.x;

        float cameraWidth =
            cam.orthographicSize * 2f * cam.aspect;

        // Calculate the background's parallax position.
        float backgroundOffset = cameraX * parallaxSpeed;

        float cameraLeft = cameraX - cameraWidth / 2f;

        float firstTileX =
            Mathf.Floor(
                (cameraLeft - backgroundOffset) / tileWidth
            ) * tileWidth + backgroundOffset;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].position = new Vector3(
                firstTileX + tileWidth * (i + 0.5f),
                cam.transform.position.y + yOffset,
                cam.transform.position.z + zPosition
            );
        }
    }
}