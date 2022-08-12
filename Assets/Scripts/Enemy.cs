using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _enemyID; // Basic = 0, Teleporter = 1, Aggressive = 2, Smart = 3, Avoid Shot = 4
    private float _baseSpeed = 3.5f;
    private float _speed = 3.5f;
    private float _deathSpeed = 0.75f;
    private bool _isDead = false;
    private int[] _points = { 10, 100, 30, 30, 30 }; // Index zero used for shields as well.

    private float _xLeftBound = -9.2f;
    private float _xRightBound = 9.2f;
    private float _yBottomBound = -6f;
    private float _yUpperBound = 8f;
    private float _xOffset = 1f;
    private float _yOffset = 1f;

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _enemyLaserPrefab;
    private float _canFire = 1f;
    private float _minFireRate = 3f;
    private float _maxFireRate = 6f;

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

    private float _canTeleport = 3f;
    private float _minTeleportRate = 3f;
    private float _maxTeleportRate = 7f;
    private WaitForSeconds _teleportationWait = new WaitForSeconds(0.5f);

    [SerializeField]
    private GameObject _missilePrefab;
    private float _canFireMissile = 3f;
    private float _minMissileFireRate = 4f;
    private float _maxMissileFireRate = 8f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = RandomSpawnPosition();

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = this.GetComponent<Animator>();

        _audioSource = this.GetComponent<AudioSource>();

        _canFire = Time.time + _minFireRate;
        _canFireMissile = Time.time + _minMissileFireRate;
        _canTeleport = Time.time + _minTeleportRate;

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
        if (!_isDead)
        {
            Movement();
            FiringMechanism();
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
        if (_ramPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, _rammingDirection, _rammingSpeed * Time.deltaTime);
        }
        else if (_avoidShot)
        {
            transform.position = Vector3.MoveTowards(transform.position, _evadeDirection, _speed * _evasionSpeedBoost * Time.deltaTime);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);
        }

        if (Time.time > _canTeleport && _enemyID == 1)
        {
            Teleport();
        }

        if (transform.position.y < _yBottomBound - _yOffset|| transform.position.y > _yUpperBound + _yOffset ||
            transform.position.x > _xRightBound + _xOffset || transform.position.x < _xLeftBound - _xOffset)
        {
            transform.position = RandomSpawnPosition();
        }
    }

    private void FiringMechanism()
    {
        if (Time.time > _canFire)
        {
            FireLaser();
        }

        if (Time.time > _canFireMissile && _enemyID == 1)
        {
            FireMissile();
        }
    }

    private void FireLaser()
    {
        GameObject laser;

        if (!_powerupDetected)
        {
            float randomFireRate = Random.Range(_minFireRate, _maxFireRate);
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

    private void FireMissile()
    {
        _canFireMissile = Time.time + Random.Range(_minMissileFireRate, _maxMissileFireRate);

        Vector3 position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        GameObject enemyMissile = Instantiate(_missilePrefab, position, Quaternion.Euler(0, 0, 180));
        enemyMissile.GetComponent<HomingMissile>().AssingEnemyMissile();
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

            _player.AddPoints(_points[_enemyID]);

            OnEnemyDeath();
        }

        if (collision.CompareTag("HomingMissile") && !collision.transform.GetComponent<HomingMissile>().IsEnemyMissile())
        {
            _player.AddPoints(_points[_enemyID]);

            OnEnemyDeath();
        }
    }

    private void OnEnemyDeath()
    {
        if (_enemyShields.activeSelf)
        {
            _enemyShields.SetActive(false);
            _player.AddPoints(_points[0]);
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

        if (!_isDead) {
            FireLaser();
        }
        
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

        if (!_isDead)
        {
            FireLaser();
        }

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

    private void Teleport()
    {
        float randomTeleportRate = Random.Range(_minTeleportRate, _maxTeleportRate);
        _canTeleport = Time.time + randomTeleportRate;

        float randomX = Random.Range(_xLeftBound + _xOffset, _xRightBound - _xOffset);
        float randomY = Random.Range(_yBottomBound + _yOffset, _yUpperBound - _yOffset);
        Vector3 randomPosition = new Vector3(randomX, randomY, 0);

        if (Vector3.Distance(randomPosition, _player.transform.position) < 3)
        {
            randomPosition.x += Random.value;
            randomPosition.y += Random.value;
        }

        StartCoroutine(TeleportWaitRoutine(randomPosition));
    }

    private IEnumerator TeleportWaitRoutine(Vector3 position)
    {
        _speed = 0;

        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        color.a = 0.25f;
        renderer.color = color;
        yield return _teleportationWait;

        transform.position = position;

        yield return _teleportationWait;
        color.a = 1f;
        renderer.color = color;
        _speed = _baseSpeed;
    }
}
