using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private float _xLeftBound = -7.2f;
    private float _xRightBound = 7.2f;
    private float _yBottomBound = -4f;
    private float _yUpperBound = 8f;

    private float _speed = 3f;

    [SerializeField] // Triple shot = 0, Speed = 1, Shield = 2, Health = 3, Ammo = 4, Scatter shot = 5, Negative Speed = 6, Missiles = 7
    private int _powerupID;

    [SerializeField]
    private AudioClip _powerupSoundClip;
    private float _zSoundClipOffset = -10;

    private bool _inPowerupCollectorZone = false;
    private Transform _player;
    private float _collectedSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(_xLeftBound, _xRightBound), _yUpperBound, 0);

        _player = GameObject.Find("Player").GetComponent<Transform>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_inPowerupCollectorZone && Input.GetKey(KeyCode.C))
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, _collectedSpeed * Time.deltaTime);
        }
        else
        {
            Movement();
        }
    }

    private void Movement()
    {
        if (transform.position.y < _yBottomBound)
        {
            Destroy(this.gameObject);
        }

        transform.Translate(_speed * Time.deltaTime * Vector3.down);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();

            Vector3 soundClipPosition = new Vector3(transform.position.x, transform.position.y, _zSoundClipOffset);

            AudioSource.PlayClipAtPoint(_powerupSoundClip, soundClipPosition);

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.LifeCollected();
                        break;
                    case 4:
                        player.AmmoCollected();
                        break;
                    case 5:
                        player.ScatterShotActive();
                        break;
                    case 6:
                        player.NegativeSpeedBoostActive();
                        break;
                    case 7:
                        player.MissilesCollected();
                        break;
                    default:
                        Debug.Log("Powerup ID not found!");
                        break;
                }

            }

            Destroy(this.gameObject);
        }

        if (collision.CompareTag("PowerupCollector"))
        {
            _inPowerupCollectorZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerupCollector"))
        {
            _inPowerupCollectorZone = false;
        }
    }
}
