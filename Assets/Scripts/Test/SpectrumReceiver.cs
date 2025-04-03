using System;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class SpectrumReceiver : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];  // 메시지 수신을 위한 버퍼
    private string receivedData = "";

    public GameObject[] spectrumBars;  // 스펙트럼바를 Unity에서 나타낼 GameObject 배열

    void Start()
    {
        try
        {
            // TCP 연결 설정
            client = new TcpClient("127.0.0.1", 5005);  // C# 콘솔 애플리케이션과 같은 포트
            stream = client.GetStream();
            Debug.Log("✅ Unity TCP 연결됨!");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ 연결 실패: " + e.Message);
        }
    }

    void Update()
    {
        if (stream != null && stream.DataAvailable)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                receivedData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // 수신된 데이터 로그 출력
                Debug.Log($"수신된 데이터: {receivedData}");

                // 쉼표로 구분된 데이터를 받아서 배열로 변환
                string[] values = receivedData.Split(',');

                // spectrumBars가 null이 아니고, 값이 맞다면
                if (values.Length == spectrumBars.Length && values.Length > 0)
                {
                    // 받은 값들을 float 배열로 변환
                    float[] floatValues = new float[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        floatValues[i] = float.Parse(values[i]);  // 0~1 범위로 클램프 하지 않고 그대로 받음
                    }

                    // 정규화: 최대값을 10으로 정규화
                    Normalize(ref floatValues);

                    // 각 대역에 대해 스펙트럼바 크기 변경
                    for (int i = 0; i < values.Length; i++)
                    {
                        // 값이 0이면 스펙트럼바 크기를 1로 설정
                        if (floatValues[i] == 0)
                        {
                            floatValues[i] = 1f;
                        }

                        // 정규화된 값으로 스펙트럼바 크기 조정
                        spectrumBars[i].transform.localScale = new Vector3(0.2f, floatValues[i], 0.1f);  // 직사각형 비율 유지하면서 Y축만 변경
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ 데이터 처리 실패: " + e.Message);
            }
        }
    }

    // 정규화 함수: 수신된 값을 0과 10 사이로 정규화
    void Normalize(ref float[] values)
    {
        float max = values.Max();  // 배열에서 가장 큰 값 찾기
        if (max > 0)  // 0으로 나누는 방지
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = (values[i] / max) * 10f;  // 최대값으로 나누고, 10을 곱해 0~10 범위로 정규화
            }
        }
    }

    void OnApplicationQuit()
    {
        // Stream과 Client 종료
        stream?.Close();
        client?.Close();
    }
}
