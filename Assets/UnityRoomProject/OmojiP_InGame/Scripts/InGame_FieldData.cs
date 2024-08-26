using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

フィールドのデータ
ScriptableObject?

敵の種類 InGame_Enemy[] (Prefabs)
敵の出現比 float[]
敵の密度 int

障害物の密度 int

背景テーマ ScriptableObject

*/


[CreateAssetMenu(fileName = "New FieldData", menuName = "StageDatas/FieldData")]
public class InGame_FieldData : ScriptableObject
{
    [Header("敵の種類")]
    public InGame_Enemy[] enemys;
    [Header("敵の出現比")]
    public float[] enemysAppearanceRatio;
    //[Header("敵の密度[%]")]
    //public int[] enemysDensity;
    [Header("一部屋の敵の最大数")]
    public int enemyMax;
    [Header("一部屋の敵の最小数")]
    public int enemyMin;
    //[Header("障害物の密度[%]")]
    //public int[] obstacleDensity;
    [Header("一部屋の障害物の最大数")]
    public int obstacleMax;
    [Header("一部屋の障害物の最小数")]
    public int obstacleMin;
    
    [Header("背景テーマ")]
    public InGame_BackgroundTheme backgroundTheme;

}

/*

GenerateMapで部屋を作って部屋間のかべを開ける処理を実装しました
あとは、

・プレイヤーを生成する
・敵が部屋から出られないようにする
・障害物をfieldDataに従って通路をふさがないように生成する
・敵をfieldDataに従って生成する
・階段を生成する
あたりの処理が必要かと思います

土日には見た目をいじるフェースに行きたいのでここら辺の処理を金曜日にやりたいです


*/