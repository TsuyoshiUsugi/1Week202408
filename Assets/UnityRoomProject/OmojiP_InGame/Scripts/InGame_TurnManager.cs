using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/*

プレイヤーや敵の行動を管理
EnemyManagerの敵のリストを見て行動処理を呼び出す

*/
public class InGame_TurnManager
{

    public InGame_TurnManager(
        InGame_PlayerInput playerInput, 
        InGame_PlayerManager playerManager, 
        InGame_RoomManager roomManager, 
        InGame_ParryManager parryManager,
        InGame_CameraManager cameraManager,
        UnityAction clearFloor
        )
    {
        _playerInput = playerInput;
        _playerManager = playerManager;
        _roomManager = roomManager;
        _parryManager = parryManager;
        _cameraManager = cameraManager;

        _clearFloor = clearFloor;
    }

    InGame_PlayerInput _playerInput;
    InGame_PlayerManager _playerManager;
    InGame_RoomManager _roomManager;
    InGame_ParryManager _parryManager;
    InGame_CameraManager _cameraManager;

    UnityAction _clearFloor;

    //PlayerTurnなら
    // PlayerInput を確認
    // Playerの行動処理
    // EnemiesTurnへ

    //EnemiesTurnなら
    // EnemyManagerからEnemyごとにアクション
    // 　ParryManagerを通してParry処理、ParryViewerからParryのView処理
    //    ParryManagerからPlayerInputを確認し、パリィしたらParry成功をParrayManagerで処理する
    // PlayerTurnへ

    // ステージ生成等の初期化処理が終わった後呼ばれる
    public void StartTurn()
    {
        // PlayerInputの初期化
        _playerInput.Init(GetKeyDownArrow, GetKeyDownParry);

        //PlayerTurnへ
        PlayerTurn();
    }

    void PlayerTurn()
    {
        _playerInput.ActivateInput(InGame_InputActivateType.PlayerTurn);
    }


    async UniTask EnemiesTurn()
    {
        _playerInput.ActivateInput(InGame_InputActivateType.EnemiesTurn);
        //await _enemyManager.EnemiesTurn();
        await _roomManager.ActiveEnemyManager.EnemiesTurn();
    }


    void GetKeyDownArrow(Vector2 arrow)
    {
        // arrow方向に移動してEnemyTurnへ
        GetKeyDownAsync(arrow).Forget();
    }

    async UniTask GetKeyDownAsync(Vector2 arrow)
    {
        // playerがその方向に移動できるかチェックする
        Vector3 arrow3 = new Vector3(arrow.x, 0, arrow.y);
        if (InGame_StaticUtility.CheckCanMoveDirection(arrow3, _playerManager.Player.transform.position))
        {
            //Debug.Log("player 移動できるので移動");
            await _playerManager.MovePlayer(arrow3);
            if(_playerManager.Player == null) return;

            // Stairsの上ならステージを移動する
            if (_playerManager.CheckIsOverStairs())
            {
                Debug.Log("階段についたのでステージを移動する");
                _clearFloor.Invoke();
                return;
            }

            // Playerが部屋から出たならカメラやPlayerを移動する
            if (_roomManager.IsPlayerOutRoom(out var addPlayerPos, out var addCameraPos))
            {
                Debug.Log("部屋を移動");

                UniTask playerMoveTask = _playerManager.MoveToNextRoom(addPlayerPos);
                UniTask cameraMoveTask = _cameraManager.MoveCamera(addCameraPos);

                // 2つのタスクが完了するのを待つ
                await UniTask.WhenAll(playerMoveTask, cameraMoveTask);
                
            }

            // 敵のターンへ
            await EnemiesTurn();
        }
        else
        {
            Debug.Log("player 移動できないのでやり直し");
        }

        PlayerTurn();
    }

    void GetKeyDownParry()
    {
        // parry
        Debug.Log("PlayerがParryしようとしている...!");

        // ParryManagerにPlayerがparryしたことを伝える
        _parryManager.TryParryPlayer();

    }
}
