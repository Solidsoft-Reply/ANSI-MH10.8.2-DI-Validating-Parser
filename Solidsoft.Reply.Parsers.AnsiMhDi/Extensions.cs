// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Solidsoft Reply Ltd">
// Copyright (c) 2018-2024 Solidsoft Reply Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <summary>
// Extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using System;
using System.Globalization;

/// <summary>
///     Extension methods.
/// </summary>
public static class Extensions {
    /// <summary>
    ///     Converts the value of this instance to its equivalent string representation using culture-invariant format
    ///     information.
    /// </summary>
    /// <param name="thisCharacter">The character to be converted.</param>
    /// <returns>A culture-invariant string.</returns>
    public static string ToInvariantString(this char thisCharacter) {
        return thisCharacter.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Returns a copy of the string converted to uppercase, using culture-neutral casing rules.
    /// </summary>
    /// <param name="thisString">The string to be converted.</param>
    /// <returns>A string converted to uppercase.</returns>
    public static string ToInvariantUpper(this string thisString) {
        return thisString.ToUpper(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Writes the invariant uppercase form of the source span into the destination buffer without allocations.
    /// </summary>
    /// <param name="source">The read-only character span to convert.</param>
    /// <param name="destination">The buffer that receives the uppercase characters.</param>
    /// <returns>True if the conversion succeeded; otherwise, false when destination is too small.</returns>
    public static bool TryToInvariantUpper(this ReadOnlySpan<char> source, Span<char> destination) {
        if (destination.Length < source.Length) {
            return false;
        }

        for (int i = 0; i < source.Length; i++) {
            destination[i] = char.ToUpper(source[i], CultureInfo.InvariantCulture);
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified character span is null, empty, or consists only of white-space or null
    /// characters.
    /// </summary>
    /// <remarks>This method treats null characters ('\0') as white-space for the purpose of evaluation. It is
    /// intended for use with spans that may contain embedded nulls or be empty.</remarks>
    /// <param name="span">The read-only character span to evaluate for null, empty, or white-space content.</param>
    /// <returns>true if the span is empty or contains only white-space or null characters; otherwise, false.</returns>
    internal static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> span) {
        foreach (var c in span) {
            if (c != '\0' && !char.IsWhiteSpace(c)) {
                return false;
            }
        }

        return true;
    }
}