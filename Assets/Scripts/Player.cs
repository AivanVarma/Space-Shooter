using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private Vector3 _startingPosition = new Vector3(0, 0, 0);

    private float _speed = 5.0f;
    private int _lives = 3;
    private int _score = 0;

    private float _xLeftBound = -9.47f;
    private float _xRightBound = 9.47f;
    private float _yBottomBound = -3.71f;
    private float _yUpperBound = 5.79f;

    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _offset = new Vector3(0f, 1f, 0f);
    private float _canFire = -1f;
    private float _fireRate = 0.15f;

    private float _powerupActiveTime = 5f;
    private WaitForSeconds _powerupWaitTime;

    [SerializeField]
    private GameObject _tripleShotPrefab;
    private bool _isTripleShotActive = false;

    private float _speedBoost = 1f;
    private float _speedBoostBaseCoefficient = 1f;
    private float _speedBoostCoefficient = 3f;

    [SerializeField]
    private GameObject _shields;
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject[] _engineDamage;

    [SerializeField]
    private GameObject[] _thrusters;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private GameObject _explosionPrefab;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = _startingPosition;
        _shields.SetActive(_isShieldActive);

        _powerupWaitTime = new WaitForSeconds(_powerupActiveTime);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;

        transform.Translate(_speedBoost * _speed * Time.deltaTime * direction);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _xLeftBound, _xRightBound), transform.position.y, 0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yBottomBound, _yUpperBound), 0);
    }

    private void FireLaser()
    {
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
        }

        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
        
        _canFire = Time.time + _fireRate;
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shields.SetActive(_isShieldActive);
            return;
        }

        _lives--;

        EngineDamage();

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    }

    private void EngineDamage()
    {
        switch (_lives)
        {
            case 1:
                if (!_engineDamage[0].activeSelf)
                {
                    _engineDamage[0].SetActive(true);
                    _thrusters[0].SetActive(false);
                }
                else
                {
                    _engineDamage[1].SetActive(true);
                    _thrusters[1].SetActive(false);
                }
                break;
            case 2:
                int engine = Random.Range(0, 2);
                _engineDamage[engine].SetActive(true);
                _thrusters[engine].SetActive(false);
                break;
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _speedBoost = _speedBoostCoefficient;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldsActive()
    {
        _isShieldActive = true;
        _shields.SetActive(_isShieldActive);
    }

    public void AddPoints (int points)
    {
        _score += points;

        _uiManager.UpdateScore(_score);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return _powerupWaitTime;

        _isTripleShotActive = false;
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return _powerupWaitTime;

        _speedBoost = _speedBoostBaseCoefficient;
    }
}
