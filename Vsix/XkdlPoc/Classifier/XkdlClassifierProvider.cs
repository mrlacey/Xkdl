using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace XkdlPoc.Classifier;

[Export(typeof(IClassifierProvider))]
[ContentType(Xkdl.ContentType)]
internal class XkdlClassifierProvider : IClassifierProvider
{
	[Import]
	private IClassificationTypeRegistryService ClassificationRegistry { get; set; }

	public IClassifier GetClassifier(ITextBuffer buffer) =>
		buffer.Properties.GetOrCreateSingletonProperty(() =>
			new XkdlClassifier(this.ClassificationRegistry));
}
