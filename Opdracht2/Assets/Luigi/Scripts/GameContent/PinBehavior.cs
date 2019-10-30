using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBehavior : MonoBehaviour
{
    private GameObject _pin;
    [SerializeField]
    private float _shootScale;
    private ObjectPooler _pool;
    [SerializeField]
    private float _startShootTimer;
    private float _shootTimer;
    private bool _canShoot;

    void Start()
    {
        _pin = this.gameObject;
        _pool = ObjectPooler.Instance;
        _shootTimer = _startShootTimer;
        _canShoot = true;
    }
    void Update()
    {
        if (_shootTimer <= 0)
        {
            _canShoot = true;
            _shootTimer = _startShootTimer;
        }
        else
        {
            _shootTimer -= Time.deltaTime;
        }
        if (_pin.transform.localScale.y >= _shootScale && _canShoot)
        {
            _pool.SpawnFromPool("Bullet", _pin.transform.position, _pin.transform.rotation);
            _canShoot = false;
        }
    }
}
