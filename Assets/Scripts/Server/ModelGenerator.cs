// 简介：用于使用文本生成模型并导入

using UnityEngine;
using UnityEngine.UI;
using server;
using System.Threading.Tasks;
using Siccity.GLTFUtility; // 引入 GLTFUtility 命名空间
using System;
using System.IO;

public class ModelGenerator : MonoBehaviour
{
    public InputField inputField ; // 输入描述文本的 UI InputField

    /// <summary>
    /// 传入一段文本，并生成导入一个模型
    /// </summary>
    /// <param name="txt">描述文本</param>
    public async void ModelGenerate(string txt)
    {

        if (txt == "")
        {
            Debug.LogError("输入为空，模型生成失败");
            return;
        }

        try
        {
            TXT2Model tx = new TXT2Model();
            
            Debug.Log("filePath");
            
            // 调用 TXT2Model 类的 ModelGeneration 方法，获取生成的 GLB 文件路径
            string filePath = await tx.ModelGeneration(txt);
            
            // string filePath = Path.Combine(Application.dataPath , "Tmp/222.glb");
            

            if (!string.IsNullOrEmpty(filePath))
            {
                Debug.Log($"GLB 文件路径: {filePath}");
                ImportGLBAsync(filePath); // 异步加载并生成模型
            }
            else
            {
                Debug.LogError("模型生成失败，未获取到有效文件路径");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"生成失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 异步导入GLB模型
    /// </summary>
    /// <param name="filepath">GLB模型的位置</param>
    void ImportGLBAsync(string filepath) {
        Importer.ImportGLBAsync(filepath, new ImportSettings(), OnFinishAsync);
    }

    /// <summary>
    /// 异步在导入GLB模型成功以后设置模型的参数
    /// </summary>
    /// <param name="result">导入成功的模型</param>
    /// <param name="animations"></param>
    void OnFinishAsync(GameObject result, AnimationClip[] animations) {
        if (result != null)
        {
            // 设置模型的位置为原点
            result.transform.position = Vector3.zero;
            result.transform.rotation = Quaternion.identity; // 重置旋转
            result.transform.localScale = Vector3.one; // 根据需要调整缩放

            Debug.Log($"模型已生成并加载到场景中");
        }
        else
        {
            Debug.LogError("模型加载失败");
        }
    }
}