using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float initialYPosition;
    private void FixedUpdate()
    {
        var playerPosition = player.transform.position;
        var cameraPosition = transform.position;
        if (cameraPosition.y > initialYPosition) cameraPosition.y = playerPosition.y;
        transform.position = cameraPosition;
    }
}
