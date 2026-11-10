using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [Header("Ground Settings")]
    public GameObject groundPrefab;

    public int numberOfPlatforms = 30;

    public float platformWidth = 1f;

    public float gapBetweenPlatforms = 0f;

    [Header("Starting Position")]
    public Vector2 startPosition;

    void Start()
    {
        GenerateFloor();
    }

    void GenerateFloor()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            float xPosition = startPosition.x +
                i * (platformWidth + gapBetweenPlatforms);

            Vector3 spawnPosition = new Vector3(
                xPosition,
                startPosition.y,
                0
            );

            Instantiate(
                groundPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}
