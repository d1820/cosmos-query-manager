using CosmosQueryEditor.Resources;
using CosmosQueryEditor.Settings;
using ICSharpCode.AvalonEdit.Highlighting;
using StructureMap;

namespace CosmosQueryEditor
{
    public class CosmosQueryEditorRegistry : Registry
    {
        public CosmosQueryEditorRegistry()
        {
            ForSingletonOf<IHighlightingDefinition>().Use(new Loader().GetDocumentDbSyntaxHighlighting());
            ForSingletonOf<IManualSaver>().Use<ManualSaver>();
        }
    }
}