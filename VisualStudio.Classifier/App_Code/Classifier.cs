using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace Definitif.VisualStudio.Classifier
{
    /// <summary>
    /// Represents classifier regular expression.
    /// </summary>
    internal class ClassifierRegex
    {
        private Regex expression;
        private IClassificationType type;

        public ClassifierRegex(Regex expression, IClassificationType type)
        {
            this.expression = expression;
            this.type = type;
        }

        public void ProcessLine(ITextSnapshotLine line, List<ClassificationSpan> result)
        {
            string text = line.GetText();
            int lastIndex = 0;

            while (true)
            {
                Match match = this.expression.Match(text, lastIndex);
                if (!match.Success) break;

                Group span = match.Groups["span"];
                result.Add(new ClassificationSpan(
                    new SnapshotSpan(line.Snapshot, line.Start + span.Index, span.Length),
                    this.type));
                lastIndex = span.Index + span.Length;
            }
        }
    }

    public class Classifier : IClassifier
    {
        private IClassificationTypeRegistryService registry;

        private static ClassifierRegex[] expressions;

        internal Classifier(IClassificationTypeRegistryService registry)
        {
            this.registry = registry;

            expressions = new ClassifierRegex[] {
                // Type definitions and names.
                new ClassifierRegex(new Regex(
                    "(model|class)\\s(?<span>[a-z0-9_]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    this.registry.GetClassificationType("box.cyan")),
                new ClassifierRegex(new Regex(
                    "(foreign\\ key|primary\\ key|new)\\s(?<span>[a-z0-9_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    this.registry.GetClassificationType("box.cyan")),
                new ClassifierRegex(new Regex(
                    "(?<span>[a-z0-9_]+)\\s[a-z0-9_]+\\s=", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    this.registry.GetClassificationType("box.cyan")),
                new ClassifierRegex(new Regex(
                    "\\[.+\\s(in|as)\\s(?<span>.+)\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    this.registry.GetClassificationType("box.cyan")),
                // Keywords classifier.
                new ClassifierRegex(new Regex(
                    "(\\s|^)(?<span>" +
                        "namespace|" +
                        "public|private|protected|internal|" +
                        "static|" +
                        "new|return|get|set|in|as|" +
                        "this|if|then|break|" +
                        "for|foreach|while|" +
                        "true|false|string|int|" +
                        "model|many\\ to\\ many|" +
                        "foreign\\ key|primary\\ key)" +
                    "(\\s|$|;|\\])", RegexOptions.Compiled), 
                    this.registry.GetClassificationType("box.blue")),
                // Strings.
                new ClassifierRegex(new Regex(
                    "(?<span>\\\"[^\"]*?\\\")", RegexOptions.Compiled),
                    this.registry.GetClassificationType("box.red")),
                // Generic comments.
                new ClassifierRegex(new Regex(
                    "(?<span>\\/\\/.+)", RegexOptions.Compiled),
                    this.registry.GetClassificationType("box.green")),
                // Autodoc comments.
                new ClassifierRegex(new Regex(
                    "(?<span>\\/\\/\\/ \\<.*)", RegexOptions.Compiled),
                    this.registry.GetClassificationType("box.gray")),
                new ClassifierRegex(new Regex(
                    "(?<span>\\/\\/\\/)", RegexOptions.Compiled),
                    this.registry.GetClassificationType("box.gray")),
            };
        }

        #pragma warning disable 67
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        #pragma warning restore 67

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
                foreach (ClassifierRegex expression in expressions)
                {
                    expression.ProcessLine(line, result);
                }

                if (line.LineNumber == endLine.LineNumber) break;
                if (snapshot.Length <= line.EndIncludingLineBreak.Position + 1) break;

                line = snapshot.GetLineFromPosition(line.EndIncludingLineBreak + 1);
            }

            return result;
        }
    }
}
