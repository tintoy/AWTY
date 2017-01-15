using AWTY;
using System;
using System.Reactive.Disposables;

namespace ConsoleDownloader
{
    /// <summary>
    ///     A single-line progress bar displayed on the console.
    /// </summary>
    class ConsoleProgressBar
        : IObserver<ProgressData<long>>
    {
        /// <summary>
        ///     Maximum number of characters in a progress title.
        /// </summary>
        const int MaxTitleChars = 10;

        /// <summary>
        ///     The starting column of the bar.
        /// </summary>
        const int BarStart = MaxTitleChars + 2; // = ": "

        /// <summary>
        ///     The size, in characters, of the bar.
        /// </summary>
        const int BarSize = 50;

        /// <summary>
        ///     The starting column of the bar.
        /// </summary>
        const int BarEnd = BarStart + BarSize;

        /// <summary>
        ///     The starting column of the percentage displayed on the end of the bar.
        /// </summary>
        const int PercentageStart = BarEnd + 2; // = "] "

        /// <summary>
        ///     The ending column of the percentage displayed on the end of the bar.
        /// </summary>
        const int PercentageEnd = PercentageStart + 6; // = "] 100%"

        /// <summary>
        ///     The progress bar title.
        /// </summary>
        readonly string _title;

        /// <summary>
        ///     The line on which the progress bar is displayed.
        /// </summary>
        readonly int _line;

        /// <summary>
        ///     Create a new console progress bar on the current line.
        /// </summary>
        /// <param name="title">
        ///     The progress bar title.
        /// </param>
        public ConsoleProgressBar(string title)
            : this(title, Console.CursorTop)
        {
        }

        /// <summary>
        ///     Create a new progress bar.
        /// </summary>
        /// <param name="title">
        ///     The progress bar title.
        /// </param>
        /// <param name="line">
        ///     The line on which the progress bar is displayed.
        /// </param>
        public ConsoleProgressBar(string title, int line)
        {
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Must supply a valid progress bar title.", nameof(title));

            if (line < 0)
                throw new ArgumentOutOfRangeException(nameof(line), line, "Line cannot be less than 0.");

            _title = title;
            if (_title.Length > MaxTitleChars)
                _title = _title.Substring(0, MaxTitleChars);

            _line = line;

            using (Line(_line))
            {
                Console.WriteLine("{0}: [{1}]",
                    _title.PadRight(MaxTitleChars),
                    new String(' ', BarSize)
                );
            }
        }

        /// <summary>
        ///     Called when the next progress value is available.
        /// </summary>
        /// <param name="value">
        ///     The progress value.
        /// </param>
        void IObserver<ProgressData<long>>.OnNext(ProgressData<long> value)
        {
            using (Position(_line, MaxTitleChars + 3))
            {
                Console.Write(new String('#', value.PercentComplete / 2));
            }
            using (Position(_line, MaxTitleChars + 56))
            {
                Console.Write("{0}%", value.PercentComplete);
            }
        }

        /// <summary>
        ///     Called when the progress sequence is completed.
        /// </summary>
        void IObserver<ProgressData<long>>.OnCompleted()
        {
            using (Position(_line, BarStart))
            {
                Console.Write("Complete.".PadRight(PercentageEnd - BarStart));
            }
        }

        /// <summary>
        ///     Called when the progress source encounters an error.
        /// </summary>
        /// <param name="error">
        ///     An <see cref="Exception"/> representing the error.
        /// </param>
        void IObserver<ProgressData<long>>.OnError(Exception error)
        {
            using (Position(_line, 0))
            using (Color(ConsoleColor.Red))
            {
                Console.SetCursorPosition(left: 0, top: _line);
                Console.Write(error.Message);
            }
        }

        /// <summary>
        ///     Move to the specified line in the console.
        /// </summary>
        /// <param name="line">
        ///     The (0-based) target line number.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> that returns to the original position when disposed.
        /// </returns>
        static IDisposable Line(int line)
        {
            return Position(line, column: 0);
        }

        /// <summary>
        ///     Move to the specified position in the console.
        /// </summary>
        /// <param name="line">
        ///     The (0-based) target line number.
        /// </param>
        /// <param name="column">
        ///     The (0-based) target column number.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> that returns to the original position when disposed.
        /// </returns>
        static IDisposable Position(int line, int column)
        {
            int previousLine = Console.CursorTop;
            int previousColumn = Console.CursorLeft;

            if (line == previousLine && column == previousColumn)
                return Disposable.Empty;

            Console.SetCursorPosition(left: column, top: line);

            return Disposable.Create(() =>
            {
                Console.SetCursorPosition(left: previousColumn, top: previousLine);
            });
        }

        /// <summary>
        ///     Switch the console foreground to the specified colour.
        /// </summary>
        /// <param name="color">
        ///     The target foreground colour.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> that returns to the original foreground colour when disposed.
        /// </returns>
        static IDisposable Color(ConsoleColor color)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            return Disposable.Create(
                () => Console.ForegroundColor = previousColor
            );
        }
    }
}