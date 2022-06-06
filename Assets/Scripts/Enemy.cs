using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 3.5f;
    private float _deathSpeed = 0.75f;

    private float _xLeftBound = -9.2f;
    private float _xRightBound = 9.2f;
    private float _yBottomBound = -6f;
    private float _yUpperBound = 8f;

    private Player _player;
    
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = RandomXPosition();

        _player = GameObject.Find("Player").GetComponent<Player>();

        if ( _player == null )
        {
            Debug.LogError("The Player is NULL!");
        }

        _anim = this.GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
       Movement();
    }

    private Vector3 RandomXPosition()
    {
        float xRandom = Random.Range(_xLeftBound, _xRightBound);
        return new Vector3(xRandom, _yUpperBound, 0);
    }

    private void Movement()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < _yBottomBound)
        {
            transform.position = RandomXPosition();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            OnEnemyDeath();
        }

        if (collision.CompareTag("Laser"))
        {            
            Destroy(collision.gameObject);

            _player.AddPoints(10);

            OnEnemyDeath();
        }
    }

    private void OnEnemyDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _speed = _deathSpeed;
        Destroy(this.gameObject, 2.8f);
    }
}
