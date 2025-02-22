// 简介：用于管理语音转文本、文本转语音的行为，播放生成的语音内容，对用户进行录音

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class VoiceController : MonoBehaviour
{
    /// <summary>当前物体的AudioSource组件<summary>
    [HideInInspector]
    public AudioSource audioSource;

    /// <summary>用户录入的音频</summary>
    private AudioClip recordedAudioClip;


    private void Start()
    {
        //获取AudioSource组件
        audioSource = GetComponent<AudioSource>();


        //测试用[!重要代码]，此操作为异步完成，需要一段时间后才能执行=>{}里的代码。
        SendTextToSpeechMsg("你好啊，我是讯飞语音助手！", auidoClip =>
        {
            audioSource.clip = auidoClip;
            audioSource.Play();
        });
    }

    #region 语音转文本 测试用

    private bool recording = false;

    /// <summary>
    /// 当 录音按钮 按下时调用
    /// </summary>
    public void OnButtonClick()
    {
        if (recording == false)
        {
            recording = true;
            //开始录音[!重要代码]
            StartRecord();

        }
        else
        {
            recording = false;
            //结束录音[!重要代码]
            EndRecord((text, _) =>
            {
                Debug.Log($"讯飞语音转文本成功！文本为：{text}");
            });
        }
    }

    #endregion

    #region 讯飞文本转语音

    /// <summary>
    /// 向XunFei发送消息，并等待其返回
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="chatGLMCallback">回调函数</param>
    public void SendTextToSpeechMsg(string text, Action<AudioClip> callback)
    {
        //构建Json字符串
        JObject jObject = new JObject();
        jObject["text"] = text;
        //可以更改成你想要的声音，具体内容在讯飞控制台中查看
        jObject["voice"] = "xiaoyan";
        //发送消息
        StartCoroutine(SendTextToSpeechMsgCoroutine(jObject, callback));
    }

    /// <summary>
    /// 向XunFei发送消息的协程
    /// </summary>
    /// <param name="message"></param>
    /// <param name="callback">收到消息后的回调函数</param>
    /// <returns></returns>
    private IEnumerator SendTextToSpeechMsgCoroutine(JObject message, Action<AudioClip> callback)
    {
        //请求数据
        Task<string> resultJson = XunFeiManager.Instance.TextToSpeech(message);
        //等待返回消息
        yield return new WaitUntil(() => resultJson.IsCompleted);   

        //成功接收到消息
        if (resultJson.IsCompletedSuccessfully == true)
        {
            //解析Json字符串
            JObject obj = JObject.Parse(resultJson.Result);
            //获取音频数据（base64字符串）
            string text = obj["data"].ToString();
            //解析音频数据
            float[] audioData = BytesToFloat(Convert.FromBase64String(text));

            if (audioData.Length == 0)//讯飞文本转语音失败
            {
                Debug.Log($"讯飞文本转语音失败，可能由于输入文本为空或不正确，导致语音长度为0，错误信息：{resultJson.Result}");
                //失败回调
                callback.Invoke(null);
            }
            //构建AudioClip
            AudioClip audioClip = AudioClip.Create("SynthesizedAudio", audioData.Length, 1, 16000, false);
            audioClip.SetData(audioData, 0);

            //Debug.Log("讯飞文本转语音成功");
            //回调
            callback.Invoke(audioClip);
        }
        else
        {
            Debug.Log($"讯飞文本转语音消息发送失败，错误信息：{resultJson.Result}");
            //失败回调
            callback.Invoke(null);
        }

    }



    /// <summary>
    /// byte[]数组转化为AudioClip可读取的float[]类型
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    private static float[] BytesToFloat(byte[] byteArray)
    {
        float[] sounddata = new float[byteArray.Length / 2];
        for (int i = 0; i < sounddata.Length; i++)
        {
            sounddata[i] = bytesToFloat(byteArray[i * 2], byteArray[i * 2 + 1]);
        }
        return sounddata;
    }
    private static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        //小端和大端顺序要调整
        short s;
        if (BitConverter.IsLittleEndian)
            s = (short)((secondByte << 8) | firstByte);
        else
            s = (short)((firstByte << 8) | secondByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }


    #endregion


    #region 讯飞语音转文本


    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRecord()
    {
        //开始录音频（最长40秒）
        recordedAudioClip = Microphone.Start(null, true, 40, 16000);
    }

    /// <summary>
    /// 结束录音
    /// </summary>
    /// <param name="speechToTextCallback">语音转文本成功后的回调函数</param>
    public void EndRecord(Action<string, AudioClip> speechToTextCallback)
    {
        //取消了录音
        if (speechToTextCallback == null) return;
        //录音结束
        Microphone.End(null);
        //去除掉没有声音的片段
        recordedAudioClip = TrimSilence(recordedAudioClip, 0.01f);
        //发送消息
        SendSpeechToTextMsg(recordedAudioClip, text =>
        {
            //回调
            speechToTextCallback.Invoke(text, recordedAudioClip);
        });
    }


    /// <summary>
    /// 向XunFei发送消息，并等待其返回
    /// </summary>
    /// <param name="strContent">音频数据</param>
    /// <param name="chatGLMCallback">回调函数</param>
    public void SendSpeechToTextMsg(AudioClip audioClip, Action<string> callback)
    {
        byte[] bytes = AudioClipToBytes(audioClip);
        //构建Json字符串
        JObject jObject = new JObject();
        jObject["data"] = Convert.ToBase64String(bytes);
        //发送消息
        StartCoroutine(SendSpeechToTextMsgCoroutine(jObject, callback));
    }

    /// <summary>
    /// 向XunFei发送消息的协程
    /// </summary>
    /// <param name="message"></param>
    /// <param name="callback">收到消息后的回调函数</param>
    /// <returns></returns>
    private IEnumerator SendSpeechToTextMsgCoroutine(JObject message, Action<string> callback)
    {
        //请求数据
        Task<string> resultJson = XunFeiManager.Instance.SpeechToText(message);
        //等待返回消息
        yield return new WaitUntil(() => resultJson.IsCompleted);

        //成功接收到消息
        if (resultJson.IsCompletedSuccessfully == true)
        {
            //解析Json字符串
            JObject obj = JObject.Parse(resultJson.Result);
            //获取相似度
            string text = obj["text"].ToString();
            //Debug.Log("讯飞语音转文本：" + text);
            //回调
            callback.Invoke(text);
        }
        else
        {
            Debug.Log("讯飞语音转文本消息发送失败");
            //失败回调
            callback.Invoke(string.Empty);
        }

    }


    /// <summary>
    /// 将AudioClip转换成byte[]数据
    /// </summary>
    /// <param name="audioClip">Unity中的音频数据</param>
    /// <returns>byte[]数据</returns>
    private static byte[] AudioClipToBytes(AudioClip audioClip)
    {
        float[] data = new float[audioClip.samples];
        audioClip.GetData(data, 0);
        int rescaleFactor = 32767; //to convert float to Int16
        byte[] outData = new byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            short temshort = (short)(data[i] * rescaleFactor);
            byte[] temdata = BitConverter.GetBytes(temshort);
            outData[i * 2] = temdata[0];
            outData[i * 2 + 1] = temdata[1];
        }
        return outData;
    }

    /// <summary>
    /// 剔除沉默音域
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    private static AudioClip TrimSilence(AudioClip clip, float min)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    private static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D = false)
    {
        int origSamples = samples.Count;

        int i;

        for (i = 0; i < samples.Count; i++)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        i -= (int)(hz * .1f);
        i = Mathf.Max(i, 0);

        // Remove start silence
        samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        // Add some tail onto it
        i += (int)(hz * .1f);
        i = Mathf.Min(i, samples.Count - 1);
        samples.RemoveRange(i, samples.Count - i);


        if (samples.Count == 0)
        {
            Debug.Log("剔除后的AudioClip长度为0");
            return null;
        }

        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D);
        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    #endregion
}
