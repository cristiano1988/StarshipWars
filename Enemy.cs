using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Prefabs")]
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        float clampedX = Mathf.Clamp(transform.position.x, GameBounds.minX, GameBounds.maxX);
        float clampedY = Mathf.Clamp(transform.position.y, GameBounds.minY, GameBounds.maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.PlayerHit(collision.gameObject);
        }
    }
}