using System.IO;

namespace AWTY.Http.IntegrationTests
{
    /// <summary>
    ///     A non-optimised version of <see cref="MemoryStream"/> for use in tests.
    /// </summary>
    /// <remarks>
    ///     <see cref="MemoryStream"/>'s CopyToAsync performs a bunch of optimisations unless subclassed.
    ///     https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/memorystream.cs#L450
    /// 
    ///     Unfortunately, these optimisations break some of our tests.
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