using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        UpdateUI();
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

    public void PlayerHit(GameObject player)
    {
        if (gameOver) return;
        gameOver = true;

        if (explosionPlayerPrefab != null)
            Instantiate(explosionPlayerPrefab, player.transform.position, Quaternion.identity);

        if (playerExplosionSound != null)
            audioSource.PlayOneShot(playerExplosionSound);

        Destroy(player);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        Invoke(nameof(ReloadScene), 2f);
    }

    public void EnemyHit(Vector3 position)
    {
        if (explosionEnemyPrefab != null)
            Instantiate(explosionEnemyPrefab, position, Quaternion.identity);

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
}