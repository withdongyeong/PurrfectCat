using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioAnalyzerLauncher : MonoBehaviour
{
    public enum FpsOption { Fps5, Fps10 }
    public FpsOption selectedFps = FpsOption.Fps5;

    public TMP_Text logText;
    public Button exitButton;

    private Process process;

    private void Awake()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(QuitGame);
    }

    private void Start()
    {
        string exePath = GetExecutablePath();
        AppendLog($"[실행 시도] {exePath}");

        if (!File.Exists(exePath))
        {
            AppendLog("[에러] 실행 파일이 존재하지 않습니다.");
            return;
        }

        try
        {
            process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    AppendLog("[콘솔] " + e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            AppendLog("[성공] AudioAnalyzer 실행됨");

            InvokeRepeating(nameof(ClearLog), 10f, 3f);
        }
        catch (System.Exception ex)
        {
            AppendLog("[실패] 실행 중 에러: " + ex.Message);
        }
    }

    private string GetExecutablePath()
    {
        string folder = selectedFps == FpsOption.Fps5 ? "5fps" : "10fps";
        string exeName = selectedFps == FpsOption.Fps5
            ? "PurrfectCatAudioCapture_5FPS.exe"
            : "PurrfectCatAudioCapture_10FPS.exe";

#if UNITY_EDITOR
        return Path.Combine(Directory.GetCurrentDirectory(), $"AudioAnalyzer/{folder}/{exeName}");
#else
        return Path.Combine(Application.dataPath, $"../AudioAnalyzer/{folder}/{exeName}");
#endif
    }

    private void AppendLog(string msg)
    {
        if (logText != null)
            logText.text += $"{msg}\n";
    }

    private void ClearLog()
    {
        if (logText != null)
            logText.text = string.Empty;
    }

    private void OnApplicationQuit()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            process.Dispose();
            process = null;
        }
    }

    public void QuitGame()
    {
        AppendLog("[종료 요청] 게임 종료 중...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
