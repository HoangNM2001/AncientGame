using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public static class TransformExtensions
{
    public static void RandomRotation(this Transform transform, bool onlyY = false)
    {
        transform.rotation = SimpleMath.RandomRotation(onlyY);
    }

    public static void RandomLocalRotation(this Transform transform, bool onlyY = false)
    {
        transform.localRotation = SimpleMath.RandomRotation(onlyY);
    }

    public static void Copy(this Transform transform, Transform other, bool position = false, bool rotation = false,
        bool scale = false, bool otherLossyScale = false)
    {
        if (position) transform.position = other.transform.position;
        if (rotation) transform.rotation = other.transform.rotation;
        if (scale)
        {
            transform.localScale = otherLossyScale ? other.transform.lossyScale : other.transform.localScale;
        }
    }

    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}

public static class NavmeshExtension
{
    public static bool IsReachDestination(this NavMeshAgent agent, float threshold = 0.1f)
    {
        return !agent.pathPending && agent.remainingDistance < agent.stoppingDistance + threshold;
    }
}

public static class EnumerationExtensions
{
    public static bool Has<T>(this Enum type, T value)
    {
        try
        {
            return (((int)(object)type & (int)(object)value) == (int)(object)value);
        }
        catch
        {
            return false;
        }
    }

    public static bool Is<T>(this Enum type, T value)
    {
        try
        {
            return (int)(object)type == (int)(object)value;
        }
        catch
        {
            return false;
        }
    }

    public static T Add<T>(this Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type | (int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
        }
    }


    public static T Remove<T>(this Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type & ~(int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
        }
    }
}