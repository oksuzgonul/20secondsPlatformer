using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    [SerializeField] public GameObject timerManager;
    private TimerManager _timerManager;
    private void Start()
    {
        _timerManager = timerManager.GetComponent<TimerManager>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerController>() == null) return;
        _timerManager.UpdateLevel();
    }
}
