using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private float _yUpperBound = 8f;
    private float _yLowerBound = -8f;

    private float _speed = 5f;
    private float _homingSpeed = 10f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private bool _targetAcquired = false;
    private GameObject _target;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_targetAcquired)
        {
            Vector3 direction = _target.transform.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            transform.Translate(_homingSpeed * Time.deltaTime * Vector3.up);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.up);
        }
        
        if (transform.position.y < _yLowerBound || transform.position.y > _yUpperBound)
        {
            Destroy(gameObject);
        }
    }

    private void OnHit()
    {
        _speed = 0f;
        
        //Destroy(this.GetComponentInChildren<HomingDetector>());
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            OnHit();
        }
    }

    public void TargetAcquired(GameObject target)
    {
        _targetAcquired = !_targetAcquired;
        _target = target;
    }
}
