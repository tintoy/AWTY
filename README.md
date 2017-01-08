# AWTY

Are we there yet? Add progress reporting to just about anything.

This is a work-in-progress (the API is pretty unstable at the moment); please check back later, or feel free to [get in touch](https://github.com/tintoy/AWTY/issues/new) if you're interested in contributing :)

## Strategies

AWTY encapsulates the logic for _when_ to report progress into [progress strategies](src/AWTY.Core/IProgressStrategy.cs).
For example, [Int32ChunkedPercentage](src/AWTY.Core/Core/Strategies/Int32ChunkedPercentage.cs) only reports progress when the percentage of completion has changed by more than the specified value.

So to only report progress for every change of 5 percent or more when data is read from a stream:

```csharp
IProgressStrategy<long> strategy = ProgressStrategy.Percentage.Chunked.Int64(5);
using (ProgressStream stream = new FileStream("foo.txt").WithReadProgress(strategy))
{
    stream.ProgressChanged += (sender, args) =>
    {
        Console.WriteLine("Progress: {0}%", args.PercentComplete);
    };

    // Read from stream.
}
```
