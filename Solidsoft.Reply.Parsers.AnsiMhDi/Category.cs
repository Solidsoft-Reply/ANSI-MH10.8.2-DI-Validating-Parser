// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Category.cs" company="Solidsoft Reply Ltd">
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
// Defined ASC MH10.8.2 categories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable UnusedMember.Global
namespace Solidsoft.Reply.Parsers.AnsiMhDi;

/// <summary>
///   Defined ASC MH10.8.2 categories.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1700:Do not name enum values 'Reserved'", Justification = "<Pending>")]

// ReSharper disable once UnusedType.Global
public enum Category {
    /// <summary>
    ///   CATEGORY 0 Special Characters Employed as Data Identifiers
    /// </summary>
    SpecialCharactersEmployedAsDataIdentifiers = 0,

    /// <summary>
    ///   CATEGORY 1 Reserved
    /// </summary>
    Reserved1 = 1,

    /// <summary>
    ///   CATEGORY 2 Container Information
    /// </summary>
    ContainerInformation = 2,

    /// <summary>
    ///   CATEGORY 3 Field Continuation
    /// </summary>
    FieldContinuation = 3,

    /// <summary>
    ///   CATEGORY 4 Date
    /// </summary>
    Date = 4,

    /// <summary>
    ///   CATEGORY 5 Environmental Factors
    /// </summary>
    EnvironmentalFactors = 5,

    /// <summary>
    ///   CATEGORY 6 Looping
    /// </summary>
    Looping = 6,

    /// <summary>
    ///   CATEGORY 7 Reserved
    /// </summary>
    Reserved = 7,

    /// <summary>
    ///   CATEGORY 8 Human Resources
    /// </summary>
    HumanResources = 8,

    /// <summary>
    ///   CATEGORY 9 Reserved
    /// </summary>
    Reserved9 = 9,

    /// <summary>
    ///   CATEGORY 10 License Plate
    /// </summary>
    // ReSharper disable once IdentifierTypo
    LicencePlate = 10,

    /// <summary>
    ///   CATEGORY 11 Transaction Reference
    /// </summary>
    TransactionReference = 11,

    /// <summary>
    ///   CATEGORY 12 Location Reference
    /// </summary>
    LocationReference = 12,

    /// <summary>
    ///   CATEGORY 13 Maintenance Codes
    /// </summary>
    MaintenanceCodes = 13,

    /// <summary>
    ///   CATEGORY 14 Industry Assigned Codes
    /// </summary>
    IndustryAssignedCodes = 14,

    /// <summary>
    ///   CATEGORY 15 Reserved
    /// </summary>
    Reserved15 = 15,

    /// <summary>
    ///   CATEGORY 16 Item Information
    /// </summary>
    ItemInformation = 16,

    /// <summary>
    ///   CATEGORY 17 Measurement
    /// </summary>
    Measurement = 17,

    /// <summary>
    ///   CATEGORY 18 Miscellaneous
    /// </summary>
    Miscellaneous = 18,

    /// <summary>
    ///   CATEGORY 19 Traceability Number for an Entity
    /// </summary>
    TraceabilityNumberForAnEntity = 19,

    /// <summary>
    ///   CATEGORY 20 Traceability Number for Groups of Entities
    /// </summary>
    TraceabilityNumberForGroupsOfEntities = 20,

    /// <summary>
    ///   CATEGORY 21 UPU / MH 10/SC8/WG2 Agreed Upon Codes
    /// </summary>
    UpuMh10Sc8Wg2AgreedUponCodes = 21,

    /// <summary>
    ///   CATEGORY 22 Party to the Transaction
    /// </summary>
    PartyToTheTransaction = 22,

    /// <summary>
    ///   CATEGORY 23 Activity Reference
    /// </summary>
    ActivityReference = 23,

    /// <summary>
    ///   CATEGORY 24 Reserved
    /// </summary>
    Reserved24 = 24,

    /// <summary>
    ///   CATEGORY 25 Internal Applications
    /// </summary>
    InternalApplications = 25,

    /// <summary>
    ///   CATEGORY 26 Mutually Defined
    /// </summary>
    MutuallyDefined = 26,
}