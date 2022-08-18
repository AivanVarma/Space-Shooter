using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private float _xLeftBound = -11f;
    private float _xRightBound = 11f;
    private float _yBottomBound = -10f;
    private float _yUpperBound = 10f;

    private float _speed = 5f;
    private float _homingSpeed = 7.5f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private bool _targetAcquired = false;
    private GameObject _target;

    private bool _isEnemyMissile = false;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_targetAcquired)
        {
            FollowTarget();
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.up);
        }
        
        if (transform.position.y < _yBottomBound || transform.position.y > _yUpperBound || 
            transform.position.x < _xLeftBound || transform.position.x > _xRightBound)
        {
            Destroy(gameObject);
        }
    }

    private void FollowTarget()
    {
        Vector3 direction = _target.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        transform.Translate(_homingSpeed * Time.deltaTime * Vector3.up);
    }

    private void OnHit()
    {
        _speed = 0f;
        
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !_isEnemyMissile)
        {
            OnHit();
        }

        if (collision.CompareTag("Player") && _isEnemyMissile)
        {
            collision.GetComponent<Player>().Damage();
            OnHit();
        }

        if (collision.CompareTag("Boss") && !_isEnemyMissile)
        {
            OnHit();
        }
    }

    public void TargetAcquired(GameObject target)
    {
        _targetAcquired = !_targetAcquired;
        _target = target;
    }

    public void AssingEnemyMissile()
    {
        _isEnemyMissile = true;
    }

    public bool IsEnemyMissile()
    {
        return _isEnemyMissile;
    }
}
