// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIdentifier.cs" company="Solidsoft Reply Ltd.">
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
// ASC MH10.8 0 Data Identifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo

namespace Solidsoft.Reply.Parsers.AnsiMhDi;

using System.Diagnostics.CodeAnalysis;

/// <summary>
///   ASC MH10.8 0 Data Identifiers.
/// </summary>
/// <remarks>
///   Flag characters not assigned or controlled by ANSI/MH10.8. The usage of any alphabetic,
///   numeric, or special character in a leading position (as a "Data Identifier or Application
///   Identifier") not defined herein is reserved for future assignment by the body controlling
///   these guidelines.
/// </remarks>
public enum DataIdentifier
{
    /// <summary>
    ///   Unrecognised entity.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    Unrecognised = -1,

    /// <summary>
    ///   The plus character.
    /// </summary>
    Plus = 0,

    /// <summary>
    ///   The ampersand character.
    /// </summary>
    Ampersand = 2,

    /// <summary>
    ///   The equal character.
    /// </summary>
    Equal = 3,

    /// <summary>
    ///   The FUNCTION 1 character.
    /// </summary>
    Func1 = 4,

    /// <summary>
    ///   The ISO.IEC 15434 preamble characters.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Isoiec15434Preamble = 5,

    /// <summary>
    ///   The hyphen character.
    /// </summary>
    Hyphen = 6,

    /// <summary>
    ///   The exclamation mark character.
    /// </summary>
    ExclamationMark = 7,

    /// <summary>
    ///   The container type.
    /// </summary>
    ContainerType = 2000,

    /// <summary>
    ///   The returnable container.
    /// </summary>
    ReturnableContainer = 2001,

    /// <summary>
    ///   The gas cylinder container.
    /// </summary>
    GasCylinderContainer = 2002,

    /// <summary>
    ///   The motor freight transport equipment.
    /// </summary>
    MotorFreightTransportEquipment = 2003,

    // ReSharper disable once CommentTypo

    /// <summary>
    ///   The SCAC.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Scac = 2004,

    /// <summary>
    ///   The receptacle asset.
    /// </summary>
    ReceptacleAsset = 2005,

    /// <summary>
    ///   The container serial.
    /// </summary>
    ContainerSerial = 2007,

    /// <summary>
    ///   The returnable container owner.
    /// </summary>
    ReturnableContainerOwner = 2008,

    /// <summary>
    ///   The container size.
    /// </summary>
    ContainerSize = 2009,

    /// <summary>
    ///   The container ownership.
    /// </summary>
    ContainerOwnership = 2010,

    /// <summary>
    ///   The van.
    /// </summary>
    Van = 2011,

    /// <summary>
    ///   The van check digit.
    /// </summary>
    VanCheckDigit = 2012,

    /// <summary>
    ///   The container.
    /// </summary>
    Container = 2013,

    /// <summary>
    ///   The tag status.
    /// </summary>
    TagStatus = 2014,

    /// <summary>
    ///   The dangerous cargo class.
    /// </summary>
    DangerousCargoClass = 2015,

    /// <summary>
    ///   The dangerous goods.
    /// </summary>
    DangerousGoods = 2016,

    /// <summary>
    ///   The transportation subject.
    /// </summary>
    TransportationSubject = 2017,

    /// <summary>
    ///   The vessel registration.
    /// </summary>
    VesselRegistration = 2018,

    /// <summary>
    ///   The voyage.
    /// </summary>
    Voyage = 2019,

    /// <summary>
    ///   The vessel country.
    /// </summary>
    VesselCountry = 2020,

    /// <summary>
    ///   The electronic seal.
    /// </summary>
    ElectronicSeal = 2021,

    /// <summary>
    ///   The entry.
    /// </summary>
    Entry = 2022,

    /// <summary>
    ///   The surety.
    /// </summary>
    Surety = 2023,

    /// <summary>
    ///   The foreign port of lading.
    /// </summary>
    ForeignPortOfLading = 2024,

    /// <summary>
    ///   The party to transaction.
    /// </summary>
    IacCinRtiSerial = 2025,

    /// <summary>
    ///   The unique returnable transport item.
    /// </summary>
    UniqueReturnableTransportItem = 2026,

    /// <summary>
    ///   The LLC RTI asset.
    /// </summary>
    LlcRtiAsset = 2027,

    /// <summary>
    ///   The SLC RTI asset.
    /// </summary>
    SlcRtiAsset = 2028,

    /// <summary>
    ///   The RPI identifier.
    /// </summary>
    RpiIdentifier = 2029,

    /// <summary>
    ///   The packaging item.
    /// </summary>
    PackagingItem = 2030,

    /// <summary>
    ///   The packaging.
    /// </summary>
    Packaging = 2031,

    /// <summary>
    ///   The RPI.
    /// </summary>
    Rpi = 2055,

    /// <summary>
    ///   The customer item code continuation.
    /// </summary>
    CustomerItemCodeCont = 3000,

    /// <summary>
    ///   The supplier traceability code continuation.
    /// </summary>
    SupplierTraceabilityCodeCont = 3001,

    /// <summary>
    ///   The supplier free text continuation.
    /// </summary>
    SupplierFreeTextCont = 3002,

    /// <summary>
    ///   The free text continuation.
    /// </summary>
    FreeTextCont = 3003,

    /// <summary>
    ///   The transaction reference continuation.
    /// </summary>
    TransactionReferenceCont = 3004,

    /// <summary>
    ///   The supplier item code continuation.
    /// </summary>
    SupplierItemCodeCont = 3005,

    /// <summary>
    ///   The date YYMMDD.
    /// </summary>
    DateYyMmDd = 4000,

    /// <summary>
    ///   The date DDMMYY.
    /// </summary>
    DateDdMmYy = 4001,

    /// <summary>
    ///   The date MMDDYY.
    /// </summary>
    DateMmDdYy = 4002,

    /// <summary>
    ///   The date YDDD julian.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    DateYdddJulian = 4003,

    /// <summary>
    ///   The date YYDDD julian.
    /// </summary>
    DateYyDddJulian = 4004,

    /// <summary>
    ///   The date and type YYMMDD.
    /// </summary>
    DateAndTypeYyMmDd = 4005,

    /// <summary>
    ///   The date and type YYYYMMDD.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    DateAndTypeYyyyMmDd = 4006,

    /// <summary>
    ///   The month MMYY.
    /// </summary>
    MonthMmYy = 4007,

    /// <summary>
    ///   The event date and time.
    /// </summary>
    EventDateAndTime = 4008,

    /// <summary>
    ///   The date.
    /// </summary>
    Date = 4009,

    /// <summary>
    ///   The week YYWW.
    /// </summary>
    WeekYyWw = 4010,

    /// <summary>
    ///   The week YYYYWW.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    WeekYyyyWw = 4011,

    /// <summary>
    ///   The date YYYYMMDD.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    DateYyyyMmDd = 4012,

    /// <summary>
    ///   The oldest and newest manufacturing date.
    /// </summary>
    OldestAndNewestManufacturingDate = 4013,

    /// <summary>
    ///   The expiration date YYYYMMDD.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    ExpirationDateYyyyMmDd = 4014,

    /// <summary>
    ///   The expiration date DDMMYYYY.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    ExpirationDateDdMmYyyy = 4015,

    /// <summary>
    ///   The production date YYYYMMDD.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    ProductionDateYyyyyMmDd = 4016,

    /// <summary>
    ///   The production date DDMMYYYY.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    ProductionDateDdMmYyyy = 4017,

    /// <summary>
    ///   The tag activation time.
    /// </summary>
    TagActivationTime = 4018,

    /// <summary>
    ///   The tag deactivation time.
    /// </summary>
    TagDeactivationTime = 4019,

    /// <summary>
    ///   The inspection date.
    /// </summary>
    InspectionDate = 4020,

    /// <summary>
    ///   The DoD MILSTAMP code.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    DodMilstampCode = 4021,

    /// <summary>
    ///   The record time.
    /// </summary>
    RecordTime = 4022,

    /// <summary>
    ///   The UTC date.
    /// </summary>
    UtcDate = 4023,

    /// <summary>
    ///   The qualified date.
    /// </summary>
    QualifiedDate = 4024,

    /// <summary>
    ///   The best before date.
    /// </summary>
    BestBeforeDate = 4025,

    /// <summary>
    ///   The first freeze date.
    /// </summary>
    FirstFreezeDate = 4026,

    /// <summary>
    ///   The harvest date.
    /// </summary>
    HarvestDate = 4027,

    /// <summary>
    ///   The harvest date range.
    /// </summary>
    HarvestDateRange = 4028,

    /// <summary>
    ///   The restricted substances classification.
    /// </summary>
    RestrictedSubstancesClassification = 5000,

    /// <summary>
    ///   The air pressure.
    /// </summary>
    AirPressure = 5001,

    /// <summary>
    ///   The maximum allowed temperature.
    /// </summary>
    MaximumAllowedTemperature = 5002,

    /// <summary>
    ///   The minimum allowed temperature.
    /// </summary>
    MinimumAllowedTemperature = 5003,

    /// <summary>
    ///   The maximum allowed relative humidity.
    /// </summary>
    MaximumAllowedRelativeHumidity = 5004,

    /// <summary>
    ///   The minimum allowed relative humidity.
    /// </summary>
    MinimumAllowedRelativeHumidity = 5005,

    /// <summary>
    ///   The refrigerator container temperature.
    /// </summary>
    RefrigeratorContainerTemperature = 5006,

    /// <summary>
    ///   The cumulative time temperature index.
    /// </summary>
    CumulativeTimeTemperatureIndex = 5010,

    /// <summary>
    ///   The time temperature index.
    /// </summary>
    TimeTemperatureIndex = 5011,

    /// <summary>
    ///   The packaging material.
    /// </summary>
    PackagingMaterial = 5012,

    /// <summary>
    ///   The MSL.
    /// </summary>
    Msl = 5013,

    /// <summary>
    ///   The looping header.
    /// </summary>
    LoopingHeader = 6000,

    /// <summary>
    ///   The parent.
    /// </summary>
    Parent = 6001,

    /// <summary>
    ///   The number of children.
    /// </summary>
    NumberOfChildren = 6003,

    /// <summary>
    ///   The logical assignment.
    /// </summary>
    LogicalAssignment = 6004,

    /// <summary>
    ///   The children.
    /// </summary>
    Children = 6005,

    /// <summary>
    ///   The name.
    /// </summary>
    Name = 8000,

    /// <summary>
    ///   The employee id.
    /// </summary>
    EmployeeId = 8001,

    /// <summary>
    ///   The social security number.
    /// </summary>
    SocialSecurityNumber = 8002,

    /// <summary>
    ///   The non employee id.
    /// </summary>
    NonEmployeeId = 8003,

    /// <summary>
    ///   The national social security number.
    /// </summary>
    NationalSocialSecurityNumber = 8004,

    /// <summary>
    ///   The last name.
    /// </summary>
    LastName = 8005,

    /// <summary>
    ///   The party name.
    /// </summary>
    PartyName = 8006,

    /// <summary>
    ///   The contact phone.
    /// </summary>
    ContactPhone = 8007,

    /// <summary>
    ///   The contact email.
    /// </summary>
    ContactEmail = 8008,

    /// <summary>
    ///   The consignee number.
    /// </summary>
    ConsigneeNumber = 8009,

    /// <summary>
    ///   The personal ID.
    /// </summary>
    PersonalId = 8010,

    /// <summary>
    ///   The first name and middle initial.
    /// </summary>
    FirstNameAndMiddleInitial = 8011,

    /// <summary>
    ///   The military grade.
    /// </summary>
    MilitaryGrade = 8012,

    /// <summary>
    ///   The NI number.
    /// </summary>
    NiNumber = 8015,

    /// <summary>
    ///   The IAC CIN personal ID.
    /// </summary>
    IacCinPersonalId = 8025,

    /// <summary>
    ///   The IAC CIN personal ID + EDIFACT party qualifier
    /// </summary>
    IacCinPersonalIdEdifactPartyQualifier = 8026,

    /// <summary>
    ///   The vin.
    /// </summary>
    Vin = 9000,

    /// <summary>
    ///   The abbreviated vin.
    /// </summary>
    AbbreviatedVin = 9002,

    /// <summary>
    ///   The transport vehicle identifier.
    /// </summary>
    TransportVehicleIdentifier = 9004,

    /// <summary>
    ///   The production vehicle identifier.
    /// </summary>
    ProductionVehicleIdentifier = 9005,

    /// <summary>
    ///   The license plate.
    /// </summary>
    LicensePlate = 10000,

    /// <summary>
    ///   The unbreakable unit license plate.
    /// </summary>
    UnbreakableUnitLicensePlate = 10001,

    /// <summary>
    ///   The transport unit license plate.
    /// </summary>
    TransportUnitLicensePlate = 10002,

    /// <summary>
    ///   The EDI unbreakable unit license plate.
    /// </summary>
    EdiUnbreakableUnitLicensePlate = 10003,

    /// <summary>
    ///   The EDI transport unit license plate.
    /// </summary>
    EdiTransportUnitLicensePlate = 10004,

    /// <summary>
    ///   The mixed transport unit license plate.
    /// </summary>
    MixedTransportUnitLicensePlate = 10005,

    /// <summary>
    ///   The master transport unit license plate.
    /// </summary>
    MasterTransportUnitLicensePlate = 10006,

    /// <summary>
    ///   The vehicle registration license plate number.
    /// </summary>
    VehicleRegistrationLicensePlateNumber = 10007,

    /// <summary>
    ///   The MMSI.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Mmsi = 10008,

    /// <summary>
    ///   The customer order number.
    /// </summary>
    CustomerOrderNumber = 11000,

    /// <summary>
    ///   The supplier order number.
    /// </summary>
    SupplierOrderNumber = 11001,

    /// <summary>
    ///   The supplier bill of lading.
    /// </summary>
    SupplierBillOfLading = 11002,

    /// <summary>
    ///   The carrier bill of lading.
    /// </summary>
    CarrierBillOfLading = 11003,

    /// <summary>
    ///   The order line.
    /// </summary>
    OrderLine = 11004,

    /// <summary>
    ///   The reference number.
    /// </summary>
    ReferenceNumber = 11005,

    /// <summary>
    ///   The pro no.
    /// </summary>
    ProNo = 11006,

    /// <summary>
    ///   The carrier mode.
    /// </summary>
    CarrierMode = 11007,

    /// <summary>
    ///   The contract number.
    /// </summary>
    ContractNumber = 11008,

    /// <summary>
    ///   The transaction reference.
    /// </summary>
    TransactionReference = 11009,

    /// <summary>
    ///   The invoice number.
    /// </summary>
    InvoiceNumber = 11010,

    /// <summary>
    ///   The packing list number.
    /// </summary>
    PackingListNumber = 11011,

    /// <summary>
    ///   The SCAC &amp; PRO Number.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    ScacProNumber = 11012,

    /// <summary>
    ///   The bill of lading number.
    /// </summary>
    BillOfLadingNumber = 11013,

    /// <summary>
    ///   The order and line.
    /// </summary>
    OrderAndLine = 11014,

    /// <summary>
    ///   The KANBAN.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Kanban = 11015,

    /// <summary>
    ///   The DELINS.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Delins = 11016,

    /// <summary>
    ///   The check.
    /// </summary>
    Check = 11017,

    /// <summary>
    ///   The reference.
    /// </summary>
    Reference = 11018,

    /// <summary>
    ///   The foreign military sales case.
    /// </summary>
    ForeignMilitarySalesCase = 11019,

    /// <summary>
    ///   The license identifier.
    /// </summary>
    LicenseIdentifier = 11020,

    /// <summary>
    ///   The customer data.
    /// </summary>
    CustomerData = 11021,

    /// <summary>
    ///   The transaction authentication.
    /// </summary>
    TransactionAuthentication = 11022,

    /// <summary>
    ///   The carrier transport units groupings.
    /// </summary>
    CarrierTransportUnitsGroupings = 11025,

    /// <summary>
    ///   The shipper transport units groupings.
    /// </summary>
    ShipperTransportUnitsGroupings = 11026,

    /// <summary>
    ///   The quotation.
    /// </summary>
    Quotation = 11027,

    /// <summary>
    ///   The storage location.
    /// </summary>
    StorageLocation = 12000,

    /// <summary>
    ///   The location.
    /// </summary>
    Location = 12001,

    /// <summary>
    ///   The ship to.
    /// </summary>
    ShipTo = 12002,

    /// <summary>
    ///   The ship from.
    /// </summary>
    ShipFrom = 12003,

    /// <summary>
    ///   The country of origin.
    /// </summary>
    CountryOfOrigin = 12004,

    /// <summary>
    ///   The ship for.
    /// </summary>
    ShipFor = 12005,

    /// <summary>
    ///   The route code.
    /// </summary>
    RouteCode = 12006,

    /// <summary>
    ///   The DoDAAC.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Dodaac = 12007,

    /// <summary>
    ///   The port of embarkation.
    /// </summary>
    PortOfEmbarkation = 12008,

    /// <summary>
    ///   The port of debarkation.
    /// </summary>
    PortOfDebarkation = 12009,

    /// <summary>
    ///   The location coordinates.
    /// </summary>
    LocationCoordinates = 12011,

    /// <summary>
    ///   The ship to coordinates.
    /// </summary>
    ShipToCoordinates = 12012,

    /// <summary>
    ///   The ship from coordinates.
    /// </summary>
    ShipFromCoordinates = 12013,

    /// <summary>
    ///   The ship for coordinates.
    /// </summary>
    ShipForCoordinates = 12015,

    /// <summary>
    ///   The tag activation location.
    /// </summary>
    TagActivationLocation = 12016,

    /// <summary>
    ///   The tag deactivation location.
    /// </summary>
    TagDeactivationLocation = 12017,

    /// <summary>
    ///   The FAO fishing area.
    /// </summary>
    FaoFishingArea = 12018,

    /// <summary>
    ///   The first level.
    /// </summary>
    FirstLevel20L = 12020,

    /// <summary>
    ///   The second level.
    /// </summary>
    SecondLevel21L = 12021,

    /// <summary>
    ///   The third level.
    /// </summary>
    ThirdLevel22L = 12022,

    /// <summary>
    ///   The fourth level.
    /// </summary>
    FourthLevel23L = 12023,

    /// <summary>
    ///   The fifth level.
    /// </summary>
    FifthLevel24L = 12024,

    /// <summary>
    ///   The party to transaction.
    /// </summary>
    IacCinLoc = 12025,

    /// <summary>
    ///   The location code.
    /// </summary>
    LocationCode = 12026,

    /// <summary>
    ///   The event location.
    /// </summary>
    EventLocation = 12027,

    /// <summary>
    ///   The number and street.
    /// </summary>
    NumberAndStreet = 12028,

    /// <summary>
    ///   The city.
    /// </summary>
    City = 12029,

    /// <summary>
    ///   The country sub entity.
    /// </summary>
    CountrySubEntity = 12030,

    /// <summary>
    ///   The postal code.
    /// </summary>
    PostalCode = 12031,

    /// <summary>
    ///   The country.
    /// </summary>
    Country = 12032,

    /// <summary>
    ///   The URL.
    /// </summary>
    Url = 12033,

    /// <summary>
    ///   The P2P URL.
    /// </summary>
    P2PUrl = 12034,

    /// <summary>
    ///   The site approval.
    /// </summary>
    SiteApproval = 12035,

    /// <summary>
    ///   The producer approval.
    /// </summary>
    ProducerApproval = 12036,

    /// <summary>
    ///   The ship from postal code.
    /// </summary>
    ShipFromPostalCode = 12051,

    /// <summary>
    ///   The ship to postal code.
    /// </summary>
    ShipToPostalCode = 12052,

    /// <summary>
    ///   The ship from global.
    /// </summary>
    ShipFromGlobal = 12054,

    /// <summary>
    ///   The ship to global.
    /// </summary>
    ShipToGlobal = 12055,

    /// <summary>
    ///   Army form 2410.
    /// </summary>
    Form2410 = 13010,

    /// <summary>
    ///   Army form 2408.
    /// </summary>
    Form2408 = 13011,

    /// <summary>
    ///   Army form 2407.
    /// </summary>
    Form2407 = 13012,

    /// <summary>
    ///   Air Force form 95.
    /// </summary>
    Form95 = 13013,

    /// <summary>
    ///   Navy form 4790.
    /// </summary>
    Form4790 = 13014,

    /// <summary>
    ///   The NSN.
    /// </summary>
    Nsn = 14000,

    /// <summary>
    ///   The CIDX product characteristic.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    CidxProductCharacteristic = 14001,

    /// <summary>
    ///   EIAJ encoded.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    EiajEncoded = 14003,

    /// <summary>
    ///   GS1 encoded.
    /// </summary>
    Gs1Encoded = 14004,

    /// <summary>
    ///   AIAG encoded.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    AiagEncoded = 14005,

    /// <summary>
    ///   MILSTRIP encoded.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    MilstripEncoded = 14006,

    /// <summary>
    ///   DTR encoded.
    /// </summary>
    DtrEncoded = 14007,

    /// <summary>
    ///   A production animal identification.
    /// </summary>
    ProductionAnimalIdentification = 14008,

    /// <summary>
    ///   IFA Pharmacy Product Number.
    /// </summary>
    Ppn = 14009,

    /// <summary>
    ///   IAC CIN encoded.
    /// </summary>
    IacCinEncoded = 14010,

    /// <summary>
    ///   RLA encoded, but with restricted use.
    /// </summary>
    RlaEncodedRestrictedUse = 14011,

    /// <summary>
    ///   RLA encoded.
    /// </summary>
    RlaEncoded = 14012,

    /// <summary>
    ///   CAICT Industrial Internet Identifier Codes.
    /// </summary>
    CaictIndustrialInternetId = 14015,

    /// <summary>
    ///   The customer item id.
    /// </summary>
    CustomerItemId = 16000,

    /// <summary>
    ///   The supplier item id.
    /// </summary>
    SupplierItemId = 16001,

    /// <summary>
    ///   The revision level.
    /// </summary>
    RevisionLevel = 16002,

    /// <summary>
    ///   The GS1 MFR item code.
    /// </summary>
    Gs1MfrItemCode = 16003,

    /// <summary>
    ///   The GS1 item code portion.
    /// </summary>
    Gs1ItemCodePortion = 16004,

    /// <summary>
    ///   The freight classification item.
    /// </summary>
    FreightClassificationItem = 16005,

    /// <summary>
    ///   The supplier item code.
    /// </summary>
    SupplierItemCode = 16006,

    /// <summary>
    ///   The CLEI.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Clei = 16007,

    /// <summary>
    ///   A GS1 GTIN-14.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Gs1Gtin14 = 16008,

    /// <summary>
    ///   The DUNS item code.
    /// </summary>
    DunsItemCode = 16009,

    /// <summary>
    ///   The hazardous material code.
    /// </summary>
    HazardousMaterialCode = 16010,

    /// <summary>
    ///   The CLEI 10.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Clei10 = 16011,

    /// <summary>
    ///   The document type.
    /// </summary>
    DocumentType = 16012,

    /// <summary>
    ///   The VMRS system.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    VmrsSystem = 16013,

    /// <summary>
    ///   The VMRS system and assembly.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    VmrsSystemAssembly = 16014,

    /// <summary>
    ///   The VMRS system, assembly and part.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    VmrsSystemAssemblyPart = 16015,

    /// <summary>
    ///   The user-modified VMRS system, assembly and part.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    UserVmrsSystemAssemblyPart = 16016,

    /// <summary>
    ///   The GS1 supplier id item code.
    /// </summary>
    Gs1SupplierIdItemCode = 16017,

    /// <summary>
    ///   The VMRS supplier id and part no.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    VmrsSupplierIdPartNo = 16018,

    /// <summary>
    ///   The component.
    /// </summary>
    Component = 16019,

    /// <summary>
    ///   The customer first level.
    /// </summary>
    CustomerFirstLevel = 16020,

    /// <summary>
    ///   The customer second level.
    /// </summary>
    CustomerSecondLevel = 16021,

    /// <summary>
    ///   The customer third level.
    /// </summary>
    CustomerThirdLevel = 16022,

    /// <summary>
    ///   The customer fourth level.
    /// </summary>
    CustomerFourthLevel = 16023,

    /// <summary>
    ///   The customer fifth level.
    /// </summary>
    CustomerFifthLevel = 16024,

    /// <summary>
    ///   The party to transaction.
    /// </summary>
    IacCinPart = 16025,

    /// <summary>
    ///   The part number.
    /// </summary>
    PartNumber = 16026,

    /// <summary>
    ///   The HTS 6 code.
    /// </summary>
    Hts6Code = 16027,

    /// <summary>
    ///   The cargo name.
    /// </summary>
    CargoName = 16028,

    /// <summary>
    ///   The GMDN product classification.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    GmdnProductClassification = 16029,

    /// <summary>
    ///   The supplier first level.
    /// </summary>
    SupplierFirstLevel = 16030,

    /// <summary>
    ///   The supplier second level.
    /// </summary>
    SupplierSecondLevel = 16031,

    /// <summary>
    ///   The supplier third level.
    /// </summary>
    SupplierThirdLevel = 16032,

    /// <summary>
    ///   The supplier fourth level.
    /// </summary>
    SupplierFourthLevel = 16033,

    /// <summary>
    ///   The supplier fifth level.
    /// </summary>
    SupplierFifthLevel = 16034,

    /// <summary>
    ///   The MSDS code.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    MsdsCode = 16040,

    /// <summary>
    ///   The export controlled by.
    /// </summary>
    ExportControlledBy = 16049,

    /// <summary>
    ///   The manufacturer item id.
    /// </summary>
    ManufacturerItemId = 16050,

    /// <summary>
    ///   The item id.
    /// </summary>
    ItemId = 16051,

    /// <summary>
    ///   The color.
    /// </summary>
    Color = 16052,

    /// <summary>
    ///   Specific Marine Equipment.
    /// </summary>
    SpecificMarineEquipment = 16053,

    /// <summary>
    ///   Unique Device Identification - Device Identifier.
    /// </summary>
    UdiDi = 16054,

    /// <summary>
    ///   DNV certificate reference.
    /// </summary>
    DnvCertificateReference = 16055,
    
    /// <summary>
    ///   The quantity.
    /// </summary>
    Quantity = 17000,

    /// <summary>
    ///   The theoretical length weight.
    /// </summary>
    TheoreticalLengthWeight = 17001,

    /// <summary>
    ///   The actual weight.
    /// </summary>
    ActualWeight = 17002,

    /// <summary>
    ///   The unit of measure.
    /// </summary>
    UnitOfMeasure = 17003,

    /// <summary>
    ///   The gross amount.
    /// </summary>
    GrossAmount = 17004,

    /// <summary>
    ///   The net amount.
    /// </summary>
    NetAmount = 17005,

    /// <summary>
    ///   The containers.
    /// </summary>
    Containers = 17006,

    /// <summary>
    ///   The quantity UOM.
    /// </summary>
    QuantityUom = 17007,

    /// <summary>
    ///   The container rated weight.
    /// </summary>
    ContainerRatedWeight = 17008,

    /// <summary>
    ///   The piece weight.
    /// </summary>
    PieceWeight = 17009,

    /// <summary>
    ///   The tare weight.
    /// </summary>
    TareWeight = 17011,

    /// <summary>
    ///   The monetary value.
    /// </summary>
    MonetaryValue = 17012,

    /// <summary>
    ///   The piece of pieces.
    /// </summary>
    PieceOfPieces = 17013,

    /// <summary>
    ///   The beginning secondary quantity.
    /// </summary>
    BeginningSecondaryQuantity = 17014,

    /// <summary>
    ///   The ending secondary quantity.
    /// </summary>
    EndingSecondaryQuantity = 17015,

    /// <summary>
    ///   The number of pieces in van.
    /// </summary>
    NumberOfPiecesInVan = 17016,

    /// <summary>
    ///   The number of shipments in van.
    /// </summary>
    NumberOfShipmentsInVan = 17017,

    /// <summary>
    ///   The cube.
    /// </summary>
    Cube = 17018,

    /// <summary>
    ///   The width.
    /// </summary>
    Width = 17019,

    /// <summary>
    ///   The height.
    /// </summary>
    Height = 17020,

    /// <summary>
    ///   The length.
    /// </summary>
    Length = 17021,

    /// <summary>
    ///   The net weight.
    /// </summary>
    NetWeight = 17022,

    /// <summary>
    ///   The van length.
    /// </summary>
    VanLength = 17023,

    /// <summary>
    ///   The inside cube of a van.
    /// </summary>
    InsideCubeOfAVan = 17024,

    /// <summary>
    ///   The net explosive weight.
    /// </summary>
    NetExplosiveWeight = 17025,

    /// <summary>
    ///   The packaging level.
    /// </summary>
    PackagingLevel = 17026,

    /// <summary>
    ///   The single product net price.
    /// </summary>
    SingleProductNetPrice = 17027,

    /// <summary>
    ///   The single price charge value for postage and packaging.
    /// </summary>
    SinglePriceChargeValueForPostageAndPackaging = 17028,

    /// <summary>
    ///   The discount percentage.
    /// </summary>
    DiscountPercentage = 17029,

    /// <summary>
    ///   The vat percentage.
    /// </summary>
    VatPercentage = 17030,

    /// <summary>
    ///   The currency.
    /// </summary>
    Currency = 17031,

    /// <summary>
    ///   The LOINC code.
    /// </summary>
    LoincCode = 17032,

    /// <summary>
    ///   The RMA.
    /// </summary>
    Rma = 18001,

    /// <summary>
    ///   The return code.
    /// </summary>
    ReturnCode = 18002,

    /// <summary>
    ///   The DoDIC.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Dodic = 18004,

    /// <summary>
    ///   The IAC CIN.
    /// </summary>
    IacCinDate = 18005,

    /// <summary>
    ///   The signature.
    /// </summary>
    Signature = 18006,

    /// <summary>
    ///   The ASFIS code.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    AsfisCode = 18007,

    /// <summary>
    ///   The FAO code.
    /// </summary>
    FaoCode = 18008,

    /// <summary>
    ///   The FAO production method.
    /// </summary>
    FaoProductionMethod = 18009,

    /// <summary>
    ///   The serial number.
    /// </summary>
    SerialNumber = 19000,

    /// <summary>
    ///   The additional code.
    /// </summary>
    AdditionalCode = 19001,

    /// <summary>
    ///   The ASN SID.
    /// </summary>
    AsnSid = 19002,

    /// <summary>
    ///   The unique package identification.
    /// </summary>
    UniquePackageIdentification = 19003,

    /// <summary>
    ///   The package identification like items.
    /// </summary>
    PackageIdentificationLikeItems = 19004,

    /// <summary>
    ///   The package identification unlike items.
    /// </summary>
    PackageIdentificationUnlikeItems = 19005,

    /// <summary>
    ///   The package identification like items multiple.
    /// </summary>
    PackageIdentificationLikeItemsMultiple = 19006,

    /// <summary>
    ///   The package identification unlike items multiple.
    /// </summary>
    PackageIdentificationUnlikeItemsMultiple = 19007,

    /// <summary>
    ///   The supplier id.
    /// </summary>
    SupplierId = 19008,

    /// <summary>
    ///   The package identification.
    /// </summary>
    PackageIdentification = 19009,

    /// <summary>
    ///   The id code.
    /// </summary>
    IdCode = 19010,

    /// <summary>
    ///   The fixed asset id code.
    /// </summary>
    FixedAssetIdCode = 19011,

    /// <summary>
    ///   The document number.
    /// </summary>
    DocumentNumber = 19012,

    /// <summary>
    ///   The container security seal.
    /// </summary>
    ContainerSecuritySeal = 19013,

    /// <summary>
    ///   The fourth class manifesting.
    /// </summary>
    FourthClassManifesting = 19014,

    /// <summary>
    ///   The serial number assigned by the vendor entity.
    /// </summary>
    SerialNumberAssignedByTheVendorEntity = 19015,

    /// <summary>
    ///   The version number.
    /// </summary>
    VersionNumber = 19016,

    /// <summary>
    ///   The GS1 supplier and unique package identification.
    /// </summary>
    Gs1SupplierAndUniquePackageIdentification = 19017,

    /// <summary>
    ///   The cage code serial number.
    /// </summary>
    CageCodeSerialNumber = 19018,

    /// <summary>
    ///   The duns and package identification.
    /// </summary>
    DunsAndPackageIdentification = 19019,

    /// <summary>
    ///   The traceability code.
    /// </summary>
    TraceabilityCode = 19020,

    /// <summary>
    ///   The tire identification number.
    /// </summary>
    TireIdentificationNumber = 19021,

    /// <summary>
    ///   The unique individual identity.
    /// </summary>
    UniqueIndividualIdentity = 19022,

    /// <summary>
    ///   The MAC address.
    /// </summary>
    MacAddress = 19023,

    /// <summary>
    ///   The RF tag unique identifier.
    /// </summary>
    RfTagUniqueIdentifier = 19024,

    /// <summary>
    ///   The party to transaction.
    /// </summary>
    IacCinSerial = 19025,

    /// <summary>
    ///   The reader id.
    /// </summary>
    ReaderId = 19026,

    /// <summary>
    ///   The item number within batch.
    /// </summary>
    ItemNumberWithinBatch = 19027,

    /// <summary>
    ///   The batch and item number.
    /// </summary>
    BatchAndItemNumber = 19028,

    /// <summary>
    ///   The additional traceability code.
    /// </summary>
    AdditionalTraceabilityCode = 19030,

    /// <summary>
    ///   The beginning serial number.
    /// </summary>
    BeginningSerialNumber = 19031,

    /// <summary>
    ///   The ending serial number.
    /// </summary>
    EndingSerialNumber = 19032,

    /// <summary>
    ///   The serial number of next higher assembly.
    /// </summary>
    SerialNumberOfNextHigherAssembly = 19033,

    /// <summary>
    ///   The part number of end item.
    /// </summary>
    PartNumberOfEndItem = 19034,

    /// <summary>
    ///   The bumper number.
    /// </summary>
    BumperNumber = 19035,

    /// <summary>
    ///   The pallet identifier.
    /// </summary>
    PalletIdentifier = 19036,

    /// <summary>
    ///   The IAC CIN PN and PSN.
    /// </summary>
    IacCinPnPsn = 19037,

    /// <summary>
    ///   The UII.
    /// </summary>
    Uii = 19042,

    /// <summary>
    ///   The ICCID.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    Iccid = 19043,

    /// <summary>
    ///   The first level.
    /// </summary>
    FirstLevel50S = 19050,

    /// <summary>
    ///   The second level.
    /// </summary>
    SecondLevel51S = 19051,

    /// <summary>
    ///   The third level.
    /// </summary>
    ThirdLevel52S = 19052,

    /// <summary>
    ///   The fourth level.
    /// </summary>
    FourthLevel53S = 19053,

    /// <summary>
    ///   The fifth level.
    /// </summary>
    FifthLevel54S = 19054,

    /// <summary>
    ///   The EPC number.
    /// </summary>
    EpcNumber = 19096,

    /// <summary>
    ///   The encrypted serial number.
    /// </summary>
    EncryptedSerialNumber = 19097,

    /// <summary>
    ///   The traceability number assigned by the customer.
    /// </summary>
    TraceabilityNumberAssignedByTheCustomer = 20000,

    /// <summary>
    ///   The traceability number assigned by the supplier.
    /// </summary>
    TraceabilityNumberAssignedByTheSupplier = 20001,

    /// <summary>
    ///   The exclusive assignment.
    /// </summary>
    ExclusiveAssignment = 20003,

    /// <summary>
    ///   The first level customer assigned.
    /// </summary>
    FirstLevelCustomerAssigned = 20020,

    /// <summary>
    ///   The second level customer assigned.
    /// </summary>
    SecondLevelCustomerAssigned = 20021,

    /// <summary>
    ///   The third level customer assigned.
    /// </summary>
    ThirdLevelCustomerAssigned = 20022,

    /// <summary>
    ///   The fourth level customer assigned.
    /// </summary>
    FourthLevelCustomerAssigned = 20023,

    /// <summary>
    ///   The fifth level customer assigned.
    /// </summary>
    FifthLevelCustomerAssigned = 20024,

    /// <summary>
    ///   The party to a transaction.
    /// </summary>
    IacCinTraceability = 20025,

    /// <summary>
    ///   The batch identifier.
    /// </summary>
    BatchIdentifier = 20026,

    /// <summary>
    ///   The batch number.
    /// </summary>
    BatchNumber = 20027,

    /// <summary>
    ///   The first level supplier assigned.
    /// </summary>
    FirstLevelSupplierAssigned = 20030,

    /// <summary>
    ///   The second level supplier assigned.
    /// </summary>
    SecondLevelSupplierAssigned = 20031,

    /// <summary>
    ///   The third level supplier assigned.
    /// </summary>
    ThirdLevelSupplierAssigned = 20032,

    /// <summary>
    ///   The fourth level supplier assigned.
    /// </summary>
    FourthLevelSupplierAssigned = 20033,

    /// <summary>
    ///   The fifth level supplier assigned.
    /// </summary>
    FifthLevelSupplierAssigned = 20034,

    /// <summary>
    ///   The postal service.
    /// </summary>
    PostalService = 21005,

    /// <summary>
    ///   The licensing post data.
    /// </summary>
    LicensingPostData = 21006,

    /// <summary>
    ///   The supplementary postal service.
    /// </summary>
    SupplementaryPostalService = 21015,

    /// <summary>
    ///   The postal administration identifications.
    /// </summary>
    PostalAdministrationIdentifications = 21016,

    /// <summary>
    ///   The UPU location code.
    /// </summary>
    UpuLocationCode = 21017,

    /// <summary>
    ///   The qualified UPU location code.
    /// </summary>
    QualifiedUpuLocationCode = 21018,

    /// <summary>
    ///   The license plate with service data and location code.
    /// </summary>
    LicensePlateWithServiceDataAndLocationCode = 21019,

    /// <summary>
    ///   The OCR data locator.
    /// </summary>
    OcrDataLocator = 21055,

    /// <summary>
    ///   The supplier code assigned by customer.
    /// </summary>
    SupplierCodeAssignedByCustomer = 22000,

    /// <summary>
    ///   The supplier code assigned by supplier.
    /// </summary>
    SupplierCodeAssignedBySupplier = 22001,

    /// <summary>
    ///   The UPC company prefix.
    /// </summary>
    UpcCompanyPrefix = 22002,

    /// <summary>
    ///   The GS1 company prefix.
    /// </summary>
    Gs1CompanyPrefix = 22003,

    /// <summary>
    ///   The carrier identification code.
    /// </summary>
    CarrierIdentificationCode = 22004,

    /// <summary>
    ///   The financial institution identification code.
    /// </summary>
    FinancialInstitutionIdentificationCode = 22005,

    /// <summary>
    ///   The manufacturer's identification code.
    /// </summary>
    ManufacturersIdentificationCode = 22006,

    /// <summary>
    ///   The liable party.
    /// </summary>
    LiableParty = 22007,

    /// <summary>
    ///   The customer code assigned by the customer.
    /// </summary>
    CustomerCodeAssignedByTheCustomer = 22008,

    /// <summary>
    ///   The customer code assigned by the supplier.
    /// </summary>
    CustomerCodeAssignedByTheSupplier = 22009,

    /// <summary>
    ///   The manufacturer id.
    /// </summary>
    ManufacturerId = 22010,

    /// <summary>
    ///   The budget holder.
    /// </summary>
    BudgetHolder = 22011,

    /// <summary>
    ///   The manufacturer duns number.
    /// </summary>
    ManufacturerDunsNumber = 22012,

    /// <summary>
    ///   The supplier duns number.
    /// </summary>
    SupplierDunsNumber = 22013,

    /// <summary>
    ///   The customer duns number.
    /// </summary>
    CustomerDunsNumber = 22014,

    /// <summary>
    ///   The carrier assigned shipper number.
    /// </summary>
    CarrierAssignedShipperNumber = 22015,

    /// <summary>
    ///   The VMRS supplier id.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    VmrsSupplierId = 22016,

    /// <summary>
    ///   The DoD cage code.
    /// </summary>
    DodCageCode = 22017,

    /// <summary>
    ///   The party to a transaction.
    /// </summary>
    IacCin = 22018,

    /// <summary>
    ///   The party's roles in a transaction.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    PartysRolesInATransaction = 22019,

    /// <summary>
    ///   The IAC CIN code values identification.
    /// </summary>
    IacCinCodeValuesIdentification = 22020,

    /// <summary>
    ///   The IAC CIN sub unit identification.
    /// </summary>
    IacCinSubUnitIdentification = 22021,

    /// <summary>
    ///   The carrier SCAC.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    CarrierScac = 22022,

    /// <summary>
    ///   The supplier VAT number.
    /// </summary>
    SupplierVatNumber = 22023,

    /// <summary>
    ///   The customer VAT number.
    /// </summary>
    CustomerVatNumber = 22024,

    /// <summary>
    ///   NCAGE/CAGE manufacturer code.
    /// </summary>
    NcageCageManufacturerCode = 22025,

    /// <summary>
    ///   The work order number.
    /// </summary>
    WorkOrderNumber = 23000,

    /// <summary>
    ///   The operation sequence number.
    /// </summary>
    OperationSequenceNumber = 23001,

    /// <summary>
    ///   The operation code.
    /// </summary>
    OperationCode = 23002,

    /// <summary>
    ///   The work order and operation sequence number.
    /// </summary>
    WorkOrderAndOperationSequenceNumber = 23003,

    /// <summary>
    ///   The status code.
    /// </summary>
    StatusCode = 23004,

    /// <summary>
    ///   The work unit code.
    /// </summary>
    WorkUnitCode = 23005,

    /// <summary>
    ///   The nomenclature.
    /// </summary>
    Nomenclature = 23006,

    /// <summary>
    ///   The form control number.
    /// </summary>
    FormControlNumber = 23010,

    /// <summary>
    ///   The quality assurance inspector.
    /// </summary>
    QualityAssuranceInspector = 23011,

    /// <summary>
    ///   The telephone number.
    /// </summary>
    TelephoneNumber = 23012,

    /// <summary>
    ///   The customer supplier.
    /// </summary>
    CustomerSupplier = 26000,

    /// <summary>
    ///   The carrier supplier.
    /// </summary>
    CarrierSupplier = 26001,

    /// <summary>
    ///   The customer carrier.
    /// </summary>
    CustomerCarrier = 26002,

    /// <summary>
    ///   The free text.
    /// </summary>
    FreeText = 26003,

    /// <summary>
    ///   The carrier trading partner.
    /// </summary>
    CarrierTradingPartner = 26004,

    /// <summary>
    ///   The header.
    /// </summary>
    Header = 26010,

    /// <summary>
    ///   The line 1.
    /// </summary>
    Line1 = 26011,

    /// <summary>
    ///   The line 2.
    /// </summary>
    Line2 = 26012,

    /// <summary>
    ///   The line 3.
    /// </summary>
    Line3 = 26013,

    /// <summary>
    ///   The line 4.
    /// </summary>
    Line4 = 26014,

    /// <summary>
    ///   The line 5.
    /// </summary>
    Line5 = 26015,

    /// <summary>
    ///   The line 6.
    /// </summary>
    Line6 = 26016,

    /// <summary>
    ///   The line 7.
    /// </summary>
    Line7 = 26017,

    /// <summary>
    ///   The line 8.
    /// </summary>
    Line8 = 26018,

    /// <summary>
    ///   The line 9.
    /// </summary>
    Line9 = 26019,

    /// <summary>
    ///   The line 10.
    /// </summary>
    Line10 = 26020,

    /// <summary>
    ///   The line 11.
    /// </summary>
    Line11 = 26021,

    /// <summary>
    ///   The line 12.
    /// </summary>
    Line12 = 26022,

    /// <summary>
    ///   The line 13.
    /// </summary>
    Line13 = 26023,

    /// <summary>
    ///   The line 14.
    /// </summary>
    Line14 = 26024,

    /// <summary>
    ///   The line 15.
    /// </summary>
    Line15 = 26025,

    /// <summary>
    ///   The line 16.
    /// </summary>
    Line16 = 26026,

    /// <summary>
    ///   The line 17.
    /// </summary>
    Line17 = 26027,

    /// <summary>
    ///   The line 18.
    /// </summary>
    Line18 = 26028,

    /// <summary>
    ///   The line 19.
    /// </summary>
    Line19 = 26029,

    /// <summary>
    ///   The line 20.
    /// </summary>
    Line20 = 26030,

    /// <summary>
    ///   The line 21.
    /// </summary>
    Line21 = 26031,

    /// <summary>
    ///   The line 22.
    /// </summary>
    Line22 = 26032,

    /// <summary>
    ///   The line 23.
    /// </summary>
    Line23 = 26033,

    /// <summary>
    ///   The line 24.
    /// </summary>
    Line24 = 26034,

    /// <summary>
    ///   The line 25.
    /// </summary>
    Line25 = 26035,

    /// <summary>
    ///   The line 26.
    /// </summary>
    Line26 = 26036,

    /// <summary>
    ///   The line 27.
    /// </summary>
    Line27 = 26037,

    /// <summary>
    ///   The line 28.
    /// </summary>
    Line28 = 26038,

    /// <summary>
    ///   The line 29.
    /// </summary>
    Line29 = 26039,

    /// <summary>
    ///   The line 30.
    /// </summary>
    Line30 = 26040,

    /// <summary>
    ///   The line 31.
    /// </summary>
    Line31 = 26041,

    /// <summary>
    ///   The line 32.
    /// </summary>
    Line32 = 26042,

    /// <summary>
    ///   The line 33.
    /// </summary>
    Line33 = 26043,

    /// <summary>
    ///   The line 34.
    /// </summary>
    Line34 = 26044,

    /// <summary>
    ///   The line 35.
    /// </summary>
    Line35 = 26045,

    /// <summary>
    ///   The line 36.
    /// </summary>
    Line36 = 26046,

    /// <summary>
    ///   The line 37.
    /// </summary>
    Line37 = 26047,

    /// <summary>
    ///   The line 38.
    /// </summary>
    Line38 = 26048,

    /// <summary>
    ///   The line 39.
    /// </summary>
    Line39 = 26049,

    /// <summary>
    ///   The line 40.
    /// </summary>
    Line40 = 26050,

    /// <summary>
    ///   The line 41.
    /// </summary>
    Line41 = 26051,

    /// <summary>
    ///   The line 42.
    /// </summary>
    Line42 = 26052,

    /// <summary>
    ///   The line 43.
    /// </summary>
    Line43 = 26053,

    /// <summary>
    ///   The line 44.
    /// </summary>
    Line44 = 26054,

    /// <summary>
    ///   The line 45.
    /// </summary>
    Line45 = 26055,

    /// <summary>
    ///   The line 46.
    /// </summary>
    Line46 = 26056,

    /// <summary>
    ///   The line 47.
    /// </summary>
    Line47 = 26057,

    /// <summary>
    ///   The line 48.
    /// </summary>
    Line48 = 26058,

    /// <summary>
    ///   The line 49.
    /// </summary>
    Line49 = 26059,

    /// <summary>
    ///   The line 50.
    /// </summary>
    Line50 = 26060,

    /// <summary>
    ///   The line 51.
    /// </summary>
    Line51 = 26061,

    /// <summary>
    ///   The line 52.
    /// </summary>
    Line52 = 26062,

    /// <summary>
    ///   The line 53.
    /// </summary>
    Line53 = 26063,

    /// <summary>
    ///   The line 54.
    /// </summary>
    Line54 = 26064,

    /// <summary>
    ///   The line 55.
    /// </summary>
    Line55 = 26065,

    /// <summary>
    ///   The line 56.
    /// </summary>
    Line56 = 26066,

    /// <summary>
    ///   The line 57.
    /// </summary>
    Line57 = 26067,

    /// <summary>
    ///   The line 58.
    /// </summary>
    Line58 = 26068,

    /// <summary>
    ///   The line 59.
    /// </summary>
    Line59 = 26069,

    /// <summary>
    ///   The line 60.
    /// </summary>
    Line60 = 26070,

    /// <summary>
    ///   The line 61.
    /// </summary>
    Line61 = 26071,

    /// <summary>
    ///   The line 62.
    /// </summary>
    Line62 = 26072,

    /// <summary>
    ///   The line 63.
    /// </summary>
    Line63 = 26073,

    /// <summary>
    ///   The line 64.
    /// </summary>
    Line64 = 26074,

    /// <summary>
    ///   The line 65.
    /// </summary>
    Line65 = 26075,

    /// <summary>
    ///   The line 66.
    /// </summary>
    Line66 = 26076,

    /// <summary>
    ///   The line 67.
    /// </summary>
    Line67 = 26077,

    /// <summary>
    ///   The line 68.
    /// </summary>
    Line68 = 26078,

    /// <summary>
    ///   The line 69.
    /// </summary>
    Line69 = 26079,

    /// <summary>
    ///   The line 70.
    /// </summary>
    Line70 = 26080,

    /// <summary>
    ///   The line 71.
    /// </summary>
    Line71 = 26081,

    /// <summary>
    ///   The line 72.
    /// </summary>
    Line72 = 26082,

    /// <summary>
    ///   The line 73.
    /// </summary>
    Line73 = 26083,

    /// <summary>
    ///   The line 74.
    /// </summary>
    Line74 = 26084,

    /// <summary>
    ///   The line 75.
    /// </summary>
    Line75 = 26085,

    /// <summary>
    ///   The line 76.
    /// </summary>
    Line76 = 26086,

    /// <summary>
    ///   The line 77.
    /// </summary>
    Line77 = 26087,

    /// <summary>
    ///   The line 78.
    /// </summary>
    Line78 = 26088,

    /// <summary>
    ///   The line 79.
    /// </summary>
    Line79 = 26089,

    /// <summary>
    ///   The line 80.
    /// </summary>
    Line80 = 26090,

    /// <summary>
    ///   The line 81.
    /// </summary>
    Line81 = 26091,

    /// <summary>
    ///   The line 82.
    /// </summary>
    Line82 = 26092,

    /// <summary>
    ///   The line 83.
    /// </summary>
    Line83 = 26093,

    /// <summary>
    ///   The line 84.
    /// </summary>
    Line84 = 26094,

    /// <summary>
    ///   The line 85.
    /// </summary>
    Line85 = 26095,

    /// <summary>
    ///   The line 86.
    /// </summary>
    Line86 = 26096,

    /// <summary>
    ///   The line 87.
    /// </summary>
    Line87 = 26097,

    /// <summary>
    ///   The line 88.
    /// </summary>
    Line88 = 26098,

    /// <summary>
    ///   The line 89.
    /// </summary>
    Line89 = 26099
}