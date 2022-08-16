using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private int _health = 9;

    private float _yStopPosition = 3.39f;
    private bool _stoppedMoving = false;
    private float _speed = 3f;

    [SerializeField]
    private GameObject _shields;
    private SpriteRenderer _shieldRenderer;
    private Color _shieldColor;
    private float[] _shieldAlphaValues = { 1f, 0.1f, 0.5f, 0.75f };
    private int _shieldBaseStrength = 4;
    private int _shieldStrength = 4;
    private float _shieldsRecharged = 20f;
    private float _shieldsRechargeTime = 20f;
    private float _shieldsMinRechargeTime = 10f;
    private float _shieldsMaxRechargeTime = 20f;

    [SerializeField]
    private GameObject[] _damages;

    private Player _player;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }

        _shieldRenderer = _shields.GetComponent<SpriteRenderer>();
        _shieldColor = _shieldRenderer.color;
        _shields.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stoppedMoving)
        {
            MoveToPlace();
        }

        if (!_shields.activeSelf && Time.time > _shieldsRecharged)
        {
            _shields.SetActive(true);
        }
    }

    private void MoveToPlace()
    {
        if (transform.position.y > _yStopPosition)
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);
        }
        else
        {
            _stoppedMoving = true;
        }
    }

    private void Damage()
    {
        if (_shields.activeSelf)
        {
            ShieldDamage();
        }
        else
        {
            _health--;

            if (_health > 0)
            {
                int randomDamage;

                do
                {
                    randomDamage = Random.Range(0, _damages.Length);

                    if (!_damages[randomDamage].activeSelf)
                    {
                        _damages[randomDamage].SetActive(true);
                        return;
                    }

                } while (_damages[randomDamage].activeSelf);
            }

        }
    }

    private void ShieldDamage()
    {
        _shieldStrength--;

        if (_shieldStrength > 0)
        {
            _shieldColor.a = _shieldAlphaValues[_shieldStrength];
            _shieldRenderer.color = _shieldColor;
        }
        else
        {
            _shields.SetActive(false);
            _shieldColor.a = _shieldAlphaValues[_shieldStrength];
            _shieldRenderer.color = _shieldColor;
            _shieldStrength = _shieldBaseStrength;
            _shieldsRechargeTime = Random.Range(_shieldsMinRechargeTime, _shieldsMaxRechargeTime);
            _shieldsRecharged = _shieldsRechargeTime + Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_stoppedMoving)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().Damage();
            Damage();
        }

        if (collision.CompareTag("Laser"))
        {
            if (_shields.activeSelf)
            {
                _player.AddPoints(50);
            }
            else
            {
                _player.AddPoints(100);
            }
            
            Destroy(collision.gameObject);

            Damage();
        }

        if (collision.CompareTag("HomingMissile"))
        {
            if (_shields.activeSelf)
            {
                _player.AddPoints(50);
            }
            else
            {
                _player.AddPoints(100);
            }

            Damage();
        }
    }
}
