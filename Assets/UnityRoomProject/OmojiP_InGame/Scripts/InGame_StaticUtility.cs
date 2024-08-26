using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InGame_StaticUtility
{
    // 1部屋の大きさ
    public const int ROOM_H_SIZE = 10;
    public const int ROOM_W_SIZE = 10;
    // 部屋がいくつ並んでいるか
    public const int ROOM_H_COUNT = 3;
    public const int ROOM_W_COUNT = 3;

    // 指定posにGroundしかないならtrue
    public static bool CheckExistOnlyGround(Vector3 pos)
    {
        
        Vector3 origin, direction;
        float distance = 20f;

        origin = pos + Vector3.up*10;
        direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);
        //Debug.DrawRay(origin, direction, Color.red);
        bool isHitOnlyGround = false;

        // ヒットしたオブジェクトに対して処理を行う
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy" || hit.collider.tag == "Wall" || hit.collider.tag == "Stairs")
            {
                //Debug.Log("Player or Enemy or Wall があるため false");
                return false;
            }
            else if(hit.collider.tag == "Ground")
            {
                isHitOnlyGround = true;
            }
        }

        if (isHitOnlyGround)
        {
            //Debug.Log("Player, Enemy, Wall, Stairs がなく、Groungがあるため true");
            return true;
        }

        // 何もhitしていない
        //Debug.Log("何もヒットしなかったため false");
        return false;
    }
    
    // 指定posのGameObjectをActive(false)する
    public static void SetUnActiveGameObjectAt(Vector3 pos)
    {
        Vector3 origin, direction;
        float distance = 20f;

        origin = pos + Vector3.up*10;
        direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        // ヒットしたオブジェクトに対して処理を行う
        foreach (RaycastHit hit in hits)
        {
            hit.collider.gameObject.SetActive(false);
        }
    }
    
    // 指定方向に物がないか確認する
    public static bool CheckCanMoveDirection(Vector3 arrow, Vector3 pos)
    {
        if(arrow ==  Vector3.zero) return true;

        // pos + arrowの位置に

        //Debug.Log("CheckCanMoveDirection");

        Vector3 origin, direction;
        float distance = 20f;

        origin = pos + arrow + Vector3.up*10;
        direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);
        //Debug.DrawRay(origin, direction, Color.red);
        bool isHitOnlyGround = false;

        //Debug.Log($"hits {hits.Length} , origin {origin}, dir {direction}");

        // ヒットしたオブジェクトに対して処理を行う
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy" || hit.collider.tag == "Wall")
            {
                //Debug.Log("Player or Enemy or Wall があるため false");
                return false;
            }
            else if(hit.collider.tag == "Ground" || hit.collider.tag == "Stairs")
            {
                isHitOnlyGround = true;
            }
        }

        if (isHitOnlyGround)
        {
            //Debug.Log("Player, Enemy, Wall がなく、Groungがあるため true");
            return true;
        }

        // 何もhitしていない
        //Debug.Log("何もヒットしなかったため false");
        return false;
    }
    
    // 指定方向に敵が進めるならtrue
    public static bool CheckCanMoveEnemyDirection(Vector3 arrow, Vector3 pos)
    {
        if(arrow ==  Vector3.zero) return true;

        // pos * arrowの位置が通路の入口ならfalse
        if(CheckIsRoomEntrance(pos + arrow))
        {
            Debug.Log($"通路の入口なので進めない {pos+arrow}");
            return false;
        }

        // pos + arrowの位置に物がないか確認

        Vector3 origin, direction;
        float distance = 20f;

        origin = pos + arrow + Vector3.up*10;
        direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);
        //Debug.DrawRay(origin, direction, Color.red);
        bool isHitOnlyGround = false;

        //Debug.Log($"hits {hits.Length} , origin {origin}, dir {direction}");

        // ヒットしたオブジェクトに対して処理を行う
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy" || hit.collider.tag == "Wall")
            {
                //Debug.Log("Player or Enemy or Wall があるため false");
                return false;
            }
            else if(hit.collider.tag == "Ground")
            {
                isHitOnlyGround = true;
            }
        }

        if (isHitOnlyGround)
        {
            //Debug.Log("Player, Enemy, Wall がなく、Groungがあるため true");
            return true;
        }

        // 何もhitしていない
        //Debug.Log("何もヒットしなかったため false");
        return false;
    }

    // 4方向どれかに進めるか確認する
    public static bool CheckCanMoveAnyDirection(Vector3 pos)
    {
        //Debug.Log($"全方向のどれかひとでも動けるかチェックする pos{pos}");

        if (CheckCanMoveDirection(Vector3.right, pos) || 
            CheckCanMoveDirection(Vector3.left, pos) || 
            CheckCanMoveDirection(Vector3.forward, pos) ||
            CheckCanMoveDirection(Vector3.back, pos))
        {
            return true;
        }
        
        return false;
    }
    
    // 4方向どれかに敵が進めるか確認する
    public static bool CheckCanMoveEnemyAnyDirection(Vector3 pos)
    {
        // 敵の場合は壁以外に通路の入り口にも移動できない

        bool canEnemyMoveRight = CheckCanMoveDirection(Vector3.right, pos) && !CheckIsRoomEntrance(Vector3.right + pos);
        bool canEnemyMoveLeft = CheckCanMoveDirection(Vector3.left, pos) && !CheckIsRoomEntrance(Vector3.left + pos);
        bool canEnemyMoveForward = CheckCanMoveDirection(Vector3.forward, pos) && !CheckIsRoomEntrance(Vector3.forward + pos);
        bool canEnemyMoveBack = CheckCanMoveDirection(Vector3.back, pos) && !CheckIsRoomEntrance(Vector3.back + pos);

        if (canEnemyMoveRight || 
            canEnemyMoveLeft || 
            canEnemyMoveForward ||
            canEnemyMoveBack)
        {
            return true;
        }
        
        return false;
    }

    // 指定Posに指定Tagのモノがあるか確認する
    public static bool CheckExistTagAtPosition(Vector3 pos , string tag)
    {

        Vector3 origin, direction;
        float distance = 20f;

        origin = pos + Vector3.up * 10;
        direction = Vector3.down;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        // ヒットしたオブジェクトに対して処理を行う
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == tag)
            {
                return true;
            }
        }
        return false;
    }


    // 指定Posが部屋の入口ならtrueを返す
    // 敵が部屋から出ないように & 部屋の入り口をふさがないように 使用する
    public static bool CheckIsRoomEntrance(Vector3 pos)
    {

        int roomWSize = InGame_StaticUtility.ROOM_W_SIZE;
        int roomHSize = InGame_StaticUtility.ROOM_H_SIZE;
        int roomWCount = InGame_StaticUtility.ROOM_W_COUNT;
        int roomHCount = InGame_StaticUtility.ROOM_H_COUNT;

        // 4つの通路の入口をもとめ、posと一致したらtrue
        for (int z = 0; z < roomHCount; z++)
        {
            for (int x = 0; x < roomWCount; x++)
            {
                Vector3 roomStartPos = new Vector3(x * roomWSize, 0, z * roomHSize);

                Vector3 upperEntrancePos = roomStartPos + new Vector3(roomWSize / 2, 0, roomHSize - 2);
                Vector3 lowerEntrancePos = roomStartPos + new Vector3(roomWSize / 2, 0, 1);
                Vector3 rightEntrancePos = roomStartPos + new Vector3(roomWSize - 2, 0, roomHSize / 2);
                Vector3 leftEntrancePos = roomStartPos + new Vector3(1, 0, roomHSize / 2);

                // 上端の部屋ならupperEntranceは無視
                if(z != roomHCount - 1)
                {
                    if(upperEntrancePos == pos)
                    {
                        return true;
                    }
                }
                // 下端の部屋ならlowerEntranceは無視
                if(z != 0)
                {
                    if (lowerEntrancePos == pos)
                    {
                        return true;
                    }
                }
                // 右端の部屋ならrightEntranceは無視
                if(x != roomWCount - 1)
                {
                    if (rightEntrancePos == pos)
                    {
                        return true;
                    }
                }
                // 左端の部屋ならleftEntranceは無視
                if(x != 0)
                {
                    if (leftEntrancePos == pos)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }



    // 部屋を抽選する(RoomStartPosの抽選)
    public static Vector3 GetRandomRoomStartPos()
    {
        (int roomX, int roomZ) = GetRandomRoom();

        return GetRoomStartPos(roomX, roomZ);
    }

    // 部屋を抽選する(roomx roomzの抽選)
    public static (int roomX, int roomZ) GetRandomRoom()
    {
        int rx = Random.Range(0, ROOM_W_COUNT);
        int rz = Random.Range(0, ROOM_H_COUNT);
        return (rx, rz);
    }

    // roomXZをRoomStartPosに変換
    public static Vector3 GetRoomStartPos(int roomX, int roomZ)
    {
        return new Vector3(roomX * ROOM_W_SIZE, 0, roomZ * ROOM_H_SIZE);
    }
}
