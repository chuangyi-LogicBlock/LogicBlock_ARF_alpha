using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ImageRecognitionHandler : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject Emittercube;
    public GameObject Split3cube;
    public GameObject Split6cube;
    public GameObject Colorcube;
    public GameObject Scalecube;
    public GameObject Musiccube;
    public GameObject EmitterPersoncube;
    public GameObject PersonChangercube;

    // 使用 trackableId 作为键来存储生成的对象
    private Dictionary<TrackableId, GameObject> instantiatedObjects = new Dictionary<TrackableId, GameObject>();

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 处理新增的图像
        foreach (var trackedImage in eventArgs.added)
        {
            GameObject prefab = GetPrefabForImage(trackedImage.referenceImage.name);
            if (prefab != null)
            {
                // 实例化对象并存储到字典中，使用 trackableId 作为键
                GameObject instantiatedObject = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
                instantiatedObjects[trackedImage.trackableId] = instantiatedObject;
            }
        }

        // 处理更新的图像 
        foreach (var trackedImage in eventArgs.updated) 
        {
            if (instantiatedObjects.TryGetValue(trackedImage.trackableId, out GameObject instantiatedObject))
            {
                // 更新对象的位置和旋转
                instantiatedObject.transform.position = trackedImage.transform.position;
                instantiatedObject.transform.rotation = trackedImage.transform.rotation;

                // 如果图像不再被跟踪，可以隐藏或销毁对象
                if (trackedImage.trackingState == TrackingState.None)
                {
                    instantiatedObject.SetActive(false);
                }
                else
                {
                    instantiatedObject.SetActive(true);
                }
            }
        }

        // 处理移除的图像
        foreach (var trackedImage in eventArgs.removed)
        {
            if (instantiatedObjects.TryGetValue(trackedImage.trackableId, out GameObject instantiatedObject))
            {
                // 销毁对象并从字典中移除
                Destroy(instantiatedObject);
                instantiatedObjects.Remove(trackedImage.trackableId);
            }
        }
    }

    private GameObject GetPrefabForImage(string imageName)
    {
        switch (imageName)
        {
            case "J1":
                return EmitterPersoncube;
            case "Q1":
                return PersonChangercube;
            case "Q2":
                return PersonChangercube;
            case "Q3":
                return PersonChangercube;
            case "Q4":
                return PersonChangercube;
            case "K1":
                return Split3cube;
            case "K2":
                return Split3cube;
            default:
                return null;
        }
    }
}