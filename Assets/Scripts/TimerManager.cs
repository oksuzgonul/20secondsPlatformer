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
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject closeButton;
    private int _current;
    private Button _timerButton;
    private Button _closeButton;
    private Coroutine _coroutine;
    private bool _isRunning;
    private PlayerController _playerController;
    private Vector3 _initialPosition;
    private int _score;
    
    //TODO: Convert platforms to prefab
    //TODO: Add environment art
    //TODO: Add character art and animation

    void Start()
    {
        //Set buttons
        _timerButton = buttonObject.GetComponent<Button>();
        _timerButton.onClick.AddListener(StartGame);
        _closeButton = closeButton.GetComponent<Button>();
        _closeButton.onClick.AddListener(CloseScore);
        //Deactivate player controllers
        _playerController = playerObject.GetComponent<PlayerController>();
        _playerController.enabled = false;
        //Record the initial player position
        _initialPosition = playerObject.transform.position;
        _score = 0;
    }
    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(1);
        _current = 0;
        timerText.SetText(_current.ToString());
        buttonObject.SetActive(true);
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
        StopCoroutine(_coroutine);
        StartCoroutine(ResetTimer());
        _playerController.enabled = false;
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
            timerText.SetText(_current.ToString());
        }
    }
}
