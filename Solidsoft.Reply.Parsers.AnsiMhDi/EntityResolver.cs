// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityResolver.cs" company="Solidsoft Reply Ltd.">
//   (c) 2018-2024 Solidsoft Reply Ltd. All rights reserved.
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
// The entity resolver for ASC (MH10.8) data identifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable BadListLineBreaks

namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

using Descriptors;
using Properties;

using Common;

/// <summary>
///   The entity resolver for ASC (MH10.8) data identifiers.
/// </summary>
[SuppressMessage(
     "ReSharper",
     "StringLiteralTypo",
     Justification = "Resharper does not hide errors correctly using in-line comments."),
 SuppressMessage(
     "ReSharper",
     "CommentTypo",
     Justification = "Resharper does not hide errors correctly using in-line comments.")]
#if NET7_0_OR_GREATER
public static partial class EntityResolver
{
    /// <summary>
    ///   A regular expression for six-digit date representation - DDMMYY.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"(((0\d|[12]\d|3[01])(0[13578]|1[02])(\d{2}))|((0\d|[12]\d|30)(0[13456789]|1[012])(\d{2}))|((0\d|1\d|2[0-8])02(\d{2}))|(2902((0[048]|[2468][048]|[13579][26]))))", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternDdMmYyRegEx();

    /// <summary>
    ///   The date pattern dd mm yyyyy.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    [GeneratedRegex(@"^(((0[1-9]|[12]\d|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1\d|2[0-8])[-/]?02)[-/]?\d{4}|29[-/]?02[-/]?(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00))$", RegexOptions.None, "en-US")]
    // ReSharper disable once IdentifierTypo
    private static partial Regex DatePatternDdMmYyyyyRegEx();

    /// <summary>
    ///   A regular expression for six-digit date representation - MMDDYY.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"(((0[13578]|1[02])(0\d|[12]\d|3[01])(\d{2}))|((0[13456789]|1[012])(0\d|[12]\d|30)(\d{2}))|(02(0\d|1\d|2[0-8])(\d{2}))|(0229((0[048]|[2468][048]|[13579][26]))))", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternMmDdYyRegEx();

    /// <summary>
    ///   The Julian date patter YDDD.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^(\d)(00[1-9]|0[1-9]\d|[1-2]\d\d|3[0-5]\d|36[0-6])$", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYDddJulianRegEx();

    /// <summary>
    ///   The Julian date patter YYDDD.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^(\d{2})(00[1-9]|0[1-9]\d|[1-2]\d\d|3[0-5]\d|36[0-6])$", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyDddJulianRegEx();

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"(((\d{2})(0[13578]|1[02])(0[1-9]|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0[1-9]|[12]\d|30))|((\d{2})02(0[1-9]|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyMmDdRegEx();

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    ///   If it is not necessary to specify the day, the day field can be filled with two zeros.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"(((\d{2})(0[13578]|1[02])(0\d|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0\d|[12]\d|30))|((\d{2})02(0\d|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyMmDdZerosRegEx();

    /// <summary>
    ///   The date patter YDWW.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyWwRegEx();

    /// <summary>
    ///   The date pattern yyyy mm dd.
    /// </summary>
    /// <returns>A regular expression.</returns>
    // ReSharper disable once IdentifierTypo
    [GeneratedRegex(@"(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyyyMmDdRegEx();

    /// <summary>
    ///   The date pattern YYYYMMDDHHMM.
    /// </summary>
    /// <returns>A regular expression.</returns>
    // ReSharper disable once IdentifierTypo
    [GeneratedRegex(@"(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:\d{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:0[1-9]|1\d|2\d))))|(?:\d{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:[01]\d|2[0-8])))))(?:0\d|1\d|2[0-3])(?:[0-5]\d)", RegexOptions.None, "en-US")]
    private static partial Regex DatePatternYyyyMmDdHhMmRegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric {1 ..}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]+", RegexOptions.None, "en-US")]
    private static partial Regex Alphanumeric01UnboundRegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric with plus {1 ..}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z+]+", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericWithPlus01UnboundRegEx();

    /// <summary>
    /// Regular Expression: Invariants except plus {1 ..}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[-!""%&'()*,./0-9:;<=>?A-Z_a-z]+", RegexOptions.None, "en-US")]
    private static partial Regex InvariantNoPlus01UnboundPlusRegEx();

    /// <summary>
    /// Regular Expression: Numeric with plus {1 ..}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9+]+", RegexOptions.None, "en-US")]
    private static partial Regex NumericPlus01UnboundRegEx();

    /// <summary>
    ///   Returns a regular expression for character strings of variable length
    ///   representing latitude, longitude and attitude.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"-?\d{1,2}(.\d{1,5})?/-?\d{1,3}(.\d{1,5})?/-?\d{1,4}", RegexOptions.None, "en-US")]
    private static partial Regex LatitudeLongitudeAttitudeRegEx();

    /// <summary>
    ///   Returns a regular expression for character strings indicating Yes (Y) and No (N).
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[YN]", RegexOptions.None, "en-US")]
    private static partial Regex YesNoLetterRegEx();

    /// <summary>
    ///   Returns a regular expression for the '+' character used for HIBC codes.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\+$", RegexOptions.None, "en-US")]
    private static partial Regex HibccPlusRegEx();

    /// <summary>
    ///   Returns a regular expression for the '$' character used for ISBT 128 codes.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\&$", RegexOptions.None, "en-US")]
    private static partial Regex IccbbaAmpersandRegEx();

    /// <summary>
    ///   Returns a regular expression for the '=' character used for ISBT 128 codes.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\=$", RegexOptions.None, "en-US")]
    private static partial Regex IccbbaEqualRegEx();

    /// <summary>
    ///   Returns a regular expression for the GS1 FNC1 character.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\x1D$", RegexOptions.None, "en-US")]
    private static partial Regex Gs1Function1RegEx();

    /// <summary>
    ///    Returns a regular expression for the message header preamble defined by ISO/IEC 15434 ",
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\[\)\>\x1E", RegexOptions.None, "en-US")]
    private static partial Regex IsoIec15434PreambleRegEx();

    /// <summary>
    ///    Returns a regular expression for the hyphen character used to specify Pharmaceutical Central Numbers (PZNs).
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\-$", RegexOptions.None, "en-US")]
    private static partial Regex IfaAbdataPznHyphenRegEx();

    /// <summary>
    ///    Returns a regular expression for the exclamation mark character used for Eurocode IBLS.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\!$", RegexOptions.None, "en-US")]
    private static partial Regex EurocodeIblsExclamationMarkRegEx();

    /// <summary>
    ///    Returns a regular expression for the Dangerous Cargo IMDG class.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\d(.\d[A-Z]?)?$", RegexOptions.None, "en-US")]
    private static partial Regex DangerousCargoClassRegEx();

    /// <summary>
    ///    Returns a regular expression for Vessel Registration Numbers.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^IMO\d{7}$", RegexOptions.None, "en-US")]
    private static partial Regex VesselRegistrationNumberRegEx();

    /// <summary>
    ///    Returns a regular expression for Electronic Seal Numbers.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^.{{6}}$", RegexOptions.None, "en-US")]
    private static partial Regex ElectronicSealNumbersRegEx();

    /// <summary>
    ///    Returns a regular expression for Surety Numbers.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^.{{6}}$", RegexOptions.None, "en-US")]
    private static partial Regex SuretyNumberRegEx();

    /// <summary>
    ///    Returns a regular expression for Foreign Ports of Lading.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^.{{6}}$", RegexOptions.None, "en-US")]
    private static partial Regex ForeignPortOfLadingRegEx();

    /// <summary>
    ///    Returns a regular expression for "Format MMYY dates.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^((0[1-9])|(1[0-2]))\d{2}$", RegexOptions.None, "en-US")]
    private static partial Regex FormatMmYyRegEx();

    /// <summary>
    ///    Returns a regular expression for Event, Date, And Time.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:\d{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:0[1-9]|1\d|2\d))))|(?:\d{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:[01]\d|2[0-8])))))(?:0\d|1\d|2[0-3])(?:[0-5]\d)\d{{1,3}}$", RegexOptions.None, "en-US")]
    private static partial Regex EventDateAndTimeRegEx();

    /// <summary>
    ///    Returns a regular expression for Format YYYYWW.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^([1-2]\d)(\d\d)(0[1-9]|[1-4]\d|5[0-3])$", RegexOptions.None, "en-US")]
    private static partial Regex FormatYyyyWwRegEx();

    /// <summary>
    ///    Returns a regular expression for the oldest and newest manufacturing date in the format YYWWYYWW.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))$", RegexOptions.None, "en-US")]
    private static partial Regex OldestAndNewestManufacturingDateRegEx();

    /// <summary>
    ///    Returns a regular expression for the harvest date range.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)$", RegexOptions.None, "en-US")]
    private static partial Regex HarvestDateRangeRegEx();

    /// <summary>
    ///    Returns a regular expression for a Uniform Resource Locator (URL).
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"^(http[s]?:\/\/((([a-zA-Z0-9]([a-zA-Z0-9]|-)*[a-zA-Z0-9]|[a-zA-Z0-9])\\.)*[a-zA-Z]([a-zA-Z0-9]|-)*[a-zA-Z0-9]|[a-zA-Z]|\d+\\.\d+\\.\d+\\.\d+)(:\d+)?(\/([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*(\/([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*)*(\\?([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*)?)?)$", RegexOptions.None, "en-US")]
    private static partial Regex UniformResourceLocatorRegEx();

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length for CAGE Code and Serial number.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z-/]{6,25}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericCageSnRegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric {2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{2}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx2();

    /// <summary>
    /// Regular Expression: Alphanumeric {3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{3}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx3();

    /// <summary>
    /// Regular Expression: Alphanumeric {4}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{4}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx4();

    /// <summary>
    /// Regular Expression: Alphanumeric {5}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{5}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx5();

    /// <summary>
    /// Regular Expression: Alphanumeric {6}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{6}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx6();

    /// <summary>
    /// Regular Expression: Alphanumeric {10}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{10}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx10();

    /// <summary>
    /// Regular Expression: Alphanumeric {12}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{12}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx12();

    /// <summary>
    /// Regular Expression: Alphanumeric {18}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{18}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx18();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,3}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0103();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 9}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,9}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0109();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 10}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,10}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0110();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 20}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,20}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0120();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 35}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,35}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0135();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 50}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,50}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0150();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 100}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,100}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx01100();

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 30}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{2,30}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0230();

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 35}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{2,35}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0235();

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 9}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{3,9}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0309();

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 22}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{3,22}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0322();

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{3,35}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0335();

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 11}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{4,11}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0411();

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 25}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{4,25}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0425();

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 29}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{5,29}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0529();

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 16}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{5,16}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0516();

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 22}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{5,22}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0522();

    /// <summary>
    /// Regular Expression: Alphanumeric {6, 35}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{6,35}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx0635();

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 12}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{10,12}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx1012();

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 15}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{10,15}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx1015();

    /// <summary>
    /// Regular Expression: Alphanumeric {13, 15}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{13,15}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx1315();

    /// <summary>
    /// Regular Expression: Alphanumeric {16, 26}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{16,26}", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericRegEx1626();

    /// <summary>
    /// Regular Expression: Alphanumeric {4} . Alphanumeric {1, 10}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{4}.[0-9A-Z]{1,10}", RegexOptions.None, "en-US")]
    private static partial Regex Alphanumeric04Alphanumeric0110RegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 32} Alphanumeric {3} with leading dashes
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{1,32}([0-9A-Z]{3}|-[0-9A-Z]{2}|--[0-9A-Z]{1}|---)", RegexOptions.None, "en-US")]
    private static partial Regex Alphanumeric0132Alphanumeric03WithDashesRegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric {11} Numeric {2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{11}\d{2}$", RegexOptions.None, "en-US")]
    private static partial Regex Alphanumeric11Numeric02();

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric and dash {1, 6} Numeric and dot {5} Alphanumeric {2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{2}[A-Z0-9-]{1,6}[0-9.]{5}[0-9A-Z]{2}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric02AlphanumericDash0106NumericDot05Alphanumeric02RegEx();

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35} + Alpha {1, 3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z]{3,35}\+[A-Z]{1,3}$", RegexOptions.None, "en-US")]
    private static partial Regex Alphanumeric0335Alpha0103RegEx();

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 27}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{2}[0-9A-Z]{3,27}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha02Alphanumeric0327RegEx();

    /// <summary>
    /// Regular Expression: Alpha {3} Numeric {14} Alphanumeric {1, 33}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{3}\d{14}[0-9A-Za-z*+-./()!]{1,33}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha03Numeric14Alphanumeric0133RegEx();

    /// <summary>
    /// Regular Expression: Alpha {1} Numeric {4} Alphanumeric {5, 20}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{1}\d{4}[0-9A-Z]{5,20}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha01Numeric04Alphanumeric0520RegEx();

    /// <summary>
    /// Regular Expression: Numeric {1, 8} Alphanumeric {2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{1,8}[0-9A-Z]{2}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0108Alphanumeric02RegEx();

    /// <summary>
    /// Regular Expression: Numeric {1, 10} Alphanumeric {3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{1,10}[0-9A-Z]{3}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0110Alphanumeric03RegEx();

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric {3, 42}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{2}[0-9A-Z]{3,42}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric02Alphanumeric0342RegEx();

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 18}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{2}[0-9A-Z]{3,18}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha02Alphanumeric0318RegEx();

    /// <summary>
    ///  Regular Expression: Alphanumeric with space {1, 60}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z ]{1,60}$", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericSpace0160RegEx();

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 50}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z ]{1,50}$", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericPlus0150RegEx();

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {20, 50}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z+]{20,50}$", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericPlus2050RegEx();

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 60}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9A-Z ]{1,60}$", RegexOptions.None, "en-US")]
    private static partial Regex AlphanumericPlus0160RegEx();

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {7}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{4}\d{7}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha04Numeric07RegEx();

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {1, 3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{4}\d{1,3}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha04Numeric0103RegEx();

    /// <summary>
    ///  Regular Expression: Alpha {3} Numeric {3}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{3}\d{3}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha03Numeric03RegEx();

    /// <summary>
    ///  Regular Expression: Alpha {2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{2}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha02RegEx();

    /// <summary>
    ///  Regular Expression: Invariant {2, 12}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{2,12}$", RegexOptions.None, "en-US")]
    private static partial Regex Invariant0212RegEx();

    /// <summary>
    ///  Regular Expression: Alpha {2} Invariant {3, 27}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[A-Z]{2}[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{3,27}$", RegexOptions.None, "en-US")]
    private static partial Regex Alpha02Invariant0327RegEx();
    
    /// <summary>
    ///  Regular Expression: Numeric {1}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{1}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric01RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {5}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{5}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric05RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {9}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{9}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric09RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {14}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{14}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric14RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {18}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{18}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric18RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {1, 2}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{1,2}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0102RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {4, 6}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{4,6}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0406RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {6, 26}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{6,26}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0626RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {7, 12}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{7,12}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0712RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {9, 13}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{9,13}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric0913RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {10, 12}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{10,12}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric1012RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {13, 14}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{13,14}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric1314RegEx();

    /// <summary>
    ///  Regular Expression: Numeric {14, 26}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"\d{14,26}$", RegexOptions.None, "en-US")]
    private static partial Regex Numeric1426RegEx();

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 10}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9.]{1,10}$", RegexOptions.None, "en-US")]
    private static partial Regex NumericDot0110RegEx();

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 20}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9.]{1,20}$", RegexOptions.None, "en-US")]
    private static partial Regex NumericDot0120RegEx();

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 05}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9.]{1,5}$", RegexOptions.None, "en-US")]
    private static partial Regex NumericDot0105RegEx();

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 06}
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"[0-9.]{1,6}$", RegexOptions.None, "en-US")]
    private static partial Regex NumericDot0106RegEx();

    /// <summary>
    ///  Regular Expression: Minus with Numeric {01, 04}
    ///   sign.
    /// </summary>
    /// <returns>A regular expression.</returns>
    [GeneratedRegex(@"-?\d{1,4}$", RegexOptions.None, "en-US")]
    private static partial Regex MinusNumeric0104RegEx();
#else
public static class EntityResolver
{
    /// <summary>
    ///   A regular expression for six-digit date representation - DDMMYY.
    /// </summary>
    private static readonly Regex DatePatternDdMmYyRegEx = new(@"(((0\d|[12]\d|3[01])(0[13578]|1[02])(\d{2}))|((0\d|[12]\d|30)(0[13456789]|1[012])(\d{2}))|((0\d|1\d|2[0-8])02(\d{2}))|(2902((0[048]|[2468][048]|[13579][26]))))", RegexOptions.None);

    /// <summary>
    ///   The date pattern dd mm yyyyy.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once IdentifierTypo
    private static readonly Regex DatePatternDdMmYyyyyRegEx = new(@"^(((0[1-9]|[12]\d|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1\d|2[0-8])[-/]?02)[-/]?\d{4}|29[-/]?02[-/]?(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00))$", RegexOptions.None);

    /// <summary>
    ///   A regular expression for six-digit date representation - MMDDYY.
    /// </summary>
    private static readonly Regex DatePatternMmDdYyRegEx = new(@"(((0[13578]|1[02])(0\d|[12]\d|3[01])(\d{2}))|((0[13456789]|1[012])(0\d|[12]\d|30)(\d{2}))|(02(0\d|1\d|2[0-8])(\d{2}))|(0229((0[048]|[2468][048]|[13579][26]))))", RegexOptions.None);

    /// <summary>
    ///   The Julian date patter YDDD.
    /// </summary>
    private static readonly Regex DatePatternYDddJulianRegEx = new(@"^(\d)(00[1-9]|0[1-9]\d|[1-2]\d\d|3[0-5]\d|36[0-6])$", RegexOptions.None);

    /// <summary>
    ///   The Julian date patter YYDDD.
    /// </summary>
    private static readonly Regex DatePatternYyDddJulianRegEx = new(@"^(\d{2})(00[1-9]|0[1-9]\d|[1-2]\d\d|3[0-5]\d|36[0-6])$", RegexOptions.None);

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    /// </summary>
    private static readonly Regex DatePatternYyMmDdRegEx = new(@"(((\d{2})(0[13578]|1[02])(0[1-9]|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0[1-9]|[12]\d|30))|((\d{2})02(0[1-9]|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))", RegexOptions.None);

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    ///   If it is not necessary to specify the day, the day field can be filled with two zeros.
    /// </summary>
    private static readonly Regex DatePatternYyMmDdZerosRegEx = new(@"(((\d{2})(0[13578]|1[02])(0\d|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0\d|[12]\d|30))|((\d{2})02(0\d|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))", RegexOptions.None);

    /// <summary>
    ///   The date patter YDWW.
    /// </summary>
    private static readonly Regex DatePatternYyWwRegEx = new(@"\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))", RegexOptions.None);

    /// <summary>
    ///   The date pattern yyyy mm dd.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    private static readonly Regex DatePatternYyyyMmDdRegEx = new(@"(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)", RegexOptions.None);

    /// <summary>
    ///   The date pattern YYYYMMDDHHMM.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    private static readonly Regex DatePatternYyyyMmDdHhMmRegEx = new(@"(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:\d{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:0[1-9]|1\d|2\d))))|(?:\d{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:[01]\d|2[0-8])))))(?:0\d|1\d|2[0-3])(?:[0-5]\d)", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1 ..}
    /// </summary>
    private static readonly Regex Alphanumeric01UnboundRegEx = new(@"[0-9A-Z]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric with plus {1 ..}
    /// </summary>
    private static readonly Regex AlphanumericWithPlus01UnboundRegEx = new(@"[0-9A-Z+]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Invariants except plus {1 ..}
    /// </summary>
    private static readonly Regex InvariantNoPlus01UnboundPlusRegEx = new(@"[-!""%&'()*,./0-9:;<=>?A-Z_a-z]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric with plus {1 ..}
    /// </summary>
    private static readonly Regex NumericPlus01UnboundRegEx = new(@"[0-9+]+", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for character strings of variable length
    ///   representing latitude, longitude and attitude.
    /// </summary>
    private static readonly Regex LatitudeLongitudeAttitudeRegEx = new(@"-?\d{1,2}(.\d{1,5})?/-?\d{1,3}(.\d{1,5})?/-?\d{1,4}", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for character strings indicating Yes (Y) and No (N).
    /// </summary>
    private static readonly Regex YesNoLetterRegEx = new(@"[YN]", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for the '+' character used for HIBC codes.
    /// </summary>
    private static readonly Regex HibccPlusRegEx = new(@"^\+$", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for the '$' character used for ISBT 128 codes.
    /// </summary>
    private static readonly Regex IccbbaAmpersandRegEx = new(@"^\&$", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for the '=' character used for ISBT 128 codes.
    /// </summary>
    private static readonly Regex IccbbaEqualRegEx = new(@"^\=$", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for the GS1 FNC1 character.
    /// </summary>
    private static readonly Regex Gs1Function1RegEx = new(@"^\x1D$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the message header preamble defined by ISO/IEC 15434 ",
    /// </summary>
    private static readonly Regex IsoIec15434PreambleRegEx = new(@"^\[\)\>\x1E", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the hyphen character used to specify Pharmaceutical Central Numbers (PZNs).
    /// </summary>
    private static readonly Regex IfaAbdataPznHyphenRegEx = new(@"^\-$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the exclamation mark character used for Eurocode IBLS.
    /// </summary>
    private static readonly Regex EurocodeIblsExclamationMarkRegEx = new(@"^\!$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the Dangerous Cargo IMDG class.
    /// </summary>
    private static readonly Regex DangerousCargoClassRegEx = new(@"^\d(.\d[A-Z]?)?$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Vessel Registration Numbers.
    /// </summary>
    private static readonly Regex VesselRegistrationNumberRegEx = new(@"^IMO\d{7}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Electronic Seal Numbers.
    /// </summary>
    private static readonly Regex ElectronicSealNumbersRegEx = new(@"^.{{6}}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Surety Numbers.
    /// </summary>
    private static readonly Regex SuretyNumberRegEx = new(@"^.{{6}}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Foreign Ports of Lading.
    /// </summary>
    private static readonly Regex ForeignPortOfLadingRegEx = new(@"^.{{6}}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for "Format MMYY dates.
    /// </summary>
    private static readonly Regex FormatMmYyRegEx = new(@"^((0[1-9])|(1[0-2]))\d{2}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Event, Date, And Time.
    /// </summary>
    private static readonly Regex EventDateAndTimeRegEx = new(@"^(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:\d{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:0[1-9]|1\d|2\d))))|(?:\d{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1\d|2\d|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1\d|2\d|3[01]))|(?:02(?:[01]\d|2[0-8])))))(?:0\d|1\d|2[0-3])(?:[0-5]\d)\d{{1,3}}$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for Format YYYYWW.
    /// </summary>
    private static readonly Regex FormatYyyyWwRegEx = new(@"^([1-2]\d)(\d\d)(0[1-9]|[1-4]\d|5[0-3])$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the oldest and newest manufacturing date in the format YYWWYYWW.
    /// </summary>
    private static readonly Regex OldestAndNewestManufacturingDateRegEx = new(@"^\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))\d{2}((0[1-9])|([1-4]\d)|(5[0-3]))$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for the harvest date range.
    /// </summary>
    private static readonly Regex HarvestDateRangeRegEx = new(@"^(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)(\d{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12]\d|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1\d|2[0-8]))|(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00)[-/]?02[-/]?29)$", RegexOptions.None);

    /// <summary>
    ///    Returns a regular expression for a Uniform Resource Locator (URL).
    /// </summary>
    private static readonly Regex UniformResourceLocatorRegEx = new(@"^(http[s]?:\/\/((([a-zA-Z0-9]([a-zA-Z0-9]|-)*[a-zA-Z0-9]|[a-zA-Z0-9])\\.)*[a-zA-Z]([a-zA-Z0-9]|-)*[a-zA-Z0-9]|[a-zA-Z]|\d+\\.\d+\\.\d+\\.\d+)(:\d+)?(\/([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*(\/([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*)*(\\?([a-zA-Z]|\d|[-$_.+]|[!*'(),]|%\d|A-F|a-f\d|A-F|a-f|;|\\?|&|=)*)?)?)$", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length for CAGE Code and Serial number.
    /// </summary>
    private static readonly Regex AlphanumericCageSnRegEx = new(@"[0-9A-Z-/]{6,25}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2}
    /// </summary>
    private static readonly Regex AlphanumericRegEx2 = new(@"[0-9A-Z]{2}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3}
    /// </summary>
    private static readonly Regex AlphanumericRegEx3 = new(@"[0-9A-Z]{3}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4}
    /// </summary>
    private static readonly Regex AlphanumericRegEx4 = new(@"[0-9A-Z]{4}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5}
    /// </summary>
    private static readonly Regex AlphanumericRegEx5 = new(@"[0-9A-Z]{5}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {6}
    /// </summary>
    private static readonly Regex AlphanumericRegEx6 = new(@"[0-9A-Z]{6}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10}
    /// </summary>
    private static readonly Regex AlphanumericRegEx10 = new(@"[0-9A-Z]{10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {12}
    /// </summary>
    private static readonly Regex AlphanumericRegEx12 = new(@"[0-9A-Z]{12}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {18}
    /// </summary>
    private static readonly Regex AlphanumericRegEx18 = new(@"[0-9A-Z]{18}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 3}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0103 = new(@"[0-9A-Z]{1,3}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 9}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0109 = new(@"[0-9A-Z]{1,9}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 10}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0110 = new(@"[0-9A-Z]{1,10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 20}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0120 = new(@"[0-9A-Z]{1,20}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 35}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0135 = new(@"[0-9A-Z]{1,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 50}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0150 = new("[0-9A-Z]{1,50}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 100}
    /// </summary>
    private static readonly Regex AlphanumericRegEx01100 = new(@"[0-9A-Z]{1,100}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 30}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0230 = new(@"[0-9A-Z]{2,30}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 35}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0235 = new(@"[0-9A-Z]{2,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 9}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0309 = new(@"[0-9A-Z]{3,9}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 22}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0322 = new(@"[0-9A-Z]{3,22}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0335 = new(@"[0-9A-Z]{3,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 11}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0411 = new(@"[0-9A-Z]{4,11}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 25}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0425 = new(@"[0-9A-Z]{4,25}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 29}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0529 = new(@"[0-9A-Z]{5,29}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 16}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0516 = new(@"[0-9A-Z]{5,16}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 22}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0522 = new(@"[0-9A-Z]{5,22}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {6, 35}
    /// </summary>
    private static readonly Regex AlphanumericRegEx0635 = new(@"[0-9A-Z]{6,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 12}
    /// </summary>
    private static readonly Regex AlphanumericRegEx1012 = new(@"[0-9A-Z]{10,12}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 15}
    /// </summary>
    private static readonly Regex AlphanumericRegEx1015 = new(@"[0-9A-Z]{10,15}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {13, 15}
    /// </summary>
    private static readonly Regex AlphanumericRegEx1315 = new(@"[0-9A-Z]{13,15}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {16, 26}
    /// </summary>
    private static readonly Regex AlphanumericRegEx1626 = new(@"[0-9A-Z]{16,26}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4} . Alphanumeric {1, 10}
    /// </summary>
    private static readonly Regex Alphanumeric04Alphanumeric0110RegEx = new(@"[0-9A-Z]{4}.[0-9A-Z]{1,10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 32} Alphanumeric {3} with leading dashes
    /// </summary>
    private static readonly Regex Alphanumeric0132Alphanumeric03WithDashesRegEx = new(@"[0-9A-Z]{1,32}([0-9A-Z]{3}|-[0-9A-Z]{2}|--[0-9A-Z]{1}|---)", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {11} Numeric {2}
    /// </summary>
    private static readonly Regex Alphanumeric11Numeric02 = new(@"[0-9A-Z]{11}\d{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric and dash {1, 6} Numeric and dot {5} Alphanumeric {2}
    /// </summary>
    private static readonly Regex Numeric02AlphanumericDash0106NumericDot05Alphanumeric02RegEx = new(@"\d{2}[A-Z0-9-]{1,6}[0-9.]{5}[0-9A-Z]{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35} + Alpha {1, 3}
    /// </summary>
    private static readonly Regex Alphanumeric0335Alpha0103RegEx = new(@"[0-9A-Z]{3,35}\+[A-Z]{1,3}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 27}
    /// </summary>
    private static readonly Regex Alpha02Alphanumeric0327RegEx = new(@"[A-Z]{2}[0-9A-Z]{3,27}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {3} Numeric {14} Alphanumeric {1, 33}
    /// </summary>
    private static readonly Regex Alpha03Numeric14Alphanumeric0133RegEx = new(@"[A-Z]{3}\d{14}[0-9A-Za-z*+-./()!]{1,33}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {1} Numeric {4} Alphanumeric {5, 20}
    /// </summary>
    private static readonly Regex Alpha01Numeric04Alphanumeric0520RegEx = new(@"[A-Z]{1}\d{4}[0-9A-Z]{5,20}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {1, 8} Alphanumeric {2}
    /// </summary>
    private static readonly Regex Numeric0108Alphanumeric02RegEx = new(@"\d{1,8}[0-9A-Z]{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {1, 10} Alphanumeric {3}
    /// </summary>
    private static readonly Regex Numeric0110Alphanumeric03RegEx = new(@"\d{1,10}[0-9A-Z]{3}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric {3, 42}
    /// </summary>
    private static readonly Regex Numeric02Alphanumeric0342RegEx = new(@"\d{2}[0-9A-Z]{3,42}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 18}
    /// </summary>
    private static readonly Regex Alpha02Alphanumeric0318RegEx = new(@"[A-Z]{2}[0-9A-Z]{3,18}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with space {1, 60}
    /// </summary>
    private static readonly Regex AlphanumericSpace0160RegEx = new(@"[0-9A-Z ]{1,60}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 50}
    /// </summary>
    private static readonly Regex AlphanumericPlus0150RegEx = new(@"[0-9A-Z ]{1,50}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {20, 50}
    /// </summary>
    private static readonly Regex AlphanumericPlus2050RegEx = new(@"[0-9A-Z+]{20,50}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 60}
    /// </summary>
    private static readonly Regex AlphanumericPlus0160RegEx = new(@"[0-9A-Z ]{1,60}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {7}
    /// </summary>
    private static readonly Regex Alpha04Numeric07RegEx = new(@"[A-Z]{4}\d{7}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {1, 3}
    /// </summary>
    private static readonly Regex Alpha04Numeric0103RegEx = new(@"[A-Z]{4}\d{1,3}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {3} Numeric {3}
    /// </summary>
    private static readonly Regex Alpha03Numeric03RegEx = new(@"[A-Z]{3}\d{3}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {2}
    /// </summary>
    private static readonly Regex Alpha02RegEx = new(@"[A-Z]{2}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Invariant {2, 12}
    /// </summary>
    private static readonly Regex Invariant0212RegEx = new(@"[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{2,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {2} Invariant {3, 27}
    /// </summary>
    private static readonly Regex Alpha02Invariant0327RegEx = new(@"[A-Z]{2}[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{3,27}$", RegexOptions.None);
    
    /// <summary>
    ///  Regular Expression: Numeric {1}
    /// </summary>
    private static readonly Regex Numeric01RegEx = new(@"\d{1}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {5}
    /// </summary>
    private static readonly Regex Numeric05RegEx = new(@"\d{5}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {9}
    /// </summary>
    private static readonly Regex Numeric09RegEx = new(@"\d{9}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {14}
    /// </summary>
    private static readonly Regex Numeric14RegEx = new(@"\d{14}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {18}
    /// </summary>
    private static readonly Regex Numeric18RegEx = new(@"\d{18}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {1, 2}
    /// </summary>
    private static readonly Regex Numeric0102RegEx = new(@"\d{1,2}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {4, 6}
    /// </summary>
    private static readonly Regex Numeric0406RegEx = new(@"\d{4,6}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {6, 26}
    /// </summary>
    private static readonly Regex Numeric0626RegEx = new(@"\d{6,26}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {7, 12}
    /// </summary>
    private static readonly Regex Numeric0712RegEx = new(@"\d{7,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {9, 13}
    /// </summary>
    private static readonly Regex Numeric0913RegEx = new(@"\d{9,13}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {10, 12}
    /// </summary>
    private static readonly Regex Numeric1012RegEx = new(@"\d{10,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {13, 14}
    /// </summary>
    private static readonly Regex Numeric1314RegEx = new(@"\d{13,14}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {14, 26}
    /// </summary>
    private static readonly Regex Numeric1426RegEx = new(@"\d{14,26}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 10}
    /// </summary>
    private static readonly Regex NumericDot0110RegEx = new(@"[0-9.]{1,10}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 20}
    /// </summary>
    private static readonly Regex NumericDot0120RegEx = new(@"[0-9.]{1,20}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 05}
    /// </summary>
    private static readonly Regex NumericDot0105RegEx = new(@"[0-9.]{1,5}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 06}
    /// </summary>
    private static readonly Regex NumericDot0106RegEx = new(@"[0-9.]{1,6}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Minus with Numeric {01, 04}
    ///   sign.
    /// </summary>
    private static readonly Regex MinusNumeric0104RegEx = new(@"-?\d{1,4}$", RegexOptions.None);
#endif
    /// <summary>
    ///   A dictionary of data identifier descriptors.
    /// </summary>
    private static readonly IDictionary<int, EntityDescriptor> Descriptors =
        new Dictionary<int, EntityDescriptor>
        {
            {
                0, new EntityDescriptor(
                    "PLUS",

                    // ReSharper disable once StringLiteralTypo
                    "The 'PLUS' character. Health Industry Business Communications Council (HIBCC) ",
#if NET7_0_OR_GREATER
                    HibccPlusRegEx
#else
                    () => HibccPlusRegEx
#endif
                    )
            },
            {
                2, new EntityDescriptor(
                    "AMPERSAND",

                    // ReSharper disable once StringLiteralTypo
                    "The 'AMPERSAND' character. ICCBBA",
#if NET7_0_OR_GREATER
                    IccbbaAmpersandRegEx
#else
                    () => IccbbaAmpersandRegEx
#endif
                )
            },
            {
                3, new EntityDescriptor(
                    "EQUAL",

                    // ReSharper disable once StringLiteralTypo
                    "The 'EQUAL' character. ICCBB",
#if NET7_0_OR_GREATER
                    IccbbaEqualRegEx
#else
                    () => IccbbaEqualRegEx
#endif
                )
            },
            {
                4, new EntityDescriptor(
                    "FUNC1",

                    // ReSharper disable once StringLiteralTypo
                    "Function 1 (FUNC1) character. Appears in the first position following the symbology start character of a Code 128, Code 49, or Code 16K Symbol to signify a GS1-controlled symbol.",
#if NET7_0_OR_GREATER
                    Gs1Function1RegEx
#else
                    () => Gs1Function1RegEx
#endif
                )
            },
            {
                5,
                new EntityDescriptor(
                    "ISOIEC15434PREAMBLE",
                    "'Left Square Bracket', 'Right Parenthesis', 'Greater Than Sign', 'Record Separator' characters. Data structure compliant to ISO/IEC 15434, Information technology - Automatic Identification and Data Capture Techniques - Syntax for High Capacity ADC Media.",
#if NET7_0_OR_GREATER
                    IsoIec15434PreambleRegEx
#else
                    () => IsoIec15434PreambleRegEx
#endif
                )
            },
            {
                6, new EntityDescriptor(
                    "HYPHEN",

                    // ReSharper disable once StringLiteralTypo
                    "The 'HYPHEN' or 'MINUS' character. Pharmaceutical Central Number (PZN), controlled by IFA-ABDATA, Germany (Registration of this system identifier expires on 2016-07-01). Replaced by '9N'.",
#if NET7_0_OR_GREATER
                    IfaAbdataPznHyphenRegEx
#else
                    () => IfaAbdataPznHyphenRegEx
#endif
                )
            },
            {
                7, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "EXCLAMATIONMARK",

                    // ReSharper disable once StringLiteralTypo
                    "The 'EXCLAMATION MARK'. Eurocode-IBLS ",
#if NET7_0_OR_GREATER
                    EurocodeIblsExclamationMarkRegEx
#else
                    () => EurocodeIblsExclamationMarkRegEx
#endif
                )
            },
            {
                2000,
                new EntityDescriptor(
                    "CONTAINER TYPE",
                    "Container Type (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                2001, new EntityDescriptor(
                    "RETURNABLE CONTAINER",

                    // ReSharper disable once StringLiteralTypo
                    "Returnable Container Identification Code assigned by the container owner or the appropriate regulatory agency (e.g., a metal tub, basket, reel, unit load device (ULD), trailer, tank, or intermodal container) (excludes gas cylinders See '2B').",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                2002,
                new EntityDescriptor(
                    "GAS CYLINDER CONTAINER",
                    "Gas Cylinder Container Identification Code assigned by the manufacturer in conformance with U.S. Department of Transportation (D.O.T.) standards.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                2003,
                new EntityDescriptor(
                    "MOTOR FREIGHT TRANSPORT EQUIPMENT",
                    "Motor Freight Transport Equipment Identification Code assigned by the manufacturer in conformance with International Organisation for Standardization (ISO) standards.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () =>                     Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                2004, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "Standard Carrier Alpha Code (SCAC) (4 alphanumeric characters) and an optional carrier assigned trailer number (one to ten alphanumeric characters). When used, the carrier assigned trailer number is separated from the SCAC by a dash '-'.",
#if NET7_0_OR_GREATER
                    Alphanumeric04Alphanumeric0110RegEx
#else
                    () => Alphanumeric04Alphanumeric0110RegEx
#endif
                )
            },
            {
                2005, new EntityDescriptor(
                    "RECEPTACLE ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Receptacle Asset Number - Consisting of two joined parts:\n- Identification of an organisation in accordance with ISO/IEC 15459 and a unique entity identification assigned in accordance with rules established by the issuing agency.\n- A unique serial number assigned by the entity, ending with a 3-character container type code taken from EDIFACT Code List 8053 or UPU standard M82-3. (If the container type code listed is less than three characters in length, the field will be dash '-' filled left to the length of three characters).",
#if NET7_0_OR_GREATER
                    Alphanumeric0132Alphanumeric03WithDashesRegEx
#else
                    () => Alphanumeric0132Alphanumeric03WithDashesRegEx
#endif
                )
            },
            {
                2007,
                new EntityDescriptor(
                    "CONTAINER SERIAL",
                    "Container Serial Number.\nAccording to ISO 6346. OC EI CSN CD, where the OC is the three letter owner code assigned in cooperation with BIC, the EI is the one letter equipment category identifier, the CSN is a 6-digit unique container identification assigned by the equipment owner, and CD is a modulus 11 check digit calculated in accordance with Annex A, ISO 6346.",
#if NET7_0_OR_GREATER
                    Alpha04Numeric07RegEx
#else
                    () => Alpha04Numeric07RegEx
#endif
                )
            },
            {
                2008,
                new EntityDescriptor(
                    "RETURNABLE CONTAINER OWNER ",
                    "Identification of a Returnable Container owner assigned in cooperation with BIC.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx3
#else
                    () => AlphanumericRegEx3
#endif
                )
            },
            {
                2009,
                new EntityDescriptor(
                    "CONTAINER SIZE ",
                    "Container Size/Type Code.\nAccording to ISO 6346, Section 4.2.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx4
#else
                    () => AlphanumericRegEx4
#endif
                )
            },
            {
                2010,
                new EntityDescriptor(
                    "CONTAINER OWNERSHIP",
                    "Container Ownership Code. Actual four-character abbreviation marked on the container by the owner. For DOD owned containers see Defense Transportation Regulation App EE-6.\n2020 Update: data source reference updated to the Defense Transportation Regulation, Part II, App TT located at:\nhttps://www.ustranscom.mil/dtr/dtrp2.cfm",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx4
#else
                    () => AlphanumericRegEx4
#endif
                )
            },
            {
                2011,
                new EntityDescriptor(
                    "VAN ",
                    "Van Number (complete number minus check digit).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                2012,
                new EntityDescriptor(
                    "VAN CHECK DIGIT",
                    "Check digit of Van Number identified in 11B.",
#if NET7_0_OR_GREATER
                    Numeric01RegEx
#else
                    () => Numeric01RegEx
#endif
                )
            },
            {
                2013,
                new EntityDescriptor(
                    "CONTAINER ",
                    "Container Number Code (last 5 digits of number not counting check digit).",
#if NET7_0_OR_GREATER
                    Numeric05RegEx
#else
                    () => Numeric05RegEx
#endif
                )
            },
            {
                2014,
                new EntityDescriptor(
                    "TAG STATUS ",
                    "Tag Status.\nY=Authorized / N=Unauthorized.",
#if NET7_0_OR_GREATER
                    YesNoLetterRegEx
#else
                    () => YesNoLetterRegEx
#endif
                )
            },
            {
                2015, new EntityDescriptor(
                    "DANGEROUS CARGO CLASS ",

                    // ReSharper disable once StringLiteralTypo
                    "Dangerous Cargo Class.\nIMDG Class in the format 'n.na' where n = numeric, decimal point expressly encoded, and a = conditional alphabetic qualifier. http://docs.imo.org/.",
#if NET7_0_OR_GREATER
                    DangerousCargoClassRegEx
#else
                    () => DangerousCargoClassRegEx
#endif
                )
            },
            {
                2016,
                new EntityDescriptor(
                    "DANGEROUS GOODS ",
                    "UN Code for Dangerous Goods.\nFor dangerous cargo provided by shipper in accordance with UN Code.\nwww.unece.org/trans/danger/publi/unrec/English/part3.pdf\n2020 Update: URL changed to http://www.unece.org/trans/danger/danger.html",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx4
#else
                    () => AlphanumericRegEx4
#endif
                )
            },
            {
                2017,
                new EntityDescriptor(
                    "TRANSPORTATION SUBJECT ",
                    "Name Of Transportation Subject.\nVessel name or vehicle code/train trip number in English.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                2018, new EntityDescriptor(
                    "VESSEL REGISTRATION ",

                    // ReSharper disable once StringLiteralTypo
                    "Vessel Registration Number.\nThe three letters 'IMO' followed by the seven-digit number assigned to all ships by IHS Fairplay when constructed. http://www.imonumbers.lrfairplay.com/.",
#if NET7_0_OR_GREATER
                    VesselRegistrationNumberRegEx
#else
                    () => VesselRegistrationNumberRegEx
#endif
                )
            },
            {
                2019,
                new EntityDescriptor(
                    "VOYAGE",
                    "Voyage number/Trip number.\nLetter and number.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx18
#else
                    () => AlphanumericRegEx18
#endif
                )
            },
            {
                2020,
                new EntityDescriptor(
                    "VESSEL COUNTRY ",
                    "Vessel Country.\nISO 3166-1 Alpha 2 Code.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx2
#else
                    () => AlphanumericRegEx2
#endif
                )
            },
            {
                2021,
                new EntityDescriptor(
                    "ELECTRONIC SEAL",
                    "Reserved for Electronic Seal Numbers.\nComprised of the 18185-1 seal tag ID - 32 bits and the ISO 14816 16-bit manufacturers ID (ISO 646).",
#if NET7_0_OR_GREATER
                    ElectronicSealNumbersRegEx
#else
                    () => ElectronicSealNumbersRegEx
#endif
                )
            },
            {
                2022, new EntityDescriptor(
                    "ENTRY",

                    // ReSharper disable once StringLiteralTypo
                    "Entry Number/Type.\nComprised of the three-digit filer code, followed by the sevendigit entry number, and completed with the one digit check digit. Entry Filer Code represents the three-character alphanumeric filer code assigned to the filer or importer by CBP. Entry Number represents the seven-digit number assigned by the filer. The number may be assigned in any manner convenient, provided that the same number is not assigned to more than one CBP Form 7501. Leading zeros must be shown. Check Digit is computed on the previous 10 characters. The formula for calculating the check digit can be found in Appendix 1, CBP 7501 Instructions.\nEntry type is a two-digit code compliant to Block 2, CBP 7501 Instructions.",
#if NET7_0_OR_GREATER
                    Alphanumeric11Numeric02
#else
                    () => Alphanumeric11Numeric02
#endif
                )
            },
            {
                2023, new EntityDescriptor(
                    "SURETY ",

                    // ReSharper disable once StringLiteralTypo
                    "Surety Number.\nThe three-digit numeric code that identifies the surety company on the Customs Bond. This code can be found in block 7 of the CBP Form 301, or is available through CBP's automated system to ABI filers, via the importer bond query transaction. For U.S. Government importations and entry types not requiring surety, code 999 should appear in this block. When cash or Government securities are used in lieu of surety, use code 998.",
#if NET7_0_OR_GREATER
                    SuretyNumberRegEx
#else
                    () => SuretyNumberRegEx
#endif
                )
            },
            {
                2024,
                new EntityDescriptor(
                    "FOREIGN PORT OF LADING ",
                    "Foreign Port of Lading.\n'Schedule K' (Classification of Foreign Ports by Geographic Trade Area and Country) for the foreign port at which the merchandise was actually laden on the vessel that carried the merchandise to the U.S. http://www.navigationdatacenter.us/wcsc/scheduleK/schedule k.htm.\n2020 Update: URL changed to: https://www.cbp.gov/sites/default/files/assets/documents/2017-Feb/appendix_f_0.pdf",
#if NET7_0_OR_GREATER
                    ForeignPortOfLadingRegEx
#else
                    () => ForeignPortOfLadingRegEx
#endif
                )
            },
            {
                2025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION ",
                    "Identification of a Party to a Transaction as defined in ISO 17364, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the RTI serial number that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                2026, new EntityDescriptor(
                    "UNIQUE RETURNABLE TRANSPORT ITEM ",

                    // ReSharper disable once StringLiteralTypo
                    "Unique Returnable Transport Item Identifier comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'RTI Number' (RTIN), followed by the '+' character, followed by the supplier assigned (or managed) 'RTI Serial Number' (RTISN) that is globally unique within the CIN holder's domain; in the format IAC CIN RTIN + RTISN (spaces added for visual clarity only; they are not part of the data). See Annex C.11.",
#if NET7_0_OR_GREATER
                    AlphanumericWithPlus01UnboundRegEx
#else
                    () => AlphanumericWithPlus01UnboundRegEx
#endif
                )
            },
            {
                2027, new EntityDescriptor(
                    "LLC RTI ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Asset Identifier of a Large Load Carrier (LLC) Returnable Transport Item (RTI) with a side  base of = 1000 mm, as defined in ISO 17365:2013, tertiary packaging, layer 3 comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by RTI Type Code 'RTITC', followed by the '+' character, followed by the owner assigned (or managed) RTI Serial Number 'RTISN' that is globally unique within the CIN holder's domain in the format IAC CIN RTITC + RTISN (spaces added for visual clarity only; they are not part of the data).",
#if NET7_0_OR_GREATER
                    AlphanumericPlus2050RegEx
#else
                    () => AlphanumericPlus2050RegEx
#endif
                )
            },
            {
                2028, new EntityDescriptor(
                    "SLC RTI ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Asset Identifier of a Small Load Carrier (SLC) Returnable Transport Item with a side base of < 1000 mm, as defined in ISO 17364:2013 (RTI), tertiary packaging, layer 2  comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by RTI Type Code 'RTITC', followed by the '+' character, followed by the owner assigned (or managed) RTI Serial Number 'RTISN' that is globally unique within the CIN holder's domain in the format IAC CIN RTITC + RTISN (spaces added for visual clarity only; they are not part of the data).",
#if NET7_0_OR_GREATER
                    AlphanumericPlus2050RegEx
#else
                    () => AlphanumericPlus2050RegEx
#endif
                )
            },
            {
                2029, new EntityDescriptor(
                    "RPI IDENTIFIER",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Returnable Packaging Item (RPI) identifier of the category packaging aid (lid, blister, inlay, ...) comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'RPI Number' RPIN, followed by the '+' character, followed by the owner assigned (or managed) 'RPI Serial Number' RPISN that is globally unique within the CIN holder's domain in the format IAC CIN RPIN + RPISN (spaces added for visual clarity only; they are not part of the data).",
#if NET7_0_OR_GREATER
                    AlphanumericPlus0150RegEx
#else
                    () => AlphanumericPlus0150RegEx
#endif
                )
            },
            {
                2030,
                new EntityDescriptor(
                    "PACKAGING ITEM",
                    "Packaging Item Number.\nNumber to identify the type of packaging item (material) used when packing products and packages.\nThe number will enable packaging item (material) be identified and separated from products, packages, Returnable Transport Items (RTIs) and Returnable Packaging Items (RPIs) during packing.\nThe number is constructed as a sequence of minimum 1 data element:\nPackaging item (material) number that is unique within the holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0235
#else
                    () => AlphanumericRegEx0235
#endif
                )
            },
            {
                2031,
                new EntityDescriptor(
                    "PACKAGING ",
                    "Global Unique Packaging Number\nGlobal unique number to identify the type of packaging item (material) used when packing products and packages.\nThe global unique number will enable packaging items (materials) be identified and separated from products, packages, Returnable Transport Items (RTIs) and Returnable Packaging Items (RPIs) during packing.\nThe number is constructed as a sequence of 3 concatenated data elements:\nThe IAC, followed by the CIN, followed by the Packaging item (material) number that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0635
#else
                    () => AlphanumericRegEx0635
#endif
                )
            },
            {
                2055,
                new EntityDescriptor(
                    "RPI",
                    "Global Unique Returnable Packaging Item (RPI) as defined in ISO 17364, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the RPI serial number that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0150
#else
                    () => AlphanumericRegEx0150
#endif
                )
            },
            {
                3000,
                new EntityDescriptor(
                    "CUSTOMER ITEM CODE CONT.",
                    "Continuation of an Item Code (Category 16) assigned by Customer that is too long for a required field size.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                3001,
                new EntityDescriptor(
                    "SUPPLIER TRACEABILITY CODE CONT.",
                    "Continuation of Traceability Code (Category 20) assigned by Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                3002,
                new EntityDescriptor(
                    "SUPPLIER FREE TEXT CONT.",
                    "Continuation of Serial Number (Category 19) assigned by Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                3003,
                new EntityDescriptor(
                    "FREE TEXT CONT.",
                    "Continuation of Free Text (Category 26) mutually defined between Supplier/Carrier/Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                3004,
                new EntityDescriptor(
                    "TRANSACTION REFERENCE CONT.",
                    "Continuation of Transaction Reference (Category 11) mutually defined between Supplier/Carrier/Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                3005,
                new EntityDescriptor(
                    "SUPPLIER ITEM CODE CONT.",
                    "Continuation of Item Code (Category 16) Assigned by Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                4000, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYMMDD.",
#if NET7_0_OR_GREATER
                    DatePatternYyMmDdZerosRegEx
#else
                    () => DatePatternYyMmDdZerosRegEx
#endif
                )
            },
            {
                4001, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format DDMMYY.",
#if NET7_0_OR_GREATER
                    DatePatternDdMmYyRegEx
#else
                    () => DatePatternDdMmYyRegEx
#endif
                )
            },
            {
                4002, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format MMDDYY.",
#if NET7_0_OR_GREATER
                    DatePatternMmDdYyRegEx
#else
                    () => DatePatternMmDdYyRegEx
#endif
                )
            },
            {
                4003, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YDDD (Julian).",
#if NET7_0_OR_GREATER
                    DatePatternYDddJulianRegEx
#else
                    () => DatePatternYDddJulianRegEx
#endif
                )
            },
            {
                4004, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYDDD (Julian).",
#if NET7_0_OR_GREATER
                    DatePatternYyDddJulianRegEx
#else
                    () => DatePatternYyDddJulianRegEx
#endif
                )
            },
            {
                4005, new EntityDescriptor(
                    "DATE AND TYPE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO format YYMMDD immediately followed by an ANSI X12.3 Data Element Number 374 Qualifier providing a code specifying type of date (e.g., ship date, manufacture date).",
#if NET7_0_OR_GREATER
                    DatePatternYyMmDdRegEx
#else
                    () => DatePatternYyMmDdRegEx
#endif
                )
            },
            {
                4006, new EntityDescriptor(
                    "DATE AND TYPE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO format YYYYMMDD immediately followed by an ANSI X12.3 Data Element Number 374 Qualifier providing a code specifying type of date (e.g., ship date, manufacture date).",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4007, new EntityDescriptor(
                    "MONTH",

                    // ReSharper disable once StringLiteralTypo
                    "Format MMYY.",
#if NET7_0_OR_GREATER
                    FormatMmYyRegEx
#else
                    () => FormatMmYyRegEx
#endif
                )
            },
            {
                4008,
                new EntityDescriptor(
                    "EVENT, DATE AND TIME ",
                    "Event, Date, And Time.\nISO format YYYYMMDDHHMM (24 hour clock - UTC) immediately followed by a UN/EDIFACT Code Qualifier 2005 providing a code specifying type of date), e.g.,\n11 [Date when goods are expected to be dispatched/shipped message is issued.];\n17 [Estimated delivery date/time when goods are expected to be delivered];\n35 [Date on which goods are delivered to their destination.];\n118 [Booking Confirmed];\n129 [Date when the vessel/merchandise departed the last foreign port in the exporting country.];\n132 [Date/time when the carrier estimates that a means of transport should arrive at the port of discharge or place of destination.];\n133 [Date/time when carrier estimates that a means of transport should depart at the place of departure];\n137 [Date/time when the supplier ships parts based on the customer's request. (Date when DESADV message is issued. Recommendation is the DESADV is issued within 30 minutes of goods being picked up at ShipFrom party];\n146 [Estimated Entry date (Customs) date on which the official date of a Customs Entry is anticipated.];\n151 [Import Date (Arrived at port with intent to unlade];\n186 Departs a Facility ('Gate-out)];\n204 [Date on which Customs releases merchandise to the carrier or importer];\n253 [Departs from a Port ('Vessel Departure')];\n252 [Arrives at a Port ('Vessel Arrival')];\n283 [Arrives at a Facility ('Gate-in)];\n342 [Conveyance Loaded];\n351 [Terminal Gate Inspection];\n411 [Ordered Stuffed];\n412 [Ordered Stripped];\n420 [Conveyance unloaded];\n534 [Repaired];\n677 [Confirmed Stuffed];\n678 [Confirmed Stripped];\n696 [Filing Date].",
#if NET7_0_OR_GREATER
                    EventDateAndTimeRegEx
#else
                    () => EventDateAndTimeRegEx
#endif
                )
            },
            {
                4009,
                new EntityDescriptor(
                    "DATE",
                    "Date (structure and significance mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                4010, new EntityDescriptor(
                    "WEEK",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYWW.",
#if NET7_0_OR_GREATER
                    DatePatternYyWwRegEx
#else
                    () => DatePatternYyWwRegEx
#endif
                )
            },
            {
                4011,
                new EntityDescriptor(
                    "WEEK",
                    "Format YYYYWW.",
#if NET7_0_OR_GREATER
                    FormatYyyyWwRegEx
#else
                    () => FormatYyyyWwRegEx
#endif
                )
            },
            {
                4012, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYYYMMDD.",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4013, new EntityDescriptor(
                    "OLDEST AND NEWEST MANUFACTURING DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Oldest and Newest Manufacturing Date in the format YYWWYYWW.",
#if NET7_0_OR_GREATER
                    OldestAndNewestManufacturingDateRegEx
#else
                    () => OldestAndNewestManufacturingDateRegEx
#endif
                )
            },
            {
                4014, new EntityDescriptor(
                    "EXPIRATION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Expiration Date (YYYYMMDD).",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4015, new EntityDescriptor(
                    "EXPIRATION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Expiration Date (DDMMYYYY).",
#if NET7_0_OR_GREATER
                    DatePatternDdMmYyyyyRegEx
#else
                    () => DatePatternDdMmYyyyyRegEx
#endif
                )
            },
            {
                4016, new EntityDescriptor(
                    "DATE OF MANUFACTURE ",

                    // ReSharper disable once StringLiteralTypo
                    "Production Date (YYYYMMDD) - Date of manufacture.",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4017, new EntityDescriptor(
                    "PRODUCTION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Production Date (DDMMYYYY).",
#if NET7_0_OR_GREATER
                    DatePatternDdMmYyyyyRegEx
#else
                    () => DatePatternDdMmYyyyyRegEx
#endif
                )
            },
            {
                4018,
                new EntityDescriptor(
                    "TAG ACTIVATION TIME",
                    "Tag Activation Time.\nYYYYMMDDHHMM (24 hour clock - UTC).",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdHhMmRegEx
#else
                    () => DatePatternYyyyMmDdHhMmRegEx
#endif
                )
            },
            {
                4019,
                new EntityDescriptor(
                    "TAG DEACTIVATION TIME ",
                    "Tag Deactivation Time.\nYYYYMMDDHHMM (24 hour clock - UTC).",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdHhMmRegEx
#else
                    () => DatePatternYyyyMmDdHhMmRegEx
#endif
                )
            },
            {
                4020, new EntityDescriptor(
                    "INSPECTION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Inspection Date (DDMMYYYY).",
#if NET7_0_OR_GREATER
                    DatePatternDdMmYyyyyRegEx
#else
                    () => DatePatternDdMmYyyyyRegEx
#endif
                )
            },
            {
                4021,
                new EntityDescriptor(
                    "DOD MILSTAMP CODE",
                    "Required Delivery Date (DDD Julian) or DOD MILSTAMP Code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                4022,
                new EntityDescriptor(
                    "RECORD TIME",
                    "Record Time.\nYYYYMMDDHHMM (24 hour clock - UTC).",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdHhMmRegEx
#else
                    () => DatePatternYyyyMmDdHhMmRegEx
#endif
                )
            },
            {
                4023, new EntityDescriptor(
                    "UTC DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Date, represented in modified UTC compliant form:\nyyyy[mm[dd[hh[mm[ss[fff]]]]]][poooo] where square brackets indicate optionality and yyyy is the year, mmdd the month and day, hhmmss the time of day in hours minutes and seconds, fff the fractions of sections and poooo the offset from UTC expressed in hours and minutes, the offset being positive if p is a point (.), negative if P is a minus sign (-).\nEXAMPLE:\n2005 - (UTC) calendar year 2005;\n200505 - (UTC) calendar month May 2005;\n20050518 - (UTC) 18 May 2005;\n200505181247 - 12:47 UTC on 18 May 2005;\n200505181247.0100 - 12:47 local time, being 11:47 UTC, on 18 May 2005;\n20050518124723099 - 99 milliseconds after UTC 12:47:23 on 18 May 200.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                4024,
                new EntityDescriptor(
                    "QUALIFIED DATE",
                    "Qualified Date, comprising the concatenation of:\n- an ISO/IEC 15459 issuing agency code;\n- a date qualifier conforming to the specifications of that issuing agency;\n- a date whose format and interpretation comply with the specifications of the issuing agency for that date qualifier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                4025, new EntityDescriptor(
                    "BEST BEFORE DATE", 
                    "Best before date: (YYYYMMDD).\nExample: 25D20170202 = February 2, 2017",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4026, new EntityDescriptor(
                    "FIRST FREEZE DATE",

                    // ReSharper disable once StringLiteralTypo
                    "First freeze date (YYYYMMDD).\nThe first freeze date is defined as the date on which products are frozen directly after slaughtering, harvesting, catching or after initial processing.\nExample: 26D20170721 = July 21, 2017.",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4027, new EntityDescriptor(
                    "HARVEST DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Harvest date (YYYYMMDD).\nThe date when an animal was slaughtered or killed, a fish has been harvested, or a crop was harvested.\nExample: 27D20170615 = June 15, 2017.",
#if NET7_0_OR_GREATER
                    DatePatternYyyyMmDdRegEx
#else
                    () => DatePatternYyyyMmDdRegEx
#endif
                )
            },
            {
                4028,
                new EntityDescriptor(
                    "HARVEST DATE RANGE",
                    "Harvest date range (YYYYMMDDYYYYMMDD).\nThe start date and end date range over which harvesting occurred.\nFor example; animals were slaughtered or killed, fish were harvested, or a crop was harvested.\nThe data stream is defined as the first YYYYMMDD as the start date and the last YYYYMMDD as the end date.\nExample:\n28D2017012320170214 = Start; January 23, 2017. End; February 14, 2017.",
#if NET7_0_OR_GREATER
                    HarvestDateRangeRegEx
#else
                    () => HarvestDateRangeRegEx
#endif
                )
            },
            {
                5000,
                new EntityDescriptor(
                    "RESTRICTED SUBSTANCES CLASSIFICATION",
                    "Restricted Substances Classification - 'Environmental Classification Code' including Lead-Free (Pb-Free) finish categories defined in JESD97 (IPC JEDEC J-STD-609), and future industry or governmental agency assigned codes related to environmental regulatory compliance and hazardous material content.",
#if NET7_0_OR_GREATER
                    Alpha02RegEx
#else
                    () => Alpha02RegEx
#endif
                )
            },
            {
                5001,
                new EntityDescriptor(
                    "AIR PRESSURE",
                    "Air Pressure - (altitude) expressed in Pascal's as the standard international measure.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                5002,
                new EntityDescriptor(
                    "MAXIMUM ALLOWED TEMPERATURE",
                    "Maximum Allowed Temperature. Maximum permitted temperature; Degrees Celsius, '-' (minus) encoded, if required.",
#if NET7_0_OR_GREATER
                    MinusNumeric0104RegEx
#else
                    () => MinusNumeric0104RegEx
#endif
                )
            },
            {
                5003,
                new EntityDescriptor(
                    "MINIMUM ALLOWED TEMPERATURE",
                    "Minimum Allowed Temperature.\nMinimum permitted temperature; Degrees Celsius, '-' (minus) encoded, if required.",
#if NET7_0_OR_GREATER
                    MinusNumeric0104RegEx
#else
                    () => MinusNumeric0104RegEx
#endif
                )
            },
            {
                5004,
                new EntityDescriptor(
                    "MAXIMUM ALLOWED RELATIVE HUMIDITY",
                    "Maximum Allowed Relative Humidity.\nMaximum permitted relative humidity, implied as percent.",
#if NET7_0_OR_GREATER
                    Numeric0102RegEx
#else
                    () => Numeric0102RegEx
#endif
                )
            },
            {
                5005,
                new EntityDescriptor(
                    "MINIMUM ALLOWED RELATIVE HUMIDITY",
                    "Minimum Allowed Relative Humidity.\nMaximum permitted relative humidity, expressed as percent.",
#if NET7_0_OR_GREATER
                    Numeric0102RegEx
#else
                    () => Numeric0102RegEx
#endif
                )
            },
            {
                5006,
                new EntityDescriptor(
                    "REFRIGERATOR CONTAINER TEMPERATURE",
                    "Refrigerator Container Temperature.\nFor temperature-controlled cargo, target specified by shipper, Degrees Celsius, '-' (minus) encoded, if required.",
#if NET7_0_OR_GREATER
                    MinusNumeric0104RegEx
#else
                    () => MinusNumeric0104RegEx
#endif
                )
            },
            {
                5010,
                new EntityDescriptor(
                    "CUMULATIVE TIME TEMPERATURE INDEX",
                    "Cumulative Time Temperature Index - expressed as the number of measurements or counts.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                5011,
                new EntityDescriptor(
                    "TIME TEMPERATURE INDEX",
                    "Time Temperature Index - Next Higher Assembly - expressed as the number of measurements or counts.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                5012, new EntityDescriptor(
                    "PACKAGING MATERIAL",

                    // ReSharper disable once StringLiteralTypo
                    "Declaration of Packaging Material Category*, Code* and Weight for a given packaging material used in a given packaging according to the EU packaging and packaging waste directive. (Material category and code defined in Annex M).\n12ECCMMMMMMNNNNNUU where:  - '12E' (an3) is the Data Identifier;\n  - 'CC' (n2) is the Material Category per Annex M;\n  - 'MMMMMM' (an1...6) is the Material Code per Annex M;\n  - 'NNNNN' (n5) Material Weight, including decimal point (e.g., 12.12);\n - 'UU' (an2) is the Unit of measure for weight (e.g., KG, GR, LB or OZ per ANSI X12.3 as in Annex D).",
#if NET7_0_OR_GREATER
                    Numeric02AlphanumericDash0106NumericDot05Alphanumeric02RegEx
#else
                    () => Numeric02AlphanumericDash0106NumericDot05Alphanumeric02RegEx
#endif
                )
            },
            {
                5013,
                new EntityDescriptor(
                    "MSL",
                    "The data following DI 13E will be one of the MSL indicators (1, 2, 2a, 3, 4, 5, 5a, 6) as shown in the LEVEL column in Table 5-1 of JEDEC standard IPC/JEDEC J-STD-020E. The Table is shown below for reference only; the currently released version of the referenced standard shall be used to obtain the correct MSL for the actual component. Example: 13E2a",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                6000,
                new EntityDescriptor(
                    "LOOPING HEADER", 
                    "Looping Header.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                6001,
                new EntityDescriptor(
                    "PARENT",
                    "My parent ______ is . . . Unique identifier followed by a Data Identifier and associated data (for use with returnable packaging). This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                6003,
                new EntityDescriptor(
                    "NUMBER OF CHILDREN",
                    "I have ______ children . . . (for use with returnable packaging, e.g.; 3F10, for ten children). This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                6004,
                new EntityDescriptor(
                    "LOGICAL ASSIGNMENT",
                    "Logical Assignment of a Page of Information within a group of pages that are spread across several data carriers, structured as a sequence of up to three (3) concatenated data elements, separated by a slash ( / ) :\n  Page number (required), followed by page count (optional, required for the last page), followed by an alphanumeric group ID (optional; if used then required for all pages and structured in accordance with ISO/IEC 15459-3 as a sequence of 3 data elements: Issuing Agency Code, followed by the Company Identification Number, followed by an alphanumeric code unique within the issuer's domain).\n Trailing slashes are optional.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                6005,
                new EntityDescriptor(
                    "CHILDREN",
                    "I have ______ children and they are . . . (for use with returnable packaging) This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8000, new EntityDescriptor(
                    "NAME",

                    // ReSharper disable once StringLiteralTypo
                    "Name of Party.\nName of a party followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3035 'Party Qualifier', e.g.;\nBY [Buyer];\nCF   [Container operator];\nCN [Consignee];\nCS [Consolidator];\nDEI  [Vessel operator/captain of vessel];\nFA [Operational staff code];\nIM [Importer];\nMF   [Manufacturer];\nOS [Shipper];\nSE [Seller];\nST [Ship To];\nUC [Ultimate consignee].",
#if NET7_0_OR_GREATER
                    AlphanumericPlus0160RegEx
#else
                    () => AlphanumericPlus0160RegEx
#endif
                )
            },
            {
                8001,
                new EntityDescriptor(
                    "EMPLOYEE ID",
                    "Employee Identification Code assigned by employer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8002,
                new EntityDescriptor(
                    "SOCIAL SECURITY NUMBER",
                    "U.S. Social Security Number.",
#if NET7_0_OR_GREATER
                    Numeric09RegEx
#else
                    () => Numeric09RegEx
#endif
                )
            },
            {
                8003,
                new EntityDescriptor(
                    "NON-EMPLOYEE ID",
                    "ID Number for Non-Employee (internally assigned or mutually defined) (e.g., contract workers, vendors, service, and delivery personnel).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8004,
                new EntityDescriptor(
                    "NATIONAL SOCIAL SECURITY NUMBER",
                    "National Social Security Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            { 
                8005, new EntityDescriptor(
                "LAST NAME", 
                "Last Name.",
#if NET7_0_OR_GREATER
                Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8006,
                new EntityDescriptor(
                    "PARTY NAME", 
                    "Party Name (Line 2).", 
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                8007, new EntityDescriptor(
                    "CONTACT PHONE",

                    // ReSharper disable once StringLiteralTypo
                    "Contact Phone.\nCountry Code, Area Code, Exchange, number [XX YYY ZZZ ZZZZ].",
#if NET7_0_OR_GREATER
                    Numeric1012RegEx
#else
                    () => Numeric1012RegEx
#endif
                )
            },
            {
                8008,
                new EntityDescriptor(
                    "CONTACT EMAIL", 
                    "Contact Email.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0335
#else
                    () => AlphanumericRegEx0335
#endif
                )
            },
            {
                8009, new EntityDescriptor(
                    "CONSIGNEE NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Consignee Number.\nThe unique identifying number can be the IRS, EIN, SSN, or the CBP assigned number, as required on the Security Filing. Only the following formats shall be used: IRS EIN: NN-NNNNNNN;\nIRS EIN w/ suffix:\nNN-NNNNNNNXX;\nSSN:   NNN-NN-NNNN;\nCBP assigned nbr: YYDDPP-NNNNN.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx1012
#else
                    () => AlphanumericRegEx1012
#endif
                )
            },
            {
                8010,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Personal Identification Code (first initial, Last initial, last four of SSN).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8011,
                new EntityDescriptor(
                    "FIRST NAME AND MIDDLE INITIAL",
                    "First Name and Middle Initial.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8012,
                new EntityDescriptor(
                    "MILITARY GRADE",
                    "Military Grade (E1-E9, W1-W5, and O1-O10).\n2020 Update: Metadata format has been changed to agree with the actual officer grades in use by the military: “an2+an3”. The Explanation information has been changed to: “Military Grade (E1-E9, W1-W5, and O1-O11).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx3
#else
                    () => AlphanumericRegEx3
#endif
                )
            },
            {
                8015, new EntityDescriptor(
                    "NI NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "A National Identification Number, National Identity Number, or National Insurance Number used as a means of identifying individuals within a country for the purposes of work, taxation, government benefits, health care, and other governmentally-related functions.\nThis structure of the identifier is DI (15H) followed by the ISO 3166-1 Alpha2 Country Code followed by the predominant government assigned identification code for individuals.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0322
#else
                    () => AlphanumericRegEx0322
#endif
                )
            },
            {
                8025,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Globally Unique Personal ID. assigned by a holder of a Company Identification Code (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as sequence of 3 concatenated data elements: IAC followed by CIN, followed by the ID unique within the holder's domain.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                8026,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Globally Unique Personal ID, with a \"Party Qualifier\" code value from EDIFACT Code List 3035, assigned by a holder of a Company Identification Code (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 5 concatenated data elements: IAC followed by CIN, followed by an ID unique within the CIN holder\'s domain, followed by the Plus character (+) and a code value from EDIFACT Code List 3035 \"Party Qualifier.\", e.g.:\nBG Employer\nGP Packer\nLK Patient\nLL Patient companion\nLM Medical treatment executant\nMF Manufacturer of goods\nExample: 26HLHHIBC987XY65+LK",
#if NET7_0_OR_GREATER
                    Alphanumeric0335Alpha0103RegEx
#else
                    () => Alphanumeric0335Alpha0103RegEx
#endif
                )
            },
            {
                9000,
                new EntityDescriptor(
                    "VIN",
                    "Exclusive Assignment - Vehicle Identification Number (VIN) as defined in the U.S. under 49 CFR, Section Section  565 and internationally by ISO 3779. (These are completely compatible data structures).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                9002,
                new EntityDescriptor(
                    "ABBREVIATED VIN ",
                    "Abbreviated VIN Code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                9004,
                new EntityDescriptor(
                    "TRANSPORT VEHICLE IDENTIFIER ",
                    "Globally unique transport vehicle identifier (e.g., Trucks) consisting of the Vehicle Identification Number (VIN) as defined in the U.S. under 49 CFR Section Section  565, and internationally by ISO 3779, followed by the '+' character, then followed by the government-issued Vehicle Registration License Plate Number in the form of '4I' 'VIN' '+' 'government-issued Vehicle Registration License Plate Number' (quotes and spaces shown for clarity only; they are not part of the data).",
#if NET7_0_OR_GREATER
                    AlphanumericWithPlus01UnboundRegEx
#else
                    () => AlphanumericWithPlus01UnboundRegEx
#endif
                )
            },
            {
                9005, new EntityDescriptor(
                    "PRODUCTION VEHICLE IDENTIFIER ",

                    // ReSharper disable once StringLiteralTypo
                    "Unique production vehicle identifier that will be used during the vehicle production processes, consisting of the Body Tag Number (BTN; or any other descriptor used to identify the raw car body, or stated another way, the assemblage of parts that are used to start the vehicle’s production), followed by the “+” character, then followed by the Production Order Number (PON), followed by the “+” character, and then followed by the Manufacturer-assigned Serial Number (SN). NOTE – The SN component shall be replaced by the VIN as soon as the VIN is available in the assembly process.\nThe construction will be as follows;\n\"5I” “BTN” “+” “PON” “+” “SN”\nchanging to (when VIN available)\n“5I” “BTN” “+” “PON” “+” “VIN”\nNOTE – Quotes and spaces are shown for clarity only; they are not part of the data.\nNOTE – This DI is never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character.\nExamples:\nSN version: 5IABCD1234+CO1234+W0L201600500001\nVIN version: 5IABCD1234+CO1234+W0L0XAP68F4050901",
#if NET7_0_OR_GREATER
                    AlphanumericWithPlus01UnboundRegEx
#else
                    () => AlphanumericWithPlus01UnboundRegEx
#endif
                )
            },
            {
                10000,
                new EntityDescriptor(
                    "LICENSE PLATE",
                    "Unique license plate number.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                10001,
                new EntityDescriptor(
                    "UNBREAKABLE UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which is the lowest level of packaging, the unbreakable unit.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                10002,
                new EntityDescriptor(
                    "TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which contains multiple packages.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                10003,
                new EntityDescriptor(
                    "EDI UNBREAKABLE UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which is the lowest level of packaging, the unbreakable unit and which has EDI data associated with the unit.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                10004,
                new EntityDescriptor(
                    "EDI TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which contains multiple packages and which is associated with EDI data.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                10005,
                new EntityDescriptor(
                    "MIXED TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a mixed transport unit containing unlike items on a single customer transaction and may or may not have associated EDI data.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                )
            },
            {
                10006,
                new EntityDescriptor(
                    "MASTER TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a master transport unit containing like items on a single customer transaction and may or may not have associated EDI data.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                )
            },
            {
                10007,
                new EntityDescriptor(
                    "VEHICLE REGISTRATION LICENSE PLATE NUMBER",
                    "Vehicle Registration License Plate Number (not unique without identification of country and issuing governmental region/authority)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                10008, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MMSI",

                    // ReSharper disable once StringLiteralTypo
                    "Maritime Mobile Service Identity (MMSI).\nA nine digit number regulated by the International Telecommunications Union to uniquely identify a ship or a coast radio station. Example:  8J211123456.",
#if NET7_0_OR_GREATER
                    Numeric09RegEx
#else
                    () => Numeric09RegEx
#endif
                )
            },
            {
                11000,
                new EntityDescriptor(
                    "CUSTOMER ORDER NUMBER",
                    "Order Number assigned by Customer to identify a Purchasing Transaction (e.g., purchase order number).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11001,
                new EntityDescriptor(
                    "SUPPLIER ORDER NUMBER",
                    "Order Number assigned by Supplier to identify a Purchasing Transaction.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11002,
                new EntityDescriptor(
                    "SUPPLIER BILL OF LADING",
                    "Bill of Lading/Waybill/Shipment Identification Code assigned by Supplier/Shipper.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11003,
                new EntityDescriptor(
                    "CARRIER BILL OF LADING",
                    "Bill of Lading/Waybill/Shipment Identification Code assigned by Carrier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11004,
                new EntityDescriptor(
                    "ORDER LINE",
                    "Line Number of the order assigned by Customer to identify a Purchasing Transaction.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11005,
                new EntityDescriptor(
                    "REFERENCE NUMBER",
                    "Reference Number assigned by the Customer to identify a Shipment Authorization (Release) against an established Purchase Order.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11006,
                new EntityDescriptor(
                    "PRO#", 
                    "PRO# Assigned by Carrier.", 
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11007,
                new EntityDescriptor(
                    "CARRIER MODE",
                    "Carrier Mode in Free Text format mutually defined between Customer and Supplier (e.g., Air, Truck, Boat, Rail).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11008,
                new EntityDescriptor(
                    "CONTRACT NUMBER", 
                    "Contract Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11009,
                new EntityDescriptor(
                    "TRANSACTION REFERENCE",
                    "Generic Transaction Reference Code (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11010,
                new EntityDescriptor(
                    "INVOICE NUMBER", 
                    "Invoice Number.", 
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11011,
                new EntityDescriptor(
                    "PACKING LIST NUMBER",
                    "Packing List Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11012, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "SCAC (Standard Carrier Alpha Code) (an4 - dash '-' filled left) and carrier assigned PROgressive number.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0529
#else
                    () => AlphanumericRegEx0529
#endif
                )
            },
            {
                11013, new EntityDescriptor(
                    "BILL OF LADING NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Bill of Lading Number /Transport Receipt Number SCAC + Container cargo's B/L number or waybill number.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0516
#else
                    () => AlphanumericRegEx0516
#endif
                )
            },
            {
                11014,
                new EntityDescriptor(
                    "ORDER AND LINE",
                    "Combined Order Number and Line Number in the format nn...nn+nn...n where a plus (+) symbol is used as a delimiter between the Order Number and Line Number.",
#if NET7_0_OR_GREATER
                    NumericPlus01UnboundRegEx
#else
                    () => NumericPlus01UnboundRegEx
#endif
                )
            },
            {
                11015, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "KANBAN",

                    // ReSharper disable once StringLiteralTypo
                    "KANBAN Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DELINS",

                    // ReSharper disable once StringLiteralTypo
                    "DELINS Number:  code assigned to identify a document which contains delivery information.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            { 11017, new EntityDescriptor(
                "CHECK", 
                "Check Number.",
#if NET7_0_OR_GREATER
                Alphanumeric01UnboundRegEx 
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11018,
                new EntityDescriptor(
                    "REFERENCE", 
                    "Structured Reference.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11019,
                new EntityDescriptor(
                    "FOREIGN MILITARY SALES CASE",
                    "Foreign Military Sales Case Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11020,
                new EntityDescriptor(
                    "LICENSE IDENTIFIER",
                    "License Identifier, being a globally unique identifier for a license or contract under which items are generated, submitted for processing and/or paid for, that is constructed by concatenating:\n - an ISO/IEC 15459 issuing agency code;\n - a license or contract number which accords with specifications of the issuing agency concerned;\n and that:\n - comprises only upper case alphabetic and/or numeric characters;\n - is unique (that is, is distinct from any other ISO/IEC 15459 compliant identifier) within the domain of the issuing agency6;\n - cannot be derived from any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, its end.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11021,
                new EntityDescriptor(
                    "CUSTOMER DATA",
                    "Customer Data, being data that:\n - from a customer perspective, is related to or associated with an item or transaction, or to a batch or related items or transactions, and\n - comprises up to 35 printable characters and/or spaces, other than plus (+), drawn from the character set defined in ISO/IEC 646.",
#if NET7_0_OR_GREATER
                    InvariantNoPlus01UnboundPlusRegEx
#else
                    () => InvariantNoPlus01UnboundPlusRegEx
#endif
                )
            },
            {
                11022,
                new EntityDescriptor(
                    "TRANSACTION AUTHENTICATION",
                    "'22K' Transaction Authentication Information, being a value, constructed by concatenating:\n - an ISO/IEC 15459 issuing agency code;\n - a value which accords with specifications of the issuing agency concerned,\n that allows verification of the authenticity of the transaction concerned and, in particular, that the transaction was initiated by the party, claimed within the transaction to have been its initiator, by:\n - the recipient of a transaction, and/or\n - one or more of the parties involved in its handling or processing, and/or\n - a trusted third party.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11025,
                new EntityDescriptor(
                    "CARRIER TRANSPORT UNITS GROUPINGS",
                    "Global Unique Identification of Groupings of Transport Units Assigned by the Carrier, defined as:\nIdentification of a Party to a Transaction as defined assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the Bill of Lading or Waybill or Shipment Identification Code that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11026,
                new EntityDescriptor(
                    "SHIPPER TRANSPORT UNITS GROUPINGS",
                    "Global Unique Identification of Groupings of Transport Units Assigned by the Shipper, defined as:\nIdentification of a Party to a Transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the Bill of Lading or Waybill or Shipment Identification Code that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11027,
                new EntityDescriptor(
                    "QUOTATION",
                    "Supplier Assigned Quotation Number - Number assigned to a quotation by the supplier in response to a request for quote from the customer.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                )
            },
            {
                12000,
                new EntityDescriptor(
                    "STORAGE LOCATION",
                    "Storage Location.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            { 12001, new EntityDescriptor(
                "LOCATION", 
                "Location.",
#if NET7_0_OR_GREATER
                Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12002,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' Location Code defined by an industry standard or mutually defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12003,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' Location Code defined by an industry standard or mutually defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12004,
                new EntityDescriptor(
                    "COUNTRY OF ORIGIN",
                    "Country of Origin, two-character ISO 3166 country code. With agreement of trading partners and when the Country of Origin is mixed, Country Code 'AA' shall be used.",
#if NET7_0_OR_GREATER
                    Alpha02RegEx
#else
                    () => Alpha02RegEx
#endif
                )
            },
            {
                12005,
                new EntityDescriptor(
                    "SHIP FOR",
                    "'Ship For:' Location Code defined by an industry standard or mutually defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12006,
                new EntityDescriptor(
                    "ROUTE CODE",
                    "Route Code assigned by the supplier to designate a specific transportation path.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DODAAC",

                    // ReSharper disable once StringLiteralTypo
                    "6-character Department of Defense Activity Code (DoDAAC).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx6
#else
                    () => AlphanumericRegEx6
#endif
                    )
            },
            {
                12008,
                new EntityDescriptor(
                    "PORT OF EMBARKATION",
                    "Port of Embarkation - Mutually Defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12009,
                new EntityDescriptor(
                    "PORT OF DEBARKATION",
                    "Port of Debarkation - Mutually Defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12011, new EntityDescriptor(
                    "LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
#if NET7_0_OR_GREATER
                    LatitudeLongitudeAttitudeRegEx
#else
                    () => LatitudeLongitudeAttitudeRegEx
#endif
                )
            },
            {
                12012, new EntityDescriptor(
                    "SHIP TO",

                    // ReSharper disable once StringLiteralTypo
                    "'Ship To:' Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
#if NET7_0_OR_GREATER
                    LatitudeLongitudeAttitudeRegEx
#else
                    () => LatitudeLongitudeAttitudeRegEx
#endif
                )
            },
            {
                12013, new EntityDescriptor(
                    "SHIP FROM",

                    // ReSharper disable once StringLiteralTypo
                    "'Ship From:' Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
#if NET7_0_OR_GREATER
                    LatitudeLongitudeAttitudeRegEx
#else
                    () => LatitudeLongitudeAttitudeRegEx
#endif
                )
            },
            {
                12015, new EntityDescriptor(
                    "SHIP FOR",

                    // ReSharper disable once StringLiteralTypo
                    "Ship For: Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
#if NET7_0_OR_GREATER
                    LatitudeLongitudeAttitudeRegEx
#else
                    () => LatitudeLongitudeAttitudeRegEx
#endif
                )
            },
            {
                12016,
                new EntityDescriptor(
                    "TAG ACTIVATION LOCATION",
                    "Tag Activation Location.\nEnglish location name (character set:  0-9, A-Z <Space>).",
#if NET7_0_OR_GREATER
                    AlphanumericSpace0160RegEx
#else
                    () => AlphanumericSpace0160RegEx
#endif
                )
            },
            {
                12017,
                new EntityDescriptor(
                    "TAG DEACTIVATION LOCATION",
                    "Tag Deactivation Location.\nEnglish location name (character set:  0-9, A-Z <Space>).",
#if NET7_0_OR_GREATER
                    AlphanumericSpace0160RegEx
#else
                    () => AlphanumericSpace0160RegEx
#endif
                )
            },
            {
                12018, new EntityDescriptor(
                    "FAO FISHING AREA",

                    // ReSharper disable once StringLiteralTypo
                    "FAO fishing area code as defined by the Fisheries and Aquaculture Department of the FAO (http://www.fao.org. Search for Fishing Area Code sub-site).\nAll characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed.\nExamples:\n18L37.1.3 Western Mediterranean Sea, Sardinia;\n18L47.B.1 Atlantic, Southeast, SEAFO Division, Namibia EEZ;\n18L67    Pacific, Northeast.",
#if NET7_0_OR_GREATER
                    Invariant0212RegEx
#else
                    () => Invariant0212RegEx
#endif
                )
            },
            {
                12020,
                new EntityDescriptor(
                    "FIRST LEVEL",
                    "First Level (internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12021,
                new EntityDescriptor(
                    "SECOND LEVEL",
                    "Second Level (internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12022,
                new EntityDescriptor(
                    "THIRD LEVEL",
                    "Third Level (internally assigned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12023,
                new EntityDescriptor(
                    "FOURTH LEVEL",
                    "Fourth Level (internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12024,
                new EntityDescriptor(
                    "FIFTH LEVEL",
                    "Fifth Level (internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a Party to a Transaction, e.g., 25L IAC CIN LOC assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the physical internal location (LOC) that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                12026, new EntityDescriptor(
                    "LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "'26L' Location Code, being a code identifying a location or geographic area, or an associated group of such locations or areas, that has relevance to a related transaction and that complies with one or the structures defined in (a) to (f) below:\na) two upper case alphabetic character corresponding to the ISO 3166-1 two alpha country code of the country in which, or consisting of which, the location(s) or area(s) are situated;\nb) three upper case alphabetic characters corresponding to the IATA code of the airport or city in, close to, or consisting of which the location(s) or area(s) are situated;\nc) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dash (-), with the balance being a postcode in the country concerned;\nd) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dot (.), with the balance being an ISO 3166-2 country subdivision code in the country concerned;\ne) five upper case alphabetic characters corresponding to the UN/LOCODE of the area in, close to, or consisting of which, the location(s) or area(s) are situated;\nf) the concatenation, being not less than seven or more than 35 characters in length, of:\n- an ISO/IEC 15459 issuing agency code;\n- a location code, consisting of characters drawn from the set {A-Z; 0-9} which accords with specifications of the issuing agency concerned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12027, new EntityDescriptor(
                    "EVENT LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "Event Location UN/LOCODE.\nUN/LOCODE followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3227  'Location function code qualifier', e.g.,\n7 Place of Final Delivery;\n5 Port of Departure;\n9 Port of Lading;\n11 Port of Unlading;\n13 Place of transhipment;\n24 Port of Entry;\n35 Exportation country;\n88 Place of Carrier Receipt;\n125 Foreign Port prior to Depart to U.S;\n147 Stowage cell/position;\n159 Place of delivery (to consignee);\n248 Loading Location\nhttp://www.unece.org/cefact/locode/.",
#if NET7_0_OR_GREATER
                    Alpha04Numeric0103RegEx
#else
                    () => Alpha04Numeric0103RegEx
#endif
                )
            },
            {
                12028,
                new EntityDescriptor(
                    "NUMBER AND STREET",
                    "Number and Street Address.\nUsed in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                12029,
                new EntityDescriptor(
                    "CITY",
                    "City Name.\nUsed in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                12030,
                new EntityDescriptor(
                    "COUNTRY SUB ENTITY",
                    "Country Sub-entity Details.\nUsed in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0109
#else
                    () => AlphanumericRegEx0109
#endif
                    )
            },
            {
                12031,
                new EntityDescriptor(
                    "POSTAL CODE",
                    "Postal Code.\nUsed in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L (If a '-' dash is used, it shall be expressly encoded).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0411
#else
                    () => AlphanumericRegEx0411
#endif
                    )
            },
            {
                12032,
                new EntityDescriptor(
                    "COUNTRY",
                    "Country Code.\nISO 3166-1 Alpha 2 Code\nUsed in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
#if NET7_0_OR_GREATER
                    Alpha02RegEx
#else
                    () => Alpha02RegEx
#endif
                )
            },
            {
                12033,
                new EntityDescriptor(
                    "URL",
                    "Uniform Resource Locator (URL).\nIncludes all characters that form a URL, including header data such as e.g., http://. Character set as listed in RFC 1738.",
#if NET7_0_OR_GREATER
                    UniformResourceLocatorRegEx
#else
                    () => UniformResourceLocatorRegEx
#endif
                )
            },
            {
                12034, new EntityDescriptor(
                    "P2P URL",

                    // ReSharper disable once StringLiteralTypo
                    "Pointer to Process URL (P2P URL) for initiating a URL to carry all other data elements encoded in an AIDC media according to the following rule: Scan the code and initiate the URL starting with the P2P URL string, omitting DI 34L and ISO/IEC 15434 envelope syntax (prefix and postfix) and append all other data elements that have been scanned in same sequence as encoded in the media, including DIs and data element separators. Convert special characters in the appended data into RFC 1738 format (e.g., Group Separator 'GS' translated into RFC 1738 sequence %1D). Note that this does not apply to the P2P URL itself.\nExample: Encoded data string (using ISO/IEC 15434) [)>RS06GS25SUN123456789PA12345GS4LUSGS16D20131108 G S34LHTTP://WWW.SECUREUID.COM/ITEMDATA/?SCAN= R S05 GS13131108RSEOT\nresults in the following URL with the transmitted data:\nHTTP://WWW.SECUREUID.COM/ITEMDATA/?SCAN=25SU N123456789PA12345%1D4LUS%1D16D20131108.\nNote: data from the '05' format envelope was not incorporated in the URL since the 34L was encoded in the '06' format envelope.",
#if NET7_0_OR_GREATER
                    UniformResourceLocatorRegEx
#else
                    () => UniformResourceLocatorRegEx
#endif
                )
            },
            {
                12035, new EntityDescriptor(
                    "SITE APPROVAL",

                    // ReSharper disable once StringLiteralTypo
                    "A government-assigned approval number of vessel / aquaculture site / farm / processor, starting with an ISO 31661 alpha-2 country code, followed by the approval number.\nAll characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed.\nExample:\n35LIECK0107EC = Country; Ireland. Vessel Name; FV Endurance DA31.",
#if NET7_0_OR_GREATER
                    Alpha02Invariant0327RegEx
#else
                    () => Alpha02Invariant0327RegEx
#endif
                )
            },
            {
                12036, new EntityDescriptor(
                    "PRODUCER APPROVAL",

                    // ReSharper disable once StringLiteralTypo
                    "A government-assigned approval number of producer or farm or first deboning / cutting hall, starting with an ISO 3166-1 alpha-2 country code, followed by the approval number.\nAll characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed.\nExample:\n36LIECK0107EC = Country; Ireland. Vessel Name; FV Endurance DA31.",
#if NET7_0_OR_GREATER
                    Alpha02Alphanumeric0327RegEx
#else
                    () => Alpha02Alphanumeric0327RegEx
#endif
                )
            },
            {
                12051,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' - Location code defined by a postal authority (e.g., 5-digit and 9-digit ZIP codes identifying U.S. locations or 6-character postal codes identifying Canadian locations).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0109
#else
                    () => AlphanumericRegEx0109
#endif
                    )
            },
            {
                12052,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' - Location code defined by a postal authority (e.g., 5-digit and 9-digit ZIP codes identifying U.S. locations or 6character postal codes identifying Canadian locations).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0109
#else
                    () => AlphanumericRegEx0109
#endif
                    )
            },
            {
                12054,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' - Location code defined by a postal authority in the format:  postal codes (e.g., 5-digit ZIP codes identifying U.S. locations or 6- or 7-character postal codes identifying United Kingdom locations) followed by two character ISO 3166 country code  (e.g., US or GB).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0109
#else
                    () => AlphanumericRegEx0109
#endif
                    )
            },
            {
                12055,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' - Location code defined by a postal authority in the format:  postal codes (e.g., 5-digit ZIP codes identifying U.S. locations or 6- or 7-character postal codes identifying United Kingdom locations) followed by two character ISO 3166 country code  (e.g., US or GB).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0109
#else
                    () => AlphanumericRegEx0109
#endif
                    )
            },
            {
                13010,
                new EntityDescriptor(
                    "FORM 2410",
                    "Army Form 2410 data. Format is data value preceded by the block number of the form 2410. Field lengths and acceptable characters can be found at;\nhttp://www.apd.army.mil/pdffiles/p738_751.pdf.\n2020 Update: The URL has been modified to:\nhttps://armypubs.army.mil/ProductMaps/PubForm/Details.asp x?PUB_ID=1408",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                13011,
                new EntityDescriptor(
                    "FORM 2408",
                    "Army Form 2408 data. Format is data value preceded by the block number of the form 2408. Field lengths and acceptable characters can be found at;\nhttp://www.apd.army.mil/pdffiles/p738_751.pdf.\n2020 Update: The URL has been modified to:\nhttps://armypubs.army.mil/ProductMaps/PubForm/Details.asp x?PUB_ID=1400",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                13012,
                new EntityDescriptor(
                    "FORM 2407",
                    "Army Form 2407 data. Format is data value preceded by the block number of the form 2407. Field lengths and acceptable characters can be found at;\nhttp://www.apd.army.mil/pdffiles/p738_751.pdf.\n2020 Update: The URL has been modified to:\nhttps://armypubs.army.mil/ProductMaps/PubForm/Details.asp x?PUB_ID=1391",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                13013,
                new EntityDescriptor(
                    "FORM 95",
                    "Air Force Form 95 data. Format is data value preceded by the block number of the form 95. Field lengths and acceptable characters can be found at;\nhttp://www.gsa.gov/portal/forms/download/116418.\n2020 Update: The Name of the Form is “Air Force Technical Order Form 95 (AFTO Form 95).” Details about the Form are sourced in Air Force Technical Order (TO) 00-20-1. The URL for this TO has been modified to:\nhttps://www.tinker.af.mil/Portals/106/Documents/Technical%20Orders/AFD-180615-00-20-1.pdf",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                13014,
                new EntityDescriptor(
                    "FORM 4790",
                    "Navy Form 4790 data. Format is data value preceded by the block number of the form 2410. Field lengths and acceptable characters can be found at;\nhttp://www.navair.navy.mil/logistics/4790/library/Chapter%201 5.pdf.\n2020 Update: The URL is no longer valid.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14000,
                new EntityDescriptor(
                    "NSN",
                    "National/NATO Stock Number (NSN).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx1315
#else
                    () => AlphanumericRegEx1315
#endif
                    )
            },
            {
                14001, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CIDX PRODUCT CHARACTERISTIC",

                    // ReSharper disable once StringLiteralTypo
                    "Product Characteristic Data defined by the Chemical Industry Data Exchange (CIDX).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14003, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "EIAJ ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "Coding Structure in Accordance with Format Defined by Electronic Industries Association Japan (EIAJ).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14004,
                new EntityDescriptor(
                    "GS1 ENCODED",
                    "Coding Structure and Formats in Accordance with GS1 Application Identifiers (AI plus data) (GS1).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14005, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "AIAG ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "Coding Structure and Formats in Accordance with AIAG Recommendations. The full Data Identifier is in the form 5Nxx where the 'xx' is found in the full code list that can be found at https://www.aiag.org/",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14006, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MILSTRIP ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "U.S. DOD Requisition and Issue Procedure Codes. The format is the appropriate MILSTRIP code followed by the data value associated with that code. (The full list of codes is available at;\nhttp://www2.dla.mil/j6/dlmso/elibrary/Manuals/DLM/MILSTRIP/MILSTRIP.pdf.\n2020 Update: The URL has been modified to:\nhttps://www.dla.mil/HQ/InformationOperations/DLMS/elibrary/manuals/MILSTRIP/",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14007,
                new EntityDescriptor(
                    "DTR ENCODED",
                    "U.S. Defense Transportation Regulation Codes. The format is the DTR code followed by the appropriate data value associated with that code. (The full list of codes is available at;\nhttp://www.transcom.mil/dtr/part-ii/dtr_part_ii_toc.pdf.\n2020 Update: The URL has been modified to:\nhttps://www.ustranscom.mil/dtr/part-ii/dtr_part_ii_toc.pdf",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14008,
                new EntityDescriptor(
                    "PRODUCTION ANIMAL IDENTIFICATION",
                    "Production Animal Identification Codes. The format is the production animal code followed by the appropriate data value associated with that code. The Technical Report and the full list of Extended Data Elements (codes) is maintained at;\nhttp://www.aimglobal.org/store/view_product.asp?id=4926441  Extended Data Elements (Codes).\nhttp://www.aimglobal.org/store/view_product.asp?id=4926483 Technical Report.\n2020 Update: The URLs have been modified to:\nhttps://web.aimglobal.org/external/wcpages/wcecommerce/ecomlistpage.aspx?Keyword=animal",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                // NB. The PPN is a general-purpose standard product identifier for pharmaceutical use, and is managed by IFA. It
                // is fundamentally alphanumeric (the last two characters are a checksum, and therefore always digits), starting with 
                // a two-character issuing agency identifier, a 1..18 alphanumeric domain identifier and a two digit checksum. The
                // MH10.82 format is simplified to 5..22 alphanumeric characters, nd so does not quite fully describe the true PPN 
                // standard. In Germany, the issuing agency is currently always numeric. For the European Medicines Verification
                // System (EMVS), the domain identifier is always based on the 8-digit PZN. So, currently, a German PPN on a medicinal
                // pack is always exactly 12 digits in length. NB. SecurPharm erroneously state that the PPN is 4..22 alphanumeric
                // characters.
                14009,
                new EntityDescriptor(
                    "PPN",
                    "Pharmacy Product Number maintained by IFA (www.ifaffm.de) with the following elements; Data Identifier (DI), two-digit product registration agency code (PRAC), the product reference (PR), and the two PPN check digits (CC), in the form DI PRAC PR CC.\nNOTE – space is added as a separator for visual clarity and is not part of the data.\nExamples:\n  • 9N1112345678CC\n  • 9N1312345MEDDEVICE1245678900CC",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0522
#else
                    () => AlphanumericRegEx0522
#endif
                    )
            },
            {
                14010,
                new EntityDescriptor(
                    "IAC CIN ENCODED",
                    "Data in the format and using semantics defined by the holder of a Company Identification Number (CIN) that has been issued by an Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, defined as a sequence of concatenated data elements:  IAC, followed by CIN, followed by the separator character ':' (colon) followed by the data in the format and using semantics as defined by the CIN holder.\nNOTE: Only the data syntax rules (if any) as provided by the declared IAC+CIN within each DI '10N' data stream shall be applied to the data following DI 10N+IAC+CIN.\nNOTE: Due to an error in the assignment of DI '10N' (there is no central authority for data-definition nor maintenance), no new uses of DI '10N' should be implemented.\nThe function of DI '10N' is established in Category 18, MISCELLANEOUS with DI '5R'. It is strongly recommended that existing applications that use DI '10N' migrate to DI 5R'.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14011, new EntityDescriptor(
                    "RLA ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "The Data construct is defined and controlled by the RLA, and is comprised of 2 segments: the field identifier code, immediately followed by the data as defined for that element according to the data dictionary of the RLA. It is essentially a catalog of fields with standardized content. The Field Identifiers are posted at http://rla.org/11ncodes. The use and structure of these codes are defined at:  http://rla.org/11nformat   Additional examples can be found at that site as well. DI '11N' shall never be encoded in a 2D or RFID tag together with any other DI elements.\nNOTE: Due to an error in the assignment of DI '11N' (the language which states: 'DI '11N' shall never be encoded in a 2D or RFID tag together with any other DI elements.' is not a valid statement), no new uses of DI '11N' should be implemented. The function of DI '11N' is established in DI '12N'. It is strongly recommended that existing applications that use DI '11N' migrate to DI '12N'.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14012,
                new EntityDescriptor(
                    "RLA ENCODED",
                    "The Data construct is defined and controlled by the RLA, comprised of 2 segments: the field identifier (FI) code, immediately followed by the data as defined for that element according to the data dictionary of the RLA. It is essentially a catalog of fields with standardized content. The Field Identifiers are posted at http://rla.org/12ncodes  The use and structure of these codes are defined at:  http://rla.org/12nformat. Examples can be found at that site.\n2020 Update: The URL has changed to:\nhttps://rla.org/page/sqrl-code-listing",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14015,
                new EntityDescriptor(
                    "CAICT INDUSTRIAL INTERNET ID",
                    "Representing Industrial Internet Identifier Codes controlled and maintained by CAICT, used in the Industrial Internet Identifier Resolution System of China and constructed as <DI><IAC><TTC><STC><CIN><SN>, in the form an3+a3+n3+n3+n8+an1…33, where an3 is the Data Identifier (DI), a3 is the Issuing Agency Code (IAC = “VAA”), n3 is the Top-Tier Code (TTC), n3 is the Secondary-Tier Code (STC), n8 is the Company Identification Number (CIN) controlled and assigned by the Secondary-Tier platform and an1…33 is the Serial Number (SN) that is controlled and assigned by the holder of the CIN, and is unique within that CIN holders’ domain, using the characters 0 through 9, upper- and lower\u0002case A through Z, * (asterisk), + (plus sign), - (dash), . (period or full stop), / (forward slash), ( (left parenthesis), ) (right parenthesis), ! (exclamation mark).\nExamples:\n15NVAA08810000000001123Ab.098\n15NVAA0881000000000112334Diat*C-\nDE!(8765)jiuY/L23+a!h",
#if NET7_0_OR_GREATER
                    Alpha03Numeric14Alphanumeric0133RegEx
#else
                    () => Alpha03Numeric14Alphanumeric0133RegEx
#endif
                )
            },
            {
                16000,
                new EntityDescriptor(
                    "CUSTOMER ITEM ID",
                    "Item Identification Code assigned by Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16001,
                new EntityDescriptor(
                    "SUPPLIER ITEM ID",
                    "Item Identification Code assigned by Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16002,
                new EntityDescriptor(
                    "REVISION LEVEL",
                    "Code Assigned to Specify the Revision Level for an Item (e.g., engineering change level, edition, or revision).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16003,
                new EntityDescriptor(
                    "GS1 MFR/ITEM CODE",
                    "Combined Manufacturer Identification Code/Item Code Under the 12/13-digit GS1 Formats, plus supplemental codes, if any.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.",
#if NET7_0_OR_GREATER
                    Numeric1314RegEx
#else
                    () => Numeric1314RegEx
#endif
                )
            },
            {
                16004,
                new EntityDescriptor(
                    "GS1 ITEM CODE PORTION",
                    "Item Code Portion of GS1 Formats.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16005,
                new EntityDescriptor(
                    "FREIGHT CLASSIFICATION ITEM",
                    "Freight Classification Item Number Assigned by Carrier for Purposes of Rating Hazardous Materials (e.g., Motor Freight, Air, Boat, Rail Classification).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16006,
                new EntityDescriptor(
                    "SUPPLIER/ITEM CODE",
                    "Combined Supplier Identification and Item Code (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CLEI",

                    // ReSharper disable once StringLiteralTypo
                    "Common Language Equipment Identification (CLEI) assigned by the manufacturer to some telecommunications equipment.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16008, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "GS1 GTIN-14",

                    // ReSharper disable once StringLiteralTypo
                    "14-digit GS1 format for GTIN-14 code structure.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.",
#if NET7_0_OR_GREATER
                    Numeric14RegEx
#else
                    () => Numeric14RegEx

#endif
                )
            },
            {
                16009,
                new EntityDescriptor(
                    "DUNS/ITEM CODE",
                    "Combined Manufacturer Identification Code (9-digit DUNS number assigned by Dun & Bradstreet) and the Item Code/Part Number (assigned by the manufacturer).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16010,
                new EntityDescriptor(
                    "HAZARDOUS MATERIAL CODE",
                    "Hazardous Material Code as defined by ANSI X12.3 in the format Data Element 208 (1-character code qualifier) followed by Data Element 209 (Hazardous Material Code).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16011, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CLEI",

                    // ReSharper disable once StringLiteralTypo
                    "10-character CLEI Code for telecommunications equipment.", 
#if NET7_0_OR_GREATER
                    AlphanumericRegEx10
#else
                    () => AlphanumericRegEx10
#endif
                    )
            },
            {
                16012,
                new EntityDescriptor(
                    "DOCUMENT TYPE",
                    "Document Type (e.g., Pick List, Design Drawing, etc.) (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16013, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System Code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16014, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM & ASSEMBLY",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System and Assembly Code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16015, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM, ASSEMBLY & PART",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System, Assembly, & Part Code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM, ASSEMBLY & PART",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System, Assembly, or Part Code. (User Modified).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16017,
                new EntityDescriptor(
                    "GS1 SUPPLIER ID & ITEM CODE",
                    "Combined GS1 Supplier Identification and Item Code Assigned By The Supplier.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16018, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SUPPLIER ID & PART NO.",

                    // ReSharper disable once StringLiteralTypo
                    "Combined VMRS supplier ID and Supplier Assigned Part Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16019,
                new EntityDescriptor(
                    "COMPONENT",
                    "Component of an Item. (One product contained in multiple packages).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16020,
                new EntityDescriptor(
                    "CUSTOMER FIRST LEVEL",
                    "First Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16021,
                new EntityDescriptor(
                    "CUSTOMER SECOND LEVEL",
                    "Second Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16022,
                new EntityDescriptor(
                    "CUSTOMER THIRD LEVEL",
                    "Third Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16023,
                new EntityDescriptor(
                    "CUSTOMER FOURTH LEVEL",
                    "Fourth Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16024,
                new EntityDescriptor(
                    "CUSTOMER FIFTH LEVEL",
                    "Fifth Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a Party to a Transaction Assigned by a Holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assigned part number that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16026,
                new EntityDescriptor(
                    "PART NUMBER",
                    "Part Number of Next Higher Assembly.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16027, new EntityDescriptor(
                    "HTS-6 CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Commodity HTS-6 Code; Using the format:  4012.11 or 4012.11.4000  (Decimal point is expressly encoded) The Harmonized System (HS) Classification is a 6-digit standardized numerical method of classifying traded products. HS numbers are used by customs authorities around the world to identify products for the application of duties and taxes. Additional digits are added to the HS number by some governments to further distinguish products in certain categories. In the United States, numbers used to classify exported products are called 'Schedule B' numbers. The U.S. Census Bureau administers the Schedule B system. Schedule B numbers, not HS numbers, must be provided on the Shippers' Export Declaration (SED).\nhttp://www.niccomp.com/rohs/files/NIC_HTS1006.pdf\nImport codes are administered by the U.S. International Trade Commission (USITC). http://hts.usitc.gov/\n2020 Update: the http://www.niccomp.com/rohs/files/NIC_HTS1006.pdf link is no longer active. Consult the shipper for an SED form.",
#if NET7_0_OR_GREATER
                    Numeric0712RegEx
#else
                    () => Numeric0712RegEx
#endif
                )
            },
            {
                16028,
                new EntityDescriptor(
                    "CARGO NAME",
                    "Cargo Name. Plain language description (English).",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx01100
#else
                    () => AlphanumericRegEx01100
#endif
                    )
            },
            {
                16029, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "GMDN PRODUCT CLASSIFICATION",

                    // ReSharper disable once StringLiteralTypo
                    "Product Classification Code as defined with the GMDN (Global Medical Device Nomenclature - http://www.gmdnagency.org).",
#if NET7_0_OR_GREATER
                    Numeric05RegEx
#else
                    () => Numeric05RegEx
#endif
                )
            },
            {
                16030,
                new EntityDescriptor(
                    "SUPPLIER FIRST LEVEL",
                    "First Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16031,
                new EntityDescriptor(
                    "SUPPLIER SECOND LEVEL",
                    "Second Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16032,
                new EntityDescriptor(
                    "SUPPLIER THIRD LEVEL",
                    "Third Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16033,
                new EntityDescriptor(
                    "SUPPLIER FOURTH LEVEL",
                    "Fourth Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16034,
                new EntityDescriptor(
                    "SUPPLIER FIFTH LEVEL",
                    "Fifth Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16040, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MSDS CODE",
                    "A Code Assigned by a Customer to the Identification Number of the Manufacturer's Material Safety Data Sheet (MSDS) document that describes the uses, hazards, and chemical composition of a hazardous material.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16049, new EntityDescriptor(
                    "EXPORT CONTROLLED BY",

                    // ReSharper disable once StringLiteralTypo
                    "Export Controlled Item. Subject to export control and or restrictions as identified in the Wassenaar Arrangement. DI followed by the Alpha-2 ISO 3166 Country Code of the country that imposed the restriction followed by Wassenaar Code (http://www.wassenaar.org/controllists/index.html).\n2020 Update: The URL has changed to https://www.wassenaar.org/control-lists/",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0309
#else
                    () => AlphanumericRegEx0309
#endif
                    )
            },
            {
                16050, new EntityDescriptor(
                    "MANUFACTURER ITEM ID",

                    // ReSharper disable once StringLiteralTypo
                    "Manufacturer-Assigned Item Identifier - comprising an item number assigned by the item manufacturer, followed by a plus (+) sign, followed - if required to uniquely identify the item within the manufacturer's product range - by a manufacturerassigned item version.\nExample 50PABC+6 would represent item number ABC, item version 6\nNOTE - The item number shall always be followed by a plus sign, even if no item version is present. This is required to permit the unambiguous concatenation of manufacturerassigned item identifier with another data construct using the concatenation character plus (+). For example, the combination of a 50P manufacturer-assigned item identifier with no item version and a serial number (Data identifier S) on an entity might be encoded as 50PDEF++S1234.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0335
#else
                    () => AlphanumericRegEx0335
#endif
                    )
            },
            {
                16051,
                new EntityDescriptor(
                    "ITEM ID",
                    "Globally Unique Item Identifier comprising the Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, followed by a plus (+) sign, followed by the Manufacturer-assigned item identifier as defined with 50P\nExample: 51PJ4LBE0431863103+ABC+ would represent the item with item number ABC and no version number manufactured by the company with Belgian VAT number 0431863103.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16052,
                new EntityDescriptor(
                    "COLOR",
                    "Color Code.\nColor of an item/object identified by a code or term mutually agreed upon between trading partners.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0150
#else
                    () => AlphanumericRegEx0150
#endif
                    )
            },
            {
                16053,
                new EntityDescriptor(
                    "SPECIFIC MARINE EQUIPMENT",
                    "Identifier for Specific Marine Equipment approved under the European Union Directive on Marine Equipment (2014/90/EU) and Implementing Regulation (EU) 2018/608.\nFormat:\n• DI (an3);\n• Type of conformity assessment (CA) module(s) set out in Annex II to Directive 2014/90/EU used for the conformity assessment (a1);\n• Notified body (NB) identification number assigned by the Commission in accordance with point 3.1 of Annex IV to Directive 2014/90/EU (n4);\n• Certificate (an5…20)",
#if NET7_0_OR_GREATER
                    Alpha01Numeric04Alphanumeric0520RegEx
#else
                    () => Alpha01Numeric04Alphanumeric0520RegEx
#endif
                )
            },
            {
                16054,
                new EntityDescriptor(
                    "UDI DI",
                    "UDI-DI (Unique Device Identification - Device Identifier) for Medical Devices (MD) and In-vitro-Diagnostics (IvD) as the unique key to public UDI data bases (GUDID, EUDAMED, etc.), according to national regulatory requirements, as outlined by the International Medical Device Regulators Forum (IMDRF). All printable characters of the UTF-8 character set are allowed.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                16055,
                new EntityDescriptor(
                    "DNV CERT REF",
                    "DNV certification reference. Indicates that the data contains a reference to a product certificate/verification statement/report, issued by DNV. Data identifier shall be followed by letters “NV” and certificate number. When certificate Number has postfix, it should be included in the datastream by using the “-“ separator character. Revision indicators shall not be provided.\nExamples:\n1. 55PNVXXXXXXX, where 55P is the data identifier, XXXXXXX is DNV Certificate Number.\n2. 55PNVXXXXXXX-Y, where 55P is the data identifier, XXXXXXX-Y is DNV Certificate Number with postfix Y.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17000,
                new EntityDescriptor(
                    "QUANTITY",
                    "Quantity, Number of Pieces, or Amount (numeric only) (unit of measure and significance mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17001,
                new EntityDescriptor(
                    "THEORETICAL LENGTH/WEIGHT",
                    "Theoretical Length/Weight (numeric only).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17002,
                new EntityDescriptor(
                    "ACTUAL WEIGHT",
                    "Actual Weight (numeric only).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17003,
                new EntityDescriptor(
                    "UNIT OF MEASURE",
                    "Unit of Measure, as defined by the two character ANSI X12.3 Data Element Number 355 Unit of Measurement Code.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx2
#else
                    () => AlphanumericRegEx2
#endif
                    )
            },
            {
                17004, new EntityDescriptor(
                    "GROSS AMOUNT", 
                    "Gross Amount.", 
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17005, new EntityDescriptor(
                    "NET AMOUNT", 
                    "Net Amount.", 
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17006,
                new EntityDescriptor(
                    "CONTAINERS",
                    "Where Multiple Containers Comprise a Single Product (the contents of each container must be combined with the content of the other containers to constitute a single product) the Data Identifier '6Q' shall be used to link the various containers. The format # of # ('this is the nth piece of x pieces to define the product') Presented in the format 'n/x', where the '/' (slash) is used as a delimiter between two values.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17007,
                new EntityDescriptor(
                    "QUANTITY/UOM",
                    "Quantity, Amount, or Number of Pieces in the format: Quantity followed by the two character ANSI X12.3 Data Element Number 355 Unit of Measurement Code.",
#if NET7_0_OR_GREATER
                    Numeric0108Alphanumeric02RegEx
#else
                    () => Numeric0108Alphanumeric02RegEx
#endif
                )
            },
            {
                17008,
                new EntityDescriptor(
                    "CONTAINER RATED WEIGHT",
                    "Container Rated Weight\nManufacturer-assigned weight carrying capability of the container. Assigned at time of manufacture. Unit of measure is kg.",
#if NET7_0_OR_GREATER
                    Numeric0406RegEx
#else
                    () => Numeric0406RegEx
#endif
                )
            },
            {
                17009,
                new EntityDescriptor(
                    "PIECE WEIGHT",
                    "Piece Weight\nWeight of a single item.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17011,
                new EntityDescriptor(
                    "TARE WEIGHT",
                    "Tare Weight: weight of an empty container.\nContainer body weight.\nManufacturer-assigned weight of the empty container. Assigned at time of manufacture. Unit of measure is kg (Tare weight).",
#if NET7_0_OR_GREATER
                    Numeric0406RegEx
#else
                    () => Numeric0406RegEx
#endif
                )
            },
            {
                17012,
                new EntityDescriptor(
                    "MONETARY VALUE",
                    "Monetary Value established by the Supplier in the format of: the value followed by an ISO 4217 data element code for representing unit of value of currencies and funds (e.g., 12Q2.50USD) (2.50 Monetary Value in USA Dollars) significance mutually defined.\nEntry Value;\nValue followed by an ISO 4217 data element code for representing unit of value of currencies and funds (e.g., 12Q2.50USD) (2.50 Monetary Value in USA Dollars)",
#if NET7_0_OR_GREATER
                    Numeric0110Alphanumeric03RegEx
#else
                    () => Numeric0110Alphanumeric03RegEx
#endif
                )
            },
            {
                17013,
                new EntityDescriptor(
                    "PIECE OF PIECES",
                    "# of # ('this is the nth piece of x pieces in this shipment').\nPresented in the format 'n/x', where the '/' (slash) is used as a delimiter between two values.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17014,
                new EntityDescriptor(
                    "BEGINNING SECONDARY QUANTITY",
                    "Beginning Secondary Quantity.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17015,
                new EntityDescriptor(
                    "ENDING SECONDARY QUANTITY",
                    "Ending Secondary Quantity.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17016,
                new EntityDescriptor(
                    "NUMBER OF PIECES IN VAN",
                    "Number Of Pieces in Van.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17017,
                new EntityDescriptor(
                    "NUMBER OF SHIPMENTS IN VAN",
                    "Number Of Shipments in Van.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17018,
                new EntityDescriptor(
                    "CUBE",
                    "Cube expressed in cubic metres or cubic feet followed by the ANSI X12.3 data element number 355 unit of measure code (CR of CF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17019,
                new EntityDescriptor(
                    "WIDTH",
                    "Width expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17020,
                new EntityDescriptor(
                    "HEIGHT",
                    "Height expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17021,
                new EntityDescriptor(
                    "LENGTH",
                    "Length expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17022,
                new EntityDescriptor(
                    "NET WEIGHT",
                    "Net Weight Of Shipment expressed in pounds or kilograms (kilos) followed by the ANSI X12.3 data element number 355 unit of measure (LB or KG). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17023,
                new EntityDescriptor(
                    "VAN LENGTH",
                    "Van Length expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure (LC or LF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17024,
                new EntityDescriptor(
                    "INSIDE CUBE OF A VAN",
                    "Inside Cube of a Van expressed in cubic metres or cubic feet followed by the ANSI X12.3 data element number 355 of unit measure code (CR or CF). No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17025,
                new EntityDescriptor(
                    "NET EXPLOSIVE WEIGHT",
                    "Net Explosive Weight (a computed value of explosive equivalent expressed in pound of TNT). The measure of NEW is used internationally for explosive safety quantity distance arc computations. No implied decimal point.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17026, new EntityDescriptor(
                    "PACKAGING LEVEL",

                    // ReSharper disable once StringLiteralTypo
                    "Packaging Level, specifying the hierarchical level of packaging in accordance with HIBC (Health Industry Bar Code) specifications.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                17027,
                new EntityDescriptor(
                    "SINGLE PRODUCT NET PRICE",
                    "Single Product Price Value, Net, '.' (dot) used as decimal point (e.g. 27Q1000.5 for the price value of 1000.50).\nStructure:  an3+an1...20\n   <DI><price value>.\nCharacter set:  0 to 9, dot (ISO 646 ASCII value decimal 46, hexadecimal 2E).\nExample of encoding using a net price value of 1000:\n27Q1000.\nExample of encoding using a net price value of 1000.50:\n27Q1000.5.\nNOTE - If currency is required it can be taken from another data element used in same code, e.g. 12Q.",
#if NET7_0_OR_GREATER
                    NumericDot0120RegEx
#else
                    () => NumericDot0120RegEx
#endif
                )
            },
            {
                17028,
                new EntityDescriptor(
                    "SINGLE PRICE CHARGE VALUE FOR POSTAGE AND PACKAGING",
                    "Single Price Charge Value For Postage And Packaging, '.' (dot) represents the position of a comma (e.g. 30Q100.50 for the value of 100,50).\nStructure:  an3+an1...10\n   <DI><price value>.\nCharacter set:  0 to 9, dot.\nExample of encoding using postage & packing value of 100:\n30Q100.\nExample of encoding using postage & packing value of 100,50:\n30Q100.50.\nNOTE - If currency is required it can be taken from another data element used in same code, e.g. 12Q.",
#if NET7_0_OR_GREATER
                    NumericDot0110RegEx
#else
                    () => NumericDot0110RegEx
#endif
                )
            },
            {
                17029,
                new EntityDescriptor(
                    "DISCOUNT PERCENTAGE",
                    "Discount Percentage, '.' (dot) represents the position of a comma (e.g. 31Q8.5 for a discount value of 8,5%).\nStructure:  an3+n1...6 (12.456)\n   <DI><discount percentage (%)>.\nCharacter set:  0 to 9, dot\nExample of encoding using discount percentage of 10%:\n31Q10.\nExample of encoding using discount percentage of 8,5%:\n31Q8.5.",
#if NET7_0_OR_GREATER
                    NumericDot0106RegEx
#else
                    () => NumericDot0106RegEx
#endif
                )
            },
            {
                17030,
                new EntityDescriptor(
                    "VAT PERCENTAGE",
                    "VAT Percentage, '.' (dot) represents the position of a comma (e.g. 27Q8.5 for the VAT value of 8.5%).\nStructure:  an3+an1...5 (12.45)\n    <DI><VAT percentage (%)>.\nCharacter set:  0 to 9, dot.\nExample of encoding using VAT percentage of 10%:\n27Q19.\nExample of encoding using VAT percentage of 8,5%:\n27Q8.5.",
#if NET7_0_OR_GREATER
                    NumericDot0105RegEx
#else
                    () => NumericDot0105RegEx
#endif
                )
            },
            {
                17031, new EntityDescriptor(
                    "CURRENCY",

                    // ReSharper disable once StringLiteralTypo
                    "Currency, ISO 4217 currency code.\nStructure:  an3+an3\n  <DI><Currency, e.g. EUR>.\nCharacter set:  A-Z, 0 to 9.\nExample of encoding using ISO alphabetic code of US Dollar:\n31QUSD.\nExample of encoding using ISO alphabetic code of EURO:\n31QEUR.\nExample of encoding using ISO numeric code of EURO:\n31Q978.",
#if NET7_0_OR_GREATER
                    Alpha03Numeric03RegEx
#else
                    () => Alpha03Numeric03RegEx
#endif
                )
            },
            {
                17032, new EntityDescriptor(
                    "LOINC CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Clinical term code as defined with the clinical nomenclature: “The international standard for identifying health measurements, observations, and documents – LOINC” (https://loinc.org), in the following sequence: <DI><LOINC Code><Plus Sign><Value>. The unit and format of the Value is defined by the LOINC Code.\nExample: 32Q28903-3+60 = LOINC Code 28903-3: Left contact lens Axis (degrees); with Value = 60.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0335
#else
                    () => AlphanumericRegEx0335
#endif
                    )
            },
            {
                18001,
                new EntityDescriptor(
                    "RMA",
                    "Return Authorization Code (RMA) assigned by the Supplier",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18002,
                new EntityDescriptor(
                    "RETURN CODE",
                    "Return Code Assigned by the Customer",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18004, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DODIC",
                    "U.S. Department of Defense Identification Code (DoDIC)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx4
#else
                    () => AlphanumericRegEx4
#endif
                    )
            },
            {
                18005,
                new EntityDescriptor(
                    "IAC CIN DATE",
                    "Data in the format and using semantics defined by the holder of a Company Identification Number (CIN) that has been issued by an Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, defined as a sequence of concatenated data elements:  IAC, followed by CIN, followed by the separator character ':' (colon) followed by the data in the format and using semantics as defined by the CIN holder.\nNOTE - Only the data syntax rules (if any) as provided by the declared IAC+CIN within each DI '5R' data stream shall be applied to the data following DI 5R+IAC+CIN.4.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18006, new EntityDescriptor(
                    "SIGNATURE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO/IEC 20248 digital signature data construct. If the underlying data carrier encoding is 7 bits, then only the ISO/IEC 20248 raw format may be used.\nExample with an URL format:\n<6R><https://20248.sigvr.it/?Oo586eJAMEYCIQCf31EqIJML GclBpHLlRgBdO>\nExample with a raw format:\n<6R><Oo586eJAMEYCIQCf31EqIJMLGclBpHLlRgBdO>\nAn ISO/IEC 20248 data structure contains a digital signature which is used to verify the specified data elements of the message of data elements. The value of 6R, as the first parameter, and the data elements to be verified (stripped from all non-printable characters), as the second parameter, is passed to the ISO/IEC 20248 DecoderVerifier - which will return the verification result: ACCEPT, REJECT or ERROR(error code), and the JSON object of decoded ISO/IEC 20248 additional fields. The ISO/IEC 20248 data structure may contain additional fields and instructions to decode and verify one or more messages of data elements. These instructions will be processed by the ISO/IEC 20248 DecoderVerifier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "ASFIS CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Aquatic Sciences and Fisheries Information System (ASFIS) 'Inter-agency 3-alpha species code', maintained by the Food and Agriculture Organisation of the United Nations (www.fao.org, then search for 'ASFIS').\nExamples;\n7RMUC = Mud carp;\n7RPCD = Australian freshwater herring;\n7RWSH = Great white shark.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0103
#else
                    () => AlphanumericRegEx0103
#endif
                    )
            },
            {
                18008, new EntityDescriptor(
                    "FAO CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Food and Agricultural Organisation (FAO) International Standard Classification of Fishing Gears (ISSCFG) code. (www.fao.org)\nAll characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed.\nExamples:\n8R02.1.0 = Beach seines;\n8R03.1.5 = Shrimp trawls;\n8R05.1.0 = Portable lift net.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0110
#else
                    () => AlphanumericRegEx0110
#endif
                    )
            },
            {
                18009,
                new EntityDescriptor(
                    "FAO PRODUCTION METHOD",
                    "Production method for fish and seafood as specified by the Fisheries and Aquaculture Department of the Food and Agricultural Organisation (FAO) of the United Nations, according to EU Regulation 1379/2013. (www.fao.org).\nAll characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed.\nExamples;\n9R01 = Caught at sea;\n9R02 = Caught in fresh water;\n9R03 = Farmed.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx2
#else
                    () => AlphanumericRegEx2
#endif
                )
            },
            {
                19000,
                new EntityDescriptor(
                    "SERIAL NUMBER",
                    "Serial Number or Code Assigned by the Supplier to an Entity for its Lifetime, (e.g., computer serial number, traceability number, contract tool identification)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19001,
                new EntityDescriptor(
                    "ADDITIONAL CODE",
                    "Additional Code Assigned by the Supplier to an Entity for its Lifetime (e.g., traceability number, computer serial number) ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19002,
                new EntityDescriptor(
                    "ASN SID",
                    "Advance Shipment Notification (ASN) Shipment ID (SID) corresponds to ANSI ASC X12 Data Element 396",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0230
#else
                    () => AlphanumericRegEx0230
#endif
                    )
            },
            {
                19003,
                new EntityDescriptor(
                    "UNIQUE PACKAGE IDENTIFICATION",
                    "Unique Package Identification Assigned by Supplier (lowest level of packaging which has a package ID code; shall contain like items) ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19004,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION LIKE ITEMS",
                    "Package Identification Assigned by Supplier to master packaging containing like items on a single customer order ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19005,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION UNLIKE ITEMS",
                    "Package Identification Assigned by Supplier to master packaging containing unlike items on a single customer order",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19006,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION LIKE ITEMS MULTIPLE",
                    "Package Identification Assigned by Supplier to master packaging containing like items over multiple customer orders",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19007,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION UNLIKE ITEMS MULTIPLE",
                    "Package Identification Assigned by Supplier to master packaging containing unlike items over multiple customer",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19008, new EntityDescriptor(
                    "SUPPLIER ID",

                    // ReSharper disable once StringLiteralTypo
                    "Supplier ID/Unique Container ID presented in the data format specified by the GS1 SSCC-18\n2020 Update: Refer to the GS1 General Specifications pertaining to the most recent version of SSCC.",
#if NET7_0_OR_GREATER
                    Numeric18RegEx
#else
                    () => Numeric18RegEx
#endif
                )
            },
            {
                19009,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION",
                    "Package Identification, Generic (mutually defined)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19010,
                new EntityDescriptor(
                    "ID CODE",
                    "Machine, Cell, or Tool ID Code ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19011,
                new EntityDescriptor(
                    "FIXED ASSET ID CODE",
                    "Fixed Asset ID Code",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19012,
                new EntityDescriptor(
                    "DOCUMENT NUMBER",
                    "Document Number (internally assigned or mutually defined)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19013,
                new EntityDescriptor(
                    "CONTAINER SECURITY SEAL",
                    "Container Security Seal",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19014,
                new EntityDescriptor(
                    "4TH CLASS MANIFESTING ",
                    "4th Class Non-identical parcel post manifesting ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19015,
                new EntityDescriptor(
                    "SERIAL NUMBER ASSIGNED BY THE VENDOR ENTITY",
                    "Serial Number Assigned by the Vendor Entity, that can only be used in conjunction with '13V' ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19016,
                new EntityDescriptor(
                    "VERSION NUMBER",
                    "Version Number, e.g., Software Version",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19017,
                new EntityDescriptor(
                    "GS1 SUPPLIER AND UNIQUE PACKAGE IDENTIFICATION",
                    "Combined 6-digit GS1 Supplier Identification and Unique Package Identification Assigned by the Supplier\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19018,
                new EntityDescriptor(
                    "CAGE CODE & SERIAL NUMBER",
                    "CAGE Code & Serial Number Unique Within CAGE ",
#if NET7_0_OR_GREATER
                    AlphanumericCageSnRegEx
#else
                    () => AlphanumericCageSnRegEx
#endif
                )
            },
            {
                19019,
                new EntityDescriptor(
                    "DUNS AND PACKAGE IDENTIFICATION",
                    "Combined Dun & Bradstreet company identification of the supplier followed by a unique package identification assigned by the supplier, in the format nn...nn+nn...n where a plus symbol (+) is used as a delimiter between the DUNS Number and unique package identification",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19020,
                new EntityDescriptor(
                    "TRACEABILITY CODE",
                    "Traceability Code for an Entity Assigned by the Customer",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19021,
                new EntityDescriptor(
                    "TIRE IDENTIFICATION NUMBER",
                    "Tire Identification Number as defined by the U.S. Department of Transportation (D.O.T) under U.S. Code 49 CFR 574.5.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19022,
                new EntityDescriptor(
                    "UNIQUE INDIVIDUAL IDENTITY",
                    "Unique Individual Identity for Cellular Mobile Telephones",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19023, new EntityDescriptor(
                    "MAC ADDRESS",

                    // ReSharper disable once StringLiteralTypo
                    "Media Access Control (MAC) Address conforming with IEEE 802.11",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx12
#else
                    () => AlphanumericRegEx12
#endif
                    )
            },
            {
                19024,
                new EntityDescriptor(
                    "RF TAG UNIQUE IDENTIFIER",
                    "According to ISO/IEC 15963 (value is a conversion of its bit value to 8-bit ASCII values). This Data Identifier could possibly assume any ASCII-256 value. For freight container tags the Registration Authority (RA) for manufacturers is the RA for ISO 14816. (ISO 646)",
#if NET7_0_OR_GREATER
                    Numeric0626RegEx
#else
                    () => Numeric0626RegEx
#endif
                )
            },
            {
                19025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assign serial number that is unique within the CIN holder's domain  (See Annex C.11)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19026,
                new EntityDescriptor(
                    "READER ID",
                    "Equipment Identifier, being a globally unique identifier for a device, an item of equipment or instance of a computer application used in the production, transport, processing or other handling of items, that is constructed by concatenating:\n - an ISO/IEC 15459 issuing agency code;\n - an equipment number which accords with specifications of the issuing agency concerned; and that:\n - comprises only upper case alphabetic and/or numeric characters;\n - is unique (that is, is distinct from any other ISO/IEC 15459 compliant identifier) within the domain of the issuing agency;\n - cannot be from any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, it end.\nReader ID.\nEquipment identifier, being a globally unique identifier for a device, an item of equipment or instance of a computer application used in the production, transport, processing or other handling of items.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135

#endif
                    )
            },
            {
                19027,
                new EntityDescriptor(
                    "ITEM NUMBER WITHIN BATCH",
                    "Item Number Within Batch, being a string of numeric digits:\n - that uniquely distinguishes an item, within an identifiable batch of related items, from all other items in the same batch;\n - whose length is the same for all items within the batch concerned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19028,
                new EntityDescriptor(
                    "BATCH-AND-ITEM NUMBER",
                    "Batch-and-Item Number, being the concatenation of a data identifier 27T batch number and the data identifier 27S item number of an item belonging to the batch concerned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19030,
                new EntityDescriptor(
                    "ADDITIONAL TRACEABILITY CODE",
                    "Additional Traceability Code For An Entity Assigned by the Supplier in addition to or different from the traceability code(s) provided by 'S' or '1S'",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19031,
                new EntityDescriptor(
                    "BEGINNING SERIAL NUMBER",
                    "Beginning Serial Number for serial numbers in sequence",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19032,
                new EntityDescriptor(
                    "ENDING SERIAL NUMBER",
                    "Ending Serial Number for serial numbers in sequence",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19033,
                new EntityDescriptor(
                    "SERIAL NUMBER OF NEXT HIGHER ASSEMBLY",
                    "Serial Number of Next Higher Assembly",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                19034,
                new EntityDescriptor(
                    "PART NUMBER OF END ITEM",
                    "Serial Number or Part Number of End Item",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19035,
                new EntityDescriptor(
                    "BUMPER NUMBER",
                    "Bumper Number (Used in Unit DOD Move)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19036,
                new EntityDescriptor(
                    "PALLET IDENTIFIER",
                    "Pallet Identifier (Used for loaded 463L air pallets) ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                19037,
                new EntityDescriptor(
                    "IAC CIN PN + PSN",
                    "Unique Item Identifier comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'Part Number (PN)', followed by the '+' character, followed by the supplier assigned (or managed) 'Part Serial Number (PSN)' that is globally unique within the CIN holder's domain; in the format IAC CIN PN + PSN (spaces provided for visual clarity only; they are not part of the data). See Annex C.11.",
#if NET7_0_OR_GREATER
                    AlphanumericWithPlus01UnboundRegEx
#else
                    () => AlphanumericWithPlus01UnboundRegEx
#endif
                )
            },
            {
                19042,
                new EntityDescriptor(
                    "UII",
                    "Unique Item Identifier (UII) in 25S format preceded by numeric value indicating serial number element length for use by systems that require the 'serial number' component of a concatenated Serial Number element (IAC+CIN+SN)  Format: DI+LI+IAC+CIN+SN (LI=length of SN) ",
#if NET7_0_OR_GREATER
                    Numeric02Alphanumeric0342RegEx
#else
                    () => Numeric02Alphanumeric0342RegEx
#endif
                )
            },
            {
                19043, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "ICCID",

                    // ReSharper disable once StringLiteralTypo
                    "Integrated Circuit Card Identifier (ICCID) in accordance with ITU-T Recommendation E.118 and ETSI Recommendation GSM 11.11; a maximum of 20 digits consisting of Issuer identification number (IIN; maximum of 7 digits), Individual account identification (variable; length determined by IIN, but the same length within individual IINs), Check digit (single digit calculated using Luhn algorithm\nhttp://en.wikipedia.org/wiki/Luhn_algorithm).\n43Siiiiiiinnnnnnnnnnnnc (i = IIN, n = account identification, c = check digit)",
#if NET7_0_OR_GREATER
                    Numeric1426RegEx
#else
                    () => Numeric1426RegEx
#endif
                )
            },
            {
                19050,
                new EntityDescriptor(
                    "FIRST LEVEL",
                    "First Level (Supplier Assigned)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                    )
            },
            {
                19051,
                new EntityDescriptor(
                    "SECOND LEVEL",
                    "Second Level (Supplier Assigned)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                    )
            },
            {
                19052,
                new EntityDescriptor(
                    "THIRD LEVEL",
                    "Third Level (Supplier Assigned)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                    )
            },
            {
                19053,
                new EntityDescriptor(
                    "FOURTH LEVEL",
                    "Fourth Level (Supplier Assigned)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                    )
            },
            {
                19054,
                new EntityDescriptor(
                    "FIFTH LEVEL",
                    "Fifth Level (Supplier Assigned)",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0120
#else
                    () => AlphanumericRegEx0120
#endif
                )
            },
            {
                19096, new EntityDescriptor(
                    "EPC NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "EPC number (Typically Serialized Global Trade Identification Number - SGTIN)\n2020 Update: The term “EPC number” is no longer used by GS1. Refer to GS1 General Specifications and the Tag Data Standard for current terminology",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx1626
#else
                    () => AlphanumericRegEx1626
#endif
                    )
            },
            {
                19097,
                new EntityDescriptor(
                    "ENCRYPTED SERIAL NUMBER",
                    "Encrypted serial number assigned by the Supplier to an entity, which can be authenticated by an independent trusted third party. The encrypted serial number does not describe any parameters of the entity without decryption by an independent third party.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0425
#else
                    () => AlphanumericRegEx0425
#endif
                    )
            },
            {
                20000,
                new EntityDescriptor(
                    "TRACEABILITY NUMBER ASSIGNED BY THE CUSTOMER",
                    "Traceability Number assigned by the Customer to identify/trace a unique group of entities (e.g., lot, batch, heat).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20001,
                new EntityDescriptor(
                    "TRACEABILITY NUMBER ASSIGNED BY THE SUPPLIER",
                    "Traceability Number assigned by the Supplier to identify/trace a unique group of entities (e.g., lot, batch, heat).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20003,
                new EntityDescriptor(
                    "EXCLUSIVE ASSIGNMENT",
                    "Exclusive Assignment (U.S. EPA vehicle identification for emissions testing).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20020,
                new EntityDescriptor(
                    "FIRST LEVEL (CUSTOMER ASSIGNED)",
                    "First Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20021,
                new EntityDescriptor(
                    "SECOND LEVEL (CUSTOMER ASSIGNED)",
                    "Second Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20022,
                new EntityDescriptor(
                    "THIRD LEVEL (CUSTOMER ASSIGNED)",
                    "Third Level (Customer Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20023,
                new EntityDescriptor(
                    "FOURTH LEVEL (CUSTOMER ASSIGNED",
                    "Fourth Level (Customer Assigned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20024,
                new EntityDescriptor(
                    "FIFTH LEVEL (CUSTOMER ASSIGNED)",
                    "Fifth Level (Customer Assigned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20025,
                new EntityDescriptor(
                    "PARTY TO A TRANSACTION",
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assigned traceability number that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20026,
                new EntityDescriptor(
                    "BATCH IDENTIFIER",
                    "Batch Identifier comprising the concatenation of either:\n - a data identifier 26S mail processing equipment identifier, or\n - a data identifier 20K license identifier, or\n - a data identifier 18V party identifier that:\n - is distinct from any other ISO/IEC 15459 compliant identifier within the domain of the issuing agency concerned;\n - cannot be derived from another party identifier or any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, its end;\n with a data identifier 27T batch number, the two being separated by a dash (-) character.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20027,
                new EntityDescriptor(
                    "BATCH NUMBER",
                    "Batch Number, issued under the control of an identified party or unit of processing equipment, or under the provisions of an identified license, that:\n - uniquely distinguishes one batch of related items from all other batches to which a batch number is assigned by the party or equipment, or under the license, concerned;\n - comprises a string of maximum length 10 characters, of which the first (numeric) character indicates the number of following characters, each of which is taken from the set {0-9; A-Z}.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20030,
                new EntityDescriptor(
                    "FIRST LEVEL (SUPPLIER ASSIGNED)",
                    "First Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20031,
                new EntityDescriptor(
                    "SECOND LEVEL (SUPPLIER ASSIGNED)",
                    "Second Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20032,
                new EntityDescriptor(
                    "THIRD LEVEL (SUPPLIER ASSIGNED) ",
                    "Third Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20033,
                new EntityDescriptor(
                    "FOURTH LEVEL (SUPPLIER ASSIGNED) ",
                    "Fourth Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                20034,
                new EntityDescriptor(
                    "FIFTH LEVEL (SUPPLIER ASSIGNED) ",
                    "Fifth Level (Supplier Assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21005,
                new EntityDescriptor(
                    "POSTAL SERVICE",
                    "Specification of a postal service and associated process data in accordance with UPU standard S25 data construct 'Service Data'",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21006,
                new EntityDescriptor(
                    "LICENSING POST DATA",
                    "Licensing Post Data, in accordance with the specification in UPU standard S25.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21015,
                new EntityDescriptor(
                    "SUPPLEMENTARY POSTAL SERVICE",
                    "Specification of supplementary postal service and associated process data in accordance with UPU standard S25 data construct 'Supplementary Service Data'.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21016,
                new EntityDescriptor(
                    "POSTAL ADMINISTRATION IDENTIFICATIONS",
                    "Postal Administration Identifications, being the identification, expressed in accordance with the specification in UPU standard S25, of one or more postal administrations involved in the processing of a mail item or batch.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21017, new EntityDescriptor(
                    "UPU LOCATION CODE",

                    // ReSharper disable once StringLiteralTypo
                    "UPU Location Code, being a code identifying a location or geographic area, or an associated group of such locations or areas, that has relevance to a related transaction and that complies with one of the structures defined in a) to g) below:\na) two upper case alphabetic characters corresponding to the ISO 3166-1 two alpha country code of the country in which, or consisting of which, the location(s) or area(s) are situated;\nb) three upper case alphabetic characters corresponding to the IATA code of the airport or city in, close to, or consisting of which the location(s) or area(s) are situated;\nc) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dash (-), with the balance being a postcode in the country concerned;\nd) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dot (.), with the balance being an ISO 3166-2 country subdivision code in the country concerned;\ne) five upper case alphabetic characters corresponding to the UN/LOCODE of the area in, close to, or consisting of which, the location(s) or area(s) are situated;\nf) six upper case alphanumeric characters corresponding to a UPU IMPC code allocated in accordance with UPU standard S34;\ng) the concatenation, being not less than seven nor more than 25 characters in length, of:\n - an issuer code allocated in accordance with UPU standards S31;\n - a location code, consisting of characters drawn from the set {A-Z; 0-9} which accords with specifications of the issuer concerned.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21018,
                new EntityDescriptor(
                    "QUALIFIED UPU LOCATION CODE",
                    "Qualified UPU Location Code, concatenation of:\n - a location category drawn from UPU code list 139;\n - a data identifier 17U UPU location code.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21019,
                new EntityDescriptor(
                    "LICENSE PLATE WITH SERVICE DATA AND LOCATION CODE",
                    "License Plate with Service Data and Location Code is a compound data construct, compliant with the specification in UPU standard S25, which includes specification of:\n - an ISO/IEC 15459-compliant item identifier;\n - a data identifier 5U compliant specification of the service to be provided in respect of the item;\n - a data identifier 17U compliant UPU location code or a data identifier 18U compliant qualified UPU location code.\n Note - For further details, please refer to UPU standard S25. The distinction between a simple UPU location code (DI 17U) and a qualified UPU location code (DI 18U) can be determined from the first character. If this is numeric, 18U applies; if it is alphabetic, 17U applies.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                21055,
                new EntityDescriptor(
                    "OCR DATA LOCATOR",
                    "OCR Data Locator.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22000,
                new EntityDescriptor(
                    "SUPPLIER CODE ASSIGNED BY CUSTOMER",
                    "Supplier Code Assigned by Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22001,
                new EntityDescriptor(
                    "SUPPLIER CODE ASSIGNED BY SUPPLIER",
                    "Supplier Code Assigned by Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22002,
                new EntityDescriptor(
                    "U.P.C. COMPANY PREFIX",
                    "U.P.C. Company Prefix.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.\nAccording to GS1, the term “U.P.C.” is no longer used and the metadata (an2+n8) is no longer correct.GS1 recommends this DI not be used for future applications.",
#if NET7_0_OR_GREATER
                    Numeric09RegEx
#else
                    () => Numeric09RegEx
#endif
                )
            },
            {
                22003,
                new EntityDescriptor(
                    "GS1 COMPANY PREFIX", 
                    "GS1 Company Prefix.\n2020 Update: GS1 recommends this DI no longer be used and that users of this DI migrate to GS1 data qualifiers (Application Identifiers) per ISO/IEC 15418 and ISO/IEC 15459-3.\nAccording to GS1, the metadata (an2+n9) is no longer correct.GS1 recommends this DI not be used for future applications.", 
#if NET7_0_OR_GREATER
                    Numeric09RegEx
#else
                    () => Numeric09RegEx
#endif
                )
            },
            {
                22004,
                new EntityDescriptor(
                    "CARRIER IDENTIFICATION CODE",
                    "Carrier Identification Code assigned by an industry standard mutually defined by the Supplier, Carrier, and Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22005,
                new EntityDescriptor(
                    "FINANCIAL INSTITUTION IDENTIFICATION CODE",
                    "Financial Institution Identification Code (mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22006,
                new EntityDescriptor(
                    "MANUFACTURER'S IDENTIFICATION CODE",
                    "Manufacturer's Identification Code (mutually defined.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22007,
                new EntityDescriptor(
                    "LIABLE PARTY",
                    "Code assigned to a party which has financial liability for an entity or group of entities (e.g., owner of inventory) (mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22008,
                new EntityDescriptor(
                    "CUSTOMER CODE ASSIGNED BY THE CUSTOMER",
                    "Customer Code Assigned by the Customer.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22009,
                new EntityDescriptor(
                    "CUSTOMER CODE ASSIGNED BY THE SUPPLIER",
                    "Customer Code Assigned by the Supplier.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22010,
                new EntityDescriptor(
                    "MANUFACTURER ID",
                    "Manufacturer ID\nNOTE - See Appendix 2, CBP 7501 Instructions.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx1015
#else
                    () => AlphanumericRegEx1015
#endif
                    )
            },
            {
                22011,
                new EntityDescriptor(
                    "BUDGET HOLDER",
                    "Organisation with budget responsibility for an entity, process, or procedure (e.g., shop, division, department)(internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22012,
                new EntityDescriptor(
                    "MANUFACTURER DUNS NUMBER",
                    "DUNS Number Identifying Manufacturer.",
#if NET7_0_OR_GREATER
                    Numeric0913RegEx
#else
                    () => Numeric0913RegEx
#endif
                )
            },
            {
                22013,
                new EntityDescriptor(
                    "SUPPLIER DUNS NUMBER",
                    "DUNS Number Identifying Supplier.",
#if NET7_0_OR_GREATER
                    Numeric0913RegEx
#else
                    () => Numeric0913RegEx
#endif
                )
            },
            {
                22014,
                new EntityDescriptor(
                    "CUSTOMER DUNS NUMBER",
                    "DUNS Number Identifying Customer.",
#if NET7_0_OR_GREATER
                    Numeric0913RegEx
#else
                    () => Numeric0913RegEx
#endif
                )
            },
            {
                22015,
                new EntityDescriptor(
                    "CARRIER-ASSIGNED SHIPPER NUMBER",
                    "Carrier-Assigned Shipper Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SUPPLIER ID",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS Supplier ID.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22017,
                new EntityDescriptor(
                    "DOD CAGE CODE", 
                    "U.S. DoD CAGE Code.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx5
#else
                    () => AlphanumericRegEx5
#endif
                    )
            },
            {
                22018,
                new EntityDescriptor(
                    "PARTY TO A TRANSACTION",
                    "Identification of a party to a transaction in which the data format consists of two concatenated segments. The first segment is the Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, the second segment is a unique entity identification Company Identification Number (CIN) assigned in accordance with rules established by the issuing agency (see http://www.aimglobal.org/?page=Reg_Authority15459&hhSear chTerms=%22IAC%22).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22019, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "PARTYS ROLE(S) IN A TRANSACTION",

                    // ReSharper disable once StringLiteralTypo
                    "Specification of a party's role(s), in a transaction, consisting of one or more code values from EDIFACT Code List 3035 'Party Qualifier', separated by plus (+) characters (Never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22020, new EntityDescriptor(
                    "IAC CIN CODE VALUES IDENTIFICATION",

                    // ReSharper disable once StringLiteralTypo
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3035 'Party Qualifier', separated by plus (+) characters (Never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22021,
                new EntityDescriptor(
                    "IAC CIN SUB UNIT IDENTIFICATION",
                    "Identification of a party to a transaction, e.g., 21V IAC CIN OSU, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the organisational sub-unit identification assigned by the CIN that is unique within the CIN holder's domain.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                22022, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CARRIER SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "Carrier SCAC.\nStandard Carrier Alpha Code - The National Motor Freight Traffic Association, Inc., (NMFTA) assigns SCACs for all companies except those codes used for identification of freight containers not operating exclusively in North America, intermodal chassis and trailers, non-railroad owned rail cars, and railroads. http://www.nmfta.org/Pages/welcome.aspx\nCompanies seeking identification codes for freight containers not operating in North America should contact the Bureau International des Containers, 38, rue des Blancs Manteaux, F75004 Paris, France, email:  bic@bic-code.org, web www.biccode.org. Railroads and owners of intermodal chassis, trailers and non-railroad owned rail cars should contact Railinc Customer Service, Attn: Private Marks, 7001 Weston Parkway, Suite 200, Cary, NC 27513, (800) 544-7245, email: private.marks@railinc.com.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx4
#else
                    () => AlphanumericRegEx4
#endif
                    )
            },
            {
                22023, new EntityDescriptor(
                    "SUPPLIER VAT NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Government-assigned Value Added Tax identification number identifying supplier, starting with an ISO 3166-1 alpha-2 country code (except for Greece, which uses the ISO 639-1 language code EL), followed by the government-assigned VAT number.\nExample:\n23VIE6388047V  assigned to Google IrelanD.",
#if NET7_0_OR_GREATER
                    Alpha02Alphanumeric0318RegEx
#else
                    () => Alpha02Alphanumeric0318RegEx
#endif
                )
            },
            {
                22024,
                new EntityDescriptor(
                    "CUSTOMER VAT NUMBER",
                    "Government-assigned Value Added Tax identification number identifying customer, starting with an ISO 3166-1 alpha-2 country code (except for Greece, which uses the ISO 639-1 language code EL), followed by the government-assigned VAT number.\nExample:\n24VIE6388047V  assigned to Google Ireland.",
#if NET7_0_OR_GREATER
                    Alpha02Alphanumeric0318RegEx
#else
                    () => Alpha02Alphanumeric0318RegEx
#endif
                )
            },
            {
                22025,
                new EntityDescriptor(
                    "NCAGE CAGE MANUFACTURER CODE",
                    "Declaring that the NCAGE/CAGE code that follows DI 25V is the Manufacturer. Party to a transaction wherein the NATO Commercial And Government Entity (NCAGE) / Commercial And Government Entity (CAGE) code used behind DI 25V is declared to be the manufacturer of the item(s) involved in the transaction. Data following DI 25V will consist of five upper\u0002case alphanumeric characters, excluding the letters “I” and “O”.",
#if NET7_0_OR_GREATER
                    AlphanumericRegEx5
#else
                    () => AlphanumericRegEx5
#endif
                    )
            },
            {
                23000,
                new EntityDescriptor(
                    "WORK ORDER NUMBER",
                    "Work Order Number (e.g., 'Production Paper') (internally assigned).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23001,
                new EntityDescriptor(
                    "OPERATION SEQUENCE NUMBER",
                    "Operation Sequence Number. A number that defines the order of a particular operation in a series of operations, generally in a manufacturing or assembly process.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23002,
                new EntityDescriptor(
                    "OPERATION CODE",
                    "Operation Code/Work Code - the type of work to be performed (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23003,
                new EntityDescriptor(
                    "WORK ORDER AND OPERATION SEQUENCE NUMBER",
                    "Combined Work Order Number and Operation Sequence Number in the format nn...n+nn...n where a plus symbol (+) is used as a delimiter between the Work Order Number and the Operation Sequence Number.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23004,
                new EntityDescriptor(
                    "STATUS CODE",
                    "Status Code (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23005,
                new EntityDescriptor(
                    "WORK UNIT CODE",
                    "Work Unit Code - identifies system, subsystem, assembly, component etc. on which maintenance is performed.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23006,
                new EntityDescriptor(
                    "NOMENCLATURE",
                    "Nomenclature - (internally assigned or mutually defined).",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23010, new EntityDescriptor(
                    "FORM CONTROL NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Form Control Number - Preprinted control number on forms.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23011,
                new EntityDescriptor(
                    "QUALITY ASSURANCE INSPECTOR",
                    "Quality Assurance Inspector - Last Name.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                23012,
                new EntityDescriptor(
                    "TELEPHONE NUMBER",
                    "Telephone Number of the Person/Activity Completing the Form - expressed in the format (country code) city or area code plus local number i.e. (1) 319 555 1212.",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26000,
                new EntityDescriptor(
                    "(CUSTOMER-SUPPLIER)",
                    "Mutually Defined Between Customer and Supplier ",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26001,
                new EntityDescriptor(
                    "(CARRIER-SUPPLIER)",
                    "Mutually Defined Between Carrier and Supplier",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26002,
                new EntityDescriptor(
                    "(CUSTOMER-CARRIER)",
                    "Mutually Defined Between Customer and Carrier",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26003,
                new EntityDescriptor(
                    "FREE TEXT", 
                    "Free Text", 
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26004,
                new EntityDescriptor(
                    "(CARRIER-TRADING PARTNER)",
                    "Mutually Defined Between Carrier and Trading Partner",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26010,
                new EntityDescriptor(
                    "HEADER",
                    "Structured Free Text  (Header Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26011,
                new EntityDescriptor(
                    "LINE 1",
                    "Structured Free Text (Line 1 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26012,
                new EntityDescriptor(
                    "LINE 2",
                    "Structured Free Text (Line 2 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26013,
                new EntityDescriptor(
                    "LINE 3",
                    "Structured Free Text (Line 3 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                26014,
                new EntityDescriptor(
                    "LINE 4",
                    "Structured Free Text (Line 4 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26015,
                new EntityDescriptor(
                    "LINE 5",
                    "Structured Free Text (Line 5 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26016,
                new EntityDescriptor(
                    "LINE 6",
                    "Structured Free Text (Line 6 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26017,
                new EntityDescriptor(
                    "LINE 7",
                    "Structured Free Text (Line 7 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26018,
                new EntityDescriptor(
                    "LINE 8",
                    "Structured Free Text (Line 8 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26019,
                new EntityDescriptor(
                    "LINE 9",
                    "Structured Free Text (Line 9 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26020,
                new EntityDescriptor(
                    "LINE 10",
                    "Structured Free Text (Line 10 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26021,
                new EntityDescriptor(
                    "LINE 11",
                    "Structured Free Text (Line 11 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26022,
                new EntityDescriptor(
                    "LINE 12",
                    "Structured Free Text (Line 12 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26023,
                new EntityDescriptor(
                    "LINE 13",
                    "Structured Free Text (Line 13 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26024,
                new EntityDescriptor(
                    "LINE 14",
                    "Structured Free Text (Line 14 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26025,
                new EntityDescriptor(
                    "LINE 15",
                    "Structured Free Text (Line 15 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26026,
                new EntityDescriptor(
                    "LINE 16",
                    "Structured Free Text (Line 16 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26027,
                new EntityDescriptor(
                    "LINE 17",
                    "Structured Free Text (Line 17 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26028,
                new EntityDescriptor(
                    "LINE 18",
                    "Structured Free Text (Line 18 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26029,
                new EntityDescriptor(
                    "LINE 19",
                    "Structured Free Text (Line 19 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26030,
                new EntityDescriptor(
                    "LINE 20",
                    "Structured Free Text (Line 20 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26031,
                new EntityDescriptor(
                    "LINE 21",
                    "Structured Free Text (Line 21 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26032,
                new EntityDescriptor(
                    "LINE 22",
                    "Structured Free Text (Line 22 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26033,
                new EntityDescriptor(
                    "LINE 23",
                    "Structured Free Text (Line 23 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26034,
                new EntityDescriptor(
                    "LINE 24",
                    "Structured Free Text (Line 24 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26035,
                new EntityDescriptor(
                    "LINE 25",
                    "Structured Free Text (Line 25 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26036,
                new EntityDescriptor(
                    "LINE 26",
                    "Structured Free Text (Line 26 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26037,
                new EntityDescriptor(
                    "LINE 27",
                    "Structured Free Text (Line 27 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26038,
                new EntityDescriptor(
                    "LINE 28",
                    "Structured Free Text (Line 28 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26039,
                new EntityDescriptor(
                    "LINE 29",
                    "Structured Free Text (Line 29 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26040,
                new EntityDescriptor(
                    "LINE 30",
                    "Structured Free Text (Line 30 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26041,
                new EntityDescriptor(
                    "LINE 31",
                    "Structured Free Text (Line 31 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26042,
                new EntityDescriptor(
                    "LINE 32",
                    "Structured Free Text (Line 32 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26043,
                new EntityDescriptor(
                    "LINE 33",
                    "Structured Free Text (Line 33 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26044,
                new EntityDescriptor(
                    "LINE 34",
                    "Structured Free Text (Line 34 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26045,
                new EntityDescriptor(
                    "LINE 35",
                    "Structured Free Text (Line 35 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26046,
                new EntityDescriptor(
                    "LINE 36",
                    "Structured Free Text (Line 36 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26047,
                new EntityDescriptor(
                    "LINE 37",
                    "Structured Free Text (Line 37 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26048,
                new EntityDescriptor(
                    "LINE 38",
                    "Structured Free Text (Line 38 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26049,
                new EntityDescriptor(
                    "LINE 39",
                    "Structured Free Text (Line 39 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26050,
                new EntityDescriptor(
                    "LINE 40",
                    "Structured Free Text (Line 40 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26051,
                new EntityDescriptor(
                    "LINE 41",
                    "Structured Free Text (Line 41 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26052,
                new EntityDescriptor(
                    "LINE 42",
                    "Structured Free Text (Line 42 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26053,
                new EntityDescriptor(
                    "LINE 43",
                    "Structured Free Text (Line 43 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26054,
                new EntityDescriptor(
                    "LINE 44",
                    "Structured Free Text (Line 44 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26055,
                new EntityDescriptor(
                    "LINE 45",
                    "Structured Free Text (Line 45 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26056,
                new EntityDescriptor(
                    "LINE 46",
                    "Structured Free Text (Line 46 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26057,
                new EntityDescriptor(
                    "LINE 47",
                    "Structured Free Text (Line 47 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26058,
                new EntityDescriptor(
                    "LINE 48",
                    "Structured Free Text (Line 48 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26059,
                new EntityDescriptor(
                    "LINE 49",
                    "Structured Free Text (Line 49 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26060,
                new EntityDescriptor(
                    "LINE 50",
                    "Structured Free Text (Line 50 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26061,
                new EntityDescriptor(
                    "LINE 51",
                    "Structured Free Text (Line 51 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26062,
                new EntityDescriptor(
                    "LINE 52",
                    "Structured Free Text (Line 52 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26063,
                new EntityDescriptor(
                    "LINE 53",
                    "Structured Free Text (Line 53 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26064,
                new EntityDescriptor(
                    "LINE 54",
                    "Structured Free Text (Line 54 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26065,
                new EntityDescriptor(
                    "LINE 55",
                    "Structured Free Text (Line 55 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26066,
                new EntityDescriptor(
                    "LINE 56",
                    "Structured Free Text (Line 56 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26067,
                new EntityDescriptor(
                    "LINE 57",
                    "Structured Free Text (Line 57 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26068,
                new EntityDescriptor(
                    "LINE 58",
                    "Structured Free Text (Line 58 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26069,
                new EntityDescriptor(
                    "LINE 59",
                    "Structured Free Text (Line 59 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26070,
                new EntityDescriptor(
                    "LINE 60",
                    "Structured Free Text (Line 60 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26071,
                new EntityDescriptor(
                    "LINE 61",
                    "Structured Free Text (Line 61 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26072,
                new EntityDescriptor(
                    "LINE 62",
                    "Structured Free Text (Line 62 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26073,
                new EntityDescriptor(
                    "LINE 63",
                    "Structured Free Text (Line 63 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26074,
                new EntityDescriptor(
                    "LINE 64",
                    "Structured Free Text (Line 64 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26075,
                new EntityDescriptor(
                    "LINE 65",
                    "Structured Free Text (Line 65 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26076,
                new EntityDescriptor(
                    "LINE 66",
                    "Structured Free Text (Line 66 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26077,
                new EntityDescriptor(
                    "LINE 67",
                    "Structured Free Text (Line 67 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26078,
                new EntityDescriptor(
                    "LINE 68",
                    "Structured Free Text (Line 68 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26079,
                new EntityDescriptor(
                    "LINE 69",
                    "Structured Free Text (Line 69 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26080,
                new EntityDescriptor(
                    "LINE 70",
                    "Structured Free Text (Line 70 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26081,
                new EntityDescriptor(
                    "LINE 71",
                    "Structured Free Text (Line 71 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26082,
                new EntityDescriptor(
                    "LINE 72",
                    "Structured Free Text (Line 72 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26083,
                new EntityDescriptor(
                    "LINE 73",
                    "Structured Free Text (Line 73 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26084,
                new EntityDescriptor(
                    "LINE 74",
                    "Structured Free Text (Line 74 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26085,
                new EntityDescriptor(
                    "LINE 75",
                    "Structured Free Text (Line 75 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26086,
                new EntityDescriptor(
                    "LINE 76",
                    "Structured Free Text (Line 76 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26087,
                new EntityDescriptor(
                    "LINE 77",
                    "Structured Free Text (Line 77 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26088,
                new EntityDescriptor(
                    "LINE 78",
                    "Structured Free Text (Line 78 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26089,
                new EntityDescriptor(
                    "LINE 79",
                    "Structured Free Text (Line 79 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26090,
                new EntityDescriptor(
                    "LINE 80",
                    "Structured Free Text (Line 80 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26091,
                new EntityDescriptor(
                    "LINE 81",
                    "Structured Free Text (Line 81 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26092,
                new EntityDescriptor(
                    "LINE 82",
                    "Structured Free Text (Line 82 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26093,
                new EntityDescriptor(
                    "LINE 83",
                    "Structured Free Text (Line 83 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26094,
                new EntityDescriptor(
                    "LINE 84",
                    "Structured Free Text (Line 84 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26095,
                new EntityDescriptor(
                    "LINE 85",
                    "Structured Free Text (Line 85 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26096,
                new EntityDescriptor(
                    "LINE 86",
                    "Structured Free Text (Line 86 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26097,
                new EntityDescriptor(
                    "LINE 87",
                    "Structured Free Text (Line 87 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                26098,
                new EntityDescriptor(
                    "LINE 88",
                    "Structured Free Text (Line 88 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                26099,
                new EntityDescriptor(
                    "LINE 89",
                    "Structured Free Text (Line 89 Data)",
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            }
        };

    /// <summary>
    ///   Resolve a first two digits of the data identifier into an entity.
    /// </summary>
    /// <param name="data">
    ///   The data buffer.
    /// </param>
    /// <param name="dataIdentifier">
    ///   The data identifier.
    /// </param>
    /// <param name="currentPosition">
    ///   The position of the data identifier for the current field.
    /// </param>
    /// <param name="includeDescriptors">
    ///   Indicates whether the descriptors should be included in the resolved identifier.
    /// </param>
    /// <returns>
    ///   An entity.
    /// </returns>
    public static ResolvedDataIdentifier Resolve(
        this string data,
        string? dataIdentifier,
        int currentPosition,
        bool includeDescriptors = true)
    {
        // Get the last character of the data identifier
        var descriptorKey = -1;
        const int numberOfDecimalPlaces = -1;

        var fnc1 = ((char)29).ToInvariantString();
        var isoIec15434Preamble = "[)>" + (char)30;

        // Resolve the descriptor key
        switch (dataIdentifier)
        {
            case "+":
                descriptorKey = 0;
                break;
            case "&":
                descriptorKey = 2;
                break;
            case "=":
                descriptorKey = 3;
                break;
            case "-":
                descriptorKey = 6;
                break;
            case "!":
                descriptorKey = 7;
                break;
            default:
                // Resolve the remaining category 0 DIs
                if (dataIdentifier == fnc1)
                {
                    descriptorKey = 4;
                    break;
                }

                if (dataIdentifier == isoIec15434Preamble)
                {
                    descriptorKey = 5;
                    break;
                }

                if (dataIdentifier?.Length == 0)
                {
                    return new ResolvedDataIdentifier(
                        new ParserException(3002, Resources.Ansi_Mh10_8_2_Error_010, false),
                        currentPosition);
                }

                var lastChar = dataIdentifier?
#if NET6_0_OR_GREATER
                    [^1..]
#else
                    .Substring(dataIdentifier.Length - 2)
#endif
                    ?.ToInvariantUpper();
                var ascii = lastChar?[0] - 64;

                if (ascii is < 1 or > 65)
                {
                    break;
                }

                if (dataIdentifier?.Length > 1)
                {
                    var entityValuePart = dataIdentifier
#if NET6_0_OR_GREATER
                        [..^1];
#else
                        .Substring(0, dataIdentifier.Length - 1);
#endif

                    if (entityValuePart.Length > 3)
                    {
                        break;
                    }

                    if (!int.TryParse(entityValuePart, out var entityValue))
                    {
                        break;
                    }

                    descriptorKey = (ascii ?? 0) * 1000 + entityValue;
                }
                else
                {
                    descriptorKey = (ascii ?? 0) * 1000;
                }

                break;
        }

        if (descriptorKey <= -1)
        {
            return new ResolvedDataIdentifier(
                new ParserException(
                    3002,
                    string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_009, dataIdentifier ?? "<unknown>"),
                    false),
                currentPosition,
                data);
        }

        if (!includeDescriptors)
        {
            return new ResolvedDataIdentifier(
                descriptorKey,
                dataIdentifier ?? string.Empty,
                numberOfDecimalPlaces,
                data,
                string.Empty,
                string.Empty,
                currentPosition);
        }

        EntityDescriptor descriptors;

        try
        {
            descriptors = descriptorKey.GetDescriptors();
        }
        catch (KeyNotFoundException)
        {
            return new ResolvedDataIdentifier(
                new ParserException(3002, $"Invalid data identifier {dataIdentifier}.", false),
                currentPosition,
                new ResolvedDataIdentifier(
                    -1,
                    dataIdentifier ?? string.Empty,
                    numberOfDecimalPlaces,
                    data,
                    string.Empty,
                    string.Empty,
                    currentPosition));
        }

        return Validate(
            new ResolvedDataIdentifier(
                descriptorKey,
                dataIdentifier ?? string.Empty,
                numberOfDecimalPlaces,
                data,
                descriptors.DataTitle,
                descriptors.Description,
                currentPosition));
    }

    /// <summary>
    ///   Returns the descriptors tuple for the given data identifier descriptor key.
    /// </summary>
    /// <param name="descriptorKey">The data identifier descriptor key.</param>
    /// <returns>The descriptors tuple for the given The data identifier descriptor key.</returns>
    private static EntityDescriptor GetDescriptors(this int descriptorKey)
    {
        return descriptorKey < 0 ? new EntityDescriptor(null, null, () => new Regex(string.Empty)) : Descriptors[descriptorKey];
    }

    /// <summary>
    ///   Validates a resolved entity.
    /// </summary>
    /// <param name="resolvedEntity">The resolved entity to be validated.</param>
    /// <returns>A resolved entity object. If the value is invalid, the object records the error.</returns>
    private static ResolvedDataIdentifier Validate(ResolvedDataIdentifier? resolvedEntity)
    {
        try
        {
            var identifierString = (resolvedEntity?.Identifier ?? string.Empty).Length > 0
                                       ? " " + resolvedEntity?.Identifier
                                       : string.Empty;

            if (resolvedEntity is null)
            {
                return new ResolvedDataIdentifier(
                    new ParserException(
                        3005,
                        string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_008, identifierString),
                        false),
                    0);
            }

            var valueString = resolvedEntity.Value.Length > 0
                                  ? " " + resolvedEntity.Value
                                  : string.Empty;

            var identifier =
                Descriptors[resolvedEntity.Entity].IsValid(
                    resolvedEntity.Value,
                    out var validationErrors)
                    ? resolvedEntity
                    : new ResolvedDataIdentifier(
                        new ParserException(
                            3005,
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Ansi_Mh10_8_2_Error_007,
                                valueString,
                                resolvedEntity.Identifier.Trim()),
                            false),
                        resolvedEntity.CharacterPosition,
                        resolvedEntity);

            // Add additional resolver exceptions to the collection
            foreach (var parserException in validationErrors)
            {
                identifier.AddException(parserException);
            }

            return identifier;
        }
        catch (ArgumentNullException)
        {
            var identifierString = (resolvedEntity?.Identifier ?? string.Empty).Length > 0
                                       ? " " + resolvedEntity?.Identifier
                                       : string.Empty;

            return new ResolvedDataIdentifier(
                new ParserException(
                    3006,
                    string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_006, identifierString),
                    false),
                resolvedEntity?.CharacterPosition ?? 0,
                resolvedEntity);
        }
        catch (RegexMatchTimeoutException)
        {
            var identifierString = (resolvedEntity?.Identifier ?? string.Empty).Length > 0
                                       ? " " + resolvedEntity?.Identifier
                                       : string.Empty;

            return new ResolvedDataIdentifier(
                new ParserException(
                    3007,
                    string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_005, identifierString),
                    true),
                resolvedEntity?.CharacterPosition ?? 0,
                resolvedEntity);
        }
    }
}