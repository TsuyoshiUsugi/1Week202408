using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private InGame_Enemy _enemyPrefab;
    private InGame_ParryManager _parryManager;
    private void Start()
    {
        _parryManager = FindObjectOfType<InGame_ParryManager>();
        _parryManager.Init(FindObjectOfType<InGame_Player>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _enemyPrefab.EnemyAction(_parryManager).Forget();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _parryManager.TryParryPlayer();
        }
    }
}
