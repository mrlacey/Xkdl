using System.Collections.Generic;

namespace XkdlPoc.Generator;

public class XkdlSettings
{
	public bool AutoGenerateClass { get; set; } = false;
	public string DefaultMarkupExtension { get; set; }
	public Dictionary<string, string> DefaultProperties { get; set; } = [];
	public Dictionary<string, string> Namespaces { get; set; } = [];
}
