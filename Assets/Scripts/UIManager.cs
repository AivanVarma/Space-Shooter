using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _victoryText;

    [SerializeField]
    private TMP_Text _victoryScoreText;

    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private TMP_Text _ammoCountText;

    [SerializeField]
    private TMP_Text _missileCountText;

    [SerializeField]
    private TMP_Text _waveNumberText;

    [SerializeField]
    private TMP_Text _helpText;

    [SerializeField]
    private Image _livesImage;

    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private Slider _thrusterSlider;

    private GameManager _gameManager;

    private WaitForSeconds _flicker = new WaitForSeconds(0.5f);
    private WaitForSeconds _showWaveNumber = new WaitForSeconds(5f);

    // Start is called before the first frame update
    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _victoryText.gameObject.SetActive(false);
        _victoryScoreText.gameObject.SetActive(false);
        //_helpText.gameObject.SetActive(true);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _helpText.gameObject.SetActive(!_helpText.gameObject.activeSelf);
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

    public void UpdateAmmoCount(int ammo, int maxAmmo)
    {
        _ammoCountText.text = ammo + "/" + maxAmmo;
    }

    public void UpdateMissileCount(int missiles, int maxMissiles)
    {
        _missileCountText.text = missiles + "/" + maxMissiles;
    }

    public void ShowWaveNumber(int wave)
    {
        if (wave == 6)
        {
            _waveNumberText.text = "Boss Wave";
        }
        else
        {
            _waveNumberText.text = "Wave " + wave;
        }
        
        StartCoroutine(WaveNumberRoutine());
    }

    public void GameOver(bool victory)
    {
        GameOverSequence(victory);
    }

    private void GameOverSequence(bool victory)
    {
        TMP_Text text = _gameOverText;

        _restartText.gameObject.SetActive(true);

        _victoryScoreText.text = _scoreText.text;
        _victoryScoreText.gameObject.SetActive(true);

        _gameManager.GameOver();

        if (victory)
        {
            text = _victoryText;
        }

        StartCoroutine(EndFlickerRoutine(text));
    }

    IEnumerator EndFlickerRoutine(TMP_Text text)
    {
        while (true)
        {
            yield return _flicker;

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            float a = Random.Range(0.5f, 1f);

            text.color = new Color(r, g, b, a);
            text.gameObject.SetActive(true);

            yield return _flicker;

            text.gameObject.SetActive(false);
        }
    }

    IEnumerator WaveNumberRoutine()
    {
        _waveNumberText.gameObject.SetActive(true);
        yield return _showWaveNumber;
        _waveNumberText.gameObject.SetActive(false);
    }
}
