using System;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
using FastReport.Code.CSharp;
#else
using System.CodeDom.Compiler;
using Microsoft.CSharp;
#endif
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using FastReport.Code;

namespace FastReport.Utils
{
    /// <summary>
    /// Specifies the behaviour of compiler when exception is thrown.
    /// </summary>
    public enum CompilerExceptionBehaviour
    {
        /// <summary>
        /// Default behaviour. Throw exception.
        /// </summary>
        Default,

        /// <summary>
        /// Show exception message and replace incorrect expression by <b>Placeholder</b>.
        /// </summary>
        ShowExceptionMessage,

        /// <summary>
        /// Replace expression with exception message. Don't show any messages.
        /// </summary>
        ReplaceExpressionWithExceptionMessage,

        /// <summary>
        /// Replace exception with <b>Placeholder</b> value. Don't show any messages.
        /// </summary>
        ReplaceExpressionWithPlaceholder
    }

    /// <summary>
    /// Contains compiler settings.
    /// </summary>
    public class CompilerSettings
    {
        #region Fields

        private string placeholder;
        private CompilerExceptionBehaviour exceptionBehaviour;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or set the string that will be used for replacing incorrect expressions.
        /// </summary>
        public string Placeholder
        {
            get { return placeholder; }
            set { placeholder = value; }
        }

        /// <summary>
        /// Gets or sets the behaviour of compiler when exception is thrown.
        /// </summary>
        public CompilerExceptionBehaviour ExceptionBehaviour
        {
            get { return exceptionBehaviour; }
            set { exceptionBehaviour = value; }
        }

        /// <summary>
        /// Get or sets number of recompiles
        /// </summary>
        /// <remarks>
        /// Report compiler can try to fix compilation errors and recompile your report again. This property sets the number of such attempts.
        /// </remarks>
        public int RecompileCount { get; set; } = 1;


#if REFLECTION_EMIT_COMPILER
        private bool _reflectionEmitCompiler = false;
        
        /// <summary>
        /// Enables faster compiler if the report script hasn't been changed
        /// </summary>
        public bool ReflectionEmitCompiler
        {
            get => _reflectionEmitCompiler;
            set 
            { 
                _reflectionEmitCompiler = value;
                CodeProvider.DefaultProvider = value ? typeof(FastReport.Code.ReflectionEmit.ReflectionEmitCodeProvider) : null;
            }
        }
#endif

#if CROSSPLATFORM || COREWIN
        // sets by user
        private CultureInfo cultureInfo;

        /// <summary>
        /// Sets culture for compiler
        /// </summary>
        public CultureInfo CultureInfo
        {
            get
            {
                if(cultureInfo == null)
                {
                    return Res.CurrentCulture;
                }
                return cultureInfo;
            }
            set
            {
                cultureInfo = value;
            }
        }
#endif

#endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilerSettings"/> class.
        /// </summary>
        public CompilerSettings()
        {
            placeholder = "";
            exceptionBehaviour = CompilerExceptionBehaviour.Default;
        }

        #endregion Constructors
    }
}
