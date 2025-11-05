using UnityEngine;

public class GameBounds : MonoBehaviour
{
    public static float minX, maxX, minY, maxY;

    void Awake()
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
}