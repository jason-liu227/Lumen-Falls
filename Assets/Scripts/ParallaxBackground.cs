using UnityEngine;

[DefaultExecutionOrder(1000)]
public sealed class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;
    [SerializeField] private SpriteRenderer sky;
    [SerializeField] private SpriteRenderer mountains;
    [SerializeField, Range(0f, 1f)] private float mountainDrift = 0.15f;

    private const float MountainOverscan = 3f;
    private float startCameraX;

    private void Start()
    {
        startCameraX = viewCamera.transform.position.x;
        LateUpdate();
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = viewCamera.transform.position;
        float viewHeight = viewCamera.orthographicSize * 2f;
        float viewWidth = viewHeight * viewCamera.aspect;
        float bottom = cameraPosition.y - viewHeight * 0.5f;

        Vector2 skySize = sky.sprite.bounds.size;
        sky.transform.localScale = new Vector3(viewWidth / skySize.x, viewHeight / skySize.y, 1f);
        sky.transform.position = new Vector3(cameraPosition.x, bottom, sky.transform.position.z);

        Vector2 mountainSize = mountains.sprite.bounds.size;
        float scale = Mathf.Max(viewHeight / mountainSize.y,
            (viewWidth + MountainOverscan * 2f) / mountainSize.x);
        mountains.transform.localScale = new Vector3(scale, scale, 1f);

        float margin = (mountainSize.x * scale - viewWidth) * 0.5f;
        float drift = Mathf.Clamp((startCameraX - cameraPosition.x) * mountainDrift, -margin, margin);
        mountains.transform.position = new Vector3(cameraPosition.x + drift, bottom, mountains.transform.position.z);
    }
}
