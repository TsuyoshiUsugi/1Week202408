using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_PlayerManager : MonoBehaviour
{
    [SerializeField] InGame_Player playerPrefab;
    InGame_Player _player;
    public InGame_Player Player { get { return _player; } }

    // fieldManagerから生成
    public (int roomX, int roomZ) GeneratePlayer(InGame_FieldData fieldData)
    {
        while (true)
        {
            // 生成する部屋を決める
            (int roomX, int roomZ) = InGame_StaticUtility.GetRandomRoom();
            Vector3 roomStartPos = InGame_StaticUtility.GetRoomStartPos(roomX, roomZ);
            // 生成位置を決める
            Vector3 pos = roomStartPos + new Vector3(Random.Range(1, InGame_StaticUtility.ROOM_W_SIZE - 1), 0, Random.Range(1, InGame_StaticUtility.ROOM_H_SIZE - 1));
            // 他のものと重なってなければ生成
            if (InGame_StaticUtility.CheckExistOnlyGround(pos) && !InGame_StaticUtility.CheckIsRoomEntrance(pos))
            {
                Debug.Log(_player);
                if(_player == null)
                {
                    _player = Instantiate(playerPrefab, pos, Quaternion.identity, this.transform);
                    Debug.Log(_player.transform.position);
                }
                else
                {
                    _player.transform.position = pos;
                    _player.transform.rotation = Quaternion.identity;
                    _player.gameObject.SetActive(true);
                }

                return (roomX, roomZ);
            }
        }
    }

    public async UniTask MovePlayer(Vector3 arrow)
    {
        // arrowの方向が移動できるかどうかは事前にチェックを入れる
        await _player.Move(arrow);
    }

    public bool CheckIsOverStairs()
    {
        // Playerが階段の上にいるかどうかを調べる
        return InGame_StaticUtility.CheckExistTagAtPosition(Player.transform.position, "Stairs");
    }

    public async UniTask MoveToNextRoom(Vector3 addPos)
    {
        await _player.MoveTo(addPos);
    }

    // 次のステージに向けて現在のプレイヤーを破棄する
    public void DestroyPlayer()
    {
        _player.gameObject.SetActive(false);
        //Destroy(_player.gameObject);
    }
}