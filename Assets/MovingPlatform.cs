using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDirection;
    private Vector3 _position;
    private bool _isPaused;

    private void Start()
    {
        //_isPaused = true;
        _position = gameObject.transform.position;
    }

    private void FixedUpdate()
    {
        if (_isPaused) return;
        var moveMagnitude = (Mathf.Sin(Time.time) * maxDistance * moveSpeed);
        gameObject.transform.position = _position + moveDirection * moveMagnitude;
    }

    public void Reset()
    {
        gameObject.transform.position = _position;
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    /*private void OnCollisionEnter2D(Collision2D col)
    {
        col.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.SetParent(null);
    }*/
}
