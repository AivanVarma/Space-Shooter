using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;

    private Vector3 _startingPosition = new Vector3(0, 0, 0);

    private float _speed = 5.0f;
    private int _lives = 3;

    private float _xLeftBound = -9.2f;
    private float _xRightBound = 9.2f;
    private float _yBottomBound = -4f;
    private float _yUpperBound = 6f;

    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _offset = new Vector3(0f, 0.8f, 0f);
    private float _canFire = -1f;
    private float _fireRate = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _startingPosition;

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        if (_laserPrefab == null)
        {
            Debug.LogError("The Laser prefab is NULL!");
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

        transform.Translate(_speed * Time.deltaTime * direction);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _xLeftBound, _xRightBound), transform.position.y, 0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yBottomBound, _yUpperBound), 0);
    }

    private void FireLaser()
    {
        Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
        _canFire = Time.time + _fireRate;
    }

    public void Damage()
    {
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
