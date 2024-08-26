using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*

入力処理？

*/

public class InGame_PlayerInput : MonoBehaviour
{
    public void Init(UnityAction<Vector2> getKeyDownArrow, UnityAction getKeyDownParry)
    {
        _getKeyDownArrow = getKeyDownArrow;
        _getKeyDownParry = getKeyDownParry;
    }

    public void ActivateInput(InGame_InputActivateType activateType)
    {
        //Debug.Log($"Input Activate {activateType}");
        _activateType = activateType;
    }

    private InGame_InputActivateType _activateType = InGame_InputActivateType.NoActivate;
    private event UnityAction<Vector2> _getKeyDownArrow;
    private event UnityAction _getKeyDownParry;

    // Update is called once per frame
    void Update()
    {
        if (_activateType == InGame_InputActivateType.NoActivate) return;


        if(_activateType == InGame_InputActivateType.PlayerTurn)
        {
            // [↑] 上に移動
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownArrow.Invoke(Vector2.up);
            }
            // [↓] 下に移動
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownArrow.Invoke(Vector2.down);
            }
            // [→] 右に移動
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownArrow.Invoke(Vector2.right);
            }
            // [←] 左に移動
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownArrow.Invoke(Vector2.left);
            }
            // [Space] その場にとどまる
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                _activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownArrow.Invoke(Vector2.zero);
            }

        }
        else if(_activateType == InGame_InputActivateType.EnemiesTurn)
        {
            // [P] パリィ
            if (Input.GetKeyDown(KeyCode.P))
            {
                //_activateType = InGame_InputActivateType.NoActivate;
                _getKeyDownParry.Invoke();
            }
        }
    }
}

public enum InGame_InputActivateType
{
    NoActivate,
    PlayerTurn,
    EnemiesTurn
}