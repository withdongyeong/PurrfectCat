using UnityEngine;
using System.Diagnostics; // System.Diagnostics.Debug

public class ExternalAppLauncher : MonoBehaviour
{
    private Process externalProcess;

    // frameRate 변수를 Start() 밖에서 선언합니다.
    public string frameRate = "60";

    void Start()
    {
        // exePath 변수는 Start() 메서드 안에서 선언
        string exePath = Application.dataPath + "/../AudioAnalyzer/" + frameRate + "fps/" + 
                         "PurrfectCatAudioCapture_" + frameRate + "FPS"+".exe";
        UnityEngine.Debug.Log("🛣 실행 경로: " + exePath);  // UnityEngine.Debug를 사용

        try
        {
            externalProcess = new Process();
            externalProcess.StartInfo.FileName = exePath;
            externalProcess.StartInfo.CreateNoWindow = true;
            externalProcess.StartInfo.UseShellExecute = false;
            externalProcess.Start();
            UnityEngine.Debug.Log("✅ ExternalApp 실행됨!");  // UnityEngine.Debug를 사용
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("❌ 실행 실패: " + e.Message);  // UnityEngine.Debug를 사용
        }
    }

    void OnApplicationQuit()
    {
        if (externalProcess != null && !externalProcess.HasExited)
        {
            externalProcess.Kill();
            UnityEngine.Debug.Log("🛑 외부 프로세스 종료됨");  // UnityEngine.Debug를 사용
        }
    }
}