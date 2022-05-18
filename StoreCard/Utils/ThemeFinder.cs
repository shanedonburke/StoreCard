// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreCard.Utils;

internal class ThemeFinder
{
    private static readonly Regex s_themePathRegex =
        new(@"^[a-zA-Z]:\\(((?![<>:""/\\|?*]).)+((?<![ .])\\)?)*\\(?<themeName>.+)\.xaml$");

    public static List<string> FindThemes()
    {
        string[] files = Directory.GetFiles(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            @"ResourceDictionaries\Themes"));

        var themeNames =
            (from file in files
                select s_themePathRegex.Match(file)
                into match
                where match.Success
                select match.Groups["themeName"].Value).ToList();

        return themeNames;
    }
}
