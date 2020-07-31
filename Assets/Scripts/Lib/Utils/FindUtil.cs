using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// ComponentUtilクラス
/// </summary>
public static class FindUtil
{
    /// <summary>
    /// Deeps the find equal.
    /// </summary>
    /// <returns>The find equal.</returns>
    /// <param name="self">Self.</param>
    /// <param name="name">Name.</param>
    /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
    public static GameObject DeepFindEqual(this GameObject self, string name, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive).Where(c => self != c.gameObject);
        foreach (var transform in children)
        {
            if (transform == self.transform)
            {
                continue;
            }

            if (transform.name.Equals(name))
            {
                return transform.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// Deeps the find.
    /// </summary>
    /// <returns>The find.</returns>
    /// <param name="self">Self.</param>
    /// <param name="name">Name.</param>
    /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
    public static GameObject DeepFind(this GameObject self, string name, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive).Where(c => self != c.gameObject);
        foreach (var transform in children)
        {
            if (transform == self.transform)
            {
                continue;
            }

            if (Regex.IsMatch(transform.name, name))
            {
                return transform.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// Deeps the finds.
    /// </summary>
    /// <returns>The finds.</returns>
    /// <param name="self">Self.</param>
    /// <param name="name">Name.</param>
    /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
    public static List<GameObject> DeepFinds(this GameObject self, string name, bool includeInactive = false)
    {
        var objs = new List<GameObject>();
        var children = self.GetComponentsInChildren<Transform>(includeInactive).Where(c => self != c.gameObject);
        foreach (var transform in children)
        {
            if (transform == self.transform)
            {
                continue;
            }

            if (Regex.IsMatch(transform.name, name))
            {
                objs.Add(transform.gameObject);
            }
        }

        return objs;
    }

    /// <summary>
    /// Gets the child.
    /// </summary>
    /// <returns>The child.</returns>
    /// <param name="self">Self.</param>
    /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
    public static GameObject GetChild(this GameObject self, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive);
        foreach (var transform in children)
        {
            if (transform == self.transform)
            {
                continue;
            }

            return transform.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Gets the child.
    /// </summary>
    /// <returns>The child.</returns>
    /// <param name="self">Self.</param>
    /// <param name="name">Name.</param>
    /// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
    public static GameObject GetChild(this GameObject self, string name, bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive).Where(c => self != c.gameObject);
        foreach (var transform in children)
        {
            if (transform == self.transform)
            {
                continue;
            }

            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }

        return null;
    }
}
