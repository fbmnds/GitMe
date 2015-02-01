﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;


using GitMe.Schema;

namespace GitMe.Common
{
    public static class FileIOHelper
    {
        public static async Task<dynamic> GetStorageFromLocalFileAsync(string folder, string storageType)
        {
            try
            {
                string storageText = await FileIOHelper.ReadLocalFileAsync(folder, String.Format("{0}.json", storageType));
                // returns null, if deserialisation to storage returned an empty collection!
                return JsonHelper.DeserializeToStorage(storageType, storageText);
            }
            catch 
            { 
                // ignore
            }
            return null;
        }

        public static async Task<string> ReadLocalFileAsync(string folderName, string fileName)
        {
            var storageFile = await GetLocalStorageFileAsync(folderName, 
                                                            fileName, 
                                                            CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(storageFile);
        }

        public static async Task<StorageFile> GetLocalStorageFileAsync(string folderName,
                                                               string fileName,
                                                               CreationCollisionOption option)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var dataFolder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile storageFile = await dataFolder.CreateFileAsync(fileName, option);
            return storageFile;
        }

        public static async Task<StorageFile> GetSDStorageFileAsync(string folderName,
                                                                    string fileName,
                                                                    CreationCollisionOption option)
        {
            var devices = Windows.Storage.KnownFolders.RemovableDevices;
            var sdCards = await devices.GetFoldersAsync();
            var firstCard = sdCards[0];
            StorageFolder folder = await firstCard.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFile storageFile = await folder.CreateFileAsync(fileName, option);
            return storageFile;
        }

        public static async Task WriteUtf8ToLocalFileAsync(string folderName, string fileName, string text)
        {
            var storageFile = await GetLocalStorageFileAsync(folderName,
                                                             fileName,
                                                             CreationCollisionOption.ReplaceExisting);
            var utf8 = new Windows.Storage.Streams.UnicodeEncoding();
            await FileIO.WriteTextAsync(storageFile, text, utf8);
        }

        public static async Task WriteUtf8ToSDAsync(string folderName, string fileName, string text)
        {
            try
            {
                StorageFile file = await GetSDStorageFileAsync(folderName,
                                               fileName,
                                               CreationCollisionOption.ReplaceExisting);
                var utf8 = new Windows.Storage.Streams.UnicodeEncoding();
                await FileIO.WriteTextAsync(file, text, utf8);
            }
            catch
            {
                // ignore
                // Note: receiving a permission denied exception
                // because the file on the SD card was generated by a previous GitMe version?
            }
        }

    }
}