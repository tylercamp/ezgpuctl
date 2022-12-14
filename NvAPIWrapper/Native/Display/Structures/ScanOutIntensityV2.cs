using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IScanOutIntensity" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct ScanOutIntensityV2 : IDisposable, IInitializable, IScanOutIntensity
    {
        internal StructureVersion _Version;
        internal uint _Width;
        internal uint _Height;
        internal IntPtr _BlendingTexture;
        internal IntPtr _OffsetTexture;
        internal uint _OffsetTextureChannels;

        /// <summary>
        ///     Creates a new instance of <see cref="ScanOutIntensityV2" />.
        /// </summary>
        /// <param name="width">The width of the input texture.</param>
        /// <param name="height">The height of the input texture</param>
        /// <param name="blendingTexture">The array of floating values building an intensity RGB texture</param>
        /// <param name="offsetTextureChannels">The number of channels per pixel in the offset texture</param>
        /// <param name="offsetTexture">The array of floating values building an offset texture</param>
        // ReSharper disable once TooManyDependencies
        public ScanOutIntensityV2(
            uint width,
            uint height,
            float[] blendingTexture,
            uint offsetTextureChannels,
            float[] offsetTexture)
        {
            if (blendingTexture?.Length != width * height * 3)
            {
                throw new ArgumentOutOfRangeException(nameof(blendingTexture));
            }

            if (offsetTexture?.Length != width * height * offsetTextureChannels)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetTexture));
            }

            this = typeof(ScanOutIntensityV2).Instantiate<ScanOutIntensityV2>();
            _Width = width;
            _Height = height;
            _BlendingTexture = Marshal.AllocHGlobal((int) (width * height * 3 * sizeof(float)));
            Marshal.Copy(blendingTexture, 0, _BlendingTexture, blendingTexture.Length);

            _OffsetTextureChannels = offsetTextureChannels;
            _OffsetTexture = Marshal.AllocHGlobal((int) (width * height * offsetTextureChannels * sizeof(float)));
            Marshal.Copy(offsetTexture, 0, _OffsetTexture, offsetTexture.Length);
        }

        /// <inheritdoc />
        public uint Width
        {
            get => _Width;
        }

        /// <inheritdoc />
        public uint Height
        {
            get => _Height;
        }

        /// <summary>
        ///     Gets the number of channels per pixel in the offset texture
        /// </summary>
        public uint OffsetTextureChannels
        {
            get => _OffsetTextureChannels;
        }

        /// <inheritdoc />
        public float[] BlendingTexture
        {
            get
            {
                var floats = new float[_Width * _Height * 3];
                Marshal.Copy(_BlendingTexture, floats, 0, floats.Length);

                return floats;
            }
        }

        /// <summary>
        ///     Gets the array of floating values building an offset texture
        /// </summary>
        public float[] OffsetTexture
        {
            get
            {
                var floats = new float[_Width * _Height * _OffsetTextureChannels];
                Marshal.Copy(_OffsetTexture, floats, 0, floats.Length);

                return floats;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Marshal.FreeHGlobal(_BlendingTexture);
            Marshal.FreeHGlobal(_OffsetTexture);
        }
    }
}