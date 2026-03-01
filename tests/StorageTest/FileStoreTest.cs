using VoidNone.Storage;

namespace VoidNone.StorageTest;

[TestClass]
public class FileStoreTest
{
    [TestMethod]
    public void WriteLog()
    {
        var store = new FileStore("testStore");
        var path = store.CreateDirectory("testDirectory");
        Assert.IsTrue(Directory.Exists(path));
    }
}
