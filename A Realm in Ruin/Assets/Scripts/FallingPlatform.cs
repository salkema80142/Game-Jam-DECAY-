using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [Header("Textures")]
    public Sprite intactTexture;
    public Sprite crackedTexture;
    public Sprite brokenTexture;

    [Header("Timing")]
    public float crackDelay = 1.5f;
    public float breakDelay = 1.5f;
    public float fallDelay = 0.5f;

    [Header("Shake")]
    public float shakeAmount = 0.04f;
    public float shakeSpeed = 25f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Vector3 originalPosition;
    private bool triggered = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        originalPosition = transform.position;

        spriteRenderer.sprite = intactTexture;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(Crumble());
        }
    }

    IEnumerator Crumble()
    {
        yield return ShakeAndWait(crackDelay);
        spriteRenderer.sprite = crackedTexture;

        yield return ShakeAndWait(breakDelay);
        spriteRenderer.sprite = brokenTexture;

        yield return ShakeAndWait(fallDelay);

        transform.position = originalPosition;
        rb.bodyType = RigidbodyType2D.Dynamic;

        Destroy(gameObject, 3f);
    }

    IEnumerator ShakeAndWait(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float shake = Mathf.Sin(Time.time * shakeSpeed)
                          * shakeAmount;

            transform.position = originalPosition +
                new Vector3(shake, 0, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
}
