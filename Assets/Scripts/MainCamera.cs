using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Vector3 _cameraOriginalPosition;
    private float _shakeDuration = 0.5f, _shakeDurationLeft;
    private float _shakeMagnitude = 0.025f;
    private bool _isCameraShaking = false;

    // Start is called before the first frame update
    void Start()
    {
        _cameraOriginalPosition = transform.position;
        _shakeDurationLeft = _shakeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCameraShaking)
        {
            ShakeCamera();
        }
    }

    private void ShakeCamera()
    {
        if (_shakeDurationLeft > 0)
        {
            transform.position = transform.position + _shakeMagnitude * Random.insideUnitSphere;
            _shakeDurationLeft -= Time.deltaTime;
        }
        else
        {
            _isCameraShaking = false;
            transform.position = _cameraOriginalPosition;
        }
    }

    public void PlayerDamageShake()
    {
        _isCameraShaking = true;
        _shakeDurationLeft = _shakeDuration;
    }
}
