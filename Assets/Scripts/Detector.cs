using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private Enemy _enemy;

    // Start is called before the first frame update
    void Start()
    {
        _enemy = transform.parent.GetComponent<Enemy>();

        if (_enemy == null)
        {
            Debug.LogError("Enemy is NULL!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Powerup") && transform.CompareTag("DetectPowerup"))
        {
            _enemy.PowerupDetected();
        }

        if (collision.CompareTag("Player"))
        {
            if (transform.CompareTag("PlayerBehind"))
            {
                _enemy.PlayerBehind();
            }
            else if (transform.CompareTag("RammingArea"))
            {
                _enemy.RamPlayer(collision.transform.position);
            }      
        }

        if (collision.CompareTag("Laser") && transform.CompareTag("AvoidShot"))
        {
            _enemy.AvoidShot();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && transform.CompareTag("RammingArea"))
        {
            _enemy.RamPlayer(collision.transform.position);
        }

        if (collision.CompareTag("Laser") && transform.CompareTag("AvoidShot"))
        {
            _enemy.AvoidShot();
        }
    }
}
