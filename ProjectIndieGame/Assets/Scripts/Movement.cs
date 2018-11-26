﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject Head;

    private Rigidbody _rigidBody;
    private ScreenShake _screenShake;
    private PlayerStatus _playerStatus;

    [SerializeField] private float _speed = 0.2f;
    [SerializeField] private float _maxSpeed = 50f;
    [SerializeField] private float _bendingPower = 1f;
    [SerializeField] private float _bendingPowerDecrease = 0.5f;
    [SerializeField] private int _receivingDamage= 5;
    [Range(1, 2)]
    [SerializeField] private int _playerID = 1;
    public bool CANTMOVE = false;

    private Attack _attackScript;

    private Vector3 forward;
    private Vector3 backward;
    private Vector3 left;
    private Vector3 right;

    private Vector3 _walkVelocity;
    private Vector3 _flyVelocity;

    private Vector2 _velocity;
    private Vector2 _lateVelocity;
    private Vector2 _normal;

    void Start()
    {
        _attackScript = Head.GetComponent<Attack>();
        _rigidBody = GetComponent<Rigidbody>();
        _screenShake = Camera.main.GetComponent<ScreenShake>();
        _playerStatus = GetComponent<PlayerStatus>();

        forward = new Vector3(0, 0, _speed);
        backward = new Vector3(0, 0, -_speed);
        left = new Vector3(-_speed, 0, 0);
        right = new Vector3(_speed, 0, 0);

        _velocity = Vector2.zero;
        _lateVelocity = Vector2.zero;
        _normal = Vector2.zero;
    }

    void Update()
    {
        if (CANTMOVE)
        {
            return;
        }

        _walkVelocity = Vector3.zero;
        //_walkVelocity *= 0.95f;

        if (Input.GetKey(KeyCode.W) || Input.GetAxis("LeftVertical_P" + _playerID) > 0)
        {
            _walkVelocity += forward;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetAxis("LeftVertical_P" + _playerID) < 0)
        {
            _walkVelocity += backward;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetAxis("LeftHorizontal_P" + _playerID) < 0)
        {
            _walkVelocity += left;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetAxis("LeftHorizontal_P" + _playerID) > 0)
        {
            _walkVelocity += right;
        }

        if (_walkVelocity.magnitude > _speed)
        {
            _walkVelocity = _walkVelocity.normalized;
            _walkVelocity = _walkVelocity * _speed;
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(_walkVelocity.magnitude);
        //print("VELOCITY: " + _rigidBody.velocity);
        if (_rigidBody.velocity.magnitude > _speed && _walkVelocity.magnitude <= 0f)
        {
            _rigidBody.velocity *= 0.99f;
        }
        else if (_rigidBody.velocity.magnitude > _speed)
        {
            _rigidBody.velocity *= 0.99f;
            _rigidBody.velocity = Vector3.RotateTowards(_rigidBody.velocity, _walkVelocity, Time.deltaTime * (_bendingPower - _bendingPowerDecrease / _maxSpeed * _rigidBody.velocity.magnitude), 0);
        }
        else if (_walkVelocity.magnitude <= 0f)
        {
            _rigidBody.velocity *= 0.9f;
        }
        else
        {
            _rigidBody.velocity = _walkVelocity;
        }


        if (_rigidBody.velocity.magnitude > _maxSpeed)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _maxSpeed;
        }

        _lateVelocity.x = _rigidBody.velocity.x;
        _lateVelocity.y = _rigidBody.velocity.z;
    }

    private void reflect(Vector3 pNormal)
    {
        StartCoroutine(Camera.main.GetComponent<ScreenShake>().Shake(0.2f, 0.1f));
        _normal.Set(pNormal.x, pNormal.z);
        _velocity = Vector2.Reflect(_lateVelocity, _normal);
        Vector3 vector = new Vector3(_velocity.x, 0, _velocity.y);
        _rigidBody.velocity = vector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(_rigidBody.velocity + " - " + _rigidBody.velocity.magnitude + "mag" + _speed);
        reflect(collision.contacts[0].normal);
    }

    private void OnTriggerEnter(Collider pOther)
    {
        if (pOther.gameObject.tag.ToUpper() == "WEAPON")
        {
            StartCoroutine(_screenShake.Shake(0.2f, 0.1f + _playerStatus.GetDamage() / 300f)); //shake the screen depending on damage
            _playerStatus.IncreaseDamage(_receivingDamage);

            _attackScript.SetCooldown();
            pOther.gameObject.SetActive(false);
            Head.transform.GetChild(0).gameObject.SetActive(false);

            Vector3 delta = transform.position - pOther.gameObject.transform.position;
            delta = delta.normalized * pOther.GetComponentInParent<Attack>().Force;
            delta.y = 0;
            //Debug.Log("DAMAGE:" + delta);
            _rigidBody.velocity = delta * _playerStatus.GetDamage();
            //Debug.Log(_playerStatus.GetDamage() / 5f);
        }
    }
}
