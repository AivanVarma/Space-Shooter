using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private float _speed = 0f;
    private Quaternion _startingRotation;
    private Quaternion _endingRotation;
    private bool _isBeamActive = false;
    [SerializeField]
    private Transform _parentTrans;

    private void Update()
    {
        if (_isBeamActive)
        {
            RotateBeam();
        }
    }

    private void RotateBeam()
    {
        _parentTrans.rotation = Quaternion.Slerp(_startingRotation, _endingRotation, _speed);
        
        if (Mathf.Approximately(_parentTrans.rotation.z, _endingRotation.z))
        {
            _parentTrans.gameObject.SetActive(false);
            _isBeamActive = false;
            _speed = 0f;
        }
        else
        {
            _speed = _speed + Time.deltaTime;
        }
    }

    public void ActivateBeam(float startRotation, float endRotation)
    {
        _isBeamActive = true;

        _startingRotation = Quaternion.Euler(0f,0f,startRotation);
        _endingRotation = Quaternion.Euler(0f,0f,endRotation);

        _parentTrans.rotation = _startingRotation;

        _parentTrans.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().Damage();
        }

        if (collision.CompareTag("Powerup"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Laser"))
        {
            if (!collision.GetComponent<Laser>().IsEnemyLaser())
            {
                Destroy(collision.gameObject);
            }
        }

        if (collision.CompareTag("HomingMissile"))
        {
            if (!collision.GetComponent<HomingMissile>().IsEnemyMissile())
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
