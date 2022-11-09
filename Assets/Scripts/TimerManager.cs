using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameObject playerObject;
    private int _current;
    private Button _button;
    private Coroutine _coroutine;
    private bool _isRunning;
    private PlayerController _playerController;
    private Vector3 _initialPosition;
    void Start()
    {
        StartCoroutine( ResetTimer());
        _button = buttonObject.GetComponent<Button>();
        _button.onClick.AddListener(StartGame);
        _playerController = playerObject.GetComponent<PlayerController>();
        _playerController.enabled = false;
        _initialPosition = playerObject.transform.position;
    }
    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(1);
        _current = 0;
        text.SetText(_current.ToString());
        buttonObject.SetActive(true);
    }
    private void Update()
    {
        if (text.text == "20") EndGame();
    }
    private void EndGame()
    {
        _isRunning = false;
        StopCoroutine(_coroutine);
        StartCoroutine(ResetTimer());
        _playerController.enabled = false;
        playerObject.transform.position = _initialPosition;
    }
    private void StartGame()
    {
        if (_isRunning) return;
        _current = 0;
        _isRunning = true;
        _coroutine = StartCoroutine(SecondUpdate());
        buttonObject.SetActive(false);
        _playerController.enabled = true;
    }
    private IEnumerator SecondUpdate()
    {
        while (_isRunning)
        {
            yield return new WaitForSeconds(1);
            _current++;
            text.SetText(_current.ToString());
        }
    }
}
