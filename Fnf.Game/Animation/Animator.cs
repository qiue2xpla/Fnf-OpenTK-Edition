using System.Collections.Generic;
using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    /// <summary>
    /// Contains a list of animations with alias names to be <br/>
    /// called with when an animation is going to be played
    /// </summary>
    public class Animator : GameObject, IRenderable, IUpdatable
    {
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        public bool animationFinished { get; private set; } = false;
        public Animation currentAnimation { get; private set; }
        public Color color = Color.White;
        public int frameIndex = 0;

        Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        float passedTime = 0;

        public void play(string name)
        {
            currentAnimation = animations[name];
            animationFinished = false;
            frameIndex = 0;
            passedTime = 0;
        }

        public void add(string name, Animation animation)
        {
            animations.Add(name, animation);
        }

        public void remove(string name)
        {
            animations.Remove(name);
        }

        public void clear()
        {
            animations.Clear();
        }

        public void Update()
        {
            if (!isUpdatable) return; 
            if (currentAnimation == null) return;
            if (animationFinished) return;

            float SecondsPerFrame = 1f / currentAnimation.frameRate;

            passedTime += Time.deltaTime;
            while (passedTime > SecondsPerFrame)
            {
                passedTime -= SecondsPerFrame;
                frameIndex++;

                if (frameIndex < currentAnimation.frames.Length) continue;

                if (currentAnimation.looped)
                {
                    frameIndex = 0;
                }
                else
                {
                    animationFinished = true;
                    frameIndex = currentAnimation.frames.Length - 1;
                }
            }
        }
        
        public void Render()
        {
            if (!isRenderable) return;
            if (currentAnimation == null) return;

            OpenGL.Color4(color);
            RenderFrame(
                currentAnimation.frames[frameIndex],
                currentAnimation.texture,
                currentAnimation.offset,
                globalPosition, globalScale, globalRotation);
        }

        public static void RenderFrame(Frame frame, int texture, Vector2 offset, Vector2 pos, Vector2 size, float rot)
        {
            Texture.Use(texture);

            OpenGL.BeginDrawing(DrawMode.Quads);
            for (int i = 0; i < 4; i++)
            {
                Matrix3 mat = Matrix3.Transform(pos, -MathUtility.ToRadian(rot), size) * Matrix3.Translation(offset);

                OpenGL.TextureCoord(frame.coords[i]);
                OpenGL.Pixel2((mat * frame.verts[i].ToHomogeneous()).ToEuclidean());
            }
            OpenGL.EndDrawing();

            Texture.Use(OpenGL.NULL);
        }

        public static (Vector2 min, Vector2 max) GetMinMax(Frame frame, Vector2 offset, Vector2 pos, Vector2 size, float rot)
        {
            Vector2 min = new Vector2(float.MaxValue);
            Vector2 max = new Vector2(float.MinValue);

            for (int i = 0; i < 4; i++)
            {
                Vector2 vert = frame.verts[i];
                vert += offset;
                vert *= size;
                vert = vert.Rotate(rot);
                vert += pos;

                max = Vector2.Max(max, vert);
                min = Vector2.Min(min, vert);
            }

            return (min, max);
        }
    }
}