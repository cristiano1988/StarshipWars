using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Duration")]
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                if (gm != null)
                {
                    gm.EnemyHit(other.transform.position);
                    gm.AddDestroyedEnemy();
                }
            }
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}