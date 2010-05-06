using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Definitif.VisualStudio.Classifier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("box")]
    internal class ClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        static Classifier boxClassifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (boxClassifier == null)
                boxClassifier = new Classifier(ClassificationRegistry);

            return boxClassifier;
        }
    }
}
