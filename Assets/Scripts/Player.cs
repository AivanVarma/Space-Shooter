using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed = 7.0f;

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
        transform.position = new Vector3(0,0,0);
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

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;

        transform.Translate(_speed * Time.deltaTime * direction);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _xLeftBound, _xRightBound), transform.position.y, 0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yBottomBound, _yUpperBound), 0);
    }

    void FireLaser()
    {
        Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
        _canFire = Time.time + _fireRate;
    }
}
