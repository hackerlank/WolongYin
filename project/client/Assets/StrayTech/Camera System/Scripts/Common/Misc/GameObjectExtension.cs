using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace StrayTech
{
    /// <summary>
    /// Houses extension methods for gameobjects and related components. 
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Copy the local position, rotation and scale of another gameobject and apply to self. 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        public static void CopyLocalTransform(this GameObject self, GameObject other)
        {
            if (other == null)
                return;

            CopyLocalTransform(self.transform, other.transform);
        }

        /// <summary>
        /// Copy the local position, rotation and scale of another transform and apply to self. 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        public static void CopyLocalTransform(this Transform self, Transform other)
        {
            if (other == null)
                return;

            self.localPosition = other.localPosition;
            self.localRotation = other.localRotation;
            self.localScale = other.localScale;
        }

        /// <summary>
        /// Zeros out the local Transform. 
        /// </summary>
        /// <param name="self"></param>
        public static void ResetLocalTransform(this Transform self)
        {
            self.localPosition = Vector3.zero;
            self.localRotation = Quaternion.identity;
            self.localScale = Vector3.one;
        }

        /// <summary>
        /// Updates the given string by removing any specified substrings, adding a prefix string and adding a suffix string. 
        /// </summary>
        public static void NiceifyGameobjectName(this GameObject current, string prefix = "", string suffix = "", List<string> substringsToRemove = null)
        {
            if (string.IsNullOrEmpty(current.name))
                return;

            //use string builder since we  are doing quite a few string operations. 
            StringBuilder currentName = new StringBuilder(current.name);

            if (substringsToRemove != null)
            {
                foreach (var subString in substringsToRemove)
                {
                    if (string.IsNullOrEmpty(subString))
                        continue;

                    //Remove all specified substrings. 
                    currentName.Replace(subString, "");
                }
            }

            //Add the prefix. 
            currentName.Insert(0, prefix);
            //Add the suffix. 
            currentName.Append(suffix);

            current.name = currentName.ToString();
        }

        /// <summary>
        /// Searches for an interface in the gameobject. 
        /// Returns null if it cannot find one. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static T GetInterface<T>(this GameObject toSearch)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogWarning("The provided type was not an interface!");
                return null;
            }

            var monoBehaviors = toSearch.GetComponents<MonoBehaviour>();

            foreach (var mono in monoBehaviors)
            {
                if (mono is T)
                {
                    return mono as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all interfaces attached to this object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static T[] GetInterfaces<T>(this GameObject toSearch)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogWarning("The provided type was not an interface!");
                return null;
            }

            List<T> toReturn = new List<T>();

            var monoBehaviors = toSearch.GetComponents<MonoBehaviour>();

            foreach (var mono in monoBehaviors)
            {
                if (mono is T)
                    toReturn.Add(mono as T);
            }

            return toReturn.ToArray();
        }

        /// <summary>
        /// Searches for all interfaces in the gameobject. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static T[] GetInterfacesInChildren<T>(this GameObject toSearch, bool includeInactive, bool searchSelf = false)
            where T : class
        {
            List<T> foundChildren = new List<T>();

            if (toSearch == null)
            {
                return foundChildren.ToArray();
            }

            if (!typeof(T).IsInterface)
                return foundChildren.ToArray();

            var childObjects = toSearch.GetComponentsInChildren<MonoBehaviour>(includeInactive);

            foreach (var child in childObjects)
            {
                if (searchSelf == false)
                {
                    if (child == toSearch)
                        continue;
                }

                if (child is T)
                    foundChildren.Add(child as T);
            }

            return foundChildren.ToArray();
        }

        /// <summary>
        /// Searches every transform in the heirarchy, returns the first mono which implement the provided interface. 
        /// </summary>
        public static T GetInterfaceInChildren<T>(this GameObject toSearch, bool includeInactive)
            where T : class
        {
            if (!typeof(T).IsInterface)
                return null;

            var childObjects = toSearch.GetComponentsInChildren<MonoBehaviour>(includeInactive);

            foreach (var child in childObjects)
            {
                if (child is T)
                    return child as T;
            }

            return null;
        }

        /// <summary>
        /// Returns the instance of the given MonoBehaviour on this GameObject, or adds one and returs it if none already exists.
        /// </summary>
        /// <typeparam name="TMonoBehaviour">The type of MonoBehaviour to return.</typeparam>
        /// <param name="source">This GameOBject.</param>
        /// <returns>An instance of TMonoBehaviour attached to this GameObject.</returns>
        public static TMonoBehaviour AddOrGetComponent<TMonoBehaviour>(this GameObject source)
            where TMonoBehaviour : Component
        {
            TMonoBehaviour output = source.GetComponent<TMonoBehaviour>();

            if (output == null)
            {
                output = source.AddComponent<TMonoBehaviour>();
            }

            return output;
        }

        /// <summary>
        /// Search the transform upwards. Returns the first found matching component. 
        /// </summary>
        public static T GetComponentUpwards<T>(this GameObject toSearch, bool searchSelfFirst = false)
            where T : Component
        {
            if (searchSelfFirst == false)
            {
                if (toSearch.transform.parent == null)
                    return null;
            }

            Transform currentParent = (searchSelfFirst == true) ? toSearch.transform : toSearch.transform.parent;
            T match = null;

            while (currentParent != null)
            {
                match = currentParent.GetComponent<T>();

                if (match != null)
                    return match;

                currentParent = currentParent.parent;
            }

            return match;
        }

        /// <summary>
        /// Returns the first child of the gameobject. 
        /// </summary>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static GameObject GetFirstChild(this GameObject toSearch)
        {
            var allChildren = toSearch.GetComponentsInChildren<Transform>(true);

            if (allChildren.Length < 2)
                return null;

            return allChildren[1].gameObject;
        }

        /// <summary>
        /// Commonly used method. Creates a child gameobject under the parent with the provided name. 
        /// Zeros out the new gameobjects local transform after parenting. 
        /// </summary>
        public static GameObject CreateChild(this GameObject parent, string name)
        {
            GameObject newGameObject = new GameObject(name);
            newGameObject.transform.parent = parent.transform;
            newGameObject.transform.ResetLocalTransform();

            return newGameObject;
        }

        /// <summary>
        /// Finds child by name. Searches deeper than the corresponding unity search. 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject FindChildDeep(this Transform parent, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var allChildren = parent.GetComponentsInChildren<Transform>(true);
            foreach (var transform in allChildren)
            {
                if (transform.name == name)
                    return transform.gameObject;
            }

            return null;
        }

        /// <summary>
        /// Get Component in children, even if disabled. 
        /// </summary>
        public static T GetComponentInChildren<T>(this GameObject component, bool includeInactive)
        where T : Component
        {
            var matches = component.GetComponentsInChildren<T>(includeInactive);

            if (matches.Length == 0)
                return default(T);

            return matches[0];
        }

        /// <summary>
        /// Get Component in children, even if disabled. 
        /// </summary>
        public static T GetComponentInChildren<T>(this Component component, bool includeInactive)
        where T : Component
        {
            return component.gameObject.GetComponentInChildren<T>(includeInactive);
        }

        /// <summary>
        /// Find a gameobject by name in the scene, or create if it wasn't found. 
        /// SEARCHES ONLY TOP LEVEL!! 
        /// </summary>
        public static GameObject FindOrCreate(string name)
        {
            //Add / to search only top level objects in scene! 
            var toReturn = GameObject.Find("/" + name);

            if (toReturn == null)
            {
                toReturn = new GameObject(name);
            }

            return toReturn;
        }

        /// <summary>
        /// Look at the target, ignoring the Y position differences. 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        public static void LookAtXZ(this Transform self, Transform target)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return;
            }

            if (target == null)
                return;

            self.LookAtXZ(target.position);
        }

        /// <summary>
        /// Look at the target, ignoring the Y position differences. 
        /// </summary>
        public static void LookAtXZ(this Transform self, Vector3 worldTarget)
        {
            //Project on a Z,X plane so we dont rotate around up. 
            var targetPosition = new Vector3(worldTarget.x, 0, worldTarget.z);
            var selfPosition = new Vector3(self.position.x, 0, self.position.z);

            //make the look rotation facing from owner to target. 
            var rotationTarget = Quaternion.LookRotation(targetPosition - selfPosition);

            //Now smooth damp rotation to face the new target. 
            self.transform.rotation = rotationTarget;
        }

        /// <summary>
        /// Find the child if it exists, if not add it!
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Transform AddOrGetChild(this Transform parent, string childName)
        {
            //validate input. 
            if (string.IsNullOrEmpty(childName))
                return null;

            Transform result = null;

            //Search for the child first. 
            result = parent.transform.FindChild(childName);

            if (result == null)
            {
                //We didn't find the child, add it. 
                result = new GameObject(childName).transform;
                result.parent = parent.transform;
                result.ResetLocalTransform();
            }

            //return the result. 
            return result;
        }

        /// <summary>
        /// Same as transform.FindChild but this returns a GameObject reference, not a transform. 
        /// </summary>
        public static GameObject FindChild(this GameObject self, string childName)
        {
            if (string.IsNullOrEmpty(childName))
                return null;

            var result = self.transform.FindChild(childName);

            if (result == null)
                return null;

            return result.gameObject;
        }

        /// <summary>
        /// Find n return all monos in the scene which implement the provided interface. 
        /// </summary>
        public static IEnumerable<T> FindAllInterfaces<T>()
            where T : class
        {
            if (!typeof(T).IsInterface)
                yield return null;

            var allMonos = GameObject.FindObjectsOfType<MonoBehaviour>();

            foreach (var mono in allMonos)
            {
                if (mono is T)
                    yield return mono as T;
            }
        }

        /// <summary>
        /// Is this component null or invalid? 
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsNullOrInvalid(this IValidates self)
        {
            return self == null || self.IsValid() == false;
        }

        /// <summary>
        /// For each gameobject in the collection try to scrape the provided interface off of it. 
        /// </summary>
        public static IEnumerable<TInterface> ScrapeInterfaces<TInterface>(this IEnumerable<GameObject> self)
            where TInterface : class
        {
            if (!typeof(TInterface).IsInterface)
                yield return null;

            if (self == null)
                yield return null;

            foreach (var item in self)
            {
                if (item == null)
                    continue;

                var foundInterface = item.GetInterface<TInterface>();

                if (foundInterface == null)
                    continue;

                yield return foundInterface;
            }
        }

        /// <summary>
        /// Returns the full hierarchy path from the topmost GameObject to this one.
        /// </summary>
        public static string FullPath(this GameObject self)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return string.Empty;
            }

            Stack<string> hierarchyObjectNames = new Stack<string>();

            Transform current = self.transform;
            while (current != null)
            {
                hierarchyObjectNames.Push(current.gameObject.name);

                current = current.parent;
            }

            StringBuilder sb = new StringBuilder();
            while (hierarchyObjectNames.Count > 0)
            {
                sb.Append(hierarchyObjectNames.Pop());
                sb.Append('.');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the full hierarchy path from the topmost GameObject to this Component.
        /// </summary>
        public static string FullPath(this Component self)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return string.Empty;
            }

            return string.Format("{0}{1}", self.gameObject.FullPath(), self.GetType().Name);
        }

        /// <summary>
        /// Return all children of the provided GameObject. 
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<GameObject> GetAllChildren(this GameObject self)
        {
            List<GameObject> toReturn = new List<GameObject>();

            if (self == null)
            {
                return toReturn;
            }

            int childCount = self.transform.childCount;

            for (int i = 0; i <= childCount - 1; i++)
            {
                toReturn.Add(self.transform.GetChild(i).gameObject);
            }

            return toReturn;
        }

        /// <summary>
        /// Populate the provided list with all of the top level children of this object. 
        /// </summary>
        public static void GetAllChildren(this GameObject self, ref List<GameObject> toPopulate)
        {
            if (self == null)
            {
                return;
            }

            int childCount = self.transform.childCount;

            if (toPopulate == null)
            {
                toPopulate = new List<GameObject>(childCount);
            }
            else
            {
                //Clear the collection. 
                toPopulate.Clear();
            }

            //Add all child roots to the collection.
            for (int i = 0; i <= childCount - 1; i++)
            {
                toPopulate.Add(self.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Return all children who match the provided predicate. 
        /// </summary>
        public static IEnumerable<T> GetComponentsInChildren<T>(this Component self, Predicate<T> filter, bool includeInactive = false)
            where T : Component
        {
            if (self == null)
            {
                yield break;
            }

            if (filter == null)
            {
                yield break;
            }

            foreach (var foundChild in self.GetComponentsInChildren<T>(includeInactive))
            {
                if (filter(foundChild) == true)
                {
                    yield return foundChild;
                }
            }
        }

        /// <summary>
        /// Return the first child component matching the provided predicate, if any. 
        /// </summary>
        public static T GetComponentInChildren<T>(this Component self, Predicate<T> filter)
            where T : Component
        {
            if (self == null)
            {
                return null;
            }

            if (filter == null)
            {
                return null;
            }

            foreach (var foundChild in self.GetComponentsInChildren<T>())
            {
                if (filter(foundChild) == true)
                {
                    return foundChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Helper method which attempts to
        /// 1. Load the provided asset in the resources folder. 
        /// 2. Get a MonoBehaviour Component of type T from the loaded asset
        /// </summary>
        /// <returns></returns>
        public static T GetComponentFromLoadedResource<T>(string resourcePath)
            where T : Component
        {
            if (string.IsNullOrEmpty(resourcePath))
                return null;

            //First try to load the object at the path.
            var resource = Resources.Load<GameObject>(resourcePath);

            if (resource == null)
            {
                Debug.LogErrorFormat("Could not load resource at path: {0}", resourcePath);
                return null;
            }

            //Try to load and instantiate the resource specified at the asset path. 
            var instantiatedObject = GameObject.Instantiate(resource) as GameObject;

            if (instantiatedObject == null)
            {
                Debug.LogError("Could not load resource at asset path: " + resourcePath);
                //Be safe and destroy immediate.. the Instantiate ~*could*~ have created something (if the 
                //asset it loaded was not a GameObject the "as" check would have made loadedResource null, however something 
                //definitely was loaded). So just call this, passing in null wont explode so it can't hurt. 
                GameObject.Destroy(instantiatedObject);
                return null;
            }

            //Attempt to get the desired component. 
            var foundComponent = instantiatedObject.GetComponent<T>();

            if (foundComponent == null)
            {
                //bail if spawned prefab didn't have the requested component. 
                Debug.LogError(string.Format("Could not get a {0} component from asset loaded from path {1}. Destroying!", typeof(T).Name, resourcePath));
                GameObject.Destroy(instantiatedObject);
                return null;
            }

            return foundComponent;
        }
    }
}