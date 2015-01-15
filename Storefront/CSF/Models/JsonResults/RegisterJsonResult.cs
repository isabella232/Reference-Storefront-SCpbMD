﻿//-----------------------------------------------------------------------
// <copyright file="RegisterJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Emits the Json result of a Register request.</summary>
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
    using Sitecore.Commerce.Services.Customers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the RegisterJsonResult class.
    /// </summary>
    public class RegisterJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterJsonResult"/> class.
        /// </summary>
        public RegisterJsonResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public RegisterJsonResult(CreateUserResult result)
        {
            this.SetResult(result);
        }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Sets the result.
        /// </summary>
        /// <param name="result">The result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void SetResult(CreateUserResult result)
        {
            if (result.CommerceUser != null)
            {
                this.UserName = result.CommerceUser.UserName;
            }

            this.SetErrors(result);
        }
    }
}