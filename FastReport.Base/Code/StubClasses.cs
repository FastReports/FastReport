
namespace FastReport.Code
{
    internal partial class AssemblyDescriptor
    {
        private void AddStubClasses()
        {
            const string stubClassesCSharp = @"
namespace System
{
internal static class Activator {}
internal static class AppContext {}
internal static class AppDomain {}
internal static class Environment {}
}

namespace System.IO
{
internal static class Directory {}
internal static class DirectoryInfo {}
internal static class DriveInfo {}
internal static class File {}
internal static class FileInfo {}
internal static class FileStream {}
internal static class FileSystemInfo {}
internal static class Path {}
}

namespace System.Diagnostics
{
internal static class FileVersionInfo {}
internal static class Process {}
internal static class ProcessModule {}
internal static class ProcessStartInfo {}
internal static class ProcessThread {}
internal static class ProcessThreadCollection {}
internal static class StackFrame {}
}

namespace System.Reflection
{
internal static class Assembly {}
internal static class AssemblyExtensions {}
internal static class Binder {}
internal static class ConstructorInfo {}
internal static class EventInfo {}
internal static class FieldInfo {}
internal static class LocalVariableInfo {}
internal static class MemberInfo {}
internal static class MethodBase {}
internal static class MethodInfo {}
internal static class Module {}
internal static class ParameterInfo {}
internal static class PropertyInfo {}
internal static class ReflectionContext {}
internal static class TypeInfo {}
}

namespace System.Timers
{
internal static class Timer {}
}

namespace System.Windows.Forms
{
internal static class Timer {}
}

namespace System.Threading
{
internal static class AsyncLocal {}
internal static class Barrier {}
internal static class Interlocked {}
internal static class Monitor {}
internal static class Mutex {}
internal static class Overlapped {}
internal static class Semaphore {}
internal static class SemaphoreSlim {}
internal static class SynchronizationContext {}
internal static class Thread {}
internal static class ThreadPool {}
internal static class Timer {}
internal static class Volatile {}
}

namespace System.Threading.Tasks
{
internal static class ConcurrentExclusiveSchedulerPair {}
internal static class Parallel {}
internal static class Task {}
internal static class Task<TResult> {}
internal static class TaskCompletionSource<TResult> {}
internal static class TaskFactory {}
internal static class TaskFactory<TResult> {}
internal static class TaskScheduler {}
}

namespace System.Net
{
internal static class AuthenticationManager {}
internal static class Authorization {}
internal static class Cookie {}
internal static class Dns {}
internal static class DnsEndPoint {}
internal static class EndPoint {}
internal static class FileWebRequest {}
internal static class FileWebResponse {}
internal static class FtpWebRequest {}
internal static class FtpWebResponse {}
internal static class HttpListener {}
internal static class HttpListenerContext {}
internal static class HttpListenerRequest {}
internal static class HttpListenerResponse {}
internal static class HttpWebRequest {}
internal static class HttpWebResponse {}
internal static class IPAddress {}
internal static class NetworkCredential {}
internal static class ServicePoint {}
internal static class ServicePointManager {}
internal static class TransportContext {}
internal static class WebClient {}
internal static class WebProxy {}
internal static class WebRequest {}
internal static class WebResponse {}
internal static class Uri {}
}

namespace System.Net.Http
{
internal static class ByteArrayContent {}
internal static class FormUrlEncodedContent {}
internal static class HttpClient {}
internal static class HttpContent {}
internal static class HttpMethod {}
internal static class HttpRequestMessage {}
internal static class HttpResponseMessage {}
internal static class MultipartContent {}
internal static class MultipartFormDataContent {}
internal static class ReadOnlyMemoryContent {}
internal static class StreamContent {}
internal static class StringContent {}
}

namespace System.Web.UI
{
internal static class Timer {}
}

namespace FastReport.Utils
{
internal static class Config {}
}
";

            const string stubClassesVBNet = @"
Namespace System
    Friend Module Activator
    End Module

    Friend Module AppContext
    End Module

    Friend Module AppDomain
    End Module

    Friend Module Environment
    End Module
End Namespace

Namespace System.IO
    Friend Module Directory
    End Module

    Friend Module DirectoryInfo
    End Module

    Friend Module DriveInfo
    End Module

    Friend Module File
    End Module

    Friend Module FileInfo
    End Module

    Friend Module FileStream
    End Module

    Friend Module FileSystemInfo
    End Module

    Friend Module Path
    End Module
End Namespace

Namespace System.Diagnostics
    Friend Module FileVersionInfo
    End Module

    Friend Module Process
    End Module

    Friend Module ProcessModule
    End Module

    Friend Module ProcessStartInfo
    End Module

    Friend Module ProcessThread
    End Module

    Friend Module ProcessThreadCollection
    End Module

    Friend Module StackFrame
    End Module
End Namespace

Namespace System.Reflection
    Friend Module Assembly
    End Module

    Friend Module AssemblyExtensions
    End Module

    Friend Module Binder
    End Module

    Friend Module ConstructorInfo
    End Module

    Friend Module EventInfo
    End Module

    Friend Module FieldInfo
    End Module

    Friend Module LocalVariableInfo
    End Module

    Friend Module MemberInfo
    End Module

    Friend Module MethodBase
    End Module

    Friend Module MethodInfo
    End Module

    Friend Module [Module]
    End Module

    Friend Module ParameterInfo
    End Module

    Friend Module PropertyInfo
    End Module

    Friend Module ReflectionContext
    End Module

    Friend Module TypeInfo
    End Module
End Namespace

Namespace System.Timers
    Friend Module Timer
    End Module
End Namespace

Namespace System.Windows.Forms
    Friend Module Timer
    End Module
End Namespace

Namespace System.Threading
    Friend Module AsyncLocal
    End Module

    Friend Module Barrier
    End Module

    Friend Module Interlocked
    End Module

    Friend Module Monitor
    End Module

    Friend Module Mutex
    End Module

    Friend Module Overlapped
    End Module

    Friend Module Semaphore
    End Module

    Friend Module SemaphoreSlim
    End Module

    Friend Module SynchronizationContext
    End Module

    Friend Module Thread
    End Module

    Friend Module ThreadPool
    End Module

    Friend Module Timer
    End Module

    Friend Module Volatile
    End Module
End Namespace

Namespace System.Threading.Tasks
    Friend Module ConcurrentExclusiveSchedulerPair
    End Module

    Friend Module Parallel
    End Module

    Friend Module Task
    End Module

    Friend Module Task(Of TResult)
    End Module

    Friend Module TaskCompletionSource(Of TResult)
    End Module

    Friend Module TaskFactory
    End Module

    Friend Module TaskFactory(Of TResult)
    End Module

    Friend Module TaskScheduler
    End Module
End Namespace

Namespace System.Net
    Friend Module AuthenticationManager
    End Module

    Friend Module Authorization
    End Module

    Friend Module Cookie
    End Module

    Friend Module Dns
    End Module

    Friend Module DnsEndPoint
    End Module

    Friend Module EndPoint
    End Module

    Friend Module FileWebRequest
    End Module

    Friend Module FileWebResponse
    End Module

    Friend Module FtpWebRequest
    End Module

    Friend Module FtpWebResponse
    End Module

    Friend Module HttpListener
    End Module

    Friend Module HttpListenerContext
    End Module

    Friend Module HttpListenerRequest
    End Module

    Friend Module HttpListenerResponse
    End Module

    Friend Module HttpWebRequest
    End Module

    Friend Module HttpWebResponse
    End Module

    Friend Module IPAddress
    End Module

    Friend Module NetworkCredential
    End Module

    Friend Module ServicePoint
    End Module

    Friend Module ServicePointManager
    End Module

    Friend Module TransportContext
    End Module

    Friend Module WebClient
    End Module

    Friend Module WebProxy
    End Module

    Friend Module WebRequest
    End Module

    Friend Module WebResponse
    End Module

    Friend Module Uri
    End Module
End Namespace

Namespace System.Net.Http
    Friend Module ByteArrayContent
    End Module

    Friend Module FormUrlEncodedContent
    End Module

    Friend Module HttpClient
    End Module

    Friend Module HttpContent
    End Module

    Friend Module HttpMethod
    End Module

    Friend Module HttpRequestMessage
    End Module

    Friend Module HttpResponseMessage
    End Module

    Friend Module MultipartContent
    End Module

    Friend Module MultipartFormDataContent
    End Module

    Friend Module ReadOnlyMemoryContent
    End Module

    Friend Module StreamContent
    End Module

    Friend Module StringContent
    End Module
End Namespace

Namespace System.Web.UI
    Friend Module Timer
    End Module
End Namespace

Namespace FastReport.Utils
    Friend Module Config
    End Module
End Namespace
";

            if (Report.ScriptLanguage == Language.CSharp)
                scriptText.Append(stubClassesCSharp);
            else
                scriptText.Append(stubClassesVBNet);
        }
    }
}
