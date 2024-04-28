using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class LB
{
    private static Vector2 referenceResolution = new Vector2(1920, 1080);

    public static void ScaleCanvas(this CanvasScaler canvasScaler, bool matchWidth = true)
    {
        float aspectRatio = (float)Screen.height / Screen.width;
        float referenceAspect = referenceResolution.y / referenceResolution.x;

        if (matchWidth)
        {
            canvasScaler.matchWidthOrHeight = aspectRatio > referenceAspect ? 0 : 1;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 1 - (aspectRatio > referenceAspect ? 0 : 1);
        }
    }

    public static bool IsTablet(string deviceModel, bool tablet)
    {
        // Các từ khóa phổ biến để nhận biết máy tính bảng trong tên thiết bị
        string[] tabletKeywords = { "tablet", "ipad", "kindle", "galaxy tab", "nexus", "surface", "playbook" };

        // Chuyển đổi tên thiết bị thành chữ thường để so sánh không phân biệt hoa thường
        deviceModel = deviceModel.ToLower();
        // Kiểm tra xem tên thiết bị có chứa từ khóa của máy tính bảng không
        foreach (string keyword in tabletKeywords)
        {

#if UNITY_EDITOR
            deviceModel = tablet == true ? "tablet" : "";
#endif
            if (deviceModel.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }
}
