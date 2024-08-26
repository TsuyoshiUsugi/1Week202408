using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_RoomManager : MonoBehaviour
{
    /*

    RoomごとにEnemyManagerで部屋の敵を管理

    Roomからプレイヤーが出たらEnemyの機能を停止してActiveRoomを切り替える

    切り替えたことを通知する形でCameraの位置を変える

    */

    [SerializeField] InGame_EnemyManager enemyManagerPrefab;
    [SerializeField] Transform eneManParent;

    //現在のActiveな部屋
    (int x,int z) activeRoomIndex = (0,0);

    InGame_EnemyManager[,] enemyManagers = 
        new InGame_EnemyManager[InGame_StaticUtility.ROOM_W_COUNT, InGame_StaticUtility.ROOM_H_COUNT];

    public InGame_EnemyManager ActiveEnemyManager 
    { 
        get 
        {
            return enemyManagers[activeRoomIndex.x, activeRoomIndex.z];
        }
    }

    public void GenerateEnemies(InGame_FieldData fieldData, InGame_ParryManager parryManager)
    {
        enemyManagers =
            new InGame_EnemyManager[InGame_StaticUtility.ROOM_W_COUNT, InGame_StaticUtility.ROOM_H_COUNT];

        for (int z = 0; z < enemyManagers.GetLength(1); z++)
        {
            for (int x = 0; x < enemyManagers.GetLength(0); x++)
            {
                Vector3 roomStartPos = new Vector3(x*InGame_StaticUtility.ROOM_W_SIZE, 0, z * InGame_StaticUtility.ROOM_H_SIZE);
                enemyManagers[x, z] = Instantiate(enemyManagerPrefab, Vector3.zero, Quaternion.identity, eneManParent);
                enemyManagers[x, z].GenerateEnemy(fieldData, parryManager, roomStartPos);
            }
        }
    }

    public void ActivateEnemyManager(int activateX, int activateZ)
    {
        bool isActive;
        activeRoomIndex = (activateX, activateZ);

        for (int z = 0; z < enemyManagers.GetLength(1); z++)
        {
            for (int x = 0; x < enemyManagers.GetLength(0); x++)
            {
                isActive = false;

                if(x == activateX && z == activateZ)
                {
                    isActive = true;
                }

                enemyManagers[x, z].AvtivateEnemyManager(isActive);
            }
        }
    }


    // Playerが移動によって部屋を出た場合、Playerとカメラの位置の増分を求め、ActiveEnemyManagerを変更する
    public bool IsPlayerOutRoom(out Vector3 addPlayerPos, out Vector3 addCameraPos)
    {
        Debug.Log("部屋の移動確認");

        addPlayerPos = Vector3.zero;
        addCameraPos = Vector3.zero;

        if (CheckPlayerOutRoom(out var nextRoomArrow))
        {
            // PlayerとCameraのposの移動させる分のVectorを求める
            addPlayerPos = nextRoomArrow * 2;
            if(nextRoomArrow == Vector3.right || nextRoomArrow == Vector3.left)
            {
                addCameraPos = nextRoomArrow * InGame_StaticUtility.ROOM_W_SIZE;
            }
            else
            {
                addCameraPos = nextRoomArrow * InGame_StaticUtility.ROOM_H_SIZE;
            }

            // EnemyManagerのActiveを変更
            ActivateEnemyManager(
                activeRoomIndex.x + (int)nextRoomArrow.x,
                activeRoomIndex.z + (int)nextRoomArrow.z);

            return true;
        }

        return false;

    }


    // playerが部屋から出たならtrueを返し、行先の部屋を伝える
    private bool CheckPlayerOutRoom(out Vector3 nextRoomArrow)
    {
        //Debug.Log($"PlayerOutRoomCheck activex:{activeRoomIndex.x}, activez:{activeRoomIndex.z}");

        int roomWSize = InGame_StaticUtility.ROOM_W_SIZE;
        int roomHSize = InGame_StaticUtility.ROOM_H_SIZE;

        nextRoomArrow = Vector3.zero;

        // playerが部屋の境目にいるか確認

        // avtiveな部屋の roomStartPos から四隅の通路を調べ、Playerがいればtrue
        Vector3 roomStartPos = new Vector3(activeRoomIndex.x * roomWSize, 0, activeRoomIndex.z * roomHSize);
        Vector3 upperSidePos = roomStartPos + new Vector3(roomWSize / 2, 0, roomHSize -1);
        Vector3 lowerSidePos = roomStartPos + new Vector3(roomWSize / 2, 0, 0);
        Vector3 rightSidePos = roomStartPos + new Vector3(roomWSize -1, 0, roomHSize / 2);
        Vector3 leftSidePos = roomStartPos + new Vector3(0, 0, roomHSize / 2);

        if (InGame_StaticUtility.CheckExistTagAtPosition(upperSidePos, "Player"))
        {
            nextRoomArrow = Vector3.forward;
            return true;
        }
        else if (InGame_StaticUtility.CheckExistTagAtPosition(lowerSidePos, "Player"))
        {
            nextRoomArrow = Vector3.back;
            return true;
        }
        else if (InGame_StaticUtility.CheckExistTagAtPosition(rightSidePos, "Player"))
        {
            nextRoomArrow = Vector3.right;
            return true;
        }
        else if (InGame_StaticUtility.CheckExistTagAtPosition(leftSidePos, "Player"))
        {
            nextRoomArrow = Vector3.left;
            return true;
        }


        return false;
    }

    // 次のステージに向けて現在のEnemyManagerたちを破棄する
    public void DestroyInRoomer()
    {
        // eneManParentの子要素をすべて削除
        foreach (Transform child in eneManParent)
        {
            Destroy(child.gameObject);
        }
        enemyManagers = null;
    }
}
