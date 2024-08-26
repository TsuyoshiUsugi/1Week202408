using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_CameraManager : MonoBehaviour
{
    [SerializeField] GameObject fieldCamera;

    [SerializeField] Vector3 room00CameraPos;

    // 変化を足すことで移動
    public async UniTask MoveCamera(Vector3 addPos)
    {
        await fieldCamera.transform.DOMove(fieldCamera.transform.position + addPos, 1f).AsyncWaitForCompletion();
    }

    // 指定roomに移動
    public void MoveToCamera(int roomX, int roomZ)
    {
        Vector3 pos = room00CameraPos + new Vector3(InGame_StaticUtility.ROOM_W_SIZE * roomX, 0, InGame_StaticUtility.ROOM_H_SIZE * roomZ);
        fieldCamera.transform.position = pos;
        //await fieldCamera.transform.DOMove(pos, 0.1f).AsyncWaitForCompletion();
    }
}
