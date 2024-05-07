// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedDataIdentifier.cs" company="Solidsoft Reply Ltd">
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
// Represents a resolved ASC (MH10.8) application identifier and its associated data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using System.Collections.Generic;
using Common;

/// <summary>
///   Represents a resolved ASC (MH10.8) application identifier and its associated data.
/// </summary>
public class ResolvedDataIdentifier : IResolvedEntity {
    /// <summary>
    ///   A list of resolver exceptions.
    /// </summary>
    private readonly List<ParserException> _exceptions = [];

    /// <summary>
    ///   Initializes a new instance of the <see cref="ResolvedDataIdentifier" /> class.
    /// </summary>
    /// <param name="entity">
    ///   The application identifier entity.
    /// </param>
    /// <param name="identifier">
    ///   The identifier, as represented in the barcode.
    /// </param>
    /// <param name="inverseExponent">
    ///   The implied decimal point position in the value.
    /// </param>
    /// <param name="value">
    ///   The value associated with the application identifier.
    /// </param>
    /// <param name="dataTitle">
    ///   The application identifier data title.
    /// </param>
    /// <param name="description">
    ///   The description of the application identifier.
    /// </param>
    /// <param name="currentPosition">
    ///   The position of the application identifier for the current field.
    /// </param>
    public ResolvedDataIdentifier(
        int entity,
        string identifier,
        int inverseExponent,
        string value,
        string? dataTitle,
        string? description,
        int currentPosition) {
        Entity = entity;
        Identifier = identifier;
        InverseExponent = inverseExponent;
        Value = value;
        DataTitle = dataTitle ?? string.Empty;
        Description = description ?? string.Empty;
        CharacterPosition = currentPosition;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ResolvedDataIdentifier" /> class.
    /// </summary>
    /// <param name="exception">A resolver exception.</param>
    /// <param name="currentPosition">
    ///   The current character position at which parsing has occurred.
    /// </param>
    /// <param name="value">
    ///   The value of the unrecognised ASC MH10.8.2 data identifier.
    /// </param>
    public ResolvedDataIdentifier(ParserException exception, int currentPosition, string? value = null) {
        Entity = -1;
        Identifier = string.Empty;
        InverseExponent = -1;
        Value = value ?? string.Empty;
        DataTitle = string.Empty;
        Description = string.Empty;
        CharacterPosition = currentPosition;
        AddException(exception);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ResolvedDataIdentifier" /> class.
    /// </summary>
    /// <param name="exception">A resolver exception.</param>
    /// <param name="currentPosition">
    ///   The current character position at which parsing has occurred.
    /// </param>
    /// <param name="di">
    ///   The ASC MH10.8.2 data identifier.
    /// </param>
    public ResolvedDataIdentifier(ParserException exception, int currentPosition, IResolvedEntity? di) {
        Entity = di?.Entity ?? -1;
        Identifier = di?.Identifier ?? string.Empty;
        InverseExponent = di?.InverseExponent ?? -1;
        Value = di?.Value ?? string.Empty;
        DataTitle = di?.DataTitle ?? string.Empty;
        Description = di?.Description ?? string.Empty;
        CharacterPosition = currentPosition;

        if (di != null) {
            foreach (var e in di.Exceptions) {
                AddException(e);
            }
        }

        AddException(exception);
    }

    /// <summary>
    ///   Gets the character position where the error occurred.
    /// </summary>
    public int CharacterPosition { get; }

    /// <summary>
    ///   Gets the application identifier data title.
    /// </summary>
    public string DataTitle { get; }

    /// <summary>
    ///   Gets the description of the application identifier.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///   Gets the data identifier entity.
    /// </summary>
    public int Entity { get; }

    /// <summary>
    ///   Gets the exceptions raised during attempted entity resolution.
    /// </summary>
    public IEnumerable<ParserException> Exceptions => _exceptions;

    /// <summary>
    ///   Gets the identifier, as represented in the barcode.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    ///   Gets the implied decimal point position in the value.
    /// </summary>
    public int? InverseExponent { get; }

    /// <summary>
    ///   Gets a value indicating whether resolution resulted in an error.
    /// </summary>
    public bool IsError => _exceptions.Count > 0;

    /// <summary>
    ///   Gets a value indicating whether an error is fatal (further parsing was aborted).
    /// </summary>
    public bool IsFatal {
        get {
            return _exceptions.Exists(exception => exception.IsFatal);
        }
    }

    /// <summary>
    ///   Gets the value associated with the application identifier.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///   Adds a resolver exception.
    /// </summary>
    /// <param name="parserException">The resolver exception to be added.</param>
    public void AddException(ParserException? parserException) {
        if (parserException is not null) {
            _exceptions.Add(parserException);
        }
    }
}