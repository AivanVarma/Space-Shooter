using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    private float _spawnInterval = 1f;

    [SerializeField]
    private GameObject[] _powerups;
    private float _minSpawnTime = 3f;
    private float _maxSpawnTime = 7f;

    [SerializeField]
    private GameObject _enemyContainer;

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

        while (!_stopSpawning)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab);
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
