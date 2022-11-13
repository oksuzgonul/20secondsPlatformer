using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    private TimerManager _timerManager;
    private void Start()
    {
        _timerManager = timer.GetComponent<TimerManager>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerController>() == null) return;
        _timerManager.EndGame(true);
    }
}
