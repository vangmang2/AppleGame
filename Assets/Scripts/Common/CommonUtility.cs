using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class CommonUtility 
{
    public static void DrawRectangle(Vector2 top_right, Vector2 bottom_left)
    {
        var offset = (top_right + bottom_left) * 0.5f;

        var displacement = top_right - bottom_left;
        var xProjection = Vector2.Dot(displacement, Vector2.right);
        var yProjection = Vector2.Dot(displacement, Vector2.up);

        var top_left = new Vector2(-xProjection, yProjection) * 0.5f + offset;
        var bottom_right = new Vector2(xProjection, -yProjection) * 0.5f + offset;

        Gizmos.DrawLine(top_right, top_left);
        Gizmos.DrawLine(top_left, bottom_left);
        Gizmos.DrawLine(bottom_left, bottom_right);
        Gizmos.DrawLine(bottom_right, top_right);
    }

    public static float fixedDeltaTime => Time.fixedDeltaTime * Time.timeScale;

    public readonly static YieldInstruction fixedUpdate = new WaitForFixedUpdate();

    static Dictionary<float, YieldInstruction> yieldSecDic = new Dictionary<float, YieldInstruction>();

    public static YieldInstruction GetYieldSec(float sec)
    {
        if (!yieldSecDic.ContainsKey(sec))
            yieldSecDic.Add(sec, new WaitForSeconds(sec));
        return yieldSecDic[sec];
    }

    public static void ForEach<T>(this T[] array, Action<T> callback)
    {
        foreach(T element in array)
        {
            callback(element);
        }
    }

    public static bool All<T>(this T[,] array, Func<T, bool> callback)
    {
        foreach (T element in array)
        {
            if (!callback(element))
                return false;
        }
        return true;
    }

    public static int Count<T>(this T[,] array, Func<T, bool> callback)
    {
        int count = 0;
        foreach(T element in array)
        {
            if (callback(element))
                count++;
        }
        return count;
    }

    public static List<T> ToList<T>(this T[,] array)
    {
        var list = new List<T>();
        foreach(T element in array)
            list.Add(element);
        return list;
    }

    public static DateTime ParseToDateTime(this string target)
        => DateTime.Parse(target);
    public static TimeSpan ParseToTimeSpan(this string target)
        => TimeSpan.Parse(target);

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random random = new System.Random();
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector2 MousePosToWorldPos(this Camera camera)
    {
        var screenPos = camera.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f);
        screenPos.x *= Screen.width;
        screenPos.y *= Screen.height;
        return screenPos;
    }
    public static void SetSprite(this Image image, Sprite sprite)
        => image.sprite = sprite;
    public static void Set(this Text text, string target)
        => text.text = target;
    public static void Set(this Text text, int target)
        => text.text = target.ToString();
    public static void SetDatasToDictionary<T>(this List<T> list, Dictionary<string, T> targetDic) 
        where T : Object
        => list.ForEach(target => targetDic.Set(target));

    /// <summary> value의 name을 키로 등록 해준다. </summary>
    public static void Set<T>(this Dictionary<string, T> dic, T value) 
        where T : Object
        => dic.Add(value.name, value);

    public static bool IsNullOrEmpty(this string target)
        => string.IsNullOrEmpty(target);

    public static T LoadResource<T>(this string path) 
        where T : Object
        => Resources.Load<T>($"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}");

    public static Vector2 Copy(this Vector2 v, float? x = null, float? y = null) 
        => new Vector2(x ?? v.x, y ?? v.y);

    public static Vector3 Copy(this Vector3 v, float? x = null, float? y = null, float? z = null)
        => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);

    public static void SetRateOverTime(this ParticleSystem.EmissionModule emission, float rate)
        => emission.rateOverTime = rate;

    public static void SetMaxParticleCount(this ParticleSystem.MainModule mainModule, int count)
        => mainModule.maxParticles = count;

    public static bool PermissionRange(this Vector3 v, Vector3 target, float range)
        => Mathf.Abs(target.x - v.x) <= range &&
           Mathf.Abs(target.y - v.y) <= range &&
           Mathf.Abs(target.z - v.z) <= range;

    public static Color CopyColor(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        => new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
}
