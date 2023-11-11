// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Solidsoft Reply Ltd.">
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
// Parser for ANSI MH10.8 data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: CLSCompliant(true)]
namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using Common;

using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Properties;

/// <summary>
///   Barcode Parser for ANSI MH10.8 data.
/// </summary>
public static partial class Parser
{
    /// <summary>
    ///   Code generator for regular expression that captures a data identifier (0..3 digits followed by a letter).
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\d{0,3}[a-zA-Z]", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex MatchDataIdentifierRegex();

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
    public static void Parse(string? data, Action<IResolvedEntity> processResolvedEntity, int initialPosition = 0)
    {
        if (processResolvedEntity is null) {
            throw new ArgumentNullException(nameof(processResolvedEntity));
        }

        // Is any data present?
        if (string.IsNullOrWhiteSpace(data))
        {
            // Handle errors
            processResolvedEntity(
                new ResolvedDataIdentifier(
                    new ParserException(3001, Resources.Ansi_Mh10_8_2_Error_003, true),
                    initialPosition));
            return;
        }

        DoParseRecords(data, processResolvedEntity, initialPosition);
    }

    /// <summary>
    ///   Parse the fields in the record buffer.
    /// </summary>
    /// <param name="recordBuffer">
    ///   The record buffer
    /// </param>
    /// <param name="processResolvedEntity">
    ///   The function for processing resolved entities.
    /// </param>
    /// <param name="currentPosition">
    ///   The current character position.
    /// </param>
    private static void DoParseFields(
        string recordBuffer,
        Action<IResolvedEntity> processResolvedEntity,
        int currentPosition)
    {
        var fieldSeparator = ((char)29).ToInvariantString();

        while (true)
        {
            // If the record buffer does not contain data, process next record
            if (string.IsNullOrWhiteSpace(recordBuffer))
            {
                return;
            }

            // Does the record buffer contain at least one field separator?
            var fieldPosition = currentPosition;
            var fieldBuffer = recordBuffer.Contains(fieldSeparator, StringComparison.Ordinal)
                                  /* Yes - Move data up the first field separator from the record buffer into the field buffer. */
                                  ? recordBuffer[..recordBuffer.IndexOf(fieldSeparator, StringComparison.Ordinal)]
                                  /* No - Move all data from the record buffer into the field buffer. */
                                  : recordBuffer;

            // Remove field from record buffer.
            recordBuffer = recordBuffer[fieldBuffer.Length..];
            currentPosition += fieldBuffer.Length;

            // Remove any leading field separator from the record buffer
            var recordBufferLength = recordBuffer.Length;
            recordBuffer = recordBuffer.StartsWith(fieldSeparator, StringComparison.Ordinal)
                               ? recordBuffer[fieldSeparator.Length..]
                               : recordBuffer;
            currentPosition += recordBufferLength - recordBuffer.Length;

            // Capture the data identifier (0..3 digits followed by a letter)
            var match = MatchDataIdentifierRegex().Match(fieldBuffer);

            if (!match.Success)
            {
                // Handle errors
                HandleMissingDataIdentifierError(fieldPosition);
                continue;
            }

            // Move the field position to the start of the field data
            fieldPosition += match.Value.Length;

            // Transmit data for further processing.
            processResolvedEntity?.Invoke(fieldBuffer[match.Length..].Resolve(match.Value, fieldPosition));
        }

        void HandleMissingDataIdentifierError(int fieldPosition)
        {
            processResolvedEntity?.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(3008, Resources.Ansi_Mh10_8_2_Error_004, false),
                    fieldPosition));
        }
    }

    /// <summary>
    ///   Parse the records in the data buffer.
    /// </summary>
    /// <param name="dataBuffer">
    ///   The data buffer
    /// </param>
    /// <param name="processResolvedEntity">
    ///   The function for processing resolved entities.
    /// </param>
    /// <param name="currentPosition">
    ///   The current character position.
    /// </param>
    private static void DoParseRecords(
        string dataBuffer,
        Action<IResolvedEntity> processResolvedEntity,
        int currentPosition)
    {
        var formatHeader = "06" + (char)29;
        var formatTrailer = ((char)30).ToInvariantString();

        // Does the data buffer contain data?
        if (string.IsNullOrWhiteSpace(dataBuffer))
        {
            // No - End parsing.
            processResolvedEntity.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(3004, Resources.Ansi_Mh10_8_2_Error_001, true),
                    currentPosition));
            return;
        }

        while (true)
        {
            // If the data buffer does not contain data, end parsing.
            if (string.IsNullOrWhiteSpace(dataBuffer))
            {
                return;
            }

            // Is the data terminated by a format trailer?
            var recordPosition = currentPosition;

            var recordBuffer = dataBuffer.Contains(formatTrailer, StringComparison.Ordinal)
                                   /* Yes - Does the data buffer start with a format header? */
                                   ? FormatTrailerTestFormatHeader()
                                   /* No - Does the data buffer start with a format header */
                                   : NoFormatTrailerTestFormatHeader();

            // Remove record from data buffer.
            dataBuffer = dataBuffer[recordBuffer.Length..];
            currentPosition += recordBuffer.Length;

            // Remove any leading format trailer from the data buffer
            var dataBufferLength = dataBuffer.Length;
            dataBuffer = dataBuffer.StartsWith(formatTrailer, StringComparison.Ordinal)
                             ? dataBuffer[formatTrailer.Length..]
                             : dataBuffer;
            currentPosition += dataBufferLength - dataBuffer.Length;

            // Remove any leading format header from the record buffer
            recordBuffer = recordBuffer.StartsWith(formatHeader, StringComparison.Ordinal)
                               ? recordBuffer[formatHeader.Length..]
                               : recordBuffer;

            // Does the record buffer contain data?
            if (string.IsNullOrWhiteSpace(recordBuffer))
            {
                // No - continue.
                continue;
            }

            DoParseFields(recordBuffer, processResolvedEntity, recordPosition);
            continue;

            string FormatTrailerTestFormatHeader() => dataBuffer.StartsWith(formatHeader, StringComparison.Ordinal)
                /* Yes - Copy data up to and including the first format trailer into record buffer */
                ? dataBuffer[..dataBuffer.IndexOf(formatTrailer, StringComparison.Ordinal)]
                /* No - Handle errors */
                : HandleFormatDataError(Resources.Ansi_Mh10_8_2_001);

            string NoFormatTrailerTestFormatHeader() => dataBuffer.StartsWith(formatHeader, StringComparison.Ordinal)
                /* Yes - Handle errors */
                ? HandleFormatDataError(Resources.Ansi_Mh10_8_2_002)
                /* No - Copy the data buffer into the record buffer */
                : dataBuffer;
        }

        string HandleFormatDataError(string formatPart)
        {
            processResolvedEntity.Invoke(
                new ResolvedDataIdentifier(
                    new ParserException(
                        3003,
                        string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_002, formatPart),
                        false),
                    currentPosition));
            return string.Empty;
        }
    }
}