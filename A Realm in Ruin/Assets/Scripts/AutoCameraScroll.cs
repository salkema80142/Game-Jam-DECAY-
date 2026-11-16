using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public float followSpeed = 5f;

    [Header("Camera Position")]
    public float yOffset = 2f;
    public float zPosition = -10f;

    private Transform player;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject with the Player tag found!");
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(
            player.position.x,
            yOffset,
            zPosition
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
