using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float initialYPosition;
    [SerializeField] private Color backgroundColor = Color.black;
    private Vector3 _initialPosition;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        var playerPosition = player.transform.position;
        var o = gameObject;
        var cameraPosition = o.transform.position;
        if (playerPosition.y > initialYPosition)
        {
            cameraPosition.y = playerPosition.y;
        }
        o.transform.position = cameraPosition;
        _camera.backgroundColor = backgroundColor;
    }
    public void ResetCamera()
    {
        gameObject.transform.position = _initialPosition;
    }
}