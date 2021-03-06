﻿#region License
//
// OVR.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2014 Stefanos Apostolopoulos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using OpenTK;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace OpenTK
{
    using OVR_Instance = IntPtr;

    /// <summary>
    /// Provides high-level access to an Oculus Rift device.
    /// </summary>
    public class OculusRift : IDisposable
    {
        static int instance_count;
        static Toolkit opentk;
        OVR_Instance instance;
        bool disposed;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTK.OculusRift"/> class.
        /// </summary>
        public OculusRift()
        {
            if (Interlocked.Increment(ref instance_count) == 1)
            {
                opentk = Toolkit.Init();
                NativeMethods.Init();
            }

            instance = NativeMethods.Create();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets a value indicating whether this instance is connected
        /// to an Oculus Rift device.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                CheckDisposed();
                return NativeMethods.IsConnected(instance);
            }
        }
            
        /// <summary>
        /// Gets the x-coordinate of the Oculus Rift display device,
        /// in global desktop coordinates (px).
        /// </summary>
        public int DesktopX
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetDesktopX(instance);
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the Oculus Rift display device,
        /// in global desktop coordinates (px).
        /// </summary>
        public int DesktopY
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetDesktopY(instance);
            }
        }

        /// <summary>
        /// Horizontal resolution of the entire HMD screen in pixels.
        /// Half the HResolution is used for each eye.
        /// This value is 1280 for the Development Kit.
        /// </summary>
        public int HResolution
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetHResolution(instance);
            }
        }

        /// <summary>
        /// Vertical resolution of the entire HMD screen in pixels.
        /// This value is 800 for the Development Kit.
        /// </summary>
        public int VResolution
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVResolution(instance);
            }
        }

        /// <summary>
        /// Horizontal dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        public float HScreenSize
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetHScreenSize(instance);
            }
        }

        /// <summary>
        /// Vertical dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        public float VScreenSize
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVScreenSize(instance);
            }
        }

        /// <summary>
        /// Physical offset from the top of the screen to eye center, in meters.
        /// Currently half VScreenSize.
        /// </summary>
        public float VScreenCenter
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVScreenCenter(instance);
            }
        }

        /// <summary>
        /// Physical distance between the lens centers, in meters. Lens centers
        /// are the centers of distortion.
        /// </summary>
        public float LensSeparationDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetLensSeparationDistance(instance);
            }
        }

        /// <summary>
        /// Configured distance between eye centers.
        /// </summary>
        public float InterpupillaryDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetInterpulpillaryDistance(instance);
            }
        }

        /// <summary>
        /// Distance from the eye to the screen, in meters. It combines
        /// distances from the eye to the lens, and from the lens to the screen.
        /// This value is needed to compute the fov correctly.
        /// </summary>
        public float EyeToScreenDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetEyeToScreenDistance(instance);
            }
        }

        /// <summary>
        /// Gets the radial distortion correction coefficients.
        /// </summary>
        /// <value>The distortion k.</value>
        public Vector4 DistortionK
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetDistortionK(instance);
            }
        }

        /// <summary>
        /// Gets the chroma AB aberration coefficients.
        /// </summary>
        public Vector4 ChromaAbAberration
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetChromaAbCorrection(instance);
            }
        }

        #region Sensor Fusion

        /// <summary>
        /// Gets a <see cref="OpenTK.Quaternion"/> representing
        /// the current accumulated orientation. Most applications
        /// will want to use <see cref="PredictedOrientation"/> instead.
        /// Use <see cref="OpenTK.Quaternion.ToAxisAngle()"/> to
        /// convert this quaternion to an axis-angle representation.
        /// Use <see cref="OpenTK.Matrix4.CreateFromQuaternion(OpenTK.Quaternion)"/>
        /// to convert this quaternion to a rotation matrix.
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetOrientation(instance);
            }
        }

        /// <summary>
        /// Gets a <see cref="OpenTK.Quaternion"/> representing
        /// the predicted orientation after <see cref="PredictionDelta"/>
        /// seconds.
        /// </summary>
        public Quaternion PredictedOrientation
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetPredictedOrientation(instance);
            }
        }

        /// <summary>
        /// Gets the last absolute acceleration reading, in m/s^2.
        /// </summary>
        /// <value>The acceleration.</value>
        public Vector3 Acceleration
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetAcceleration(instance);
            }
        }

        /// <summary>
        /// Gets the last angular velocity reading, in rad/s.
        /// </summary>
        /// <value>The angular velocity.</value>
        public Vector3 AngularVelocity
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetAngularVelocity(instance);
            }
        }

        /// <summary>
        /// Gets or sets the delta time for sensor prediction, in seconds.
        /// </summary>
        public float PredictionDelta
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetPredictionDelta(instance);
            }
            set
            {
                CheckDisposed();
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                NativeMethods.SetPrediction(instance, value, IsPredictionEnabled ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether sensor prediction is enabled.
        /// </summary>
        public bool IsPredictionEnabled
        {
            get
            {
                CheckDisposed();
                return NativeMethods.IsPredictionEnabled(instance) != 0;
            }
            set
            {
                CheckDisposed();
                NativeMethods.SetPredictionEnabled(instance, value ? 1 : 0);
            }
        }

        #endregion

        #endregion

        #region Private Members

        void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Releases all resource used by the <see cref="OpenTK.OculusRift"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="OpenTK.OculusRift"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="OpenTK.OculusRift"/> in an unusable state. After calling
        /// <see cref="Dispose()"/>, you must release all references to the <see cref="OpenTK.OculusRift"/> so the garbage
        /// collector can reclaim the memory that the <see cref="OpenTK.OculusRift"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    NativeMethods.Destroy(instance);
                    if (Interlocked.Decrement(ref instance_count) == 0)
                    {
                        NativeMethods.Shutdown();
                        opentk.Dispose();
                        opentk = null;
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="OpenTK.OculusRift"/>
        /// is reclaimed by garbage collection.
        /// </summary>
        ~OculusRift()
        {
            Console.Error.WriteLine("[Warning] OculusRift instance leaked. Did you forget to call Dispose()?");
        }

        #endregion

        #region NativeMethods

        static class NativeMethods
        {
            const string lib = "OVR";

            static NativeMethods()
            {
                if (Configuration.RunningOnWindows &&
                    !Configuration.RunningOnMono)
                {
                    // When running on Windows, we need to load the
                    // native library into the address space of the
                    // application. This allows us to use DllImport
                    // to load the correct (x86 or x64) library at
                    // runtime.
                    // Non-windows platforms rely on the dll.config
                    // file to load the correct library.

                    if (IntPtr.Size == 4)
                    {
                        // Running on 32bit system
                        LoadLibrary("lib/x86/OVR.dll");
                    }
                    else
                    {
                        // Running on 64bit system
                        LoadLibrary("lib/x64/OVR.dll");
                    }
                }
            }

            [DllImport("kernel32.dll")]
            static extern IntPtr LoadLibrary(string filename);

            #region Device

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Init", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Init();

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Shutdown", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Shutdown();

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Create", CallingConvention = CallingConvention.Cdecl)]
            public static extern OVR_Instance Create();

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Destroy", CallingConvention = CallingConvention.Cdecl)]
            public static extern OVR_Instance Destroy(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_IsConnected", CallingConvention = CallingConvention.Cdecl)]
            public static extern bool IsConnected(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetHScreenSize", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetHScreenSize(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVScreenSize", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetVScreenSize(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVScreenCenter", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetVScreenCenter(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDesktopX", CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetDesktopX(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDesktopY", CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetDesktopY(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetHResolution", CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetHResolution(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVResolution", CallingConvention = CallingConvention.Cdecl)]
            public static extern int GetVResolution(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetEyeToScreenDistance", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetEyeToScreenDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetLensSeparationDistance", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetLensSeparationDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetInterpupillaryDistance", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetInterpulpillaryDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDistortionK", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector4 GetDistortionK(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetChromaAbCorrection", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector4 GetChromaAbCorrection(OVR_Instance inst);

            #endregion

            #region Sensor Fusion

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetOrientation", CallingConvention = CallingConvention.Cdecl)]
            public static extern Quaternion GetOrientation(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetPredictedOrientation", CallingConvention = CallingConvention.Cdecl)]
            public static extern Quaternion GetPredictedOrientation(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetAcceleration", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3 GetAcceleration(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetAngularVelocity", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3 GetAngularVelocity(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetPredictionDelta", CallingConvention = CallingConvention.Cdecl)]
            public static extern float GetPredictionDelta(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_SetPrediction", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPrediction(OVR_Instance inst, float dt, int enable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_SetPredictionEnabled", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetPredictionEnabled(OVR_Instance inst, int enable);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_IsPredictionEnabled", CallingConvention = CallingConvention.Cdecl)]
            public static extern int IsPredictionEnabled(OVR_Instance inst);

            #endregion
        }

        #endregion
    }
}

