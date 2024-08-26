using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

初期化処理やステージの遷移を行う

*/
public class InGame_GameManager : MonoBehaviour
{
    [SerializeField] InGame_CameraManager cameraManager;
    [SerializeField] InGame_FieldManager fieldManager;
    [SerializeField] InGame_StairsManager stairsManager;
    [SerializeField] InGame_PlayerInput playerInput;
    [SerializeField] InGame_PlayerManager playerManager;
    [SerializeField] InGame_RoomManager roomManager;
    [SerializeField] InGame_ParryManager parryManager;
    [SerializeField] ParryView parryView;
    [SerializeField] InGame_FieldData testFieldData;
    public int Floor { get; private set; } = 1;
    private InGame_TurnManager turnManager;
    public event Action<InGame_Player> OnInitPlayer;

    private void Start()
    {
        StartStage( GetFieldData(1) );
    }


    private void StartStage(InGame_FieldData fieldData)
    {
        //ステージ生成
        //マップ生成
        fieldManager.GenerateMap(fieldData);
        //階段生成
        stairsManager.GenerateStairs(fieldData);
        //Player生成
        (int roomX, int roomZ) = playerManager.GeneratePlayer(fieldData);
        //RoomManager経由で敵生成
        roomManager.GenerateEnemies(fieldData, parryManager);
        //ParryManagerにPlayerをセット
        parryManager.Init(playerManager.Player);
        OnInitPlayer?.Invoke(playerManager.Player);
        parryView.SetPlayer(playerManager.Player);

        //PlayerのいるRoomをActivateする
        roomManager.ActivateEnemyManager(roomX,roomZ);
        //PlayerのいるRoomにカメラを移動する
        cameraManager.MoveToCamera(roomX, roomZ);

        //ターンの開始
        turnManager = new(playerInput, playerManager, roomManager, parryManager, cameraManager, ClearFloor);
        turnManager.StartTurn();
    }

    // フロアに応じたFieldDataを受け取る
    private InGame_FieldData GetFieldData(int floor)
    {
        Floor = floor;
        return testFieldData;
    }

    // フロアをクリアしたときに呼ばれる
    //  TurnManagerからUnityAction経由
    private void ClearFloor()
    {
        Debug.Log("Floor Clear!!");

        // クリア演出

        // 暗転させてる間に今のステージを消す


        DestroyStage();

        // 次のフロアへ
        StartStage(GetFieldData(++Floor));
    }

    private void DestroyStage()
    {
        //ステージ削除
        //マップ削除
        fieldManager.DestroyMap();
        //階段削除
        stairsManager.DestroyStairs();
        //Player削除(Poolした方がいいかも？)
        playerManager.DestroyPlayer();
        //RoomManager経由で敵削除
        roomManager.DestroyInRoomer();

    }

}
