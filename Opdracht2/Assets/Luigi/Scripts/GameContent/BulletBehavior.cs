using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody _rb;
    private float _speed = 0.5f;

    private float _destroyTimer;
    [SerializeField]
    private float _startDestroyTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _destroyTimer = _startDestroyTimer;
    }
    void Update()
    {
        if (_destroyTimer >= 0)
        {
            _rb.velocity = new Vector2(-transform.position.x, -transform.position.y) * _speed;
            _destroyTimer -= Time.deltaTime;
        }
        else
        {
            _destroyTimer = _startDestroyTimer;
            this.gameObject.SetActive(false);
        }
    }
}
