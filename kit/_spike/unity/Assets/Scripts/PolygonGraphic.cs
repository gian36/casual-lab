// UGUI has no polygon primitive, so the aircraft silhouette needs a Graphic that
// builds its own mesh. Keeps geometry parity with the Godot and Phaser slices: the
// same unit-space outline, triangulated here because UGUI will not do it for us.
// THROWAWAY.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonGraphic : MaskableGraphic
{
    // Unit space, 0..1, y down — identical numbers to the other two slices.
    public Vector2[] Points = new Vector2[0];

    private static readonly List<int> Triangles = new List<int>(128);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (Points == null || Points.Length < 3) return;

        Rect r = GetPixelAdjustedRect();
        var pixels = new Vector2[Points.Length];
        for (int i = 0; i < Points.Length; i++)
        {
            pixels[i] = new Vector2(
                r.x + Points[i].x * r.width,
                r.y + (1f - Points[i].y) * r.height);
            vh.AddVert(pixels[i], color, Vector2.zero);
        }

        Triangulate(pixels, Triangles);
        for (int i = 0; i + 2 < Triangles.Count; i += 3)
        {
            vh.AddTriangle(Triangles[i], Triangles[i + 1], Triangles[i + 2]);
        }
    }

    // Ear clipping. The silhouette is concave (wings and tail cut into it), so a
    // triangle fan would render it wrong.
    private static void Triangulate(Vector2[] poly, List<int> outTriangles)
    {
        outTriangles.Clear();
        int n = poly.Length;
        if (n < 3) return;

        // Walk the outline counter-clockwise, which is what the convexity test below
        // assumes. FIXED 2026-09-13: the first version used the (b.x-a.x)(b.y+a.y)
        // shoelace form, which is positive for CLOCKWISE polygons, so it walked the
        // outline backwards, rejected real ears, and drew the aircraft as shards.
        var indices = new List<int>(n);
        if (SignedArea(poly) > 0f)
            for (int i = 0; i < n; i++) indices.Add(i);
        else
            for (int i = n - 1; i >= 0; i--) indices.Add(i);

        int guard = indices.Count * indices.Count;
        while (indices.Count > 2 && guard-- > 0)
        {
            bool clipped = false;
            for (int i = 0; i < indices.Count; i++)
            {
                int i0 = indices[(i + indices.Count - 1) % indices.Count];
                int i1 = indices[i];
                int i2 = indices[(i + 1) % indices.Count];

                Vector2 a = poly[i0], b = poly[i1], c = poly[i2];
                if (Cross(b - a, c - b) <= 0f) continue;  // reflex, not an ear

                bool contains = false;
                foreach (int j in indices)
                {
                    if (j == i0 || j == i1 || j == i2) continue;
                    if (InTriangle(poly[j], a, b, c)) { contains = true; break; }
                }
                if (contains) continue;

                outTriangles.Add(i0);
                outTriangles.Add(i1);
                outTriangles.Add(i2);
                indices.RemoveAt(i);
                clipped = true;
                break;
            }
            if (!clipped) break;  // degenerate input; draw what we have
        }
    }

    // Standard shoelace: positive for counter-clockwise in y-up space.
    private static float SignedArea(Vector2[] p)
    {
        float sum = 0f;
        for (int i = 0; i < p.Length; i++)
        {
            Vector2 a = p[i], b = p[(i + 1) % p.Length];
            sum += a.x * b.y - b.x * a.y;
        }
        return sum * 0.5f;
    }

    private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    // Strictly inside a counter-clockwise triangle. FIXED 2026-09-13: the first version
    // also counted points lying ON an edge as inside, and this outline has collinear
    // vertices along its fuselage, which blocked valid ears.
    private static bool InTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        const float eps = 1e-4f;
        return Cross(b - a, p - a) > eps && Cross(c - b, p - b) > eps && Cross(a - c, p - c) > eps;
    }
}
