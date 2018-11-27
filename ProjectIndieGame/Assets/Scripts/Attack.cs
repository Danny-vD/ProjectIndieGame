﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Range(1,2)]
    [SerializeField] int _playerID = 1;
    [SerializeField] float _attackCooldown = 0.5f;
    private float _timer = 0;
    public float Force = 10;

    void Update()
    {
        if (Pause.Paused)
        {
            return;
        }

        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        if (Input.GetButton("Fire1") || Input.GetButtonDown("RightBumper_P" + _playerID))
        {
            transform.root.GetComponentInChildren<PlayerSound>().playAttackingSound();
            SetCooldown();
            transform.GetChild(0).gameObject.SetActive(true);
            Invoke("DisableHitBox", 0.25f);
        }
    }

    void DisableHitBox()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetCooldown()
    {
        _timer = _attackCooldown;
    }
}
