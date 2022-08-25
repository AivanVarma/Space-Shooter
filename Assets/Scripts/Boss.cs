using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    private int _health = 9;

    private int[] _points = { 50, 100, 1000 }; // Shield shot, Body shot, Death

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
    private float _shieldsMinRechargeTime = 5f;
    private float _shieldsMaxRechargeTime = 10f;

    [SerializeField]
    private GameObject[] _damages;

    [SerializeField]
    private GameObject[] _laserBanks;
    [SerializeField]
    private GameObject _laserPrefab;
    private int _degreesBetweenLasers = 10;
    private int _maxSpredDeg = 90;
    private float _canFireLaser;
    private float _laserFireRate;
    private float _minLaserFireRate = 1f;
    private float _maxLaserFireRate = 3f;

    [SerializeField]
    private GameObject[] _missileBanks;
    [SerializeField]
    private GameObject _missilePrefab;
    private int _degreesBetweenMissiles = 25;
    private float _canFireMissile;
    private float _missileFireRate;
    private float _minMissileFireRate = 3f;
    private float _maxMissileFireRate = 6f;

    [SerializeField]
    private GameObject[] _beams;
    private float _canFireBeam;
    private float _beamFireRate;
    private float _minBeamFireRate = 6f;
    private float _maxBeamFireRate = 9f;
    private float _minBeamDeg = 0f;
    private float _maxBeamDeg = 90f;

    [SerializeField]
    private GameObject _explosionPrefab;
    HashSet<int> _explosionPositions = new HashSet<int>();
    private WaitForSeconds _waitBetweenExplosions = new WaitForSeconds(0.5f);

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

        _canFireLaser = Time.time + _maxLaserFireRate;
        _canFireMissile = Time.time + _maxMissileFireRate;
        _canFireBeam = Time.time + _maxBeamFireRate;

        foreach (var beam in _beams)
        {
            beam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stoppedMoving)
        {
            MoveToPlace();
        }

        if (!_shields.activeSelf && Time.time > _shieldsRecharged && _health > 0)
        {
            _shields.SetActive(true);
        }

        if (Time.time > _canFireLaser && _stoppedMoving && _health > 0)
        {
            FireLaser();
        }

        if (Time.time > _canFireMissile && _stoppedMoving && _health > 0)
        {
            FireMissile();
        }

        if (Time.time > _canFireBeam && _stoppedMoving && _health > 0)
        {
            FireBeam();
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

    public void Damage()
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
            else
            {
                OnDeath();
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
            _shieldsRecharged = _shieldsRechargeTime + _health + Time.time;
        }
    }

    private void FireLaser()
    {
        _laserFireRate = Random.Range(_minLaserFireRate, _maxLaserFireRate);
        _canFireLaser = _laserFireRate + _health + Time.time;

        int degrees = 0;
        int deg = 0;
        GameObject laser;

        while (degrees <= _maxSpredDeg)
        {
            for (int i = 0; i < _laserBanks.Length; i++)
            {
                if (i % 2 == 0)
                {
                    deg = -1 * degrees;
                }
                else
                {
                    deg = degrees;
                }

                laser = Instantiate(_laserPrefab, _laserBanks[i].transform.position, Quaternion.Euler(0, 0, deg));

                laser.GetComponent<Laser>().AssignEnemyLaser();
            }

            degrees += _health * _degreesBetweenLasers;
        }
    }

    private void FireMissile()
    {
        _missileFireRate = Random.Range(_minMissileFireRate, _maxMissileFireRate);
        _canFireMissile = _missileFireRate + _health + Time.time;

        int degrees = 0;
        int deg = 0;
        GameObject missile;

        while (degrees <= _maxSpredDeg)
        {
            for (int i = 0; i < _missileBanks.Length; i++)
            {
                if (i % 2 == 0)
                {
                    deg = -1 * degrees;
                }
                else
                {
                    deg = degrees;
                }

                missile = Instantiate(_missilePrefab, _missileBanks[i].transform.position, Quaternion.Euler(0, 0, 180 + deg));

                missile.GetComponent<HomingMissile>().AssingEnemyMissile();
            }

            degrees += _health * _degreesBetweenMissiles;
        }
    }

    private void FireBeam()
    {
        _beamFireRate = Random.Range(_minBeamFireRate, _maxBeamFireRate);
        _canFireBeam = _beamFireRate + _health + Time.time;

        int direction = Mathf.RoundToInt(Random.value);
        int sign = 1;

        for (int i = 0; i < _beams.Length; i++)
        {
            if (i % 2 == 0)
            {
                sign = -1;
            }
            else
            {
                sign = 1;
            }

            if (direction == 0)
            {
                _beams[i].GetComponentInChildren<Beam>().ActivateBeam(_minBeamDeg, sign * _maxBeamDeg);
            }
            else
            {
                _beams[i].GetComponentInChildren<Beam>().ActivateBeam(sign * _maxBeamDeg, _minBeamDeg);
            }
        }
    }

    private void OnDeath()
    {
        _player.AddPoints(_points[2]);

        
        int rnd;

        while (_explosionPositions.Count < 3)
        {
            rnd = Random.Range(0, _damages.Length);
            _explosionPositions.Add(rnd);
        }

        StartCoroutine(DeathRoutine());

        Destroy(this.transform.GetComponent<SpriteRenderer>(), 0.5f);
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

        if (collision.CompareTag("Laser") && !collision.gameObject.GetComponent<Laser>().IsEnemyLaser())
        {
            if (_shields.activeSelf)
            {
                _player.AddPoints(_points[0]);
            }
            else
            {
                _player.AddPoints(_points[1]);
            }

            Destroy(collision.gameObject);

            Damage();
        }

        if (collision.CompareTag("HomingMissile") && !collision.gameObject.GetComponent<HomingMissile>().IsEnemyMissile())
        {
            if (_shields.activeSelf)
            {
                _player.AddPoints(_points[0]);
            }
            else
            {
                _player.AddPoints(_points[1]);
            }

            Damage();
        }
    }

    IEnumerator DeathRoutine()
    {
        foreach (var position in _explosionPositions)
        {
            Instantiate(_explosionPrefab, _damages[position].transform.position, Quaternion.identity);

            yield return _waitBetweenExplosions;
        }

        Destroy(this.gameObject);
    }
}
