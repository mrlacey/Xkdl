using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace XkdlPoc.Classifier;

internal class XkdlClassifier : IClassifier
{
	public readonly IClassificationType classificationTypeName;
	public readonly IClassificationType classificationAttribute;
	public readonly IClassificationType classificationAttributeValue;
	public readonly IClassificationType classificationDefault;
	public readonly IClassificationType classificationPunctuation;
	public readonly IClassificationType classificationComment;
	public readonly IClassificationType classificationCSharp;

	public XkdlClassifier(IClassificationTypeRegistryService registry)
	{
		classificationTypeName = registry.GetClassificationType(PredefinedClassificationTypeNames.MarkupNode);
		classificationAttribute = registry.GetClassificationType(PredefinedClassificationTypeNames.MarkupAttribute);
		classificationAttributeValue = registry.GetClassificationType(PredefinedClassificationTypeNames.MarkupAttributeValue);
		classificationPunctuation = registry.GetClassificationType(PredefinedClassificationTypeNames.Punctuation);
		classificationComment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
		classificationCSharp = registry.GetClassificationType(PredefinedClassificationTypeNames.BidirectionalTextControlCharacter);
		classificationDefault = registry.GetClassificationType(PredefinedClassificationTypeNames.Other);
	}

	public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

	public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
	{
		var list = new List<ClassificationSpan>();

		try
		{
			var text = span.GetText();

			var whitespace = text.Substring(0, text.Length - text.TrimStart().Length);
			bool endsWithPunctuation = text.Trim().EndsWith("\\") || text.Trim().EndsWith("{") || text.Trim().EndsWith("}");

			// TODO: Also handle quotes

			//var rawTextParts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var textParts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			//var textPartsList = new List<string>();

			//var inQuote = false;

			//for (int i = 0; i < text.Length; i++)
			//{
			//	if (!inQuote)
			//	{
			//		if (text[i])
			//	}
			//	else
			//	{
			//		if (text[i])
			//		{ }
			//	}
			//}

			//var textParts = textPartsList.ToArray();

			if (textParts.Length >= 1)
			{
				list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start, whitespace.Length), classificationDefault));

				for (int i = 0; i < textParts.Length - (endsWithPunctuation ? 1 : 0); i++)
				{
					if (!textParts[i].Contains("="))
					{
						if (endsWithPunctuation || textParts.Length > 1)
						{
							list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]), textParts[i].Length), classificationTypeName));
						}
						else
						{
							list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]), textParts[i].Length), classificationAttribute));
						}
					}
					else
					{
						if (textParts[i].StartsWith("/-"))
						{
							list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]), textParts[i].Length), classificationComment));
						}
						else
						{
							// Manually split on the first equals so don't have to account for equals signs in the attribute (embedded C#)
							var equalsIndex = textParts[i].IndexOf("=");
							var attributePart1 = textParts[i].Substring(0, equalsIndex);
							var attributePart2 = textParts[i].Substring(equalsIndex + 1);

							list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]), attributePart1.Length), classificationAttribute));
							list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]) + attributePart1.Length, 1), classificationPunctuation));

							if (attributePart2.StartsWith("\"@") && attributePart2.EndsWith("@\""))
							{
								list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]) + attributePart1.Length + 1, 1), classificationPunctuation));
								list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]) + attributePart1.Length + 2, attributePart2.Length - 2), classificationCSharp));
								list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]) + attributePart1.Length + 2 + attributePart2.Length - 2, 1), classificationPunctuation));
							}
							else
							{
								list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + text.IndexOf(textParts[i]) + attributePart1.Length + 1, attributePart2.Length), classificationAttributeValue));
							}
						}
					}
				}
			}

			if (endsWithPunctuation)
			{
				list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, text.TrimEnd().Length - 1, 1), classificationPunctuation));
			}
		}
		catch (Exception exc)
		{
			Debug.WriteLine(exc);
			Debug.WriteLine(exc.Message);
			Debug.WriteLine(exc.StackTrace);

			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
		}

		return list;
	}
}
