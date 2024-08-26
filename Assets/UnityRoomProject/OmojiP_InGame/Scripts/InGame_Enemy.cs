using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRoomProject.Audio;

/*

敵

攻撃、移動などを行う
攻撃する際はParryManagerの関数を呼び出すこと

*/
[RequireComponent(typeof(Animator))]
public class InGame_Enemy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float overtunTime = 0.5f; // パリィのタイミングをずらす時間
    [SerializeField] BGMType bgmType = BGMType.ジャズでモダンなフィールド; // 攻撃時のBGM

    // parryできる時間の配列

    [Header("attackMoveDurationの長さにattackStartSecの長さが自動で合わせられます")]
    [Tooltip("StartParryTimeから見た攻撃の動き出しの開始時間")]
    public float[] attackStartSec = { 0.5f }; // StartParryTimeから見た攻撃の動き出しの開始時間
    [Tooltip("attackStartSecから何秒後に攻撃がPlayerに当たるか")]
    public float[] attackMoveDuration = { 1f}; // attackStartSecから何秒後に攻撃がPlayerに当たるか

    public GameObject parryEffectPrefab;
    public int[] atks = { 1 };
    public int maxHp; //敵のHP
    public int currentHp; //敵のHP
    public bool isAlive;
    public float OverTurnTime => overtunTime;
    public BGMType BGMType => bgmType;
    
    private void Start()
    {
        if(atks.Length != attackMoveDuration.Length)
        {
            Debug.LogError("parryTimesとatksの要素数を一致させてください");
        }

        isAlive = true;
    }

    public async UniTask EnemyAction(InGame_ParryManager inGame_ParryManager)
    {
        // 攻撃圏内ならAttack
        // 視界圏内ならMoveToPlayer
        // それ以外ならMoveRandom

        // float[] = {0f, 1.5f }
        // Animationからトリガーで関数を呼び出す

        if(CheckPlayerExist(transform, out Vector3 playerPos))
        {
            await Attack(inGame_ParryManager);
        }
        else
        {
            await Move();
        }
    }


    public async UniTask Attack(InGame_ParryManager inGame_ParryManager)
    {
        /*
        Attack

        ParryManager(Model)にパリィに必要なタイミング情報を渡す
        ParryViewer(View)にパリィ演出に必要な敵情報・敵アニメーション・タイミング情報等を渡す

        */

        //animator.SetTrigger("Enemy Attack");
        await inGame_ParryManager.StartParryTime(this);

    }

    public async UniTask Move()
    {
        /*
        Move
        // 視界圏内ならMoveToPlayer
        // それ以外ならMoveRandom
        */

        //RandomMove
        // arrowを抽選

        Vector3 arrow = Vector3.zero;

        //全ての方向が移動できない場合は移動しない
        if (InGame_StaticUtility.CheckCanMoveEnemyAnyDirection(transform.position))
        {
            while (true)
            {
                int _r = Random.Range(0, 4);
                float _x = Mathf.PI / 2 * _r;
                arrow = new Vector3(Mathf.Cos(_x), 0, Mathf.Sin(_x));

                if (InGame_StaticUtility.CheckCanMoveEnemyDirection(arrow, transform.position))
                {
                    break;
                }
            }


            //arrowの方向を向き、移動アニメーションを流しながら移動

            // arrowの方向にオブジェクトを向かせる
            await transform.DORotateQuaternion(Quaternion.LookRotation(arrow), Constant.FIELD_MOVE_SEC).AsyncWaitForCompletion(); ;

            // 進みはじめるときに"isMove"をtrueにする
            //animator.SetBool("isMove", true);

            // その方向に1だけ進む
            await transform.DOMove(transform.position + arrow.normalized, Constant.FIELD_MOVE_SEC).AsyncWaitForCompletion(); ;

            // 進み終わったら"isMove"をfalseにする
            //animator.SetBool("isMove", false);

        }
        else
        {
            Debug.Log("動けないので動かない");
        }

        await UniTask.Delay(1);
    }


    public void Damaged(int damage)
    {
        currentHp -= damage;

        if(currentHp <= 0)
        {
            // 実際に倒れるのはパリィが全て終わった後？
            isAlive = false;
        }
    }


    // Enemyの周囲5x5にPlayerがいればtrueを返す
    // 汎用的な形にしてStaticUtilityに移動する
    bool CheckPlayerExist(Transform transform, out Vector3 playerPos)
    {
        Vector3 origin, direction;
        float distance = 20f;

        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                if (x == 0 && z == 0) continue;

                Vector3 moveVec = new Vector3(x, 0, z);
                origin = transform.position + moveVec + Vector3.up*10;
                direction = Vector3.down;

                RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Player")
                    {
                        playerPos = hit.collider.transform.position;
                        return true;
                    }
                }
            }
        }

        playerPos = Vector3.zero;
        return false;

    }


#if UNITY_EDITOR
    public void OnValidate()
    {
        if (attackStartSec.Length != attackMoveDuration.Length || atks.Length != attackMoveDuration.Length)
        {
            Debug.LogWarning("attackMoveDurationまたはatksの長さがattackStartSecの長さと一致しないため、attackStartSecの長さに合わせます。");

            float[] newAttackMoveDuration = new float[attackStartSec.Length];
            for (int i = 0; i < newAttackMoveDuration.Length; i++)
            {
                newAttackMoveDuration[i] = i < attackStartSec.Length ? attackStartSec[i] : 0.5f;
            }
            attackStartSec = newAttackMoveDuration;

            int[] newAtks = new int[attackStartSec.Length];
            for (int i = 0; i < newAtks.Length; i++)
            {
                newAtks[i] = i < atks.Length ? atks[i] : 1;
            }
            atks = newAtks;
        }
    }
#endif
}