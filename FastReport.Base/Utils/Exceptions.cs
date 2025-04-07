using System;
using System.Collections.Generic;
using System.IO;

namespace FastReport.Utils
{
    /// <summary>
    /// The exception that is thrown when the user tried to set object's name that is already exists.
    /// </summary>
    public class DuplicateNameException : Exception
    {
        internal DuplicateNameException(string name)
          : base(String.Format(Res.Get("Messages,DuplicateName"), name))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the user tried to rename an object that is introduced in the ancestor report.
    /// </summary>
    public class AncestorException : Exception
    {
        internal AncestorException(string name)
          : base(String.Format(Res.Get("Messages,RenameAncestor"), name))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the user tried to rename an object that is introduced in the ancestor report.
    /// </summary>
    public class SwissQrCodeException : Exception
    {
        internal SwissQrCodeException()
        {
        }

        internal SwissQrCodeException(string message)
            : base(message)
        {
        }

        internal SwissQrCodeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when loading bad formed xml report file.
    /// </summary>
    public class FileFormatException : Exception
    {
        internal FileFormatException()
          : base(Res.Get("Messages,WrongFileFormat"))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when loading an encrypted report with wrong password.
    /// </summary>
    public class DecryptException : Exception
    {
        internal DecryptException()
          : base(Res.Get("Messages,DecryptError"))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown if there is an error in the report's script code.
    /// </summary>
    public class CompilerException : Exception
    {
        /// <summary>
        /// Contains a list of errors.
        /// </summary>
        public Info[] Errors { get; }
        
        /// <summary>
        /// Creates a new instance of CompilerException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="errors">Array of error info items.</param>
        public CompilerException(string message, Info[] errors = null) : base(message)
        {
            Errors = errors;
        }

        /// <summary>
        /// The exception info.
        /// </summary>
        public class Info
        {
            /// <summary>
            /// Line number.
            /// </summary>
            public int Line { get; }
            
            /// <summary>
            /// Column number.
            /// </summary>
            public int Column { get; }
            
            /// <summary>
            /// Report object in which exception occurs.
            /// </summary>
            public string ReportObject { get; }
            
            /// <summary>
            /// Error message.
            /// </summary>
            public string Message { get; }

            /// <summary>
            /// Creates a new instance of Info class.
            /// </summary>
            /// <param name="line">Line number.</param>
            /// <param name="column">Column number.</param>
            /// <param name="reportObject">Name of report object in which an error occured.</param>
            /// <param name="message">Error message.</param>
            public Info(int line, int column, string reportObject, string message)
            {
                Line = line;
                Column = column;
                ReportObject = reportObject;
                Message = message;
            }
        }
    }

    /// <summary>
    /// The exception that is thrown when trying to set an object's <b>Parent</b> property to 
    /// an object that not accepts children of this type.
    /// </summary>
    public class ParentException : Exception
    {
        internal ParentException(Base parent, Base child)
          : base(String.Format(Res.Get("Messages,ParentError"), parent.GetType().Name, child.GetType().Name))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when trying to load a report file that contains reference to an 
    /// unknown object type.
    /// </summary>
    public class ClassException : Exception
    {
        internal ClassException(string name)
          : base(Res.Get("Messages,CantFindObject") + " " + name)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when initializing a table datasource which
    /// <b>TableName</b> or <b>Alias</b> is not set properly.
    /// </summary>
    public class DataTableException : Exception
    {
        internal DataTableException(string alias)
          : base(alias + ": " + Res.Get("Messages,TableIsNull"))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when trying to access a row of a datasource that is not initialized yet.
    /// </summary>
    public class DataNotInitializedException : Exception
    {
        internal DataNotInitializedException(string alias)
          : base(alias + ": " + Res.Get("Messages,DataNotInitialized"))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown if an error occurs in the <b>TableObject.ManualBuild</b> event.
    /// </summary>
    public class TableManualBuildException : Exception
    {
        internal TableManualBuildException()
          : base(Res.Get("Messages,TableManualBuildError"))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown if an error occurs in the <b>MatrixObject.ManualBuild</b> event.
    /// </summary>
    public class MatrixValueException : Exception
    {
        internal MatrixValueException(int count)
          : base(String.Format(Res.Get("Messages,MatrixValueError"), count))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown if a report object's Name property is set to wrong value.
    /// </summary>
    public class NotValidIdentifierException : Exception
    {
        internal NotValidIdentifierException(string value)
          : base(String.Format("'{0}' is not valid identifier name", value))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown if an unknown value is supplied to some methods dealing with totals, variables etc.
    /// </summary>
    public class UnknownNameException : Exception
    {
        internal UnknownNameException(string value)
          : base(String.Format("Unknown name '{0}'", value))
        {
        }
    }

    /// <summary>
    /// <see cref="FastReport.Cloud.StorageClient.CloudStorageClient"/> throws this exception if an error occurs in the <b>SaveReport</b> method.
    /// See inner exception for detailed information.
    /// </summary>
    public class CloudStorageException : Exception
    {
        internal CloudStorageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the Group Header has no group condition.
    /// </summary>
    public class GroupHeaderHasNoGroupCondition : Exception
    {
        internal GroupHeaderHasNoGroupCondition(string name)
          : base(String.Format(Res.Get("Messages,GroupHeaderHasNoGroupCondition"), name))
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the image cannot be loaded.
    /// </summary>
    public class ImageLoadException : FileLoadException
    {
        internal ImageLoadException(Exception ex)
          : base(Res.Get("Messages,ImageLoadException"), ex)
        {
        }
    }
}