using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LB
{
    public static float ScaleCanvas()
    {
        Vector2 scale = new Vector2(Screen.width * 1.0f / 1080, Screen.height * 1.0f / 1920);
        return (1 - scale.magnitude);
    }
}
