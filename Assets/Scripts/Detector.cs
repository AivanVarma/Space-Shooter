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
        if (collision.CompareTag("Powerup"))
        {
            _enemy.PowerupDetected();
        }

        if (collision.CompareTag("Player"))
        {
            _enemy.RamPlayer(collision.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _enemy.RamPlayer(collision.transform.position);
        }
    }
}
