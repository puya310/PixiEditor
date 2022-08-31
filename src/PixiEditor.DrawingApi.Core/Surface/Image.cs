﻿using PixiEditor.DrawingApi.Core.Bridge;

namespace PixiEditor.DrawingApi.Core.Surface
{
    /// <summary>An abstraction for drawing a rectangle of pixels.</summary>
    /// <remarks>
    ///     <para>An image is an abstraction of pixels, though the particular type of image could be actually storing its data on the GPU, or as drawing commands (picture or PDF or otherwise), ready to be played back into another canvas.</para>
    ///     <para />
    ///     <para>The content of an image is always immutable, though the actual storage may change, if for example that image can be recreated via encoded data or other means.</para>
    ///     <para />
    ///     <para>An image always has a non-zero dimensions. If there is a request to create a new image, either directly or via a surface, and either of the requested dimensions are zero, then <see langword="null" /> will be returned.</para>
    /// </remarks>
    public class Image : PixelDataObject
    {
        public override void Dispose()
        {
            DrawingBackendApi.Current.ImageOperations.DisposeImage(this);
        }

        public static Image FromEncodedData(string path)
        {
            return DrawingBackendApi.Current.ImageOperations.FromEncodedData(path);
        }
    }
}
