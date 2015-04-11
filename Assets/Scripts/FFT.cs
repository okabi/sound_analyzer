/********************************************************************
 *  FFT.cs
 *    マイク入力のFFT結果を出力する。
 ********************************************************************/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FFT : MonoBehaviour {
    public GameObject attachedGameObject;  // このスクリプトをアタッチするオブジェクト
    public Camera mainCamera;  // メインカメラ
    private AudioSource mic;  // 解析対象のAudioSource
    public LineRenderer lineRenderer;  // 対数振幅スペクトル
    private const float RateHertz = 4.0f;  // Hertz倍率
    private const float RatePower = 5.0f;  // Power倍率
    private const float PositionHertz = 0.1f;  // スペクトル描画開始位置(X座標)
    private const float PositionPower = 0.1f;  // スペクトル描画開始位置(Y座標)


    void Start () {
        // マイクの設定
        mic = attachedGameObject.AddComponent<AudioSource>();
        const string micName = "UAB-80";
        mic.clip = Microphone.Start(micName, true, 999, 44100);
        if (mic.clip == null)   Debug.LogError("Microphone.Start");
        mic.loop = true;
        mic.mute = true;

        // グリッドの設定
        // 500Hzごとに線を引く(10000Hzまで)
        for (int i = 0; i < 20; i++)
        {
            float x = PositionHertz + 500.0f * i * RateHertz / (AudioSettings.outputSampleRate / 2.0f);
            GameObject generatedObject = Instantiate(GameObject.Find("grid"));
            LineRenderer gridHertz = generatedObject.GetComponent<LineRenderer>();
            gridHertz.SetPosition(0, mainCamera.ViewportToWorldPoint(new Vector3(x, PositionPower, mainCamera.nearClipPlane)));
            gridHertz.SetPosition(1, mainCamera.ViewportToWorldPoint(new Vector3(x, 1.0f, mainCamera.nearClipPlane)));
            if (i % 2 == 0)
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
        List<KeyValuePair<float, float>> spectrum = SoundAnalyzer.GetSpectrumData(mic);

        // 対数振幅スペクトルの描画
        lineRenderer.SetVertexCount(spectrum.Count);
        for (int i = 0; i < spectrum.Count; i++)
        {
            float x = PositionHertz + i * RateHertz / spectrum.Count;
            float y = PositionPower + RatePower * (float)Math.Log(spectrum[i].Value + 1.0);
            lineRenderer.SetPosition(i, mainCamera.ViewportToWorldPoint(new Vector3(x, y, mainCamera.nearClipPlane)));
        }

        Debug.Log(SoundAnalyzer.GetFundamentalFrequency(mic));
    }
}
