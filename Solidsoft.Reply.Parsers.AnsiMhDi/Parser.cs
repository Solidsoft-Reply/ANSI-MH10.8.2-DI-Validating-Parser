// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Solidsoft Reply Ltd">
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
// Parser for ANSI MH10.8 data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: CLSCompliant(true)]

namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using Common;

using Properties;

using System;
using System.Globalization;
using System.Text.RegularExpressions;

#if NET7_0_OR_GREATER
/// <summary>
/// Delegate for processing element data elements with minimal heap allocations.
/// </summary>
/// <param name="resolvedElement">The element element to process.</param>
public delegate void ResolvedElementDelegate(scoped in ResolvedDataIdentifierRef resolvedElement);
#endif

/// <summary>
///   Barcode Parser for ANSI MH10.8 data.
/// </summary>
#if NET7_0_OR_GREATER
public static partial class Parser {
#else
public static class Parser {
#endif

#if !NET7_0_OR_GREATER
    /// <summary>
    ///   Code generator for regular expression that captures a data identifier (0..3 digits followed by a letter).
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static readonly Regex MatchDataIdentifierRegex = new (@"^\d{0,3}[a-zA-Z]", RegexOptions.IgnoreCase);
#endif

    /// <summary>
    ///   Parse ANSI MH10.8-encoded data.
    /// </summary>
    /// <param name="data">The data to be parsed.</param>
    /// <param name="processResolvedEntity">
    ///   The function for processing resolved entities.
    /// </param>
    /// <param name="initialPosition">
    ///   The initial character position.
    /// </param>
    /// <return>The pack identifier.</return>
    public static void Parse(string? data, Action<IResolvedEntity> processResolvedEntity, int initialPosition = 0) {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(processResolvedEntity);
#else
        if (processResolvedEntity is null) {
            throw new ArgumentNullException(nameof(processResolvedEntity));
        }
#endif

        // Is any data present?
        if (string.IsNullOrWhiteSpace(data)) {
            // Handle errors
            processResolvedEntity(
                new ResolvedDataIdentifier(
                    new ParserException(string.Empty, 3001, Resources.Ansi_Mh10_8_2_Error_003, true),
                    initialPosition));
            return;
        }

#if NET7_0_OR_GREATER
        DoParseRecords(data.AsSpan(), processResolvedEntity, null, initialPosition);
#else
        DoParseRecords(data.AsSpan(), processResolvedEntity, initialPosition);
#endif
    }

#if NET7_0_OR_GREATER
    /// <summary>
    ///     Parse ANSI MH10.8-encoded data..
    /// </summary>
    /// <param name="data">
    ///     The data to be parsed.
    /// </param>
    /// <param name="processResolvedEntity">
    ///     A delegate that is invoked to process each resolved entity.  Use this overload to minmise heap allocations for greatest performance.
    /// </param>
    /// <param name="initialPosition">
    ///     The initial character position.
    /// </param>
    /// <remarks>
    /// Use this method as an alternative to Parse() for the very highest performance scenarios.  By using the ResolvedEntityDelegate delegate,
    /// you can avoid unecessary heap allocations.
    /// </remarks>
    public static void ParseEx(ReadOnlySpan<char> data, ResolvedElementDelegate processResolvedEntity, int initialPosition = 0) {
        ArgumentNullException.ThrowIfNull(processResolvedEntity);

        // Is any data present?
        if (data.IsNullOrWhiteSpace()) {
            var entity = new ResolvedDataIdentifierRef(
                    new ParserException(string.Empty, 3001, Resources.Ansi_Mh10_8_2_Error_003, true),
                    initialPosition);

            // Handle errors
            processResolvedEntity(in entity);
            return;
        }

        DoParseRecords(data, null, processResolvedEntity, initialPosition);
    }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    ///     Parse ANSI MH10.8-encoded data..
    /// </summary>
    /// <param name="data">
    ///     The data to be parsed.
    /// </param>
    /// <param name="processResolvedEntity">
    ///     An action that is invoked to process each resolved entity.
    /// </param>
    /// <param name="initialPosition">
    ///     The initial character position.
    /// </param>
    public static void Parse(ReadOnlySpan<char> data, Action<IResolvedEntity> processResolvedEntity, int initialPosition = 0) {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(processResolvedEntity);
#else
        if (processResolvedEntity is null) {
            throw new ArgumentNullException(nameof(processResolvedEntity));
        }
#endif

        // Is any data present?
        if (data.IsNullOrWhiteSpace()) {
            // Handle errors
            processResolvedEntity(
                new ResolvedDataIdentifier(
                    new ParserException(string.Empty, 3001, Resources.Ansi_Mh10_8_2_Error_003, true),
                    initialPosition));
            return;
        }

#if NET7_0_OR_GREATER
        DoParseRecords(data, processResolvedEntity, null, initialPosition);
#else
        DoParseRecords(data, processResolvedEntity, initialPosition);
#endif
    }
#endif

#if NET7_0_OR_GREATER
    /// <summary>
    ///   Code generator for regular expression that captures a data identifier (0..3 digits followed by a letter).
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\d{0,3}[a-zA-Z]", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MatchDataIdentifierRegex();
#endif

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
    /// <summary>
    ///   Parse the fields in the record buffer.
    /// </summary>
    /// <param name="recordBuffer">
    ///   The record buffer.
    /// </param>
    /// <param name="processResolvedEntity">
    ///   The function for processing resolved entities.
    /// </param>
#if NET7_0_OR_GREATER
    /// <param name="processResolvedElementDelegate">
    ///     A delegate that is invoked to process each element element.
    /// </param>
    /// <remarks>
    /// If a delegate is provided, it is invoked to process each element element.
    /// </remarks>
#endif
    /// <param name="currentPosition">
    ///   The current character position.
    /// </param>
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    private static void DoParseFields(
        ReadOnlySpan<char> recordBuffer,
        Action<IResolvedEntity>? processResolvedEntity,
#if NET7_0_OR_GREATER
        ResolvedElementDelegate? processResolvedElementDelegate,
#endif
        int currentPosition) {
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        var fieldSeparator = ((char)29).ToInvariantString();

        while (true) {
            // If the record buffer does not contain data, process next record
            if (recordBuffer.IsNullOrWhiteSpace()) {
                return;
            }

            // Does the record buffer contain at least one field separator?
            var fieldPosition = currentPosition;
            var fieldBuffer = recordBuffer.Contains(fieldSeparator ?? string.Empty, StringComparison.Ordinal)
                /* Yes - Move data up the first field separator from the record buffer into the field buffer. */
#if NET6_0_OR_GREATER
                ? recordBuffer[..recordBuffer.IndexOf(fieldSeparator ?? string.Empty, StringComparison.Ordinal)]
#else
                ? recordBuffer.Slice(0, recordBuffer.IndexOf(fieldSeparator ?? string.Empty, StringComparison.Ordinal))
#endif
                /* No - Move all data from the record buffer into the field buffer. */
                : recordBuffer;

            // Remove field from record buffer.
#if NET6_0_OR_GREATER
            recordBuffer = recordBuffer[fieldBuffer.Length..];
#else
            recordBuffer = recordBuffer.Slice(fieldBuffer.Length);
#endif
            currentPosition += fieldBuffer.Length;

            // Remove any leading field separator from the record buffer
            var recordBufferLength = recordBuffer.Length;
            recordBuffer = recordBuffer.StartsWith(fieldSeparator ?? string.Empty, StringComparison.Ordinal)
#if NET6_0_OR_GREATER
                ? recordBuffer[(fieldSeparator ?? string.Empty).Length..]
#else
                ? recordBuffer.Slice((fieldSeparator ?? string.Empty).Length)
#endif
                : recordBuffer;
            currentPosition += recordBufferLength - recordBuffer.Length;

            // Capture the data identifier (0..3 digits followed by a letter)
#if NET7_0_OR_GREATER
            var enumerator = MatchDataIdentifierRegex().EnumerateMatches(fieldBuffer);
            var hasMatch = enumerator.MoveNext();
            if (!hasMatch) {
                HandleMissingDataIdentifierError(fieldPosition);
                continue;
            }

            var first = enumerator.Current;

            // Move the field position to the start of the field data
            fieldPosition += first.Length;

            // Transmit data for further processing.
            if (processResolvedElementDelegate is not null) {
                ResolvedDataIdentifierRef defaultElement = new (
                    -1,
                    stackalloc char[4],
                    null,
                    null,
                    stackalloc char[ResolvedDataIdentifierRef.ValueMaxLength],
                    false,
                    stackalloc char[ResolvedDataIdentifierRef.DataTitleMaxLength],
                    stackalloc char[ResolvedDataIdentifierRef.DescriptionMaxLength],
                    0,
                    0);
                processResolvedElementDelegate
                    .Invoke(fieldBuffer[first.Length..]
                    .ResolveEx(defaultElement, fieldBuffer[first.Index..first.Length], fieldPosition));
                return;
            }

            processResolvedEntity?
                .Invoke(fieldBuffer[first.Length..]
                .Resolve(fieldBuffer[first.Index..first.Length], fieldPosition));

#else
            var match = MatchDataIdentifierRegex.Match(fieldBuffer.ToString());  // must materialize to string for Regex use

            if (!match.Success) {
                // Handle errors
                HandleMissingDataIdentifierError(fieldPosition);
                continue;
            }

            // Move the field position to the start of the field data
            fieldPosition += match.Value.Length;

#if NET6_0_OR_GREATER
            processResolvedEntity?.Invoke(fieldBuffer[match.Length..]
#else
            processResolvedEntity?.Invoke(fieldBuffer.Slice(match.Length)
#endif
                .Resolve(match.Value, fieldPosition));
#endif
        }

        void HandleMissingDataIdentifierError(int fieldPosition) {
            processResolvedEntity?.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(string.Empty, 3008, Resources.Ansi_Mh10_8_2_Error_004, false),
                    fieldPosition));
        }
    }

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
    /// <summary>
    ///   Parse the records in the data buffer.
    /// </summary>
    /// <param name="dataBuffer">
    ///   The data buffer.
    /// </param>
    /// <param name="processResolvedEntity">
    ///   The function for processing resolved entities.
    /// </param>
#if NET7_0_OR_GREATER
    /// <param name="processResolvedElementDelegate">
    ///     A delegate that is invoked to process each element element.
    /// </param>
    /// <remarks>
    /// If a delegate is provided, it is invoked to process each element element.
    /// </remarks>
#endif
    /// <param name="currentPosition">
    ///   The current character position.
    /// </param>
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    private static void DoParseRecords(
        ReadOnlySpan<char> dataBuffer,
        Action<IResolvedEntity>? processResolvedEntity,
#if NET7_0_OR_GREATER
        ResolvedElementDelegate? processResolvedElementDelegate,
#endif
        int currentPosition) {
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        var formatHeader = "06" + (char)29;
        var formatTrailer = ((char)30).ToInvariantString();

        // Does the data buffer contain data?
        if (dataBuffer.IsNullOrWhiteSpace()) {
            // No - End parsing.
#if NET7_0_OR_GREATER
            if (processResolvedElementDelegate is not null) {
                processResolvedElementDelegate.Invoke(
                    new ResolvedDataIdentifierRef(
                        new ParserException(string.Empty, 3004, Resources.Ansi_Mh10_8_2_Error_001, true),
                        currentPosition));
                return;
            }
#endif
            processResolvedEntity?.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(string.Empty, 3004, Resources.Ansi_Mh10_8_2_Error_001, true),
                    currentPosition));
            return;
        }

        while (true) {
            // If the data buffer does not contain data, end parsing.
            if (dataBuffer.IsNullOrWhiteSpace()) {
                return;
            }

            // Is the data terminated by a format trailer?
            var recordPosition = currentPosition;
            var recordBuffer = dataBuffer!.Contains(formatTrailer, StringComparison.Ordinal)
               ? FormatTrailerTestFormatHeader(dataBuffer)
               /* No - Does the data buffer start with a format header */
               : NoFormatTrailerTestFormatHeader(dataBuffer);

            // Remove record from data buffer.
#if NET6_0_OR_GREATER
            dataBuffer = dataBuffer[recordBuffer.Length..];

#else
            dataBuffer = dataBuffer.Slice(recordBuffer.Length);
#endif
            currentPosition += recordBuffer.Length;

            // Remove any leading format trailer from the data buffer
            var dataBufferLength = dataBuffer.Length;
            dataBuffer = dataBuffer.StartsWith(formatTrailer, StringComparison.Ordinal)
#if NET6_0_OR_GREATER
                ? dataBuffer[formatTrailer.Length..]
#else
                ? dataBuffer.Slice(formatTrailer.Length)
#endif
                : dataBuffer;
            currentPosition += dataBufferLength - dataBuffer.Length;

            // Remove any leading format header from the record buffer
            recordBuffer = recordBuffer.StartsWith(formatHeader, StringComparison.Ordinal)
#if NET6_0_OR_GREATER
                ? recordBuffer[formatHeader.Length..]
#else
                ? recordBuffer.Slice(formatHeader.Length)
#endif
                : recordBuffer;

            // Does the record buffer contain data?
            if (recordBuffer.IsNullOrWhiteSpace()) {
                // No - continue.
                continue;
            }

#if NET7_0_OR_GREATER
            if (processResolvedElementDelegate is not null) {
                DoParseFields(recordBuffer, null, processResolvedElementDelegate, recordPosition);
            }
            else {
                DoParseFields(recordBuffer, processResolvedEntity, null, recordPosition);
            }
#else
            DoParseFields(recordBuffer, processResolvedEntity, recordPosition);
#endif
            continue;

            ReadOnlySpan<char> FormatTrailerTestFormatHeader(ReadOnlySpan<char> buffer) => buffer.StartsWith(formatHeader, StringComparison.Ordinal)
                /* Yes - Copy data up to and including the first format trailer into record buffer */
#if NET6_0_OR_GREATER
                ? buffer[..buffer.IndexOf(formatTrailer, StringComparison.Ordinal)]
#else
                ? buffer.Slice(buffer.IndexOf(formatTrailer, StringComparison.Ordinal))
#endif
                /* No - Handle errors */
                : HandleFormatDataError(Resources.Ansi_Mh10_8_2_001);

            ReadOnlySpan<char> NoFormatTrailerTestFormatHeader(ReadOnlySpan<char> buffer) => buffer.StartsWith(formatHeader, StringComparison.Ordinal)
                /* Yes - Handle errors */
                ? HandleFormatDataError(Resources.Ansi_Mh10_8_2_002)
                /* No - Copy the data buffer into the record buffer */
                : buffer;
        }

        string HandleFormatDataError(string formatPart) {
#if NET7_0_OR_GREATER
            if (processResolvedElementDelegate is not null) {
                processResolvedElementDelegate.Invoke(
                    new ResolvedDataIdentifierRef(
                        new ParserException(
                            string.Empty,
                            3003,
                            string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_002, formatPart),
                            false),
                        currentPosition));
                return string.Empty;
            }
#endif
            processResolvedEntity?.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(
                        string.Empty,
                        3003,
                        string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_002, formatPart),
                        false),
                    currentPosition));
            return string.Empty;
        }
    }
}