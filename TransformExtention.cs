using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


 public static class TransformExtension
 {
     //Breadth-first search
    public static Transform FindChildBFS(this Transform Parent, string Name)
    {
        var result = Parent.Find(Name);
        if (result != null)
            return result;
        foreach(Transform child in Parent)
        {
            result = child.FindChildBFS(Name);
            if (result != null)
                return result;
        }
        return null;
    }

    

    //Depth-first search
    public static Transform FindChildDFS(this Transform Parent, string Name)
    {
      return Parent.Find(Name);
        Parent.Children()
          .Select(child => child.FindChildDFS(Name))
          .Where(result => result != null)
          .FirstOrDefault();
    }

    public static Transform FindChildbyTag(this Transform Parent, string Tag)
    {
		    return Parent.Children()
			     .Where(child => child.tag == Tag)
			     .FirstOrDefault();
    }


    //Breadth-first search
    public static Transform FindChildByTagBFS(this Transform Parent, string Tag)
    {
        var result = Parent.FindChildbyTag(Tag);
        if (result != null)
            return result;
        foreach(Transform child in Parent)
        {
            result = child.FindChildByTagBFS(Tag);
            if (result != null)
                return result;
        }
        return null;
    }


    //Depth-first search
    public static Transform FindChildByTagDFS(this Transform Parent, string Tag)
    {
      return Parent.FindChildbyTag(Tag) ??
        Parent.Children()
          .Select(child => child.FindChildByTagDFS(Tag))
          .Where(result => result != null)
          .FirstOrDefault();
    }

    
    public static Transform FindParentWithName(this Transform Child, string Name)
    {
        if(Child.parent==null)
            return null;
        
        if(Child.parent.name==Name)
            return Child.parent;
        else return FindParentWithName(Child.parent, Name);
    }

    public static Transform FindParentWithTag(this Transform Child, string Tag)
    {
        if(Child.parent==null)
            return null;
        
        if(Child.parent.tag==Tag)
            return Child.parent;
        else return FindParentWithTag(Child.parent, Tag);
    }

    public static Transform ClearChildren(this Transform transform)
    {
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }


    public static List<Transform> MoveChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild)
    {
        List<Transform> toReturn = new List<Transform>();
        for (int i=fromChild; i<toChild; i++)
        {
            Transform Child = parent.GetChild(i);
            Child.SetParent(newParent);

            toReturn.Add(Child);
        }

        return toReturn;
    }
 
    public static List<Transform> MoveAllChildrenToDifferentParent(this Transform parent, Transform newParent)
    {
        int to = parent.childCount;
        
        List<Transform> toReturn = MoveChildrenToDifferentParent(parent, newParent, 0, to);

        return toReturn;
    }

    
    public static List<Transform> CopyChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild)
    {
        List<Transform> toReturn = new List<Transform>();
        for (int i=fromChild; i<toChild; i++)
        {
            Transform newChild = CopyChild(newParent,parent.GetChild(i));

            toReturn.Add(newChild);
        }
        return toReturn;
    }
 

    

    public static List<Transform> CopyChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild, Vector3 childPosition, Space relativeTo = Space.World)
    {
        List<Transform> toReturn = parent.CopyChildrenToDifferentParent(newParent,fromChild,toChild);

        foreach (Transform newChild in toReturn)
        {
            PlaceInPosition(newChild,childPosition,relativeTo);
        }

        return toReturn;

    }

    public static List<Transform> CopyAllChildrenToDifferentParent(this Transform parent, Transform newParent, Vector3 childPosition, Space relativeTo = Space.World)
    {
       
        int to = parent.childCount;

        List<Transform> toReturn = CopyChildrenToDifferentParent(parent, newParent, 0, to,childPosition, relativeTo);
        
        return toReturn;
    }

    public static List<Transform> CopyAllChildrenToDifferentParent(this Transform parent, Transform newParent)
    {
       
        int to = parent.childCount;

        List<Transform> toReturn = CopyChildrenToDifferentParent(parent, newParent, 0, to);

        return toReturn;
    }

    public static void Move (this Transform objectToMove, Vector3 from, Vector3 to, float speed, Space relativeTo = Space.World)
    {
        MonoBehaviour mb =objectToMove.GetComponent<MonoBehaviour>();

        if (mb==null)
        {
            Debug.LogError("You are trying to call this function from a non-MonoBehaviour script. Please use a MonoBehaviour script.");
        }
        else
            mb.StartCoroutine(objectToMove.MoveFromTo(from,to,speed,relativeTo));
    }

    
    public static IEnumerator MoveFromTo (this Transform objectToMove, Vector3 from, Vector3 to, float speed, Space relativeTo = Space.World) {
        bool isRect = (objectToMove is RectTransform);

        float step = (speed / (from - to).magnitude) * Time.fixedDeltaTime;

        float t = 0;

        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            
            if(isRect) // deal the RectTransform movement
            {
                (objectToMove as RectTransform).anchoredPosition = Vector3.Lerp(from, to, t); // Move objectToMove closer to b
            }
            else if(relativeTo==Space.World)
            {                 
                objectToMove.position = Vector3.Lerp(from, to, t); // Move objectToMove closer to b
            }
            else if(relativeTo==Space.Self)
            {
                objectToMove.localPosition = Vector3.Lerp(from, to, t); // Move objectToMove closer to b
            }
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }

        if(isRect) // deal the RectTransform movement
        {
            (objectToMove as RectTransform).anchoredPosition = to;
        }
        else 
            PlaceInPosition(objectToMove,to,relativeTo);
        
    }
    
    // helper functions
    static Transform CopyChild (Transform newParent, Transform Child)
    {   
        Transform newChild = UnityEngine.Object.Instantiate(Child) as Transform;
        newChild.SetParent(newParent);
        
        return newChild;
    }

    static void PlaceInPosition(Transform child, Vector3 Position, Space relativeTo)
    {
        if(relativeTo == Space.World)
        {
            child.transform.position = Position;
        }
        else if (relativeTo == Space.Self)
        {
            child.transform.localPosition = Position;
        }
    }

    private static IEnumerable<Transform> Children(this Transform Parent) {
      foreach (Transform child in Parent) {
        yield return child;
      }
    }
 }

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }


    public static float GetLeft(this RectTransform rt)
    { 
        return rt.offsetMin.x;
    }

    public static float GetRight(this RectTransform rt)
    {
        return -rt.offsetMax.x;
    }

    public static float GetTop(this RectTransform rt)
    {
        return -rt.offsetMax.y;
    }

    public static float GetBottom(this RectTransform rt)
    {
        return rt.offsetMin.y;
    }
}
