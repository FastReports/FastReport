using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace FastReport.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ZipArchive
    {
        string rootFolder;
        string errors;
        List<ZipFileItem> fileList;
        List<ZipLocalFile> fileObjects;
        string comment;

        private uint CopyStream(Stream source, Stream target)
        {
            source.Position = 0;
            int bufflength = 8192;
            uint crc = Crc32.Begin();
            byte[] buff = new byte[bufflength];            
            int i;
            while ((i = source.Read(buff, 0, bufflength)) > 0)
            {
                target.Write(buff, 0, i);
                crc = Crc32.Update(crc, buff, 0, i);
            }
            return Crc32.End(crc);
        }

        /// <summary>
        /// Clear all files in archive.
        /// </summary>
        public void Clear()        
        {
            foreach (ZipFileItem item in fileList)
                item.Clear();
            foreach (ZipLocalFile item in fileObjects)
                item.Clear();
            fileList.Clear();
            fileObjects.Clear();            
            errors = "";
            rootFolder = "";
            comment = "";
        }

        /// <summary>
        /// Check for exisiting file in archive.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool FileExists(string FileName)
        {
            foreach(ZipFileItem item in fileList)
            {
                if (item.Name == FileName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the file form disk to the archive.
        /// </summary>
        /// <param name="FileName"></param>
        public void AddFile(string FileName)
        {
            if (!FileExists(FileName)) // check for exisiting file in archive
            {
                if (File.Exists(FileName))
                {
                    fileList.Add(new ZipFileItem(FileName));
                    if (rootFolder == String.Empty)
                        rootFolder = Path.GetDirectoryName(FileName);
                }
                else
                    errors += "File " + FileName + " not found\r";
            }
        }

        /// <summary>
        /// Adds all files from directory (recursive) on the disk to the archive.
        /// </summary>
        /// <param name="DirName"></param>
        public void AddDir(string DirName)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(DirName));
            foreach (string file in files)
            {
                if ((File.GetAttributes(file) & FileAttributes.Hidden) != 0)
                    continue;
                AddFile(file);
            }

            List<string> folders = new List<string>();
            folders.AddRange(Directory.GetDirectories(DirName));
            foreach (string folder in folders)
            {
                if ((File.GetAttributes(folder) & FileAttributes.Hidden) != 0)
                    continue;
                AddDir(folder);
            }            
        }

        /// <summary>
        /// Adds the stream to the archive.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        public void AddStream(string fileName, Stream stream)
        {
            if (!FileExists(fileName)) // check for exisiting file in archive
                fileList.Add(new ZipFileItem(fileName, stream));
        }

        private void AddStreamToZip(Stream stream, ZipLocalFile ZipFile)
        {
            if (stream.Length > 128)
            {
                using (DeflateStream deflate = new DeflateStream(ZipFile.FileData, CompressionMode.Compress, true))
                    ZipFile.LocalFileHeader.Crc32 = CopyStream(stream, deflate);
                ZipFile.LocalFileHeader.CompressionMethod = 8;
            }
            else
            {
                ZipFile.LocalFileHeader.Crc32 = CopyStream(stream, ZipFile.FileData);
                ZipFile.LocalFileHeader.CompressionMethod = 0;
            }
            ZipFile.LocalFileHeader.CompressedSize = (uint)ZipFile.FileData.Length;
            ZipFile.LocalFileHeader.UnCompressedSize = (uint)stream.Length;
        }

        /// <summary>
        /// Creates the zip and writes it to rhe Stream
        /// </summary>
        /// <param name="Stream"></param>
        public void SaveToStream(Stream Stream)
        {
            ZipLocalFile ZipFile;
            ZipCentralDirectory ZipDir;
            ZipFileHeader ZipFileHeader;
            long CentralStartPos, CentralEndPos;

            for (int i = 0; i < fileList.Count; i++)
            {
                ZipFile = new ZipLocalFile();
                using (ZipFile.FileData = new MemoryStream())
                {
                    if (fileList[i].Disk)
                    {
                        ZipFile.LocalFileHeader.FileName = fileList[i].Name.Replace(rootFolder + Path.DirectorySeparatorChar, "");
                        using (FileStream file = new FileStream(fileList[i].Name, FileMode.Open))
                            AddStreamToZip(file, ZipFile);
                    }
                    else
                    {
                        ZipFile.LocalFileHeader.FileName = fileList[i].Name;
                        fileList[i].Stream.Position = 0;
                        AddStreamToZip(fileList[i].Stream, ZipFile);
                    }
                    ZipFile.Offset = (uint)Stream.Position;
                    ZipFile.LocalFileHeader.LastModFileDate = fileList[i].FileDateTime;
                    ZipFile.SaveToStream(Stream);
                }
                ZipFile.FileData = null;
                fileObjects.Add(ZipFile);
            }
            
            CentralStartPos = Stream.Position;
            for (int i = 0; i < fileObjects.Count; i++)
            {
                ZipFile = fileObjects[i];
                ZipFileHeader = new ZipFileHeader();
                ZipFileHeader.CompressionMethod = ZipFile.LocalFileHeader.CompressionMethod;
                ZipFileHeader.LastModFileDate = ZipFile.LocalFileHeader.LastModFileDate;
                ZipFileHeader.GeneralPurpose = ZipFile.LocalFileHeader.GeneralPurpose;
                ZipFileHeader.Crc32 = ZipFile.LocalFileHeader.Crc32;
                ZipFileHeader.CompressedSize = ZipFile.LocalFileHeader.CompressedSize;
                ZipFileHeader.UnCompressedSize = ZipFile.LocalFileHeader.UnCompressedSize;
                ZipFileHeader.RelativeOffsetLocalHeader = ZipFile.Offset;
                ZipFileHeader.FileName = ZipFile.LocalFileHeader.FileName;
                ZipFileHeader.SaveToStream(Stream);
            }
            CentralEndPos = Stream.Position;
            ZipDir = new ZipCentralDirectory();
            ZipDir.TotalOfEntriesCentralDirOnDisk = (ushort)fileList.Count;
            ZipDir.TotalOfEntriesCentralDir = (ushort)fileList.Count;
            ZipDir.SizeOfCentralDir = (uint)(CentralEndPos - CentralStartPos);
            ZipDir.OffsetStartingDiskDir = (uint)CentralStartPos;
            ZipDir.SaveToStream(Stream);
        }

        /// <summary>
        /// Creates the ZIP archive and writes it to the file.
        /// </summary>
        /// <param name="FileName"></param>
        public void SaveToFile(string FileName)
        {
            using (FileStream file = new FileStream(FileName, FileMode.Create))
                SaveToStream(file);
        }

        /// <summary>
        /// Gets or sets the Root Folder.
        /// </summary>
        public string RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public string Errors
        {
            get { return errors; }
            set { errors = value; }
        }

        /// <summary>
        /// Gets or sets the commentary to the archive.
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        /// <summary>
        /// Gets count of files in archive.
        /// </summary>
        public int FileCount
        {
            get { return fileList.Count; }
        }

        /// <summary>
        /// Creates the new zip archive.
        /// </summary>
        public ZipArchive()
        {
            fileList = new List<ZipFileItem>();
            fileObjects = new List<ZipLocalFile>();
            Clear();
        }                
    }

    internal class ZipFileItem
    {
        private string name;
        private Stream stream;
        private bool disk;
        private uint fileDateTime;

        private uint GetDosDateTime(DateTime date)
        {
            return (uint)(
                ((date.Year - 1980 & 0x7f) << 25) |
                ((date.Month & 0xF) << 21) |
                ((date.Day & 0x1F) << 16) |
                ((date.Hour & 0x1F) << 11) |
                ((date.Minute & 0x3F) << 5) |
                (date.Second >> 1));
        }        

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Stream Stream
        {
            get { return stream; }
        }

        public bool Disk
        {
            get { return disk; }
            set { disk = value; }
        }

        public uint FileDateTime
        {
            get { return fileDateTime; }
            set { fileDateTime = value; }
        }

        public void Clear()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        public ZipFileItem()
        {
            stream = new MemoryStream();
            fileDateTime = GetDosDateTime(SystemFake.DateTime.Now);
            disk = false;
        }

        public ZipFileItem(string fileName, Stream stream)
        {
            this.stream = stream;
            name = fileName;
            fileDateTime = GetDosDateTime(SystemFake.DateTime.Now);
            disk = false;
        }

        public ZipFileItem(string fileName)
        {
            name = fileName;
            fileDateTime = GetDosDateTime(File.GetLastWriteTime(fileName));
            disk = true;
        }
    }

    internal class ZipLocalFileHeader
    {
        private uint  localFileHeaderSignature;
        private ushort version;
        private ushort generalPurpose;
        private ushort compressionMethod;
        private uint crc32;
        private uint lastModFileDate;
        private uint compressedSize;
        private uint unCompressedSize;
        private string extraField;
        private string fileName;
        private ushort fileNameLength;
        private ushort extraFieldLength;

        public void SaveToStream(Stream Stream)
        {
            Stream.Write(BitConverter.GetBytes(localFileHeaderSignature), 0, 4);
            Stream.Write(BitConverter.GetBytes(version), 0, 2);
            Stream.Write(BitConverter.GetBytes(generalPurpose), 0, 2);
            Stream.Write(BitConverter.GetBytes(compressionMethod), 0, 2);
            Stream.Write(BitConverter.GetBytes(lastModFileDate), 0, 4);
            Stream.Write(BitConverter.GetBytes(crc32), 0, 4);
            Stream.Write(BitConverter.GetBytes(compressedSize), 0, 4);
            Stream.Write(BitConverter.GetBytes(unCompressedSize), 0, 4);
            Stream.Write(BitConverter.GetBytes(fileNameLength), 0, 2);
            Stream.Write(BitConverter.GetBytes(extraFieldLength), 0, 2);            
            if (fileNameLength > 0)
                Stream.Write(Converter.StringToByteArray(fileName), 0, fileNameLength);
            if (extraFieldLength > 0)
                Stream.Write(Converter.StringToByteArray(extraField), 0, extraFieldLength);
        }

        public uint LocalFileHeaderSignature
        {
            get { return localFileHeaderSignature; }
        }

        public ushort Version
        {
            get { return version; }
            set { version = value; }
            
        }

        public ushort GeneralPurpose
        {
            get { return generalPurpose; }
            set { generalPurpose = value; }
        }
    
        public ushort CompressionMethod
        {
            get { return compressionMethod; }
            set { compressionMethod =  value; }
        }

        public uint LastModFileDate
        {
            get { return lastModFileDate; }
            set { lastModFileDate = value; }
        }
    
        public uint Crc32
        {
            get { return crc32; }
            set { crc32 = value; }
        }

        public uint CompressedSize
        {
            get { return compressedSize; }
            set { compressedSize = value; }
        }

        public uint UnCompressedSize
        {
            get { return unCompressedSize; }
            set { unCompressedSize = value; }
        }

        public ushort FileNameLength
        {
            get { return fileNameLength; }
            set { fileNameLength = value; }
        }
        public ushort ExtraFieldLength
        {
            get { return extraFieldLength; }
            set { extraFieldLength = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set 
            {                
                fileName = value.Replace('\\', '/');
                fileNameLength = (ushort)value.Length;            
            }
        }

        public string ExtraField
        {
            get { return extraField; }
            set 
            {   
                extraField = value;
                extraFieldLength = (ushort)value.Length;
            }
        }

        // constructor
        public ZipLocalFileHeader()
        {
            localFileHeaderSignature = 0x04034b50;
            version = 20;
            generalPurpose = 0;
            compressionMethod = 0;
            crc32 = 0;
            lastModFileDate = 0;
            compressedSize = 0;
            unCompressedSize = 0;
            extraField = "";
            fileName = "";
            fileNameLength = 0;
            extraFieldLength = 0;
        }
    }

    internal class ZipCentralDirectory
    {
        private uint endOfChentralDirSignature;
        private ushort numberOfTheDisk;
        private ushort totalOfEntriesCentralDirOnDisk;
        private ushort numberOfTheDiskStartCentralDir;
        private ushort totalOfEntriesCentralDir;
        private uint sizeOfCentralDir;
        private uint offsetStartingDiskDir;
        private string comment;
        private ushort commentLength;

        public void SaveToStream(Stream Stream)
        {
            Stream.Write(BitConverter.GetBytes(endOfChentralDirSignature), 0, 4);
            Stream.Write(BitConverter.GetBytes(numberOfTheDisk), 0, 2);
            Stream.Write(BitConverter.GetBytes(numberOfTheDiskStartCentralDir), 0, 2);
            Stream.Write(BitConverter.GetBytes(totalOfEntriesCentralDirOnDisk), 0, 2);
            Stream.Write(BitConverter.GetBytes(totalOfEntriesCentralDir), 0, 2);
            Stream.Write(BitConverter.GetBytes(sizeOfCentralDir), 0, 4);
            Stream.Write(BitConverter.GetBytes(offsetStartingDiskDir), 0, 4);
            Stream.Write(BitConverter.GetBytes(commentLength), 0, 2);
            if (commentLength > 0)
                Stream.Write(Converter.StringToByteArray(comment), 0, commentLength);
        }

        public uint EndOfChentralDirSignature
        {
            get { return endOfChentralDirSignature; }
        }

        public ushort NumberOfTheDisk
        {
            get { return numberOfTheDisk; }
            set { numberOfTheDisk = value; }
        }

        public ushort NumberOfTheDiskStartCentralDir
        {
            get { return numberOfTheDiskStartCentralDir; }
            set { numberOfTheDiskStartCentralDir = value; }
        }
        
        public ushort TotalOfEntriesCentralDirOnDisk
        {
            get { return totalOfEntriesCentralDirOnDisk; }
            set { totalOfEntriesCentralDirOnDisk = value; }
        }
        
        public ushort TotalOfEntriesCentralDir
        {
            get { return totalOfEntriesCentralDir; }
            set { totalOfEntriesCentralDir = value; }
        }
        
        public uint SizeOfCentralDir
        {
            get { return sizeOfCentralDir; }
            set { sizeOfCentralDir = value; }
        }

        public uint OffsetStartingDiskDir
        {
            get { return offsetStartingDiskDir; }
            set { offsetStartingDiskDir = value; }
        }

        public ushort CommentLength
        {
            get { return commentLength; }
            set { commentLength = value; }
        }

        public string Comment
        {
            get { return comment; }
            set 
            {
                comment = value;
                commentLength = (ushort)value.Length;
            }
        }

        // constructor
        public ZipCentralDirectory()
        {
            endOfChentralDirSignature = 0x06054b50;
            numberOfTheDisk = 0;
            numberOfTheDiskStartCentralDir = 0;
            totalOfEntriesCentralDirOnDisk = 0;
            totalOfEntriesCentralDir = 0;
            sizeOfCentralDir = 0;
            offsetStartingDiskDir = 0;
            commentLength = 0;
            comment = "";
        }
    }

    internal class ZipFileHeader
    {
        private uint centralFileHeaderSignature;
        private uint relativeOffsetLocalHeader;
        private uint unCompressedSize;
        private uint compressedSize;
        private uint crc32;
        private uint externalFileAttribute;
        private string extraField;
        private string fileComment;
        private string fileName;
        private ushort compressionMethod;
        private ushort diskNumberStart;
        private uint lastModFileDate;
        private ushort versionMadeBy;
        private ushort generalPurpose;
        private ushort fileNameLength;
        private ushort internalFileAttribute;
        private ushort extraFieldLength;
        private ushort versionNeeded;
        private ushort fileCommentLength;

        public void SaveToStream(Stream Stream)
        {
            Stream.Write(BitConverter.GetBytes(centralFileHeaderSignature), 0, 4);
            Stream.Write(BitConverter.GetBytes(versionMadeBy), 0, 2);
            Stream.Write(BitConverter.GetBytes(versionNeeded), 0, 2);
            Stream.Write(BitConverter.GetBytes(generalPurpose), 0, 2);
            Stream.Write(BitConverter.GetBytes(compressionMethod), 0, 2);
            Stream.Write(BitConverter.GetBytes(lastModFileDate), 0, 4);
            Stream.Write(BitConverter.GetBytes(crc32), 0, 4);
            Stream.Write(BitConverter.GetBytes(compressedSize), 0, 4);
            Stream.Write(BitConverter.GetBytes(unCompressedSize), 0, 4);
            Stream.Write(BitConverter.GetBytes(fileNameLength), 0, 2);
            Stream.Write(BitConverter.GetBytes(extraFieldLength), 0, 2);
            Stream.Write(BitConverter.GetBytes(fileCommentLength), 0, 2);
            Stream.Write(BitConverter.GetBytes(diskNumberStart), 0, 2);
            Stream.Write(BitConverter.GetBytes(internalFileAttribute), 0, 2);
            Stream.Write(BitConverter.GetBytes(externalFileAttribute), 0, 4);
            Stream.Write(BitConverter.GetBytes(relativeOffsetLocalHeader), 0, 4);
            Stream.Write(Converter.StringToByteArray(fileName), 0, fileNameLength);
            Stream.Write(Converter.StringToByteArray(extraField), 0, extraFieldLength);
            Stream.Write(Converter.StringToByteArray(fileComment), 0, fileCommentLength);
        }

        public uint CentralFileHeaderSignature
        {
            get { return centralFileHeaderSignature; }
        }

        public ushort VersionMadeBy
        {
            get { return versionMadeBy; }
        }

        public ushort VersionNeeded
        {
            get { return versionNeeded; }
        }

        public ushort GeneralPurpose
        {
            get { return generalPurpose; }
            set { generalPurpose = value; }
        }

        public ushort CompressionMethod
        {
            get { return compressionMethod; }
            set { compressionMethod = value; }
        }

        public uint LastModFileDate
        {
            get { return lastModFileDate; }
            set { lastModFileDate = value; }
        }

        public uint Crc32
        {
            get { return crc32; }
            set { crc32 = value; }
        }

        public uint CompressedSize
        { 
            get { return compressedSize; }
            set { compressedSize = value; }
        }

        public uint UnCompressedSize
        {
            get { return unCompressedSize; }
            set { unCompressedSize =value; }
        }
        
        public ushort FileNameLength
        {
            get { return fileNameLength; }
            set { fileNameLength = value; }
        }

        public ushort ExtraFieldLength
        {
            get { return extraFieldLength; }
            set { extraFieldLength = value; }
        }

        public ushort FileCommentLength
        {
            get { return fileCommentLength; }
            set { fileCommentLength = value; }
        }

        public ushort DiskNumberStart
        {
            get { return diskNumberStart; }
            set { diskNumberStart = value; }
        }

        public ushort InternalFileAttribute
        {
            get { return internalFileAttribute; }
            set { internalFileAttribute = value; }
        }
        
        public uint ExternalFileAttribute
        {
            get { return externalFileAttribute; }
            set { externalFileAttribute = value; }
        }
        
        public uint RelativeOffsetLocalHeader
        {
            get { return relativeOffsetLocalHeader; }
            set { relativeOffsetLocalHeader = value; }
        }
        
        public string FileName
        {
            get { return fileName; }
            set 
            { 
                fileName = value.Replace('\\', '/');
                fileNameLength = (ushort)value.Length;
            }
        }
        
        public string ExtraField
        {
            get { return extraField; }
            set 
            { 
                extraField = value;
                extraFieldLength = (ushort)value.Length; 
            }
        }
        
        public string FileComment
        {
            get { return fileComment; }
            set
            {
                fileComment = value;
                fileCommentLength = (ushort)value.Length;
            }
        }

        // constructor
        public ZipFileHeader()
        {
            centralFileHeaderSignature = 0x02014b50;
            relativeOffsetLocalHeader = 0;
            unCompressedSize = 0;
            compressedSize = 0;
            crc32 = 0;
            externalFileAttribute = 0;
            extraField = "";
            fileComment = "";
            fileName = "";
            compressionMethod = 0;
            diskNumberStart = 0;
            lastModFileDate = 0;
            versionMadeBy = 20;
            generalPurpose = 0;
            fileNameLength = 0;
            internalFileAttribute = 0;
            extraFieldLength = 0;
            versionNeeded = 20;
            fileCommentLength = 0;
        }
    }

    internal class ZipLocalFile
    {
        ZipLocalFileHeader localFileHeader;
        MemoryStream fileData;
        uint offset;

        public void SaveToStream(Stream Stream)
        {
            localFileHeader.SaveToStream(Stream);
            fileData.Position = 0;
            fileData.WriteTo(Stream);
            fileData.Dispose();
            fileData = null;
        }

        public ZipLocalFileHeader LocalFileHeader
        {
            get { return localFileHeader; }
        }

        public MemoryStream FileData
        {
            get { return fileData; }
            set { fileData = value; }
        }

        public uint Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public void Clear()
        {
            if (fileData != null)
            {
                fileData.Dispose();
                fileData = null;
            }
        }

        // constructor
        public ZipLocalFile()
        {
            localFileHeader = new ZipLocalFileHeader();
            offset = 0;
        }
    }

}