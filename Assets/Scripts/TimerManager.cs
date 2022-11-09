using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private int _current;
    private Button _button;
    private Coroutine _coroutine;
    private bool _isRunning;
    private Image _buttonImage;
    void Start()
    {
        StartCoroutine( ResetTimer());
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartGame);
        _buttonImage = gameObject.GetComponent<Image>();
    }

    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(1);
        _current = 0;
        text.SetText(_current.ToString());
        SetButtonVisibility(true);
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
        gameObject.SetActive(true);
    }

    private void StartGame()
    {
        if (_isRunning) return;
        _current = 0;
        _isRunning = true;
        _coroutine = StartCoroutine(SecondUpdate());
        SetButtonVisibility(false);
    }

    private void SetButtonVisibility(bool visible)
    {
        _buttonImage.color = visible ? new Color(191,191,191,255) : new Color(0, 0, 0, 0);
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
