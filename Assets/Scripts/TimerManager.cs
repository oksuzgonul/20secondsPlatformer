using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject timerButtonObj;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject closeButtonObj;
    [SerializeField] private GameObject cameraController;
    [SerializeField] private TextMeshProUGUI explanationText;
    private int _current;
    private Button _timerButton;
    private Button _closeButton;
    private Coroutine _coroutine;
    private bool _isRunning;
    private PlayerController _playerController;
    private Vector3 _initialPosition;
    private int _score;
    private GameObject[] _platforms;

    void Start()
    {
        //Set buttons
        _timerButton = timerButtonObj.GetComponent<Button>();
        _timerButton.onClick.AddListener(StartGame);
        _closeButton = closeButtonObj.GetComponent<Button>();
        _closeButton.onClick.AddListener(CloseScore);
        //Deactivate player controllers
        _playerController = playerObject.GetComponent<PlayerController>();
        _playerController.enabled = false;
        //Record the initial player position
        _initialPosition = playerObject.transform.position;
        _score = 0;
        Time.timeScale = 0;
        //Capture all platforms
        _platforms = GameObject.FindGameObjectsWithTag("Platform");
    }
    private void ResetTimer()
    {
        _current = 0;
        timerText.SetText(_current.ToString());
        timerButtonObj.SetActive(true);
    }
    private void FixedUpdate()
    {
        if (_playerController.IsPlayerGrounded())
        {
            var currScore = (int)(_playerController.transform.position.y - _initialPosition.y ) * 100;
            _score = Math.Max(_score, currScore);
        }
        if (timerText.text == "20") EndGame(false);
    }
    public void EndGame(bool isWon)
    {
        _isRunning = false;
        _playerController.enabled = false;
        StopCoroutine(_coroutine);
        Time.timeScale = 0;
        ShowScore(isWon);
    }

    private void ShowScore(bool isWon)
    {
        explanationText.text = isWon ? "You made it to the Gate!" : "Time is up!";
        _score = isWon ? _score + 5000 : _score;
        scorePanel.SetActive(true);
        scoreText.text = "Score: " + _score;
    }

    private void CloseScore()
    {
        scoreText.text = "";
        _score = 0;
        scorePanel.SetActive(false);
        playerObject.transform.position = _initialPosition;
        cameraController.GetComponent<FollowCamera>().ResetCamera();
        ResetTimer();
        ResetAllPlatforms();
    }

    private void StartGame()
    {
        if (_isRunning) return;
        _current = 0;
        _isRunning = true;
        _coroutine = StartCoroutine(SecondUpdate());
        timerButtonObj.SetActive(false);
        _playerController.enabled = true;
        Time.timeScale = 1;
    }
    private IEnumerator SecondUpdate()
    {
        while (_isRunning)
        {
            yield return new WaitForSeconds(1);
            _current++;
            timerText.SetText(_current.ToString());
        }
    }

    private void ResetAllPlatforms()
    {
        foreach (var platform in _platforms)
        {
            platform.GetComponent<MovingPlatform>()?.Reset();
        }
    }
}
