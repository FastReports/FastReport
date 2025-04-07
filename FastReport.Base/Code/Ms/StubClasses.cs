
namespace FastReport.Code.Ms
{
    partial class MsAssemblyDescriptor
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
internal static class StreamReader {}
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
    Friend Class Activator
    End Class

    Friend Class AppContext
    End Class

    Friend Class AppDomain
    End Class

    Friend Class Environment
    End Class
End Namespace

Namespace System.IO
    Friend Class Directory
    End Class

    Friend Class DirectoryInfo
    End Class

    Friend Class DriveInfo
    End Class

    Friend Class File
    End Class

    Friend Class FileInfo
    End Class

    Friend Class FileStream
    End Class

    Friend Class FileSystemInfo
    End Class

    Friend Class Path
    End Class

    Friend Class StreamReader
    End Class
End Namespace

Namespace System.Diagnostics
    Friend Class FileVersionInfo
    End Class

    Friend Class Process
    End Class

    Friend Class ProcessClass
    End Class

    Friend Class ProcessStartInfo
    End Class

    Friend Class ProcessThread
    End Class

    Friend Class ProcessThreadCollection
    End Class

    Friend Class StackFrame
    End Class
End Namespace

Namespace System.Reflection
    Friend Class Assembly
    End Class

    Friend Class AssemblyExtensions
    End Class

    Friend Class Binder
    End Class

    Friend Class ConstructorInfo
    End Class

    Friend Class EventInfo
    End Class

    Friend Class FieldInfo
    End Class

    Friend Class LocalVariableInfo
    End Class

    Friend Class MemberInfo
    End Class

    Friend Class MethodBase
    End Class

    Friend Class MethodInfo
    End Class

    Friend Class [Class]
    End Class

    Friend Class ParameterInfo
    End Class

    Friend Class PropertyInfo
    End Class

    Friend Class ReflectionContext
    End Class

    Friend Class TypeInfo
    End Class
End Namespace

Namespace System.Timers
    Friend Class Timer
    End Class
End Namespace

Namespace System.Windows.Forms
    Friend Class Timer
    End Class
End Namespace

Namespace System.Threading
    Friend Class AsyncLocal
    End Class

    Friend Class Barrier
    End Class

    Friend Class Interlocked
    End Class

    Friend Class Monitor
    End Class

    Friend Class Mutex
    End Class

    Friend Class Overlapped
    End Class

    Friend Class Semaphore
    End Class

    Friend Class SemaphoreSlim
    End Class

    Friend Class SynchronizationContext
    End Class

    Friend Class Thread
    End Class

    Friend Class ThreadPool
    End Class

    Friend Class Timer
    End Class

    Friend Class Volatile
    End Class
End Namespace

Namespace System.Threading.Tasks
    Friend Class ConcurrentExclusiveSchedulerPair
    End Class

    Friend Class Parallel
    End Class

    Friend Class Task
    End Class

    Friend Class Task(Of TResult)
    End Class

    Friend Class TaskCompletionSource(Of TResult)
    End Class

    Friend Class TaskFactory
    End Class

    Friend Class TaskFactory(Of TResult)
    End Class

    Friend Class TaskScheduler
    End Class
End Namespace

Namespace System.Net
    Friend Class AuthenticationManager
    End Class

    Friend Class Authorization
    End Class

    Friend Class Cookie
    End Class

    Friend Class Dns
    End Class

    Friend Class DnsEndPoint
    End Class

    Friend Class EndPoint
    End Class

    Friend Class FileWebRequest
    End Class

    Friend Class FileWebResponse
    End Class

    Friend Class FtpWebRequest
    End Class

    Friend Class FtpWebResponse
    End Class

    Friend Class HttpListener
    End Class

    Friend Class HttpListenerContext
    End Class

    Friend Class HttpListenerRequest
    End Class

    Friend Class HttpListenerResponse
    End Class

    Friend Class HttpWebRequest
    End Class

    Friend Class HttpWebResponse
    End Class

    Friend Class IPAddress
    End Class

    Friend Class NetworkCredential
    End Class

    Friend Class ServicePoint
    End Class

    Friend Class ServicePointManager
    End Class

    Friend Class TransportContext
    End Class

    Friend Class WebClient
    End Class

    Friend Class WebProxy
    End Class

    Friend Class WebRequest
    End Class

    Friend Class WebResponse
    End Class

    Friend Class Uri
    End Class
End Namespace

Namespace System.Net.Http
    Friend Class ByteArrayContent
    End Class

    Friend Class FormUrlEncodedContent
    End Class

    Friend Class HttpClient
    End Class

    Friend Class HttpContent
    End Class

    Friend Class HttpMethod
    End Class

    Friend Class HttpRequestMessage
    End Class

    Friend Class HttpResponseMessage
    End Class

    Friend Class MultipartContent
    End Class

    Friend Class MultipartFormDataContent
    End Class

    Friend Class ReadOnlyMemoryContent
    End Class

    Friend Class StreamContent
    End Class

    Friend Class StringContent
    End Class
End Namespace

Namespace System.Web.UI
    Friend Class Timer
    End Class
End Namespace

Namespace FastReport.Utils
    Friend Class Config
    End Class
End Namespace
";

            if (Report.ScriptLanguage == Language.CSharp)
                ScriptText.Append(stubClassesCSharp);
            else
                ScriptText.Append(stubClassesVBNet);
        }
    }
}
