/********************************************************************
 *  FrequencyAndFFT.cs
 *    マイク入力の基本周波数推定結果とFFT結果を出力する。
 ********************************************************************/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FrequencyAndFFT : MonoBehaviour {
    public GameObject attachedGameObject;  // このスクリプトをアタッチするオブジェクト
    public GameObject FFTObject;  // FFT結果出力用のオブジェクト
    public Camera mainCamera;  // メインカメラ
    private AudioSource mic;  // 解析対象のAudioSource
    public LineRenderer lineRenderer;  // 基本周波数推定
    private const float RateTime = 0.8f;  // Time倍率
    private const float RateHertz = 30.0f;  // Hertz倍率
    private const float RateFFTHertz = 4.0f;  // FFTのHertz倍率
    private const float RateFFTPower = 5.0f;  // FFTのPower倍率
    private const float PositionTime = 0.1f;  // 推定線描画開始位置(X座標)
    private const float PositionHertz = 0.1f;  // 推定線描画開始位置(Y座標)
    private const int NumTimes = 1000;  // 0.01sごとに1つの座標を置く。10秒まで表示
    private int beforeTime;  // 前フレームのマイク秒数×100


    void Start () {
        // マイクの設定
        mic = attachedGameObject.AddComponent<AudioSource>();
        const string micName = "UAB-80";
        mic.clip = Microphone.Start(micName, true, 999, 44100);
        if (mic.clip == null)   Debug.LogError("Microphone.Start");
        mic.loop = true;
        mic.mute = true;
        beforeTime = 0;

        // グリッドの設定
        // FFT結果について、500Hzごとに線を引く(10000Hzまで)
        for (int i = 0; i < 20; i++)
        {
            float x = PositionTime + 500.0f * i * RateFFTHertz / (AudioSettings.outputSampleRate / 2.0f);
            GameObject generatedObject = Instantiate(GameObject.Find("grid"));
            LineRenderer gridHertz = generatedObject.GetComponent<LineRenderer>();
            gridHertz.SetPosition(0, mainCamera.ViewportToWorldPoint(new Vector3(x, PositionHertz, mainCamera.nearClipPlane)));
            gridHertz.SetPosition(1, mainCamera.ViewportToWorldPoint(new Vector3(x, 1.0f, mainCamera.nearClipPlane)));
            if (i % 2 == 0)
            {
                Color color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
                gridHertz.SetColors(color, color);
            }
        }

        // 100Hzごとに線を引く(2000Hzまで)
        for (int i = 0; i < 20; i++)
        {
            float y = PositionHertz + 100.0f * i * RateHertz / (AudioSettings.outputSampleRate / 2.0f);
            GameObject generatedObject = Instantiate(GameObject.Find("grid"));
            LineRenderer gridHertz = generatedObject.GetComponent<LineRenderer>();
            gridHertz.SetPosition(0, mainCamera.ViewportToWorldPoint(new Vector3(PositionTime, y, mainCamera.nearClipPlane)));
            gridHertz.SetPosition(1, mainCamera.ViewportToWorldPoint(new Vector3(1.0f, y, mainCamera.nearClipPlane)));
            if (i % 5 == 0)
            {
                Color color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
                gridHertz.SetColors(color, color);
            }
        }
        GameObject.Find("grid").SetActive(false);

        // 録音の準備が出来るまで待つ
        while (!(Microphone.GetPosition(micName) > 0)) { }
        mic.Play();
    }


	void Update () {
        // 対数振幅スペクトルの描画
        List<KeyValuePair<float, float>> spectrum = SoundAnalyzer.GetSpectrumData(mic);
        LineRenderer FFTRenderer = FFTObject.GetComponent<LineRenderer>();
        FFTRenderer.SetVertexCount(spectrum.Count);
        for (int i = 0; i < spectrum.Count; i++)
        {
            float x = PositionTime + i * RateFFTHertz / spectrum.Count;
            float y = PositionHertz + RateFFTPower * (float)Math.Log(spectrum[i].Value + 1.0);
            FFTRenderer.SetPosition(i, mainCamera.ViewportToWorldPoint(new Vector3(x, y, mainCamera.nearClipPlane)));
        }

        // 基本周波数の取得
        float freq = SoundAnalyzer.GetFundamentalFrequency(mic);

        // マイク秒数の取得
        int now = (int)(mic.time * 100.0f);

        // 基本周波数の描画
        int vertexCount = now % NumTimes;
        int beforeVertexCount = beforeTime % NumTimes;
        if (vertexCount < beforeVertexCount)
        {
            // 表示がループした時は、頂点0から描画を開始
            beforeVertexCount = 0;
        }
        lineRenderer.SetVertexCount(vertexCount);
        for (int i = beforeVertexCount; i < vertexCount; i++)
        {
            float x = PositionTime + i * RateTime / NumTimes;
            float y = PositionHertz + freq * RateHertz / (AudioSettings.outputSampleRate / 2.0f);
            lineRenderer.SetPosition(i, mainCamera.ViewportToWorldPoint(new Vector3(x, y, mainCamera.nearClipPlane)));
        }

        beforeTime = now;
    }
}
