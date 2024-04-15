// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityResolver.cs" company="Solidsoft Reply Ltd">
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
// The entity resolver for ASC (MH10.8) data identifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable BadListLineBreaks
#pragma warning disable SA1009
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
     Justification = "Resharper does not hide errors correctly using in-line comments.")]
[SuppressMessage(
     "ReSharper",
     "CommentTypo",
     Justification = "Resharper does not hide errors correctly using in-line comments.")]
#if NET7_0_OR_GREATER
public static partial class EntityResolver {
#else
public static class EntityResolver {
#endif

#if !NET7_0_OR_GREATER
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
    /// Regular Expression: Alphanumeric {1 ..}.
    /// </summary>
    private static readonly Regex Alphanumeric01UnboundRegEx = new(@"[0-9A-Z]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric with plus {1 ..}.
    /// </summary>
    private static readonly Regex AlphanumericWithPlus01UnboundRegEx = new(@"[0-9A-Z+]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Invariants except plus {1 ..}.
    /// </summary>
    private static readonly Regex InvariantNoPlus01UnboundPlusRegEx = new(@"[-!""%&'()*,./0-9:;<=>?A-Z_a-z]+", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric with plus {1 ..}.
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
    ///    Returns a regular expression for the message header preamble defined by ISO/IEC 15434.
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
    private static readonly Regex UniformResourceLocatorRegEx = new(@"^(?:(?:http|https|ftp|telnet|gopher|ms\-help|file|notes)://)?(?:(?:[a-z][\w~%!&amp;',;=\-\.$\(\)\*\+]*):.*@)?(?:(?:[a-z0-9][\w\-]*[a-z0-9]*\.)*(?:(?:(?:(?:[a-z0-9][\w\-]*[a-z0-9]*)(?:\.[a-z0-9]+)?)|(?:(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))(?::[0-9]+)?))?(?:(?:(?:/(?:[\w`~!$=;\-\+\.\^\(\)\|\{\}\[\]]|(?:%\d\d))+)*/(?:[\w`~!$=;\-\+\.\^\(\)\|\{\}\[\]]|(?:%\d\d))*)(?:\?[^#]+)?(?:#[a-z0-9]\w*)?)?$", RegexOptions.None);

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length for CAGE Code and Serial number.
    /// </summary>
    private static readonly Regex AlphanumericCageSnRegEx = new(@"[0-9A-Z-/]{6,25}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx2 = new(@"[0-9A-Z]{2}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx3 = new(@"[0-9A-Z]{3}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx4 = new(@"[0-9A-Z]{4}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx5 = new(@"[0-9A-Z]{5}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {6}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx6 = new(@"[0-9A-Z]{6}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx10 = new(@"[0-9A-Z]{10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {12}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx12 = new(@"[0-9A-Z]{12}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {18}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx18 = new(@"[0-9A-Z]{18}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 3}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0103 = new(@"[0-9A-Z]{1,3}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 9}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0109 = new(@"[0-9A-Z]{1,9}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 10}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0110 = new(@"[0-9A-Z]{1,10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 20}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0120 = new(@"[0-9A-Z]{1,20}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 35}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0135 = new(@"[0-9A-Z]{1,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 50}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0150 = new("[0-9A-Z]{1,50}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 100}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx01100 = new(@"[0-9A-Z]{1,100}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 30}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0230 = new(@"[0-9A-Z]{2,30}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {2, 35}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0235 = new(@"[0-9A-Z]{2,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 9}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0309 = new(@"[0-9A-Z]{3,9}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 22}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0322 = new(@"[0-9A-Z]{3,22}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0335 = new(@"[0-9A-Z]{3,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 11}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0411 = new(@"[0-9A-Z]{4,11}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4, 25}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0425 = new(@"[0-9A-Z]{4,25}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 29}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0529 = new(@"[0-9A-Z]{5,29}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 16}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0516 = new(@"[0-9A-Z]{5,16}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {5, 22}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0522 = new(@"[0-9A-Z]{5,22}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {6, 35}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx0635 = new(@"[0-9A-Z]{6,35}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 12}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx1012 = new(@"[0-9A-Z]{10,12}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {10, 15}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx1015 = new(@"[0-9A-Z]{10,15}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {13, 15}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx1315 = new(@"[0-9A-Z]{13,15}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {16, 26}.
    /// </summary>
    private static readonly Regex AlphanumericRegEx1626 = new(@"[0-9A-Z]{16,26}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {4} . Alphanumeric {1, 10}.
    /// </summary>
    private static readonly Regex Alphanumeric04Alphanumeric0110RegEx = new(@"[0-9A-Z]{4}.[0-9A-Z]{1,10}", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {1, 32} Alphanumeric {3} with leading dashes.
    /// </summary>
    private static readonly Regex Alphanumeric0132Alphanumeric03WithDashesRegEx = new(@"[0-9A-Z]{1,32}([0-9A-Z]{3}|-[0-9A-Z]{2}|--[0-9A-Z]{1}|---)", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {11} Numeric {2}.
    /// </summary>
    private static readonly Regex Alphanumeric11Numeric02 = new(@"[0-9A-Z]{11}\d{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric and dash {1, 6} Numeric and dot {5} Alphanumeric {2}.
    /// </summary>
    private static readonly Regex Numeric02AlphanumericDash0106NumericDot05Alphanumeric02RegEx = new(@"\d{2}[A-Z0-9-]{1,6}[0-9.]{5}[0-9A-Z]{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alphanumeric {3, 35} + Alpha {1, 3}.
    /// </summary>
    private static readonly Regex Alphanumeric0335Alpha0103RegEx = new(@"[0-9A-Z]{3,35}\+[A-Z]{1,3}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 27}.
    /// </summary>
    private static readonly Regex Alpha02Alphanumeric0327RegEx = new(@"[A-Z]{2}[0-9A-Z]{3,27}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {3} Numeric {14} Alphanumeric {1, 33}.
    /// </summary>
    private static readonly Regex Alpha03Numeric14Alphanumeric0133RegEx = new(@"[A-Z]{3}\d{14}[0-9A-Za-z*+-./()!]{1,33}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {1} Numeric {4} Alphanumeric {5, 20}.
    /// </summary>
    private static readonly Regex Alpha01Numeric04Alphanumeric0520RegEx = new(@"[A-Z]{1}\d{4}[0-9A-Z]{5,20}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {1, 8} Alphanumeric {2}.
    /// </summary>
    private static readonly Regex Numeric0108Alphanumeric02RegEx = new(@"\d{1,8}[0-9A-Z]{2}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {1, 10} Alphanumeric {3}.
    /// </summary>
    private static readonly Regex Numeric0110Alphanumeric03RegEx = new(@"\d{1,10}[0-9A-Z]{3}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Numeric {2} Alphanumeric {3, 42}.
    /// </summary>
    private static readonly Regex Numeric02Alphanumeric0342RegEx = new(@"\d{2}[0-9A-Z]{3,42}$", RegexOptions.None);

    /// <summary>
    /// Regular Expression: Alpha {2} Alphanumeric {3, 18}.
    /// </summary>
    private static readonly Regex Alpha02Alphanumeric0318RegEx = new(@"[A-Z]{2}[0-9A-Z]{3,18}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with space {1, 60}.
    /// </summary>
    private static readonly Regex AlphanumericSpace0160RegEx = new(@"[0-9A-Z ]{1,60}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 50}.
    /// </summary>
    private static readonly Regex AlphanumericPlus0150RegEx = new(@"[0-9A-Z ]{1,50}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {20, 50}.
    /// </summary>
    private static readonly Regex AlphanumericPlus2050RegEx = new(@"[0-9A-Z+]{20,50}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alphanumeric with plus {1, 60}.
    /// </summary>
    private static readonly Regex AlphanumericPlus0160RegEx = new(@"[0-9A-Z ]{1,60}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {7}.
    /// </summary>
    private static readonly Regex Alpha04Numeric07RegEx = new(@"[A-Z]{4}\d{7}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {4} Numeric {1, 3}.
    /// </summary>
    private static readonly Regex Alpha04Numeric0103RegEx = new(@"[A-Z]{4}\d{1,3}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {3} Numeric {3}.
    /// </summary>
    private static readonly Regex Alpha03Numeric03RegEx = new(@"[A-Z]{3}\d{3}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {2}.
    /// </summary>
    private static readonly Regex Alpha02RegEx = new(@"[A-Z]{2}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Invariant {2, 12}.
    /// </summary>
    private static readonly Regex Invariant0212RegEx = new(@"[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{2,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Alpha {2} Invariant {3, 27}.
    /// </summary>
    private static readonly Regex Alpha02Invariant0327RegEx = new(@"[A-Z]{2}[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]{3,27}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {1}.
    /// </summary>
    private static readonly Regex Numeric01RegEx = new(@"\d{1}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {5}.
    /// </summary>
    private static readonly Regex Numeric05RegEx = new(@"\d{5}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {9}.
    /// </summary>
    private static readonly Regex Numeric09RegEx = new(@"\d{9}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {14}.
    /// </summary>
    private static readonly Regex Numeric14RegEx = new(@"\d{14}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {18}.
    /// </summary>
    private static readonly Regex Numeric18RegEx = new(@"\d{18}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {1, 2}.
    /// </summary>
    private static readonly Regex Numeric0102RegEx = new(@"\d{1,2}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {4, 6}.
    /// </summary>
    private static readonly Regex Numeric0406RegEx = new(@"\d{4,6}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {6, 26}.
    /// </summary>
    private static readonly Regex Numeric0626RegEx = new(@"\d{6,26}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {7, 12}.
    /// </summary>
    private static readonly Regex Numeric0712RegEx = new(@"\d{7,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {9, 13}.
    /// </summary>
    private static readonly Regex Numeric0913RegEx = new(@"\d{9,13}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {10, 12}.
    /// </summary>
    private static readonly Regex Numeric1012RegEx = new(@"\d{10,12}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {13, 14}.
    /// </summary>
    private static readonly Regex Numeric1314RegEx = new(@"\d{13,14}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric {14, 26}.
    /// </summary>
    private static readonly Regex Numeric1426RegEx = new(@"\d{14,26}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 10}.
    /// </summary>
    private static readonly Regex NumericDot0110RegEx = new(@"[0-9.]{1,10}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 20}.
    /// </summary>
    private static readonly Regex NumericDot0120RegEx = new(@"[0-9.]{1,20}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 05}.
    /// </summary>
    private static readonly Regex NumericDot0105RegEx = new(@"[0-9.]{1,5}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Numeric with dot {01, 06}.
    /// </summary>
    private static readonly Regex NumericDot0106RegEx = new(@"[0-9.]{1,6}$", RegexOptions.None);

    /// <summary>
    ///  Regular Expression: Minus with Numeric {01, 04}.
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
                    AnsiMh10_8_2DataIdentifier.di0,
#if NET7_0_OR_GREATER
                    HibccPlusRegEx
#else
                    () => HibccPlusRegEx
#endif
#pragma warning disable SA1111 // Closing parenthesis should be on line of last parameter
                    )
            },
            {
                2, new EntityDescriptor(
                    "AMPERSAND",
                    AnsiMh10_8_2DataIdentifier.di2,
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
                    AnsiMh10_8_2DataIdentifier.di3,
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
                    AnsiMh10_8_2DataIdentifier.di4,
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
                    AnsiMh10_8_2DataIdentifier.di5,
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
                    AnsiMh10_8_2DataIdentifier.di6,
#if NET7_0_OR_GREATER
                    IfaAbdataPznHyphenRegEx
#else
                    () => IfaAbdataPznHyphenRegEx
#endif
                )
            },
            {
                7, new EntityDescriptor(
                    "EXCLAMATIONMARK",
                    AnsiMh10_8_2DataIdentifier.di7,
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
                    AnsiMh10_8_2DataIdentifier.di2000,
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
                    AnsiMh10_8_2DataIdentifier.di2001,
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
                    AnsiMh10_8_2DataIdentifier.di2002,
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
                    AnsiMh10_8_2DataIdentifier.di2003,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx

#endif
                )
            },
            {
                2004, new EntityDescriptor(
                    "SCAC",
                    AnsiMh10_8_2DataIdentifier.di2004,
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
                    AnsiMh10_8_2DataIdentifier.di2005,
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
                    AnsiMh10_8_2DataIdentifier.di2007,
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
                    AnsiMh10_8_2DataIdentifier.di2008,
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
                    AnsiMh10_8_2DataIdentifier.di2009,
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
                    AnsiMh10_8_2DataIdentifier.di2010,
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
                    AnsiMh10_8_2DataIdentifier.di2011,
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
                    AnsiMh10_8_2DataIdentifier.di2012,
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
                    AnsiMh10_8_2DataIdentifier.di2013,
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
                    AnsiMh10_8_2DataIdentifier.di2014,
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
                    AnsiMh10_8_2DataIdentifier.di2015,
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
                    AnsiMh10_8_2DataIdentifier.di2016,
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
                    AnsiMh10_8_2DataIdentifier.di2017,
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
                    AnsiMh10_8_2DataIdentifier.di2018,
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
                    AnsiMh10_8_2DataIdentifier.di2019,
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
                    AnsiMh10_8_2DataIdentifier.di2020,
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
                    AnsiMh10_8_2DataIdentifier.di2021,
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
                    AnsiMh10_8_2DataIdentifier.di2022,
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
                    AnsiMh10_8_2DataIdentifier.di2023,
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
                    AnsiMh10_8_2DataIdentifier.di2024,
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
                    AnsiMh10_8_2DataIdentifier.di2025,
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
                    AnsiMh10_8_2DataIdentifier.di2026,
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
                    AnsiMh10_8_2DataIdentifier.di2027,
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
                    AnsiMh10_8_2DataIdentifier.di2028,
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
                    AnsiMh10_8_2DataIdentifier.di2029,
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
                    AnsiMh10_8_2DataIdentifier.di2030,
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
                    AnsiMh10_8_2DataIdentifier.di2031,
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
                    AnsiMh10_8_2DataIdentifier.di2055,
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
                    AnsiMh10_8_2DataIdentifier.di3000,
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
                    AnsiMh10_8_2DataIdentifier.di3001,
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
                    AnsiMh10_8_2DataIdentifier.di3002,
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
                    AnsiMh10_8_2DataIdentifier.di3003,
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
                    AnsiMh10_8_2DataIdentifier.di3004,
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
                    AnsiMh10_8_2DataIdentifier.di3005,
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
                    AnsiMh10_8_2DataIdentifier.di4000,
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
                    AnsiMh10_8_2DataIdentifier.di4001,
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
                    AnsiMh10_8_2DataIdentifier.di4002,
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
                    AnsiMh10_8_2DataIdentifier.di4003,
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
                    AnsiMh10_8_2DataIdentifier.di4004,
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
                    AnsiMh10_8_2DataIdentifier.di4005,
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
                    AnsiMh10_8_2DataIdentifier.di4006,
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
                    AnsiMh10_8_2DataIdentifier.di4007,
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
                    AnsiMh10_8_2DataIdentifier.di4008,
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
                    AnsiMh10_8_2DataIdentifier.di4009,
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
                    AnsiMh10_8_2DataIdentifier.di4010,
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
                    AnsiMh10_8_2DataIdentifier.di4011,
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
                    AnsiMh10_8_2DataIdentifier.di4012,
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
                    AnsiMh10_8_2DataIdentifier.di4013,
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
                    AnsiMh10_8_2DataIdentifier.di4014,
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
                    AnsiMh10_8_2DataIdentifier.di4015,
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
                    AnsiMh10_8_2DataIdentifier.di4016,
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
                    AnsiMh10_8_2DataIdentifier.di4017,
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
                    AnsiMh10_8_2DataIdentifier.di4018,
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
                    AnsiMh10_8_2DataIdentifier.di4019,
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
                    AnsiMh10_8_2DataIdentifier.di4020,
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
                    AnsiMh10_8_2DataIdentifier.di4021,
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
                    AnsiMh10_8_2DataIdentifier.di4022,
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
                    AnsiMh10_8_2DataIdentifier.di4023,
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
                    AnsiMh10_8_2DataIdentifier.di4024,
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
                    AnsiMh10_8_2DataIdentifier.di4025,
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
                    AnsiMh10_8_2DataIdentifier.di4026,
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
                    AnsiMh10_8_2DataIdentifier.di4027,
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
                    AnsiMh10_8_2DataIdentifier.di4028,
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
                    AnsiMh10_8_2DataIdentifier.di5000,
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
                    AnsiMh10_8_2DataIdentifier.di5001,
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
                    AnsiMh10_8_2DataIdentifier.di5002,
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
                    AnsiMh10_8_2DataIdentifier.di5003,
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
                    AnsiMh10_8_2DataIdentifier.di5004,
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
                    AnsiMh10_8_2DataIdentifier.di5005,
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
                    AnsiMh10_8_2DataIdentifier.di5006,
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
                    AnsiMh10_8_2DataIdentifier.di5010,
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
                    AnsiMh10_8_2DataIdentifier.di5011,
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
                    AnsiMh10_8_2DataIdentifier.di5012,
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
                    AnsiMh10_8_2DataIdentifier.di5013,
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
                    AnsiMh10_8_2DataIdentifier.di6000,
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
                    AnsiMh10_8_2DataIdentifier.di6001,
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
                    AnsiMh10_8_2DataIdentifier.di6003,
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
                    AnsiMh10_8_2DataIdentifier.di6004,
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
                    AnsiMh10_8_2DataIdentifier.di6005,
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
                    AnsiMh10_8_2DataIdentifier.di8000,
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
                    AnsiMh10_8_2DataIdentifier.di8001,
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
                    AnsiMh10_8_2DataIdentifier.di8002,
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
                    AnsiMh10_8_2DataIdentifier.di8003,
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
                    AnsiMh10_8_2DataIdentifier.di8004,
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
                    AnsiMh10_8_2DataIdentifier.di8005,
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
                    AnsiMh10_8_2DataIdentifier.di8006,
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
                    AnsiMh10_8_2DataIdentifier.di8007,
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
                    AnsiMh10_8_2DataIdentifier.di8008,
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
                    AnsiMh10_8_2DataIdentifier.di8009,
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
                    AnsiMh10_8_2DataIdentifier.di8010,
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
                    AnsiMh10_8_2DataIdentifier.di8011,
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
                    AnsiMh10_8_2DataIdentifier.di8012,
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
                    AnsiMh10_8_2DataIdentifier.di8015,
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
                    AnsiMh10_8_2DataIdentifier.di8025,
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
                    AnsiMh10_8_2DataIdentifier.di8026,
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
                    AnsiMh10_8_2DataIdentifier.di9000,
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
                    AnsiMh10_8_2DataIdentifier.di9002,
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
                    AnsiMh10_8_2DataIdentifier.di9004,
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
                    AnsiMh10_8_2DataIdentifier.di9005,
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
                    AnsiMh10_8_2DataIdentifier.di10000,
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
                    AnsiMh10_8_2DataIdentifier.di10001,
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
                    AnsiMh10_8_2DataIdentifier.di10002,
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
                    AnsiMh10_8_2DataIdentifier.di10003,
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
                    AnsiMh10_8_2DataIdentifier.di10004,
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
                    AnsiMh10_8_2DataIdentifier.di10005,
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
                    AnsiMh10_8_2DataIdentifier.di10006,
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
                    AnsiMh10_8_2DataIdentifier.di10007,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                10008, new EntityDescriptor(
                    "MMSI",
                    AnsiMh10_8_2DataIdentifier.di10008,
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
                    AnsiMh10_8_2DataIdentifier.di11000,
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
                    AnsiMh10_8_2DataIdentifier.di11001,
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
                    AnsiMh10_8_2DataIdentifier.di11002,
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
                    AnsiMh10_8_2DataIdentifier.di11003,
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
                    AnsiMh10_8_2DataIdentifier.di11004,
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
                    AnsiMh10_8_2DataIdentifier.di11005,
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
                    AnsiMh10_8_2DataIdentifier.di11006,
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
                    AnsiMh10_8_2DataIdentifier.di11007,
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
                    AnsiMh10_8_2DataIdentifier.di11008,
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
                    AnsiMh10_8_2DataIdentifier.di11009,
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
                    AnsiMh10_8_2DataIdentifier.di11010,
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
                    AnsiMh10_8_2DataIdentifier.di11011,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11012, new EntityDescriptor(
                    "SCAC",
                    AnsiMh10_8_2DataIdentifier.di11012,
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
                    AnsiMh10_8_2DataIdentifier.di11013,
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
                    AnsiMh10_8_2DataIdentifier.di11014,
#if NET7_0_OR_GREATER
                    NumericPlus01UnboundRegEx
#else
                    () => NumericPlus01UnboundRegEx
#endif
                )
            },
            {
                11015, new EntityDescriptor(
                    "KANBAN",
                    AnsiMh10_8_2DataIdentifier.di11015,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11016, new EntityDescriptor(
                    "DELINS",
                    AnsiMh10_8_2DataIdentifier.di11016,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                11017, new EntityDescriptor(
                    "CHECK",
                    AnsiMh10_8_2DataIdentifier.di11017,
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
                    AnsiMh10_8_2DataIdentifier.di11018,
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
                    AnsiMh10_8_2DataIdentifier.di11019,
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
                    AnsiMh10_8_2DataIdentifier.di11020,
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
                    AnsiMh10_8_2DataIdentifier.di11021,
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
                    AnsiMh10_8_2DataIdentifier.di11022,
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
                    AnsiMh10_8_2DataIdentifier.di11025,
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
                    AnsiMh10_8_2DataIdentifier.di11026,
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
                    AnsiMh10_8_2DataIdentifier.di11027,
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
                    AnsiMh10_8_2DataIdentifier.di12000,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12001, new EntityDescriptor(
                    "LOCATION",
                    AnsiMh10_8_2DataIdentifier.di12001,
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
                    AnsiMh10_8_2DataIdentifier.di12002,
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
                    AnsiMh10_8_2DataIdentifier.di12003,
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
                    AnsiMh10_8_2DataIdentifier.di12004,
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
                    AnsiMh10_8_2DataIdentifier.di12005,
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
                    AnsiMh10_8_2DataIdentifier.di12006,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                12007, new EntityDescriptor(
                    "DODAAC",
                    AnsiMh10_8_2DataIdentifier.di12007,
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
                    AnsiMh10_8_2DataIdentifier.di12008,
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
                    AnsiMh10_8_2DataIdentifier.di12009,
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
                    AnsiMh10_8_2DataIdentifier.di12011,
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
                    AnsiMh10_8_2DataIdentifier.di12012,
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
                    AnsiMh10_8_2DataIdentifier.di12013,
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
                    AnsiMh10_8_2DataIdentifier.di12015,
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
                    AnsiMh10_8_2DataIdentifier.di12016,
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
                    AnsiMh10_8_2DataIdentifier.di12017,
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
                    AnsiMh10_8_2DataIdentifier.di12018,
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
                    AnsiMh10_8_2DataIdentifier.di12020,
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
                    AnsiMh10_8_2DataIdentifier.di12021,
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
                    AnsiMh10_8_2DataIdentifier.di12022,
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
                    AnsiMh10_8_2DataIdentifier.di12023,
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
                    AnsiMh10_8_2DataIdentifier.di12024,
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
                    AnsiMh10_8_2DataIdentifier.di12025,
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
                    AnsiMh10_8_2DataIdentifier.di12026,
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
                    AnsiMh10_8_2DataIdentifier.di12027,
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
                    AnsiMh10_8_2DataIdentifier.di12028,
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
                    AnsiMh10_8_2DataIdentifier.di12029,
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
                    AnsiMh10_8_2DataIdentifier.di12030,
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
                    AnsiMh10_8_2DataIdentifier.di12031,
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
                    AnsiMh10_8_2DataIdentifier.di12032,
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
                    AnsiMh10_8_2DataIdentifier.di12033,
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
                    AnsiMh10_8_2DataIdentifier.di12034,
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
                    AnsiMh10_8_2DataIdentifier.di12035,
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
                    AnsiMh10_8_2DataIdentifier.di12036,
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
                    AnsiMh10_8_2DataIdentifier.di12051,
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
                    AnsiMh10_8_2DataIdentifier.di12052,
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
                    AnsiMh10_8_2DataIdentifier.di12054,
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
                    AnsiMh10_8_2DataIdentifier.di12055,
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
                    AnsiMh10_8_2DataIdentifier.di13010,
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
                    AnsiMh10_8_2DataIdentifier.di13011,
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
                    AnsiMh10_8_2DataIdentifier.di13012,
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
                    AnsiMh10_8_2DataIdentifier.di13013,
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
                    AnsiMh10_8_2DataIdentifier.di13014,
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
                    AnsiMh10_8_2DataIdentifier.di14000,
#if NET7_0_OR_GREATER
                    AlphanumericRegEx1315
#else
                    () => AlphanumericRegEx1315
#endif
                    )
            },
            {
                14001, new EntityDescriptor(
                    "CIDX PRODUCT CHARACTERISTIC",
                    AnsiMh10_8_2DataIdentifier.di14001,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14003, new EntityDescriptor(
                    "EIAJ ENCODED",
                    AnsiMh10_8_2DataIdentifier.di14003,
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
                    AnsiMh10_8_2DataIdentifier.di14004,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14005, new EntityDescriptor(
                    "AIAG ENCODED",
                    AnsiMh10_8_2DataIdentifier.di14005,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                14006, new EntityDescriptor(
                    "MILSTRIP ENCODED",
                    AnsiMh10_8_2DataIdentifier.di14006,
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
                    AnsiMh10_8_2DataIdentifier.di14007,
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
                    AnsiMh10_8_2DataIdentifier.di14008,
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
                    AnsiMh10_8_2DataIdentifier.di14009,
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
                    AnsiMh10_8_2DataIdentifier.di14010,
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
                    AnsiMh10_8_2DataIdentifier.di14011,
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
                    AnsiMh10_8_2DataIdentifier.di14012,
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
                    AnsiMh10_8_2DataIdentifier.di14015,
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
                    AnsiMh10_8_2DataIdentifier.di16000,
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
                    AnsiMh10_8_2DataIdentifier.di16001,
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
                    AnsiMh10_8_2DataIdentifier.di16002,
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
                    AnsiMh10_8_2DataIdentifier.di16003,
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
                    AnsiMh10_8_2DataIdentifier.di16004,
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
                    AnsiMh10_8_2DataIdentifier.di16005,
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
                    AnsiMh10_8_2DataIdentifier.di16006,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16007, new EntityDescriptor(
                    "CLEI",
                    AnsiMh10_8_2DataIdentifier.di16007,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16008, new EntityDescriptor(
                    "GS1 GTIN-14",
                    AnsiMh10_8_2DataIdentifier.di16008,
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
                    AnsiMh10_8_2DataIdentifier.di16009,
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
                    AnsiMh10_8_2DataIdentifier.di16010,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16011, new EntityDescriptor(
                    "CLEI",
                    AnsiMh10_8_2DataIdentifier.di16011,
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
                    AnsiMh10_8_2DataIdentifier.di16012,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16013, new EntityDescriptor(
                    "VMRS SYSTEM",
                    AnsiMh10_8_2DataIdentifier.di16013,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16014, new EntityDescriptor(
                    "VMRS SYSTEM & ASSEMBLY",
                    AnsiMh10_8_2DataIdentifier.di16014,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16015, new EntityDescriptor(
                    "VMRS SYSTEM, ASSEMBLY & PART",
                    AnsiMh10_8_2DataIdentifier.di16015,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16016, new EntityDescriptor(
                    "VMRS SYSTEM, ASSEMBLY & PART",
                    AnsiMh10_8_2DataIdentifier.di16016,
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
                    AnsiMh10_8_2DataIdentifier.di16017,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16018, new EntityDescriptor(
                    "VMRS SUPPLIER ID & PART NO.",
                    AnsiMh10_8_2DataIdentifier.di16018,
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
                    AnsiMh10_8_2DataIdentifier.di16019,
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
                    AnsiMh10_8_2DataIdentifier.di16020,
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
                    AnsiMh10_8_2DataIdentifier.di16021,
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
                    AnsiMh10_8_2DataIdentifier.di16022,
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
                    AnsiMh10_8_2DataIdentifier.di16023,
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
                    AnsiMh10_8_2DataIdentifier.di16024,
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
                    AnsiMh10_8_2DataIdentifier.di16025,
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
                    AnsiMh10_8_2DataIdentifier.di16026,
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
                    AnsiMh10_8_2DataIdentifier.di16027,
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
                    AnsiMh10_8_2DataIdentifier.di16028,
#if NET7_0_OR_GREATER
                    AlphanumericRegEx01100
#else
                    () => AlphanumericRegEx01100
#endif
                    )
            },
            {
                16029, new EntityDescriptor(
                    "GMDN PRODUCT CLASSIFICATION",
                    AnsiMh10_8_2DataIdentifier.di16029,
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
                    AnsiMh10_8_2DataIdentifier.di16030,
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
                    AnsiMh10_8_2DataIdentifier.di16031,
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
                    AnsiMh10_8_2DataIdentifier.di16032,
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
                    AnsiMh10_8_2DataIdentifier.di16033,
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
                    AnsiMh10_8_2DataIdentifier.di16034,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                16040, new EntityDescriptor(
                    "MSDS CODE",
                    AnsiMh10_8_2DataIdentifier.di16040,
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
                    AnsiMh10_8_2DataIdentifier.di16049,
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
                    AnsiMh10_8_2DataIdentifier.di16050,
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
                    AnsiMh10_8_2DataIdentifier.di16051,
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
                    AnsiMh10_8_2DataIdentifier.di16052,
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
                    AnsiMh10_8_2DataIdentifier.di16053,
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
                    AnsiMh10_8_2DataIdentifier.di16054,
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
                    AnsiMh10_8_2DataIdentifier.di16055,
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
                    AnsiMh10_8_2DataIdentifier.di17000,
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
                    AnsiMh10_8_2DataIdentifier.di17001,
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
                    AnsiMh10_8_2DataIdentifier.di17002,
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
                    AnsiMh10_8_2DataIdentifier.di17003,
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
                    AnsiMh10_8_2DataIdentifier.di17004,
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
                    AnsiMh10_8_2DataIdentifier.di17005,
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
                    AnsiMh10_8_2DataIdentifier.di17006,
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
                    AnsiMh10_8_2DataIdentifier.di17007,
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
                    AnsiMh10_8_2DataIdentifier.di17008,
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
                    AnsiMh10_8_2DataIdentifier.di17009,
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
                    AnsiMh10_8_2DataIdentifier.di17011,
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
                    AnsiMh10_8_2DataIdentifier.di17012,
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
                    AnsiMh10_8_2DataIdentifier.di17013,
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
                    AnsiMh10_8_2DataIdentifier.di17014,
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
                    AnsiMh10_8_2DataIdentifier.di17015,
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
                    AnsiMh10_8_2DataIdentifier.di17016,
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
                    AnsiMh10_8_2DataIdentifier.di17017,
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
                    AnsiMh10_8_2DataIdentifier.di17018,
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
                    AnsiMh10_8_2DataIdentifier.di17019,
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
                    AnsiMh10_8_2DataIdentifier.di17020,
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
                    AnsiMh10_8_2DataIdentifier.di17021,
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
                    AnsiMh10_8_2DataIdentifier.di17022,
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
                    AnsiMh10_8_2DataIdentifier.di17023,
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
                    AnsiMh10_8_2DataIdentifier.di17024,
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
                    AnsiMh10_8_2DataIdentifier.di17025,
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
                    AnsiMh10_8_2DataIdentifier.di17026,
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
                    AnsiMh10_8_2DataIdentifier.di17027,
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
                    AnsiMh10_8_2DataIdentifier.di17028,
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
                    AnsiMh10_8_2DataIdentifier.di17029,
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
                    AnsiMh10_8_2DataIdentifier.di17030,
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
                    AnsiMh10_8_2DataIdentifier.di17031,
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
                    AnsiMh10_8_2DataIdentifier.di17032,
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
                    AnsiMh10_8_2DataIdentifier.di18001,
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
                    AnsiMh10_8_2DataIdentifier.di18002,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18004, new EntityDescriptor(
                    "DODIC",
                    AnsiMh10_8_2DataIdentifier.di18004,
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
                    AnsiMh10_8_2DataIdentifier.di18005,
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
                    AnsiMh10_8_2DataIdentifier.di18006,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                18007, new EntityDescriptor(
                    "ASFIS CODE",
                    AnsiMh10_8_2DataIdentifier.di18007,
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
                    AnsiMh10_8_2DataIdentifier.di18008,
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
                    AnsiMh10_8_2DataIdentifier.di18009,
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
                    AnsiMh10_8_2DataIdentifier.di19000,
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
                    AnsiMh10_8_2DataIdentifier.di19001,
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
                    AnsiMh10_8_2DataIdentifier.di19002,
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
                    AnsiMh10_8_2DataIdentifier.di19003,
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
                    AnsiMh10_8_2DataIdentifier.di19004,
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
                    AnsiMh10_8_2DataIdentifier.di19005,
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
                    AnsiMh10_8_2DataIdentifier.di19006,
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
                    AnsiMh10_8_2DataIdentifier.di19007,
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
                    AnsiMh10_8_2DataIdentifier.di19008,
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
                    AnsiMh10_8_2DataIdentifier.di19009,
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
                    AnsiMh10_8_2DataIdentifier.di19010,
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
                    AnsiMh10_8_2DataIdentifier.di19011,
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
                    AnsiMh10_8_2DataIdentifier.di19012,
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
                    AnsiMh10_8_2DataIdentifier.di19013,
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
                    AnsiMh10_8_2DataIdentifier.di19014,
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
                    AnsiMh10_8_2DataIdentifier.di19015,
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
                    AnsiMh10_8_2DataIdentifier.di19016,
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
                    AnsiMh10_8_2DataIdentifier.di19017,
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
                    AnsiMh10_8_2DataIdentifier.di19018,
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
                    AnsiMh10_8_2DataIdentifier.di19019,
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
                    AnsiMh10_8_2DataIdentifier.di19020,
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
                    AnsiMh10_8_2DataIdentifier.di19021,
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
                    AnsiMh10_8_2DataIdentifier.di19022,
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
                    AnsiMh10_8_2DataIdentifier.di19023,
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
                    AnsiMh10_8_2DataIdentifier.di19024,
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
                    AnsiMh10_8_2DataIdentifier.di19025,
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
                    AnsiMh10_8_2DataIdentifier.di19026,
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
                    AnsiMh10_8_2DataIdentifier.di19027,
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
                    AnsiMh10_8_2DataIdentifier.di19028,
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
                    AnsiMh10_8_2DataIdentifier.di19030,
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
                    AnsiMh10_8_2DataIdentifier.di19031,
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
                    AnsiMh10_8_2DataIdentifier.di19032,
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
                    AnsiMh10_8_2DataIdentifier.di19033,
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
                    AnsiMh10_8_2DataIdentifier.di19034,
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
                    AnsiMh10_8_2DataIdentifier.di19035,
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
                    AnsiMh10_8_2DataIdentifier.di19036,
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
                    AnsiMh10_8_2DataIdentifier.di19037,
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
                    AnsiMh10_8_2DataIdentifier.di19042,
#if NET7_0_OR_GREATER
                    Numeric02Alphanumeric0342RegEx
#else
                    () => Numeric02Alphanumeric0342RegEx
#endif
                )
            },
            {
                19043, new EntityDescriptor(
                    "ICCID",
                    AnsiMh10_8_2DataIdentifier.di19043,
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
                    AnsiMh10_8_2DataIdentifier.di19050,
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
                    AnsiMh10_8_2DataIdentifier.di19051,
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
                    AnsiMh10_8_2DataIdentifier.di19052,
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
                    AnsiMh10_8_2DataIdentifier.di19053,
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
                    AnsiMh10_8_2DataIdentifier.di19054,
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
                    AnsiMh10_8_2DataIdentifier.di19096,
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
                    AnsiMh10_8_2DataIdentifier.di19097,
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
                    AnsiMh10_8_2DataIdentifier.di20000,
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
                    AnsiMh10_8_2DataIdentifier.di20001,
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
                    AnsiMh10_8_2DataIdentifier.di20003,
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
                    AnsiMh10_8_2DataIdentifier.di20020,
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
                    AnsiMh10_8_2DataIdentifier.di20021,
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
                    AnsiMh10_8_2DataIdentifier.di20022,
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
                    AnsiMh10_8_2DataIdentifier.di20023,
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
                    AnsiMh10_8_2DataIdentifier.di20024,
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
                    AnsiMh10_8_2DataIdentifier.di20025,
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
                    AnsiMh10_8_2DataIdentifier.di20026,
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
                    AnsiMh10_8_2DataIdentifier.di20027,
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
                    AnsiMh10_8_2DataIdentifier.di20030,
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
                    AnsiMh10_8_2DataIdentifier.di20031,
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
                    AnsiMh10_8_2DataIdentifier.di20032,
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
                    AnsiMh10_8_2DataIdentifier.di20033,
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
                    AnsiMh10_8_2DataIdentifier.di20034,
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
                    AnsiMh10_8_2DataIdentifier.di21005,
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
                    AnsiMh10_8_2DataIdentifier.di21006,
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
                    AnsiMh10_8_2DataIdentifier.di21015,
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
                    AnsiMh10_8_2DataIdentifier.di21016,
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
                    AnsiMh10_8_2DataIdentifier.di21017,
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
                    AnsiMh10_8_2DataIdentifier.di21018,
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
                    AnsiMh10_8_2DataIdentifier.di21019,
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
                    AnsiMh10_8_2DataIdentifier.di21055,
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
                    AnsiMh10_8_2DataIdentifier.di22000,
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
                    AnsiMh10_8_2DataIdentifier.di22001,
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
                    AnsiMh10_8_2DataIdentifier.di22002,
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
                    AnsiMh10_8_2DataIdentifier.di22003,
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
                    AnsiMh10_8_2DataIdentifier.di22004,
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
                    AnsiMh10_8_2DataIdentifier.di22005,
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
                    AnsiMh10_8_2DataIdentifier.di22006,
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
                    AnsiMh10_8_2DataIdentifier.di22007,
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
                    AnsiMh10_8_2DataIdentifier.di22008,
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
                    AnsiMh10_8_2DataIdentifier.di22009,
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
                    AnsiMh10_8_2DataIdentifier.di22010,
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
                    AnsiMh10_8_2DataIdentifier.di22011,
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
                    AnsiMh10_8_2DataIdentifier.di22012,
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
                    AnsiMh10_8_2DataIdentifier.di22013,
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
                    AnsiMh10_8_2DataIdentifier.di22014,
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
                    AnsiMh10_8_2DataIdentifier.di22015,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22016, new EntityDescriptor(
                    "VMRS SUPPLIER ID",
                    AnsiMh10_8_2DataIdentifier.di22016,
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
                    AnsiMh10_8_2DataIdentifier.di22017,
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
                    AnsiMh10_8_2DataIdentifier.di22018,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
            },
            {
                22019, new EntityDescriptor(
                    "PARTYS ROLE(S) IN A TRANSACTION",
                    AnsiMh10_8_2DataIdentifier.di22019,
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
                    AnsiMh10_8_2DataIdentifier.di22020,
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
                    AnsiMh10_8_2DataIdentifier.di22021,
#if NET7_0_OR_GREATER
                    AlphanumericRegEx0135
#else
                    () => AlphanumericRegEx0135
#endif
                    )
            },
            {
                22022, new EntityDescriptor(
                    "CARRIER SCAC",
                    AnsiMh10_8_2DataIdentifier.di22022,
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
                    AnsiMh10_8_2DataIdentifier.di22023,
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
                    AnsiMh10_8_2DataIdentifier.di22024,
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
                    AnsiMh10_8_2DataIdentifier.di22025,
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
                    AnsiMh10_8_2DataIdentifier.di23000,
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
                    AnsiMh10_8_2DataIdentifier.di23001,
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
                    AnsiMh10_8_2DataIdentifier.di23002,
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
                    AnsiMh10_8_2DataIdentifier.di23003,
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
                    AnsiMh10_8_2DataIdentifier.di23004,
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
                    AnsiMh10_8_2DataIdentifier.di23005,
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
                    AnsiMh10_8_2DataIdentifier.di23006,
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
                    AnsiMh10_8_2DataIdentifier.di23010,
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
                    AnsiMh10_8_2DataIdentifier.di23011,
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
                    AnsiMh10_8_2DataIdentifier.di23012,
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
                    "CUSTOMER-SUPPLIER",
                    AnsiMh10_8_2DataIdentifier.di26000,
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
                    "CARRIER-SUPPLIER",
                    AnsiMh10_8_2DataIdentifier.di26001,
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
                    "CUSTOMER-CARRIER",
                    AnsiMh10_8_2DataIdentifier.di26002,
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
                    AnsiMh10_8_2DataIdentifier.di26003,
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
                    "CARRIER-TRADING PARTNER",
                    AnsiMh10_8_2DataIdentifier.di26004,
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
                    AnsiMh10_8_2DataIdentifier.di26010,
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
                    AnsiMh10_8_2DataIdentifier.di26011,
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
                    AnsiMh10_8_2DataIdentifier.di26012,
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
                    AnsiMh10_8_2DataIdentifier.di26013,
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
                    AnsiMh10_8_2DataIdentifier.di26014,
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
                    AnsiMh10_8_2DataIdentifier.di26015,
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
                    AnsiMh10_8_2DataIdentifier.di26016,
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
                    AnsiMh10_8_2DataIdentifier.di26017,
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
                    AnsiMh10_8_2DataIdentifier.di26018,
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
                    AnsiMh10_8_2DataIdentifier.di26019,
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
                    AnsiMh10_8_2DataIdentifier.di26020,
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
                    AnsiMh10_8_2DataIdentifier.di26021,
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
                    AnsiMh10_8_2DataIdentifier.di26022,
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
                    AnsiMh10_8_2DataIdentifier.di26023,
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
                    AnsiMh10_8_2DataIdentifier.di26024,
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
                    AnsiMh10_8_2DataIdentifier.di26025,
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
                    AnsiMh10_8_2DataIdentifier.di26026,
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
                    AnsiMh10_8_2DataIdentifier.di26027,
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
                    AnsiMh10_8_2DataIdentifier.di26028,
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
                    AnsiMh10_8_2DataIdentifier.di26029,
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
                    AnsiMh10_8_2DataIdentifier.di26030,
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
                    AnsiMh10_8_2DataIdentifier.di26031,
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
                    AnsiMh10_8_2DataIdentifier.di26032,
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
                    AnsiMh10_8_2DataIdentifier.di26033,
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
                    AnsiMh10_8_2DataIdentifier.di26034,
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
                    AnsiMh10_8_2DataIdentifier.di26035,
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
                    AnsiMh10_8_2DataIdentifier.di26036,
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
                    AnsiMh10_8_2DataIdentifier.di26037,
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
                    AnsiMh10_8_2DataIdentifier.di26038,
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
                    AnsiMh10_8_2DataIdentifier.di26039,
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
                    AnsiMh10_8_2DataIdentifier.di26040,
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
                    AnsiMh10_8_2DataIdentifier.di26041,
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
                    AnsiMh10_8_2DataIdentifier.di26042,
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
                    AnsiMh10_8_2DataIdentifier.di26043,
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
                    AnsiMh10_8_2DataIdentifier.di26044,
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
                    AnsiMh10_8_2DataIdentifier.di26045,
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
                    AnsiMh10_8_2DataIdentifier.di26046,
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
                    AnsiMh10_8_2DataIdentifier.di26047,
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
                    AnsiMh10_8_2DataIdentifier.di26048,
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
                    AnsiMh10_8_2DataIdentifier.di26049,
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
                    AnsiMh10_8_2DataIdentifier.di26050,
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
                    AnsiMh10_8_2DataIdentifier.di26051,
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
                    AnsiMh10_8_2DataIdentifier.di26052,
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
                    AnsiMh10_8_2DataIdentifier.di26053,
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
                    AnsiMh10_8_2DataIdentifier.di26054,
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
                    AnsiMh10_8_2DataIdentifier.di26055,
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
                    AnsiMh10_8_2DataIdentifier.di26056,
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
                    AnsiMh10_8_2DataIdentifier.di26057,
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
                    AnsiMh10_8_2DataIdentifier.di26058,
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
                    AnsiMh10_8_2DataIdentifier.di26059,
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
                    AnsiMh10_8_2DataIdentifier.di26060,
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
                    AnsiMh10_8_2DataIdentifier.di26061,
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
                    AnsiMh10_8_2DataIdentifier.di26062,
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
                    AnsiMh10_8_2DataIdentifier.di26063,
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
                    AnsiMh10_8_2DataIdentifier.di26064,
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
                    AnsiMh10_8_2DataIdentifier.di26065,
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
                    AnsiMh10_8_2DataIdentifier.di26066,
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
                    AnsiMh10_8_2DataIdentifier.di26067,
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
                    AnsiMh10_8_2DataIdentifier.di26068,
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
                    AnsiMh10_8_2DataIdentifier.di26069,
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
                    AnsiMh10_8_2DataIdentifier.di26070,
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
                    AnsiMh10_8_2DataIdentifier.di26071,
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
                    AnsiMh10_8_2DataIdentifier.di26072,
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
                    AnsiMh10_8_2DataIdentifier.di26073,
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
                    AnsiMh10_8_2DataIdentifier.di26074,
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
                    AnsiMh10_8_2DataIdentifier.di26075,
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
                    AnsiMh10_8_2DataIdentifier.di26076,
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
                    AnsiMh10_8_2DataIdentifier.di26077,
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
                    AnsiMh10_8_2DataIdentifier.di26078,
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
                    AnsiMh10_8_2DataIdentifier.di26079,
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
                    AnsiMh10_8_2DataIdentifier.di26080,
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
                    AnsiMh10_8_2DataIdentifier.di26081,
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
                    AnsiMh10_8_2DataIdentifier.di26082,
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
                    AnsiMh10_8_2DataIdentifier.di26083,
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
                    AnsiMh10_8_2DataIdentifier.di26084,
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
                    AnsiMh10_8_2DataIdentifier.di26085,
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
                    AnsiMh10_8_2DataIdentifier.di26086,
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
                    AnsiMh10_8_2DataIdentifier.di26087,
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
                    AnsiMh10_8_2DataIdentifier.di26088,
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
                    AnsiMh10_8_2DataIdentifier.di26089,
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
                    AnsiMh10_8_2DataIdentifier.di26090,
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
                    AnsiMh10_8_2DataIdentifier.di26091,
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
                    AnsiMh10_8_2DataIdentifier.di26092,
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
                    AnsiMh10_8_2DataIdentifier.di26093,
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
                    AnsiMh10_8_2DataIdentifier.di26094,
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
                    AnsiMh10_8_2DataIdentifier.di26095,
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
                    AnsiMh10_8_2DataIdentifier.di26096,
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
                    AnsiMh10_8_2DataIdentifier.di26097,
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
                    AnsiMh10_8_2DataIdentifier.di26098,
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
                    AnsiMh10_8_2DataIdentifier.di26099,
#if NET7_0_OR_GREATER
                    Alphanumeric01UnboundRegEx
#else
                    () => Alphanumeric01UnboundRegEx
#endif
                )
#pragma warning restore SA1111 // Closing parenthesis should be on line of last parameter
            },
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
        bool includeDescriptors = true) {
        // Get the last character of the data identifier
        var descriptorKey = -1;
        const int numberOfDecimalPlaces = -1;

        var fnc1 = ((char)29).ToInvariantString();
        var isoIec15434Preamble = "[)>" + (char)30;

        // Resolve the descriptor key
        switch (dataIdentifier) {
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
                if (dataIdentifier == fnc1) {
                    descriptorKey = 4;
                    break;
                }

                if (dataIdentifier == isoIec15434Preamble) {
                    descriptorKey = 5;
                    break;
                }

                if (dataIdentifier?.Length == 0) {
                    return new ResolvedDataIdentifier(
                        new ParserException(3002, Resources.Ansi_Mh10_8_2_Error_010, false),
                        currentPosition);
                }

#if NET6_0_OR_GREATER
                var lastChar = dataIdentifier?[^1..]?.ToInvariantUpper();
#else
                var lastChar = dataIdentifier?.Substring(dataIdentifier.Length - 2).ToInvariantUpper();
#endif

                var ascii = lastChar?[0] - 64;

                if (ascii is < 1 or > 65) {
                    break;
                }

                if (dataIdentifier?.Length > 1) {
#if NET6_0_OR_GREATER
                    var entityValuePart = dataIdentifier[..^1];
#else
                    var entityValuePart = dataIdentifier.Substring(0, dataIdentifier.Length - 1);
#endif

                    if (entityValuePart.Length > 3) {
                        break;
                    }

                    if (!int.TryParse(entityValuePart, out var entityValue)) {
                        break;
                    }

                    descriptorKey = (ascii ?? 0) * 1000 + entityValue;
                }
                else {
                    descriptorKey = (ascii ?? 0) * 1000;
                }

                break;
        }

        if (descriptorKey <= -1) {
            return new ResolvedDataIdentifier(
                new ParserException(
                    3002,
                    string.Format(CultureInfo.CurrentCulture, Resources.Ansi_Mh10_8_2_Error_009, dataIdentifier ?? "<unknown>"),
                    false),
                currentPosition,
                data);
        }

        if (!includeDescriptors) {
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

        try {
            descriptors = descriptorKey.GetDescriptors();
        }
        catch (KeyNotFoundException) {
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

#if NET7_0_OR_GREATER
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
    // ReSharper disable once IdentifierTypo
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    [GeneratedRegex(@"^(((0[1-9]|[12]\d|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1\d|2[0-8])[-/]?02)[-/]?\d{4}|29[-/]?02[-/]?(\d{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0\d|1[0-6])00))$", RegexOptions.None, "en-US")]
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
    [GeneratedRegex(@"^(?:(?:http|https|ftp|telnet|gopher|ms\-help|file|notes)://)?(?:(?:[a-z][\w~%!&amp;',;=\-\.$\(\)\*\+]*):.*@)?(?:(?:[a-z0-9][\w\-]*[a-z0-9]*\.)*(?:(?:(?:(?:[a-z0-9][\w\-]*[a-z0-9]*)(?:\.[a-z0-9]+)?)|(?:(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))(?::[0-9]+)?))?(?:(?:(?:/(?:[\w`~!$=;\-\+\.\^\(\)\|\{\}\[\]]|(?:%\d\d))+)*/(?:[\w`~!$=;\-\+\.\^\(\)\|\{\}\[\]]|(?:%\d\d))*)(?:\?[^#]+)?(?:#[a-z0-9]\w*)?)?$", RegexOptions.None, "en-US")]
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
#endif

    /// <summary>
    ///   Returns the descriptors tuple for the given data identifier descriptor key.
    /// </summary>
    /// <param name="descriptorKey">The data identifier descriptor key.</param>
    /// <returns>The descriptors tuple for the given The data identifier descriptor key.</returns>
    private static EntityDescriptor GetDescriptors(this int descriptorKey) {
        return descriptorKey < 0 ? new EntityDescriptor(null, null, () => new Regex(string.Empty)) : Descriptors[descriptorKey];
    }

    /// <summary>
    ///   Validates a resolved entity.
    /// </summary>
    /// <param name="resolvedEntity">The resolved entity to be validated.</param>
    /// <returns>A resolved entity object. If the value is invalid, the object records the error.</returns>
    private static ResolvedDataIdentifier Validate(ResolvedDataIdentifier? resolvedEntity) {
        try {
            var identifierString = (resolvedEntity?.Identifier ?? string.Empty).Length > 0
                                       ? " " + resolvedEntity?.Identifier
                                       : string.Empty;

            if (resolvedEntity is null) {
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
            foreach (var parserException in validationErrors) {
                identifier.AddException(parserException);
            }

            return identifier;
        }
        catch (ArgumentNullException) {
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
        catch (RegexMatchTimeoutException) {
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