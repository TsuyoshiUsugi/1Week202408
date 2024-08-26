using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_StairsManager : MonoBehaviour
{
    // 階段を生成する

    //[SerializeField] InGame_Stairs stairsPrefab;
    InGame_Stairs _stairs;
    public InGame_Stairs Stairs { get { return _stairs; } }

    // fieldManagerから生成
    public void GenerateStairs(InGame_FieldData fieldData)
    {
        while (true)
        {
            // 生成する部屋を決める
            Vector3 roomStartPos = InGame_StaticUtility.GetRandomRoomStartPos();
            // 生成位置を決める
            Vector3 pos = roomStartPos + new Vector3(Random.Range(1, InGame_StaticUtility.ROOM_W_SIZE - 1), 0, Random.Range(1, InGame_StaticUtility.ROOM_H_SIZE - 1));
            // 他のものと重なってなければ生成
            if (InGame_StaticUtility.CheckExistOnlyGround(pos) && !InGame_StaticUtility.CheckIsRoomEntrance(pos))
            {
                InGame_StaticUtility.SetUnActiveGameObjectAt(pos);
                _stairs = Instantiate(fieldData.backgroundTheme.stairs, pos, Quaternion.identity, this.transform);
                break;
            }
        }
    }

    // 次のステージに向けて現在の階段を破棄する
    public void DestroyStairs()
    {
        Destroy(_stairs.gameObject);
    }
}
