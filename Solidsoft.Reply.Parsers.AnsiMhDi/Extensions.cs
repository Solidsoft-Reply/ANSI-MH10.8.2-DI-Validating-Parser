﻿// --------------------------------------------------------------------------------------------------------------------
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
}