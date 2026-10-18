using Microsoft.Azure.Cosmos;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// Builds a single-page <see cref="FeedIterator{T}"/> over an in-memory list using Moq,
    /// so the exec pipeline (which now consumes FeedIterator) can be driven headlessly.
    /// </summary>
    public static class FeedIteratorFactory
    {
        public static FeedIterator<T> Create<T>(IReadOnlyList<T> items)
        {
            var response = new Mock<FeedResponse<T>>();
            response.Setup(r => r.Count).Returns(items.Count);
            response.Setup(r => r.RequestCharge).Returns(1.0);
            response.Setup(r => r.ActivityId).Returns("fake-activity-id");
            response.Setup(r => r.GetEnumerator()).Returns(() => items.GetEnumerator());

            var iterator = new Mock<FeedIterator<T>>();
            var read = false;
            iterator.Setup(i => i.HasMoreResults).Returns(() => !read);
            iterator.Setup(i => i.ReadNextAsync(It.IsAny<CancellationToken>()))
                    .Returns(() =>
                    {
                        read = true;
                        return Task.FromResult(response.Object);
                    });
            return iterator.Object;
        }
    }
}
