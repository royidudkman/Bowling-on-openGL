using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OpenGL
{
    internal class Particle
    {

        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public float Lifespan { get; set; }
        public Particle(Vector position, Vector velocity, Color color, float size, float lifespan)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Size = size;
            Lifespan = lifespan;
        }

    }
}
