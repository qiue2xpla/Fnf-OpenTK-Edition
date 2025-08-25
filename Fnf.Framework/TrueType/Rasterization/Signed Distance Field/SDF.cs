using System.Collections.Generic;
using System.Drawing;
using System;

namespace Fnf.Framework.TrueType.Rasterization
{
    public static class SDF
    {
        public static void ApplyToSection(FastBitmap fastBitmap, int x, int y, int width, int height, float spread)
        {
            Map<bool> ImageFill = new Map<bool>(width, height);

            for (int v = y; v < y + height; v++)
            {
                for (int h = x; h < x + width; h++)
                {
                    ImageFill[h - x, v - y] = fastBitmap.GetPixel(h, v).a == 255;
                }
            }

            // Initialize vectors represented as half values.(half pixels)
            Point defaultValue = new Point(2 * width + 1, 2 * height + 1);
            Map<Point> half_vector = new Map<Point>(width, height, defaultValue);

            sdf_partial(ImageFill, half_vector, false);
            sdf_partial(ImageFill, half_vector, true);

            Map<float> out_ = new Map<float>(width, height);

            for (int i = 0; i < half_vector.size; i++)
            {
                out_[i] = euclidian(half_vector[i].x, half_vector[i].y) / 2;
                if (ImageFill[i]) out_[i] = -out_[i];
            }

            for (int v = 0; v < height; v++)
            {
                for (int h = 0; h < width; h++)
                {
                    fastBitmap.SetPixel(h + x, v + y, getc(out_[h, v]));
                }
            }

            Color getc(float f)
            {
                float alpha;

                if (f < 0)
                {
                    // Inside shape
                    alpha = map(-f, 0.5f, spread, 0.5f, 1);
                }
                else
                {
                    // Outside shape
                    alpha = map(f+1, 0.5f, spread, 0.5f, 0);
                }

                if (alpha < 0) alpha = 0;
                if (alpha > 1) alpha = 1;

                return new Color(255, 255, 255, (byte)(alpha * 255));

                float map(float v, float in_min, float in_max, float out_min, float out_max)
                {
                    return (v - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
                }
            }
        }

        public static Bitmap Generate(Bitmap Image, float spread)
        {
            Map<bool> ImageFill = ToMap(Image);

            // Initialize vectors represented as half values.(half pixels)
            Point defaultValue = new Point(2 * ImageFill.width + 1, 2 * ImageFill.height + 1);
            Map<Point> half_vector = new Map<Point>(ImageFill.width, ImageFill.height, defaultValue);

            sdf_partial(ImageFill, half_vector, false);
            sdf_partial(ImageFill, half_vector, true);

            Map<float> out_ = new Map<float>(ImageFill.width, Image.Height);

            for (int i = 0; i < half_vector.size; i++)
            {
                out_[i] = euclidian(half_vector[i].x, half_vector[i].y) / 2;
                if (ImageFill[i]) out_[i] = -out_[i];
            }

            return ToBitmap(out_, spread);
        }

        private static Map<bool> ToMap(Bitmap bitmap)
        {
            Map<bool> map = new Map<bool>(bitmap.Width, bitmap.Height);

            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    map[x, y] = bitmap.GetPixel(x, y).A == 255;
                }
            }

            return map;
        }

        private static Bitmap ToBitmap(Map<float> image,float spread)
        {
            Bitmap bitmap = new Bitmap(image.width, image.height);

            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    bitmap.SetPixel(x, y, getc(image[x, y]));
                }
            }

            return bitmap;

            System.Drawing.Color getc(float f)
            {
                float alpha;

                if(f < 0)
                {
                    // Inside shape
                    alpha = map(-f, 0, spread, 0.5f, 1);
                }
                else
                {
                    // Outside shape
                    alpha = map(f, 0, spread, 0.5f, 0);
                }

                if (alpha < 0) alpha = 0;
                if (alpha > 1) alpha = 1;

                return System.Drawing.Color.FromArgb((int)(alpha * 255), 255,255,255);

                float map(float x, float in_min, float in_max, float out_min, float out_max)
                {
                    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
                }
            }
        }

        private static void sdf_partial(Map<bool> in_filled, Map<Point> half_vector, bool negate)
        {
            bool valid_pixel(int x, int y) => (x >= 0) && (x < in_filled.width) && (y >= 0) && (y < in_filled.height);
            int coord(int x, int y) => x + in_filled.width * y;
            bool filled(int x, int y) => valid_pixel(x, y) ? in_filled[coord(x, y)] ^ negate : false ^ negate;

            // Allows us to write loops over a neighborhood of a cell.
            void do_neighbors(int x, int y, Action<Point> f)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (valid_pixel(x + dx, y + dy))
                        {
                            f.Invoke(new Point(x + dx, y + dy));
                        }
                    }
                }
            }
            Map<bool> closed = new Map<bool>(in_filled.width, in_filled.height);

            PriorityQueue<QueueElement, float> pq = new PriorityQueue<QueueElement, float>();

            void add_to_queue(int x, int y, int dx, int dy)
            {
                float d = euclidian(dx, dy);
                pq.Enqueue(new QueueElement
                {
                    x = x,
                    y = y,
                    dx = dx,
                    dy = dy,
                    dist = d
                },d);
            };

            // A. Seed phase: Find all filled (black) pixels that border an
            // empty pixel. Add half distances to every surrounding unfilled
            // (white) pixel.
            for (int y = 0; y < in_filled.height; y++)
            {
                for (int x = 0; x < in_filled.width; x++)
                {
                    if (filled(x, y))
                    {
                        do_neighbors(x, y, (Point v) =>
                        {
                            if (!filled(v.x, v.y))
                                add_to_queue(v.x, v.y, v.x - x, v.y - y);
                        });
                    }
                }
            }

            // B. Propagation phase: Add surrounding pixels to queue and
            // discard the ones that are already closed.
            while (pq.Count != 0)
            {
                QueueElement current = pq.Dequeue();

                // If it's already been closed then the shortest vector has
                // already been found.
                if (closed[coord(current.x, current.y)])
                    continue;

                // Close this one and store the half vector.
                closed[coord(current.x, current.y)] = true;
                half_vector[coord(current.x, current.y)] = new Point(current.dx, current.dy);

                // Add all open neighbors to the queue.
                do_neighbors(current.x, current.y, (Point v) => {
                    if (!filled(v.x, v.y) && !closed[coord(v.x, v.y)])
                    {
                        int dx = 2 * (v.x - current.x);
                        int dy = 2 * (v.y - current.y);
                        Point tv = half_vector[coord(current.x, current.y)];
                        (int ddx,int ddy) = (tv.x, tv.y);
                        dx += ddx;
                        dy += ddy;
                        add_to_queue(v.x, v.y, dx, dy);
                    }
                });
            }
        }

        static float euclidian(int dx, int dy)
        {
            return (float)Math.Sqrt((float)dx * dx + dy * dy);
        }
    }

    struct QueueElement
    {
        public int x, y, dx, dy;
        public float dist;
    }
}