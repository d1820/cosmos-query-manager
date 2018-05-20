using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace CosmosQueryEditor.Resources
{
    public class Loader
    {
        public IHighlightingDefinition GetDocumentDbSyntaxHighlighting()
        {
            using (var stream = Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream(GetType(), "DocumentDb.xshd"))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}