using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// A CustomAttribute which will render a helpbox. 
        /// </summary>
        public class Helpbox : PropertyAttribute, IValidates
        {
            #region inner classes
                /// <summary>
                /// Cant use UnityEditor.MessageType because that requires UnityEditor. 
                /// </summary>
                public enum Type
                {
                    Info,
                    Warning,
                    Error
                }
            #endregion inner classes

            #region members
                /// <summary>
                /// The message type to display. 
                /// </summary>
                public readonly Type MessageType;
                /// <summary>
                /// The message to display. 
                /// </summary>
                public readonly string Message;
            #endregion members

            #region constructors
                public Helpbox(string message, Type displayType = Type.Info)
                {
                    this.MessageType = displayType;
                    this.Message = message;
                }
            #endregion constructors

            #region methods
                /// <summary>
                /// Do we have our valid components? 
                /// </summary>
                /// <returns></returns>
                public bool IsValid()
                {
                    if (string.IsNullOrEmpty(Message) == true)
                        return false;

                    return true;
                }
            #endregion methods
        }
    }
}