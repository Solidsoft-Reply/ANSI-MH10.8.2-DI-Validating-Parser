// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityDescriptor.cs" company="Solidsoft Reply Ltd.">
//   (c) 2018-2023 Solidsoft Reply Ltd. All rights reserved.
// </copyright>
// <license>
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
// </license>
// <summary>
// A descriptor for a ASC MH10.8.2 entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Reply.Parsers.AnsiMhDi.Descriptors;

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Properties;

using Common;

/// <summary>
///   A descriptor for a ASC MH10.8.2 entity.
/// </summary>

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EntityDescriptor
{
    /// <summary>
    ///   The regular expression pattern of the entity.
    /// </summary>
    private readonly Func<Regex> _pattern;

    /// <summary>
    ///   The regular expression pattern of the entity.
    /// </summary>
    private Regex? _validator;

    /////////// <summary>
    ///////////   Initializes a new instance of the <see cref="EntityDescriptor" /> class.
    /////////// </summary>
    /////////// <param name="dataTitle">
    ///////////   The data title.
    /////////// </param>
    /////////// <param name="description">
    ///////////   The description.
    /////////// </param>
    /////////// <param name="pattern">
    ///////////   The pattern.
    /////////// </param>
    ////////public EntityDescriptor(string? dataTitle, string? description, string? pattern)
    ////////{
    ////////    DataTitle = dataTitle;
    ////////    Description = description;
    ////////    _pattern = pattern;
    ////////    _validator = null;
    ////////}

    /// <summary>
    ///   Initializes a new instance of the <see cref="EntityDescriptor" /> class.
    /// </summary>
    /// <param name="dataTitle">
    ///   The data title.
    /// </param>
    /// <param name="description">
    ///   The description.
    /// </param>
    /// <param name="pattern">
    ///   The pattern.
    /// </param>
    public EntityDescriptor(string? dataTitle, string? description, Func<Regex> pattern) {
        DataTitle = dataTitle;
        Description = description;
        _pattern = pattern;
        _validator = null;
    }

    /// <summary>
    ///   Gets the data title of the entity.
    /// </summary>

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? DataTitle { get; }

    /// <summary>
    ///   Gets the description of the entity.
    /// </summary>

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Description { get; }

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
    /// <returns>True, if valid. Otherwise false.</returns>
    public virtual bool IsValid(string? value, out IList<ParserException> validationErrors)
    {
        validationErrors = new List<ParserException>();

        var result = Pattern.IsMatch(value ?? string.Empty);

        if (result)
        {
            return true;
        }

        var valueString = (value ?? string.Empty).Length > 0 ? " " + value : string.Empty;
        validationErrors.Add(
            new ParserException(
                3100,
                string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_011, valueString),
                false));
        return false;
    }
}