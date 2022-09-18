using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatter_Shot : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;

    private int _numberOfShots = 10;

    private float _minSpreadDeg = -60f;
    private float _maxSpreadDeg = 60f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _numberOfShots; i++)
        {
            float randomDeg = Random.Range(_minSpreadDeg, _maxSpreadDeg);

            Instantiate(_laserPrefab, transform.position, Quaternion.Euler(0,0,randomDeg));
        }

        Destroy(gameObject);
    }

}
