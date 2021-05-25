﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace BooksPlant.Utilities
{
    public class PhysicalFileProvider2 : IFileProvider
    {
        private string root;
        private PhysicalFileProvider physicalFileProvider;

        public PhysicalFileProvider2(string root)
        {
            this.root = root;
        }

        private PhysicalFileProvider GetPhysicalFileProvider()
        {
            if (!File.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            if (physicalFileProvider == null)
            {
                physicalFileProvider = new PhysicalFileProvider(root);
            }
            return physicalFileProvider;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return GetPhysicalFileProvider().GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return GetPhysicalFileProvider().GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return GetPhysicalFileProvider().Watch(filter);
        }

        IChangeToken IFileProvider.Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
