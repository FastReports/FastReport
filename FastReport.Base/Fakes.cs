//#define USE_FAKES

// This file represent mock for System.DateTime & System.Guid (because Microsoft Fakes doesn't correct work in .Net Core)

using System.Runtime.CompilerServices;

namespace SystemFake
{
    internal struct DateTime
    {
        internal static System.DateTime Now
        {
#if NETSTANDARD || NETCOREAPP
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
#if !USE_FAKES
                return System.DateTime.Now;
#else
                return new System.DateTime(0, System.DateTimeKind.Utc);
#endif
            }
        }


        internal static System.DateTime UtcNow
        {
#if NETSTANDARD || NETCOREAPP
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
#if !USE_FAKES
                return System.DateTime.UtcNow;
#else
                return new System.DateTime(0, System.DateTimeKind.Utc);
#endif
            }
        }
    }


    internal struct Guid
    {
#if NETSTANDARD || NETCOREAPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static System.Guid NewGuid()
        {
#if !USE_FAKES
            return System.Guid.NewGuid();
#else
            return System.Guid.Empty;
#endif
        }
    }
}
