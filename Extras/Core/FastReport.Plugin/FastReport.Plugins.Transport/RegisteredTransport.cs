using System;
using System.Collections.Generic;

namespace FastReport.Cloud.Transports
{
    /// <summary>
    /// Provides information about registered transport types for cloud storage providers
    /// </summary>
    public static class RegisteredTransport
    {
        /// <summary>
        /// Defines the available transport types for cloud storage
        /// </summary>
        [Flags]
        public enum TransportType
        {
            /// <summary>No transports selected</summary>
            None = 0,

            /// <summary>Google Drive transport</summary>
            GoogleDrive = 1,

            /// <summary>Yandex Disk transport</summary>
            YandexDisk = 2,

            /// <summary>Amazon S3 transport</summary>
            AmazonS3 = 4,

            /// <summary>One Drive transport</summary>
            OneDrive = 8,

            /// <summary>NextCloud Files transport</summary>
            NextCloud = 16,

            /// <summary>All available transports</summary>
            All = GoogleDrive | YandexDisk | AmazonS3 | OneDrive | NextCloud
        }
        static bool needRefresh = true;
        static Dictionary<string, Type> transportList;
        static TransportType availableTransports = TransportType.All;

        /// <summary>
        /// Gets or sets the flag determining which transport types are available
        /// Default is all transports
        /// </summary>
        public static TransportType AvailableTransports
        {
            get { return availableTransports; }
            set
            {
                availableTransports = value;
                needRefresh = true;
            }
        }

        /// <summary>
        /// Gets the dictionary of available transports, where the key is the transport name
        /// and the value is the corresponding type
        /// </summary>
        public static Dictionary<string, Type> AvailableTransportList
        {
            get
            {
                if (transportList == null || needRefresh)
                {
                    transportList = new Dictionary<string, Type>();
                    if (AvailableTransports.HasFlag(TransportType.GoogleDrive))
                        transportList.Add("GoogleDrive", typeof(Transport.GoogleDrive.GoogleDriveTransport));
                    if (AvailableTransports.HasFlag(TransportType.YandexDisk))
                        transportList.Add("YandexDisk", typeof(Transport.YandexDisk.YandexDiskTransport));
                    if (AvailableTransports.HasFlag(TransportType.AmazonS3))
                        transportList.Add("AmazonS3", typeof(Transport.AmazonS3.AmazonS3Transport));
                    if (AvailableTransports.HasFlag(TransportType.OneDrive))
                        transportList.Add("OneDrive", typeof(Transport.OneDrive.OneDriveTransport));
                    if (AvailableTransports.HasFlag(TransportType.NextCloud))
                        transportList.Add("NextCloud", typeof(Transport.NextCloud.NextCloudTransport));
                    needRefresh = false;
                }

                return transportList;
            }
        }
    }
}