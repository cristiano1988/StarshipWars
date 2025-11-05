using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float boostFactor = 1.2f;
    public float rotateSpeed = 270f;
    public float minMouseDistance = 0.3f;

    [Header("Shooting")]
    public float bulletSpeed = 12f;

    [Header("Prefabs")]
    public GameObject bulletPrefab;

    private bool isMoving = false;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleShooting();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        isMoving = (h != 0 || v != 0);

        bool altHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        Vector3 moveDir;

        if (altHeld)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dirToMouse = (mousePos - transform.position);

            if (dirToMouse.magnitude > minMouseDistance)
            {
                float targetAngle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }

            moveDir = (Vector3.up * v + Vector3.right * h).normalized;
            transform.Translate(moveDir * moveSpeed * boostFactor * Time.deltaTime, Space.Self);
        }
        else
        {
            moveDir = (Vector3.up * v + Vector3.right * h).normalized;
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);
        }

        float clampedX = Mathf.Clamp(transform.position.x, GameBounds.minX, GameBounds.maxX);
        float clampedY = Mathf.Clamp(transform.position.y, GameBounds.minY, GameBounds.maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void HandleRotation()
    {
        if (isMoving || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position);

        if (dir.magnitude < minMouseDistance)
            return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = bullet.transform.up * bulletSpeed;
        }
    }
}