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
        int maxIndex = Utility.ArrayArgmax<float>(spectrum);
        float freqIndex = (float)maxIndex;
        if (maxIndex > 0 && maxIndex < spectrum.Length - 1)
        {
            float dr = spectrum[maxIndex + 1] / spectrum[maxIndex];
            float dl = spectrum[maxIndex - 1] / spectrum[maxIndex];
            freqIndex += 0.5f * (dr * dr - dl * dl); 
        }
        return freqIndex * AudioSettings.outputSampleRate / 2.0f / spectrum.Length;
    }
}