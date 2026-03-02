using VoidNone.Storage;

namespace VoidNone.StorageTest;

[TestClass]
public class MimeMappingTest
{
    [TestMethod]
    public void TryGetContentType()
    {
        MimeMapping.TryGetContentType(".jpg", out var contentType);
        Assert.AreEqual("image/jpeg", contentType);
    }
}
