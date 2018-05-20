using CosmosQueryEditor.Resources;
using Shouldly;
using Xunit;

namespace CosmosQueryEditor.Tests.Resources
{
    public class LoaderFixture
    {
        [Fact]
        public void WillLoadDocumentDbHighlighting()
        {
            var documentDbSyntaxHighlighting = new Loader().GetDocumentDbSyntaxHighlighting();

            documentDbSyntaxHighlighting.ShouldNotBeNull();
        }
    }
}