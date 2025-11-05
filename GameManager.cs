using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    public float playerMoveSpeed = 5f;
    public float playerBoostFactor = 1.2f;
    public float playerRotateSpeed = 270f;
    public float playerMinMouseDistance = 0.3f;
    public float bulletSpeed = 12f;
    public GameObject bulletPrefab;

    [Header("Enemy Settings")]
    public float enemyMoveSpeed = 2f;
    public GameObject enemyPrefab;
    public float spawnMinDelay = 0.5f;
    public float spawnMaxDelay = 1.5f;

    [Header("Game Bounds")]
    public static float minX, maxX, minY, maxY;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;

    [Header("Prefabs")]
    public GameObject explosionPlayerPrefab;
    public GameObject explosionEnemyPrefab;

    [Header("Audio")]
    public AudioClip playerExplosionSound;
    public AudioClip enemyExplosionSound;

    private AudioSource audioSource;
    private int destroyedCount = 0;
    private bool gameOver = false;
    private GameObject player;
    private Camera mainCamera;

    void Awake()
    {
        SetupGameBounds();
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        UpdateUI();
        StartCoroutine(SpawnRoutine());
    }

    void SetupGameBounds()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float margin = 0.5f;

        minX = -camWidth + margin;
        maxX = camWidth - margin;
        minY = -camHeight + margin;
        maxY = camHeight - margin;
    }

    void Update()
    {
        if (player != null)
        {
            HandlePlayerMovement();
            HandlePlayerRotation();
            HandlePlayerShooting();
        }
    }

    void HandlePlayerMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isMoving = (h != 0 || v != 0);

        bool altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        Vector3 moveDir;

        if (altHeld)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dirToMouse = (mousePos - player.transform.position);

            if (dirToMouse.magnitude > playerMinMouseDistance)
            {
                float targetAngle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg - 90f;
                player.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }

            moveDir = (Vector3.up * v + Vector3.right * h).normalized;
            player.transform.Translate(moveDir * playerMoveSpeed * playerBoostFactor * Time.deltaTime, Space.Self);
        }
        else
        {
            moveDir = (Vector3.up * v + Vector3.right * h).normalized;
            player.transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime, Space.Self);
        }

        float clampedX = Mathf.Clamp(player.transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(player.transform.position.y, minY, maxY);
        player.transform.position = new Vector3(clampedX, clampedY, player.transform.position.z);
    }

    void HandlePlayerRotation()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        if (isMoving || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - player.transform.position);

        if (dir.magnitude < playerMinMouseDistance)
            return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = player.transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, playerRotateSpeed * Time.deltaTime);

        player.transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    void HandlePlayerShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, player.transform.position, player.transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = bullet.transform.up * bulletSpeed;
                
            Destroy(bullet, 3f);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(spawnMinDelay, spawnMaxDelay);
            yield return new WaitForSeconds(wait);

            if (enemyPrefab == null) continue;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0: y = maxY; break;
                case 1: y = minY; break;
                case 2: x = maxX; break;
                case 3: x = minX; break;
            }

            Vector3 spawnPos = new Vector3(x, y, 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(EnemyMovement(enemy.transform));
        }
    }

    IEnumerator EnemyMovement(Transform enemy)
    {
        while (enemy != null && player != null)
        {
            enemy.position = Vector3.MoveTowards(
                enemy.position,
                player.position,
                enemyMoveSpeed * Time.deltaTime
            );

            float clampedX = Mathf.Clamp(enemy.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(enemy.position.y, minY, maxY);
            enemy.position = new Vector3(clampedX, clampedY, enemy.position.z);

            yield return null;
        }
    }

    public void AddDestroyedEnemy()
    {
        if (gameOver) return;
        destroyedCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "" + destroyedCount;
    }

    public void PlayerHit(GameObject playerObj)
    {
        if (gameOver) return;
        gameOver = true;

        if (explosionPlayerPrefab != null)
            Instantiate(explosionPlayerPrefab, playerObj.transform.position, Quaternion.identity);

        if (playerExplosionSound != null)
            audioSource.PlayOneShot(playerExplosionSound);

        Destroy(playerObj);

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            StartCoroutine(FadeInGameOver());
        }

        Invoke(nameof(ReloadScene), 2f);
    }

    public void EnemyHit(Vector3 position)
    {
        if (explosionEnemyPrefab != null)
            Instantive(explosionEnemyPrefab, position, Quaternion.identity);

        if (enemyExplosionSound != null)
            audioSource.PlayOneShot(enemyExplosionSound);
    }

    IEnumerator FadeInGameOver()
    {
        if (gameOverText == null) yield break;
        gameOverText.alpha = 0f;
        while (gameOverText.alpha < 1f)
        {
            gameOverText.alpha += Time.deltaTime * 1f;
            yield return null;
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Collision handling for enemies and bullets
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.gameObject.name.Contains("Bullet"))
        {
            EnemyHit(other.transform.position);
            AddDestroyedEnemy();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.name.Contains("Enemy"))
        {
            PlayerHit(collision.gameObject);
        }
    }
}