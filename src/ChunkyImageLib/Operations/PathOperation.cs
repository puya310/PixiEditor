﻿using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.ColorsImpl;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.DrawingApi.Core.Surface;
using PixiEditor.DrawingApi.Core.Surface.PaintImpl;
using PixiEditor.DrawingApi.Core.Surface.Vector;

namespace ChunkyImageLib.Operations;
internal class PathOperation : IMirroredDrawOperation
{
    private readonly VectorPath path;

    private readonly Paint paint;
    private readonly RectI bounds;

    public bool IgnoreEmptyChunks => false;

    public PathOperation(VectorPath path, Color color, float strokeWidth, StrokeCap cap, BlendMode blendMode, RectI? customBounds = null)
    {
        this.path = new VectorPath(path);
        paint = new() { Color = color, Style = PaintStyle.Stroke, StrokeWidth = strokeWidth, StrokeCap = cap, BlendMode = blendMode };

        RectI floatBounds = customBounds ?? (RectI)(path.TightBounds).RoundOutwards();
        bounds = floatBounds.Inflate((int)Math.Ceiling(strokeWidth) + 1);
    }

    public void DrawOnChunk(Chunk chunk, VecI chunkPos)
    {
        paint.IsAntiAliased = chunk.Resolution != ChunkResolution.Full;
        var surf = chunk.Surface.DrawingSurface;
        surf.Canvas.Save();
        surf.Canvas.Scale((float)chunk.Resolution.Multiplier());
        surf.Canvas.Translate(-chunkPos * ChunkyImage.FullChunkSize);
        surf.Canvas.DrawPath(path, paint);
        surf.Canvas.Restore();
    }

    public AffectedArea FindAffectedArea(VecI imageSize)
    {
        return new AffectedArea(OperationHelper.FindChunksTouchingRectangle(bounds, ChunkyImage.FullChunkSize), bounds);
    }

    public IDrawOperation AsMirrored(int? verAxisX, int? horAxisY)
    {
        var matrix = Matrix3X3.CreateScale(verAxisX is not null ? -1 : 1, horAxisY is not null ? -1 : 1, verAxisX ?? 0, horAxisY ?? 0);
        using var copy = new VectorPath(path);
        copy.Transform(matrix);

        RectI newBounds = bounds;
        if (verAxisX is not null)
            newBounds = newBounds.ReflectX((int)verAxisX);
        if (horAxisY is not null)
            newBounds = newBounds.ReflectY((int)horAxisY);
        return new PathOperation(copy, paint.Color, paint.StrokeWidth, paint.StrokeCap, paint.BlendMode, newBounds);
    }

    public void Dispose()
    {
        path.Dispose();
        paint.Dispose();
    }
}
