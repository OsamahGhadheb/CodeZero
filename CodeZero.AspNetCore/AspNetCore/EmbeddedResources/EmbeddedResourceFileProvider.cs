﻿//  <copyright file="EmbeddedResourceFileProvider.cs" project="CodeZero.AspNetCore" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System;
using CodeZero.Dependency;
using CodeZero.Resources.Embedded;
using CodeZero.Web.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.IO;

namespace CodeZero.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private readonly IIocResolver _iocResolver;
        private readonly Lazy<IEmbeddedResourceManager> _embeddedResourceManager;
        private readonly Lazy<IWebEmbeddedResourcesConfiguration> _configuration;
        private bool _isInitialized;

        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
                iocResolver.Resolve<IEmbeddedResourceManager>,
                true
            );

            _configuration = new Lazy<IWebEmbeddedResourcesConfiguration>(
                iocResolver.Resolve<IWebEmbeddedResourcesConfiguration>,
                true
            );
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!IsInitialized())
            {
                return new NotFoundFileInfo(subpath);
            }

            var filename = Path.GetFileName(subpath);
            var resource = _embeddedResourceManager.Value.GetResource(subpath);

            if (resource == null || IsIgnoredFile(resource))
            {
                return new NotFoundFileInfo(subpath);
            }

            return new EmbeddedResourceItemFileInfo(resource, filename);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!IsInitialized())
            {
                return new NotFoundDirectoryContents();
            }

            // The file name is assumed to be the remainder of the resource name. 
            if (subpath == null)
            {
                return new NotFoundDirectoryContents();
            }

            var resources = _embeddedResourceManager.Value.GetResources(subpath);
            return new EmbeddedResourceItemDirectoryContents(resources
                .Where(r=> !IsIgnoredFile(r))
                .Select(r=> new EmbeddedResourceItemFileInfo(r, r.FileName.Substring(subpath.Length-1))));
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        protected virtual bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && _configuration.Value.IgnoredFileExtensions.Contains(resource.FileExtension);
        }

        private bool IsInitialized()
        {
            if (_isInitialized)
            {
                return true;
            }

            _isInitialized = _iocResolver.IsRegistered<IEmbeddedResourceManager>();

            return _isInitialized;
        }
    }
}