﻿//-----------------------------------------------------------------------
// <copyright file="AddressItemJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AddressItemJsonResult class.</summary>
//-----------------------------------------------------------------------
// Copyright 2015 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Storefront.Models.JsonResults
{
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connect.DynamicsRetail.Entities;
    using Sitecore.Commerce.Storefront.Managers;

    /// <summary>
    /// Json result for party operations.
    /// </summary>
    public class AddressItemJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressItemJsonResult" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public AddressItemJsonResult(CommerceParty address)
        {
            Assert.ArgumentNotNull(address, "address");

            this.ExternalId = address.ExternalId;
            this.Name = address.Name;
            this.Address1 = address.Address1;
            this.City = address.City;
            this.State = address.State;
            this.ZipPostalCode = address.ZipPostalCode;
            this.Country = address.Country;
            this.IsPrimary = address.IsPrimary;
            this.FullAddress = string.Concat(address.Address1, ",", address.City, ",", address.State, ",", address.ZipPostalCode, ",", address.Country);
            this.DetailsUrl = string.Concat(StorefrontManager.StorefrontUri("/accountmanagement/addressbook"), "?id=", address.ExternalId);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        /// <value>
        /// The address1.
        /// </value>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>
        /// The postal code.
        /// </value>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the full address.
        /// </summary>
        /// <value>
        /// The full address.
        /// </value>
        public string FullAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the details URL.
        /// </summary>
        /// <value>
        /// The details URL.
        /// </value>
        public string DetailsUrl { get; set; }
    }
}