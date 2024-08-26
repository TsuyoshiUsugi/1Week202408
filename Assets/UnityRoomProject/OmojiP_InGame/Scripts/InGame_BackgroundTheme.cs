using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// 背景や障害物などの見た目
[CreateAssetMenu(fileName ="New BackgroundThemaData", menuName = "StageDatas/BackgroundThemaData")]
public class InGame_BackgroundTheme : ScriptableObject
{
    public GameObject obstacles;
    public GameObject wall;
    public GameObject ground;
    public InGame_Stairs stairs;
}
