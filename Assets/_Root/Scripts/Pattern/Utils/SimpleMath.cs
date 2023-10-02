using UnityEngine;

public static class SimpleMath
{
    public static bool InRange(Vector3 p, Vector3 c, float r, out float sqrDst, bool compareY = false)
    {
        if (!compareY) p.y = c.y;
        sqrDst = SqrDist(p, c);
        Debug.LogError(sqrDst);
        return sqrDst <= r * r;
    }

    public static bool InRange(Vector3 p, Vector3 c, float r, bool compareY = false)
    {
        if (!compareY) p.y = c.y;
        var sqrDst = SqrDist(p, c);
        return sqrDst <= r * r;
    }

    public static float SqrDist(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    public static Quaternion RandomRotation(bool onlyY = false)
    {
        if (onlyY) return Quaternion.Euler(0, Random.value * 360f, 0);
        return Quaternion.Euler(RandomVector3() * 180f);
    }

    public static Vector3 RandomVector3(bool zeroY = false)
    {
        var result = Random.insideUnitSphere;
        if (zeroY) result.y = 0;
        return result;
    }

    public static int GetNearestIndex(Vector3 pos, Vector3[] list)
    {
        var minDist = Mathf.Infinity;
        var index = -1;
        var dist = 0f;
        for (var i = 0; i < list.Length; i++)
        {
            dist = SqrDist(pos, list[i]);
            if (dist <= minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return index;
    }

    public static Vector3 RandomBetween(Vector3 a, Vector3 b)
    {
        return a + (b - a).normalized * Random.value * (b - a).magnitude;
    }
}