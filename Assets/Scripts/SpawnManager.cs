using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    private float _spawnInterval = 1f;
    [SerializeField]
    private GameObject _enemyContainer;
    private int _rotationOption = 0;
    private int _maxRotationOptions = 3;
    private float _minDeg = -90f;
    private float _maxDeg = 90f;

    [SerializeField]
    private GameObject[] _powerups;
    private float _minSpawnTime = 3f;
    private float _maxSpawnTime = 7f;

    private bool _stopSpawning = false;

    private WaitForSeconds _waitBeforeSpawning = new WaitForSeconds(3f);

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    {
        yield return _waitBeforeSpawning;

        float randomDeg;
        int randomShield;

        GameObject newEnemy = null;

        while (!_stopSpawning)
        {
            _rotationOption = Random.Range(0, _maxRotationOptions);

            if (_rotationOption == 0)
            {
                newEnemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            }
            else if (_rotationOption == 1)
            {
                randomDeg = Random.Range(_minDeg, 0);
                newEnemy = Instantiate(_enemyPrefab, transform.position, Quaternion.Euler(0, 0, randomDeg));
            }
            else if (_rotationOption == 2)
            {
                randomDeg = Random.Range(0, _maxDeg);
                newEnemy = Instantiate(_enemyPrefab, transform.position, Quaternion.Euler(0, 0, randomDeg));
            }

            randomShield = Random.Range(0, 100);

            if (randomShield < 33)
            {
                newEnemy.GetComponent<Enemy>().ActivateShields();
            }

            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return _waitBeforeSpawning;

        while (!_stopSpawning)
        {
            int randomPowerup = Random.Range(0, _powerups.Length);

            Instantiate(_powerups[randomPowerup]);

            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
