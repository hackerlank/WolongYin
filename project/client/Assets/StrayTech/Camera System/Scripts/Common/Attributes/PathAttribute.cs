using UnityEngine;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// Causes a string field to be rendered with a path selection button.
        /// </summary>
        public class PathAttribute : PropertyAttribute
        {
            #region inner types
                /// <summary>
                /// The type of selection dialog that will be presented to the user.
                /// </summary>
                public enum SelectionType
                {
                    /// <summary>
                    /// A selection dialog that presents a folder picker.
                    /// </summary>
                    Folder,

                    /// <summary>
                    /// A selection dialog that presents a file picker.
                    /// </summary>
                    File,
                }
            #endregion inner types

            #region members
                /// <summary>
                /// The type of path selection dialog that is shown for this field.
                /// </summary>
                public readonly SelectionType PathType;

                /// <summary>
                /// If this field is used to select a file, the value FilePathExtension contstrains the valid file extensions that can be selected.
                /// </summary>
                public readonly string FileExtension;

                /// <summary>
                /// Whether or not the selected path is relative to the Project Assets folder.
                /// </summary>
                public readonly bool RelativeToAssetsFolder;
            #endregion members

            #region constructors
                /// <summary>
                /// Causes this field to be rendered with a button that allows easy selection of a file path.
                /// </summary>
                /// <param name="fileExtension">The extension that file selection for this field should be constrained to. Do not include the preceding period.</param>
                /// <param name="relativeToAssetsFolder">Whether or not the selected path is relative to the Project Assets folder.</param>
                public PathAttribute(string fileExtension, bool relativeToAssetsFolder = false)
                {
                    this.PathType = SelectionType.File;
                    this.FileExtension = fileExtension ?? string.Empty;
                    this.RelativeToAssetsFolder = relativeToAssetsFolder;
                }

                /// <summary>
                /// Causes this field to be rendered with a button athat allows easy selection of a path.
                /// </summary>
                /// <param name="pathType">The type of path selection that is allowed.</param>
                /// <param name="relativeToAssetsFolder">Whether or not the selected path is relative to the Project Assets folder.</param>
                public PathAttribute(SelectionType pathType, bool relativeToAssetsFolder = false)
                {
                    this.PathType = pathType;
                    this.FileExtension = string.Empty;
                    this.RelativeToAssetsFolder = relativeToAssetsFolder;
                }

                /// <summary>
                /// Causes this field to be rendered with a button that allows easy selection of a file path.
                /// </summary>
                /// <param name="relativeToAssetsFolder">Whether or not the selected path is relative to the Project Assets folder.</param>
                public PathAttribute(bool relativeToAssetsFolder = false)
                {
                    this.PathType = SelectionType.File;
                    this.FileExtension = string.Empty;
                    this.RelativeToAssetsFolder = relativeToAssetsFolder;
                }
            #endregion constructors
        }
    }
}