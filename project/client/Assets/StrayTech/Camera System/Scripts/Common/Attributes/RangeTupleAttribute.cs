using UnityEngine;
using System.Collections;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// Causes a Vector2 to be rendered as a "range" with a minimum and a maximum.
        /// </summary>
        public class RangeTupleAttribute : PropertyAttribute
        {
            /// <summary>
            /// The minimum value for this Vector2's X.
            /// </summary>
            public readonly float Min;

            /// <summary>
            /// The maximum value of the attached Vector2's Y.
            /// </summary>
            public readonly float Max;

            /// <summary>
            /// Whether or not only integer values should be settable in the inspector.
            /// </summary>
            public bool ConstrainToIntegralValues;

            /// <summary>
            /// Whether or not the args passed to this attribute's constructor were valid.
            /// </summary>
            public bool AllArgsValid = true;

            /// <summary>
            /// Denotes that this Vector2 should have it's X and Y values constrained between min and max.
            /// </summary>
            /// <param name="min">The minimum value of the attached Vector2's X.</param>
            /// <param name="max">The maximum value of the attached Vector2's Y.</param>
            /// <param name="constrainToInts">Whether or not the inspector should force the user to select only ints.</param>
            public RangeTupleAttribute(float min, float max, bool constrainToInts = false)
            {
                if (min >= max)
                {
                    this.AllArgsValid = false;
                }

                this.Min = min;
                this.Max = max;
                this.ConstrainToIntegralValues = constrainToInts;
            }
        }
    }
}
