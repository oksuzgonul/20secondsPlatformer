using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private Animator _animatorP;

    private void Awake()
    {
        _animatorP = GetComponent<Animator>();
    }

    private void Start()
    {
        _initialPosition = gameObject.transform.position;
        SetProperties();
        if (!isHiddenPlatform) return;
        SetPlatformA(0);
    }

    private void SetProperties()
    {
        isHiddenPlatform = Random.Range(0.0f, 1.0f) > 0.7f;
        movesCollided = Random.Range(0.0f, 1.0f) > 0.1f;
        var moveDir = Random.Range(0.0f, 1.0f);
        moveDirection = moveDir switch
        {
            < 0.7f => new Vector3(0, 0, 0),
            < 0.85f => new Vector3(1, 0, 0),
            _ => new Vector3(0, 1, 0),
        };
    }

    private void SetPlatformA(int aValue)
    {
        _platformColor = gameObject.transform.GetComponentInChildren<SpriteRenderer>().color;
        _platformColor.a = aValue;
        gameObject.transform.GetComponentInChildren<SpriteRenderer>().color = _platformColor;
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

    public void SetPosition(Vector3 newPos)
    {
        _animatorP.Rebind();
        _animatorP.Update(0f);
        _initialPosition = newPos;
        transform.position = newPos;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var player = col.gameObject.GetComponent<PlayerController>();
        var isLanding = transform.position.y < player.transform.position.y;
        player.PlayCollideSound(isLanding);
        if (!isLanding) _animatorP.Play("jiggle");
        if (!player.IsPlayerGrounded())
        {
            return;
        }
        if (isHiddenPlatform) {SetPlatformA(255);}
        if (isLanding) col.transform.SetParent(transform);
        _hasCollided = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.activeSelf && gameObject.activeSelf)
        {
            Transform t = transform.parent;
            other.transform.SetParent(t);
        }
    }
}
