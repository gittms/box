using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace Definitif.VisualStudio.Classifier
{
    public class Classifier : IClassifier
    {
        IClassificationTypeRegistryService registry;

        internal Classifier(IClassificationTypeRegistryService registry)
        {
            this.registry = registry;
        }

        #pragma warning disable 67
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        #pragma warning restore 67

        private static string[] keywords = new string[] {
                "public", "private", "protected", "internal",
                "static",
                "new", "return", "get", "set", "in",
                "this", "if", "then", "break",
                "foreach", "for", "while",
                "true", "false", "string", "int",

                "model", "foreign key", "primary key",
                "many to many"
        };

        public void ProcessLine(ITextSnapshotLine line, List<ClassificationSpan> result)
        {
            string text = line.GetText();

            IClassificationType
                stringType = this.registry.GetClassificationType("box.red"),
                keywordType = this.registry.GetClassificationType("box.blue"),
                commentType = this.registry.GetClassificationType("box.green"),
                typeType = this.registry.GetClassificationType("box.cyan"),
                autodocType = this.registry.GetClassificationType("box.gray");

            int commentPosition;
            // Autodocumentation comments lines.
            commentPosition = text.IndexOf("///");
            if (commentPosition >= 0)
            {
                if (commentPosition == 0 || text.Trim().IndexOf("///") == 0)
                {
                    result.Add(new ClassificationSpan(
                        new SnapshotSpan(line.Snapshot, new Span(line.Start + commentPosition, line.Length - commentPosition)),
                        autodocType));
                    return;
                }
            }
            // Generic single line comments.
            commentPosition = text.IndexOf("//");
            if (commentPosition >= 0)
            {
                result.Add(new ClassificationSpan(
                    new SnapshotSpan(line.Snapshot, new Span(line.Start + commentPosition, line.Length - commentPosition)),
                    commentType));
                if (text.Trim().IndexOf("//") == 0) return;
            }

            // Keywords.
            foreach (string keyword in keywords)
            {
                int keywordPosition = text.IndexOf(keyword),
                    keywordPositionEnd = keywordPosition + keyword.Length;

                if (keywordPosition < 0 || (commentPosition >= 0 && keywordPosition > commentPosition)) continue;

                // If no leading whitespace character..
                if (keywordPosition > 0 && !char.IsWhiteSpace(text[keywordPosition - 1])) continue;
                // .. or trailing one.
                if (keywordPositionEnd < text.Length - 1 && !char.IsWhiteSpace(text[keywordPositionEnd])) continue;

                result.Add(new ClassificationSpan(
                    new SnapshotSpan(line.Snapshot, new Span(line.Start + keywordPosition, keyword.Length)),
                    keywordType));
            }

            // String definition.
            int stringSearchStart = 0;
            while (true)
            {
                int stringPosition = text.IndexOf("\"", stringSearchStart);
                if (stringPosition >= stringSearchStart)
                {
                    int stringClosePosition = text.IndexOf("\"", stringPosition + 1);
                    if (stringClosePosition > stringSearchStart)
                    {
                        result.Add(new ClassificationSpan(
                            new SnapshotSpan(line.Snapshot, new Span(line.Start + stringPosition, stringClosePosition - stringPosition + 1)),
                            stringType));
                        stringSearchStart = stringClosePosition + 1;
                    }
                    else
                    {
                        result.Add(new ClassificationSpan(
                            new SnapshotSpan(line.Snapshot, new Span(line.Start + stringPosition, line.End - (line.Start + stringPosition))),
                            stringType));
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Classify the given spans, which, for diff files, classifies
        /// a line at a time.
        /// </summary>
        /// <param name="span">The span of interest in this projection buffer.</param>
        /// <returns>The list of <see cref="ClassificationSpan"/> as contributed by the source buffers.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> result = new List<ClassificationSpan>();
            if (span.Length == 0) return result;

            ITextSnapshot snapshot = span.Snapshot;
            ITextSnapshotLine line = snapshot.GetLineFromPosition(span.Start.Position);
            ITextSnapshotLine endLine = snapshot.GetLineFromPosition(span.End.Position);

            while (true)
            {
                ProcessLine(line, result);

                if (line.LineNumber == endLine.LineNumber) break;
                line = snapshot.GetLineFromPosition(line.EndIncludingLineBreak + 1);
            }

            return result;
        }
    }
}
