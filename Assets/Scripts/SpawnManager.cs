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
    private float _minAbsDeg = 15f;
    private float _maxAbsDeg = 90f;
    private int[] _enemyWeights = { 20, 1, 5, 5, 5 }; // Basic, Teleporter, Aggressive, Smart, Avoid Shot
    private int _enemyWeightsSum;
    private int[] _rotationWeights = { 20, 5, 5 }; // Top-down, Left-right, Right-left
    private int _rotationWeightsSum;

    [SerializeField]
    private GameObject[] _powerups;
    private float _minSpawnTimePowerup = 3f;
    private float _maxSpawnTimePowerup = 7f;
    private int[] _powerupWeights = { 6, 6, 3, 1, 20, 2, 2, 1 }; // Triple shot, Speed, Shield, Health, Ammo, Scatter shot, Negative Speed, Missiles
    private int _powerupWeightsSum;

    private bool _stopSpawning = false;

    private WaitForSeconds _waitBeforeSpawningEnemies = new WaitForSeconds(1.5f);
    private WaitForSeconds _waitBeforeSpawningPowerups = new WaitForSeconds(3f);
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

        _enemyWeightsSum = WeightsSum(_enemyWeights);
        _powerupWeightsSum = WeightsSum(_powerupWeights);
        _rotationWeightsSum = WeightsSum(_rotationWeights);
    }

    private int WeightsSum(int[] weights)
    {
        int sum = 0;

        foreach (int weight in weights)
        {
            sum += weight;
        }

        return sum;
    }

    private int RouletteSelector(int[] weights, int weightsSum)
    {
        int i;

        int rndWeight = Random.Range(0, weightsSum);

        for (i = 0; i < weights.Length; i++)
        {
            if (rndWeight < weights[i])
            {
                break;
            }

            rndWeight -= weights[i];
        }

        return i;
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
        yield return _waitBeforeSpawningEnemies;

        float randomDeg;
        int randomShield;
        int randomEnemy;
        int rotationOption;

        GameObject newEnemy = null;

        while (!_stopSpawning)
        {
            randomEnemy = RouletteSelector(_enemyWeights, _enemyWeightsSum);
            rotationOption = RouletteSelector(_rotationWeights, _rotationWeightsSum);

            switch (rotationOption)
            {
                case 0:
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.identity);
                    break;
                case 1:
                    randomDeg = Random.Range(- _maxAbsDeg, - _minAbsDeg);
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.Euler(0, 0, randomDeg));
                    break;
                case 2:
                    randomDeg = Random.Range(_minAbsDeg, _maxAbsDeg);
                    newEnemy = Instantiate(_enemyPrefabs[randomEnemy], transform.position, Quaternion.Euler(0, 0, randomDeg));
                    break;
            }
           
            randomShield = Random.Range(0, 100);

            if (randomShield < 20)
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
        yield return _waitBeforeSpawningPowerups;

        int randomPowerup;

        while (!_stopSpawning)
        {
            randomPowerup = RouletteSelector(_powerupWeights, _powerupWeightsSum);

            Instantiate(_powerups[randomPowerup]);

            yield return new WaitForSeconds(Random.Range(_minSpawnTimePowerup, _maxSpawnTimePowerup));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
