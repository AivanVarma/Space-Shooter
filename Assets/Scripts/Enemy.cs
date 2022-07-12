using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 3.5f;
    private float _deathSpeed = 0.75f;
    private bool _isDead = false;

    private float _xLeftBound = -9.2f;
    private float _xRightBound = 9.2f;
    private float _yBottomBound = -6f;
    private float _yUpperBound = 8f;
    private float _xOffset = 2f;
    private float _yOffset = 4f;

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _enemyLaserPrefab;
    private float _canFire = 1f;
    private float _minimumFireRate = 3f;
    private float _maximumFireRate = 6f;

    [SerializeField]
    private GameObject _enemyShields;

    private bool _powerupDetected = false;

    private bool _ramPlayer = false;
    private Vector3 _rammingDirection;
    private float _rammingSpeed = 5f;

    private bool _playerBehind = false;

    private bool _avoidShot = false;
    private Vector3 _evadeDirection;
    private int _maxEvasionDirection = 10;
    private float _evasionSpeedBoost = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = RandomSpawnPosition();

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = this.GetComponent<Animator>();

        _audioSource = this.GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL!");
        }

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();

        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    public void ActivateShields()
    {
        _enemyShields.SetActive(true);
    }

    private Vector3 RandomSpawnPosition()
    {

        float xPosition = Random.Range(_xLeftBound, _xRightBound);
        float yPosition = _yUpperBound;

        if (transform.rotation.z < 0f)
        {
            xPosition = _xRightBound + _xOffset;
            yPosition = Random.Range(0f, _yUpperBound);
        }
        else if (transform.rotation.z > 0f)
        {
            xPosition = _xLeftBound - _xOffset;
            yPosition = Random.Range(0f, _yUpperBound);
        }

        return new Vector3(xPosition, yPosition, 0f);

    }

    private void Movement()
    {
        if (_ramPlayer && !_isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, _rammingDirection, _rammingSpeed * Time.deltaTime);
        }
        else if (_avoidShot && !_isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, _evadeDirection, _speed * _evasionSpeedBoost * Time.deltaTime);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);
        }

        if (transform.position.y < _yBottomBound || transform.position.y > _yUpperBound + _yOffset ||
            transform.position.x > _xRightBound + _xOffset || transform.position.x < _xLeftBound - _xOffset)
        {
            transform.position = RandomSpawnPosition();
        }
    }

    private void FireLaser()
    {
        if (_isDead)
        {
            return;
        }

        GameObject laser;

        if (!_powerupDetected)
        {
            float randomFireRate = Random.Range(_minimumFireRate, _maximumFireRate);
            _canFire = Time.time + randomFireRate;
        }

        if (_playerBehind)
        {
            laser = Instantiate(_enemyLaserPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, 180f));
        }
        else
        {
            laser = Instantiate(_enemyLaserPrefab, transform.position, transform.rotation);
        }

        laser.GetComponent<Laser>().AssignEnemyLaser();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            OnEnemyDeath();
        }

        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);

            if (!_enemyShields.activeSelf)
            {
                _player.AddPoints(10);
            }


            OnEnemyDeath();
        }

        if (collision.CompareTag("HomingMissile"))
        {
            _player.AddPoints(20);

            OnEnemyDeath();
        }
    }

    private void OnEnemyDeath()
    {
        if (_enemyShields.activeSelf)
        {
            _enemyShields.SetActive(false);
            return;
        }

        _isDead = true;

        Destroy(GetComponent<Collider2D>());

        _anim.SetTrigger("OnEnemyDeath");
        _speed = _deathSpeed;

        _audioSource.Play();

        Destroy(this.gameObject, 2.8f);
    }

    public void PowerupDetected()
    {
        _powerupDetected = true;
        FireLaser();
        _powerupDetected = false;
    }

    public void RamPlayer(Vector3 rammingDirection)
    {
        _ramPlayer = !_ramPlayer;
        _rammingDirection = rammingDirection;
    }

    public void PlayerBehind()
    {
        _playerBehind = true;
        FireLaser();
        _playerBehind = false;
    }

    public void AvoidShot()
    {
        _avoidShot = !_avoidShot;

        int randomDirection = Random.Range(0, _maxEvasionDirection);

        if (transform.rotation.z < 0)
        {
            _evadeDirection = new Vector3(-randomDirection, randomDirection, 0);
        }
        else if (transform.rotation.z > 0)
        {
            _evadeDirection = new Vector3(randomDirection, randomDirection, 0);
        }
        else
        {
            if (randomDirection % 2 == 0)
            {
                _evadeDirection = new Vector3(randomDirection, -randomDirection, 0);
            }
            else
            {
                _evadeDirection = new Vector3(-randomDirection, -randomDirection, 0);
            }
        }
        
        _evadeDirection += transform.position;
    }
}
