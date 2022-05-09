using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _speed = 10.0f;

    private float _yUpperBound = 8.0f;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);

        if (transform.position.y > _yUpperBound)
        {
            Destroy(this.gameObject);
        }
    }
}
