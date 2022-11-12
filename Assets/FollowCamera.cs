using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float initialYPosition;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = gameObject.transform.position;
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
    }

    public void ResetCamera()
    {
        gameObject.transform.position = _initialPosition;
    }
}