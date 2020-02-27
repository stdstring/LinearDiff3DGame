using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenGLTools
{
    public class OpenGLHelper : IDisposable
    {
        public OpenGLHelper(IntPtr controlHandle, Int32 width, Int32 height)
        {
            this.controlHandle = controlHandle;
            SetViewport(width, height);
            CreateOpenGLContext(controlHandle);
        }

        public void Dispose()
        {
            DisposeOpenGLContext(controlHandle);
        }

        ~OpenGLHelper()
        {
            Dispose();
        }

        public void SetViewport(Int32 width, Int32 height)
        {
            OpenGLImport.glViewport(0, 0, width, height);
        }

        public void SetProjection(Double fovy, Double aspect, Double zNear, Double zFar)
        {
            OpenGLImport.glMatrixMode(OpenGLImport.GL_PROJECTION);
            OpenGLImport.glLoadIdentity();
            OpenGLImport.gluPerspective(fovy, aspect, zNear, zFar);
        }

        public void SetDefaultProjection(Int32 width, Int32 height)
        {
            float dAspect = width <= height ? (float)height / width : (float)width / height;
            SetProjection(45.0, dAspect, 0.01, 10000.0);
        }

        public void ActivateContext()
        {
            OpenGLImport.wglMakeCurrent(HDC, HRC);
        }

        public void DeactivateContext()
        {
            OpenGLImport.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        public void SwapBuffers()
        {
            OpenGLImport.SwapBuffers(HDC);
        }

        internal IntPtr HDC { get; private set; }
        internal IntPtr HRC { get; private set; }

        private void CreateOpenGLContext(IntPtr handle)
        {
            try
            {
                // Fill in the pixel format descriptor.
                OpenGLImport.PIXELFORMATDESCRIPTOR pfd = new OpenGLImport.PIXELFORMATDESCRIPTOR();
                pfd.Initialize();
                HDC = OpenGLImport.GetDC(handle);
                if (HDC == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Choose appropriate pixel format supported by a HDC context
                int iPixelformat = OpenGLImport.ChoosePixelFormat(HDC, ref pfd);
                if (iPixelformat == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Set the pixel format
                if (OpenGLImport.SetPixelFormat(HDC, iPixelformat, ref pfd) == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Create a new OpenGL rendering context
                HRC = OpenGLImport.wglCreateContext(HDC);
                if (HRC == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Make HRC rendering context as a current context
                if (OpenGLImport.wglMakeCurrent(HDC, HRC) == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Release the OpenGL rendering context 
                //OpenGLImport.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception)
            {
                if (HRC != IntPtr.Zero)
                    OpenGLImport.wglDeleteContext(HRC);
                if (HDC != IntPtr.Zero)
                    OpenGLImport.ReleaseDC(handle, HDC);
                throw;
            }
        }

        private void DisposeOpenGLContext(IntPtr handle)
        {
            try
            {
                // Release the OpenGL rendering context 
                OpenGLImport.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                // Delete the OpenGL rendering context.
                if(HRC != IntPtr.Zero) OpenGLImport.wglDeleteContext(HRC);
                // Release the device context
                if(HDC != IntPtr.Zero) OpenGLImport.ReleaseDC(handle, HDC);
                HDC = HRC = IntPtr.Zero;
            }
                // ReSharper disable EmptyGeneralCatchClause
            catch(Exception)
                // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private IntPtr controlHandle;
    }
}