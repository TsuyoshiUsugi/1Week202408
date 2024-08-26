using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityRoomProject.Audio;

public class AudioTest : MonoBehaviour
{
    private AudioManager _audioManager;

    void Start()
    {
        // AudioManagerのインスタンスを取得
        _audioManager = AudioManager.Instance;

        if (_audioManager == null)
        {
            Debug.LogError("AudioManager instance not found!");
            return;
        }

        // BGMの再生テスト
        //TestPlayBGM();

        // SEの再生テスト
        //TestPlaySE();

        // Voiceの再生テスト
        //TestPlayVoice();

        // 一時停止と再開のテスト
        //TestPauseAndResume();

        // 停止のテスト
        //TestStopAudio();
    }

    public void TestPlayBGM()
    {
        Debug.Log("Playing BGM...");
        _audioManager.PlayBGM(BGMType.ジャズでモダンなフィールド); // BGMType.Track1を再生（Enumで指定）
    }

    public async void TestPlayBGMWithFade()
    {
        Debug.Log("Playing first BGM with fade...");
        await _audioManager.PlayBGMWithFade(BGMType.スタイリッシュな戦闘, 1f); // BGMType.Track1を3秒でフェードイン

        // 5秒待機して次のBGMに切り替え
        await UniTask.Delay(5000);

        Debug.Log("Switching to second BGM with fade...");
        await _audioManager.PlayBGMWithFade(BGMType.ジャズでモダンなフィールド, 2f); // BGMType.Track2に2秒でフェードイン
    }

    public void TestPlaySE()
    {
        Debug.Log("Playing SE...");
        int seIndex = _audioManager.PlaySE(SEType.EnemyAttack); // SEType.Clickを再生（Enumで指定）
        if (seIndex >= 0)
        {
            Debug.Log($"SE is playing at index {seIndex}");
        }
    }

    // public void TestPlayVoice()
    // {
    //     Debug.Log("Playing Voice...");
    //     int voiceIndex = _audioManager.PlayVoice(VoiceType.Greeting); // VoiceType.Greetingを再生（Enumで指定）
    //     if (voiceIndex >= 0)
    //     {
    //         Debug.Log($"Voice is playing at index {voiceIndex}");
    //     }
    // }

    public async void TestMultiPlaySE()
    {
        Debug.Log("Playing multiple SEs...");

        // 最初のSEクリップを再生
        int firstSEIndex = _audioManager.PlaySE(SEType.Attackbgm);
        if (firstSEIndex >= 0)
        {
            Debug.Log($"First SE is playing at index {firstSEIndex}");
        }

        int secondSEIndex = _audioManager.PlaySE(SEType.Damaged);
        if (secondSEIndex >= 0)
        {
            Debug.Log($"Second SE is playing at index {secondSEIndex}");
        }

        // 3秒待機
        await UniTask.Delay(3000); // 3秒待つ

        // 最初のSEを停止
        _audioManager.StopSE(firstSEIndex);
        Debug.Log($"Stopped SE at index {firstSEIndex}");
    }

    public async void TestPauseAndResume()
    {
        Debug.Log("Pausing audio...");
        _audioManager.Pause();

        await UniTask.Delay(3000); // 3秒待つ

        Debug.Log("Resuming audio...");
        _audioManager.Resume();
    }

    public void TestStopAudio()
    {
        Debug.Log("Stopping BGM...");
        _audioManager.StopBGM();

        Debug.Log("Stopping SE...");
        if (_audioManager.PlaySE(SEType.Attackbgm) >= 0)
        {
            _audioManager.StopSE(0);
        }

        // Debug.Log("Stopping Voice...");
        // if (_audioManager.PlayVoice(VoiceType.Greeting) >= 0)
        // {
        //     _audioManager.StopVoice(0);
        // }
    }
}
