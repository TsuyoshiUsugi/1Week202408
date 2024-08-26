using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DungeonView : MonoBehaviour
{
    [Header("左上")]
    [SerializeField] Text _levelText;
    [SerializeField] Slider _expSlider;
    [SerializeField] Text _killCountText;
    [SerializeField] Text _hpText;
    [SerializeField] Text _atkText;
    [Header("右上")]
    [SerializeField] Text _floorText;
    [SerializeField] Button _optionButton;
    InGame_Player _player;
    [Header("参照")]
    [SerializeField] InGame_GameManager _gameManager;
    
    private void Start()
    {
        _gameManager.OnInitPlayer += Init;
    }
    
    public void Init(InGame_Player player)
    {
        _player = player;
        _player.ObserveEveryValueChanged(x => x.level).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.exp).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.maxExp).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.KillCount).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.currentHp).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.maxHp).Subscribe(_ => SetStatusView()).AddTo(_player);
        _player.ObserveEveryValueChanged(x => x.atk).Subscribe(_ => SetStatusView()).AddTo(_player);
        _gameManager.ObserveEveryValueChanged(x => x.Floor).Subscribe(SetFloorText).AddTo(_gameManager);
        SetStatusView();
    }

    private void SetStatusView()
    {
        _levelText.text = $"Lv.{_player.level}";
        _expSlider.value = _player.exp;
        _expSlider.maxValue = _player.maxExp;
        _killCountText.text = $"{_player.KillCount}";
        _hpText.text = $"{_player.currentHp}/{_player.maxHp}";
        _atkText.text = $"{_player.atk}";
    }
    
    public void SetFloorText(int floor)
    {
        _floorText.text = $"{floor}F";
    }
}
