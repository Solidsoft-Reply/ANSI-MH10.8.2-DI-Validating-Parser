// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityDescriptors.cs" company="Solidsoft Reply Ltd">
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
// A descriptor for an ASC MH10.8.2 entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Reply.Parsers.AnsiMhDi.Descriptors;

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Properties;

using Common;

/// <summary>
///   A descriptor for an ASC MH10.8.2 entity.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="EntityDescriptors" /> class.
/// </remarks>
/// <param name="dataTitle">
///   The data title.
/// </param>
/// <param name="description">
///   The description.
/// </param>
/// <param name="pattern">
///   The pattern.
/// </param>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EntityDescriptors(string? dataTitle, string? description, Func<Regex> pattern) {
    /// <summary>
    ///   The regular expression pattern of the entity.
    /// </summary>
    private readonly Func<Regex> _pattern = pattern;

    /// <summary>
    ///   The regular expression pattern of the entity.
    /// </summary>
    private Regex? _validator = null;

    /// <summary>
    ///   Gets the data title of the entity.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? DataTitle { get; } = dataTitle;

    /// <summary>
    ///   Gets the description of the entity.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Description { get; } = description;

    /// <summary>
    ///   Gets the compiled regular expression object for validating the entity.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public Regex Pattern => _validator ??= _pattern();

    /// <summary>
    ///   Validate data against the descriptor.
    /// </summary>
    /// <param name="value">The data to be validated.</param>
    /// <param name="validationErrors">A list of validation errors.</param>
    /// <returns>True, if valid. Otherwise, false.</returns>
    public virtual bool IsValid(string? value, out IList<ParserException>? validationErrors) {
        validationErrors = null;
        var result = Pattern.IsMatch(value ?? string.Empty);

        if (result) {
            return true;
        }

        var valueString = (value ?? string.Empty).Length > 0 ? " " + value : string.Empty;
        validationErrors = [];

        validationErrors.Add(
            new ParserException(
                3100,
                string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_011, valueString),
                false));
        return false;
    }
}