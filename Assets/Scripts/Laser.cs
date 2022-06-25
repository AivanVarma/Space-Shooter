using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _speed = 10f;

    private float _yUpperBound = 8.0f;
    private float _yLowerBound = -6f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_isEnemyLaser)
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.up);
        }
        

        if (transform.position.y > _yUpperBound || transform.position.y < _yLowerBound)
        {
            if (transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = collision.gameObject.GetComponent<Player>();

            player.Damage();

            Destroy(this.gameObject);
        }
    }
}
