using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

   [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private Image _livesImage;

    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private Slider _thrusterSlider;

    private GameManager _gameManager;

    private WaitForSeconds _flicker = new WaitForSeconds(0.5f);

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL!");
        }
    }

    public void UpdateScore(int points)
    {
        _scoreText.text = "Score: " + points;
    }

    public void UpdateLives(int lives)
    {
        if (lives < 1)
        {
            _livesImage.sprite = _livesSprites[0];
            GameOverSequence();
        }
        else
        {
            _livesImage.sprite = _livesSprites[lives];
        }      
    }

    public void UpdateThrusterUI(float value)
    {
        if (value < 0)
        {
            _thrusterSlider.value = 0;
        }
        else if (value > 1)
        {
            _thrusterSlider.value = 1;
        }
        else
        {
            _thrusterSlider.value = value;
        }
    }

    private void GameOverSequence()
    {
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();

        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            yield return _flicker;

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            float a = Random.Range(0.5f, 1f);

            _gameOverText.color = new Color(r, g, b, a);
            _gameOverText.gameObject.SetActive(true);

            yield return _flicker;

            _gameOverText.gameObject.SetActive(false);
        }
    }
}
