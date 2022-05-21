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

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("The Enemy prefab is NULL!");
        }

        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_spawnInterval);

            GameObject newEnemy = Instantiate(_enemyPrefab);
            newEnemy.transform.parent = _enemyContainer.transform;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
