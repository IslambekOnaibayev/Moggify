using SkiaSharp;

namespace Infrastructure.CoverGeneration
{
    internal sealed class SkiaSharpCoverService : ISongCoverService
    {
        private const int Size = 400;
        private const int RenderScale = 2;
        private const int RenderSize = Size * RenderScale;
        private const int OutputSize = 640;
        private const int WebpQuality = 90;

        public byte[] GenerateCover(long userSeed, int globalIndex, double likesAvg, string locale, string title, string artist)
        {
            var likesComponent = (long)Math.Round(likesAvg * 10);
            var localeComponent = (long)(StableHash(locale) % 100000);
            var combined = userSeed + likesComponent * 2_654_435_761L + localeComponent * 40_503L;
            var seed = (int)((combined * globalIndex + globalIndex) & 0x7FFFFFFF);
            var rng = new Random(seed);

            using var bitmap = new SKBitmap(RenderSize, RenderSize);
            using var canvas = new SKCanvas(bitmap);
            canvas.Scale(RenderScale);

            var scene = rng.Next(54);
            switch (scene)
            {
                case 0: DrawMountains(canvas, rng); break;
                case 1: DrawCity(canvas, rng); break;
                case 2: DrawSunsetSea(canvas, rng); break;
                case 3: DrawFlowers(canvas, rng); break;
                case 4: DrawImpressionist(canvas, rng); break;
                case 5: DrawAurora(canvas, rng); break;
                case 6: DrawPeople(canvas, rng); break;
                case 7: DrawAnimals(canvas, rng); break;
                case 8: DrawPlanet(canvas, rng); break;
                case 9: DrawForest(canvas, rng); break;
                case 10: DrawDesert(canvas, rng); break;
                case 11: DrawBalloons(canvas, rng); break;
                case 12: DrawSynthwave(canvas, rng); break;
                case 13: DrawButterflies(canvas, rng); break;
                case 14: DrawVinyl(canvas, rng); break;
                case 15: DrawLighthouse(canvas, rng); break;
                case 16: DrawGuitar(canvas, rng); break;
                case 17: DrawPianoKeys(canvas, rng); break;
                case 18: DrawMicrophone(canvas, rng); break;
                case 19: DrawHeadphones(canvas, rng); break;
                case 20: DrawTrumpet(canvas, rng); break;
                case 21: DrawMusicNotes(canvas, rng); break;
                case 22: DrawCassette(canvas, rng); break;
                case 23: DrawSaxophone(canvas, rng); break;
                case 24: DrawDrumKit(canvas, rng); break;
                case 25: DrawEqualizer(canvas, rng); break;
                case 26: DrawBoombox(canvas, rng); break;
                case 27: DrawFireworks(canvas, rng); break;
                case 28: DrawWaves(canvas, rng); break;
                case 29: DrawCampfire(canvas, rng); break;
                case 30: DrawSailboat(canvas, rng); break;
                case 31: DrawCherryBlossom(canvas, rng); break;
                case 32: DrawNebula(canvas, rng); break;
                case 33: DrawRainyWindow(canvas, rng); break;
                case 34: DrawSnowfall(canvas, rng); break;
                case 35: DrawRainbow(canvas, rng); break;
                case 36: DrawWaterfall(canvas, rng); break;
                case 37: DrawVolcano(canvas, rng); break;
                case 38: DrawGalaxy(canvas, rng); break;
                case 39: DrawConstellation(canvas, rng); break;
                case 40: DrawViolin(canvas, rng); break;
                case 41: DrawTurntable(canvas, rng); break;
                case 42: DrawAmplifier(canvas, rng); break;
                case 43: DrawFerrisWheel(canvas, rng); break;
                case 44: DrawWindmill(canvas, rng); break;
                case 45: DrawKoiPond(canvas, rng); break;
                case 46: DrawLavenderField(canvas, rng); break;
                case 47: DrawPalmBeach(canvas, rng); break;
                case 48: DrawJellyfish(canvas, rng); break;
                case 49: DrawCoralReef(canvas, rng); break;
                case 50: DrawAutumnLeaves(canvas, rng); break;
                case 51: DrawMandala(canvas, rng); break;
                case 52: DrawStainedGlass(canvas, rng); break;
                default: DrawGeometric(canvas, rng); break;
            }

            DrawVignette(canvas);

            using var resized = bitmap.Resize(
                new SKImageInfo(OutputSize, OutputSize), SKFilterQuality.High);
            using var pixmap = (resized ?? bitmap).PeekPixels();
            using var data = pixmap.Encode(
                new SKWebpEncoderOptions(SKWebpEncoderCompression.Lossy, WebpQuality));
            return data.ToArray();
        }

        private static void DrawMountains(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: rng.NextDouble() > 0.5);
            DrawCelestialBody(canvas, rng);

            var layers = 4;
            for (var layer = 0; layer < layers; layer++)
            {
                var depth = (layer + 1f) / layers;
                var baseY = Size * (0.45f + layer * 0.13f);
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = HsvToSkColor(rng.Next(200, 260), 0.3f, 0.25f + depth * 0.4f)
                };

                using var path = new SKPath();
                path.MoveTo(0, Size);
                path.LineTo(0, baseY);
                var x = 0f;
                while (x < Size)
                {
                    var peak = baseY - rng.Next(20, 70) * (1.2f - depth);
                    x += rng.Next(40, 90);
                    path.LineTo(x, peak);
                }
                path.LineTo(Size, baseY);
                path.LineTo(Size, Size);
                path.Close();
                canvas.DrawPath(path, paint);
            }
        }

        private static void DrawCity(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: true);
            DrawCelestialBody(canvas, rng);

            var skyline = Size * 0.55f;
            var x = 0f;
            while (x < Size)
            {
                var w = rng.Next(28, 58);
                var h = rng.Next(60, 200);
                var top = Size - h;
                using var building = new SKPaint
                {
                    IsAntialias = true,
                    Color = HsvToSkColor(rng.Next(210, 250), 0.35f, 0.12f + (float)rng.NextDouble() * 0.18f)
                };
                canvas.DrawRect(x, top, w, h, building);

                using var window = new SKPaint { Color = new SKColor(255, 220, 130, 200), IsAntialias = true };
                for (var wy = top + 8; wy < Size - 6; wy += 12)
                {
                    for (var wx = x + 5; wx < x + w - 5; wx += 10)
                    {
                        if (rng.NextDouble() > 0.5)
                            canvas.DrawRect(wx, wy, 4, 6, window);
                    }
                }
                x += w + rng.Next(2, 10);
            }

            _ = skyline;
        }

        private static void DrawSunsetSea(SKCanvas canvas, Random rng)
        {
            var hue = rng.Next(0, 45);
            var sky = new[]
            {
                HsvToSkColor(hue, 0.75f, 0.95f),
                HsvToSkColor((hue + 20) % 360, 0.85f, 0.7f),
                HsvToSkColor((hue + 330) % 360, 0.7f, 0.45f),
            };
            using (var paint = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size * 0.6f), sky, null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size * 0.6f, paint);
            }

            var sunX = rng.Next(120, 280);
            var sunY = rng.Next(140, 220);
            using (var sun = new SKPaint { IsAntialias = true, Color = new SKColor(255, 240, 200, 235) })
                canvas.DrawCircle(sunX, sunY, rng.Next(40, 60), sun);

            using (var seaPaint = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, Size * 0.6f), new SKPoint(0, Size),
                    [HsvToSkColor((hue + 200) % 360, 0.6f, 0.4f), HsvToSkColor((hue + 210) % 360, 0.7f, 0.15f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, Size * 0.6f, Size, Size * 0.4f, seaPaint);
            }

            using var glint = new SKPaint { Color = new SKColor(255, 235, 200, 120), StrokeWidth = 2, IsAntialias = true };
            for (var y = (int)(Size * 0.62f); y < Size; y += 8)
            {
                var len = rng.Next(20, 70);
                canvas.DrawLine(sunX - len / 2, y, sunX + len / 2, y, glint);
            }
        }

        private static void DrawFlowers(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: false, light: true);

            var count = rng.Next(5, 9);
            for (var i = 0; i < count; i++)
            {
                var cx = rng.Next(20, Size - 20);
                var cy = rng.Next(20, Size - 20);
                var petals = rng.Next(5, 9);
                var radius = rng.Next(18, 40);
                var hue = rng.Next(0, 360);

                using var petalPaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = HsvToSkColor(hue, 0.6f, 0.9f).WithAlpha(220)
                };
                for (var p = 0; p < petals; p++)
                {
                    var angle = p * 2 * Math.PI / petals;
                    var px = cx + (float)(Math.Cos(angle) * radius * 0.6);
                    var py = cy + (float)(Math.Sin(angle) * radius * 0.6);
                    canvas.DrawCircle(px, py, radius * 0.5f, petalPaint);
                }
                using var center = new SKPaint
                {
                    IsAntialias = true,
                    Color = HsvToSkColor((hue + 40) % 360, 0.8f, 1f)
                };
                canvas.DrawCircle(cx, cy, radius * 0.35f, center);
            }
        }

        private static void DrawImpressionist(SKCanvas canvas, Random rng)
        {
            var palette = GeneratePalette(rng);
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(Size, Size), palette, null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            var baseHue = rng.Next(0, 360);
            using var dab = new SKPaint { IsAntialias = true };
            for (var i = 0; i < 900; i++)
            {
                var hue = (baseHue + rng.Next(-50, 50) + 360) % 360;
                dab.Color = HsvToSkColor(hue, 0.5f + (float)rng.NextDouble() * 0.4f,
                    0.4f + (float)rng.NextDouble() * 0.5f).WithAlpha((byte)rng.Next(90, 180));
                var x = rng.Next(0, Size);
                var y = rng.Next(0, Size);
                canvas.DrawCircle(x, y, rng.Next(3, 8), dab);
            }
        }

        private static void DrawAurora(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: true);

            using (var star = new SKPaint { Color = new SKColor(255, 255, 255, 180), IsAntialias = true })
            {
                for (var i = 0; i < 60; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size / 2), rng.Next(1, 3), star);
            }

            for (var ribbon = 0; ribbon < 4; ribbon++)
            {
                var hue = rng.Next(90, 300);
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = rng.Next(14, 30),
                    Color = HsvToSkColor(hue, 0.7f, 0.85f).WithAlpha(70)
                };
                using var path = new SKPath();
                var yBase = rng.Next(60, Size - 120);
                var amp = rng.Next(20, 55);
                var freq = rng.NextDouble() * 0.02 + 0.01;
                var phase = rng.NextDouble() * Math.PI;
                path.MoveTo(0, yBase);
                for (var x = 0; x <= Size; x += 6)
                    path.LineTo(x, yBase + (float)(amp * Math.Sin(x * freq + phase)));
                canvas.DrawPath(path, paint);
            }
        }

        private static void DrawPeople(SKCanvas canvas, Random rng)
        {
            if (rng.NextDouble() > 0.5) DrawSunsetSea(canvas, rng);
            else { VerticalSky(canvas, rng, dusk: rng.NextDouble() > 0.5); DrawCelestialBody(canvas, rng); }

            var groundY = Size * (0.82f + (float)rng.NextDouble() * 0.08f);
            using (var ground = new SKPaint { IsAntialias = true, Color = new SKColor(15, 15, 20, 230) })
                canvas.DrawRect(0, groundY, Size, Size - groundY, ground);

            using var body = new SKPaint { IsAntialias = true, Color = new SKColor(10, 10, 15, 245) };
            var count = rng.Next(1, 4);
            for (var i = 0; i < count; i++)
            {
                var scale = 0.7f + (float)rng.NextDouble() * 0.6f;
                var x = rng.Next(50, Size - 50);
                var h = 110f * scale;
                var headR = 12f * scale;
                var top = groundY - h;

                canvas.DrawCircle(x, top, headR, body);
                using var torso = new SKPath();
                torso.MoveTo(x - 10 * scale, top + headR);
                torso.LineTo(x + 10 * scale, top + headR);
                torso.LineTo(x + 13 * scale, groundY);
                torso.LineTo(x - 13 * scale, groundY);
                torso.Close();
                canvas.DrawPath(torso, body);
            }
        }

        private static void DrawAnimals(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: rng.NextDouble() > 0.4);
            DrawCelestialBody(canvas, rng);

            using var bird = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(20, 20, 25, 235)
            };

            var flock = rng.Next(6, 12);
            for (var i = 0; i < flock; i++)
            {
                var cx = rng.Next(20, Size - 20);
                var cy = rng.Next(40, (int)(Size * 0.6f));
                var w = rng.Next(14, 34);
                using var path = new SKPath();
                path.MoveTo(cx - w, cy);
                path.QuadTo(cx - w / 2f, cy - w * 0.7f, cx, cy);
                path.QuadTo(cx + w / 2f, cy - w * 0.7f, cx + w, cy);
                canvas.DrawPath(path, bird);
            }

            using var solid = new SKPaint { IsAntialias = true, Color = new SKColor(12, 12, 16, 245) };
            var bx = rng.Next(60, Size - 60);
            var by = Size * 0.72f;
            canvas.DrawOval(new SKRect(bx - 30, by - 18, bx + 30, by + 18), solid);
            canvas.DrawCircle(bx + 28, by - 14, 11, solid);
            using var tail = new SKPath();
            tail.MoveTo(bx - 28, by);
            tail.LineTo(bx - 60, by + 6);
            tail.LineTo(bx - 28, by + 12);
            tail.Close();
            canvas.DrawPath(tail, solid);
        }

        private static void DrawPlanet(SKCanvas canvas, Random rng)
        {
            using (var space = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size / 2f), Size * 0.9f,
                    [HsvToSkColor(rng.Next(220, 260), 0.6f, 0.2f), new SKColor(4, 4, 12)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, space);
            }

            using (var star = new SKPaint { Color = new SKColor(255, 255, 255, 220), IsAntialias = true })
            {
                for (var i = 0; i < 120; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 1.6f + 0.4f, star);
            }

            var px = rng.Next(120, 280);
            var py = rng.Next(120, 260);
            var pr = rng.Next(70, 110);

            using (var glow = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(px, py), pr * 1.4f,
                    [HsvToSkColor(205, 0.8f, 0.9f).WithAlpha(120), new SKColor(0, 0, 0, 0)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(px, py, pr * 1.4f, glow);
            }

            using (var ocean = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(px - pr * 0.3f, py - pr * 0.3f), pr * 1.4f,
                    [HsvToSkColor(205, 0.7f, 0.85f), HsvToSkColor(220, 0.85f, 0.35f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(px, py, pr, ocean);
            }

            canvas.Save();
            using (var clip = new SKPath())
            {
                clip.AddCircle(px, py, pr);
                canvas.ClipPath(clip, antialias: true);
                using var land = new SKPaint { IsAntialias = true };
                var blobs = rng.Next(4, 8);
                for (var i = 0; i < blobs; i++)
                {
                    land.Color = HsvToSkColor(rng.Next(90, 140), 0.6f, 0.5f).WithAlpha(220);
                    var lx = px + rng.Next(-pr, pr);
                    var ly = py + rng.Next(-pr, pr);
                    using var blob = new SKPath();
                    var pts = rng.Next(5, 8);
                    var rad = rng.Next(15, 40);
                    for (var p = 0; p < pts; p++)
                    {
                        var ang = p * 2 * Math.PI / pts;
                        var rr = rad * (0.6 + rng.NextDouble() * 0.6);
                        var qx = lx + (float)(Math.Cos(ang) * rr);
                        var qy = ly + (float)(Math.Sin(ang) * rr);
                        if (p == 0) blob.MoveTo(qx, qy); else blob.LineTo(qx, qy);
                    }
                    blob.Close();
                    canvas.DrawPath(blob, land);
                }
            }
            canvas.Restore();
        }

        private static void DrawForest(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: rng.NextDouble() > 0.4);
            DrawCelestialBody(canvas, rng);

            for (var layer = 0; layer < 3; layer++)
            {
                var baseY = Size * (0.6f + layer * 0.14f);
                var shade = 0.1f + layer * 0.06f;
                using var paint = new SKPaint { IsAntialias = true, Color = HsvToSkColor(rng.Next(110, 160), 0.5f, shade) };

                var x = -10f;
                while (x < Size + 10)
                {
                    var th = rng.Next(50, 110) * (1f + layer * 0.2f);
                    var tw = th * 0.5f;
                    using var tree = new SKPath();
                    for (var tier = 0; tier < 3; tier++)
                    {
                        var ty = baseY - tier * th * 0.28f;
                        var tWidth = tw * (1f - tier * 0.22f);
                        tree.MoveTo(x - tWidth, ty);
                        tree.LineTo(x, ty - th * 0.45f);
                        tree.LineTo(x + tWidth, ty);
                        tree.Close();
                    }
                    canvas.DrawPath(tree, paint);
                    x += rng.Next(28, 55);
                }
            }
        }

        private static void DrawDesert(SKCanvas canvas, Random rng)
        {
            var hue = rng.Next(20, 45);
            using (var sky = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(hue, 0.5f, 0.95f), HsvToSkColor((hue + 20) % 360, 0.75f, 0.7f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }

            using (var sun = new SKPaint { IsAntialias = true, Color = new SKColor(255, 235, 190, 235) })
                canvas.DrawCircle(rng.Next(100, 300), rng.Next(80, 150), rng.Next(38, 55), sun);

            for (var d = 0; d < 3; d++)
            {
                var baseY = Size * (0.6f + d * 0.13f);
                using var dune = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.55f, 0.6f - d * 0.15f) };
                using var path = new SKPath();
                path.MoveTo(0, Size);
                path.LineTo(0, baseY);
                for (var x = 0; x <= Size; x += 10)
                    path.LineTo(x, baseY + (float)(Math.Sin(x * 0.012 + d) * 18));
                path.LineTo(Size, Size);
                path.Close();
                canvas.DrawPath(path, dune);
            }

            using var cactus = new SKPaint { IsAntialias = true, Color = HsvToSkColor(130, 0.5f, 0.35f) };
            var cx = rng.Next(60, Size - 60);
            var cy = Size * 0.82f;
            using var stem = new SKPath();
            stem.AddRoundRect(new SKRoundRect(new SKRect(cx - 9, cy - 90, cx + 9, cy), 9, 9));
            stem.AddRoundRect(new SKRoundRect(new SKRect(cx + 9, cy - 55, cx + 30, cy - 40), 7, 7));
            stem.AddRoundRect(new SKRoundRect(new SKRect(cx + 21, cy - 70, cx + 30, cy - 40), 5, 5));
            stem.AddRoundRect(new SKRoundRect(new SKRect(cx - 30, cy - 65, cx - 9, cy - 50), 7, 7));
            stem.AddRoundRect(new SKRoundRect(new SKRect(cx - 30, cy - 80, cx - 21, cy - 50), 5, 5));
            canvas.DrawPath(stem, cactus);
        }

        private static void DrawBalloons(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: false);
            using (var cloud = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 90) })
            {
                for (var i = 0; i < 5; i++)
                {
                    var cy = rng.Next(40, Size - 40);
                    var cx = rng.Next(0, Size);
                    canvas.DrawOval(new SKRect(cx - 40, cy - 14, cx + 40, cy + 14), cloud);
                }
            }

            var count = rng.Next(2, 5);
            for (var i = 0; i < count; i++)
            {
                var x = rng.Next(40, Size - 40);
                var y = rng.Next(60, Size - 120);
                var r = rng.Next(26, 44);
                var hue = rng.Next(0, 360);

                using var balloon = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(x - r * 0.3f, y - r * 0.3f), r * 1.6f,
                        [HsvToSkColor(hue, 0.7f, 0.95f), HsvToSkColor(hue, 0.85f, 0.55f)],
                        null, SKShaderTileMode.Clamp)
                };
                using var bulb = new SKPath();
                bulb.AddOval(new SKRect(x - r, y - r, x + r, y + r * 1.1f));
                bulb.MoveTo(x - r * 0.4f, y + r);
                bulb.LineTo(x, y + r * 1.5f);
                bulb.LineTo(x + r * 0.4f, y + r);
                bulb.Close();
                canvas.DrawPath(bulb, balloon);

                using var basket = new SKPaint { IsAntialias = true, Color = new SKColor(90, 60, 30) };
                canvas.DrawRect(x - 6, y + r * 1.7f, 12, 10, basket);
                using var line = new SKPaint { Color = new SKColor(40, 40, 40, 160), StrokeWidth = 1.5f, IsAntialias = true };
                canvas.DrawLine(x - 6, y + r * 1.5f, x - 5, y + r * 1.7f, line);
                canvas.DrawLine(x + 6, y + r * 1.5f, x + 5, y + r * 1.7f, line);
            }
        }

        private static void DrawSynthwave(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size * 0.6f),
                    [HsvToSkColor(265, 0.7f, 0.35f), HsvToSkColor(320, 0.7f, 0.5f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size * 0.6f, sky);
            }

            var sunCx = Size / 2f;
            var sunCy = Size * 0.5f;
            var sunR = 90f;
            canvas.Save();
            using (var clip = new SKPath())
            {
                clip.AddCircle(sunCx, sunCy, sunR);
                canvas.ClipPath(clip, antialias: true);
                using var sun = new SKPaint
                {
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, sunCy - sunR), new SKPoint(0, sunCy + sunR),
                        [HsvToSkColor(50, 0.9f, 1f), HsvToSkColor(330, 0.9f, 0.9f)],
                        null, SKShaderTileMode.Clamp)
                };
                canvas.DrawRect(0, 0, Size, Size, sun);
                using var gap = new SKPaint { Color = HsvToSkColor(285, 0.7f, 0.2f), IsAntialias = true };
                var stripe = sunCy;
                var gapH = 6f;
                while (stripe < sunCy + sunR)
                {
                    canvas.DrawRect(0, stripe, Size, gapH, gap);
                    gapH += 1.5f;
                    stripe += 14;
                }
            }
            canvas.Restore();

            using (var floor = new SKPaint { Color = HsvToSkColor(280, 0.6f, 0.12f), IsAntialias = true })
                canvas.DrawRect(0, Size * 0.6f, Size, Size * 0.4f, floor);

            using var grid = new SKPaint { Color = HsvToSkColor(320, 0.9f, 0.9f).WithAlpha(160), StrokeWidth = 1.5f, IsAntialias = true };
            var horizon = Size * 0.6f;
            for (var i = -6; i <= 6; i++)
                canvas.DrawLine(sunCx, horizon, sunCx + i * 70, Size, grid);
            var y = horizon;
            var step = 6f;
            while (y < Size)
            {
                canvas.DrawLine(0, y, Size, y, grid);
                step *= 1.35f;
                y += step;
            }
        }

        private static void DrawButterflies(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: false, light: true);

            var count = rng.Next(5, 9);
            for (var i = 0; i < count; i++)
            {
                var cx = rng.Next(30, Size - 30);
                var cy = rng.Next(30, Size - 30);
                var s = rng.Next(14, 30);
                var hue = rng.Next(0, 360);
                using var wing = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.7f, 0.9f).WithAlpha(225) };
                canvas.DrawOval(new SKRect(cx - s, cy - s, cx, cy), wing);
                canvas.DrawOval(new SKRect(cx, cy - s, cx + s, cy), wing);
                canvas.DrawOval(new SKRect(cx - s * 0.8f, cy, cx, cy + s * 0.8f), wing);
                canvas.DrawOval(new SKRect(cx, cy, cx + s * 0.8f, cy + s * 0.8f), wing);
                using var body = new SKPaint { IsAntialias = true, Color = new SKColor(30, 30, 30, 220) };
                canvas.DrawRect(cx - 1.5f, cy - s, 3, s * 1.8f, body);
            }
        }

        private static void DrawVinyl(SKCanvas canvas, Random rng)
        {
            var palette = GeneratePalette(rng);
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(Size, Size), palette, null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            var cx = Size / 2f;
            var cy = Size * 0.45f;
            var r = 130f;

            using (var disc = new SKPaint { IsAntialias = true, Color = new SKColor(18, 18, 18) })
                canvas.DrawCircle(cx, cy, r, disc);

            using var groove = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = new SKColor(60, 60, 60, 160) };
            for (var rr = 30f; rr < r; rr += 6)
                canvas.DrawCircle(cx, cy, rr, groove);

            using (var label = new SKPaint { IsAntialias = true, Color = HsvToSkColor(rng.Next(0, 360), 0.8f, 0.9f) })
                canvas.DrawCircle(cx, cy, 28, label);
            using (var hole = new SKPaint { IsAntialias = true, Color = new SKColor(18, 18, 18) })
                canvas.DrawCircle(cx, cy, 5, hole);

            using var sheen = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(cx - r, cy - r), new SKPoint(cx + r, cy + r),
                    [new SKColor(255, 255, 255, 60), new SKColor(255, 255, 255, 0)],
                    null, SKShaderTileMode.Clamp)
            };
            canvas.DrawCircle(cx, cy, r, sheen);
        }

        private static void DrawLighthouse(SKCanvas canvas, Random rng)
        {
            DrawSunsetSea(canvas, rng);

            var x = rng.Next(70, Size - 70);
            var baseY = Size * 0.62f;
            var h = 130f;
            var topY = baseY - h;

            using (var white = new SKPaint { IsAntialias = true, Color = new SKColor(240, 240, 240) })
            {
                using var tower = new SKPath();
                tower.MoveTo(x - 18, baseY);
                tower.LineTo(x - 12, topY);
                tower.LineTo(x + 12, topY);
                tower.LineTo(x + 18, baseY);
                tower.Close();
                canvas.DrawPath(tower, white);
            }
            using (var red = new SKPaint { IsAntialias = true, Color = new SKColor(200, 40, 45) })
            {
                for (var band = 0; band < 4; band++)
                {
                    var by = topY + band * (h / 4f) + (h / 8f);
                    var t = (by - topY) / h;
                    var halfW = 12 + 6 * t;
                    canvas.DrawRect(x - halfW, by, halfW * 2, h / 8f, red);
                }
            }
            using (var lamp = new SKPaint { IsAntialias = true, Color = new SKColor(255, 235, 150) })
                canvas.DrawRect(x - 10, topY - 16, 20, 16, lamp);
            using var beam = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(255, 240, 180, 90)
            };
            using var beamPath = new SKPath();
            beamPath.MoveTo(x, topY - 8);
            beamPath.LineTo(Size, topY - 50);
            beamPath.LineTo(Size, topY + 40);
            beamPath.Close();
            canvas.DrawPath(beamPath, beam);
        }

        private static void StudioBackground(SKCanvas canvas, Random rng)
        {
            var hue = rng.Next(0, 360);
            using var paint = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size * 0.4f), Size * 0.85f,
                    [HsvToSkColor(hue, 0.55f, 0.5f), HsvToSkColor((hue + 25) % 360, 0.7f, 0.18f)],
                    null, SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(0, 0, Size, Size, paint);
        }

        private static void DrawGuitar(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var cx = Size * 0.42f;
            var cy = Size * 0.62f;
            var wood = HsvToSkColor(rng.Next(20, 40), 0.7f, 0.6f);

            using (var body = new SKPaint { IsAntialias = true, Color = wood })
            {
                canvas.DrawCircle(cx, cy + 28, 78, body);
                canvas.DrawCircle(cx, cy - 34, 58, body);
            }
            using (var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 5, Color = new SKColor(20, 12, 8) })
                canvas.DrawCircle(cx, cy, 26, ring);
            using (var hole = new SKPaint { IsAntialias = true, Color = new SKColor(15, 10, 6) })
                canvas.DrawCircle(cx, cy, 21, hole);
            using (var bridge = new SKPaint { IsAntialias = true, Color = new SKColor(30, 18, 10) })
                canvas.DrawRect(cx - 22, cy + 52, 44, 10, bridge);

            var nx = cx + 50;
            var ny = cy - 70;
            using (var neck = new SKPaint { IsAntialias = true, Color = new SKColor(45, 28, 16) })
            {
                canvas.Save();
                canvas.RotateDegrees(35, nx, ny);
                canvas.DrawRect(nx - 12, ny - 150, 24, 160, neck);
                using var head = new SKPaint { IsAntialias = true, Color = new SKColor(30, 18, 10) };
                canvas.DrawRect(nx - 15, ny - 178, 30, 34, head);
                using var fret = new SKPaint { Color = new SKColor(200, 200, 200, 180), StrokeWidth = 1.5f };
                for (var f = 0; f < 7; f++)
                    canvas.DrawLine(nx - 12, ny - 20 - f * 18, nx + 12, ny - 20 - f * 18, fret);
                using var str = new SKPaint { Color = new SKColor(230, 230, 230, 150), StrokeWidth = 1 };
                for (var s = -2; s <= 2; s++)
                    canvas.DrawLine(nx + s * 5, ny - 150, nx + s * 5, ny + 8, str);
                canvas.Restore();
            }
        }

        private static void DrawPianoKeys(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var top = Size * 0.32f;
            var height = Size * 0.5f;
            const int whiteCount = 10;
            var keyW = (float)Size / whiteCount;

            using (var shadow = new SKPaint { Color = new SKColor(0, 0, 0, 120) })
                canvas.DrawRect(0, top - 14, Size, 14, shadow);

            using (var white = new SKPaint { IsAntialias = true, Color = new SKColor(248, 248, 245) })
            using (var edge = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f, Color = new SKColor(120, 120, 120) })
            {
                for (var i = 0; i < whiteCount; i++)
                {
                    var x = i * keyW;
                    canvas.DrawRect(x, top, keyW - 1, height, white);
                    canvas.DrawRect(x, top, keyW - 1, height, edge);
                }
            }

            using (var black = new SKPaint { IsAntialias = true, Color = new SKColor(18, 18, 18) })
            {
                var blackW = keyW * 0.6f;
                for (var i = 0; i < whiteCount - 1; i++)
                {
                    var posInOctave = i % 7;
                    if (posInOctave == 2 || posInOctave == 6) continue;
                    var x = (i + 1) * keyW - blackW / 2;
                    canvas.DrawRect(x, top, blackW, height * 0.62f, black);
                }
            }
        }

        private static void DrawMicrophone(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var cx = Size / 2f;
            var cy = Size * 0.4f;

            using (var glow = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(cx, cy), 130,
                    [new SKColor(255, 255, 255, 70), new SKColor(255, 255, 255, 0)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(cx, cy, 130, glow);
            }

            using (var grille = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(cx - 16, cy - 16), 70,
                    [new SKColor(200, 205, 215), new SKColor(70, 75, 85)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(cx, cy, 50, grille);
            }
            using (var mesh = new SKPaint { Color = new SKColor(30, 30, 35, 140), StrokeWidth = 1.5f })
            {
                for (var gx = -45; gx <= 45; gx += 9)
                    canvas.DrawLine(cx + gx, cy - 48, cx + gx, cy + 48, mesh);
                for (var gy = -45; gy <= 45; gy += 9)
                    canvas.DrawLine(cx - 48, cy + gy, cx + 48, cy + gy, mesh);
            }

            using (var body = new SKPaint { IsAntialias = true, Color = new SKColor(40, 42, 48) })
            {
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(cx - 18, cy + 44, cx + 18, cy + 110), 8, 8), body);
                canvas.DrawRect(cx - 4, cy + 110, 8, Size - (cy + 110), body);
            }
        }

        private static void DrawHeadphones(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var cx = Size / 2f;
            var cy = Size * 0.5f;
            var span = 150f;
            var accent = HsvToSkColor(rng.Next(0, 360), 0.7f, 0.85f);

            using (var band = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 22,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(35, 35, 40)
            })
            {
                var rect = new SKRect(cx - span / 2, cy - span / 2, cx + span / 2, cy + span / 2);
                canvas.DrawArc(rect, 180, 180, false, band);
            }

            foreach (var side in new[] { -1, 1 })
            {
                var ex = cx + side * span / 2;
                var ey = cy;
                using var cup = new SKPaint { IsAntialias = true, Color = new SKColor(28, 28, 32) };
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(ex - 26, ey - 34, ex + 26, ey + 50), 16, 16), cup);
                using var pad = new SKPaint { IsAntialias = true, Color = accent };
                canvas.DrawCircle(ex, ey + 8, 16, pad);
            }
        }

        private static void DrawTrumpet(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var brass = HsvToSkColor(45, 0.85f, 0.85f);
            var brassDark = HsvToSkColor(40, 0.9f, 0.55f);
            var cy = Size * 0.5f;

            canvas.Save();
            canvas.RotateDegrees(-12, Size / 2f, cy);

            using (var tube = new SKPaint { IsAntialias = true, Color = brass, Style = SKPaintStyle.Stroke, StrokeWidth = 16, StrokeCap = SKStrokeCap.Round })
                canvas.DrawLine(70, cy, 300, cy, tube);

            using (var bell = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(290, cy - 50), new SKPoint(360, cy + 50),
                    [brass, brassDark], null, SKShaderTileMode.Clamp)
            })
            {
                using var path = new SKPath();
                path.MoveTo(295, cy - 12);
                path.LineTo(370, cy - 48);
                path.LineTo(370, cy + 48);
                path.LineTo(295, cy + 12);
                path.Close();
                canvas.DrawPath(path, bell);
            }

            using (var mp = new SKPaint { IsAntialias = true, Color = brassDark })
                canvas.DrawCircle(66, cy, 12, mp);

            using (var valve = new SKPaint { IsAntialias = true, Color = brassDark })
            {
                for (var v = 0; v < 3; v++)
                    canvas.DrawRect(150 + v * 34, cy - 40, 12, 34, valve);
            }
            canvas.Restore();
        }

        private static void DrawMusicNotes(SKCanvas canvas, Random rng)
        {
            var palette = GeneratePalette(rng);
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(Size, Size), palette, null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            using (var staff = new SKPaint { Color = new SKColor(255, 255, 255, 70), StrokeWidth = 2 })
            {
                for (var i = 0; i < 5; i++)
                    canvas.DrawLine(0, Size * 0.38f + i * 16, Size, Size * 0.38f + i * 16, staff);
            }

            var count = rng.Next(6, 11);
            for (var i = 0; i < count; i++)
            {
                var x = rng.Next(30, Size - 50);
                var y = rng.Next(40, Size - 60);
                var col = new SKColor(255, 255, 255, (byte)rng.Next(180, 255));
                using var note = new SKPaint { IsAntialias = true, Color = col };
                canvas.Save();
                canvas.RotateDegrees(-20, x, y);
                canvas.DrawOval(new SKRect(x - 11, y - 8, x + 11, y + 8), note);
                canvas.Restore();
                using var stem = new SKPaint { Color = col, StrokeWidth = 3 };
                canvas.DrawLine(x + 10, y - 2, x + 10, y - 46, stem);
                using var flag = new SKPath();
                flag.MoveTo(x + 10, y - 46);
                flag.QuadTo(x + 30, y - 38, x + 22, y - 18);
                flag.QuadTo(x + 26, y - 34, x + 10, y - 34);
                flag.Close();
                canvas.DrawPath(flag, note);
            }
        }

        private static void DrawCassette(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var bodyColor = HsvToSkColor(rng.Next(0, 360), 0.6f, 0.8f);
            var rect = new SKRect(Size * 0.12f, Size * 0.3f, Size * 0.88f, Size * 0.68f);

            using (var shadow = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 90) })
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(rect.Left + 6, rect.Top + 8, rect.Right + 6, rect.Bottom + 8), 14, 14), shadow);
            using (var body = new SKPaint { IsAntialias = true, Color = bodyColor })
                canvas.DrawRoundRect(new SKRoundRect(rect, 14, 14), body);

            var label = new SKRect(rect.Left + 18, rect.Top + 14, rect.Right - 18, rect.MidY - 6);
            using (var lbl = new SKPaint { IsAntialias = true, Color = new SKColor(245, 245, 240) })
                canvas.DrawRoundRect(new SKRoundRect(label, 6, 6), lbl);
            using (var lines = new SKPaint { Color = new SKColor(150, 150, 150), StrokeWidth = 2 })
            {
                canvas.DrawLine(label.Left + 8, label.Top + 14, label.Right - 8, label.Top + 14, lines);
                canvas.DrawLine(label.Left + 8, label.Top + 26, label.Right - 30, label.Top + 26, lines);
            }

            var win = new SKRect(rect.Left + 40, rect.MidY + 4, rect.Right - 40, rect.Bottom - 14);
            using (var window = new SKPaint { IsAntialias = true, Color = new SKColor(30, 30, 35, 200) })
                canvas.DrawRoundRect(new SKRoundRect(win, 8, 8), window);
            using (var reel = new SKPaint { IsAntialias = true, Color = new SKColor(220, 220, 220) })
            {
                canvas.DrawCircle(win.Left + 34, win.MidY, 18, reel);
                canvas.DrawCircle(win.Right - 34, win.MidY, 18, reel);
            }
            using (var hub = new SKPaint { IsAntialias = true, Color = new SKColor(60, 60, 65) })
            {
                canvas.DrawCircle(win.Left + 34, win.MidY, 7, hub);
                canvas.DrawCircle(win.Right - 34, win.MidY, 7, hub);
            }
        }

        private static void DrawSaxophone(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var brass = HsvToSkColor(45, 0.85f, 0.9f);
            var brassDark = HsvToSkColor(38, 0.9f, 0.55f);

            using (var body = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 22,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(Size * 0.3f, 0), new SKPoint(Size * 0.7f, Size),
                    [brass, brassDark], null, SKShaderTileMode.Clamp)
            })
            {
                using var path = new SKPath();
                path.MoveTo(Size * 0.56f, Size * 0.2f);
                path.LineTo(Size * 0.5f, Size * 0.52f);
                path.QuadTo(Size * 0.45f, Size * 0.8f, Size * 0.63f, Size * 0.81f);
                path.QuadTo(Size * 0.8f, Size * 0.82f, Size * 0.75f, Size * 0.62f);
                canvas.DrawPath(path, body);
            }

            using (var bell = new SKPaint { IsAntialias = true, Color = brass })
            {
                using var path = new SKPath();
                path.MoveTo(Size * 0.67f, Size * 0.6f);
                path.LineTo(Size * 0.88f, Size * 0.5f);
                path.LineTo(Size * 0.85f, Size * 0.68f);
                path.LineTo(Size * 0.73f, Size * 0.66f);
                path.Close();
                canvas.DrawPath(path, bell);
            }

            using (var key = new SKPaint { IsAntialias = true, Color = brassDark })
                for (var i = 0; i < 6; i++)
                    canvas.DrawCircle(Size * 0.5f + i * 1.5f, Size * 0.3f + i * 24, 6, key);
            using (var mp = new SKPaint { IsAntialias = true, Color = new SKColor(25, 25, 30) })
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(Size * 0.52f, Size * 0.13f, Size * 0.62f, Size * 0.21f), 4, 4), mp);
        }

        private static void DrawDrumKit(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var shellHue = rng.Next(0, 360);
            var shell = HsvToSkColor(shellHue, 0.7f, 0.7f);
            var shellDark = HsvToSkColor(shellHue, 0.8f, 0.4f);
            var cx = Size / 2f;
            var cy = Size * 0.62f;

            using (var rod = new SKPaint { Color = new SKColor(180, 180, 185), StrokeWidth = 3, IsAntialias = true })
            {
                canvas.DrawLine(cx - 95, cy - 112, cx - 70, cy + 40, rod);
                canvas.DrawLine(cx + 95, cy - 122, cx + 70, cy + 40, rod);
            }
            using (var cym = new SKPaint { IsAntialias = true, Color = HsvToSkColor(48, 0.7f, 0.78f) })
            {
                canvas.DrawOval(new SKRect(cx - 150, cy - 120, cx - 40, cy - 104), cym);
                canvas.DrawOval(new SKRect(cx + 40, cy - 130, cx + 150, cy - 114), cym);
            }

            foreach (var side in new[] { -1, 1 })
            {
                var tx = cx + side * 62;
                var ty = cy - 42;
                using var tom = new SKPaint { IsAntialias = true, Color = shell };
                canvas.DrawOval(new SKRect(tx - 34, ty - 22, tx + 34, ty + 22), tom);
                using var th = new SKPaint { IsAntialias = true, Color = new SKColor(245, 245, 240, 230) };
                canvas.DrawOval(new SKRect(tx - 26, ty - 15, tx + 26, ty + 15), th);
            }

            using (var bass = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(cx, cy + 30), 95, [shell, shellDark], null, SKShaderTileMode.Clamp)
            })
                canvas.DrawCircle(cx, cy + 30, 90, bass);
            using (var rim = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 8, Color = new SKColor(230, 230, 235) })
                canvas.DrawCircle(cx, cy + 30, 90, rim);
            using (var head = new SKPaint { IsAntialias = true, Color = new SKColor(245, 245, 240, 235) })
                canvas.DrawCircle(cx, cy + 30, 56, head);
        }

        private static void DrawEqualizer(SKCanvas canvas, Random rng)
        {
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [new SKColor(12, 12, 18), HsvToSkColor(rng.Next(0, 360), 0.5f, 0.2f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            const int bars = 16;
            var bw = (float)Size / bars;
            var baseHue = rng.Next(0, 360);
            var baseY = Size * 0.82f;
            for (var i = 0; i < bars; i++)
            {
                var h = rng.Next(30, 230);
                var hue = (baseHue + i * 12) % 360;
                using var bar = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, baseY - h), new SKPoint(0, baseY),
                        [HsvToSkColor(hue, 0.8f, 0.95f), HsvToSkColor(hue, 0.9f, 0.5f)],
                        null, SKShaderTileMode.Clamp)
                };
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(i * bw + 3, baseY - h, (i + 1) * bw - 3, baseY), 3, 3), bar);
                using var refl = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.8f, 0.9f).WithAlpha(60) };
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(i * bw + 3, baseY, (i + 1) * bw - 3, baseY + h * 0.3f), 3, 3), refl);
            }
        }

        private static void DrawBoombox(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var body = new SKRect(Size * 0.1f, Size * 0.32f, Size * 0.9f, Size * 0.72f);
            using (var shadow = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 90) })
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(body.Left + 6, body.Top + 8, body.Right + 6, body.Bottom + 8), 16, 16), shadow);
            using (var box = new SKPaint { IsAntialias = true, Color = new SKColor(40, 42, 48) })
                canvas.DrawRoundRect(new SKRoundRect(body, 16, 16), box);

            using (var handle = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 8, Color = new SKColor(60, 62, 68) })
                canvas.DrawArc(new SKRect(Size * 0.35f, Size * 0.18f, Size * 0.65f, Size * 0.42f), 180, 180, false, handle);

            foreach (var side in new[] { -1, 1 })
            {
                var sx = Size / 2f + side * Size * 0.24f;
                var sy = body.MidY + 10;
                using var ring = new SKPaint { IsAntialias = true, Color = HsvToSkColor(rng.Next(0, 360), 0.7f, 0.85f) };
                canvas.DrawCircle(sx, sy, 50, ring);
                using var cone = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(sx, sy), 42, [new SKColor(60, 60, 65), new SKColor(15, 15, 18)], null, SKShaderTileMode.Clamp)
                };
                canvas.DrawCircle(sx, sy, 42, cone);
                using var cap = new SKPaint { IsAntialias = true, Color = new SKColor(25, 25, 30) };
                canvas.DrawCircle(sx, sy, 12, cap);
            }

            using (var deck = new SKPaint { IsAntialias = true, Color = new SKColor(20, 20, 24) })
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(Size * 0.4f, body.Top + 12, Size * 0.6f, body.Top + 44), 4, 4), deck);
        }

        private static void DrawFireworks(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [new SKColor(8, 8, 24), new SKColor(22, 14, 42)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }

            var bursts = rng.Next(3, 6);
            for (var b = 0; b < bursts; b++)
            {
                var cx = rng.Next(60, Size - 60);
                var cy = rng.Next(50, (int)(Size * 0.6f));
                var hue = rng.Next(0, 360);
                var rays = rng.Next(18, 30);
                var len = rng.Next(40, 80);
                using var spark = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2.2f,
                    StrokeCap = SKStrokeCap.Round,
                    Color = HsvToSkColor(hue, 0.6f, 1f)
                };
                using var tip = new SKPaint { IsAntialias = true, Color = HsvToSkColor((hue + 30) % 360, 0.4f, 1f) };
                for (var i = 0; i < rays; i++)
                {
                    var ang = i * 2 * Math.PI / rays;
                    var ix = cx + (float)(Math.Cos(ang) * len * 0.35);
                    var iy = cy + (float)(Math.Sin(ang) * len * 0.35);
                    var ox = cx + (float)(Math.Cos(ang) * len);
                    var oy = cy + (float)(Math.Sin(ang) * len);
                    canvas.DrawLine(ix, iy, ox, oy, spark);
                    canvas.DrawCircle(ox, oy, 2.5f, tip);
                }
                using var core = new SKPaint { IsAntialias = true, Color = SKColors.White.WithAlpha(220) };
                canvas.DrawCircle(cx, cy, 4, core);
            }

            using (var skyline = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 235) })
            {
                var x = 0f;
                while (x < Size)
                {
                    var w = rng.Next(24, 50);
                    var h = rng.Next(30, 90);
                    canvas.DrawRect(x, Size - h, w, h, skyline);
                    x += w + rng.Next(0, 6);
                }
            }
        }

        private static void DrawWaves(SKCanvas canvas, Random rng)
        {
            var hue = rng.Next(180, 230);
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor((hue + 20) % 360, 0.4f, 0.95f), HsvToSkColor(hue, 0.6f, 0.7f)],
                    null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            const int layers = 6;
            for (var l = 0; l < layers; l++)
            {
                var baseY = Size * (0.35f + l * 0.11f);
                var amp = 22f + l * 4;
                var freq = 0.018 + rng.NextDouble() * 0.01;
                var phase = rng.NextDouble() * Math.PI * 2;

                using var wave = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.6f, 0.3f + l * 0.1f) };
                using var path = new SKPath();
                path.MoveTo(0, Size);
                path.LineTo(0, baseY);
                for (var x = 0; x <= Size; x += 8)
                    path.LineTo(x, baseY + (float)(Math.Sin(x * freq + phase) * amp));
                path.LineTo(Size, Size);
                path.Close();
                canvas.DrawPath(path, wave);

                using var foam = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2, Color = new SKColor(255, 255, 255, 90) };
                using var crest = new SKPath();
                crest.MoveTo(0, baseY);
                for (var x = 0; x <= Size; x += 8)
                    crest.LineTo(x, baseY + (float)(Math.Sin(x * freq + phase) * amp));
                canvas.DrawPath(crest, foam);
            }
        }

        private static void DrawCampfire(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: true);

            using (var star = new SKPaint { Color = new SKColor(255, 255, 255, 180), IsAntialias = true })
                for (var i = 0; i < 50; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, (int)(Size * 0.55f)), rng.Next(1, 3), star);

            var gx = Size / 2f;
            var gy = Size * 0.78f;

            using (var glow = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(gx, gy), 140,
                    [new SKColor(255, 150, 40, 120), new SKColor(255, 150, 40, 0)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(gx, gy, 140, glow);
            }

            using (var log = new SKPaint { IsAntialias = true, Color = new SKColor(70, 45, 25) })
            {
                canvas.Save();
                canvas.RotateDegrees(18, gx, gy);
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(gx - 55, gy - 6, gx + 55, gy + 8), 6, 6), log);
                canvas.Restore();
                canvas.Save();
                canvas.RotateDegrees(-18, gx, gy);
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(gx - 55, gy - 6, gx + 55, gy + 8), 6, 6), log);
                canvas.Restore();
            }

            void Flame(float h, float w, SKColor c)
            {
                using var f = new SKPaint { IsAntialias = true, Color = c };
                using var p = new SKPath();
                p.MoveTo(gx, gy - h);
                p.QuadTo(gx - w, gy - h * 0.4f, gx - w * 0.5f, gy);
                p.QuadTo(gx, gy - h * 0.2f, gx + w * 0.5f, gy);
                p.QuadTo(gx + w, gy - h * 0.4f, gx, gy - h);
                p.Close();
                canvas.DrawPath(p, f);
            }
            Flame(95, 38, new SKColor(220, 70, 20, 235));
            Flame(70, 26, new SKColor(255, 150, 30, 240));
            Flame(45, 16, new SKColor(255, 225, 120, 245));
        }

        private static void DrawSailboat(SKCanvas canvas, Random rng)
        {
            DrawSunsetSea(canvas, rng);

            var bx = rng.Next(120, Size - 120);
            var waterY = Size * 0.66f;

            using (var hull = new SKPaint { IsAntialias = true, Color = new SKColor(35, 25, 20) })
            {
                using var p = new SKPath();
                p.MoveTo(bx - 44, waterY);
                p.LineTo(bx + 44, waterY);
                p.LineTo(bx + 30, waterY + 20);
                p.LineTo(bx - 30, waterY + 20);
                p.Close();
                canvas.DrawPath(p, hull);
            }

            using (var mast = new SKPaint { Color = new SKColor(30, 22, 18), StrokeWidth = 3, IsAntialias = true })
                canvas.DrawLine(bx, waterY, bx, waterY - 110, mast);

            using (var sail = new SKPaint { IsAntialias = true, Color = new SKColor(245, 240, 235, 235) })
            {
                using var main = new SKPath();
                main.MoveTo(bx - 4, waterY - 108);
                main.LineTo(bx - 4, waterY - 6);
                main.LineTo(bx - 58, waterY - 6);
                main.Close();
                canvas.DrawPath(main, sail);

                using var jib = new SKPath();
                jib.MoveTo(bx + 4, waterY - 100);
                jib.LineTo(bx + 4, waterY - 6);
                jib.LineTo(bx + 48, waterY - 6);
                jib.Close();
                canvas.DrawPath(jib, sail);
            }
        }

        private static void DrawCherryBlossom(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: false, light: true);
            DrawCelestialBody(canvas, rng);

            using (var bark = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(70, 48, 40),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 16,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            })
            {
                using var trunk = new SKPath();
                trunk.MoveTo(Size * 0.18f, Size);
                trunk.QuadTo(Size * 0.3f, Size * 0.6f, Size * 0.5f, Size * 0.4f);
                canvas.DrawPath(trunk, bark);
            }
            using (var twig = new SKPaint { IsAntialias = true, Color = new SKColor(70, 48, 40), Style = SKPaintStyle.Stroke, StrokeWidth = 5, StrokeCap = SKStrokeCap.Round })
            {
                canvas.DrawLine(Size * 0.34f, Size * 0.6f, Size * 0.2f, Size * 0.42f, twig);
                canvas.DrawLine(Size * 0.42f, Size * 0.5f, Size * 0.62f, Size * 0.36f, twig);
                canvas.DrawLine(Size * 0.5f, Size * 0.4f, Size * 0.5f, Size * 0.24f, twig);
            }

            var hue = rng.Next(330, 350);
            var clusters = rng.Next(60, 90);
            for (var i = 0; i < clusters; i++)
            {
                var cx = rng.Next((int)(Size * 0.12f), (int)(Size * 0.75f));
                var cy = rng.Next((int)(Size * 0.16f), (int)(Size * 0.5f));
                var r = rng.Next(5, 11);
                using var petal = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.35f, 1f).WithAlpha(220) };
                for (var p = 0; p < 5; p++)
                {
                    var a = p * 2 * Math.PI / 5;
                    canvas.DrawCircle(cx + (float)(Math.Cos(a) * r * 0.5), cy + (float)(Math.Sin(a) * r * 0.5), r * 0.5f, petal);
                }
                using var center = new SKPaint { IsAntialias = true, Color = HsvToSkColor(50, 0.5f, 1f).WithAlpha(220) };
                canvas.DrawCircle(cx, cy, r * 0.28f, center);
            }

            using (var fp = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.3f, 1f).WithAlpha(180) })
                for (var i = 0; i < 18; i++)
                {
                    var px = rng.Next(0, Size);
                    var py = rng.Next((int)(Size * 0.45f), Size);
                    canvas.DrawOval(new SKRect(px - 4, py - 2, px + 4, py + 2), fp);
                }
        }

        private static void DrawNebula(SKCanvas canvas, Random rng)
        {
            using (var space = new SKPaint { Color = new SKColor(6, 6, 16) })
                canvas.DrawRect(0, 0, Size, Size, space);

            var hueA = rng.Next(0, 360);
            var hueB = (hueA + rng.Next(60, 180)) % 360;
            for (var i = 0; i < 26; i++)
            {
                var hue = (i % 2 == 0 ? hueA : hueB) + rng.Next(-20, 20);
                var cx = rng.Next(0, Size);
                var cy = rng.Next(0, Size);
                var r = rng.Next(60, 160);
                using var cloud = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(cx, cy), r,
                        [HsvToSkColor(hue, 0.7f, 0.7f).WithAlpha((byte)rng.Next(20, 55)), new SKColor(0, 0, 0, 0)],
                        null, SKShaderTileMode.Clamp)
                };
                canvas.DrawCircle(cx, cy, r, cloud);
            }

            using (var star = new SKPaint { IsAntialias = true })
                for (var i = 0; i < 160; i++)
                {
                    star.Color = new SKColor(255, 255, 255, (byte)rng.Next(120, 255));
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 1.6f + 0.3f, star);
                }

            using (var bright = new SKPaint { Color = new SKColor(255, 255, 255, 230), StrokeWidth = 1.5f, IsAntialias = true })
                for (var i = 0; i < 5; i++)
                {
                    var sx = rng.Next(20, Size - 20);
                    var sy = rng.Next(20, Size - 20);
                    canvas.DrawLine(sx - 7, sy, sx + 7, sy, bright);
                    canvas.DrawLine(sx, sy - 7, sx, sy + 7, bright);
                }
        }

        private static void DrawRainyWindow(SKCanvas canvas, Random rng)
        {
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [new SKColor(20, 28, 48), new SKColor(8, 12, 24)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            for (var i = 0; i < 26; i++)
            {
                var hue = rng.Next(0, 60) + (rng.NextDouble() > 0.5 ? 180 : 0);
                var x = rng.Next(0, Size);
                var y = rng.Next(0, Size);
                var r = rng.Next(10, 28);
                using var glow = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(x, y), r,
                        [HsvToSkColor(hue, 0.4f, 1f).WithAlpha(90), new SKColor(0, 0, 0, 0)], null, SKShaderTileMode.Clamp)
                };
                canvas.DrawCircle(x, y, r, glow);
            }

            using (var frame = new SKPaint { Color = new SKColor(15, 15, 20, 180), IsAntialias = true })
            {
                canvas.DrawRect(Size / 2f - 4, 0, 8, Size, frame);
                canvas.DrawRect(0, Size * 0.5f - 4, Size, 8, frame);
            }
            using (var streak = new SKPaint { Color = new SKColor(200, 220, 255, 70), StrokeWidth = 1.5f, IsAntialias = true })
                for (var i = 0; i < 42; i++)
                {
                    var x = rng.Next(0, Size);
                    var y = rng.Next(0, Size);
                    canvas.DrawLine(x, y, x - 3, y + rng.Next(8, 26), streak);
                }
            using (var drop = new SKPaint { Color = new SKColor(220, 235, 255, 120), IsAntialias = true })
                for (var i = 0; i < 30; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 3 + 1, drop);
        }

        private static void DrawSnowfall(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(210, 0.3f, 0.55f), HsvToSkColor(220, 0.25f, 0.8f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }
            DrawCelestialBody(canvas, rng);

            for (var l = 0; l < 3; l++)
            {
                var baseY = Size * (0.62f + l * 0.12f);
                using var hill = new SKPaint { IsAntialias = true, Color = HsvToSkColor(210, 0.12f, 0.95f - l * 0.12f) };
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(0, baseY);
                for (var x = 0; x <= Size; x += 10) p.LineTo(x, baseY + (float)(Math.Sin(x * 0.01 + l) * 16));
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, hill);
            }

            using (var pine = new SKPaint { IsAntialias = true, Color = new SKColor(30, 55, 45) })
                for (var i = 0; i < 5; i++)
                {
                    var x = rng.Next(30, Size - 30);
                    var by = Size * 0.7f;
                    var th = rng.Next(40, 70);
                    using var tr = new SKPath();
                    for (var tier = 0; tier < 3; tier++)
                    {
                        var ty = by - tier * th * 0.3f;
                        var tw = 18 * (1f - tier * 0.22f);
                        tr.MoveTo(x - tw, ty);
                        tr.LineTo(x, ty - th * 0.45f);
                        tr.LineTo(x + tw, ty);
                        tr.Close();
                    }
                    canvas.DrawPath(tr, pine);
                }

            using (var snow = new SKPaint { Color = SKColors.White.WithAlpha(220), IsAntialias = true })
                for (var i = 0; i < 80; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 2.4f + 0.6f, snow);
        }

        private static void DrawRainbow(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(205, 0.4f, 0.95f), HsvToSkColor(200, 0.25f, 0.75f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }

            var cx = Size * 0.5f;
            var cy = Size * 1.05f;
            var hues = new[] { 0, 35, 55, 130, 210, 260, 290 };
            using (var arc = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 9 })
                for (var i = 0; i < hues.Length; i++)
                {
                    arc.Color = HsvToSkColor(hues[i], 0.7f, 0.95f).WithAlpha(180);
                    var r = 260 - i * 9;
                    canvas.DrawArc(new SKRect(cx - r, cy - r, cx + r, cy + r), 180, 180, false, arc);
                }

            using (var cloud = new SKPaint { IsAntialias = true, Color = SKColors.White.WithAlpha(220) })
                for (var i = 0; i < 4; i++)
                {
                    var x = rng.Next(40, Size - 40);
                    var y = rng.Next(40, (int)(Size * 0.4f));
                    canvas.DrawOval(new SKRect(x - 44, y - 16, x + 44, y + 16), cloud);
                    canvas.DrawOval(new SKRect(x - 20, y - 28, x + 30, y + 8), cloud);
                }

            using (var hill = new SKPaint { IsAntialias = true, Color = HsvToSkColor(120, 0.5f, 0.5f) })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(0, Size * 0.78f);
                for (var x = 0; x <= Size; x += 10) p.LineTo(x, Size * 0.78f + (float)(Math.Sin(x * 0.012) * 20));
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, hill);
            }
        }

        private static void DrawWaterfall(SKCanvas canvas, Random rng)
        {
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(150, 0.4f, 0.5f), HsvToSkColor(190, 0.5f, 0.35f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            using (var cliff = new SKPaint { IsAntialias = true, Color = new SKColor(45, 55, 50) })
            {
                canvas.DrawRect(0, 0, Size * 0.32f, Size, cliff);
                canvas.DrawRect(Size * 0.68f, 0, Size * 0.32f, Size, cliff);
            }
            using (var water = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [new SKColor(225, 240, 255, 235), new SKColor(180, 210, 235, 235)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(Size * 0.32f, 0, Size * 0.36f, Size * 0.82f, water);
            }
            using (var streak = new SKPaint { Color = new SKColor(255, 255, 255, 120), StrokeWidth = 2, IsAntialias = true })
                for (var i = 0; i < 22; i++)
                {
                    var x = Size * 0.32f + (float)rng.NextDouble() * Size * 0.36f;
                    canvas.DrawLine(x, 0, x, Size * 0.8f, streak);
                }
            using (var pool = new SKPaint { IsAntialias = true, Color = new SKColor(200, 225, 240) })
                canvas.DrawOval(new SKRect(Size * 0.2f, Size * 0.78f, Size * 0.8f, Size), pool);
            using (var foam = new SKPaint { IsAntialias = true, Color = SKColors.White.WithAlpha(160) })
                for (var i = 0; i < 14; i++)
                    canvas.DrawCircle(rng.Next((int)(Size * 0.3f), (int)(Size * 0.7f)), rng.Next((int)(Size * 0.8f), (int)(Size * 0.92f)), rng.Next(6, 16), foam);
        }

        private static void DrawVolcano(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [new SKColor(40, 20, 40), new SKColor(120, 40, 30)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }

            using (var mtn = new SKPaint { IsAntialias = true, Color = new SKColor(30, 22, 28) })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(Size * 0.36f, Size * 0.32f);
                p.LineTo(Size * 0.44f, Size * 0.32f);
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, mtn);
            }
            using (var glow = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size * 0.4f, Size * 0.32f), 80,
                    [new SKColor(255, 180, 40, 200), new SKColor(255, 80, 20, 0)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(Size * 0.4f, Size * 0.32f, 80, glow);
            }
            using (var lava = new SKPaint { IsAntialias = true, Color = new SKColor(255, 120, 30) })
            {
                using var p = new SKPath();
                p.MoveTo(Size * 0.38f, Size * 0.33f);
                p.LineTo(Size * 0.34f, Size);
                p.LineTo(Size * 0.46f, Size);
                p.LineTo(Size * 0.42f, Size * 0.33f);
                p.Close();
                canvas.DrawPath(p, lava);
            }
            using (var spark = new SKPaint { IsAntialias = true, Color = new SKColor(255, 220, 120) })
                for (var i = 0; i < 24; i++)
                    canvas.DrawCircle(Size * 0.4f + rng.Next(-50, 50), Size * 0.32f - rng.Next(0, 90), (float)rng.NextDouble() * 2 + 1, spark);
            using (var smoke = new SKPaint { IsAntialias = true, Color = new SKColor(60, 50, 55, 120) })
                for (var i = 0; i < 6; i++)
                    canvas.DrawCircle(Size * 0.4f + rng.Next(-30, 40), Size * 0.18f - i * 16, 26 + i * 4, smoke);
        }

        private static void DrawGalaxy(SKCanvas canvas, Random rng)
        {
            using (var space = new SKPaint { Color = new SKColor(5, 4, 14) })
                canvas.DrawRect(0, 0, Size, Size, space);

            var cx = Size / 2f;
            var cy = Size / 2f;
            var hueA = rng.Next(0, 360);
            var hueB = (hueA + rng.Next(40, 120)) % 360;
            var arms = 2 + rng.Next(2);
            using (var star = new SKPaint { IsAntialias = true })
                for (var i = 0; i < 1400; i++)
                {
                    var arm = i % arms;
                    var t = rng.NextDouble();
                    var radius = t * Size * 0.5;
                    var angle = t * 6 + arm * (2 * Math.PI / arms) + (rng.NextDouble() - 0.5) * 0.5;
                    var x = cx + (float)(Math.Cos(angle) * radius);
                    var y = cy + (float)(Math.Sin(angle) * radius * 0.7);
                    star.Color = HsvToSkColor(t < 0.3 ? hueA : hueB, 0.5f, 1f).WithAlpha((byte)rng.Next(120, 230));
                    canvas.DrawCircle(x, y, (float)rng.NextDouble() * 1.5f + 0.4f, star);
                }
            using (var core = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(cx, cy), 70,
                    [new SKColor(255, 245, 220, 230), new SKColor(255, 200, 140, 0)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(cx, cy, 70, core);
            }
        }

        private static void DrawConstellation(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size / 2f), Size * 0.8f,
                    [new SKColor(18, 20, 45), new SKColor(6, 6, 16)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, sky);
            }

            using (var bg = new SKPaint { IsAntialias = true })
                for (var i = 0; i < 140; i++)
                {
                    bg.Color = SKColors.White.WithAlpha((byte)rng.Next(60, 160));
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 1.2f + 0.3f, bg);
                }

            var n = rng.Next(5, 8);
            var px = new float[n];
            var py = new float[n];
            for (var i = 0; i < n; i++) { px[i] = rng.Next(60, Size - 60); py[i] = rng.Next(60, Size - 60); }

            using (var line = new SKPaint { Color = new SKColor(150, 180, 255, 150), StrokeWidth = 1.5f, IsAntialias = true })
                for (var i = 0; i < n - 1; i++)
                    canvas.DrawLine(px[i], py[i], px[i + 1], py[i + 1], line);

            using (var star = new SKPaint { IsAntialias = true })
                for (var i = 0; i < n; i++)
                {
                    using var glow = new SKPaint
                    {
                        IsAntialias = true,
                        Shader = SKShader.CreateRadialGradient(
                            new SKPoint(px[i], py[i]), 12,
                            [new SKColor(200, 220, 255, 180), new SKColor(0, 0, 0, 0)], null, SKShaderTileMode.Clamp)
                    };
                    canvas.DrawCircle(px[i], py[i], 12, glow);
                    star.Color = SKColors.White;
                    canvas.DrawCircle(px[i], py[i], 3, star);
                }
        }

        private static void DrawViolin(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var wood = HsvToSkColor(25, 0.7f, 0.55f);
            var woodDark = HsvToSkColor(20, 0.8f, 0.35f);
            var cx = Size * 0.5f;
            var cy = Size * 0.56f;

            canvas.Save();
            canvas.RotateDegrees(18, cx, cy);
            using (var body = new SKPaint { IsAntialias = true, Color = wood })
            {
                canvas.DrawOval(new SKRect(cx - 46, cy - 6, cx + 46, cy + 86), body);
                canvas.DrawOval(new SKRect(cx - 38, cy - 78, cx + 38, cy + 10), body);
                canvas.DrawRect(cx - 26, cy - 20, 52, 40, body);
            }
            using (var f = new SKPaint { Color = new SKColor(20, 12, 8), StrokeWidth = 3, IsAntialias = true, Style = SKPaintStyle.Stroke })
            {
                canvas.DrawLine(cx - 14, cy - 6, cx - 14, cy + 30, f);
                canvas.DrawLine(cx + 14, cy - 6, cx + 14, cy + 30, f);
            }
            using (var neck = new SKPaint { IsAntialias = true, Color = woodDark })
                canvas.DrawRect(cx - 7, cy - 170, 14, 100, neck);
            using (var scroll = new SKPaint { IsAntialias = true, Color = woodDark })
                canvas.DrawCircle(cx, cy - 176, 12, scroll);
            using (var str = new SKPaint { Color = new SKColor(230, 230, 230, 170), StrokeWidth = 1, IsAntialias = true })
                for (var s = -1; s <= 2; s++)
                    canvas.DrawLine(cx + s * 3, cy - 168, cx + s * 3, cy + 60, str);
            canvas.Restore();
        }

        private static void DrawTurntable(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var rect = new SKRect(Size * 0.1f, Size * 0.22f, Size * 0.9f, Size * 0.82f);
            using (var deck = new SKPaint { IsAntialias = true, Color = new SKColor(35, 35, 40) })
                canvas.DrawRoundRect(new SKRoundRect(rect, 14, 14), deck);

            var cx = Size * 0.44f;
            var cy = Size * 0.52f;
            var r = 120f;
            using (var platter = new SKPaint { IsAntialias = true, Color = new SKColor(20, 20, 22) })
                canvas.DrawCircle(cx, cy, r, platter);
            using (var groove = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = new SKColor(55, 55, 60), IsAntialias = true })
                for (var rr = 24f; rr < r; rr += 7) canvas.DrawCircle(cx, cy, rr, groove);
            using (var label = new SKPaint { IsAntialias = true, Color = HsvToSkColor(rng.Next(0, 360), 0.8f, 0.9f) })
                canvas.DrawCircle(cx, cy, 30, label);
            using (var hole = new SKPaint { IsAntialias = true, Color = new SKColor(20, 20, 22) })
                canvas.DrawCircle(cx, cy, 4, hole);
            using (var arm = new SKPaint { Color = new SKColor(200, 200, 205), StrokeWidth = 6, IsAntialias = true, StrokeCap = SKStrokeCap.Round })
                canvas.DrawLine(rect.Right - 30, rect.Top + 24, cx + r * 0.7f, cy - r * 0.5f, arm);
            using (var pivot = new SKPaint { IsAntialias = true, Color = new SKColor(150, 150, 155) })
                canvas.DrawCircle(rect.Right - 30, rect.Top + 24, 10, pivot);
        }

        private static void DrawAmplifier(SKCanvas canvas, Random rng)
        {
            StudioBackground(canvas, rng);

            var rect = new SKRect(Size * 0.16f, Size * 0.2f, Size * 0.84f, Size * 0.82f);
            using (var cab = new SKPaint { IsAntialias = true, Color = new SKColor(40, 30, 25) })
                canvas.DrawRoundRect(new SKRoundRect(rect, 10, 10), cab);

            var grille = new SKRect(rect.Left + 14, rect.Top + 50, rect.Right - 14, rect.Bottom - 14);
            using (var cloth = new SKPaint { IsAntialias = true, Color = new SKColor(60, 55, 48) })
                canvas.DrawRoundRect(new SKRoundRect(grille, 6, 6), cloth);
            using (var weave = new SKPaint { Color = new SKColor(30, 28, 24, 120), StrokeWidth = 1 })
            {
                for (var x = grille.Left; x < grille.Right; x += 6) canvas.DrawLine(x, grille.Top, x, grille.Bottom, weave);
                for (var y = grille.Top; y < grille.Bottom; y += 6) canvas.DrawLine(grille.Left, y, grille.Right, y, weave);
            }
            foreach (var pos in new[] { 0.32f, 0.68f })
            {
                var sx = rect.Left + (rect.Right - rect.Left) * pos;
                var sy = grille.MidY;
                using var cone = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(sx, sy), 52, [new SKColor(50, 45, 40), new SKColor(20, 18, 16)], null, SKShaderTileMode.Clamp)
                };
                canvas.DrawCircle(sx, sy, 50, cone);
                using var cap = new SKPaint { IsAntialias = true, Color = new SKColor(25, 22, 20) };
                canvas.DrawCircle(sx, sy, 14, cap);
            }
            using (var knob = new SKPaint { IsAntialias = true, Color = new SKColor(200, 200, 205) })
                for (var i = 0; i < 5; i++)
                    canvas.DrawCircle(rect.Left + 30 + i * 36, rect.Top + 26, 9, knob);
        }

        private static void DrawFerrisWheel(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: true);
            DrawCelestialBody(canvas, rng);

            var cx = Size * 0.5f;
            var cy = Size * 0.46f;
            var r = 150f;
            const int spokes = 12;
            using (var leg = new SKPaint { Color = new SKColor(200, 200, 215), StrokeWidth = 6, IsAntialias = true })
            {
                canvas.DrawLine(cx, cy, cx - 70, Size, leg);
                canvas.DrawLine(cx, cy, cx + 70, Size, leg);
            }
            using (var rim = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 4, Color = new SKColor(230, 230, 240) })
                canvas.DrawCircle(cx, cy, r, rim);
            using (var spoke = new SKPaint { Color = new SKColor(200, 200, 215, 200), StrokeWidth = 2, IsAntialias = true })
            using (var cab = new SKPaint { IsAntialias = true })
                for (var i = 0; i < spokes; i++)
                {
                    var a = i * 2 * Math.PI / spokes;
                    var ex = cx + (float)(Math.Cos(a) * r);
                    var ey = cy + (float)(Math.Sin(a) * r);
                    canvas.DrawLine(cx, cy, ex, ey, spoke);
                    cab.Color = HsvToSkColor(i * 30 % 360, 0.7f, 0.9f);
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(ex - 9, ey - 7, ex + 9, ey + 10), 3, 3), cab);
                }
            using (var hub = new SKPaint { IsAntialias = true, Color = new SKColor(180, 180, 190) })
                canvas.DrawCircle(cx, cy, 10, hub);
        }

        private static void DrawWindmill(SKCanvas canvas, Random rng)
        {
            VerticalSky(canvas, rng, dusk: false);
            DrawCelestialBody(canvas, rng);

            using (var field = new SKPaint { IsAntialias = true, Color = HsvToSkColor(90, 0.5f, 0.55f) })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(0, Size * 0.72f);
                for (var x = 0; x <= Size; x += 12) p.LineTo(x, Size * 0.72f + (float)(Math.Sin(x * 0.02) * 10));
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, field);
            }

            var bx = Size * 0.5f;
            var by = Size * 0.72f;
            using (var tower = new SKPaint { IsAntialias = true, Color = new SKColor(180, 170, 150) })
            {
                using var p = new SKPath();
                p.MoveTo(bx - 28, by);
                p.LineTo(bx - 16, Size * 0.34f);
                p.LineTo(bx + 16, Size * 0.34f);
                p.LineTo(bx + 28, by);
                p.Close();
                canvas.DrawPath(p, tower);
            }
            using (var cap = new SKPaint { IsAntialias = true, Color = new SKColor(120, 60, 50) })
                canvas.DrawCircle(bx, Size * 0.34f, 20, cap);

            var sx = bx;
            var sy = Size * 0.34f;
            using (var sail = new SKPaint { IsAntialias = true, Color = new SKColor(90, 70, 55) })
            using (var blade = new SKPaint { IsAntialias = true, Color = new SKColor(220, 210, 190) })
                for (var i = 0; i < 4; i++)
                {
                    canvas.Save();
                    canvas.RotateDegrees(i * 90 + 20, sx, sy);
                    canvas.DrawRect(sx - 5, sy - 90, 10, 90, sail);
                    canvas.DrawRect(sx - 22, sy - 90, 17, 80, blade);
                    canvas.Restore();
                }
            using (var hub = new SKPaint { IsAntialias = true, Color = new SKColor(60, 45, 38) })
                canvas.DrawCircle(sx, sy, 8, hub);
        }

        private static void DrawKoiPond(SKCanvas canvas, Random rng)
        {
            using (var water = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size / 2f), Size * 0.8f,
                    [HsvToSkColor(190, 0.5f, 0.5f), HsvToSkColor(205, 0.7f, 0.28f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, water);
            }
            using (var ripple = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f, Color = new SKColor(255, 255, 255, 40), IsAntialias = true })
                for (var i = 0; i < 5; i++)
                {
                    var x = rng.Next(0, Size);
                    var y = rng.Next(0, Size);
                    for (var rr = 8; rr < 40; rr += 10) canvas.DrawCircle(x, y, rr, ripple);
                }

            void Koi(float x, float y, float rot, SKColor c)
            {
                canvas.Save();
                canvas.RotateDegrees(rot, x, y);
                using var body = new SKPaint { IsAntialias = true, Color = c };
                canvas.DrawOval(new SKRect(x - 16, y - 9, x + 16, y + 9), body);
                using var tail = new SKPath();
                tail.MoveTo(x + 14, y);
                tail.LineTo(x + 30, y - 10);
                tail.LineTo(x + 30, y + 10);
                tail.Close();
                canvas.DrawPath(tail, body);
                using var spot = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 200) };
                canvas.DrawCircle(x - 4, y, 4, spot);
                canvas.Restore();
            }
            Koi(Size * 0.35f, Size * 0.4f, 20, new SKColor(240, 120, 40));
            Koi(Size * 0.62f, Size * 0.58f, -150, new SKColor(245, 245, 245));
            Koi(Size * 0.5f, Size * 0.72f, 80, new SKColor(240, 80, 60));

            using (var pad = new SKPaint { IsAntialias = true, Color = HsvToSkColor(120, 0.5f, 0.4f) })
                for (var i = 0; i < 4; i++)
                    canvas.DrawCircle(rng.Next(30, Size - 30), rng.Next(30, Size - 30), 16, pad);
            using (var lotus = new SKPaint { IsAntialias = true, Color = HsvToSkColor(335, 0.35f, 1f).WithAlpha(230) })
            {
                var lx = Size * 0.24f;
                var ly = Size * 0.68f;
                for (var p = 0; p < 6; p++)
                {
                    canvas.Save();
                    canvas.RotateDegrees(p * 60, lx, ly);
                    canvas.DrawOval(new SKRect(lx - 5, ly - 14, lx + 5, ly + 2), lotus);
                    canvas.Restore();
                }
                using var c = new SKPaint { IsAntialias = true, Color = HsvToSkColor(50, 0.6f, 1f) };
                canvas.DrawCircle(lx, ly, 4, c);
            }
        }

        private static void DrawLavenderField(SKCanvas canvas, Random rng)
        {
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size * 0.5f),
                    [HsvToSkColor(35, 0.4f, 0.95f), HsvToSkColor(280, 0.25f, 0.8f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size * 0.5f, sky);
            }
            DrawCelestialBody(canvas, rng);
            using (var ground = new SKPaint { IsAntialias = true, Color = HsvToSkColor(110, 0.35f, 0.4f) })
                canvas.DrawRect(0, Size * 0.5f, Size, Size * 0.5f, ground);

            var vpx = Size * 0.5f;
            var horizon = Size * 0.5f;
            using (var row = new SKPaint { Color = HsvToSkColor(275, 0.55f, 0.6f), StrokeWidth = 3, IsAntialias = true })
                for (var i = -7; i <= 7; i++)
                    canvas.DrawLine(vpx + i * 8, horizon, vpx + i * 70, Size, row);

            for (var i = 0; i < 160; i++)
            {
                var y = rng.Next((int)(horizon + 6), Size);
                var depth = (y - horizon) / (Size - horizon);
                var x = rng.Next(0, Size);
                using var tuft = new SKPaint { IsAntialias = true, Color = HsvToSkColor(275 + rng.Next(-10, 15), 0.6f, 0.4f + depth * 0.5f) };
                canvas.DrawCircle(x, y, 2 + depth * 5, tuft);
            }
        }

        private static void DrawPalmBeach(SKCanvas canvas, Random rng)
        {
            var hue = rng.Next(20, 45);
            using (var sky = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(hue, 0.6f, 0.98f), HsvToSkColor((hue + 25) % 360, 0.7f, 0.75f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size * 0.6f, sky);
            }
            using (var sun = new SKPaint { IsAntialias = true, Color = new SKColor(255, 240, 200, 235) })
                canvas.DrawCircle(Size * 0.7f, Size * 0.28f, 46, sun);
            using (var sea = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, Size * 0.55f), new SKPoint(0, Size * 0.75f),
                    [HsvToSkColor(190, 0.6f, 0.6f), HsvToSkColor(200, 0.7f, 0.4f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, Size * 0.55f, Size, Size * 0.2f, sea);
            }
            using (var sand = new SKPaint { IsAntialias = true, Color = HsvToSkColor(45, 0.4f, 0.85f) })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(0, Size * 0.74f);
                for (var x = 0; x <= Size; x += 14) p.LineTo(x, Size * 0.74f + (float)(Math.Sin(x * 0.02) * 6));
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, sand);
            }

            var bx = Size * 0.24f;
            var by = Size * 0.78f;
            using (var trunk = new SKPaint { Color = new SKColor(110, 75, 45), StrokeWidth = 11, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round })
            {
                using var p = new SKPath();
                p.MoveTo(bx, by);
                p.QuadTo(bx - 20, Size * 0.5f, bx + 6, Size * 0.34f);
                canvas.DrawPath(p, trunk);
            }
            using (var frond = new SKPaint { Color = HsvToSkColor(110, 0.6f, 0.5f), StrokeWidth = 6, IsAntialias = true, StrokeCap = SKStrokeCap.Round })
            {
                var tx = bx + 6;
                var ty = Size * 0.34f;
                for (var i = 0; i < 6; i++)
                {
                    var a = Math.PI + i * Math.PI / 5;
                    canvas.DrawLine(tx, ty, tx + (float)(Math.Cos(a) * 60), ty + (float)(Math.Sin(a) * 40) - 10, frond);
                }
            }
        }

        private static void DrawJellyfish(SKCanvas canvas, Random rng)
        {
            using (var water = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(220, 0.7f, 0.45f), new SKColor(4, 8, 30)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, water);
            }
            using (var bub = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = new SKColor(200, 230, 255, 90), IsAntialias = true })
                for (var i = 0; i < 30; i++)
                    canvas.DrawCircle(rng.Next(0, Size), rng.Next(0, Size), (float)rng.NextDouble() * 5 + 1, bub);

            void Jelly(float x, float y, float s, int hue)
            {
                using var glow = new SKPaint
                {
                    IsAntialias = true,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(x, y), s * 1.6f,
                        [HsvToSkColor(hue, 0.6f, 1f).WithAlpha(70), new SKColor(0, 0, 0, 0)], null, SKShaderTileMode.Clamp)
                };
                canvas.DrawCircle(x, y, s * 1.6f, glow);
                using var bell = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hue, 0.55f, 0.95f).WithAlpha(210) };
                using var p = new SKPath();
                p.AddArc(new SKRect(x - s, y - s, x + s, y + s), 180, 180);
                p.LineTo(x + s * 0.6f, y + s * 0.5f);
                p.LineTo(x - s * 0.6f, y + s * 0.5f);
                p.Close();
                canvas.DrawPath(p, bell);
                using var ten = new SKPaint { Color = HsvToSkColor(hue, 0.5f, 1f).WithAlpha(160), StrokeWidth = 2, IsAntialias = true, Style = SKPaintStyle.Stroke };
                for (var t = -3; t <= 3; t++)
                {
                    var tx = x + t * s * 0.18f;
                    using var tp = new SKPath();
                    tp.MoveTo(tx, y + s * 0.4f);
                    tp.QuadTo(tx + (float)Math.Sin(t) * 6, y + s * 1.1f, tx, y + s * 1.7f);
                    canvas.DrawPath(tp, ten);
                }
            }
            Jelly(Size * 0.35f, Size * 0.4f, 38, 300);
            Jelly(Size * 0.66f, Size * 0.6f, 28, 190);
            Jelly(Size * 0.5f, Size * 0.28f, 20, 50);
        }

        private static void DrawCoralReef(SKCanvas canvas, Random rng)
        {
            using (var water = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size),
                    [HsvToSkColor(190, 0.6f, 0.7f), HsvToSkColor(210, 0.7f, 0.35f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, water);
            }
            using (var ray = new SKPaint { Color = new SKColor(255, 255, 255, 30), IsAntialias = true })
                for (var i = 0; i < 5; i++)
                {
                    var x = rng.Next(0, Size);
                    using var p = new SKPath();
                    p.MoveTo(x, 0);
                    p.LineTo(x + 40, 0);
                    p.LineTo(x + 120, Size);
                    p.LineTo(x + 60, Size);
                    p.Close();
                    canvas.DrawPath(p, ray);
                }
            using (var sand = new SKPaint { IsAntialias = true, Color = HsvToSkColor(45, 0.3f, 0.7f) })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size);
                p.LineTo(0, Size * 0.82f);
                for (var x = 0; x <= Size; x += 14) p.LineTo(x, Size * 0.82f + (float)(Math.Sin(x * 0.03) * 8));
                p.LineTo(Size, Size);
                p.Close();
                canvas.DrawPath(p, sand);
            }
            for (var i = 0; i < 6; i++)
            {
                var x = rng.Next(20, Size - 20);
                var baseY = Size * 0.85f;
                var hue = rng.Next(0, 60) + (rng.NextDouble() > 0.5 ? 280 : 0);
                using var coral = new SKPaint { Color = HsvToSkColor(hue, 0.6f, 0.8f), StrokeWidth = rng.Next(5, 9), IsAntialias = true, StrokeCap = SKStrokeCap.Round };
                for (var b = 0; b < 3; b++)
                {
                    var a = -Math.PI / 2 + (b - 1) * 0.5;
                    canvas.DrawLine(x, baseY, x + (float)(Math.Cos(a) * 40), baseY + (float)(Math.Sin(a) * 50), coral);
                }
            }

            void Fish(float x, float y, SKColor c, int dir)
            {
                using var b = new SKPaint { IsAntialias = true, Color = c };
                canvas.DrawOval(new SKRect(x - 12, y - 7, x + 12, y + 7), b);
                using var t = new SKPath();
                t.MoveTo(x - dir * 10, y);
                t.LineTo(x - dir * 22, y - 8);
                t.LineTo(x - dir * 22, y + 8);
                t.Close();
                canvas.DrawPath(t, b);
                using var e = new SKPaint { IsAntialias = true, Color = SKColors.White };
                canvas.DrawCircle(x + dir * 6, y - 2, 2, e);
            }
            Fish(Size * 0.3f, Size * 0.4f, HsvToSkColor(40, 0.8f, 0.95f), 1);
            Fish(Size * 0.65f, Size * 0.55f, HsvToSkColor(10, 0.7f, 0.9f), -1);
            Fish(Size * 0.5f, Size * 0.3f, HsvToSkColor(300, 0.6f, 0.9f), 1);
        }

        private static void DrawAutumnLeaves(SKCanvas canvas, Random rng)
        {
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(Size, Size),
                    [HsvToSkColor(35, 0.6f, 0.85f), HsvToSkColor(18, 0.7f, 0.55f)], null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }
            using (var branch = new SKPaint { Color = new SKColor(70, 45, 30), StrokeWidth = 7, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round })
            {
                using var p = new SKPath();
                p.MoveTo(0, Size * 0.12f);
                p.QuadTo(Size * 0.4f, Size * 0.05f, Size, Size * 0.2f);
                canvas.DrawPath(p, branch);
            }
            var hues = new[] { 15, 30, 45, 10, 35 };
            for (var i = 0; i < 22; i++)
            {
                var x = rng.Next(10, Size - 10);
                var y = rng.Next(10, Size - 10);
                var s = rng.Next(8, 18);
                canvas.Save();
                canvas.RotateDegrees(rng.Next(0, 360), x, y);
                using var leaf = new SKPaint { IsAntialias = true, Color = HsvToSkColor(hues[rng.Next(hues.Length)], 0.8f, 0.85f) };
                using var p = new SKPath();
                p.MoveTo(x, y - s);
                p.QuadTo(x + s, y, x, y + s);
                p.QuadTo(x - s, y, x, y - s);
                p.Close();
                canvas.DrawPath(p, leaf);
                using var vein = new SKPaint { Color = new SKColor(80, 45, 25, 150), StrokeWidth = 1, IsAntialias = true };
                canvas.DrawLine(x, y - s, x, y + s, vein);
                canvas.Restore();
            }
        }

        private static void DrawMandala(SKCanvas canvas, Random rng)
        {
            var palette = GeneratePalette(rng);
            using (var bg = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size / 2f), Size * 0.7f, palette, null, SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawRect(0, 0, Size, Size, bg);
            }

            var cx = Size / 2f;
            var cy = Size / 2f;
            var baseHue = rng.Next(0, 360);
            for (var ring = 5; ring >= 1; ring--)
            {
                var r = ring * 36f;
                var petals = ring * 6;
                var hue = (baseHue + ring * 30) % 360;
                using var petal = new SKPaint
                {
                    IsAntialias = true,
                    Color = HsvToSkColor(hue, 0.6f, 0.95f).WithAlpha(200),
                    Style = ring % 2 == 0 ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
                    StrokeWidth = 2
                };
                for (var p = 0; p < petals; p++)
                {
                    var a = p * 2 * Math.PI / petals;
                    canvas.DrawCircle(cx + (float)(Math.Cos(a) * r), cy + (float)(Math.Sin(a) * r), ring * 3f, petal);
                }
            }
            using (var center = new SKPaint { IsAntialias = true, Color = HsvToSkColor(baseHue, 0.7f, 1f) })
                canvas.DrawCircle(cx, cy, 14, center);
        }

        private static void DrawStainedGlass(SKCanvas canvas, Random rng)
        {
            using (var bg = new SKPaint { Color = new SKColor(15, 12, 18) })
                canvas.DrawRect(0, 0, Size, Size, bg);

            const int cols = 6, rows = 6;
            var cw = (float)Size / cols;
            var ch = (float)Size / rows;
            var baseHue = rng.Next(0, 360);
            for (var r = 0; r < rows; r++)
                for (var c = 0; c < cols; c++)
                {
                    var hue = (baseHue + (r + c) * 22 + rng.Next(-15, 15)) % 360;
                    using var glass = new SKPaint
                    {
                        IsAntialias = true,
                        Shader = SKShader.CreateLinearGradient(
                            new SKPoint(c * cw, r * ch), new SKPoint((c + 1) * cw, (r + 1) * ch),
                            [HsvToSkColor(hue, 0.7f, 0.85f), HsvToSkColor(hue, 0.85f, 0.5f)], null, SKShaderTileMode.Clamp)
                    };
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(c * cw + 3, r * ch + 3, (c + 1) * cw - 3, (r + 1) * ch - 3), 6, 6), glass);
                }

            using (var rose = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 4, Color = new SKColor(10, 8, 12) })
                canvas.DrawCircle(Size / 2f, Size / 2f, Size * 0.28f, rose);
            using (var petal = new SKPaint { IsAntialias = true, Color = HsvToSkColor(baseHue, 0.5f, 1f).WithAlpha(180) })
                for (var p = 0; p < 8; p++)
                {
                    var a = p * 2 * Math.PI / 8;
                    canvas.DrawCircle(Size / 2f + (float)(Math.Cos(a) * Size * 0.14f), Size / 2f + (float)(Math.Sin(a) * Size * 0.14f), 22, petal);
                }
        }

        private static void DrawGeometric(SKCanvas canvas, Random rng)
        {
            var palette = GeneratePalette(rng);
            var gradientType = rng.Next(3);
            using (var paint = new SKPaint { IsAntialias = true })
            {
                paint.Shader = gradientType switch
                {
                    0 => SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(Size, Size), palette, null, SKShaderTileMode.Clamp),
                    1 => SKShader.CreateRadialGradient(new SKPoint(Size / 2f, Size / 2f), Size * 0.75f, palette, null, SKShaderTileMode.Clamp),
                    _ => SKShader.CreateSweepGradient(new SKPoint(Size / 2f, Size / 2f), palette, null),
                };
                canvas.DrawRect(0, 0, Size, Size, paint);
            }

            using var shape = new SKPaint { IsAntialias = true };
            for (var i = 0; i < 7; i++)
            {
                shape.Color = new SKColor(255, 255, 255, (byte)rng.Next(25, 70));
                shape.Style = rng.NextDouble() > 0.5 ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
                shape.StrokeWidth = rng.Next(2, 8);

                var sides = rng.Next(3, 7);
                var cx = rng.Next(40, Size - 40);
                var cy = rng.Next(40, Size - 40);
                var radius = rng.Next(30, 120);
                var angleOffset = rng.NextDouble() * Math.PI;

                using var path = new SKPath();
                for (var j = 0; j < sides; j++)
                {
                    var angle = angleOffset + j * 2 * Math.PI / sides;
                    var px = cx + (float)(radius * Math.Cos(angle));
                    var py = cy + (float)(radius * Math.Sin(angle));
                    if (j == 0) path.MoveTo(px, py); else path.LineTo(px, py);
                }
                path.Close();
                canvas.DrawPath(path, shape);
            }
        }

        private static void VerticalSky(SKCanvas canvas, Random rng, bool dusk, bool light = false)
        {
            var hue = rng.Next(0, 360);
            SKColor[] colors = light
                ? [HsvToSkColor(hue, 0.25f, 0.95f), HsvToSkColor((hue + 30) % 360, 0.35f, 0.8f)]
                : dusk
                    ? [HsvToSkColor(rng.Next(220, 280), 0.6f, 0.35f), HsvToSkColor(rng.Next(280, 330), 0.5f, 0.15f)]
                    : [HsvToSkColor(rng.Next(190, 220), 0.5f, 0.85f), HsvToSkColor(rng.Next(200, 230), 0.45f, 0.6f)];

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(0, Size), colors, null, SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(0, 0, Size, Size, paint);
        }

        private static void DrawCelestialBody(SKCanvas canvas, Random rng)
        {
            var x = rng.Next(60, Size - 60);
            var y = rng.Next(50, 150);
            var r = rng.Next(28, 50);
            using var glow = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(x, y), r * 2.2f,
                    [new SKColor(255, 245, 215, 200), new SKColor(255, 245, 215, 0)],
                    null, SKShaderTileMode.Clamp)
            };
            canvas.DrawCircle(x, y, r * 2.2f, glow);
            using var disc = new SKPaint { IsAntialias = true, Color = new SKColor(255, 248, 230, 235) };
            canvas.DrawCircle(x, y, r, disc);
        }

        private static void DrawVignette(SKCanvas canvas)
        {
            var colors = new[] { new SKColor(0, 0, 0, 0), new SKColor(0, 0, 0, 120) };
            using var paint = new SKPaint
            {
                IsAntialias = true,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(Size / 2f, Size / 2f), Size * 0.78f, colors, null, SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(0, 0, Size, Size, paint);
        }

        private static SKColor[] GeneratePalette(Random rng)
        {
            var hue1 = rng.Next(0, 360);
            var hue2 = (hue1 + rng.Next(60, 180)) % 360;
            var hue3 = (hue1 + rng.Next(180, 300)) % 360;

            return
            [
                HsvToSkColor(hue1, 0.7f + (float)rng.NextDouble() * 0.3f, 0.5f + (float)rng.NextDouble() * 0.3f),
                HsvToSkColor(hue2, 0.7f + (float)rng.NextDouble() * 0.3f, 0.4f + (float)rng.NextDouble() * 0.3f),
                HsvToSkColor(hue3, 0.6f + (float)rng.NextDouble() * 0.4f, 0.3f + (float)rng.NextDouble() * 0.3f)
            ];
        }

        private static long StableHash(string s)
        {
            unchecked
            {
                uint h = 2166136261;
                foreach (var c in s)
                {
                    h ^= c;
                    h *= 16777619;
                }
                return h;
            }
        }

        private static SKColor HsvToSkColor(float h, float s, float v)
        {
            var c = v * s;
            var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
            var m = v - c;

            float r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new SKColor(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }
    }
}
