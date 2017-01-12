# AWTY

Are we there yet? Add progress reporting to just about anything.

This is a work-in-progress (the API is fairly unstable at the moment); please check back later, or feel free to [get in touch](https://github.com/tintoy/AWTY/issues/new) if you're interested in contributing :)

## Strategies

AWTY encapsulates the logic for _when_ to report progress into [progress strategies](src/AWTY.Core/Core/Strategies/ProgressStrategy.cs).
For example, [Int32ChunkedPercentageStrategy](src/AWTY.Core/Core/Strategies/Int32ChunkedPercentageStrategy.cs) only reports progress when the percentage of completion has changed by more than the specified value.

### Examples

#### Progress for reading from a FileStream

To report progress for every change of 5 percent or more when data is read from a `FileStream`:

```csharp
using (ProgressStream stream = new FileStream("foo.txt").WithReadProgress())
{
    stream.Progress.Percentage(minChange: 5).Subscribe(progress =>
    {
        Console.WriteLine("Progress: {0}%", progress.PercentComplete);
    });

    // Read from stream.
}
```

#### Progress for an HttpClient (specific response)

To report progress for every change of 5 percent or more when the content is read from an `HttpClient` response:

```csharp
// ProgressStrategy can also be created using the .Percentage() Rx LINQ operator (see examples above and below).
ProgressStrategy<long> progressObserver = ProgressStrategy.Percentage.Chunked.Int64(5);
progressObserver.Subscribe(progress =>
{
    Console.WriteLine("Progress: {0}%", progress.PercentComplete);
});

// Only works if we get a Content-Length header.
// Use HttpCompletionOption.ResponseHeadersRead to request that the response be returned before the entire message has been received.

HttpClient client = new HttpClient();
using (HttpResponseMessage response = await client.GetAsync("http://www.microsoft.com/", HttpCompletionOption.ResponseHeadersRead).WithProgress(progressObserver))
{
    Console.WriteLine(
        await response.Content.ReadAsStringAsync()
    );
}
```

#### Progress for an HttpClient (all responses)

To report progress for every change of 5 percent or more when a response is received by an `HttpClient`:

```csharp
HttpClientHandler clientHandler = new HttpClientHandler();
ProgressHandler progressHandler = new ProgressHandler(clientHandler, HttpProgressTypes.Response);

// Notify each time we start receiving an HTTP response.
progressHandler.ResponseStarted.Subscribe(responseStarted =>
{
    Output.WriteLine("Started receiving {0} response for '{1}'...",
        responseStarted.RequestMethod,
        responseStarted.RequestUri
    );

    // Report progress for this response.
    responseStarted.Progress.Percentage(5).Subscribe(progress =>
    {
        actualPercentages.Add(progress.PercentComplete);

        Output.WriteLine("{0} '{1}' ({2}% complete)...",
            responseStarted.RequestMethod,
            responseStarted.RequestUri,
            progress.PercentComplete
        );
    });
});

HttpClient client = new HttpClient(progressHandler);

// Only works if we get a Content-Length header.
// Use HttpCompletionOption.ResponseHeadersRead to request that the response be returned before the entire message has been received.

Uri requestUri = new Uri("https://raw.githubusercontent.com/tintoy/AWTY/development/v1.0/src/AWTY.Http/Http/ProgressHandler.cs");
using (HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
{
    // If you see progress simply jump to 100%, consider using ReadAsStreamAsync instead, as this makes it more likely that content won't be buffered in a single pass.
    await response.Content.ReadAsStringAsync();
}

requestUri = new Uri("https://raw.githubusercontent.com/tintoy/AWTY/development/v1.0/test/AWTY.Http.IntegrationTests/ProgressHandlerTests.cs");
using (HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
{
    // If you see progress simply jump to 100%, consider using ReadAsStreamAsync instead, as this makes it more likely that content won't be buffered in a single pass.
    await response.Content.ReadAsStringAsync();
}
```