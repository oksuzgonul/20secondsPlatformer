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
    private int _current;
    private Button _timerButton;
    private Button _closeButton;
    private Coroutine _coroutine;
    private bool _isRunning;
    private PlayerController _playerController;
    private Vector3 _initialPosition;
    private int _score;

    //TODO: Add button art
    //TODO: Add footstep and jump sounds
    //TODO: Add game end, final countdown etc sounds
    //TODO: Reset the platforms on restart
    //TODO: Prevent getting stuck on sides of platforms
    //TODO: Prevent being pushed out of bounds by platforms
    //TODO: Add disappearing/reappearing platforms
    //TODO: Add platforms that move after player steps on
    
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
        if (timerText.text == "20") EndGame();
    }
    private void EndGame()
    {
        _isRunning = false;
        _playerController.enabled = false;
        StopCoroutine(_coroutine);
        Time.timeScale = 0;
        ShowScore();
    }

    private void ShowScore()
    {
        scorePanel.SetActive(true);
        scoreText.text = "Score: " + _score;
    }

    private void CloseScore()
    {
        scoreText.text = "";
        _score = 0;
        scorePanel.SetActive(false);
        playerObject.transform.position = _initialPosition;
        ResetTimer();
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
}
