using System.IO;

namespace AWTY.Http.IntegrationTests
{
    /// <summary>
    ///     A non-optimised version of <see cref="MemoryStream"/> for use in tests.
    /// </summary>
    /// <remarks>
    ///     <see cref="MemoryStream"/> performs a bunch of optimisations unless subclassed.
    /// 
    ///     Unfortunately, these optimisations break the tests.
    ///     Although you obviously wouldn't use a MemoryStream in real life, it's the simplest options for these tests.
    /// </remarks>
    public sealed class DumbMemoryStream
        : MemoryStream
    {
        public DumbMemoryStream()
            : base()
        {
        }
    }
}