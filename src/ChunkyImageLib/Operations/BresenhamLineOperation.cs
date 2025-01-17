﻿using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.ColorsImpl;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.DrawingApi.Core.Surface;
using PixiEditor.DrawingApi.Core.Surface.PaintImpl;

namespace ChunkyImageLib.Operations;
internal class BresenhamLineOperation : IMirroredDrawOperation
{
    public bool IgnoreEmptyChunks => false;
    private readonly VecI from;
    private readonly VecI to;
    private readonly Color color;
    private readonly BlendMode blendMode;
    private readonly Point[] points;
    private Paint paint;

    public BresenhamLineOperation(VecI from, VecI to, Color color, BlendMode blendMode)
    {
        this.from = from;
        this.to = to;
        this.color = color;
        this.blendMode = blendMode;
        paint = new Paint() { BlendMode = blendMode };
        points = BresenhamLineHelper.GetBresenhamLine(from, to);
    }

    public void DrawOnChunk(Chunk chunk, VecI chunkPos)
    {
        // a hacky way to make the lines look slightly better on non full res chunks
        paint.Color = new Color(color.R, color.G, color.B, (byte)(color.A * chunk.Resolution.Multiplier()));

        var surf = chunk.Surface.DrawingSurface;
        surf.Canvas.Save();
        surf.Canvas.Scale((float)chunk.Resolution.Multiplier());
        surf.Canvas.Translate(-chunkPos * ChunkyImage.FullChunkSize);
        surf.Canvas.DrawPoints(PointMode.Points, points, paint);
        surf.Canvas.Restore();
    }

    public AffectedArea FindAffectedArea(VecI imageSize)
    {
        RectI bounds = RectI.FromTwoPixels(from, to);
        return new AffectedArea(OperationHelper.FindChunksTouchingRectangle(bounds, ChunkyImage.FullChunkSize), bounds);
    }

    public IDrawOperation AsMirrored(int? verAxisX, int? horAxisY)
    {
        RectI newFrom = new RectI(from, new VecI(1));
        RectI newTo = new RectI(to, new VecI(1));
        if (verAxisX is not null)
        {
            newFrom = newFrom.ReflectX((int)verAxisX);
            newTo = newTo.ReflectX((int)verAxisX);
        }
        if (horAxisY is not null)
        {
            newFrom = newFrom.ReflectY((int)horAxisY);
            newTo = newTo.ReflectY((int)horAxisY);
        }
        return new BresenhamLineOperation(newFrom.Pos, newTo.Pos, color, blendMode);
    }

    public void Dispose()
    {
        paint.Dispose();
    }
}
