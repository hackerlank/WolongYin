using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

namespace StrayTech
{
    /// <summary>
    /// Houses extension methods relating to .net reflection. 
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// Returns a collection of the custom attribute specified if any are found on this object.
        /// IGNORES INHERIT!
        /// </summary>
        /// <typeparam name="TAttribute">The type of custom attribute to search for.</typeparam>
        /// <param name="toScan">This object.</param>
        /// <param name="inherit">Whether or not to search the object's inheritance hierarchy for isntances of TAttribute.</param>
        /// <returns>An IList:lt;TAttribute&gt; containing the instances of TAttribute found on this object, or an empty set if none are found.</returns>
        public static IList<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider toScan, bool inherit = false)
            where TAttribute : System.Attribute
        {
            List<TAttribute> output = new List<TAttribute>();

            foreach (System.Attribute customAttribute in toScan.GetCustomAttributes(typeof(TAttribute), inherit))
            {
                TAttribute asDesiredType = customAttribute as TAttribute;

                if (asDesiredType != null)
                {
                    output.Add(asDesiredType);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a custom attribute of the specified type if found on this object.
        /// IGNORES INHERIT!
        /// </summary>
        /// <typeparam name="TAttribute">The type of custom attribute to search for.</typeparam>
        /// <param name="toScan">This object.</param>
        /// <param name="inherit">Whether or not to search the object's inheritance hierarchy for instances of TAttribute.</param>
        /// <returns>An instance of TAttribute if one was found, or null.</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider toScan, bool inherit = false)
            where TAttribute : System.Attribute
        {
            IList<TAttribute> foundAttributes = toScan.GetCustomAttributes<TAttribute>(inherit);

            if (foundAttributes.Count == 0)
            {
                return null;
            }

            if (foundAttributes.Count > 1)
            {
                Debug.LogWarning(string.Format("Searched for an instance of '{0}', but found more then one. Returning only the first.", typeof(TAttribute).Name));
            }

            return foundAttributes[0];
        }

        /// <summary>
        /// Returns a collection of the custom attribute specified if any are found on this FieldInfo.
        /// DOES NOT IGNORE INHERIT!
        /// </summary>
        /// <typeparam name="TAttribute">The type of custom attribute to search for.</typeparam>
        /// <param name="toScan">This object.</param>
        /// <param name="inherit">Whether or not to search the object's inheritance hierarchy for isntances of TAttribute.</param>
        /// <returns>An IList:lt;TAttribute&gt; containing the instances of TAttribute found on this object, or an empty set if none are found.</returns>
        public static IList<TAttribute> GetFieldAttributes<TAttribute>(this FieldInfo field, bool inherit = false)
            where TAttribute : System.Attribute
        {
            var output = new List<TAttribute>();

            foreach (var customAttribute in System.Attribute.GetCustomAttributes(field, typeof(TAttribute), inherit))
            {
                TAttribute asDesiredType = customAttribute as TAttribute;

                if (asDesiredType != null)
                {
                    output.Add(asDesiredType);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a custom attribute of the specified type if found on this object.
        /// DOES NOT IGNORE INHERIT!
        /// </summary>
        /// <typeparam name="TAttribute">The type of custom attribute to search for.</typeparam>
        /// <param name="toScan">This object.</param>
        /// <param name="inherit">Whether or not to search the object's inheritance hierarchy for instances of TAttribute.</param>
        /// <returns>An instance of TAttribute if one was found, or null.</returns>
        public static TAttribute GetFieldAttribute<TAttribute>(this FieldInfo field, bool inherit = false)
            where TAttribute : System.Attribute
        {
            IList<TAttribute> foundAttributes = field.GetFieldAttributes<TAttribute>(inherit);

            if (foundAttributes.Count == 0)
            {
                return null;
            }

            if (foundAttributes.Count > 1)
            {
                Debug.LogWarning(string.Format("Searched for an instance of '{0}', but found more then one. Returning only the first.", typeof(TAttribute).Name));
            }

            return foundAttributes[0];
        }


        /// <summary>
        /// Return all fields contains in the provided type and its base classes. 
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }


        /// Copies the values from the target component to this component.
        /// </summary>
        /// <param name="source">The Component who's values we want to overwrite.</param>
        /// <param name="target">The Component who's values will overwrite this Component's values.</param>
        public static void CopyFromOther(this UnityEngine.Object source, UnityEngine.Object target)
        {
            if (source == null || target == null)
                throw new UnityException("source and/or target cannot be null!");

            if (source.GetType() != target.GetType())
                throw new UnityException("The source and target components must be of the same (most-derived) type!");

            //Debug.Log(target.GetType().ToString() + " " + target.GetType().GetFields().Length);
            foreach (FieldInfo field in target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                //Debug.Log(string.Format("Copying field '{0}'", field.Name));
                field.SetValue(source, field.GetValue(target));
            }
        }

        /// Copies the values from the target component to this component, allows you to skip copying certain Syste.Types. 
        /// </summary>
        /// <param name="source">The Component who's values we want to overwrite.</param>
        /// <param name="target">The Component who's values will overwrite this Component's values.</param>
        public static void CopyFromOther(this UnityEngine.Object source, UnityEngine.Object target, HashSet<System.Type> typesToSkip)
        {
            if (source == null || target == null || typesToSkip == null)
                throw new UnityException("source and/or target ( or types to skip) cannot be null!");

            if (source.GetType() != target.GetType())
                throw new UnityException("The source and target components must be of the same (most-derived) type!");

            foreach (FieldInfo field in target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (typesToSkip.Contains(field.FieldType))
                {
                    //Debug.Log("skipping copy of field: " + field.Name + " because it was in the skip type!");
                    continue;
                }

                //Debug.Log(string.Format("Copying field '{0}'", field.Name));
                field.SetValue(source, field.GetValue(target));
            }
        }
    }
}