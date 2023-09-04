using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerManager : MonoBehaviour
{
    [SerializeField] private GameObject levelManager;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI maxScoreText;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject timerButtonObj;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject closeButtonObj;
    [SerializeField] private GameObject cameraController;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private int runTime = 50;
   
    private int _current;
    private Button _timerButton;
    private Button _closeButton;
    private Coroutine _coroutine;
    private bool _isRunning;
    private PlayerController _playerController;
    private Vector3 _initialPosition;
    private int _score;
    private int _maxScore;
    private GameObject[] _platforms;
    private LevelGenerator _levelGenerator;
    private Rigidbody2D _rb;
    private bool _isPaused;

    private static string _savePath;
    private int _addScore = 0;
    void Awake()
    {
        _savePath = Application.persistentDataPath + "/Save.txt";
        InitSaveFile();
        _maxScore = int.Parse(ReadScore());
        _rb = playerObject.GetComponent<Rigidbody2D>();
        _levelGenerator = levelManager.GetComponent<LevelGenerator>();
        CheckSaveFile();
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
        //Set score to zero at the beginning of a new game
        _score = 0;
        //Time is stopped before Start is pressed
        Time.timeScale = 0;
        //Capture all platforms in an array
        _current = runTime;
    }
    static void CheckSaveFile() {
        if (!File.Exists(_savePath)){
            File.Create(_savePath).Close();
            Debug.Log(_savePath);
        }
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
        if (timerText.text == "0") EndGame();
    }
    private void EndGame()
    {
        playerObject.transform.SetParent(levelManager.transform);
        _isRunning = false;
        _playerController.enabled = false;
        StopCoroutine(_coroutine);
        Time.timeScale = 0;
        if (_score > _maxScore)
        {
            _maxScore = _score;
            SaveScore(_maxScore.ToString());
        }
        ShowScore();
    }
    private void ResetPlayer()
    {
        playerObject.transform.position = _initialPosition;
        cameraController.GetComponent<FollowCamera>().ResetCamera();
    }
    private void AddTime()
    {
        _current += runTime;
    }
    private void ShowScore()
    {
        explanationText.text = "Time is up!";
        timerText.gameObject.SetActive(false);
        scorePanel.SetActive(true);
        timerText.text = "";
        currentScoreText.text = "Score: " + _score;
        maxScoreText.text = "Your max is: " + _maxScore;
    }
    private void CloseScore()
    {
        SaveScore(_maxScore.ToString());
        currentScoreText.text = "";
        maxScoreText.text = "";
        timerText.text = "";
        _score = 0;
        _addScore = 0;
        scorePanel.SetActive(false);
        playerObject.transform.position = _initialPosition;
        cameraController.GetComponent<FollowCamera>().ResetCamera();
        ResetTimer();
        _levelGenerator.GenerateLevel();
    }

    private void InitSaveFile()
    {
        if(File.Exists(_savePath)) return;
        File.WriteAllText(_savePath,"0");
    }
    private void SaveScore(string s)
    {
        File.WriteAllText(_savePath,s);
    }
    private string ReadScore()
    {
        return File.ReadAllText(_savePath);
    }
    private void AddScore()
    {
        _addScore += _score;
    }
    private void StartGame()
    {
        if (_isRunning) return;
        timerText.gameObject.SetActive(true);
        _current = runTime;
        _isRunning = true;
        _coroutine = StartCoroutine(SecondUpdate());
        timerButtonObj.SetActive(false);
        _playerController.enabled = true;
        Time.timeScale = 1;
    }
    private IEnumerator SecondUpdate()
    {
        timerText.SetText(_current.ToString());
        while (_isRunning)
        {
            yield return new WaitForSeconds(1);
            if (_isPaused) continue;
            _current--;
            timerText.SetText(_current.ToString());
        }
    }
    public void UpdateLevel()
    {
        PlayAndWaitForAnimation();
    }

    private void PlayAndWaitForAnimation()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _isPaused = true;
        _levelGenerator.PlayAnimation();
        levelUpText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }
    public void SetupLevel()
    {
        levelUpText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        _levelGenerator.GenerateLevel();
        ResetPlayer();
        AddScore();
        AddTime();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _isPaused = false;
    }
}
