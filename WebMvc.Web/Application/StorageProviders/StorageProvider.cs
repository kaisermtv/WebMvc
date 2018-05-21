using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Domain.Constants;
using WebMvc.Domain.Interfaces.Providers;

namespace WebMvc.Web.Application.StorageProviders
{
    public static class StorageProvider
    {
        private static readonly Lazy<IStorageProvider> CurrentStorageProvider = new Lazy<IStorageProvider>(() =>
        {
            var type = SiteConstants.Instance.StorageProviderType;
            if (string.IsNullOrEmpty(type))
            {
                return new DiskStorageProvider();
            }

            try
            {
                return TypeFactory.GetInstanceOf<IStorageProvider>(type);
            }
            catch (Exception)
            {
                return new DiskStorageProvider();
            }
        });

        public static IStorageProvider Current => CurrentStorageProvider.Value;
    }
}