using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OpenGL
{
    internal class ParticleEmitter
    {
        public List<Particle> particles;
        private Random random = new Random();
        
        GLUquadric obj;

        public ParticleEmitter()
        {
            particles = new List<Particle>();
            obj = GLU.gluNewQuadric(); 
        }
        ~ParticleEmitter()
        {
            GLU.gluDeleteQuadric(obj); 
        }

        public void Emit(Vector position, int count,float fireY=2)
        {
            for (int i = 0; i < count; i++)
            {

                Color color = CalculateColor(position);

                Vector velocity = new Vector(
                    (float)(random.NextDouble() * 1 - 0.5), // X (spread horizontally)
                    (float)(random.NextDouble() * fireY + 1),   // Y (upward motion)
                    (float)(random.NextDouble() * 1 - 0.5)  // Z (spread horizontally)
                );

                //Color color = Color.FromArgb(255, random.Next(200,256), random.Next(0, 120), random.Next(0, 51));

                float size = (float)(random.NextDouble() * 0.3 + 0.1); 
                float lifespan = (float)(random.NextDouble() * 1 + 1); 
                
                particles.Add(new Particle(position, velocity, color, size, lifespan));
            }
        }


        public void Update(float deltaTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {

                Particle particle = particles[i];              
                //particle.Color = CalculateColor(particle.Position);
              
                particle.Position += particle.Velocity * deltaTime ;

                particle.Lifespan -= deltaTime;

                if (particle.Lifespan <= 0 )
                {
                    particles.RemoveAt(i);
                }
            }
            
        }
        
        public void Draw()
        {
           // GL.glEnable(GL.GL_BLEND);
           // GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
         
            foreach (var particle in particles)
            {
                GL.glColor4ub(particle.Color.R, particle.Color.G, particle.Color.B, (byte)(particle.Lifespan  * 255));               
                GL.glTranslated(particle.Position.X, particle.Position.Y,particle.Position.Z);
                GLU.gluSphere(obj, 0.18f, 10, 10);
                GL.glTranslated(-particle.Position.X, -particle.Position.Y, -particle.Position.Z);
            }

           // GL.glDisable(GL.GL_BLEND);
        }
        private Color CalculateColor(Vector position)
        {              
            Vector center = CalculateCenter(); 
          
            float distance = (float)Math.Sqrt(Math.Pow(position.X-center.X, 2) + Math.Pow(position.Y-center.Y - 0.4f, 2));

            float whiteThreshold = 0.0f; 
            float yellowThreshold = 0.5f; 
            float orangeThreshold = 1f; 

            Color color=Color.White;

            if (distance < yellowThreshold)
            {
                color = Color.LightYellow;
            }
            else if (distance < orangeThreshold)
            {
                color = Color.Yellow;
            }
            else if (distance > orangeThreshold) 
            {
                color = Color.OrangeRed; // Beyond orange threshold, stay orange
            }

            return color;
        }
        private Vector CalculateCenter()
        {
            if (particles.Count == 0)
            {
                return new Vector(0,0,0); // Return some default value when there are no particles.
            }
            int randomIndex = random.Next(0, particles.Count);
         
            Vector randomPosition = particles[randomIndex].Position;

            return randomPosition;
           

        }
    }
}
