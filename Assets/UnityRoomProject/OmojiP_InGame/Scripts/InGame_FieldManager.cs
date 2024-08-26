using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

初期化処理のタイミングでステージ生成


ステージ生成時にEnemyManagerの敵の生成処理を呼び出す

ステージ生成

・ステージObjeの配置
・Playerの生成(とりあえずpoolとか考えない)
・敵の生成(EnemyManager経由)

*/

public class InGame_FieldManager : MonoBehaviour
{
    // 1部屋の大きさ
    int roomHSize = InGame_StaticUtility.ROOM_H_SIZE;
    int roomWSize = InGame_StaticUtility.ROOM_W_SIZE;
    // 部屋がいくつ並んでいるか
    int roomHCount = InGame_StaticUtility.ROOM_H_COUNT;
    int roomWCount = InGame_StaticUtility.ROOM_W_COUNT;

    //[SerializeField] GameObject groundObj;
    //[SerializeField] GameObject wallObj;
    [SerializeField] Transform mapsParent;

    // fieldDataからマップを生成
    public void GenerateMap(InGame_FieldData fieldData)
    {

        /*
        10x10の部屋(動ける範囲は8x8)を3x3で並べる
        横の部屋に行けるように穴をあける
        fieldDataにしたがって障害物を置く
        */

        for (int h = 0; h < roomHCount; h++)
        {
            for (int w = 0; w < roomWCount; w++)
            {
                Vector3 roomStartPos = new Vector3(w * roomWSize, 0, h * roomHSize);
                GenerateRoom(roomStartPos, fieldData);
            }
        }

    }

    void GenerateRoom(Vector3 roomStartPos, InGame_FieldData fieldData)
    {

        //障害物を obstacleMin~Maxの間で生成
        int obstaclesLength = Random.Range(fieldData.obstacleMin, fieldData.obstacleMax + 1);
        int obstaclesCount = 0;
        Vector3[] obstaclesPositions = new Vector3[obstaclesLength];

        while (true)
        {
            var randomPos = new Vector3(Random.Range(1, roomWSize-1), 0, Random.Range(1, roomHSize-1));
            Vector3 instantiatePos = roomStartPos + randomPos;

            //かぶった or 通路をふさぐ位置なら 再抽選
            if ( InGame_StaticUtility.CheckExistTagAtPosition(instantiatePos, "Wall") || InGame_StaticUtility.CheckIsRoomEntrance(instantiatePos))
            {
                // かぶったので再抽選
                continue;
            }
            else
            {
                // かぶってないので生成する
                obstaclesCount++;
                Instantiate(fieldData.backgroundTheme.obstacles, instantiatePos, Quaternion.identity, mapsParent);
            }

            if(obstaclesCount >= obstaclesLength)
            {
                break;
            }
        }


        for (int z = 0; z < roomHSize; z++)
        {
            for (int x = 0; x < roomWSize; x++)
            {
                Vector3 instantiatePos = roomStartPos + new Vector3(x, 0, z);

                // xかzの壁の真ん中
                if (
                    ((x == 0 || x == roomWSize - 1) && z == roomHSize / 2) || // xの壁の真ん中
                    ((z == 0 || z == roomHSize - 1) && x == roomWSize / 2) // zの壁の真ん中
                    )
                {
                    // 壁に穴をあけるか
                    if (CheckIsNoWall(roomStartPos, x, z))
                    {
                        Instantiate(fieldData.backgroundTheme.ground, instantiatePos, Quaternion.identity, mapsParent);
                    }
                    else
                    {
                        Instantiate(fieldData.backgroundTheme.wall, instantiatePos, Quaternion.identity, mapsParent);
                    }
                }
                // 真ん中以外の壁
                else if (x == 0 || x == roomWSize - 1 || z == 0 || z == roomHSize - 1)
                {
                    Instantiate(fieldData.backgroundTheme.wall, instantiatePos, Quaternion.identity, mapsParent);
                }
                // 床
                else
                {
                    Instantiate(fieldData.backgroundTheme.ground, instantiatePos, Quaternion.identity, mapsParent);
                }

            }
        }
        
    }

    // 次のステージに向けて現在のマップを破棄する
    public void DestroyMap()
    {
        // mapParentの子要素をすべて削除
        foreach (Transform child in mapsParent)
        {
            Destroy(child.gameObject);
        }
    }

    // 部屋の仕切りの壁を開けるかどうかをstartRoomPosから判定
    bool CheckIsNoWall(Vector3 roomStartPos, int x, int z)
    {
        // x正側の壁について、x正側に部屋がないなら、x正側の壁は開けない
        if (x == roomWSize-1 && roomStartPos.x == roomWSize * (roomWCount-1))
        {
            return false;
        }
        // x負側の壁について、x負側に部屋がないなら、x負側の壁は開けない
        if (x == 0 && roomStartPos.x == 0)
        {
            return false;
        }
        // z正側の壁について、z正側に部屋がないなら、z正側の壁は開けない
        if (z == roomHSize-1 && roomStartPos.z == roomHSize * (roomHCount-1))
        {
            return false;
        }
        // z負側の壁について、z負側に部屋がないなら、z負側の壁は開けない
        if (z == 0 && roomStartPos.z == 0)
        {
            return false;
        }

        //それ以外なら壁に穴をあける
        return true;

    }
}