// 简介：用于管理与讯飞相关的语音转文本、文本转语音的API的使用

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class XunFeiManager : MonoBehaviour
{
    /// <summary>单例</summary>
    public static XunFeiManager Instance;

    /// <summary>固定不动</summary>
    private const string STTURL = "wss://iat-api.xfyun.cn/v2/iat";
    /// <summary>固定不动</summary>
    private const string TTSURL = "wss://tts-api.xfyun.cn/v2/tts";
    /// <summary>自己的APPID!</summary>
    private const string APPID = "460b57de";
    /// <summary>自己的APISecret!</summary>
    private const string APISecret = "YWI2M2MxNjZmNTgxODIyZDZmOTA2NzNk";
    /// <summary>自己的APIKey!</summary>
    private const string APIKey = "74d26483796e2f8ba036c6ae0b99c331";


    /// <summary>WebSocket</summary>
    private ClientWebSocket webSocket;


    private void Awake()
    {
        //初始化单例
        Instance = this;
    }


    public async Task<string> SpeechToText(JObject request)
    {
        //获取请求数据
        byte[] bytes = Convert.FromBase64String(request["data"].ToString());
        //建立连接
        await Connect(STTURL);
        //发送消息
        await STTSendMessage(bytes);
        //接收消息
        string text = await STTReceiveMessage();
        //关闭接连
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

        //构建返回Json数据
        JObject response = new JObject();
        response["text"] = text;
        //响应Post请求
        return response.ToString();
    }


    public async Task<string> TextToSpeech(JObject request)
    {
        //获取请求数据
        string text = request["text"].ToString();
        string voice = request["voice"].ToString();
        //建立连接
        await Connect(TTSURL);
        //发送消息
        await TTSSendMessage(text, voice);
        //接收消息
        string base64String = await TTSReceiveMessage();
        //关闭接连
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

        //构建返回Json数据
        JObject response = new JObject();
        response["data"] = base64String;
        //响应Post请求
        return response.ToString();

    }


    /// <summary>
    /// 连接讯飞API
    /// </summary>
    /// <returns></returns>
    private async Task Connect(string url)
    {
        //新建ClientWebSocket
        webSocket = new ClientWebSocket();
        //使用WebSocket连接讯飞的服务
        await webSocket.ConnectAsync(new Uri(GetUrl(url)), CancellationToken.None);
        //await Console.Out.WriteLineAsync("讯飞WebSocket连接成功");
    }


    /// <summary>
    /// 讯飞语音转文本，发送消息
    /// </summary>
    private async Task STTSendMessage(byte[] bytes)
    {
        //状态值 0代表第1帧，1代表中间帧，2代表结尾帧
        int status;
        int remainLength = bytes.Length;
        //分段发送消息
        while (remainLength > 0)
        {
            byte[] currBytes;
            if (remainLength > 1280)
            {
                status = remainLength == bytes.Length ? 0 : 1;
                currBytes = new byte[1280];
                Array.Copy(bytes, bytes.Length - remainLength, currBytes, 0, 1280);
                remainLength -= 1280;
            }
            else
            {
                status = 2;
                currBytes = new byte[remainLength];
                Array.Copy(bytes, bytes.Length - remainLength, currBytes, 0, remainLength);
                remainLength = 0;
            }

            JObject jsonData = STTCreateJsonData(status, currBytes);
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonData.ToString());
            await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
            //等待30ms
            await Task.Delay(20);
            //Console.WriteLine("发送消息："+jsonData.ToString());
        }
        //Console.WriteLine("数据发送完成");
    }

    /// <summary>
    /// 讯飞语音转文本，生成需要发送的Json数据
    /// </summary>
    /// <param name="status"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private JObject STTCreateJsonData(int status, byte[] bytes)
    {
        JObject requestObj = new JObject();
        JObject commonJson = new JObject();
        commonJson["app_id"] = APPID;
        requestObj["common"] = commonJson;
        JObject bussinessJson = new JObject();
        bussinessJson["language"] = "zh_cn";
        bussinessJson["domain"] = "iat";
        bussinessJson["accent"] = "mandarin";
        bussinessJson["dwa"] = "wpgs";
        requestObj["business"] = bussinessJson;
        JObject dataJson = new JObject();
        dataJson["status"] = status;
        dataJson["format"] = "audio/L16;rate=16000";
        dataJson["encoding"] = "raw";
        dataJson["audio"] = Convert.ToBase64String(bytes);
        requestObj["data"] = dataJson;
        return requestObj;
    }

    /// <summary>
    /// 讯飞语音转文本，接收消息
    /// </summary>
    /// <returns></returns>
    private async Task<string> STTReceiveMessage()
    {
        //webSocket.
        //状态值
        int status = 0;
        string finalText = string.Empty;
        while (status != 2)
        {
            byte[] buffer = new byte[8 * 1024];
            WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
            //await Console.Out.WriteLineAsync("receivedMessage：" + receivedMessage);
            finalText += STTParseMessage(receivedMessage, out status);
        }
        Debug.Log("讯飞语音转文本：" + finalText);
        return finalText;

    }
    /// <summary>
    /// 讯飞语音转文本，解析收到的Json消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    private string STTParseMessage(string message, out int status)
    {
        JObject jObject = JObject.Parse(message);

        int code = (int)jObject["code"];
        if (code != 0)//失败
        {
            string errMsg = jObject["message"].ToString();
            Debug.LogError("讯飞语音转文本，解析Json消息失败，错误信息：" + errMsg);
        }
        else//成功
        {
            string result = string.Empty;
            foreach (JObject i in jObject["data"]["result"]["ws"])
            {
                foreach (JObject w in i["cw"])
                {
                    result += w["w"].ToString();
                }
            }
            //状态值
            //识别结果是否结束标识：
            //0：识别的第一块结果
            //1：识别中间结果
            //2：识别最后一块结果
            status = (int)jObject["data"]["status"];
            return result;
        }
        status = -1;
        return string.Empty;
    }


    /// <summary>
    /// 讯飞文本转语音，发送消息
    /// </summary>
    private async Task TTSSendMessage(string text, string voice)
    {
        //构建请求需要的Json字符串
        JObject jsonData = TTSCreateJsonData(text, voice);
        byte[] messageBytes = Encoding.UTF8.GetBytes(jsonData.ToString());
        //发送消息
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }


    /// <summary>
    /// 讯飞文本转语音，生成需要发送的Json数据
    /// </summary>
    /// <param name="status"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private JObject TTSCreateJsonData(string text, string voice)
    {
        JObject requestObj = new JObject();
        JObject commonJson = new JObject();
        commonJson["app_id"] = APPID;
        requestObj["common"] = commonJson;
        JObject bussinessJson = new JObject();
        bussinessJson["aue"] = "raw";
        bussinessJson["vcn"] = voice;
        bussinessJson["speed"] = 50;
        bussinessJson["volume"] = 50;
        bussinessJson["pitch"] = 50;
        bussinessJson["tte"] = "UTF8";
        requestObj["business"] = bussinessJson;
        JObject dataJson = new JObject();
        dataJson["status"] = 2;
        dataJson["text"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        requestObj["data"] = dataJson;
        return requestObj;
    }


    /// <summary>
    /// 讯飞文本转语音，接收消息
    /// </summary>
    /// <returns>语音合成后的base64字符串</returns>
    private async Task<string> TTSReceiveMessage()
    {
        //webSocket.
        //状态值
        int status = 0;
        List<byte> bytes = new List<byte>();
        while (status != 2)
        {
            bool receivedCompleted = false;
            string receivedMessage = string.Empty;
            while (!receivedCompleted)
            {
                byte[] buffer = new byte[8 * 1024];
                WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                receivedMessage += Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
                receivedCompleted = webSocketReceiveResult.Count != 8 * 1024;
            }
            //await Console.Out.WriteLineAsync("receivedMessage：" + receivedMessage);
            bytes.AddRange(Convert.FromBase64String(TTSParseMessage(receivedMessage, out status)).ToList());
            //finalAudioBase64String += TTSParseMessage(receivedMessage, out status).TrimEnd('=');
        }
        string finalAudioBase64String = Convert.ToBase64String(bytes.ToArray());
        //await Console.Out.WriteLineAsync("讯飞语音转文本：" + finalAudioBase64String);
        return finalAudioBase64String;


    }

    /// <summary>
    /// 讯飞文本转语音，解析收到的Json消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    private string TTSParseMessage(string message, out int status)
    {
        JObject jObject = JObject.Parse(message);
        if (jObject["message"].ToString() == "success")
        {
            if (jObject["data"] != null)
            {
                if (jObject["data"]["audio"] != null)
                {
                    if ((int)jObject["data"]["status"] == 2)
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 1;
                    }
                    return jObject["data"]["audio"].ToString();

                }
            }
            Debug.LogError("ERROR:TTSParseMessage失败，data为空");
            status = 0;
            return string.Empty;
        }
        else
        {
            Debug.LogError("ERROR:TTSParseMessage失败，错误消息：" + jObject["message"].ToString());
            status = 0;
            return string.Empty;
        }

    }



    #region 生成URL
    private string GetUrl(string url)
    {
        Uri uri = new Uri(url);
        //官方文档要求时间必须是UTC+0或GMT时区，RFC1123格式(Thu, 01 Aug 2019 01:53:21 GMT)。
        string date = DateTime.Now.ToString("r");
        //组装生成鉴权
        string authorization = ComposeAuthUrl(uri, date);
        //生成最终鉴权
        string uriStr = $"{uri}?authorization={authorization}&date={date}&host={uri.Host}";
        //返回生成后的Url
        return uriStr;
    }
    /// <summary>
    /// 组装生成鉴权
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="date"></param>
    /// <returns>最终编码后的鉴权</returns>
    private string ComposeAuthUrl(Uri uri, string date)
    {
        string signature; //最终编码后的签名
        string authorization_origin; //原始鉴权
                                     //原始签名
        string signature_origin = string.Format("host: " + uri.Host + "\ndate: " + date + "\nGET " + uri.AbsolutePath + " HTTP/1.1");
        //使用hmac-sha256算法加密后的signature
        string signature_sha = HmacSHA256(signature_origin, APISecret); //使用hmac - sha256算法结合apiSecret对signature_origin签名
        signature = signature_sha;
        string auth = "api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"";
        authorization_origin = string.Format(auth, APIKey, "hmac-sha256", "host date request-line", signature); //参数介绍：APIKey,加密算法名，headers是参与签名的参数(该参数名是固定的"host date request-line")，生成的签名
        return ToBase64String(authorization_origin);
    }

    /// <summary>
    /// 加密算法HmacSHA256  
    /// </summary>
    /// <param name="secret"></param>
    /// <param name="signKey"></param>
    /// <returns></returns>
    private static string HmacSHA256(string secret, string signKey)
    {
        string signRet = string.Empty;
        using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
        {
            byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
            signRet = Convert.ToBase64String(hash);
        }
        return signRet;
    }

    /// <summary>
    /// UTF字符串转成Base64字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string ToBase64String(string value)
    {
        if (value == null || value == "")
        {
            return "";
        }
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    #endregion
}

