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

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("The Enemy prefab is NULL!");
        }

        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_spawnInterval);

            GameObject newEnemy = Instantiate(_enemyPrefab);
            newEnemy.transform.parent = _enemyContainer.transform;
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

            int randomPowerup = Random.Range(0, 2);

            Instantiate(_powerups[randomPowerup]);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
