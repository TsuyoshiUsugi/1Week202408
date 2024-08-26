using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Constant
{
    public const float PARRY_TRANSITION_SEC = 2.5f;  //エネミーが通常フィールドからパリィフィールドに移動する時間
    public const float PARRY_START_SEC = 0.5f; //パリィフィールドに移動してからパリィが始まるまでの時間
    public const float MILLI_SEC = 1000f; //ミリ秒を秒に変換するための定数
    public const float AFTER_LAST_PARRY_SEC = 2f; //最後にパリィしてから待つ時間
    public const float PARRY_SUCCESS_SPAN = 1.030f; // 前後で60ms
    public const float PARRY_JUDGE_SPAN = 1.075f; // 前後で150ms

    public const float FIELD_MOVE_SEC = 0.1f; // フィールドの移動や向きの変更にかける時間；
    public const float FIELD_MOVED_WAIT_SEC = 0.1f; // フィールドの移動後に少し待つ
}
