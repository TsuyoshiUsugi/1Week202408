using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityRoomProject.Audio;

/*

パリィ処理を行うモデル部分

*/
public class InGame_ParryManager : MonoBehaviour
{
    /*
    
    PlayerからのParryしたという情報

    EnemyからのParryのタイミング情報

    からParryの成功 or 失敗を判断

    */

    ParryTiming[] _parryTimings;
    float _parryStartTime;
    bool isActivate = false;

    [SerializeField] Image parryTimeImage;

    InGame_Player _player;
    InGame_Enemy _enemy;
    
    public event Action<InGame_Enemy> OnStartParryTime; 
    public event Action OnParrySuccess;
    public event Action OnParryFail;
    public event Action OnTryParry;
    
    public float OffsetTime { get; set; }

    public void Init(InGame_Player player)
    {
        _player = player;
    }


    // 範囲内のparrytimeの数をカウントする テスト用
    int parryInSpanCount = 0;

    private void Update()
    {
        // parry中のみActivateする
        if (!isActivate) return;

        parryInSpanCount = 0;

        // parryされていなく、かつ、現在時間が PARRY_JUDGE_SPANの範囲を超えて通り過ぎたParrytimeは失敗とする
        float nowTime = Time.time - _parryStartTime;

        for (int i = 0; i < _parryTimings.Length; i++)
        {

            if (!_parryTimings[i].isJudged && ((nowTime - (_parryTimings[i].attackStartSec + _parryTimings[i].attackMoveDuration)) >= Constant.PARRY_JUDGE_SPAN))
            {
                // isJudged = trueにする
                _parryTimings[i].isJudged = true;
                Debug.Log("通り過ぎたのでParry失敗");
                FailParry(_enemy.atks[i]);
            }

            // 成功範囲内なら表示を出してみる（テスト用）
            if (!_parryTimings[i].isJudged && (Mathf.Abs(nowTime - (_parryTimings[i].attackStartSec + _parryTimings[i].attackMoveDuration)) <= Constant.PARRY_SUCCESS_SPAN))
            {
                parryInSpanCount++;
                //Debug.Log("Parry Chance!!!");
                parryTimeImage.color = Color.yellow;
            }
        }

        if(parryInSpanCount <= 0)
        {
            parryTimeImage.color = Color.black;
        }

    }

    // playerinputでパリィされたときに呼ばれる
    public void TryParryPlayer()
    {
        AudioManager.Instance.PlaySE(SEType.TryParry);
        // parrytimeスタートからの時間
        float nowTime = Time.time - _parryStartTime + OffsetTime;
        OnTryParry?.Invoke();

        for (int i = 0; i < _parryTimings.Length; i++)
        {
            //parryされていなく、かつ、時間が PARRY_JUDGE_SPANの範囲に収まっていればParryの成功・失敗を判定する
            if (!_parryTimings[i].isJudged && (Mathf.Abs(nowTime - (_parryTimings[i].attackStartSec + _parryTimings[i].attackMoveDuration)) <= Constant.PARRY_JUDGE_SPAN))
            {
                // isJudged = trueにする
                _parryTimings[i].isJudged = true;

                // nowTimeとparryTimeの時間差が PARRY_SUCCESS_SPAN 以下なら成功
                if (Mathf.Abs(nowTime - (_parryTimings[i].attackStartSec + _parryTimings[i].attackMoveDuration)) <= Constant.PARRY_SUCCESS_SPAN)
                {
                    Debug.Log("Parry成功");
                    SuccessParry();
                }
                else
                {
                    Debug.Log("Parryしたが、成功範囲外だったのでParry失敗");
                    FailParry(_enemy.atks[i]);
                }
                break;
            }
        }

    }

    // 敵からパリィ開始時に呼ばれる
    public async UniTask StartParryTime(InGame_Enemy enemy)
    {
        _enemy = enemy;
        var attackStartSec = enemy.attackStartSec;
        var attackMoveDuration = enemy.attackMoveDuration;
        if (attackMoveDuration.Length == 0)
        {
            Debug.LogWarning("enemyのparryTimesの要素が空です");
            return;
        }
        
        _parryTimings = new ParryTiming[attackStartSec.Length];
        for (int i = 0; i < _parryTimings.Length; i++)
        {
            _parryTimings[i] = new ParryTiming(attackStartSec[i], attackMoveDuration[i]);
        }
        OnStartParryTime?.Invoke(enemy);
        await UniTask.Delay(System.TimeSpan.FromSeconds(Constant.PARRY_TRANSITION_SEC));
        _parryStartTime = Time.time;
        isActivate = true;
        //各StartTimeとParryTimeを足して、最後の要素の指定時間後まで待機
        await UniTask.Delay(System.TimeSpan.FromSeconds((attackStartSec.Last() + attackMoveDuration.Last()*2 + Constant.AFTER_LAST_PARRY_SEC)));
        // ParryのActivateをfalseにする
        isActivate = false;
        _enemy = null;
        _parryTimings = null;
        //await UniTask.Delay(System.TimeSpan.FromSeconds(Constant.AFTER_LAST_PARRY_SEC));
    }


    // パリィ失敗
    void FailParry(int atk)
    {
        // playerがダメージを受ける
        _player.Damaged(atk);
        OnParryFail?.Invoke();
    }


    // パリィ成功
    void SuccessParry()
    {
        // 敵がダメージを受ける
        _enemy.Damaged(_player.atk);
        OnParrySuccess?.Invoke();
    }


    class ParryTiming
    {
        public float attackStartSec;
        public float attackMoveDuration;
        // StartParryTimeから何秒後にパリィするか（Playerのところに攻撃が来るか）
        public float parryTimingFromStartParry;
        public bool isJudged;

        public ParryTiming(float _attackStartSec, float _attackMoveDuration)
        {
            isJudged = false;
            attackStartSec = _attackStartSec;
            attackMoveDuration = _attackMoveDuration;
            parryTimingFromStartParry = _attackStartSec + _attackMoveDuration;
        }
    }
}
