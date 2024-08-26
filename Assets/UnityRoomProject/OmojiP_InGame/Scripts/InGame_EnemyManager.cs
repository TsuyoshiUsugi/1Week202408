using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
一部屋の敵の生成処理や参照の管理RoomManagerによって９部屋分のEnemyManagerが管理される

このGameObjectもPrefab化する

*/
public class InGame_EnemyManager : MonoBehaviour
{
    //[SerializeField] InGame_Enemy enemy;
    List<InGame_Enemy> enemyList;
    InGame_ParryManager _parryManager;
    // fieldManagerから生成
    public void GenerateEnemy(InGame_FieldData fieldData, InGame_ParryManager parryManager, Vector3 roomStartPos)
    {
        _parryManager = parryManager;

        // Playerの近くには配置しないとか、fieldDataを基に配置するとかは後で考える

        // 生成する敵の数を決める
        int enemyLimit = Random.Range(fieldData.enemyMin, fieldData.enemyMax + 1);
        int enemyInstantiateCount = 0;
        enemyList = new();

        while (true)
        {
            Vector3 pos = roomStartPos + new Vector3(Random.Range(1, InGame_StaticUtility.ROOM_W_SIZE - 1), 0, Random.Range(1, InGame_StaticUtility.ROOM_H_SIZE - 1));
            // 他のものと重なってなければ生成
            if (InGame_StaticUtility.CheckExistOnlyGround(pos) && !InGame_StaticUtility.CheckIsRoomEntrance(pos))
            {

                var e = Instantiate(fieldData.enemys[Random.Range(0, fieldData.enemys.Length)], pos, Quaternion.identity, this.transform);
                enemyList.Add(e);

                enemyInstantiateCount++;
                if (enemyInstantiateCount >= enemyLimit)
                {
                    break;
                }
            }
        }

        
    }
    // 敵の行動を順番に処理
    public async UniTask EnemiesTurn()
    {
        foreach (var enemy in enemyList)
        {
            if (!enemy.isAlive) continue;

            await enemy.EnemyAction(_parryManager);
        }
        await UniTask.Delay(1);
    }


    public void AvtivateEnemyManager(bool isActive)
    {
        // 敵をSetActive(false)


    }
}