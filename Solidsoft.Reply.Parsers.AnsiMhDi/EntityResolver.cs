// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityResolver.cs" company="Solidsoft Reply Ltd.">
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
public static class EntityResolver
{
    /// <summary>
    ///   A regular expression for six-digit date representation - DDMMYY.
    /// </summary>
    private const string DatePatternDdMmYy =
        @"(((0[0-9]|[12]\d|3[01])(0[13578]|1[02])(\d{2}))|((0[0-9]|[12]\d|30)(0[13456789]|1[012])(\d{2}))|((0[0-9]|1\d|2[0-8])02(\d{2}))|(2902((0[048]|[2468][048]|[13579][26]))))";

    /// <summary>
    ///   The date pattern dd mm yyyyy.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]

    // ReSharper disable once IdentifierTypo
    private const string DatePatternDdMmYyyyy =
        @"^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$";

    /// <summary>
    ///   A regular expression for six-digit date representation - MMDDYY.
    /// </summary>
    private const string DatePatternMmDdYy =
        @"(((0[13578]|1[02])(0[0-9]|[12]\d|3[01])(\d{2}))|((0[13456789]|1[012])(0[0-9]|[12]\d|30)(\d{2}))|(02(0[0-9]|1\d|2[0-8])(\d{2}))|(0229((0[048]|[2468][048]|[13579][26]))))";

    /// <summary>
    ///   The Julian date patter YDDD.
    /// </summary>
    private const string DatePatternYDddJulian =
        @"^([0-9])(00[1-9]|0[1-9][0-9]|[1-2][0-9][0-9]|3[0-5][0-9]|36[0-6])$";

    /// <summary>
    ///   The Julian date patter YYDDD.
    /// </summary>
    private const string DatePatternYyDddJulian =
        @"^([0-9]{2})(00[1-9]|0[1-9][0-9]|[1-2][0-9][0-9]|3[0-5][0-9]|36[0-6])$";

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    /// </summary>
    private const string DatePatternYyMmDd =
        @"(((\d{2})(0[13578]|1[02])(0[1-9]|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0[1-9]|[12]\d|30))|((\d{2})02(0[1-9]|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))";

    /// <summary>
    ///   A regular expression for six-digit date representation - YYMMDD.
    ///   If it is not necessary to specify the day, the day field can be filled with two zeros.
    /// </summary>
    private const string DatePatternYyMmDdZeros =
        @"(((\d{2})(0[13578]|1[02])(0[0-9]|[12]\d|3[01]))|((\d{2})(0[13456789]|1[012])(0[0-9]|[12]\d|30))|((\d{2})02(0[0-9]|1\d|2[0-8]))|(((0[048]|[2468][048]|[13579][26]))0229))";

    /// <summary>
    ///   The date patter YDWW.
    /// </summary>
    private const string DatePatternYyWw = @"\d{2}((0[1-9])|([1-4][0-9])|(5[0-3]))";

    /// <summary>
    ///   The date pattern yyyy mm dd.
    /// </summary>

    // ReSharper disable once IdentifierTypo
    private const string DatePatternYyyyMmDd =
        @"([0-9]{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12][0-9]|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1[0-9]|2[0-8]))|([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00)[-/]?02[-/]?29)";

    /// <summary>
    ///   The date pattern YYYYMMDDHHMM.
    /// </summary>

    // ReSharper disable once IdentifierTypo
    private const string DatePatternYyyyMmDdHhMm =
        @"(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:[0-9]{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:0[1-9]|1[0-9]|2[0-9]))))|(?:[0-9]{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:[01][0-9]|2[0-8])))))(?:0[0-9]|1[0-9]|2[0-3])(?:[0-5][0-9])";

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length for CAGE Code and Serial number.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphanumericCageSnRegEx =
        (lb, ub) => @"[0-9A-Z-/]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphanumericRegEx =
        (lb, ub) => @"[0-9A-Z]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length for tag location.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphanumericTagLocationRegEx =
        (lb, ub) => @"[0-9A-Z ]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length with dash.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphanumericWithDashRegEx =
        (lb, ub) => @"[A-Z0-9-]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for alphanumeric character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphanumericWithPlusRegEx =
        (lb, ub) => @"[0-9A-Z+]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for alphabetic variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetAlphaRegEx = (lb, ub) =>
        @"[A-Z]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for Character Set 82 except '+' character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetCharacterSet82ExceptPlusRegEx = (lb, ub) =>
        @"[-!""%&'()*,./0-9:;<=>?A-Z_a-z]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for Character Set 82 character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetCharacterSet82RegEx = (lb, ub) =>
        @"[-!""%&'()*+,./0-9:;<=>?A-Z_a-z]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for character strings of variable length
    ///   representing latitude, longitude and attitude.
    /// </summary>
    private static readonly Func<string> GetLatitudeLongitudeAttitudeRegEx =
        () => @"-?\d{1,2}(.\d{1,5})?/-?\d{1,3}(.\d{1,5})?/-?\d{1,4}";

    /// <summary>
    ///   Returns a regular expression for numeric character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetNumericRegEx =
        (lb, ub) => @"[0-9]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for numeric character strings of variable length.
    /// </summary>
    private static readonly Func<int, int, string> GetNumericWithDotRegEx =
        (lb, ub) => @"[0-9.]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for numeric character strings of variable length that may include a leading minus
    ///   sign.
    /// </summary>
    private static readonly Func<int, int, string> GetNumericWithMinusRegEx =
        (lb, ub) => @"-?[0-9]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");

    /// <summary>
    ///   Returns a regular expression for numeric character strings of variable length that may include plus signs.
    /// </summary>
    private static readonly Func<int, int, string> GetNumericWithPlusRegEx =
        (lb, ub) => @"[0-9+]" + (ub >= lb ? $@"{{{lb},{ub}}}" : "+");
    
    /// <summary>
    ///   Returns a regular expression for character strings indicating Yes (Y) and No (N).
    /// </summary>
    private static readonly Func<string> GetYesNoLetterRegEx = () => @"[YN]";

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
                    @"^\+$")
            },
            {
                2, new EntityDescriptor(
                    "AMPERSAND",

                    // ReSharper disable once StringLiteralTypo
                    "The 'AMPERSAND' character. ICCBBA",
                    @"^\&$")
            },
            {
                3, new EntityDescriptor(
                    "EQUAL",

                    // ReSharper disable once StringLiteralTypo
                    "The 'EQUAL' character. ICCBB",
                    @"^\=$")
            },
            {
                4, new EntityDescriptor(
                    "FUNC1",

                    // ReSharper disable once StringLiteralTypo
                    "Function 1 (FUNC1) character. Appears in the first position following the symbology start character of a Code 128, Code 49, or Code 16K Symbol to signify a GS1-controlled symbol.",
                    @"^\x1D$")
            },
            {
                5,
                new EntityDescriptor(
                    "ISOIEC15434PREAMBLE",
                    "'Left Square Bracket', 'Right Parenthesis', 'Greater Than Sign', 'Record Separator' characters. Data structure compliant to ISO/IEC 15434, Information technology - Automatic Identification and Data Capture Techniques - Syntax for High Capacity ADC Media.",
                    @"^\[\)\>\x1E")
            },
            {
                6, new EntityDescriptor(
                    "HYPHEN",

                    // ReSharper disable once StringLiteralTypo
                    "The 'HYPHEN' or 'MINUS' character. Pharmaceutical Central Number (PZN), controlled by IFA-ABDATA, Germany (Registration of this system identifier expires on 2016-07-01). Replaced by '9N'.",
                    @"^\-$")
            },
            {
                7, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "EXCLAMATIONMARK",

                    // ReSharper disable once StringLiteralTypo
                    "The 'EXCLAMATION MARK'. Eurocode-IBLS ",
                    @"^\!$")
            },
            {
                2000,
                new EntityDescriptor(
                    "CONTAINER TYPE",
                    "Container Type (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                2001, new EntityDescriptor(
                    "RETURNABLE CONTAINER",

                    // ReSharper disable once StringLiteralTypo
                    "Returnable Container Identification Code assigned by the container owner or the appropriate regulatory agency (e.g., a metal tub, basket, reel, unit load device (ULD), trailer, tank, or intermodal container) (excludes gas cylinders See '2B').",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                2002,
                new EntityDescriptor(
                    "GAS CYLINDER CONTAINER",
                    "Gas Cylinder Container Identification Code assigned by the manufacturer in conformance with U.S. Department of Transportation (D.O.T.) standards.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                2003,
                new EntityDescriptor(
                    "MOTOR FREIGHT TRANSPORT EQUIPMENT",
                    "Motor Freight Transport Equipment Identification Code assigned by the manufacturer in conformance with International Organisation for Standardization (ISO) standards.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                2004, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "Standard Carrier Alpha Code (SCAC) (4 alphanumeric characters) and an optional carrier assigned trailer number (one to ten alphanumeric characters). When used, the carrier assigned trailer number is separated from the SCAC by a dash '-'.",
                    $@"^{GetAlphanumericRegEx(4, 4)}(.{GetAlphanumericRegEx(1, 10)})?$")
            },
            {
                2005, new EntityDescriptor(
                    "RECEPTACLE ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Receptacle Asset Number - Consisting of two joined parts: - Identification of an organisation in accordance with ISO/IEC 15459 and a unique entity identification assigned in accordance with rules established by the issuing agency. - A unique serial number assigned by the entity, ending with a 3-character container type code taken from EDIFACT Code List 8053 or UPU standard M82-3. (If the container type code listed is less than three characters in length, the field will be dash '-' filled left to the length of three characters).",
                    $@"^{GetAlphanumericRegEx(1, 32)}({GetAlphanumericRegEx(3, 3)}|-{GetAlphanumericRegEx(2, 2)}|--{GetAlphanumericRegEx(1, 1)}|---)$")
            },
            {
                2007,
                new EntityDescriptor(
                    "CONTAINER SERIAL",
                    "Container Serial Number. According to ISO 6346. OC EI CSN CD, where the OC is the three letter owner code assigned in cooperation with BIC, the EI is the one letter equipment category identifier, the CSN is a 6-digit unique container identification assigned by the equipment owner, and CD is a modulus 11 check digit calculated in accordance with Annex A, ISO 6346.",
                    $@"^{GetAlphaRegEx(4, 4)}{GetNumericRegEx(7, 7)}$")
            },
            {
                2008,
                new EntityDescriptor(
                    "RETURNABLE CONTAINER OWNER ",
                    "Identification of a Returnable Container owner assigned in cooperation with BIC.",
                    $@"^{GetAlphanumericRegEx(3, 3)}$")
            },
            {
                2009,
                new EntityDescriptor(
                    "CONTAINER SIZE ",
                    "Container Size/Type Code. According to ISO 6346, Section 4.2.",
                    $@"^{GetAlphanumericRegEx(4, 4)}$")
            },
            {
                2010,
                new EntityDescriptor(
                    "CONTAINER OWNERSHIP",
                    "Container Ownership Code. Actual four-character abbreviation marked on the container by the owner. For DOD owned containers see Defense Transportation Regulation App EE-6.",
                    $@"^{GetAlphanumericRegEx(4, 4)}$")
            },
            {
                2011,
                new EntityDescriptor(
                    "VAN ",
                    "Van Number (complete number minus check digit).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                2012,
                new EntityDescriptor(
                    "VAN CHECK DIGIT",
                    "Check digit of Van Number identified in 11B.",
                    $@"^{GetNumericRegEx(1, 1)}$")
            },
            {
                2013,
                new EntityDescriptor(
                    "CONTAINER ",
                    "Container Number Code (last 5 digits of number not counting check digit).",
                    $@"^{GetNumericRegEx(5, 5)}$")
            },
            {
                2014,
                new EntityDescriptor(
                    "TAG STATUS ",
                    "Tag Status. Y=Authorized / N=Unauthorized.",
                    $@"^{GetYesNoLetterRegEx()}$")
            },
            {
                2015, new EntityDescriptor(
                    "DANGEROUS CARGO CLASS ",

                    // ReSharper disable once StringLiteralTypo
                    "Dangerous Cargo Class. IMDG Class in the format 'n.na' where n = numeric, decimal point expressly encoded, and a = conditional alphabetic qualifier. http://docs.imo.org/.",
                    @"^\d(.\d[A-Z]?)?$")
            },
            {
                2016,
                new EntityDescriptor(
                    "DANGEROUS GOODS ",
                    "UN Code for Dangerous Goods ",
                    $@"^{GetAlphanumericRegEx(4, 4)}$")
            },
            {
                2017,
                new EntityDescriptor(
                    "TRANSPORTATION SUBJECT ",
                    "Name Of Transportation Subject. Vessel name or vehicle code/train trip number in English.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                2018, new EntityDescriptor(
                    "VESSEL REGISTRATION ",

                    // ReSharper disable once StringLiteralTypo
                    "Vessel Registration Number. The three letters 'IMO' followed by the seven-digit number assigned to all ships by IHS Fairplay when constructed. http://www.imonumbers.lrfairplay.com/.",
                    @"^IMO\d{7}$")
            },
            {
                2019,
                new EntityDescriptor(
                    "VOYAGE",
                    "Voyage number/Trip number. Letter and number.",
                    $@"^{GetAlphanumericRegEx(18, 18)}$")
            },
            {
                2020,
                new EntityDescriptor(
                    "VESSEL COUNTRY ",
                    "Vessel Country. ISO 3166-1 Alpha 2 Code.",
                    $@"^{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                2021,
                new EntityDescriptor(
                    "ELECTRONIC SEAL",
                    "Reserved for Electronic Seal Numbers. Comprised of the 18185-1 seal tag ID - 32 bits and the ISO 14816 16-bit manufacturers ID (ISO 646).",
                    @"^.{{6}}$")
            },
            {
                2022, new EntityDescriptor(
                    "ENTRY",

                    // ReSharper disable once StringLiteralTypo
                    "Entry Number/Type. Comprised of the three-digit filer code, followed by the sevendigit entry number, and completed with the one digit check digit. Entry Filer Code represents the three-character alphanumeric filer code assigned to the filer or importer by CBP. Entry Number represents the seven-digit number assigned by the filer. The number may be assigned in any manner convenient, provided that the same number is not assigned to more than one CBP Form 7501. Leading zeros must be shown. Check Digit is computed on the previous 10 characters. The formula for calculating the check digit can be found in Appendix 1, CBP 7501 Instructions. Entry type is a two-digit code compliant to Block 2, CBP 7501 Instructions.",
                    $@"^{GetAlphanumericRegEx(11, 11)}\d{2}$")
            },
            {
                2023, new EntityDescriptor(
                    "SURETY ",

                    // ReSharper disable once StringLiteralTypo
                    "Surety Number. The three-digit numeric code that identifies the surety company on the Customs Bond. This code can be found in block 7 of the CBP Form 301, or is available through CBP's automated system to ABI filers, via the importer bond query transaction. For U.S. Government importations and entry types not requiring surety, code 999 should appear in this block. When cash or Government securities are used in lieu of surety, use code 998.",
                    @"^\d{3}$")
            },
            {
                2024,
                new EntityDescriptor(
                    "FOREIGN PORT OF LADING ",
                    "Foreign Port of Lading. 'Schedule K' (Classification of Foreign Ports by Geographic Trade Area and Country) for the foreign port at which the merchandise was actually laden on the vessel that carried the merchandise to the U.S. http://www.navigationdatacenter.us/wcsc/scheduleK/schedule k.htm.",
                    @"^\d{5}$")
            },
            {
                2025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION ",
                    "Identification of a Party to a Transaction as defined in ISO 17364, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the RTI serial number that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                2026, new EntityDescriptor(
                    "UNIQUE RETURNABLE TRANSPORT ITEM ",

                    // ReSharper disable once StringLiteralTypo
                    "Unique Returnable Transport Item Identifier comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'RTI Number' (RTIN), followed by the '+' character, followed by the supplier assigned (or managed) 'RTI Serial Number' (RTISN) that is globally unique within the CIN holder's domain; in the format IAC CIN RTIN + RTISN (spaces added for visual clarity only; they are not part of the data). See Annex C.11.",
                    $@"^{GetAlphanumericWithPlusRegEx(1, -1)}$")
            },
            {
                2027, new EntityDescriptor(
                    "LLC RTI ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Asset Identifier of a Large Load Carrier (LLC) Returnable Transport Item (RTI) with a side  base of = 1000 mm, as defined in ISO 17365:2013, tertiary packaging, layer 3 comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by RTI Type Code 'RTITC', followed by the '+' character, followed by the owner assigned (or managed) RTI Serial Number 'RTISN' that is globally unique within the CIN holder's domain in the format IAC CIN RTITC + RTISN (spaces added for visual clarity only; they are not part of the data).",
                    $@"^{GetAlphanumericWithPlusRegEx(20, 50)}$")
            },
            {
                2028, new EntityDescriptor(
                    "SLC RTI ASSET",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Asset Identifier of a Small Load Carrier (SLC) Returnable Transport Item with a side base of < 1000 mm, as defined in ISO 17364:2013 (RTI), tertiary packaging, layer 2  comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by RTI Type Code 'RTITC', followed by the '+' character, followed by the owner assigned (or managed) RTI Serial Number 'RTISN' that is globally unique within the CIN holder's domain in the format IAC CIN RTITC + RTISN (spaces added for visual clarity only; they are not part of the data).",
                    $@"^{GetAlphanumericWithPlusRegEx(20, 50)}$")
            },
            {
                2029, new EntityDescriptor(
                    "RPI IDENTIFIER",

                    // ReSharper disable once StringLiteralTypo
                    "Globally Unique Returnable Packaging Item (RPI) identifier of the category packaging aid (lid, blister, inlay, ...) comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'RPI Number' RPIN, followed by the '+' character, followed by the owner assigned (or managed) 'RPI Serial Number' RPISN that is globally unique within the CIN holder's domain in the format IAC CIN RPIN + RPISN (spaces added for visual clarity only; they are not part of the data).",
                    $@"^{GetAlphanumericWithPlusRegEx(1, 50)}$")
            },
            {
                2030,
                new EntityDescriptor(
                    "PACKAGING ITEM",
                    "Packaging Item Number. Number to identify the type of packaging item (material) used when packing products and packages The number will enable packaging item (material) be identified and separated from products, packages, Returnable Transport Items (RTIs) and Returnable Packaging Items (RPIs) during packing. The number is constructed as a sequence of minimum 1 data element: Packaging item (material) number that is unique within the holder's domain.",
                    $@"^{GetAlphanumericRegEx(2, 35)}$")
            },
            {
                2031,
                new EntityDescriptor(
                    "PACKAGING ",
                    "Global Unique Packaging Number Global unique number to identify the type of packaging item (material) used when packing products and packages. The global unique number will enable packaging items (materials) be identified and separated from products, packages, Returnable Transport Items (RTIs) and Returnable Packaging Items (RPIs) during packing. The number is constructed as a sequence of 3 concatenated data elements: The IAC, followed by the CIN, followed by the Packaging item (material) number that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(6, 35)}$")
            },
            {
                2055,
                new EntityDescriptor(
                    "RPI",
                    "Global Unique Returnable Packaging Item (RPI) as defined in ISO 17364, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the RPI serial number that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, 50)}$")
            },
            {
                3000,
                new EntityDescriptor(
                    "CUSTOMER ITEM CODE CONT.",
                    "Continuation of an Item Code (Category 16) assigned by Customer that is too long for a required field size.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                3001,
                new EntityDescriptor(
                    "SUPPLIER TRACEABILITY CODE CONT.",
                    "Continuation of Traceability Code (Category 20) assigned by Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                3002,
                new EntityDescriptor(
                    "SUPPLIER FREE TEXT CONT.",
                    "Continuation of Serial Number (Category 19) assigned by Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                3003,
                new EntityDescriptor(
                    "FREE TEXT CONT.",
                    "Continuation of Free Text (Category 26) mutually defined between Supplier/Carrier/Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                3004,
                new EntityDescriptor(
                    "TRANSACTION REFERENCE CONT.",
                    "Continuation of Transaction Reference (Category 11) mutually defined between Supplier/Carrier/Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                3005,
                new EntityDescriptor(
                    "SUPPLIER ITEM CODE CONT.",
                    "Continuation of Item Code (Category 16) Assigned by Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                4000, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYMMDD.",
                    $@"^{DatePatternYyMmDdZeros}$")
            },
            {
                4001, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format DDMMYY.",
                    $@"^{DatePatternDdMmYy}$")
            },
            {
                4002, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format MMDDYY.",
                    $@"^{DatePatternMmDdYy}$")
            },
            {
                4003, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YDDD (Julian).",
                    DatePatternYDddJulian)
            },
            {
                4004, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYDDD (Julian).",
                    DatePatternYyDddJulian)
            },
            {
                4005, new EntityDescriptor(
                    "DATE AND TYPE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO format YYMMDD immediately followed by an ANSI X12.3 Data Element Number 374 Qualifier providing a code specifying type of date (e.g., ship date, manufacture date).",
                    DatePatternYyMmDd)
            },
            {
                4006, new EntityDescriptor(
                    "DATE AND TYPE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO format YYYYMMDD immediately followed by an ANSI X12.3 Data Element Number 374 Qualifier providing a code specifying type of date (e.g., ship date, manufacture date).",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4007, new EntityDescriptor(
                    "MONTH",

                    // ReSharper disable once StringLiteralTypo
                    "Format MMYY.",
                    @"^((0[1-9])|(1[0-2]))\d{2}$")
            },
            {
                4008,
                new EntityDescriptor(
                    "EVENT, DATE AND TIME ",
                    "Event, Date, And Time. ISO format YYYYMMDDHHMM (24 hour clock - UTC) immediately followed by a UN/EDIFACT Code Qualifier 2005 providing a code specifying type of date), e.g., 11 [Date when goods are expected to be dispatched/shipped message is issued.]; 17 [Estimated delivery date/time when goods are expected to be delivered]; 35 [Date on which goods are delivered to their destination.]; 118 [Booking Confirmed]; 129 [Date when the vessel/merchandise departed the last foreign port in the exporting country.]; 132 [Date/time when the carrier estimates that a means of transport should arrive at the port of discharge or place of destination.]; 133 [Date/time when carrier estimates that a means of transport should depart at the place of departure]; 137 [Date/time when the supplier ships parts based on the customer's request. (Date when DESADV message is issued. Recommendation is the DESADV is issued within 30 minutes of goods being picked up at ShipFrom party]; 146 [Estimated Entry date (Customs) date on which the official date of a Customs Entry is anticipated.]; 151 [Import Date (Arrived at port with intent to unlade]; 186 Departs a Facility ('Gate-out)]; 204 [Date on which Customs releases merchandise to the carrier or importer]; 253 [Departs from a Port ('Vessel Departure')]; 252 [Arrives at a Port ('Vessel Arrival')]; 283 [Arrives at a Facility ('Gate-in)]; 342 [Conveyance Loaded]; 351 [Terminal Gate Inspection]; 411 [Ordered Stuffed]; 412 [Ordered Stripped]; 420 [Conveyance unloaded]; 534 [Repaired]; 677 [Confirmed Stuffed]; 678 [Confirmed Stripped]; 696 [Filing Date].",
                    $@"^{DatePatternYyyyMmDdHhMm}\d{{1,3}}$")
            },
            {
                4009,
                new EntityDescriptor(
                    "DATE",
                    "Date (structure and significance mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                4010, new EntityDescriptor(
                    "WEEK",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYWW.",
                    $@"^{DatePatternYyWw}$")
            },
            {
                4011,
                new EntityDescriptor(
                    "WEEK",
                    "Format YYYYWW.",
                    @"^([1-2][0-9])([0-9][0-9])(0[1-9]|[1-4][0-9]|5[0-3])$")
            },
            {
                4012, new EntityDescriptor(
                    "DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Format YYYYMMDD.",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4013, new EntityDescriptor(
                    "OLDEST AND NEWEST MANUFACTURING DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Oldest and Newest Manufacturing Date in the format YYWWYYWW.",
                    $@"^{DatePatternYyWw}{DatePatternYyWw}$")
            },
            {
                4014, new EntityDescriptor(
                    "EXPIRATION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Expiration Date (YYYYMMDD).",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4015, new EntityDescriptor(
                    "EXPIRATION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Expiration Date (DDMMYYYY).",
                    DatePatternDdMmYyyyy)
            },
            {
                4016, new EntityDescriptor(
                    "DATE OF MANUFACTURE ",

                    // ReSharper disable once StringLiteralTypo
                    "Production Date (YYYYMMDD) - Date of manufacture.",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4017, new EntityDescriptor(
                    "PRODUCTION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Production Date (DDMMYYYY).",
                    DatePatternDdMmYyyyy)
            },
            {
                4018,
                new EntityDescriptor(
                    "TAG ACTIVATION TIME",
                    "Tag Activation Time. YYYYMMDDHHMM (24 hour clock - UTC).",
                    $@"^{DatePatternYyyyMmDdHhMm}$")
            },
            {
                4019,
                new EntityDescriptor(
                    "TAG DEACTIVATION TIME ",
                    "Tag Deactivation Time. YYYYMMDDHHMM (24 hour clock - UTC).",
                    $@"^{DatePatternYyyyMmDdHhMm}$")
            },
            {
                4020, new EntityDescriptor(
                    "INSPECTION DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Inspection Date (DDMMYYYY).",
                    DatePatternDdMmYyyyy)
            },
            {
                4021,
                new EntityDescriptor(
                    "DOD MILSTAMP CODE",
                    "Required Delivery Date (DDD Julian) or DOD MILSTAMP Code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                4022,
                new EntityDescriptor(
                    "RECORD TIME",
                    "Record Time. YYYYMMDDHHMM (24 hour clock - UTC).",
                    $@"^{DatePatternYyyyMmDdHhMm}$")
            },
            {
                4023, new EntityDescriptor(
                    "UTC DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Date, represented in modified UTC compliant form: yyyy[mm[dd[hh[mm[ss[fff]]]]]][poooo] where square brackets indicate optionality and yyyy is the year, mmdd the month and day, hhmmss the time of day in hours minutes and seconds, fff the fractions of sections and poooo the offset from UTC expressed in hours and minutes, the offset being positive if p is a point (.), negative if P is a minus sign (-). EXAMPLE: 2005 - (UTC) calendar year 2005; 200505 - (UTC) calendar month May 2005; 20050518 - (UTC) 18 May 2005; 200505181247 - 12:47 UTC on 18 May 2005; 200505181247.0100 - 12:47 local time, being 11:47 UTC, on 18 May 2005; 20050518124723099 - 99 milliseconds after UTC 12:47:23 on 18 May 200.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                4024,
                new EntityDescriptor(
                    "QUALIFIED DATE",
                    "Qualified Date, comprising the concatenation of: - an ISO/IEC 15459 issuing agency code; - a date qualifier conforming to the specifications of that issuing agency; - a date whose format and interpretation comply with the specifications of the issuing agency for that date qualifier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                4025, new EntityDescriptor(
                    "BEST BEFORE DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Best before date: (YYYYMMDD). Example: 25D20170202 = February 2, 2017.",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4026, new EntityDescriptor(
                    "FIRST FREEZE DATE",

                    // ReSharper disable once StringLiteralTypo
                    "First freeze date (YYYYMMDD). The first freeze date is defined as the date on which products are frozen directly after slaughtering, harvesting, catching or after initial processing. Example: 26D20170721 = July 21, 2017.",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4027, new EntityDescriptor(
                    "HARVEST DATE",

                    // ReSharper disable once StringLiteralTypo
                    "Harvest date (YYYYMMDD). The date when an animal was slaughtered or killed, a fish has been harvested, or a crop was harvested. Example: 27D20170615 = June 15, 2017.",
                    $"^{DatePatternYyyyMmDd}$")
            },
            {
                4028,
                new EntityDescriptor(
                    "HARVEST DATE RANGE",
                    "Harvest date range (YYYYMMDDYYYYMMDD). The start date and end date range over which harvesting occurred. For example; animals were slaughtered or killed, fish were harvested, or a crop was harvested. The data stream is defined as the first YYYYMMDD as the start date and the last YYYYMMDD as the end date. Example: 28D2017012320170214 = Start; January 23, 2017. End; February 14, 2017.",
                    $"^{DatePatternYyyyMmDd}{DatePatternYyyyMmDd}$")
            },
            {
                5000,
                new EntityDescriptor(
                    "RESTRICTED SUBSTANCES CLASSIFICATION",
                    "Restricted Substances Classification - 'Environmental Classification Code' including Lead-Free (Pb-Free) finish categories defined in JESD97 (IPC JEDEC J-STD-609), and future industry or governmental agency assigned codes related to environmental regulatory compliance and hazardous material content.",
                    $@"^{GetAlphaRegEx(2, 2)}$")
            },
            {
                5001,
                new EntityDescriptor(
                    "AIR PRESSURE",
                    "Air Pressure - (altitude) expressed in Pascal's as the standard international measure.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                5002,
                new EntityDescriptor(
                    "MAXIMUM ALLOWED TEMPERATURE",
                    "Maximum Allowed Temperature. Maximum permitted temperature; Degrees Celsius, '-' (minus) encoded, if required.",
                    $@"^{GetNumericWithMinusRegEx(1, 4)}$")
            },
            {
                5003,
                new EntityDescriptor(
                    "MINIMUM ALLOWED TEMPERATURE",
                    "Minimum Allowed Temperature. Minimum permitted temperature; Degrees Celsius, '-' (minus) encoded, if required.",
                    $@"^{GetNumericWithMinusRegEx(1, 4)}$")
            },
            {
                5004,
                new EntityDescriptor(
                    "MAXIMUM ALLOWED RELATIVE HUMIDITY",
                    "Maximum Allowed Relative Humidity. Maximum permitted relative humidity, implied as percent.",
                    $@"^{GetNumericRegEx(1, 2)}$")
            },
            {
                5005,
                new EntityDescriptor(
                    "MINIMUM ALLOWED RELATIVE HUMIDITY",
                    "Minimum Allowed Relative Humidity. Maximum permitted relative humidity, expressed as percent.",
                    $@"^{GetNumericRegEx(1, 2)}$")
            },
            {
                5006,
                new EntityDescriptor(
                    "REFRIGERATOR CONTAINER TEMPERATURE",
                    "Refrigerator Container Temperature. For temperature-controlled cargo, target specified by shipper, Degrees Celsius, '-' (minus) encoded, if required.",
                    $@"^{GetNumericWithMinusRegEx(1, 4)}$")
            },
            {
                5010,
                new EntityDescriptor(
                    "CUMULATIVE TIME TEMPERATURE INDEX",
                    "Cumulative Time Temperature Index - expressed as the number of measurements or counts.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                5011,
                new EntityDescriptor(
                    "TIME TEMPERATURE INDEX",
                    "Time Temperature Index - Next Higher Assembly - expressed as the number of measurements or counts.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                5012, new EntityDescriptor(
                    "PACKAGING MATERIAL",

                    // ReSharper disable once StringLiteralTypo
                    "Declaration of Packaging Material Category*, Code* and Weight for a given packaging material used in a given packaging according to the EU packaging and packaging waste directive. (Material category and code defined in Annex M). 12ECCMMMMMMNNNNNUU where:  - '12E' (an3) is the Data Identifier;  - 'CC' (n2) is the Material Category per Annex M;  - 'MMMMMM' (an1...6) is the Material Code per Annex M;  - 'NNNNN' (n5) Material Weight, including decimal point (e.g., 12.12); - 'UU' (an2) is the Unit of measure for weight (e.g., KG, GR, LB or OZ per ANSI X12.3 as in Annex D).",
                    $@"^{GetNumericRegEx(2, 2)}{GetAlphanumericWithDashRegEx(1, 6)}{GetNumericWithDotRegEx(5, 5)}{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                5013,
                new EntityDescriptor(
                    "MSL",
                    "The data following DI 13E will be one of the MSL indicators (1, 2, 2a, 3, 4, 5, 5a, 6) as shown in the LEVEL column in Table 5-1 of JEDEC standard IPC/JEDEC J-STD-020E. The Table is shown below for reference only; the currently released version of the referenced standard shall be used to obtain the correct MSL for the actual component. Example: 13E2a",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                6000,
                new EntityDescriptor("LOOPING HEADER", "Looping Header.", $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                6001,
                new EntityDescriptor(
                    "PARENT",
                    "My 'parent' is . . . Unique identifier followed by a Data Identifier and associated data (for use with returnable packaging). This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                6003,
                new EntityDescriptor(
                    "NUMBER OF CHILDREN",
                    "I have ______ children . . . (for use with returnable packaging, e.g.; 3F10, for ten children). This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                6004,
                new EntityDescriptor(
                    "LOGICAL ASSIGNMENT",
                    "Logical Assignment of a Page of Information within a group of pages that are spread across several data carriers, structured as a sequence of up to three (3) concatenated data elements, separated by a slash ( / ) :  Page number (required), followed by page count (optional, required for the last page), followed by an alphanumeric group ID (optional; if used then required for all pages and structured in accordance with ISO/IEC 15459-3 as a sequence of 3 data elements: Issuing Agency Code, followed by the Company Identification Number, followed by an alphanumeric code unique within the issuer's domain). Trailing slashes are optional.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                6005,
                new EntityDescriptor(
                    "CHILDREN",
                    "I have ______ children and they are . . . (for use with returnable packaging) This Data Identifier must immediately follow the field (constructed of a Data Identifier, data and a group separator) with which it is associated.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8000, new EntityDescriptor(
                    "NAME",

                    // ReSharper disable once StringLiteralTypo
                    "Name of Party. Name of a party followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3035 'Party Qualifier', e.g., BY [Buyer]; CF   [Container operator]; CN [Consignee]; CS [Consolidator]; DEI  [Vessel operator/captain of vessel]; FA [Operational staff code]; IM [Importer]; MF   [Manufacturer]; OS [Shipper]; SE [Seller]; ST [Ship To]; UC [Ultimate consignee].",
                    $@"^{GetAlphanumericWithPlusRegEx(1, 60)}$")
            },
            {
                8001,
                new EntityDescriptor(
                    "EMPLOYEE ID",
                    "Employee Identification Code assigned by employer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8002,
                new EntityDescriptor(
                    "SOCIAL SECURITY NUMBER",
                    "U.S. Social Security Number.",
                    $@"^{GetNumericRegEx(9, 9)}$")
            },
            {
                8003,
                new EntityDescriptor(
                    "NON-EMPLOYEE ID",
                    "ID Number for Non-Employee (internally assigned or mutually defined) (e.g., contract workers, vendors, service, and delivery personnel).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8004,
                new EntityDescriptor(
                    "NATIONAL SOCIAL SECURITY NUMBER",
                    "National Social Security Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            { 8005, new EntityDescriptor("LAST NAME", "Last Name.", $@"^{GetAlphanumericRegEx(1, -1)}$") },
            {
                8006,
                new EntityDescriptor("PARTY NAME", "Party Name (Line 2).", $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                8007, new EntityDescriptor(
                    "CONTACT PHONE",

                    // ReSharper disable once StringLiteralTypo
                    "Contact Phone. Country Code, Area Code, Exchange, number [XX YYY ZZZ ZZZZ].",
                    $@"^{GetNumericRegEx(10, 12)}$")
            },
            {
                8008,
                new EntityDescriptor("CONTACT EMAIL", "Contact Email.", $@"^{GetAlphanumericRegEx(3, 35)}$")
            },
            {
                8009, new EntityDescriptor(
                    "CONSIGNEE NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Consignee Number. The unique identifying number can be the IRS, EIN, SSN, or the CBP assigned number, as required on the Security Filing. Only the following formats shall be used: IRS EIN: NN-NNNNNNN; IRS EIN w/ suffix: NN-NNNNNNNXX; SSN:   NNN-NN-NNNN; CBP assigned nbr: YYDDPP-NNNNN.",
                    $@"^{GetAlphanumericRegEx(10, 12)}$")
            },
            {
                8010,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Personal Identification Code (first initial, Last initial, last four of SSN).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8011,
                new EntityDescriptor(
                    "FIRST NAME AND MIDDLE INITIAL",
                    "First Name and Middle Initial.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8012,
                new EntityDescriptor(
                    "MILITARY GRADE",
                    "Military Grade (E1-E9, W1-W5, and O1-O10).",
                    $@"^{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                8015, new EntityDescriptor(
                    "NI NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "A National Identification Number, National Identity Number, or National Insurance Number used as a means of identifying individuals within a country for the purposes of work, taxation, government benefits, health care, and other governmentallyrelated functions. This structure of the identifier is DI (15H) followed by the ISO 3166-1 Alpha2 Country Code followed by the predominant government assigned identification code for individuals.",
                    $@"^{GetAlphanumericRegEx(3, 22)}$")
            },
            {
                8025,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Globally Unique Personal ID. assigned by a holder of a Company Identification Code (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as sequence of 3 concatenated data elements: IAC followed by CIN, followed by the ID unique within the holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                8026,
                new EntityDescriptor(
                    "PERSONAL ID",
                    "Globally Unique Personal ID, with a \"Party Qualifier\" code value from EDIFACT Code List 3035, assigned by a holder of a Company Identification Code (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 5 concatenated data elements: IAC followed by CIN, followed by an ID unique within the CIN holder\'s domain, followed by the Plus character (+) and a code value from EDIFACT Code List 3035 \"Party Qualifier.\"",
                    $@"^{GetAlphanumericRegEx(3, 35)}\+{GetAlphaRegEx(1, 3)}$")
            },
            {
                9000,
                new EntityDescriptor(
                    "VIN",
                    "Exclusive Assignment - Vehicle Identification Number (VIN) as defined in the U.S. under 49 CFR, Section Section  565 and internationally by ISO 3779. (These are completely compatible data structures).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                9002,
                new EntityDescriptor(
                    "ABBREVIATED VIN ",
                    "Abbreviated VIN Code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                9004,
                new EntityDescriptor(
                    "TRANSPORT VEHICLE IDENTIFIER ",
                    "Globally unique transport vehicle identifier (e.g., Trucks) consisting of the Vehicle Identification Number (VIN) as defined in the U.S. under 49 CFR Section Section  565, and internationally by ISO 3779, followed by the '+' character, then followed by the government-issued Vehicle Registration License Plate Number in the form of '4I' 'VIN' '+' 'government-issued Vehicle Registration License Plate Number' (quotes and spaces shown for clarity only; they are not part of the data).",
                    $@"^{GetAlphanumericWithPlusRegEx(1, -1)}$")
            },
            {
                9005, new EntityDescriptor(
                    "PRODUCTION VEHICLE IDENTIFIER ",

                    // ReSharper disable once StringLiteralTypo
                    "Unique production vehicle identifier that will be used during the vehicle production processes, consisting of the Body Tag Number (BTN; or any other descriptor used to identify the raw car body, or stated another way, the assemblage of parts that are used to start the vehicle's production), followed by the '+' character, then followed by the Production Order Number (PON), followed by the '+' character, and then followed by the Manufacturer-assigned Serial Number (SN). NOTE: The SN component shall be replaced by the VIN as soon as the VIN is available in the assembly process. The construction will be as follows; '5I' 'BTN' '+' 'PON' '+' 'SN'            changing to (when VIN available); '5I' 'BTN' '+' 'PON' '+' 'VIN'   NOTE: Quotes and spaces are shown for clarity only; they are not part of the data. NOTE: This DI is never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character. Examples: SN version: 5IABCD1234+CO1234+W0L201600500001; VIN version:  5IABCD1234+CO1234+W0L0XAP68F4050901.",
                    $@"^{GetAlphanumericWithPlusRegEx(1, -1)}$")
            },
            {
                10000,
                new EntityDescriptor(
                    "LICENSE PLATE",
                    "Unique license plate number.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                10001,
                new EntityDescriptor(
                    "UNBREAKABLE UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which is the lowest level of packaging, the unbreakable unit.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                10002,
                new EntityDescriptor(
                    "TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which contains multiple packages.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                10003,
                new EntityDescriptor(
                    "EDI UNBREAKABLE UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which is the lowest level of packaging, the unbreakable unit and which has EDI data associated with the unit.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                10004,
                new EntityDescriptor(
                    "EDI TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a transport unit which contains multiple packages and which is associated with EDI data.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                10005,
                new EntityDescriptor(
                    "MIXED TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a mixed transport unit containing unlike items on a single customer transaction and may or may not have associated EDI data.",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                10006,
                new EntityDescriptor(
                    "MASTER TRANSPORT UNIT LICENSE PLATE",
                    "Unique license plate number assigned to a master transport unit containing like items on a single customer transaction and may or may not have associated EDI data.",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                10007,
                new EntityDescriptor(
                    "VEHICLE REGISTRATION LICENSE PLATE NUMBER",
                    "Vehicle Registration License Plate Number (not unique without identification of country and issuing governmental region/authority)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                10008, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MMSI",

                    // ReSharper disable once StringLiteralTypo
                    "Maritime Mobile Service Identity (MMSI). A nine digit number regulated by the International Telecommunications Union to uniquely identify a ship or a coast radio station. Example:  8J211123456.",
                    $@"^{GetNumericRegEx(9, 9)}$")
            },
            {
                11000,
                new EntityDescriptor(
                    "CUSTOMER ORDER NUMBER",
                    "Order Number assigned by Customer to identify a Purchasing Transaction (e.g., purchase order number).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11001,
                new EntityDescriptor(
                    "SUPPLIER ORDER NUMBER",
                    "Order Number assigned by Supplier to identify a Purchasing Transaction.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11002,
                new EntityDescriptor(
                    "SUPPLIER BILL OF LADING",
                    "Bill of Lading/Waybill/Shipment Identification Code assigned by Supplier/Shipper.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11003,
                new EntityDescriptor(
                    "CARRIER BILL OF LADING",
                    "Bill of Lading/Waybill/Shipment Identification Code assigned by Carrier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11004,
                new EntityDescriptor(
                    "ORDER LINE",
                    "Line Number of the order assigned by Customer to identify a Purchasing Transaction.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11005,
                new EntityDescriptor(
                    "REFERENCE NUMBER",
                    "Reference Number assigned by the Customer to identify a Shipment Authorization (Release) against an established Purchase Order.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11006,
                new EntityDescriptor("PRO#", "PRO# Assigned by Carrier.", $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11007,
                new EntityDescriptor(
                    "CARRIER MODE",
                    "Carrier Mode in Free Text format mutually defined between Customer and Supplier (e.g., Air, Truck, Boat, Rail).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11008,
                new EntityDescriptor("CONTRACT NUMBER", "Contract Number.", $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11009,
                new EntityDescriptor(
                    "TRANSACTION REFERENCE",
                    "Generic Transaction Reference Code (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11010,
                new EntityDescriptor("INVOICE NUMBER", "Invoice Number.", $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11011,
                new EntityDescriptor(
                    "PACKING LIST NUMBER",
                    "Packing List Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11012, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "SCAC (Standard Carrier Alpha Code) (an4 - dash '-' filled left) and carrier assigned PROgressive number.",
                    $@"^{GetAlphanumericRegEx(5, 29)}$")
            },
            {
                11013, new EntityDescriptor(
                    "BILL OF LADING NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Bill of Lading Number /Transport Receipt Number SCAC + Container cargo's B/L number or waybill number.",
                    $@"^{GetAlphanumericRegEx(5, 16)}$")
            },
            {
                11014,
                new EntityDescriptor(
                    "ORDER AND LINE",
                    "Combined Order Number and Line Number in the format nn...nn+nn...n where a plus (+) symbol is used as a delimiter between the Order Number and Line Number.",
                    $@"^{GetNumericWithPlusRegEx(1, -1)}$")
            },
            {
                11015, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "KANBAN",

                    // ReSharper disable once StringLiteralTypo
                    "KANBAN Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DELINS",

                    // ReSharper disable once StringLiteralTypo
                    "DELINS Number:  code assigned to identify a document which contains delivery information.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            { 11017, new EntityDescriptor("CHECK", "Check Number.", $@"^{GetAlphanumericRegEx(1, -1)}$") },
            {
                11018,
                new EntityDescriptor("REFERENCE", "Structured Reference.", $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11019,
                new EntityDescriptor(
                    "FOREIGN MILITARY SALES CASE",
                    "Foreign Military Sales Case Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11020,
                new EntityDescriptor(
                    "LICENSE IDENTIFIER",
                    "License Identifier, being a globally unique identifier for a license or contract under which items are generated, submitted for processing and/or paid for, that is constructed by concatenating: - an ISO/IEC 15459 issuing agency code; - a license or contract number which accords with specifications of the issuing agency concerned; and that: - comprises only upper case alphabetic and/or numeric characters; - is unique (that is, is distinct from any other ISO/IEC 15459 compliant identifier) within the domain of the issuing agency6; - cannot be derived from any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, its end.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11021,
                new EntityDescriptor(
                    "CUSTOMER DATA",
                    "Customer Data, being data that: - from a customer perspective, is related to or associated with an item or transaction, or to a batch or related items or transactions, and - comprises up to 35 printable characters and/or spaces, other than plus (+), drawn from the character set defined in ISO/IEC 646.",
                    $@"^{GetCharacterSet82ExceptPlusRegEx(1, -1)}$")
            },
            {
                11022,
                new EntityDescriptor(
                    "TRANSACTION AUTHENTICATION",
                    "'22K' Transaction Authentication Information, being a value, constructed by concatenating: - an ISO/IEC 15459 issuing agency code; - a value which accords with specifications of the issuing agency concerned, that allows verification of the authenticity of the transaction concerned and, in particular, that the transaction was initiated by the party, claimed within the transaction to have been its initiator, by: - the recipient of a transaction, and/or - one or more of the parties involved in its handling or processing, and/or - a trusted third party.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11025,
                new EntityDescriptor(
                    "CARRIER TRANSPORT UNITS GROUPINGS",
                    "Global Unique Identification of Groupings of Transport Units Assigned by the Carrier, defined as: Identification of a Party to a Transaction as defined assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the Bill of Lading or Waybill or Shipment Identification Code that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11026,
                new EntityDescriptor(
                    "SHIPPER TRANSPORT UNITS GROUPINGS",
                    "Global Unique Identification of Groupings of Transport Units Assigned by the Shipper, defined as: Identification of a Party to a Transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the Bill of Lading or Waybill or Shipment Identification Code that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                11027,
                new EntityDescriptor(
                    "QUOTATION",
                    "Supplier Assigned Quotation Number - Number assigned to a quotation by the supplier in response to a request for quote from the customer.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                12000,
                new EntityDescriptor(
                    "STORAGE LOCATION",
                    "Storage Location.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            { 12001, new EntityDescriptor("LOCATION", "Location.", $@"^{GetAlphanumericRegEx(1, -1)}$") },
            {
                12002,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' Location Code defined by an industry standard or mutually defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12003,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' Location Code defined by an industry standard or mutually defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12004,
                new EntityDescriptor(
                    "COUNTRY OF ORIGIN",
                    "Country of Origin, two-character ISO 3166 country code. With agreement of trading partners and when the Country of Origin is mixed, Country Code 'AA' shall be used.",
                    $@"^{GetAlphaRegEx(2, 2)}$")
            },
            {
                12005,
                new EntityDescriptor(
                    "SHIP FOR",
                    "'Ship For:' Location Code defined by an industry standard or mutually defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12006,
                new EntityDescriptor(
                    "ROUTE CODE",
                    "Route Code assigned by the supplier to designate a specific transportation path.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DODAAC",

                    // ReSharper disable once StringLiteralTypo
                    "6-character Department of Defense Activity Code (DoDAAC).",
                    $@"^{GetAlphanumericRegEx(6, 6)}$")
            },
            {
                12008,
                new EntityDescriptor(
                    "PORT OF EMBARKATION",
                    "Port of Embarkation - Mutually Defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12009,
                new EntityDescriptor(
                    "PORT OF DEBARKATION",
                    "Port of Debarkation - Mutually Defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12011, new EntityDescriptor(
                    "LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
                    $@"^{GetLatitudeLongitudeAttitudeRegEx()}$")
            },
            {
                12012, new EntityDescriptor(
                    "SHIP TO",

                    // ReSharper disable once StringLiteralTypo
                    "'Ship To:' Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
                    $@"^{GetLatitudeLongitudeAttitudeRegEx()}$")
            },
            {
                12013, new EntityDescriptor(
                    "SHIP FROM",

                    // ReSharper disable once StringLiteralTypo
                    "'Ship From:' Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
                    $@"^{GetLatitudeLongitudeAttitudeRegEx()}$")
            },
            {
                12015, new EntityDescriptor(
                    "SHIP FOR",

                    // ReSharper disable once StringLiteralTypo
                    "Ship For: Location (Latitude/Longitude/Altitude) encoded in the format xnn.nnnnn/xnnn.nnnnnn/xnnnn.",
                    $@"^{GetLatitudeLongitudeAttitudeRegEx()}$")
            },
            {
                12016,
                new EntityDescriptor(
                    "TAG ACTIVATION LOCATION",
                    "Tag Activation Location. English location name (character set:  0-9, A-Z <Space>).",
                    $@"^{GetAlphanumericTagLocationRegEx(1, 60)}$")
            },
            {
                12017,
                new EntityDescriptor(
                    "TAG DEACTIVATION LOCATION",
                    "Tag Deactivation Location. English location name (character set:  0-9, A-Z <Space>).",
                    $@"^{GetAlphanumericTagLocationRegEx(1, 60)}$")
            },
            {
                12018, new EntityDescriptor(
                    "FAO FISHING AREA",

                    // ReSharper disable once StringLiteralTypo
                    "FAO fishing area code as defined by the Fisheries and Aquaculture Department of the FAO (http://www.fao.org. Search for Fishing Area Code sub-site). All characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed. Examples: 18L37.1.3 Western Mediterranean Sea, Sardinia; 18L47.B.1 Atlantic, Southeast, SEAFO Division, Namibia EEZ; 18L67    Pacific, Northeast.",
                    $@"^{GetCharacterSet82RegEx(2, 12)}$")
            },
            {
                12020,
                new EntityDescriptor(
                    "FIRST LEVEL",
                    "First Level (internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12021,
                new EntityDescriptor(
                    "SECOND LEVEL",
                    "Second Level (internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12022,
                new EntityDescriptor(
                    "THIRD LEVEL",
                    "Third Level (internally assigned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12023,
                new EntityDescriptor(
                    "FOURTH LEVEL",
                    "Fourth Level (internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12024,
                new EntityDescriptor(
                    "FIFTH LEVEL",
                    "Fifth Level (internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a Party to a Transaction, e.g., 25L IAC CIN LOC assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the physical internal location (LOC) that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                12026, new EntityDescriptor(
                    "LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "'26L' Location Code, being a code identifying a location or geographic area, or an associated group of such locations or areas, that has relevance to a related transaction and that complies with one or the structures defined in (a) to (f) below: a) two upper case alphabetic character corresponding to the ISO 3166-1 two alpha country code of the country in which, or consisting of which, the location(s) or area(s) are situated; b) three upper case alphabetic characters corresponding to the IATA code of the airport or city in, close to, or consisting of which the location(s) or area(s) are situated; c) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dash (-), with the balance being a postcode in the country concerned; d) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dot (.), with the balance being an ISO 3166-2 country subdivision code in the country concerned; e) five upper case alphabetic characters corresponding to the UN/LOCODE of the area in, close to, or consisting of which, the location(s) or area(s) are situated; f) the concatenation, being not less than seven or more than 35 characters in length, of: - an ISO/IEC 15459 issuing agency code; - a location code, consisting of characters drawn from the set {A-Z; 0-9} which accords with specifications of the issuing agency concerned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                12027, new EntityDescriptor(
                    "EVENT LOCATION",

                    // ReSharper disable once StringLiteralTypo
                    "Event Location UN/LOCODE. UN/LOCODE followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3227  'Location function code qualifier', e.g., 7 Place of Final Delivery; 5 Port of Departure; 9 Port of Lading; 11 Port of Unlading; 13 Place of transhipment; 24 Port of Entry; 35 Exportation country; 88 Place of Carrier Receipt; 125 Foreign Port prior to Depart to U.S; 147 Stowage cell/position; 159 Place of delivery (to consignee); 248 Loading Location http://www.unece.org/cefact/locode/.",
                    $@"^{GetAlphaRegEx(4, 4)}\+{GetNumericRegEx(1, 3)}$")
            },
            {
                12028,
                new EntityDescriptor(
                    "NUMBER AND STREET",
                    "Number and Street Address. Used in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                12029,
                new EntityDescriptor(
                    "CITY",
                    "City Name. Used in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                12030,
                new EntityDescriptor(
                    "COUNTRY SUB ENTITY",
                    "Country Sub-entity Details. Used in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
                    $@"^{GetAlphanumericRegEx(1, 9)}$")
            },
            {
                12031,
                new EntityDescriptor(
                    "POSTAL CODE",
                    "Postal Code. Used in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L (If a '-' dash is used, it shall be expressly encoded).",
                    $@"^{GetAlphanumericRegEx(4, 11)}$")
            },
            {
                12032,
                new EntityDescriptor(
                    "COUNTRY",
                    "Country Code. ISO 3166-1 Alpha 2 Code  Used in conjunction with H, 6H, 28L, 29L, 30L, 31L, 32L.",
                    $@"^{GetAlphaRegEx(2, 2)}$")
            },
            {
                12033,
                new EntityDescriptor(
                    "URL",
                    "Uniform Resource Locator (URL). Includes all characters that form a URL, including header data such as e.g., http://. Character set as listed in RFC 1738.",
                    $@"^{Rfc1738UrlPattern()}$")
            },
            {
                12034, new EntityDescriptor(
                    "P2P URL",

                    // ReSharper disable once StringLiteralTypo
                    "Pointer to Process URL (P2P URL) for initiating a URL to carry all other data elements encoded in an AIDC media according to the following rule: Scan the code and initiate the URL starting with the P2P URL string, omitting DI 34L and ISO/IEC 15434 envelope syntax (prefix and postfix) and append all other data elements that have been scanned in same sequence as encoded in the media, including DIs and data element separators. Convert special characters in the appended data into RFC 1738 format (e.g., Group Separator 'GS' translated into RFC 1738 sequence %1D). Note that this does not apply to the P2P URL itself. Example: Encoded data string (using ISO/IEC 15434) [)>RS06GS25SUN123456789PA12345GS4LUSGS16D20131108 G S34LHTTP://WWW.SECUREUID.COM/ITEMDATA/?SCAN= R S05 GS13131108RSEOT results in the following URL with the transmitted data: HTTP://WWW.SECUREUID.COM/ITEMDATA/?SCAN=25SU N123456789PA12345%1D4LUS%1D16D20131108. Note: data from the '05' format envelope was not incorporated in the URL since the 34L was encoded in the '06' format envelope.",
                    $@"^{Rfc1738UrlPattern()}$")
            },
            {
                12035, new EntityDescriptor(
                    "SITE APPROVAL",

                    // ReSharper disable once StringLiteralTypo
                    "A government-assigned approval number of vessel / aquaculture site / farm / processor, starting with an ISO 31661 alpha-2 country code, followed by the approval number. All characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed. Example: 35LIECK0107EC = Country; Ireland. Vessel Name; FV Endurance DA31.",
                    $@"^{GetAlphaRegEx(2, 2)}{GetCharacterSet82RegEx(3, 27)}$")
            },
            {
                12036, new EntityDescriptor(
                    "PRODUCER APPROVAL",

                    // ReSharper disable once StringLiteralTypo
                    "A government-assigned approval number of producer or farm or first deboning / cutting hall, starting with an ISO 3166-1 alpha-2 country code, followed by the approval number. All characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed. Example: 36LIECK0107EC = Country; Ireland. Vessel Name; FV Endurance DA31.",
                    $@"^{GetAlphaRegEx(2, 2)}{GetAlphanumericRegEx(3, 27)}$")
            },
            {
                12051,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' - Location code defined by a postal authority (e.g., 5-digit and 9-digit ZIP codes identifying U.S. locations or 6-character postal codes identifying Canadian locations).",
                    $@"^{GetAlphanumericRegEx(1, 9)}$")
            },
            {
                12052,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' - Location code defined by a postal authority (e.g., 5-digit and 9-digit ZIP codes identifying U.S. locations or 6character postal codes identifying Canadian locations).",
                    $@"^{GetAlphanumericRegEx(1, 9)}$")
            },
            {
                12054,
                new EntityDescriptor(
                    "SHIP FROM",
                    "'Ship From:' - Location code defined by a postal authority in the format:  postal codes (e.g., 5-digit ZIP codes identifying U.S. locations or 6- or 7-character postal codes identifying United Kingdom locations) followed by two character ISO 3166 country code  (e.g., US or GB).",
                    $@"^{GetAlphanumericRegEx(1, 9)}$")
            },
            {
                12055,
                new EntityDescriptor(
                    "SHIP TO",
                    "'Ship To:' - Location code defined by a postal authority in the format:  postal codes (e.g., 5-digit ZIP codes identifying U.S. locations or 6- or 7-character postal codes identifying United Kingdom locations) followed by two character ISO 3166 country code  (e.g., US or GB).",
                    $@"^{GetAlphanumericRegEx(1, 9)}$")
            },
            {
                13010,
                new EntityDescriptor(
                    "FORM 2410",
                    "Army Form 2410 data. Format is data value preceded by the block number of the form 2410. Field lengths and acceptable characters can be found at; http://www.apd.army.mil/pdffiles/p738_751.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                13011,
                new EntityDescriptor(
                    "FORM 2408",
                    "Army Form 2408 data. Format is data value preceded by the block number of the form 2408. Field lengths and acceptable characters can be found at; http://www.apd.army.mil/pdffiles/p738_751.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                13012,
                new EntityDescriptor(
                    "FORM 2407",
                    "Army Form 2407 data. Format is data value preceded by the block number of the form 2407. Field lengths and acceptable characters can be found at; http://www.apd.army.mil/pdffiles/p738_751.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                13013,
                new EntityDescriptor(
                    "FORM 95",
                    "Air Force Form 95 data. Format is data value preceded by the block number of the form 95. Field lengths and acceptable characters can be found at; http://www.gsa.gov/portal/forms/download/116418.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                13014,
                new EntityDescriptor(
                    "FORM 4790",
                    "Navy Form 4790 data. Format is data value preceded by the block number of the form 2410. Field lengths and acceptable characters can be found at; http://www.navair.navy.mil/logistics/4790/library/Chapter%201 5.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14000,
                new EntityDescriptor(
                    "NSN",
                    "National/NATO Stock Number (NSN).",
                    $@"^{GetAlphanumericRegEx(13, 15)}$")
            },
            {
                14001, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CIDX PRODUCT CHARACTERISTIC",

                    // ReSharper disable once StringLiteralTypo
                    "Product Characteristic Data defined by the Chemical Industry Data Exchange (CIDX).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14003, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "EIAJ ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "Coding Structure in Accordance with Format Defined by Electronic Industries Association Japan (EIAJ).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14004,
                new EntityDescriptor(
                    "GS1 ENCODED",
                    "Coding Structure and Formats in Accordance with GS1 Application Identifiers (AI plus data) (GS1).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14005, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "AIAG ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "Coding Structure and Formats in Accordance with AIAG Recommendations. The full Data Identifier is in the form 5Nxx where the 'xx' is found in the full code list that can be found at http://www.mhi.org/standards - see under 'MH10 Data Identifiers (Continuous Maintenance Version)'.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14006, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MILSTRIP ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "U.S. DOD Requisition and Issue Procedure Codes. The format is the appropriate MILSTRIP code followed by the data value associated with that code. (The full list of codes is available at;  http://www2.dla.mil/j6/dlmso/elibrary/Manuals/DLM/MILSTRIP/MILSTRIP.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14007,
                new EntityDescriptor(
                    "DTR ENCODED",
                    "U.S. Defense Transportation Regulation Codes. The format is the DTR code followed by the appropriate data value associated with that code. (The full list of codes is available at;  http://www.transcom.mil/dtr/part-ii/dtr_part_ii_toc.pdf.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14008,
                new EntityDescriptor(
                    "PRODUCTION ANIMAL IDENTIFICATION",
                    "Production Animal Identification Codes. The format is the production animal code followed by the appropriate data value associated with that code. The Technical Report and the full list of Extended Data Elements (codes) is maintained at; http://www.aimglobal.org/store/view_product.asp?id=4926441  Extended Data Elements (Codes). http://www.aimglobal.org/store/view_product.asp?id=4926483 Technical Report.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
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
                    "Pharmacy Product Number maintained by IFA (www.ifaffm.de) with the following elements; Data Identifier (DI), two-digit product registration agency code (PRAC), the product reference (PR), and the two PPN check digits (CC), in the form DI PRAC PR CC.",
                    $@"^{GetAlphanumericRegEx(5, 22)}$")
            },
            {
                14010,
                new EntityDescriptor(
                    "IAC CIN ENCODED",
                    "Data in the format and using semantics defined by the holder of a Company Identification Number (CIN) that has been issued by an Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, defined as a sequence of concatenated data elements:  IAC, followed by CIN, followed by the separator character ':' (colon) followed by the data in the format and using semantics as defined by the CIN holder. NOTE: Only the data syntax rules (if any) as provided by the declared IAC+CIN within each DI '10N' data stream shall be applied to the data following DI 10N+IAC+CIN. NOTE: Due to an error in the assignment of DI '10N' (there is no central authority for data-definition nor maintenance), no new uses of DI '10N' should be implemented. The function of DI '10N' is established in Category 18, MISCELLANEOUS with DI '5R'. It is strongly recommended that existing applications that use DI '10N' migrate to DI 5R'.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14011, new EntityDescriptor(
                    "RLA ENCODED",

                    // ReSharper disable once StringLiteralTypo
                    "The Data construct is defined and controlled by the RLA, and is comprised of 2 segments: the field identifier code, immediately followed by the data as defined for that element according to the data dictionary of the RLA. It is essentially a catalog of fields with standardized content. The Field Identifiers are posted at http://rla.org/11ncodes. The use and structure of these codes are defined at:  http://rla.org/11nformat   Additional examples can be found at that site as well. DI '11N' shall never be encoded in a 2D or RFID tag together with any other DI elements. NOTE: Due to an error in the assignment of DI '11N' (the language which states: 'DI '11N' shall never be encoded in a 2D or RFID tag together with any other DI elements.' is not a valid statement), no new uses of DI '11N' should be implemented. The function of DI '11N' is established in DI '12N'. It is strongly recommended that existing applications that use DI '11N' migrate to DI '12N'.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14012,
                new EntityDescriptor(
                    "RLA ENCODED",
                    "The Data construct is defined and controlled by the RLA, comprised of 2 segments: the field identifier (FI) code, immediately followed by the data as defined for that element according to the data dictionary of the RLA. It is essentially a catalog of fields with standardized content. The Field Identifiers are posted at http://rla.org/12ncodes  The use and structure of these codes are defined at:  http://rla.org/12nformat. Examples can be found at that site.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                14015,
                new EntityDescriptor(
                    "CAICT INDUSTRIAL INTERNET ID",
                    "Representing Industrial Internet Identifier Codes controlled and maintained by CAICT, used in the Industrial Internet Identifier Resolution System of China and constructed as <DI><IAC><TTC><STC><CIN><SN>, in the form an3+a3+n3+n3+n8+an1…33, where an3 is the Data Identifier (DI), a3 is the Issuing Agency Code (IAC = “VAA”), n3 is the Top-Tier Code (TTC), n3 is the Secondary-Tier Code (STC), n8 is the Company Identification Number (CIN) controlled and assigned by the Secondary-Tier platform and an1…33 is the Serial Number (SN) that is controlled and assigned by the holder of the CIN, and is unique within that CIN holders’ domain, using the characters 0 through 9, upper- and lower\u0002case A through Z, * (asterisk), + (plus sign), - (dash), . (period or full stop), / (forward slash), ( (left parenthesis), ) (right parenthesis), ! (exclamation mark).",
                    $@"^{GetAlphaRegEx(3, 3)}{GetNumericRegEx(14, 14)}{GetAlphanumericRegEx(1, 33)}$")
            },
            {
                16000,
                new EntityDescriptor(
                    "CUSTOMER ITEM ID",
                    "Item Identification Code assigned by Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16001,
                new EntityDescriptor(
                    "SUPPLIER ITEM ID",
                    "Item Identification Code assigned by Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16002,
                new EntityDescriptor(
                    "REVISION LEVEL",
                    "Code Assigned to Specify the Revision Level for an Item (e.g., engineering change level, edition, or revision).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16003,
                new EntityDescriptor(
                    "GS1 MFR/ITEM CODE",
                    "Combined Manufacturer Identification Code/Item Code Under the 12/13-digit GS1 Formats, plus supplemental codes, if any.",
                    $@"^{GetNumericRegEx(13, 14)}$")
            },
            {
                16004,
                new EntityDescriptor(
                    "GS1 ITEM CODE PORTION",
                    "Item Code Portion of GS1 Formats.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16005,
                new EntityDescriptor(
                    "FREIGHT CLASSIFICATION ITEM",
                    "Freight Classification Item Number Assigned by Carrier for Purposes of Rating Hazardous Materials (e.g., Motor Freight, Air, Boat, Rail Classification).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16006,
                new EntityDescriptor(
                    "SUPPLIER/ITEM CODE",
                    "Combined Supplier Identification and Item Code (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CLEI",

                    // ReSharper disable once StringLiteralTypo
                    "Common Language Equipment Identification (CLEI) assigned by the manufacturer to some telecommunications equipment.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16008, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "GS1 GTIN-14",

                    // ReSharper disable once StringLiteralTypo
                    "14-digit GS1 format for GTIN-14 code structure.",
                    $@"^{GetNumericRegEx(14, 14)}$")
            },
            {
                16009,
                new EntityDescriptor(
                    "DUNS/ITEM CODE",
                    "Combined Manufacturer Identification Code (9-digit DUNS number assigned by Dun & Bradstreet) and the Item Code/Part Number (assigned by the manufacturer).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16010,
                new EntityDescriptor(
                    "HAZARDOUS MATERIAL CODE",
                    "Hazardous Material Code as defined by ANSI X12.3 in the format Data Element 208 (1-character code qualifier) followed by Data Element 209 (Hazardous Material Code).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16011, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CLEI",

                    // ReSharper disable once StringLiteralTypo
                    "10-character CLEI Code for telecommunications equipment.",
                    $@"^{GetAlphanumericRegEx(10, 10)}$")
            },
            {
                16012,
                new EntityDescriptor(
                    "DOCUMENT TYPE",
                    "Document Type (e.g., Pick List, Design Drawing, etc.) (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16013, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System Code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16014, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM & ASSEMBLY",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System and Assembly Code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16015, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM, ASSEMBLY & PART",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System, Assembly, & Part Code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SYSTEM, ASSEMBLY & PART",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS System, Assembly, or Part Code. (User Modified).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16017,
                new EntityDescriptor(
                    "GS1 SUPPLIER ID & ITEM CODE",
                    "Combined GS1 Supplier Identification and Item Code Assigned By The Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16018, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SUPPLIER ID & PART NO.",

                    // ReSharper disable once StringLiteralTypo
                    "Combined VMRS supplier ID and Supplier Assigned Part Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16019,
                new EntityDescriptor(
                    "COMPONENT",
                    "Component of an Item. (One product contained in multiple packages).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16020,
                new EntityDescriptor(
                    "CUSTOMER FIRST LEVEL",
                    "First Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16021,
                new EntityDescriptor(
                    "CUSTOMER SECOND LEVEL",
                    "Second Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16022,
                new EntityDescriptor(
                    "CUSTOMER THIRD LEVEL",
                    "Third Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16023,
                new EntityDescriptor(
                    "CUSTOMER FOURTH LEVEL",
                    "Fourth Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16024,
                new EntityDescriptor(
                    "CUSTOMER FIFTH LEVEL",
                    "Fifth Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a Party to a Transaction Assigned by a Holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assigned part number that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16026,
                new EntityDescriptor(
                    "PART NUMBER",
                    "Part Number of Next Higher Assembly.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16027, new EntityDescriptor(
                    "HTS-6 CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Commodity HTS-6 Code; Using the format:  4012.11 or 4012.11.4000  (Decimal point is expressly encoded) The Harmonized System (HS) Classification is a 6-digit standardized numerical method of classifying traded products. HS numbers are used by customs authorities around the world to identify products for the application of duties and taxes. Additional digits are added to the HS number by some governments to further distinguish products in certain categories. In the United States, numbers used to classify exported products are called 'Schedule B' numbers. The U.S. Census Bureau administers the Schedule B system. Schedule B numbers, not HS numbers, must be provided on the Shippers' Export Declaration (SED). http://www.niccomp.com/rohs/files/NIC_HTS1006.pdf    Import codes are administered by the U.S. International Trade Commission (USITC). http://hts.usitc.gov/.",
                    $@"^{GetNumericRegEx(7, 12)}$")
            },
            {
                16028,
                new EntityDescriptor(
                    "CARGO NAME",
                    "Cargo Name. Plain language description (English).",
                    $@"^{GetAlphanumericRegEx(1, 100)}$")
            },
            {
                16029, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "GMDN PRODUCT CLASSIFICATION",

                    // ReSharper disable once StringLiteralTypo
                    "Product Classification Code as defined with the GMDN (Global Medical Device Nomenclature - http://www.gmdnagency.org).",
                    $@"^{GetNumericRegEx(5, 5)}$")
            },
            {
                16030,
                new EntityDescriptor(
                    "SUPPLIER FIRST LEVEL",
                    "First Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16031,
                new EntityDescriptor(
                    "SUPPLIER SECOND LEVEL",
                    "Second Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16032,
                new EntityDescriptor(
                    "SUPPLIER THIRD LEVEL",
                    "Third Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16033,
                new EntityDescriptor(
                    "SUPPLIER FOURTH LEVEL",
                    "Fourth Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16034,
                new EntityDescriptor(
                    "SUPPLIER FIFTH LEVEL",
                    "Fifth Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16040, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "MSDS CODE",
                    "A Code Assigned by a Customer to the Identification Number of the Manufacturer's Material Safety Data Sheet (MSDS) document that describes the uses, hazards, and chemical composition of a hazardous material.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16049, new EntityDescriptor(
                    "EXPORT CONTROLLED BY",

                    // ReSharper disable once StringLiteralTypo
                    "Export Controlled Item. Subject to export control and or restrictions as identified in the Wassenaar Arrangement. DI followed by the Alpha-2 ISO 3166 Country Code of the country that imposed the restriction followed by Wassenaar Code (http://www.wassenaar.org/controllists/index.html).",
                    $@"^{GetAlphanumericRegEx(3, 9)}$")
            },
            {
                16050, new EntityDescriptor(
                    "MANUFACTURER ITEM ID",

                    // ReSharper disable once StringLiteralTypo
                    "Manufacturer-Assigned Item Identifier - comprising an item number assigned by the item manufacturer, followed by a plus (+) sign, followed - if required to uniquely identify the item within the manufacturer's product range - by a manufacturerassigned item version. Example 50PABC+6 would represent item number ABC, item version 6 Note: The item number shall always be followed by a plus sign, even if no item version is present. This is required to permit the unambiguous concatenation of manufacturerassigned item identifier with another data construct using the concatenation character plus (+). For example, the combination of a 50P manufacturer-assigned item identifier with no item version and a serial number (Data identifier S) on an entity might be encoded as 50PDEF++S1234.",
                    $@"^{GetAlphanumericRegEx(3, 35)}$")
            },
            {
                16051,
                new EntityDescriptor(
                    "ITEM ID",
                    "Globally Unique Item Identifier comprising the Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, followed by a plus (+) sign, followed by the Manufacturer-assigned item identifier as defined with 50P Example: 51PJ4LBE0431863103+ABC+ would represent the item with item number ABC and no version number manufactured by the company with Belgian VAT number 0431863103.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                16052,
                new EntityDescriptor(
                    "COLOR",
                    "Color Code. Color of an item/object identified by a code or term mutually agreed upon between trading partners.",
                    $@"^{GetAlphanumericRegEx(1, 50)}$")
            },
            {
                16053,
                new EntityDescriptor(
                    "SPECIFIC MARINE EQUIPMENT",
                    "Identifier for Specific Marine Equipment approved under the European Union Directive on Marine Equipment (2014/90/EU) and Implementing Regulation (EU) 2018/608.",
                    $@"^{GetAlphaRegEx(1, 1)}{GetNumericRegEx(4, 4)}{GetAlphanumericRegEx(5, 20)}$")
            },
            {
                16054,
                new EntityDescriptor(
                    "UDI DI",
                    "UDI-DI (Unique Device Identification - Device Identifier) for Medical Devices (MD) and In-vitro-Diagnostics (IvD) as the unique key to public UDI data bases (GUDID, EUDAMED, etc.), according to national regulatory requirements, as outlined by the International Medical Device Regulators Forum (IMDRF). All printable characters of the UTF-8 character set are allowed.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                16055,
                new EntityDescriptor(
                    "DNV CERT REF",
                    "DNV certification reference. Indicates that the data contains a reference to a product certificate/verification statement/report, issued by DNV. Data identifier shall be followed by letters “NV” and certificate number. When certificate Number has postfix, it should be included in the datastream by using the “-“ separator character. Revision indicators shall not be provided.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17000,
                new EntityDescriptor(
                    "QUANTITY",
                    "Quantity, Number of Pieces, or Amount (numeric only) (unit of measure and significance mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17001,
                new EntityDescriptor(
                    "THEORETICAL LENGTH/WEIGHT",
                    "Theoretical Length/Weight (numeric only).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17002,
                new EntityDescriptor(
                    "ACTUAL WEIGHT",
                    "Actual Weight (numeric only).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17003,
                new EntityDescriptor(
                    "UNIT OF MEASURE",
                    "Unit of Measure, as defined by the two character ANSI X12.3 Data Element Number 355 Unit of Measurement Code.",
                    $@"^{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                17004, new EntityDescriptor(
                    "GROSS AMOUNT", 
                    "Gross Amount.", 
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17005, new EntityDescriptor(
                    "NET AMOUNT", 
                    "Net Amount.", 
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17006,
                new EntityDescriptor(
                    "CONTAINERS",
                    "Where Multiple Containers Comprise a Single Product (the contents of each container must be combined with the content of the other containers to constitute a single product) the Data Identifier '6Q' shall be used to link the various containers. The format # of # ('this is the nth piece of x pieces to define the product') Presented in the format 'n/x', where the '/' (slash) is used as a delimiter between two values.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17007,
                new EntityDescriptor(
                    "QUANTITY/UOM",
                    "Quantity, Amount, or Number of Pieces in the format: Quantity followed by the two character ANSI X12.3 Data Element Number 355 Unit of Measurement Code.",
                    $@"^{GetNumericRegEx(1, 8)}{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                17008,
                new EntityDescriptor(
                    "CONTAINER RATED WEIGHT",
                    "Manufacturer-assigned weight carrying capability of the container. Assigned at time of manufacture. Unit of measure is kg.",
                    $@"^{GetNumericRegEx(4, 6)}$")
            },
            {
                17009,
                new EntityDescriptor(
                    "PIECE WEIGHT",
                    "Weight of a single item.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17011,
                new EntityDescriptor(
                    "TARE WEIGHT",
                    "Weight of an empty container. Container body weight. Manufacturer-assigned weight of the empty container. Assigned at time of manufacture. Unit of measure is kg (Tare weight).",
                    $@"^{GetNumericRegEx(4, 6)}$")
            },
            {
                17012,
                new EntityDescriptor(
                    "MONETARY VALUE",
                    "Monetary Value established by the Supplier in the format of: the value followed by an ISO 4217 data element code for representing unit of value of currencies and funds (e.g., 12Q2.50USD) (2.50 Monetary Value in USA Dollars). Significance mutually defined. Entry Value: Value followed by an ISO 4217 data element code for representing unit of value of currencies and funds (e.g., 12Q2.50USD) (2.50 Monetary Value in USA Dollars).",
                    $@"^{GetNumericRegEx(1, 10)}{GetAlphanumericRegEx(3, 3)}$")
            },
            {
                17013,
                new EntityDescriptor(
                    "PIECE OF PIECES",
                    "# of # ('this is the nth piece of x pieces in this shipment'). Presented in the format 'n/x', where the '/' (slash) is used as a delimiter between two values.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17014,
                new EntityDescriptor(
                    "BEGINNING SECONDARY QUANTITY",
                    "Beginning Secondary Quantity.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17015,
                new EntityDescriptor(
                    "ENDING SECONDARY QUANTITY",
                    "Ending Secondary Quantity.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17016,
                new EntityDescriptor(
                    "NUMBER OF PIECES IN VAN",
                    "Number Of Pieces in Van.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17017,
                new EntityDescriptor(
                    "NUMBER OF SHIPMENTS IN VAN",
                    "Number Of Shipments in Van.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17018,
                new EntityDescriptor(
                    "CUBE",
                    "Cube expressed in cubic metres or cubic feet followed by the ANSI X12.3 data element number 355 unit of measure code (CR of CF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17019,
                new EntityDescriptor(
                    "WIDTH",
                    "Width expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17020,
                new EntityDescriptor(
                    "HEIGHT",
                    "Height expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17021,
                new EntityDescriptor(
                    "LENGTH",
                    "Length expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure code (LC or LF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17022,
                new EntityDescriptor(
                    "NET WEIGHT",
                    "Net Weight Of Shipment expressed in pounds or kilograms (kilos) followed by the ANSI X12.3 data element number 355 unit of measure (LB or KG). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17023,
                new EntityDescriptor(
                    "VAN LENGTH",
                    "Van Length expressed in linear metres or linear feet followed by the ANSI X12.3 data element number 355 unit of measure (LC or LF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17024,
                new EntityDescriptor(
                    "INSIDE CUBE OF A VAN",
                    "Inside Cube of a Van expressed in cubic metres or cubic feet followed by the ANSI X12.3 data element number 355 of unit measure code (CR or CF). No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17025,
                new EntityDescriptor(
                    "NET EXPLOSIVE WEIGHT",
                    "Net Explosive Weight (a computed value of explosive equivalent expressed in pound of TNT). The measure of NEW is used internationally for explosive safety quantity distance arc computations. No implied decimal point.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17026, new EntityDescriptor(
                    "PACKAGING LEVEL",

                    // ReSharper disable once StringLiteralTypo
                    "Packaging Level, specifying the hierarchical level of packaging in accordance with HIBC (Health Industry Bar Code) specifications.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                17027,
                new EntityDescriptor(
                    "SINGLE PRODUCT NET PRICE",
                    "Single Product Price Value, Net, '.' (dot) used as decimal point (e.g. 27Q1000.5 for the price value of 1000.50). Structure:  an3+an1...20   <DI><price value>. Character set:  0 to 9, dot (ISO 646 ASCII value decimal 46, hexadecimal 2E). Example of encoding using a net price value of 1000: 27Q1000. Example of encoding using a net price value of 1000.50: 27Q1000.5. NOTE: If currency is required it can be taken from another data element used in same code, e.g. 12Q.",
                    $@"^{GetNumericWithDotRegEx(1, 20)}$")
            },
            {
                17028,
                new EntityDescriptor(
                    "SINGLE PRICE CHARGE VALUE FOR POSTAGE AND PACKAGING",
                    "Single Price Charge Value For Postage And Packaging, '.' (dot) represents the position of a comma (e.g. 30Q100.50 for the value of 100,50). Structure:  an3+an1...10   <DI><price value>. Character set:  0 to 9, dot. Example of encoding using postage & packing value of 100: 30Q100. Example of encoding using postage & packing value of 100,50: 30Q100.50. NOTE: If currency is required it can be taken from another data element used in same code, e.g. 12Q.",
                    $@"^{GetNumericWithDotRegEx(1, 10)}$")
            },
            {
                17029,
                new EntityDescriptor(
                    "DISCOUNT PERCENTAGE",
                    "Discount Percentage, '.' (dot) represents the position of a comma (e.g. 31Q8.5 for a discount value of 8,5%). Structure:  an3+n1...6 (12.456)   <DI><discount percentage (%)>. Character set:  0 to 9, dot Example of encoding using discount percentage of 10%: 31Q10. Example of encoding using discount percentage of 8,5%: 31Q8.5.",
                    $@"^{GetNumericWithDotRegEx(1, 6)}$")
            },
            {
                17030,
                new EntityDescriptor(
                    "VAT PERCENTAGE",
                    "VAT Percentage, '.' (dot) represents the position of a comma (e.g. 27Q8.5 for the VAT value of 8.5%). Structure:  an3+an1...5 (12.45)    <DI><VAT percentage (%)>. Character set:  0 to 9, dot. Example of encoding using VAT percentage of 19%: 27Q19. Example of encoding using VAT percentage of 8,5%: 27Q8.5.",
                    $@"^{GetNumericWithDotRegEx(1, 5)}$")
            },
            {
                17031, new EntityDescriptor(
                    "CURRENCY",

                    // ReSharper disable once StringLiteralTypo
                    "Currency, ISO 4217 currency code. Structure:  an3+an3  <DI><Currency, e.g. EUR>. Character set:  A-Z, 0 to 9. Example of encoding using ISO alphabetic code of US Dollar:   31QUSD. Example of encoding using ISO alphabetic code of EURO: 31QEUR. Example of encoding using ISO numeric code of EURO: 31Q978.",
                    $@"^({GetAlphaRegEx(3, 3)}|{GetNumericRegEx(3, 3)})$")
            },
            {
                17032, new EntityDescriptor(
                    "LOINC CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Clinical term code as defined with the clinical nomenclature: “The international standard for identifying health measurements, observations, and documents – LOINC” (https://loinc.org), in the following sequence: <DI><LOINC Code><Plus Sign><Value>. The unit and format of the Value is defined by the LOINC Code.",
                    $@"^{GetAlphanumericRegEx(3, 35)}$")
            },
            {
                18001,
                new EntityDescriptor(
                    "RMA",
                    "Return Authorization Code (RMA) assigned by the Supplier",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                18002,
                new EntityDescriptor(
                    "RETURN CODE",
                    "Return Code Assigned by the Customer",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                18004, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "DODIC",
                    "U.S. Department of Defense Identification Code (DoDIC)",
                    $@"^{GetAlphanumericRegEx(4, 4)}$")
            },
            {
                18005,
                new EntityDescriptor(
                    "IAC CIN DATE",
                    "Data in the format and using semantics defined by the holder of a Company Identification Number (CIN) that has been issued by an Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, defined as a sequence of concatenated data elements:  IAC, followed by CIN, followed by the separator character ':' (colon) followed by the data in the format and using semantics as defined by the CIN holder. NOTE: Only the data syntax rules (if any) as provided by the declared IAC+CIN within each DI '5R' data stream shall be applied to the data following DI 5R+IAC+CIN.4.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                18006, new EntityDescriptor(
                    "SIGNATURE",

                    // ReSharper disable once StringLiteralTypo
                    "ISO/IEC 20248 digital signature data construct. If the underlying data carrier encoding is 7 bits, then only the ISO/IEC 20248 raw format may be used. Example with an URL format:  <6R><https://20248.sigvr.it/?Oo586eJAMEYCIQCf31EqIJML GclBpHLlRgBdO>  Example with a raw format:  <6R><Oo586eJAMEYCIQCf31EqIJMLGclBpHLlRgBdO>  An ISO/IEC 20248 data structure contains a digital signature which is used to verify the specified data elements of the message of data elements. The value of 6R, as the first parameter, and the data elements to be verified (stripped from all non-printable characters), as the second parameter, is passed to the ISO/IEC 20248 DecoderVerifier - which will return the verification result: ACCEPT, REJECT or ERROR(error code), and the JSON object of decoded ISO/IEC 20248 additional fields. The ISO/IEC 20248 data structure may contain additional fields and instructions to decode and verify one or more messages of data elements. These instructions will be processed by the ISO/IEC 20248 DecoderVerifier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                18007, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "ASFIS CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Aquatic Sciences and Fisheries Information System (ASFIS) 'Inter-agency 3-alpha species code', maintained by the Food and Agriculture Organisation of the United Nations (www.fao.org, then search for 'ASFIS'). Examples; 7RMUC = Mud carp; 7RPCD = Australian freshwater herring; 7RWSH = Great white shark.",
                    $@"^{GetAlphanumericRegEx(1, 3)}$")
            },
            {
                18008, new EntityDescriptor(
                    "FAO CODE",

                    // ReSharper disable once StringLiteralTypo
                    "Food and Agricultural Organisation (FAO) International Standard Classification of Fishing Gears (ISSCFG) code. (www.fao.org)  All characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed. Examples: 8R02.1.0 = Beach seines; 8R03.1.5 = Shrimp trawls; 8R05.1.0 = Portable lift net.",
                    $@"^{GetAlphanumericRegEx(1, 10)}$")
            },
            {
                18009,
                new EntityDescriptor(
                    "FAO PRODUCTION METHOD",
                    "Production method for fish and seafood as specified by the Fisheries and Aquaculture Department of the Food and Agricultural Organisation (FAO) of the United Nations, according to EU Regulation 1379/2013. (www.fao.org). All characters of the GS1 General Specification-defined subset of ISO/IEC 646 are allowed. Examples; 9R01 = Caught at sea; 9R02 = Caught in fresh water; 9R03 = Farmed.",
                    $@"^{GetAlphanumericRegEx(2, 2)}$")
            },
            {
                19000,
                new EntityDescriptor(
                    "SERIAL NUMBER",
                    "Serial Number or Code Assigned by the Supplier to an Entity for its Lifetime, (e.g., computer serial number, traceability number, contract tool identification)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19001,
                new EntityDescriptor(
                    "ADDITIONAL CODE",
                    "Additional Code Assigned by the Supplier to an Entity for its Lifetime (e.g., traceability number, computer serial number) ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19002,
                new EntityDescriptor(
                    "ASN SID",
                    "Advance Shipment Notification (ASN) Shipment ID (SID) corresponds to ANSI ASC X12 Data Element 396",
                    $@"^{GetAlphanumericRegEx(2, 30)}$")
            },
            {
                19003,
                new EntityDescriptor(
                    "UNIQUE PACKAGE IDENTIFICATION",
                    "Unique Package Identification Assigned by Supplier (lowest level of packaging which has a package ID code; shall contain like items) ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19004,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION LIKE ITEMS",
                    "Package Identification Assigned by Supplier to master packaging containing like items on a single customer order ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19005,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION UNLIKE ITEMS",
                    "Package Identification Assigned by Supplier to master packaging containing unlike items on a single customer order",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19006,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION LIKE ITEMS MULTIPLE",
                    "Package Identification Assigned by Supplier to master packaging containing like items over multiple customer orders",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19007,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION UNLIKE ITEMS MULTIPLE",
                    "Package Identification Assigned by Supplier to master packaging containing unlike items over multiple customer",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19008, new EntityDescriptor(
                    "SUPPLIER ID",

                    // ReSharper disable once StringLiteralTypo
                    "Supplier ID/Unique Container ID presented in the data format specified by the GS1 SSCC-18",
                    $@"^{GetNumericRegEx(18, 18)}$")
            },
            {
                19009,
                new EntityDescriptor(
                    "PACKAGE IDENTIFICATION",
                    "Package Identification, Generic (mutually defined)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19010,
                new EntityDescriptor(
                    "ID CODE",
                    "Machine, Cell, or Tool ID Code ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19011,
                new EntityDescriptor(
                    "FIXED ASSET ID CODE",
                    "Fixed Asset ID Code",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19012,
                new EntityDescriptor(
                    "DOCUMENT NUMBER",
                    "Document Number (internally assigned or mutually defined)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19013,
                new EntityDescriptor(
                    "CONTAINER SECURITY SEAL",
                    "Container Security Seal",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19014,
                new EntityDescriptor(
                    "4TH CLASS MANIFESTING ",
                    "4th Class Non-identical parcel post manifesting ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19015,
                new EntityDescriptor(
                    "SERIAL NUMBER ASSIGNED BY THE VENDOR ENTITY",
                    "Serial Number Assigned by the Vendor Entity, that can only be used in conjunction with '13V' ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19016,
                new EntityDescriptor(
                    "VERSION NUMBER",
                    "Version Number, e.g., Software Version",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19017,
                new EntityDescriptor(
                    "GS1 SUPPLIER AND UNIQUE PACKAGE IDENTIFICATION",
                    "Combined 6-digit GS1 Supplier Identification and Unique Package Identification Assigned by the Supplier",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19018,
                new EntityDescriptor(
                    "CAGE CODE & SERIAL NUMBER",
                    "CAGE Code & Serial Number Unique Within CAGE ",
                    $@"^{GetAlphanumericCageSnRegEx(6, 25)}$")
            },
            {
                19019,
                new EntityDescriptor(
                    "DUNS AND PACKAGE IDENTIFICATION",
                    "Combined Dun & Bradstreet company identification of the supplier followed by a unique package identification assigned by the supplier, in the format nn...nn+nn...n where a plus symbol (+) is used as a delimiter between the DUNS Number and unique package identification",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19020,
                new EntityDescriptor(
                    "TRACEABILITY CODE",
                    "Traceability Code for an Entity Assigned by the Customer",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19021,
                new EntityDescriptor(
                    "TIRE IDENTIFICATION NUMBER",
                    "Tire Identification Number as defined by the U.S. Department of Transportation (D.O.T) under U.S. Code 49 CFR 574.5.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19022,
                new EntityDescriptor(
                    "UNIQUE INDIVIDUAL IDENTITY",
                    "Unique Individual Identity for Cellular Mobile Telephones",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19023, new EntityDescriptor(
                    "MAC ADDRESS",

                    // ReSharper disable once StringLiteralTypo
                    "Media Access Control (MAC) Address conforming with IEEE 802.11",
                    $@"^{GetAlphanumericRegEx(12, 12)}$")
            },
            {
                19024,
                new EntityDescriptor(
                    "RF TAG UNIQUE IDENTIFIER",
                    "According to ISO/IEC 15963 (value is a conversion of its bit value to 8-bit ASCII values). This Data Identifier could possibly assume any ASCII-256 value. For freight container tags the Registration Authority (RA) for manufacturers is the RA for ISO 14816. (ISO 646)",
                    $@"^{GetNumericRegEx(6, 26)}$")
            },
            {
                19025,
                new EntityDescriptor(
                    "PARTY TO TRANSACTION",
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assign serial number that is unique within the CIN holder's domain  (See Annex C.11)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19026,
                new EntityDescriptor(
                    "READER ID",
                    "Equipment Identifier, being a globally unique identifier for a device, an item of equipment or instance of a computer application used in the production, transport, processing or other handling of items, that is constructed by concatenating: - an ISO/IEC 15459 issuing agency code; - an equipment number which accords with specifications of the issuing agency concerned; and that: - comprises only upper case alphabetic and/or numeric characters; - is unique (that is, is distinct from any other ISO/IEC 15459 compliant identifier) within the domain of the issuing agency; - cannot be from any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, it end. Reader ID Equipment identifier, being a globally unique identifier for a device, an item of equipment or instance of a computer application used in the production, transport, processing or other handling of items",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                19027,
                new EntityDescriptor(
                    "ITEM NUMBER WITHIN BATCH",
                    "Item Number Within Batch, being a string of numeric digits: - that uniquely distinguishes an item, within an identifiable batch of related items, from all other items in the same batch; - whose length is the same for all items within the batch concerned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19028,
                new EntityDescriptor(
                    "BATCH-AND-ITEM NUMBER",
                    "Batch-and-Item Number, being the concatenation of a data identifier 27T batch number and the data identifier 27S item number of an item belonging to the batch concerned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19030,
                new EntityDescriptor(
                    "ADDITIONAL TRACEABILITY CODE",
                    "Additional Traceability Code For An Entity Assigned by the Supplier in addition to or different from the traceability code(s) provided by 'S' or '1S'",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19031,
                new EntityDescriptor(
                    "BEGINNING SERIAL NUMBER",
                    "Beginning Serial Number for serial numbers in sequence",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19032,
                new EntityDescriptor(
                    "ENDING SERIAL NUMBER",
                    "Ending Serial Number for serial numbers in sequence",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19033,
                new EntityDescriptor(
                    "SERIAL NUMBER OF NEXT HIGHER ASSEMBLY",
                    "Serial Number of Next Higher Assembly",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19034,
                new EntityDescriptor(
                    "PART NUMBER OF END ITEM",
                    "Serial Number or Part Number of End Item",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19035,
                new EntityDescriptor(
                    "BUMPER NUMBER",
                    "Bumper Number (Used in Unit DOD Move)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19036,
                new EntityDescriptor(
                    "PALLET IDENTIFIER",
                    "Pallet Identifier (Used for loaded 463L air pallets) ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                19037,
                new EntityDescriptor(
                    "IAC CIN PN + PSN",
                    "Unique Item Identifier comprised of a sequence of 5 data elements: 'IAC', followed by 'CIN', followed by 'Part Number (PN)', followed by the '+' character, followed by the supplier assigned (or managed) 'Part Serial Number (PSN)' that is globally unique within the CIN holder's domain; in the format IAC CIN PN + PSN (spaces provided for visual clarity only; they are not part of the data). See Annex C.11.",
                    $@"^{GetAlphanumericWithPlusRegEx(1, -1)}$")
            },
            {
                19042,
                new EntityDescriptor(
                    "UII",
                    "Unique Item Identifier (UII) in 25S format preceded by numeric value indicating serial number element length for use by systems that require the 'serial number' component of a concatenated Serial Number element (IAC+CIN+SN)  Format: DI+LI+IAC+CIN+SN (LI=length of SN) ",
                    $@"^{GetNumericRegEx(2, 2)}{GetAlphanumericRegEx(3, 42)}$")
            },
            {
                19043, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "ICCID",

                    // ReSharper disable once StringLiteralTypo
                    "Integrated Circuit Card Identifier (ICCID) in accordance with ITU-T Recommendation E.118 and ETSI Recommendation GSM 11.11; a maximum of 20 digits consisting of Issuer identification number (IIN; maximum of 7 digits), Individual account identification (variable; length determined by IIN, but the same length within individual IINs), Check digit (single digit calculated using Luhn algorithm http://en.wikipedia.org/wiki/Luhn_algorithm). 43Siiiiiiinnnnnnnnnnnnc (i = IIN, n = account identification, c = check digit)",
                    $@"^{GetNumericRegEx(14, 26)}$")
            },
            {
                19050,
                new EntityDescriptor(
                    "FIRST LEVEL",
                    "First Level (Supplier Assigned)",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                19051,
                new EntityDescriptor(
                    "SECOND LEVEL",
                    "Second Level (Supplier Assigned)",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                19052,
                new EntityDescriptor(
                    "THIRD LEVEL",
                    "Third Level (Supplier Assigned)",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                19053,
                new EntityDescriptor(
                    "FOURTH LEVEL",
                    "Fourth Level (Supplier Assigned)",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                19054,
                new EntityDescriptor(
                    "FIFTH LEVEL",
                    "Fifth Level (Supplier Assigned)",
                    $@"^{GetAlphanumericRegEx(1, 20)}$")
            },
            {
                19096, new EntityDescriptor(
                    "EPC NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "EPC number (Typically Serialized Global Trade Identification Number - SGTIN)",
                    $@"^{GetAlphanumericRegEx(16, 26)}$")
            },
            {
                19097,
                new EntityDescriptor(
                    "ENCRYPTED SERIAL NUMBER",
                    "Encrypted serial number assigned by the Supplier to an entity, which can be authenticated by an independent trusted third party. The encrypted serial number does not describe any parameters of the entity without decryption by an independent third party.",
                    $@"^{GetAlphanumericRegEx(4, 25)}$")
            },
            {
                20000,
                new EntityDescriptor(
                    "TRACEABILITY NUMBER ASSIGNED BY THE CUSTOMER",
                    "Traceability Number assigned by the Customer to identify/trace a unique group of entities (e.g., lot, batch, heat).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20001,
                new EntityDescriptor(
                    "TRACEABILITY NUMBER ASSIGNED BY THE SUPPLIER",
                    "Traceability Number assigned by the Supplier to identify/trace a unique group of entities (e.g., lot, batch, heat).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20003,
                new EntityDescriptor(
                    "EXCLUSIVE ASSIGNMENT",
                    "Exclusive Assignment (U.S. EPA vehicle identification for emissions testing).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20020,
                new EntityDescriptor(
                    "FIRST LEVEL (CUSTOMER ASSIGNED)",
                    "First Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20021,
                new EntityDescriptor(
                    "SECOND LEVEL (CUSTOMER ASSIGNED)",
                    "Second Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20022,
                new EntityDescriptor(
                    "THIRD LEVEL (CUSTOMER ASSIGNED)",
                    "Third Level (Customer Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20023,
                new EntityDescriptor(
                    "FOURTH LEVEL (CUSTOMER ASSIGNED",
                    "Fourth Level (Customer Assigned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20024,
                new EntityDescriptor(
                    "FIFTH LEVEL (CUSTOMER ASSIGNED)",
                    "Fifth Level (Customer Assigned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20025,
                new EntityDescriptor(
                    "PARTY TO A TRANSACTION",
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the supplier assigned traceability number that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20026,
                new EntityDescriptor(
                    "BATCH IDENTIFIER",
                    "Batch Identifier comprising the concatenation of either: - a data identifier 26S mail processing equipment identifier, or - a data identifier 20K license identifier, or - a data identifier 18V party identifier that: - is distinct from any other ISO/IEC 15459 compliant identifier within the domain of the issuing agency concerned; - cannot be derived from another party identifier or any other ISO/IEC 15459 compliant identifier, issued under the same issuing agency, by the simple addition of characters to, or their removal from, its end; with a data identifier 27T batch number, the two being separated by a dash (-) character.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20027,
                new EntityDescriptor(
                    "BATCH NUMBER",
                    "Batch Number, issued under the control of an identified party or unit of processing equipment, or under the provisions of an identified license, that: - uniquely distinguishes one batch of related items from all other batches to which a batch number is assigned by the party or equipment, or under the license, concerned; - comprises a string of maximum length 10 characters, of which the first (numeric) character indicates the number of following characters, each of which is taken from the set {0-9; A-Z}.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20030,
                new EntityDescriptor(
                    "FIRST LEVEL (SUPPLIER ASSIGNED)",
                    "First Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20031,
                new EntityDescriptor(
                    "SECOND LEVEL (SUPPLIER ASSIGNED)",
                    "Second Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20032,
                new EntityDescriptor(
                    "THIRD LEVEL (SUPPLIER ASSIGNED) ",
                    "Third Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20033,
                new EntityDescriptor(
                    "FOURTH LEVEL (SUPPLIER ASSIGNED) ",
                    "Fourth Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                20034,
                new EntityDescriptor(
                    "FIFTH LEVEL (SUPPLIER ASSIGNED) ",
                    "Fifth Level (Supplier Assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21005,
                new EntityDescriptor(
                    "POSTAL SERVICE",
                    "Specification of a postal service and associated process data in accordance with UPU standard S25 data construct 'Service Data'",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21006,
                new EntityDescriptor(
                    "LICENSING POST DATA",
                    "Licensing Post Data, in accordance with the specification in UPU standard S25.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21015,
                new EntityDescriptor(
                    "SUPPLEMENTARY POSTAL SERVICE",
                    "Specification of supplementary postal service and associated process data in accordance with UPU standard S25 data construct 'Supplementary Service Data'.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21016,
                new EntityDescriptor(
                    "POSTAL ADMINISTRATION IDENTIFICATIONS",
                    "Postal Administration Identifications, being the identification, expressed in accordance with the specification in UPU standard S25, of one or more postal administrations involved in the processing of a mail item or batch.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21017, new EntityDescriptor(
                    "UPU LOCATION CODE",

                    // ReSharper disable once StringLiteralTypo
                    "UPU Location Code, being a code identifying a location or geographic area, or an associated group of such locations or areas, that has relevance to a related transaction and that complies with one of the structures defined in a) to g) below: a) two upper case alphabetic characters corresponding to the ISO 3166-1 two alpha country code of the country in which, or consisting of which, the location(s) or area(s) are situated; b) three upper case alphabetic characters corresponding to the IATA code of the airport or city in, close to, or consisting of which the location(s) or area(s) are situated; c) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dash (-), with the balance being a postcode in the country concerned; d) four or more characters of which the first three correspond to an ISO 3166-1 country code followed by a dot (.), with the balance being an ISO 3166-2 country subdivision code in the country concerned; e) five upper case alphabetic characters corresponding to the UN/LOCODE of the area in, close to, or consisting of which, the location(s) or area(s) are situated; f) six upper case alphanumeric characters corresponding to a UPU IMPC code allocated in accordance with UPU standard S34; g) the concatenation, being not less than seven nor more than 25 characters in length, of: - an issuer code allocated in accordance with UPU standards S31; - a location code, consisting of characters drawn from the set {A-Z; 0-9} which accords with specifications of the issuer concerned.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21018,
                new EntityDescriptor(
                    "QUALIFIED UPU LOCATION CODE",
                    "Qualified UPU Location Code, concatenation of: - a location category drawn from UPU code list 139; - a data identifier 17U UPU location code.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21019,
                new EntityDescriptor(
                    "LICENSE PLATE WITH SERVICE DATA AND LOCATION CODE",
                    "License Plate with Service Data and Location Code is a compound data construct, compliant with the specification in UPU standard S25, which includes specification of: - an ISO/IEC 15459-compliant item identifier; - a data identifier 5U compliant specification of the service to be provided in respect of the item; - a data identifier 17U compliant UPU location code or a data identifier 18U compliant qualified UPU location code. Note:  For further details, please refer to UPU standard S25. The distinction between a simple UPU location code (DI 17U) and a qualified UPU location code (DI 18U) can be determined from the first character. If this is numeric, 18U applies; if it is alphabetic, 17U applies.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                21055,
                new EntityDescriptor(
                    "OCR DATA LOCATOR",
                    "OCR Data Locator.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22000,
                new EntityDescriptor(
                    "SUPPLIER CODE ASSIGNED BY CUSTOMER",
                    "Supplier Code Assigned by Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22001,
                new EntityDescriptor(
                    "SUPPLIER CODE ASSIGNED BY SUPPLIER",
                    "Supplier Code Assigned by Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22002,
                new EntityDescriptor(
                    "U.P.C. COMPANY PREFIX",
                    "U.P.C. Company Prefix.",
                    $@"^{GetNumericRegEx(9, 9)}$")
            },
            {
                22003,
                new EntityDescriptor("GS1 COMPANY PREFIX", "GS1 Company Prefix.", $@"^{GetNumericRegEx(9, 9)}$")
            },
            {
                22004,
                new EntityDescriptor(
                    "CARRIER IDENTIFICATION CODE",
                    "Carrier Identification Code assigned by an industry standard mutually defined by the Supplier, Carrier, and Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22005,
                new EntityDescriptor(
                    "FINANCIAL INSTITUTION IDENTIFICATION CODE",
                    "Financial Institution Identification Code (mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22006,
                new EntityDescriptor(
                    "MANUFACTURER'S IDENTIFICATION CODE",
                    "Manufacturer's Identification Code (mutually defined.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22007,
                new EntityDescriptor(
                    "LIABLE PARTY",
                    "Code assigned to a party which has financial liability for an entity or group of entities (e.g., owner of inventory) (mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22008,
                new EntityDescriptor(
                    "CUSTOMER CODE ASSIGNED BY THE CUSTOMER",
                    "Customer Code Assigned by the Customer.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22009,
                new EntityDescriptor(
                    "CUSTOMER CODE ASSIGNED BY THE SUPPLIER",
                    "Customer Code Assigned by the Supplier.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22010,
                new EntityDescriptor(
                    "MANUFACTURER ID",
                    "Manufacturer ID  NOTE: See Appendix 2, CBP 7501 Instructions.",
                    $@"^{GetAlphanumericRegEx(10, 15)}$")
            },
            {
                22011,
                new EntityDescriptor(
                    "BUDGET HOLDER",
                    "Organisation with budget responsibility for an entity, process, or procedure (e.g., shop, division, department)(internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22012,
                new EntityDescriptor(
                    "MANUFACTURER DUNS NUMBER",
                    "DUNS Number Identifying Manufacturer.",
                    $@"^{GetNumericRegEx(9, 13)}$")
            },
            {
                22013,
                new EntityDescriptor(
                    "SUPPLIER DUNS NUMBER",
                    "DUNS Number Identifying Supplier.",
                    $@"^{GetNumericRegEx(9, 13)}$")
            },
            {
                22014,
                new EntityDescriptor(
                    "CUSTOMER DUNS NUMBER",
                    "DUNS Number Identifying Customer.",
                    $@"^{GetNumericRegEx(9, 13)}$")
            },
            {
                22015,
                new EntityDescriptor(
                    "CARRIER-ASSIGNED SHIPPER NUMBER",
                    "Carrier-Assigned Shipper Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22016, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "VMRS SUPPLIER ID",

                    // ReSharper disable once StringLiteralTypo
                    "VMRS Supplier ID.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22017,
                new EntityDescriptor("DOD CAGE CODE", "U.S. DoD CAGE Code.", $@"^{GetAlphanumericRegEx(5, 5)}$")
            },
            {
                22018,
                new EntityDescriptor(
                    "PARTY TO A TRANSACTION",
                    "Identification of a party to a transaction in which the data format consists of two concatenated segments. The first segment is the Issuing Agency Code (IAC) in accordance with ISO/IEC 15459, the second segment is a unique entity identification Company Identification Number (CIN) assigned in accordance with rules established by the issuing agency (see http://www.aimglobal.org/?page=Reg_Authority15459&hhSear chTerms=%22IAC%22).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22019, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "PARTYS ROLE(S) IN A TRANSACTION",

                    // ReSharper disable once StringLiteralTypo
                    "Specification of a party's role(s), in a transaction, consisting of one or more code values from EDIFACT Code List 3035 'Party Qualifier', separated by plus (+) characters (Never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22020, new EntityDescriptor(
                    "IAC CIN CODE VALUES IDENTIFICATION",

                    // ReSharper disable once StringLiteralTypo
                    "Identification of a party to a transaction assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by a plus (+) character followed by one or more code values from EDIFACT Code List 3035 'Party Qualifier', separated by plus (+) characters (Never to be concatenated with other DIs in a linear symbol or other media where the concatenation character is a plus (+) character).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                22021,
                new EntityDescriptor(
                    "IAC CIN SUB UNIT IDENTIFICATION",
                    "Identification of a party to a transaction, e.g., 21V IAC CIN OSU, assigned by a holder of a Company Identification Number (CIN) and including the related Issuing Agency Code (IAC) in accordance with ISO/IEC 15459 and its registry, structured as a sequence of 3 concatenated data elements:  IAC, followed by CIN, followed by the organisational sub-unit identification assigned by the CIN that is unique within the CIN holder's domain.",
                    $@"^{GetAlphanumericRegEx(1, 35)}$")
            },
            {
                22022, new EntityDescriptor(

                    // ReSharper disable once StringLiteralTypo
                    "CARRIER SCAC",

                    // ReSharper disable once StringLiteralTypo
                    "Carrier SCAC Standard Carrier Alpha Code - The National Motor Freight Traffic Association, Inc., (NMFTA) assigns SCACs for all companies except those codes used for identification of freight containers not operating exclusively in North America, intermodal chassis and trailers, non-railroad owned rail cars, and railroads. http://www.nmfta.org/Pages/welcome.aspx  Companies seeking identification codes for freight containers not operating in North America should contact the Bureau International des Containers, 38, rue des Blancs Manteaux, F75004 Paris, France, email:  bic@bic-code.org, web www.biccode.org. Railroads and owners of intermodal chassis, trailers and non-railroad owned rail cars should contact Railinc Customer Service, Attn: Private Marks, 7001 Weston Parkway, Suite 200, Cary, NC 27513, (800) 544-7245, email: private.marks@railinc.com.",
                    $@"^{GetAlphanumericRegEx(4, 4)}$")
            },
            {
                22023, new EntityDescriptor(
                    "SUPPLIER VAT NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Government-assigned Value Added Tax identification number identifying supplier, starting with an ISO 3166-1 alpha-2 country code (except for Greece, which uses the ISO 639-1 language code EL), followed by the government-assigned VAT number. Example: 23VIE6388047V  assigned to Google IrelanD.",
                    $@"^{GetAlphaRegEx(2, 2)}{GetAlphanumericRegEx(3, 18)}$")
            },
            {
                22024,
                new EntityDescriptor(
                    "CUSTOMER VAT NUMBER",
                    "Government-assigned Value Added Tax identification number identifying customer, starting with an ISO 3166-1 alpha-2 country code (except for Greece, which uses the ISO 639-1 language code EL), followed by the government-assigned VAT number. Example: 24VIE6388047V  assigned to Google Ireland.",
                    $@"^{GetAlphaRegEx(2, 2)}{GetAlphanumericRegEx(3, 18)}$")
            },
            {
                22025,
                new EntityDescriptor(
                    "NCAGE CAGE MANUFACTURER CODE",
                    "Declaring that the NCAGE/CAGE code that follows DI 25V is the Manufacturer. Party to a transaction wherein the NATO Commercial And Government Entity (NCAGE) / Commercial And Government Entity (CAGE) code used behind DI 25V is declared to be the manufacturer of the item(s) involved in the transaction. Data following DI 25V will consist of five upper\u0002case alphanumeric characters, excluding the letters “I” and “O”.",
                    $@"^{GetAlphanumericRegEx(5, 5)}$")
            },
            {
                23000,
                new EntityDescriptor(
                    "WORK ORDER NUMBER",
                    "Work Order Number (e.g., 'Production Paper') (internally assigned).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23001,
                new EntityDescriptor(
                    "OPERATION SEQUENCE NUMBER",
                    "Operation Sequence Number. A number that defines the order of a particular operation in a series of operations, generally in a manufacturing or assembly process.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23002,
                new EntityDescriptor(
                    "OPERATION CODE",
                    "Operation Code/Work Code - the type of work to be performed (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23003,
                new EntityDescriptor(
                    "WORK ORDER AND OPERATION SEQUENCE NUMBER",
                    "Combined Work Order Number and Operation Sequence Number in the format nn...n+nn...n where a plus symbol (+) is used as a delimiter between the Work Order Number and the Operation Sequence Number.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23004,
                new EntityDescriptor(
                    "STATUS CODE",
                    "Status Code (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23005,
                new EntityDescriptor(
                    "WORK UNIT CODE",
                    "Work Unit Code - identifies system, subsystem, assembly, component etc. on which maintenance is performed.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23006,
                new EntityDescriptor(
                    "NOMENCLATURE",
                    "Nomenclature - (internally assigned or mutually defined).",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23010, new EntityDescriptor(
                    "FORM CONTROL NUMBER",

                    // ReSharper disable once StringLiteralTypo
                    "Form Control Number - Preprinted control number on forms.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23011,
                new EntityDescriptor(
                    "QUALITY ASSURANCE INSPECTOR",
                    "Quality Assurance Inspector - Last Name.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                23012,
                new EntityDescriptor(
                    "TELEPHONE NUMBER",
                    "Telephone Number of the Person/Activity Completing the Form - expressed in the format (country code) city or area code plus local number i.e. (1) 319 555 1212.",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26000,
                new EntityDescriptor(
                    "(CUSTOMER-SUPPLIER)",
                    "Mutually Defined Between Customer and Supplier ",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26001,
                new EntityDescriptor(
                    "(CARRIER-SUPPLIER)",
                    "Mutually Defined Between Carrier and Supplier",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26002,
                new EntityDescriptor(
                    "(CUSTOMER-CARRIER)",
                    "Mutually Defined Between Customer and Carrier",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26003,
                new EntityDescriptor(
                    "FREE TEXT", 
                    "Free Text", 
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26004,
                new EntityDescriptor(
                    "(CARRIER-TRADING PARTNER)",
                    "Mutually Defined Between Carrier and Trading Partner",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26010,
                new EntityDescriptor(
                    "HEADER",
                    "Structured Free Text  (Header Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26011,
                new EntityDescriptor(
                    "LINE 1",
                    "Structured Free Text (Line 1 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26012,
                new EntityDescriptor(
                    "LINE 2",
                    "Structured Free Text (Line 2 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26013,
                new EntityDescriptor(
                    "LINE 3",
                    "Structured Free Text (Line 3 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26014,
                new EntityDescriptor(
                    "LINE 4",
                    "Structured Free Text (Line 4 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26015,
                new EntityDescriptor(
                    "LINE 5",
                    "Structured Free Text (Line 5 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26016,
                new EntityDescriptor(
                    "LINE 6",
                    "Structured Free Text (Line 6 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26017,
                new EntityDescriptor(
                    "LINE 7",
                    "Structured Free Text (Line 7 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26018,
                new EntityDescriptor(
                    "LINE 8",
                    "Structured Free Text (Line 8 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26019,
                new EntityDescriptor(
                    "LINE 9",
                    "Structured Free Text (Line 9 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26020,
                new EntityDescriptor(
                    "LINE 10",
                    "Structured Free Text (Line 10 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26021,
                new EntityDescriptor(
                    "LINE 11",
                    "Structured Free Text (Line 11 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26022,
                new EntityDescriptor(
                    "LINE 12",
                    "Structured Free Text (Line 12 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26023,
                new EntityDescriptor(
                    "LINE 13",
                    "Structured Free Text (Line 13 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26024,
                new EntityDescriptor(
                    "LINE 14",
                    "Structured Free Text (Line 14 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26025,
                new EntityDescriptor(
                    "LINE 15",
                    "Structured Free Text (Line 15 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26026,
                new EntityDescriptor(
                    "LINE 16",
                    "Structured Free Text (Line 16 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26027,
                new EntityDescriptor(
                    "LINE 17",
                    "Structured Free Text (Line 17 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26028,
                new EntityDescriptor(
                    "LINE 18",
                    "Structured Free Text (Line 18 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26029,
                new EntityDescriptor(
                    "LINE 19",
                    "Structured Free Text (Line 19 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26030,
                new EntityDescriptor(
                    "LINE 20",
                    "Structured Free Text (Line 20 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26031,
                new EntityDescriptor(
                    "LINE 21",
                    "Structured Free Text (Line 21 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26032,
                new EntityDescriptor(
                    "LINE 22",
                    "Structured Free Text (Line 22 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26033,
                new EntityDescriptor(
                    "LINE 23",
                    "Structured Free Text (Line 23 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26034,
                new EntityDescriptor(
                    "LINE 24",
                    "Structured Free Text (Line 24 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26035,
                new EntityDescriptor(
                    "LINE 25",
                    "Structured Free Text (Line 25 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26036,
                new EntityDescriptor(
                    "LINE 26",
                    "Structured Free Text (Line 26 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26037,
                new EntityDescriptor(
                    "LINE 27",
                    "Structured Free Text (Line 27 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26038,
                new EntityDescriptor(
                    "LINE 28",
                    "Structured Free Text (Line 28 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26039,
                new EntityDescriptor(
                    "LINE 29",
                    "Structured Free Text (Line 29 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26040,
                new EntityDescriptor(
                    "LINE 30",
                    "Structured Free Text (Line 30 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26041,
                new EntityDescriptor(
                    "LINE 31",
                    "Structured Free Text (Line 31 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26042,
                new EntityDescriptor(
                    "LINE 32",
                    "Structured Free Text (Line 32 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26043,
                new EntityDescriptor(
                    "LINE 33",
                    "Structured Free Text (Line 33 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26044,
                new EntityDescriptor(
                    "LINE 34",
                    "Structured Free Text (Line 34 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26045,
                new EntityDescriptor(
                    "LINE 35",
                    "Structured Free Text (Line 35 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26046,
                new EntityDescriptor(
                    "LINE 36",
                    "Structured Free Text (Line 36 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26047,
                new EntityDescriptor(
                    "LINE 37",
                    "Structured Free Text (Line 37 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26048,
                new EntityDescriptor(
                    "LINE 38",
                    "Structured Free Text (Line 38 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26049,
                new EntityDescriptor(
                    "LINE 39",
                    "Structured Free Text (Line 39 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26050,
                new EntityDescriptor(
                    "LINE 40",
                    "Structured Free Text (Line 40 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26051,
                new EntityDescriptor(
                    "LINE 41",
                    "Structured Free Text (Line 41 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26052,
                new EntityDescriptor(
                    "LINE 42",
                    "Structured Free Text (Line 42 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26053,
                new EntityDescriptor(
                    "LINE 43",
                    "Structured Free Text (Line 43 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26054,
                new EntityDescriptor(
                    "LINE 44",
                    "Structured Free Text (Line 44 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26055,
                new EntityDescriptor(
                    "LINE 45",
                    "Structured Free Text (Line 45 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26056,
                new EntityDescriptor(
                    "LINE 46",
                    "Structured Free Text (Line 46 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26057,
                new EntityDescriptor(
                    "LINE 47",
                    "Structured Free Text (Line 47 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26058,
                new EntityDescriptor(
                    "LINE 48",
                    "Structured Free Text (Line 48 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26059,
                new EntityDescriptor(
                    "LINE 49",
                    "Structured Free Text (Line 49 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26060,
                new EntityDescriptor(
                    "LINE 50",
                    "Structured Free Text (Line 50 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26061,
                new EntityDescriptor(
                    "LINE 51",
                    "Structured Free Text (Line 51 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26062,
                new EntityDescriptor(
                    "LINE 52",
                    "Structured Free Text (Line 52 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26063,
                new EntityDescriptor(
                    "LINE 53",
                    "Structured Free Text (Line 53 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26064,
                new EntityDescriptor(
                    "LINE 54",
                    "Structured Free Text (Line 54 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26065,
                new EntityDescriptor(
                    "LINE 55",
                    "Structured Free Text (Line 55 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26066,
                new EntityDescriptor(
                    "LINE 56",
                    "Structured Free Text (Line 56 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26067,
                new EntityDescriptor(
                    "LINE 57",
                    "Structured Free Text (Line 57 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26068,
                new EntityDescriptor(
                    "LINE 58",
                    "Structured Free Text (Line 58 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26069,
                new EntityDescriptor(
                    "LINE 59",
                    "Structured Free Text (Line 59 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26070,
                new EntityDescriptor(
                    "LINE 60",
                    "Structured Free Text (Line 60 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26071,
                new EntityDescriptor(
                    "LINE 61",
                    "Structured Free Text (Line 61 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26072,
                new EntityDescriptor(
                    "LINE 62",
                    "Structured Free Text (Line 62 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26073,
                new EntityDescriptor(
                    "LINE 63",
                    "Structured Free Text (Line 63 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26074,
                new EntityDescriptor(
                    "LINE 64",
                    "Structured Free Text (Line 64 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26075,
                new EntityDescriptor(
                    "LINE 65",
                    "Structured Free Text (Line 65 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26076,
                new EntityDescriptor(
                    "LINE 66",
                    "Structured Free Text (Line 66 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26077,
                new EntityDescriptor(
                    "LINE 67",
                    "Structured Free Text (Line 67 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26078,
                new EntityDescriptor(
                    "LINE 68",
                    "Structured Free Text (Line 68 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26079,
                new EntityDescriptor(
                    "LINE 69",
                    "Structured Free Text (Line 69 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26080,
                new EntityDescriptor(
                    "LINE 70",
                    "Structured Free Text (Line 70 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26081,
                new EntityDescriptor(
                    "LINE 71",
                    "Structured Free Text (Line 71 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26082,
                new EntityDescriptor(
                    "LINE 72",
                    "Structured Free Text (Line 72 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26083,
                new EntityDescriptor(
                    "LINE 73",
                    "Structured Free Text (Line 73 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26084,
                new EntityDescriptor(
                    "LINE 74",
                    "Structured Free Text (Line 74 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26085,
                new EntityDescriptor(
                    "LINE 75",
                    "Structured Free Text (Line 75 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26086,
                new EntityDescriptor(
                    "LINE 76",
                    "Structured Free Text (Line 76 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26087,
                new EntityDescriptor(
                    "LINE 77",
                    "Structured Free Text (Line 77 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26088,
                new EntityDescriptor(
                    "LINE 78",
                    "Structured Free Text (Line 78 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26089,
                new EntityDescriptor(
                    "LINE 79",
                    "Structured Free Text (Line 79 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26090,
                new EntityDescriptor(
                    "LINE 80",
                    "Structured Free Text (Line 80 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26091,
                new EntityDescriptor(
                    "LINE 81",
                    "Structured Free Text (Line 81 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26092,
                new EntityDescriptor(
                    "LINE 82",
                    "Structured Free Text (Line 82 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26093,
                new EntityDescriptor(
                    "LINE 83",
                    "Structured Free Text (Line 83 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26094,
                new EntityDescriptor(
                    "LINE 84",
                    "Structured Free Text (Line 84 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26095,
                new EntityDescriptor(
                    "LINE 85",
                    "Structured Free Text (Line 85 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26096,
                new EntityDescriptor(
                    "LINE 86",
                    "Structured Free Text (Line 86 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26097,
                new EntityDescriptor(
                    "LINE 87",
                    "Structured Free Text (Line 87 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26098,
                new EntityDescriptor(
                    "LINE 88",
                    "Structured Free Text (Line 88 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
            },
            {
                26099,
                new EntityDescriptor(
                    "LINE 89",
                    "Structured Free Text (Line 89 Data)",
                    $@"^{GetAlphanumericRegEx(1, -1)}$")
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

                var lastChar = dataIdentifier?[^1..]?.ToInvariantUpper();
                var ascii = lastChar?[0] - 64;

                if (ascii is < 1 or > 65)
                {
                    break;
                }

                if (dataIdentifier?.Length > 1)
                {
                    var entityValuePart = dataIdentifier[..^1];

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
        return descriptorKey < 0 ? new EntityDescriptor(null, null, null) : Descriptors[descriptorKey];
    }

    /// <summary>
    ///   RFC 1738 URL regular expression pattern.
    /// </summary>
    /// <returns>An RFC 1738 URL regular expression pattern.</returns>
    private static string Rfc1738UrlPattern()
    {
        const string alpha = "[a-zA-Z]";
        const string digit = "[0-9]";
        const string digits = $"{digit}+";
        const string safe = "[-$_.+]";
        const string extra = "[!*'(),]";
        const string alphaDigit = "[a-zA-Z0-9]";
        const string topLabel = $"{alpha}({alphaDigit}|-)*{alphaDigit}|{alpha}";
        const string domainLabel = $"{alphaDigit}({alphaDigit}|-)*{alphaDigit}|{alphaDigit}";
        const string hostName = $"(({domainLabel})\\.)*{topLabel}";
        const string hostNumber = $"{digits}\\.{digits}\\.{digits}\\.{digits}";
        const string host = $"{hostName}|{hostNumber}";
        const string hostPort = $"({host})(:{digits})?";
        const string hex = $"{digit}|A-F|a-f";
        const string escape = $"%{hex}{hex}";
        const string unreserved = $"{alpha}|{digit}|{safe}|{extra}";

        // ReSharper disable once IdentifierTypo
        const string uchar = $"{unreserved}|{escape}";

        // ReSharper disable once IdentifierTypo
        const string hsegment = $"({uchar}|;|\\?|&|=)*";
        const string search = $"({uchar}|;|\\?|&|=)*";

        // ReSharper disable once IdentifierTypo
        const string hpath = $@"{hsegment}(\/{hsegment})*";
        const string httpUrl = $@"(http[s]?:\/\/{hostPort}(\/{hpath}(\\?{search})?)?)";
        return httpUrl;
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