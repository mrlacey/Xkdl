﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Kadlet;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace XkdlPoc.Generator;

public class XkdlToXamlGenerator : BaseCodeGeneratorWithSite
{
	public const string Name = nameof(XkdlToXamlGenerator);

	public const string Description = "Create XAML files from .xkdl docs";

	public override string GetDefaultExtension() => ".xaml";

	protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
	{
		try
		{

			var lines = inputFileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			//var tLines = Tokenizer.TokenizeLines(lines.ToList());
			//var cLines = Logic.Classifier.ClassifyLines(tLines);

			//var generated = "Generated XAML will go here"; // CSharpGenerator.GenerateOutput(cLines);


			//sb.AppendLine("/// </auto-generated>");
			//sb.AppendLine();
			//sb.AppendLine($"namespace {FileNamespace};");
			//sb.AppendLine($"input file name: {System.IO.Path.GetFileNameWithoutExtension(this.InputFilePath)};");
			//sb.AppendLine();
			//sb.AppendLine($"{generated}");

			var settings = GetSettings(inputFileName);


			var doc = new KdlReader().Parse(inputFileContent);


			var sbXaml = new StringBuilder();
			var sbCs = new StringBuilder();

			GenerateOutput(settings, doc, 0, sbXaml, sbCs);

			var csFileName = Path.ChangeExtension(inputFileName, ".xkdl.xaml.cs");

			var csString = sbCs.ToString();

			if (string.IsNullOrEmpty(csString))
			{
				if (File.Exists(csFileName))
				{
					File.Delete(csFileName);
				}
			}
			else
			{
				var partialCSharpFile = new StringBuilder();
				partialCSharpFile.AppendLine($"namespace {this.FileNamespace};");
				partialCSharpFile.AppendLine(string.Empty);
				partialCSharpFile.AppendLine($"partial class {Path.GetFileNameWithoutExtension(this.InputFilePath)}");
				partialCSharpFile.AppendLine($"{{");
				partialCSharpFile.AppendLine($"    {csString.Replace(Environment.NewLine, $"{Environment.NewLine}    ")}");
				partialCSharpFile.AppendLine($"}}");

				File.WriteAllText(csFileName, partialCSharpFile.ToString());
			}

			return Encoding.UTF8.GetBytes(sbXaml.ToString());

		}
		catch (Exception exc)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<! --");
			sb.AppendLine($"/// There was an error converting XKDL to XAML (v{Vsix.Version})");
			sb.AppendLine($"/// version {Vsix.Version}");
			sb.AppendLine(string.Empty);
			sb.AppendLine(exc.Message);
			sb.AppendLine(exc.StackTrace);
			sb.AppendLine("-->");

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
	}

	XkdlSettings GetSettings(string relativePath)
	{
		var result = new XkdlSettings();

		const string settingsFileName = "xkdl-settings.kdl";

		string settingsFileContents = null;

		var dir = Path.GetDirectoryName(relativePath);

		while (settingsFileContents is null && Directory.Exists(dir))
		{
			if (File.Exists(Path.Combine(dir, settingsFileName)))
			{
				settingsFileContents = File.ReadAllText(Path.Combine(dir, settingsFileName));
				break;
			}
			else
			{
				dir = Path.GetDirectoryName(dir);
			}
		}

		if (settingsFileContents is not null)
		{
			var doc = new KdlReader().Parse(settingsFileContents);

			if (doc.Nodes.Count == 1 && doc.Nodes.First().Identifier == "config" && doc.Nodes.First().Children is not null)
			{
				foreach (var setting in doc.Nodes.First()!.Children.Nodes)
				{
					switch (setting.Identifier)
					{
						case "autoGenerateClass":
							result.AutoGenerateClass = bool.Parse(setting.Arguments.First().ToKdlString());
							break;

						case "defaultMarkupExtension":
							result.DefaultMarkupExtension = setting.Arguments.FirstOrDefault().ToKdlString().Trim('"');
							break;

						case "namespaces":

							foreach (var prop in setting.Properties)
							{
								result.Namespaces.Add(prop.Key, prop.Value.ToKdlString());
							}

							break;

						case "defaultProperties":

							foreach (var prop in setting.Properties)
							{
								result.DefaultProperties.Add(prop.Key.Trim('"'), prop.Value.ToKdlString().Trim('"'));
							}
							break;

						default:
							break;
					}
				}
			}
		}

		return result;
	}

	void GenerateOutput(XkdlSettings settings, KdlDocument document, int hierarchy, StringBuilder sbXaml, StringBuilder sbCSharp)
	{
		if (hierarchy == 0)
		{
			sbXaml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
		}

		foreach (var node in document.Nodes)
		{
			var indent = new string(' ', hierarchy * 4);

			sbXaml.Append($"{indent}<{node.Identifier} ");

			if (node.Arguments.Count == 1 && settings.DefaultProperties.ContainsKey(node.Identifier))
			{
				var defaultProp = settings.DefaultProperties[node.Identifier];

				if (defaultProp is not null)
				{
					sbXaml.Append($"{defaultProp}={node.Arguments.First().ToKdlString()} ");
				}
			}

			if (hierarchy == 0 && !node.Properties.Any(p => p.Key == "x:Class"))
			{
				sbXaml.Append($"x:Class=\"{this.FileNamespace}.{Path.GetFileNameWithoutExtension(this.InputFilePath)}\" ");
			}

			foreach (var prop in node.Properties)
			{
				var pValue = prop.Value.ToKdlString().Trim('"');

				if (pValue.StartsWith("{") && pValue.EndsWith("}"))
				{
					if (!pValue.Contains(" "))
					{
						pValue = pValue.Replace("{", $"{{{settings.DefaultMarkupExtension} ");
					}
				}
				else if (pValue.StartsWith("@") && pValue.EndsWith("@"))
				{
					var methodName = $"Gen_On_{node.Identifier}_{prop.Key}";

					if (node.Properties.TryGetValue("Name", out var nameProp))
					{
						methodName = $"Gen_On_{nameProp.ToKdlString().Trim('"')}_{prop.Key}";
					}
					else if (node.Properties.TryGetValue("x:Name", out var xnameProp))
					{
						methodName = $"Gen_On_{xnameProp.ToKdlString().Trim('"')}_{prop.Key}";
					}

					sbCSharp.AppendLine($"private void {methodName}(object sender, EventArgs e)");
					sbCSharp.AppendLine("{");
					sbCSharp.AppendLine($"    {pValue.Trim('@')}");
					sbCSharp.AppendLine("}");

					pValue = methodName;
				}

				sbXaml.Append($"{prop.Key}=\"{pValue}\" ");
			}

			if (hierarchy == 0)
			{
				foreach (var xmlns in settings.Namespaces)
				{
					sbXaml.AppendLine();
					sbXaml.Append($"        {xmlns.Key}={xmlns.Value} ");
				}
			}

			if (node.Children is not null)
			{
				sbXaml.AppendLine(">");
				GenerateOutput(settings, node.Children, hierarchy + 1, sbXaml, sbCSharp);
				sbXaml.AppendLine($"{indent}</{node.Identifier}>");
			}
			else
			{
				sbXaml.AppendLine(" />");
			}
		}

	}
}