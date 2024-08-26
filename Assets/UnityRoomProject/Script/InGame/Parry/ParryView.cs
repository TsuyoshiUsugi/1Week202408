using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityRoomProject.Audio;

public class ParryView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InGame_ParryManager _parryManager;
    [SerializeField] private RenderTexture _parryViewRenderTexture;
    private InGame_Player _player;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private ResultView _resultView;
    [SerializeField] private RectTransform _playerHpGauge;
    [SerializeField] private RectTransform _enemyHpGauge;
    [SerializeField] private RectTransform _border;
    [SerializeField] private Transform[] _scrollField;
    [Header("parrySettings")]
    [SerializeField] private float _modelTransitionHeight = 10;
    [SerializeField] private Transform _parryViewEnemyPosition;
    [SerializeField] private Transform _parryViewPlayerPosition;
    [SerializeField] private Transform _parryEffectInstantiatePosition;
    [SerializeField] private RawImage _parryViewImage;
    [Header("LifeGauge")]
    [SerializeField, FormerlySerializedAs("_lifeGaugeParent")] private RectTransform _playerLifeGaugeParent;
    [SerializeField, FormerlySerializedAs("_lifeGaugeImage")] private Image _playerLifeGaugeImage;
    [SerializeField] private RectTransform _enemyLifeGaugeParent;
    [SerializeField] private Image _enemyLifeGaugeImage;
    [SerializeField]private List<Image> _playerLifeGaugeImages = new ();
    [SerializeField] private List<Image> _enemyLifeGaugeImages = new ();  
    [SerializeField] private List<GameObject> _currentParryEffects = new ();
    [Header("パリィ画面のサイズ設定")]
    private float _parryViewExpandSize = 400;
    private float _parryViewExpandPosY = 200;
    private float _parryViewNormalSize = 270;
    private float _parryViewNormalPosY = 135;
    private float _currentAttackEffectMoveDuration;
    private int _currentEnemyHp;
    private Tween _currentTween;

    private Vector3 _originalEnemyPosition; // 敵の上画面上の位置を記憶する
    private Vector3 _previousPlayerPosition;
    private float _fieldSize;

    private void Start()
    {
        _parryManager.OnStartParryTime += StartParryTime;
        _parryManager.OnTryParry += () =>
        {
            _playerAnimator.SetTrigger("Attack");
        };
        _parryManager.OnParrySuccess += async () =>
        {
            //パリィ成功時の演出
            AudioManager.Instance.PlaySE(SEType.ParrySuccess);
            //_currentTween.Kill();
            if (_currentAttackEffectMoveDuration < 0)
            {
                Debug.LogError("パリィエフェクトの時間が設定されていません");
            }
            else
            {
                var currentParryEffect = _currentParryEffects.First();
                _currentParryEffects.RemoveAt(0);
                await currentParryEffect.transform.DOMove(_parryEffectInstantiatePosition.position, _currentAttackEffectMoveDuration).SetLink(gameObject)
                    .OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySE(SEType.Damaged);
                        Destroy(currentParryEffect);
                        ShowEnemyLife(_currentEnemyHp -= _player.atk);
                    })
                    .SetEase(Ease.Linear)
                    .ToUniTask();
            }
        };
        _parryManager.OnParryFail += () =>
        {
            //パリィ失敗時の演出
            AudioManager.Instance.PlaySE(SEType.Damaged);
            var currentParryEffect = _currentParryEffects.First();
            _currentParryEffects.RemoveAt(0);
            if (_player.currentHp <= 0)
            {
                _playerAnimator.SetBool("Death", true);
                _resultView.gameObject.SetActive(true);
                _resultView.ShowResult();
            }
            else
            {
                _playerAnimator.SetTrigger("Damaged");
            }
            Destroy(currentParryEffect);
        };
        _fieldSize = _scrollField[0].GetComponent<BoxCollider>().size.x;
    }

    public void SetPlayer(InGame_Player player)
    {
        _player = player;
        _player.ObserveEveryValueChanged(_ => (player.currentHp, player.maxHp)).Subscribe(x => ShowPlayerHp(x.currentHp, x.maxHp)).AddTo(_player);
        ShowPlayerHp(_player.currentHp, _player.maxHp);

        // 上画面のPlayerの移動を検知して下画面のPlayerをアニメーションさせる
        _previousPlayerPosition = _player.transform.position;
        _player.UpdateAsObservable().Subscribe(_ =>
        {
            Vector3 currentPosition = _player.transform.position;
            bool isMoving = currentPosition != _previousPlayerPosition;
            _playerAnimator.SetBool("Run", isMoving);
            if (isMoving)
            {
                // Playerの動きに合わせて下画面の背景を動かす
                ScrollField(Vector3.Distance(currentPosition, _previousPlayerPosition));
            }
            _previousPlayerPosition = currentPosition;
        }).AddTo(this);
    }

    // 下画面の背景をスクロールさせる
    private void ScrollField(float scrollValue)
    {
        var limit = -61;
        foreach (var field in _scrollField)
        {
            field.position -= Vector3.forward * scrollValue;
            if (field.position.z < limit)
            {
                field.position += Vector3.forward * _fieldSize;
            }
        }
    }

    // PlayerのHPを下画面のバーに表示
    private void ShowPlayerHp(int currentHp, int maxHp)
    {

        //HPバーのimage数をmaxHpに合わせる
        if (maxHp >= _playerLifeGaugeImages.Count)
        {
            for (var i = _playerLifeGaugeImages.Count; i <= maxHp; i++)
            {
                var image = Instantiate(_playerLifeGaugeImage, _playerLifeGaugeParent);
                _playerLifeGaugeImages.Add(image);
            }
        }
        else
        {
            for (var i = _playerLifeGaugeImages.Count; i >= maxHp; i--)
            {

                Destroy( _playerLifeGaugeImages[^1].gameObject);
                _playerLifeGaugeImages.RemoveAt(_playerLifeGaugeImages.Count - 1);
            }
        }


        for (var i = 0; i < _playerLifeGaugeImages.Count; i++)
        {
            _playerLifeGaugeImages[i].gameObject.GetComponent<Image>().enabled = i < currentHp;
        }
    }

    // EnemyのHPを下画面のバーに表示
    private void ShowEnemyLife(int hp)
    {
        //if (hp > _enemyLifeGaugeImages.Count)
        //{
        //    for (var i = _enemyLifeGaugeImages.Count; i < hp; i++)
        //    {
        //        var image = Instantiate(_enemyLifeGaugeImage, _enemyLifeGaugeParent);
        //        _enemyLifeGaugeImages.Add(image);
        //    }
        //}
        for (int i = 0; i < _enemyLifeGaugeImages.Count; i++)
        {
            _enemyLifeGaugeImages[i].gameObject.GetComponent<Image>().enabled = i < hp;
        }
    }

    // もとの敵のHPバーを消し、次の敵のHPバーを生成
    // 最大HPと現在のHP両方使う or 最大HPまでImageを生成してShowEnemyHpのほうで現在のHPは表現する の方が良さそう
    private void SetEnemyHpView(InGame_Enemy enemy)
    {
        foreach (var image in _enemyLifeGaugeImages)
        {
            Destroy(image.gameObject);
        }

        _enemyLifeGaugeImages.Clear();

        for (int i = 0; i < enemy.maxHp; i++)
        {
            var image = Instantiate(_enemyLifeGaugeImage, _enemyLifeGaugeParent);
            _enemyLifeGaugeImages.Add(image);
        }
    }

    // 下画面を広げる,狭める
    private void SetRendererSize(bool setExpand)
    {
        return;
        var expandNum = 117;
        var expandTime = 1; // 広げる,狭めるにかける時間
        var borderMovePosY = 130;
        if (setExpand)
        {
            _parryViewRenderTexture.Release();
            _parryViewRenderTexture.height = 400;
            _parryViewRenderTexture.Create();
            _border.DOAnchorPosY(_border.anchoredPosition.y + borderMovePosY, expandTime);
            _playerHpGauge.DOAnchorPosY(_enemyHpGauge.anchoredPosition.y + expandNum, expandTime);
            _enemyHpGauge.DOAnchorPosY(_enemyHpGauge.anchoredPosition.y + expandNum, expandTime);
            _parryViewImage.rectTransform.DOAnchorPosY(_parryViewExpandPosY, expandTime);
            _parryViewImage.rectTransform.DOSizeDelta(new Vector2(_parryViewImage.rectTransform.sizeDelta.x, _parryViewExpandSize), expandTime);
        }
        else
        {
            _parryViewRenderTexture.Release();
            _parryViewRenderTexture.height = 270;
            _parryViewRenderTexture.Create();
            _border.DOAnchorPosY(_border.anchoredPosition.y - borderMovePosY, expandTime);
            _playerHpGauge.DOAnchorPosY(_enemyHpGauge.anchoredPosition.y - expandNum, expandTime);
            _enemyHpGauge.DOAnchorPosY(_enemyHpGauge.anchoredPosition.y - expandNum, expandTime);
            _parryViewImage.rectTransform.DOAnchorPosY(_parryViewNormalPosY, expandTime);
            _parryViewImage.rectTransform.DOSizeDelta(new Vector2(_parryViewImage.rectTransform.sizeDelta.x, _parryViewNormalSize), expandTime);
        }
    }

    // ParryManagerのStartParryTimeから発火する
    private async void StartParryTime(InGame_Enemy enemy)
    {
        //下画面を広げる
        SetRendererSize(true);
        // 敵の攻撃BGMをかける
        AudioManager.Instance.PlayBGM(enemy.BGMType);

        // 敵の上画面上の位置を記憶する
        _originalEnemyPosition = enemy.transform.position;
        // 敵の開始時のHPを代入
        _currentEnemyHp = enemy.currentHp;

        // 敵のhpに合わせてHPバーをセットし、現在のHPを表示する
        SetEnemyHpView(enemy);
        ShowEnemyLife(enemy.currentHp);

        //エネミーのモデルをパリィViewの位置に移動させる
        var sequence = DOTween.Sequence();
        var moveSec = Constant.PARRY_TRANSITION_SEC - Constant.PARRY_START_SEC;
        enemy.transform.DORotate(new Vector3(0, 135, 0), Constant.PARRY_TRANSITION_SEC, RotateMode.FastBeyond360)
            .SetLink(gameObject);
        sequence.Append(enemy.transform.DOMoveY(_modelTransitionHeight, moveSec / 2).SetLink(gameObject));
        sequence.AppendCallback(() => enemy.transform.position = _parryViewEnemyPosition.position + Vector3.up * _modelTransitionHeight);
        sequence.Append(enemy.transform.DOMove(_parryViewEnemyPosition.position, moveSec / 2).SetLink(gameObject));
        await sequence.Play();
        //ここまでで敵の移動完了

        await UniTask.Delay(System.TimeSpan.FromSeconds(Constant.PARRY_START_SEC));

        float allWaitedTime = 0f;

        //パリィエフェクトを生成し、プレイヤーの位置に移動させる
        for (var i = 0; i < enemy.attackStartSec.Length; i++)
        {
            // attackStartSecまで待つ
            var attackStartSec = enemy.attackStartSec[i];
            await UniTask.Delay(TimeSpan.FromSeconds(attackStartSec - allWaitedTime)) ;
            allWaitedTime = attackStartSec;

            _currentAttackEffectMoveDuration = enemy.attackMoveDuration[i];

            // 敵の攻撃エフェクトを生成
            var parryEffect = Instantiate(enemy.parryEffectPrefab, _parryEffectInstantiatePosition.position, Quaternion.identity);

            // 敵の攻撃エフェクトをリストに追加　パリィ成功・失敗時にリストからRemoveされる
            _currentParryEffects.Add(parryEffect);

            // AttackSEを鳴らす
            AudioManager.Instance.PlaySE(SEType.EnemyAttack);

            // エフェクトを _parryViewPlayerPosition まで attackMoveDuration 秒かけて移動させる
            _currentTween = parryEffect.transform
                .DOMove(_parryViewPlayerPosition.position, enemy.attackMoveDuration[i]).SetLink(gameObject)
                .SetEase(Ease.Linear);
            _currentTween.Play();
        }

        // 最後の_currentAttackEffectMoveDuration と AFTER_LAST_PARRY_SEC 秒待つ
        await UniTask.Delay(TimeSpan.FromSeconds(_currentAttackEffectMoveDuration*2 + Constant.AFTER_LAST_PARRY_SEC));
        
        // 画面を狭める
        SetRendererSize(false);
        //Debug.Log("終了");
        AudioManager.Instance.StopBGM();
        // 敵をもとの位置に戻す
        ReturnEnemyToOriginalPosition(enemy);
    }

    // 敵をもとの位置に戻す
    private void ReturnEnemyToOriginalPosition(InGame_Enemy enemy)
    {
        var sequence = DOTween.Sequence();
        sequence = DOTween.Sequence();
        sequence.Append(enemy.transform.DOMoveY(_modelTransitionHeight, Constant.PARRY_TRANSITION_SEC / 2).SetLink(gameObject));
        sequence.AppendCallback(() => enemy.transform.position = _originalEnemyPosition + Vector3.up * _modelTransitionHeight);
        sequence.Append(enemy.transform.DOMove(_originalEnemyPosition, Constant.PARRY_TRANSITION_SEC / 2).SetLink(gameObject));
        sequence.Play();
    }

    private void Reset()
    {
        _parryManager = FindObjectOfType<InGame_ParryManager>();
    }
}
