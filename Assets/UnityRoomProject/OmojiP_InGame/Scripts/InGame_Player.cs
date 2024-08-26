using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

プレイヤーの行動等

*/

public class InGame_Player : MonoBehaviour
{
    public int currentHp; // 現在のHP
    public int maxHp; // 最大HP
    public int atk;
    public int level;
    public int exp;
    public int maxExp;
    public int KillCount;
    [SerializeField] private AnimationCurve _levelRequireExpCurve;
    [SerializeField] private AnimationCurve _levelUpAtkCurve;
    [SerializeField] private int _levelUpHealAmount;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public async UniTask Move(Vector3 arrow)
    {
        if(arrow == Vector3.zero)
        {
            //Idle処理
        }
        else
        {
            //arrowの方向を向き、移動アニメーションを流しながら移動
            //Debug.Log($"移動：{arrow}");
            
            // arrowの方向にオブジェクトを向かせる
            await transform.DORotateQuaternion(Quaternion.LookRotation(arrow), Constant.FIELD_MOVE_SEC).AsyncWaitForCompletion(); ;

            // 進みはじめるときに"isMove"をtrueにする
            animator.SetBool("Run", true);
            Debug.Log(animator.GetBool("Run"));
            // その方向に1だけ進む
            await transform.DOMove(transform.position + arrow.normalized, Constant.FIELD_MOVE_SEC).AsyncWaitForCompletion(); ;

            // 進み終わったら"isMove"をfalseにする
            animator.SetBool("Run", false);

            await UniTask.Delay((int)(Constant.FIELD_MOVED_WAIT_SEC * 1000));
        }

    }

    // Roomの移動時に使用
    public async UniTask MoveTo(Vector3 addPos)
    {

        //nextへの方向を向き、移動アニメーションを流しながら移動

        // 進みはじめるときに"isMove"をtrueにする
        animator.SetBool("isMove", true);

        // その方向に1だけ進む
        await transform.DOMove(transform.position + addPos, 0.1f).AsyncWaitForCompletion();

        // 進み終わったら"isMove"をfalseにする
        animator.SetBool("isMove", false);

    }


    public void Damaged(int damage)
    {
        Debug.Log($"{gameObject.name} に {damage} のダメージ");
        currentHp -= damage;

        if (currentHp <= 0)
        {
            // 実際に倒れるのはパリィが全て終わった後？
            Debug.Log("hp0 倒れる");
        }
    }
    
    public void AddExp(int exp)
    {
        this.exp += exp;
        if(this.exp >= maxExp)
        {
            LevelUp();
        }
    }
    
    public void LevelUp()
    {
        level++;
        maxExp = (int)_levelRequireExpCurve.Evaluate(level);
        exp = 0;
        currentHp = Mathf.Min(currentHp + _levelUpHealAmount, maxHp);
        atk += (int)_levelUpAtkCurve.Evaluate(level);
    }

    private void OnDestroy()
    {
        Debug.Log("Playerが破棄されました");
    }
}
