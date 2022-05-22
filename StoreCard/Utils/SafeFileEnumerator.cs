using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StoreCard.Utils;

// From https://stackoverflow.com/a/9758932
public class SafeFileEnumerator : IEnumerable<FileSystemInfo>
{
    /// <summary>
    /// Starting directory to search from
    /// </summary>
    private readonly DirectoryInfo _root;

    /// <summary>
    /// Filter pattern
    /// </summary>
    private readonly string _pattern;

    /// <summary>
    /// Indicator if search is recursive or not
    /// </summary>
    private readonly SearchOption _searchOption;

    /// <summary>
    /// Any errors captured
    /// </summary>
    private readonly IList<Exception> _errors;

    /// <summary>
    /// Create an Enumerator that will scan the file system, skipping directories where access is denied
    /// </summary>
    /// <param name="root">Starting Directory</param>
    /// <param name="pattern">Filter pattern</param>
    /// <param name="option">Recursive or not</param>
    public SafeFileEnumerator(string root, string pattern, SearchOption option)
        : this(new DirectoryInfo(root), pattern, option)
    {
    }

    /// <summary>
    /// Create an Enumerator that will scan the file system, skipping directories where access is denied
    /// </summary>
    /// <param name="root">Starting Directory</param>
    /// <param name="pattern">Filter pattern</param>
    /// <param name="option">Recursive or not</param>
    public SafeFileEnumerator(DirectoryInfo root, string pattern, SearchOption option)
        : this(root, pattern, option, new List<Exception>())
    {
    }

    // Internal constructor for recursive itterator
    private SafeFileEnumerator(DirectoryInfo root, string pattern, SearchOption option, IList<Exception> errors)
    {
        if (root is not {Exists: true})
        {
            throw new ArgumentException(@"Root directory is not set or does not exist.", nameof(root));
        }

        _root = root;
        _searchOption = option;
        _pattern = string.IsNullOrEmpty(pattern)
            ? "*"
            : pattern;
        _errors = errors;
    }

    /// <summary>
    /// Errors captured while parsing the file system.
    /// </summary>
    public Exception[] Errors
    {
        get => _errors.ToArray();
    }

    /// <summary>
    /// Helper class to enumerate the file system.
    /// </summary>
    private class Enumerator : IEnumerator<FileSystemInfo>
    {
        // Core enumerator that we will be walking though
        private IEnumerator<FileSystemInfo>? _fileEnumerator;

        // Directory enumerator to capture access errors
        private IEnumerator<DirectoryInfo>? _directoryEnumerator;

        private readonly DirectoryInfo _root;
        private readonly string _pattern;
        private readonly SearchOption _searchOption;
        private readonly IList<Exception> _errors;

        public Enumerator(DirectoryInfo root, string pattern, SearchOption option, IList<Exception> errors)
        {
            _root = root;
            _pattern = pattern;
            _errors = errors;
            _searchOption = option;

            Reset();
        }

        /// <summary>
        /// Current item the primary iterator is pointing to
        /// </summary>
        public FileSystemInfo Current
        {
            get
            {
                if (_fileEnumerator == null)
                {
                    throw new ObjectDisposedException("FileEnumerator");
                }
                return _fileEnumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public void Dispose()
        {
            Dispose(true, true);
        }

        private void Dispose(bool file, bool dir)
        {
            if (file)
            {
                _fileEnumerator?.Dispose();
                _fileEnumerator = null;
            }

            if (!dir)
            {
                return;
            }

            _directoryEnumerator?.Dispose();

            _directoryEnumerator = null;
        }

        public bool MoveNext()
        {
            // Enumerate the files in the current folder
            if (_fileEnumerator != null && _fileEnumerator.MoveNext())
                return true;

            // Don't go recursive...
            if (_searchOption == SearchOption.TopDirectoryOnly)
            {
                return false;
            }

            while (_directoryEnumerator != null && _directoryEnumerator.MoveNext())
            {
                Dispose(true, false);

                try
                {
                    _fileEnumerator = new SafeFileEnumerator(
                        _directoryEnumerator.Current,
                        _pattern,
                        SearchOption.AllDirectories,
                        _errors
                    ).GetEnumerator();
                }
                catch (Exception ex)
                {
                    _errors.Add(ex);
                    continue;
                }

                // Open up the current folder file enumerator
                if (_fileEnumerator.MoveNext())
                    return true;
            }

            Dispose(true, true);

            return false;
        }

        public void Reset()
        {
            Dispose(true, true);

            // Safely get the enumerators, including in the case where the root is not accessable
            try
            {
                _fileEnumerator = _root
                    .GetFileSystemInfos(_pattern, SearchOption.TopDirectoryOnly)
                    .AsEnumerable()
                    .GetEnumerator();
            }
            catch (Exception ex)
            {
                _errors.Add(ex);
                _fileEnumerator = null;
            }

            try
            {
                _directoryEnumerator = _root
                    .GetDirectories(_pattern, SearchOption.TopDirectoryOnly)
                    .AsEnumerable()
                    .GetEnumerator();
            }
            catch (Exception ex)
            {
                _errors.Add(ex);
                _directoryEnumerator = null;
            }
        }
    }

    public IEnumerator<FileSystemInfo> GetEnumerator()
    {
        return new Enumerator(_root, _pattern, _searchOption, _errors);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
