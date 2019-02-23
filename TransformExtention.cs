using UnityEngine;
using System.Collections;
using UnityEditor;
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
        foreach(Transform child in Parent)
        {
            if(child.name == Name )
                return child;
            var result = child.FindChildDFS(Name);
            if (result != null)
                return result;
        }
        return null;
    }

    //Breadth-first search
    public static Transform FindChildByTagBFS(this Transform Parent, string Tag)
    {
        if(child.tag == Tag )
            return child;
        foreach(Transform child in Parent)
        {
            var result = child.FindChildByTagBFS(Tag);
            if (result != null)
                return result;
        }
        return null;
    }
    //Depth-first search
    public static Transform FindChildByTagDFS(this Transform Parent, string Tag)
    {
        foreach(Transform child in Parent)
        {
            if(child.tag == Tag )
                return child;
            var result = child.FindChildByTagDFS(Tag);
            if (result != null)
                return result;
        }
        return null;
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

    public static Transform FindChildByTag(this Transform Parent, string Tag)
    {
         foreach(Transform child in Parent)
         {
             if(child.tag == Tag )
                 return child;
         }
         return null;
     }

    public static Transform ClearChildren(this Transform transform)
     {
         foreach (Transform child in transform) {
             GameObject.Destroy(child.gameObject);
         }
         return transform;
     }


    public static void MoveChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild)
    {
        for (int i=fromChild; i<toChild; i++)
        {
            parent.GetChild(i).SetParent(newParent);
        }
    }
 
    public static void MoveAllChildrenToDifferentParent(this Transform parent, Transform newParent)
    {
        int to = parent.childCount;
        MoveChildrenToDifferentParent(parent, newParent, 0, to);
    }

    public static void CopyChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild)
    {
        for (int i=fromChild; i<toChild; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            GameObject newChild = PrefabUtility.InstantiatePrefab(child) as GameObject;
            newChild.transform.SetParent(newParent);
        }
    }
 
    public static void CopyChildrenToDifferentParent(this Transform parent, Transform newParent, int fromChild, int toChild, Vector3 childPosition, Space relativeTo = Space.World)
    {
        for (int i=fromChild; i<toChild; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            GameObject newChild = PrefabUtility.InstantiatePrefab(child) as GameObject;
            newChild.transform.SetParent(newParent);

            if(relativeTo == Space.World)
                    newChild.transform.position = childPosition;
            else if (relativeTo == Space.Self)
                    newChild.transform.localPosition = childPosition;
        }
    }

    public static void MoveFromTo (this Transform objectToMove, Vector3 from, Vector3 to, float speed, Space relativeTo = Space.World)
    {
        MonoBehaviour mb =objectToMove.GetComponent<MonoBehaviour>();
        if (mb==null)
        {
            Debug.LogError("You are trying to call this function from a non-MonoBehaviour script. Please use a MonoBehaviour script or use StartCoroutine(transform.MoveFromTo(...))");
        }
        else
            mb.StartCoroutine(transform.MoveFromTo(objectToMove,from,to,speed,relativeTo));
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
        else if(relativeTo==Space.World)
        {
            objectToMove.position = to;
        }
        else if(relativeTo==Space.Self)
        {
            objectToMove.localPosition = to;
        } 
    }


 }