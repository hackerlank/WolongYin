using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// Allows classes to perform validation, and returns whether or not it has passed it's IsValid() function. 
    /// </summary>
    public interface IValidates
    {
        #region methods
            /// <summary>
            /// Does this instance validate? 
            /// </summary>
            bool IsValid();
        #endregion methods
    }

    /// <summary>
    /// Raised by the caller of IValidates.IsValid if validation fails.
    /// </summary>
    [System.Serializable]
    public class FailedValidationException : System.Exception
    {
        public FailedValidationException() { }
        public FailedValidationException(string message) : base(message) { }
        public FailedValidationException(string message, System.Exception inner) : base(message, inner) { }
        protected FailedValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}