# AWTY

Are we there yet? Add progress reporting to just about anything.

This is a work-in-progress (the API is fairly unstable at the moment); please check back later, or feel free to [get in touch](https://github.com/tintoy/AWTY/issues/new) if you're interested in contributing :)

## Strategies

AWTY encapsulates the logic for _when_ to report progress into [progress strategies](src/AWTY.Core/Core/Strategies/ProgressStrategy.cs).
For example, [Int32ChunkedPercentageStrategy](src/AWTY.Core/Core/Strategies/Int32ChunkedPercentageStrategy.cs) only reports progress when the percentage of completion has changed by more than the specified value.

### Examples

#### Progress for reading from a FileStream

To only report progress for every change of 5 percent or more when data is read from a `FileStream`:

```csharp
using (ProgressStream stream = new FileStream("foo.txt").WithReadProgress(progressObserver))
{
    stream.Progress.Percentage(minChange: 5).Subscribe(progress =>
    {
        Console.WriteLine("Progress: {0}%", progress.PercentComplete);
    });

    // Read from stream.
}
```

#### Progress for an HttpClient

To only report progress for every change of 5 percent or more when the content is read from an HttpClient response:

```csharp
ProgressStrategy<long> progressObserver = ProgressStrategy.Percentage.Chunked.Int64(5);
progressObserver.Subscribe(progress =>
{
    Console.WriteLine("Progress: {0}%", progress.PercentComplete);
});

HttpClient client = new HttpClient();
using (HttpResponseMessage response = await client.GetAsync("http://www.microsoft.com/", HttpCompletionOption.ResponseHeadersRead).WithProgress(progressObserver))
{
    Console.WriteLine(
        await response.Content.ReadAsStringAsync()
    );
}
```
