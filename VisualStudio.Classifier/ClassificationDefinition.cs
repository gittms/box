using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Definitif.VisualStudio.Classifier
{
    internal static class ClassificationDefinitions
    {
        /// <summary>
        /// Default content type definition.
        /// </summary>
        [Export, Name("box"), BaseDefinition("text")]
        internal static ContentTypeDefinition boxContentTypeDefinition = null;
        /// <summary>
        /// Default extension to register.
        /// </summary>
        [Export, FileExtension(".box"), ContentType("text")]
        internal static FileExtensionToContentTypeDefinition boxFileExtensionDefinition = null;
        /// <summary>
        /// Default classification definition.
        /// </summary>
        [Export, Name("box")]
        internal static ClassificationTypeDefinition boxClassificationDefinition = null;

        /// <summary>
        /// Blue color highlight for keywords.
        /// </summary>
        [Export, Name("box.blue"), BaseDefinition("box")]
        internal static ClassificationTypeDefinition boxBlueDefinition = null;

        [Export(typeof(EditorFormatDefinition)), Name("box.blue")]
        [ClassificationType(ClassificationTypeNames = "box.blue")]
        internal sealed class BlueFormat : ClassificationFormatDefinition
        {
            public BlueFormat() { this.ForegroundColor = Colors.Blue; }
        }

        /// <summary>
        /// Cyan color highlight for model names and types.
        /// </summary>
        [Export, Name("box.cyan"), BaseDefinition("box")]
        internal static ClassificationTypeDefinition boxCyanDefinition = null;

        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition)), Name("box.cyan")]
        [ClassificationType(ClassificationTypeNames = "box.cyan")]
        internal sealed class CyanFormat : ClassificationFormatDefinition
        {
            public CyanFormat() { this.ForegroundColor = Color.FromRgb(0, 0xBB, 0); }
        }

        /// <summary>
        /// Red color highlight for strings.
        /// </summary>
        [Export, Name("box.red"), BaseDefinition("box")]
        internal static ClassificationTypeDefinition boxRedDefinition = null;

        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition)), Name("box.red")]
        [ClassificationType(ClassificationTypeNames = "box.red")]
        internal sealed class RedFormat : ClassificationFormatDefinition
        {
            public RedFormat() { this.ForegroundColor = Colors.Firebrick; }
        }

        /// <summary>
        /// Green color highlight for comments.
        /// </summary>
        [Export, Name("box.green"), BaseDefinition("box")]
        internal static ClassificationTypeDefinition boxGreenDefinition = null;

        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition)), Name("box.green")]
        [ClassificationType(ClassificationTypeNames = "box.green")]
        internal sealed class GreenFormat : ClassificationFormatDefinition
        {
            public GreenFormat() { this.ForegroundColor = Colors.ForestGreen; }
        }

        /// <summary>
        /// Gray color highlight for comment tags.
        /// </summary>
        [Export, Name("box.gray"), BaseDefinition("box")]
        internal static ClassificationTypeDefinition boxGrayDefinition = null;

        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition)), Name("box.gray")]
        [ClassificationType(ClassificationTypeNames = "box.gray")]
        internal sealed class GrayFormat : ClassificationFormatDefinition
        {
            public GrayFormat() { this.ForegroundColor = Colors.DarkGray; }
        }
    }
}
