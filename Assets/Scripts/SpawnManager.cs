using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    private float _minSpawnTimeEnemy = 1.5f;
    private float _maxSpawnTimeEnemy = 4f;
    [SerializeField]
    private GameObject _enemyContainer;
    private int _rotationOption = 0;
    private int _maxRotationOptions = 3;
    private float _minDeg = -90f;
    private float _maxDeg = 90f;

    [SerializeField]
    private GameObject[] _powerups;
    private float _minSpawnTimePowerup = 3f;
    private float _maxSpawnTimePowerup = 7f;

    private bool _stopSpawning = false;

    private WaitForSeconds _waitBeforeSpawning = new WaitForSeconds(3f);
    private WaitForSeconds _waitBetweenWaves = new WaitForSeconds(6f);
    private WaitForSeconds _waitSpawning = new WaitForSeconds(1f);

    private int _wavesTotal = 5;
    private int _enemiesBaseAmount = 5;
    private int _enemiesSpawned = 0;

    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UIManager is NULL!");
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnWavesRoutine());
    }

    IEnumerator SpawnWavesRoutine()
    {
        int wave = 1;
        int enemiesInWave = _enemiesBaseAmount;
        bool allEnemiesSpawned = false;

        _uiManager.ShowWaveNumber(wave);

        yield return _waitBetweenWaves;

        Coroutine spawnEnemies = StartCoroutine(SpawnEnemyRoutine());
        Coroutine spawnPowerups = StartCoroutine(SpawnPowerupRoutine());

        while (wave <= _wavesTotal)
        {
            if (_enemiesSpawned == enemiesInWave && !allEnemiesSpawned)
            {
                StopCoroutine(spawnEnemies);
                allEnemiesSpawned = true;
            }

            if (_enemyContainer.transform.childCount == 0 && allEnemiesSpawned)
            {
                StopCoroutine(spawnPowerups);
                _enemiesSpawned = 0;

                if (wave < _wavesTotal)
                {
                    wave++;

                    enemiesInWave = _enemiesBaseAmount * wave;
                    allEnemiesSpawned = false;
                    _enemiesSpawned = 0;

                    _uiManager.ShowWaveNumber(wave);

                    yield return _waitBetweenWaves;

                    spawnEnemies = StartCoroutine(SpawnEnemyRoutine());
                    spawnPowerups = StartCoroutine(SpawnPowerupRoutine());
                }
            }

            yield return _waitSpawning;
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return _waitBeforeSpawning;

        float randomDeg;
        int randomShield;
        int randomEnemy;

        GameObject newEnemy = null;

        while (!_stopSpawning)
        {
            randomEnemy = Random.Range(0,_enemyPrefabs.Length);
            _rotationOption = Random.Range(0, _maxRotationOptions);

            switch (_rotationOption)
            {
                case 0:
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.identity);
                    break;
                case 1:
                    randomDeg = Random.Range(_minDeg, 0);
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.Euler(0, 0, randomDeg));
                    break;
                case 2:
                    randomDeg = Random.Range(0, _maxDeg);
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.Euler(0, 0, randomDeg));
                    break;
            }
           
            randomShield = Random.Range(0, 100);

            if (randomShield < 33)
            {
                newEnemy.GetComponent<Enemy>().ActivateShields();
            }

            newEnemy.transform.parent = _enemyContainer.transform;

            _enemiesSpawned++;

            yield return new WaitForSeconds(Random.Range(_minSpawnTimeEnemy, _maxSpawnTimeEnemy));
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return _waitBeforeSpawning;

        int randomPowerup;

        while (!_stopSpawning)
        {
            randomPowerup = Random.Range(0, _powerups.Length);

            Instantiate(_powerups[randomPowerup]);

            yield return new WaitForSeconds(Random.Range(_minSpawnTimePowerup, _maxSpawnTimePowerup));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
