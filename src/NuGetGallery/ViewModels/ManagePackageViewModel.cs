﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using NuGet.Services.Entities;
using NuGet.Versioning;

namespace NuGetGallery
{
    public class ManagePackageViewModel : DisplayPackageViewModel
    {
        public bool IsCurrentUserAnAdmin;

        public ManagePackageViewModel(Package package, User currentUser, IReadOnlyList<ReportPackageReason> reasons, UrlHelper url, string readMe)
            : base(
                  package, 
                  currentUser, 
                  package.PackageRegistration.Packages.OrderByDescending(p => new NuGetVersion(p.Version)))
        {
            IsCurrentUserAnAdmin = currentUser != null && currentUser.IsAdministrator;
            
            DeletePackagesRequest = new DeletePackagesRequest
            {
                Packages = new List<string>
                {
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}|{1}",
                        package.PackageRegistration.Id,
                        package.Version)
                },
                ReasonChoices = reasons
            };

            IsLocked = package.PackageRegistration.IsLocked;

            var versionSelectPackages = PackageVersions.Where(p => !p.Deleted);
            var manageTemplate = url.ManagePackageTemplate();
            VersionSelectList = new SelectList(PackageVersions.Where(p => !p.Deleted).Select(p => new
            {
                value = p.Version,
                text = p.FullVersion + (p.LatestVersionSemVer2 ? " (Latest)" : string.Empty),
                url = manageTemplate.Resolve(p)
            }), "url", "text", manageTemplate.Resolve(this));

            // Update edit model with the readme.md data.
            ReadMe = new EditPackageVersionReadMeRequest();
            if (package.HasReadMe)
            {
                ReadMe.ReadMe.SourceType = ReadMeService.TypeWritten;
                ReadMe.ReadMe.SourceText = readMe;
            }
        }

        public SelectList VersionSelectList { get; set; }

        public DeletePackagesRequest DeletePackagesRequest { get; set; }

        public bool IsLocked { get; }

        public EditPackageVersionReadMeRequest ReadMe { get; set; }
    }
}