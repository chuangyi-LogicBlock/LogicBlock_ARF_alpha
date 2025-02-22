// 简介：包含文本转模型用于与后端发送请求和异步下载的代码

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using UnityEngine;

namespace server
{
    class TXT2Model
    {
        private static readonly HttpClient client;

        static TXT2Model()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true; // 忽略 HTTPS 证书验证
            client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(5); // 将超时时间设置为 5 分钟
        }

        /// <summary>
        /// 传入一段文本，生成相应的模型，并返回生成的 GLB 文件路径
        /// </summary>
        /// <param name="text">描述文本</param>
        /// <returns>生成的 GLB 文件路径</returns>
        public async Task<string> ModelGeneration(string text)
        {
            UnityEngine.Debug.Log("向服务端发送了请求");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string url = "https://localhost:7281/api/convet/TXTtoAct"; // 服务器地址
            string content = "\"" + text + "\""; // 要发送的字符串，注意这里是 JSON 格式的字符串
            UnityEngine.Debug.Log(content);
            
            string downloadPath = Path.Combine(Application.persistentDataPath ,"Tmp/") ; // 文件下载路径

            var contentData = new StringContent(content, Encoding.UTF8, "application/json");

            try
            {
                // 发送 POST 请求
                UnityEngine.Debug.Log("向服务端发送了请求{ " + url + " , " + contentData + "}");
                var response = await client.PostAsync(url, contentData);

                if (response.IsSuccessStatusCode)
                {
                    // 读取并输出响应内容
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TXT2ModelEntity>(responseContent);

                    // 输出反序列化后的结果
                    UnityEngine.Debug.Log("接收到成功响应");
                    UnityEngine.Debug.Log($"文本= {result.Message}, 下载地址= {result.Loading}");
                    stopwatch.Stop();
                    UnityEngine.Debug.Log("程序运行时间: " + stopwatch.Elapsed.TotalSeconds + " 秒");

                    // 下载并保存 GLB 文件
                    string modelUrl = result.Loading;
                    string filePath = Path.Combine(downloadPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".glb");
                    UnityEngine.Debug.Log("开始下载");
                    await DownloadFileAsync(modelUrl, filePath); // 下载文件并保存到指定路径
                    UnityEngine.Debug.Log($"GLB 文件已保存至: {filePath}");

                    // 返回生成的 GLB 文件路径
                    return filePath;
                }
                else
                {
                    UnityEngine.Debug.Log("Request failed with status code: " + response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                // 错误处理
                UnityEngine.Debug.Log("Error: " + ex.Message);
                UnityEngine.Debug.Log("Stack Trace: " + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 用于从对应 URL 中异步下载文件
        /// </summary>
        /// <param name="fileUrl">API 的 URL</param>
        /// <param name="filePath">下载文件目录</param>
        private async Task DownloadFileAsync(string fileUrl, string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var response = await client.GetAsync(fileUrl, CancellationToken.None); // 异步发送 GET 请求下载文件
                response.EnsureSuccessStatusCode(); // 确保请求成功

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(fileStream); // 将响应内容流复制到文件流中
                    UnityEngine.Debug.Log($"文件下载完成: {filePath}");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("文件下载失败: " + ex.Message);
                UnityEngine.Debug.Log("Stack Trace: " + ex.StackTrace);
                throw; // 重新抛出异常
            }
        }
    }

    class TXT2ModelEntity
    {
        public string Loading { get; set; }
        public string Message { get; set; }
    }
}