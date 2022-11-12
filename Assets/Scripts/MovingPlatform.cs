using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private bool isHiddenPlatform;
    [SerializeField] private bool movesCollided;
    private Vector3 _initialPosition;
    private bool _hasCollided;
    private Color _platformColor;
    private float _offTime;
    private void Start()
    {
        _initialPosition = gameObject.transform.position;

        if (!isHiddenPlatform) return;
        SetPlatformA(0);
    }

    private void SetPlatformA(int aValue)
    {
        _platformColor = gameObject.GetComponent<SpriteRenderer>().color;
        _platformColor.a = aValue;
        gameObject.GetComponent<SpriteRenderer>().color = _platformColor;
    }

    private void FixedUpdate()
    {
        if (movesCollided && !_hasCollided)
        {
            _offTime = Time.time;
            return;
        }
        var moveMagnitude = (Mathf.Sin((Time.time - _offTime)* moveSpeed) * maxDistance );
        gameObject.transform.position = _initialPosition + moveDirection * moveMagnitude;
    }

    public void Reset()
    {
        gameObject.transform.position = _initialPosition;
        _offTime = Time.time;
        _hasCollided = false;
        if (isHiddenPlatform) {SetPlatformA(0);}
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.GetComponent<PlayerController>().IsPlayerGrounded()) return;
        if (isHiddenPlatform) {SetPlatformA(255);}
        col.transform.SetParent(transform);
        _hasCollided = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.SetParent(null);
    }
}
