/********************************************************************
 *  SoundAnalyzer.cs
 *    音声解析のためのクラス。
 ********************************************************************/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyLibrary;

public class SoundAnalyzer : MonoBehaviour {
    public const int NumSamples = 2048;  // 2の累乗、64~8192。数値が大きいほど精度高
    public const FFTWindow Window = FFTWindow.BlackmanHarris;  // フーリエ変換で用いる窓関数
    
    /**
     * <summary>AudioSourceの高速フーリエ変換結果を返す。</summary>
     * <param name="audio">解析対象のAudioSource</param>
     * <returns>周波数 -> パワー のPairのList</returns>
     */
    public static List<KeyValuePair<float, float>> GetSpectrumData(AudioSource audio)
    {
        float[] spectrum = new float[NumSamples];
        audio.GetSpectrumData(spectrum, 0, Window);
        List<KeyValuePair<float, float>> result = new List<KeyValuePair<float,float>>();
        for (int i = 0; i < spectrum.Length; i++)
        {
            float frequency = ((float)AudioSettings.outputSampleRate * i) / (2.0f * spectrum.Length);
            result.Add(new KeyValuePair<float, float>(frequency, spectrum[i])); 
        }
        return result;
    }


    /**
     * <summary>AudioSourceの再生箇所の基本周波数を返す。</summary>
     * <param name="audio">解析対象のAudioSource</param>
     * <returns>基本周波数</returns>
     */
    public static float GetFundamentalFrequency(AudioSource audio)
    {
        float[] spectrum = new float[NumSamples];
        audio.GetSpectrumData(spectrum, 0, Window);
        
        // 最大パワーを見つける
        int maxIndex = Utility.ArrayArgmax<float>(spectrum);

        // 基本周波数の候補とするパワー閾値を算出
        float threshold = (float)System.Math.Log(spectrum[maxIndex] + 1.0f) / 10.0f;

        // 低い周波数から、閾値を超えるパワーを持つピーク点を求める。
        // それを基本周波数とする(いろいろ問題ありそうなアルゴリズムだ)。
        int fundamentalIndex = maxIndex;
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            if (spectrum[i - 1] <= spectrum[i] && spectrum[i] >= spectrum[i + 1])
            {
                float power = (float)System.Math.Log(spectrum[i] + 1.0f);
                if (power > threshold)
                {
                    fundamentalIndex = i;
                    break;
                }
            }
        }
        
        float freqIndex = (float)fundamentalIndex;
        if (fundamentalIndex > 0 && fundamentalIndex < spectrum.Length - 1)
        {
            float dr = spectrum[fundamentalIndex + 1] / spectrum[fundamentalIndex];
            float dl = spectrum[fundamentalIndex - 1] / spectrum[fundamentalIndex];
            freqIndex += 0.5f * (dr * dr - dl * dl); 
        }
        return freqIndex * AudioSettings.outputSampleRate / 2.0f / spectrum.Length;
    }
}