﻿//-----------------------------------------------------------------------
// <copyright file="GetShippingMethodsInputModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2014
// </copyright>
// <summary>Controller parameters required to get the shipping methods.</summary>
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

namespace Sitecore.Commerce.Storefront.Models.InputModels
{
    using Sitecore.Commerce.Storefront.Models.InputModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetShippingMethodsInputModel class.
    /// </summary>
    public class GetShippingMethodsInputModel : BaseInputModel
    {
        /// <summary>
        /// Gets or sets the type of the shipping preference.
        /// </summary>
        /// <value>
        /// The type of the shipping preference.
        /// </value>
        public string ShippingPreferenceType { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        /// <value>
        /// The shipping address.
        /// </value>
        public PartyInputModelItem ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<CartLineInputModelItem> Lines { get; set; }
    }
}