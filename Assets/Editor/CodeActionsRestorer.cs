using System.Linq;
using System.Xml.Linq;
using UnityEditor;

// This is for fixing VSCode not finding references when CTRL+. (this problem appeared in Unity version 2022.2.10)
// Remove the .sln and .csproj files, then regenerate all (Edit>Preferences>External Tools)
public class CodeActionsRestorer : AssetPostprocessor
{
    private static string OnGeneratedCSProject(string path, string content)
    {
        var document = XDocument.Parse(content);
        document.Root.Descendants()
            .Where(x => x.Name.LocalName == "Analyzer")
            .Where(x => x.Attribute("Include").Value.Contains("Unity.SourceGenerators"))
            .Remove();
        return document.Declaration + System.Environment.NewLine + document.Root;
    }
}