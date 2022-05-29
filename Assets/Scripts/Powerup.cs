using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private float _xLeftBound = -7.2f;
    private float _xRightBound = 7.2f;
    private float _yBottomBound = -4f;
    private float _yUpperBound = 8f;
    
    private float _speed = 3f;

    [SerializeField] // Triple shot = 0, Speed = 1, Shield = 2 
    private int _powerupID;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(_xLeftBound, _xRightBound), _yUpperBound, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (transform.position.y < _yBottomBound)
        {
            Destroy(this.gameObject);
        }

        transform.Translate(_speed * Time.deltaTime * Vector3.down);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();

            if (player != null)
            {
                switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        break;
                }
                
            }
            
            Destroy(this.gameObject);
        }
    }
}
