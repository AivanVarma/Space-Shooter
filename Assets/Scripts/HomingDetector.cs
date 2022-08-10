using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingDetector : MonoBehaviour
{
    private HomingMissile _homingMissile;
    private List<Collider2D> _enemies = new List<Collider2D> ();

    private bool _isEnemyMissile = false;

    // Start is called before the first frame update
    void Start()
    {
        _homingMissile = transform.parent.GetComponent<HomingMissile> ();

        if (_homingMissile == null)
        {
            Debug.LogError("HomingMissile is NULL!");
        }

        _isEnemyMissile = _homingMissile.IsEnemyMissile();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateClosestEnemy();
    }

    private void CalculateClosestEnemy()
    {
        if (_enemies.Count > 0)
        {
            float closest = 10000000f;
            Collider2D closestEnemy = new Collider2D();

            foreach (Collider2D enemy in _enemies)
            {
                if (enemy != null)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distance < closest)
                    {
                        closest = distance;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                _homingMissile.TargetAcquired(closestEnemy.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !_isEnemyMissile)
        {
            _enemies.Add(collision);
        }

        if (collision.CompareTag("Player") && _isEnemyMissile)
        {
            _homingMissile.TargetAcquired(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !_isEnemyMissile)
        {
            _homingMissile.TargetAcquired(collision.gameObject);
        }  

        if (collision.CompareTag("Player") && _isEnemyMissile)
        {
            _homingMissile.TargetAcquired(collision.gameObject);
        }
    }
}
