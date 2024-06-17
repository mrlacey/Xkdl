using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

internal static class Xkdl
{
	internal const string ContentType = nameof(Xkdl);

	internal const string FileExtension = ".xkdl";

	[Export]
	[Name(ContentType)]
	[BaseDefinition("code")]
#pragma warning disable SA1401 // Fields must be private
	internal static ContentTypeDefinition ContentTypeDefinition = null;

	[Export]
	[Name(ContentType + nameof(FileExtensionToContentTypeDefinition))]
	[ContentType(ContentType)]
	[FileExtension(FileExtension)]
	internal static FileExtensionToContentTypeDefinition FileExtensionToContentTypeDefinition = null;
#pragma warning restore SA1401 // Fields must be private
}
