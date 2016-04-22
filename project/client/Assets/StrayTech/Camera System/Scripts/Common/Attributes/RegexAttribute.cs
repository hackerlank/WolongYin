using System.Text.RegularExpressions;
using UnityEngine;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// Forces the given text field to conform to the provided regular expression.
        /// </summary>
        public class RegexAttribute : PropertyAttribute
        {
            #region inner types
                /// <summary>
                /// Represents different ways the input field can react to invalid input.
                /// </summary>
                public enum Mode
                {
                    /// <summary>
                    /// Input is not accecpted into the field unless it would match the regular expression.
                    /// </summary>
                    Force,

                    /// <summary>
                    /// Invalid input is allowed, but the field's value will not be saved if it's not valid.
                    /// </summary>
                    DisplayInvalid
                }
            #endregion inner types

            #region members
                /// <summary>
                /// The regular expression to match the text field against.
                /// </summary>
                public readonly string Pattern;

                /// <summary>
                /// How this field should react to invalid input.
                /// </summary>
                public readonly Mode InputMode;

                /// <summary>
                /// Controls additional options for the parsing of the regular expression.
                /// </summary>
                public readonly RegexOptions MatchOptions;
            #endregion members

            #region constructors
                /// <summary>
                /// Creates a new RegularExpression attribute.
                /// </summary>
                /// <param name="regularExpression">The regular expression to match the text field against.</param>
                /// <param name="inputMode">How this field should react to invalid input.</param>\
                /// <param name="matchOptions">Controls additional options for the parsing of the regular expression.</param>
                public RegexAttribute(string pattern, Mode inputMode = Mode.DisplayInvalid, RegexOptions matchOptions = RegexOptions.Singleline)
                {
                    this.Pattern = pattern ?? string.Empty;
                    this.InputMode = inputMode;
                    this.MatchOptions = matchOptions;
                }
            #endregion constructors
        }
    }
}