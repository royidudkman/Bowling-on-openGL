using System;
using System.Collections.Generic;
using System.Windows.Forms;

//2
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;
        float aimLine;
        public float ballRadius = 0.8f;
        public float ballSpin = 0.0f;
        public float ballX = -9.0f;
        public float ballY = 0.8f;
        public float ballZ = 0.0f;
        public float cameras = 0;
        public float[] light0 = new float[4];
        public float[] light1 = new float[4];
        public float[] firelight= new float[4];
        public float[] pos = new float[4];
        public bool Mode;
        private bool DontDrawBall = false;
        private bool DontDrawWheel = false;
        public bool water = false;
        public float d;
        public bool fire=false;
        private ParticleEmitter particleEmitter;
        float deltaTime;
        GLUquadric obj;

        public cOGL(Control pb)
        {
            p=pb;
            Width = p.Width;
            Height = p.Height; 
            InitializeGL();
            particleEmitter= new ParticleEmitter();
            obj = GLU.gluNewQuadric(); //!!!
            PrepareLists();
            InitializeDeltaTime();
        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); //!!!
            WGL.wglDeleteContext(m_uint_RC);
        }

		uint m_uint_HWND = 0;

        public uint HWND
		{
			get{ return m_uint_HWND; }
		}
		
        uint m_uint_DC   = 0;

        public uint DC
		{
			get{ return m_uint_DC;}
		}
		uint m_uint_RC   = 0;

        public uint RC
		{
			get{ return m_uint_RC; }
		}


       

        
      

        public float[] ScrollValue = new float[10];
        public float zShift = 0.0f;
        public float yShift = 0.0f;
        public float xShift = 0.0f;
        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;
        double[] AccumulatedRotationsTraslations = new double[16];
        private Stopwatch stopwatch = new Stopwatch();
        int sign = 1;
        public void Draw()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

            GL.glLoadIdentity();
            makingMath();

            switch (cameras)
            {
                case 0:
                    GLU.gluLookAt(-13, 5, 0, -3, 3, 0, 0, 3, 0);

                    break;

                case 1:
                    GLU.gluLookAt(15, 7, 0, -3, 0, 0, 0, 3, 0);
                    break;

                case 2:
                    GLU.gluLookAt(2, 2.5f, 0, 10, 2, 0, 0, 1, 0);
                    break;

                case 3:
                    GLU.gluLookAt(8, 2, 5, 8, 0, 0, 0, 3, 0);

                    break;

                case 4:
                    if (!Mode) sign = 1; else sign = -1;
                    GLU.gluLookAt(4, 8, sign*25, 2, 0, 0, 0, 3, 0);

                    break;

                default:
                    GLU.gluLookAt(-13, 5, 0, -3, 0, 0, 0, 3, 0);

                    break;
            }

            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_LIGHT1);

            if (!fire)
            {
                // light0[0] = -14; light0[1] = 5; light0[2] = 0; light0[3] = 1;   // BALL
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light0);

                //light1[0] = 8f; light1[1] = 4; light1[2] = 0; light1[3] = 0;   // PINS 
                GL.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, light1);
            }
            else
            {
                firelight[0] = ballX; firelight[1] = ballY; firelight[2] = ballZ; // FIRE LIGHT
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, firelight);
            }

            //-----------------------------------VISUAL LIGHT SOURCE---------------------------------
            GL.glPushMatrix();
            GL.glColor4f(1, 1, 0, 0.5f);
            GL.glTranslatef(light0[0], light0[1], light0[2]);
            GLU.gluSphere(obj, 0.2f, 20, 20);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glColor4f(1, 1, 0, 0.5f);
            GL.glTranslatef(light1[0], light1[1], light1[2]);
            GLU.gluSphere(obj, 0.2f, 20, 20);
            GL.glPopMatrix();
            //----------------------------------------------------------------------------------------



            //ROAD MODE
            if (Mode)
            {
                GL.glEnable(GL.GL_LIGHTING);

                Mode = !Mode; // for color to stay              
                DrawRoadFigures(Mode);      
                Mode = !Mode;

                float[,] floor = new float[3, 3];
                floor[0, 0] = -1;
                floor[0, 1] = 0.01f;
                floor[0, 2] = -2.5f;

                floor[1, 0] = -7.5f;
                floor[1, 1] = 0.01f;
                floor[1, 2] = -1;

                floor[2, 0] = -7.5f;
                floor[2, 1] = 0.01f;
                floor[2, 2] = 1;

                GL.glDisable(GL.GL_LIGHTING);
                DrawFloor(Mode);

                // BALL SHADOW
                GL.glPushMatrix();
                {
                    if (!fire) pos = light0; else pos = firelight;
                    MakeShadowMatrix(floor);
                    GL.glMultMatrixf(cubeXform);
                    GL.glTranslatef(ballX, ballY, ballZ);
                    DrawWheel(Mode);
                }
                GL.glPopMatrix();
                
                // PINS SHADOW
                GL.glPushMatrix();
                {
                    if (!fire) pos = light1; else pos = firelight;
                    MakeShadowMatrix(floor);
                    GL.glMultMatrixf(cubeXform);
                    DontDrawWheel = true; // use drawfigures for all figures except for the ball
                    DrawRoadFigures(Mode);                   
                    DontDrawWheel = false;
                }
                GL.glPopMatrix();


            }


            DrawTexturedCube();








            //BOWLING MODE
            if (!Mode)
            {
                
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);


                //only floor, draw only to STENCIL buffer
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
                GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
                GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
                GL.glDisable(GL.GL_DEPTH_TEST);
                DrawFloor(Mode);

                // restore regular settings
                GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
                GL.glEnable(GL.GL_DEPTH_TEST);

                // reflection is drawn only where STENCIL buffer value equal to 1
                GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

                GL.glEnable(GL.GL_STENCIL_TEST);

                GL.glEnable(GL.GL_LIGHTING);


                // draw reflected scene
                GL.glPushMatrix();
                {
                    DrawBowlingFigures(-1,Mode); // -1 for reflection
                    

                }
                GL.glPopMatrix();



                // really draw floor 
                GL.glDepthMask((byte)GL.GL_FALSE);

                GL.glDisable(GL.GL_LIGHTING); // for floor texture to look good
                DrawFloor(Mode);
                GL.glEnable(GL.GL_LIGHTING);

                GL.glDepthMask((byte)GL.GL_TRUE);
                // Disable GL.GL_STENCIL_TEST to show All, else it will be cut on GL.GL_STENCIL
                GL.glDisable(GL.GL_STENCIL_TEST);

                GL.glPushMatrix();
                {
                    DrawBowlingFigures(1,Mode); // 1 for real drawing
                   
                }
                GL.glPopMatrix();

                GL.glDisable(GL.GL_LIGHTING);

              
            }
            

            DrawSideLanes();

            GL.glPushMatrix(); // draw aim line
            {
                GL.glTranslatef(0, 0, 0);
                GL.glColor3f(1, 1, 1);
                GL.glLineWidth(5);
                GL.glBegin(GL.GL_LINES);
                {
                    GL.glVertex3f(-9, 0.5f, 0);
                    GL.glVertex3f(3, 0.5f, aimLine);
                }
                GL.glEnd();
            }
            GL.glPopMatrix();

            if(fire)
            {
                deltaTime = CalculateDeltaTime();

                particleEmitter.Update(deltaTime);

                particleEmitter.Draw();
            }



            GL.glFlush();
            
            WGL.wglSwapBuffers(m_uint_DC);

        }

        void DrawBowlingFigures(int reflect,bool mode)
        {
            GL.glPushMatrix(); //draw lifter
            {
                if (reflect == -1) GL.glScalef(1, -1, 1); //swap on Y axis
                drawLifter(mode);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); //draw static cylinders
            {
                if (reflect == -1) GL.glScalef(1, -1, 1); //swap on Y axis
                drawCyl(mode);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); //draw machine
            {

                GL.glTranslatef(8, reflect * 5f, 0);
                if (reflect == -1) GL.glScalef(1, -1, 1); //swap on Y axis
                GL.glRotatef(90, 0, 1, 0);
                GL.glCallList(ROBOT_LIST);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); // draw pins
            {
                GL.glTranslatef(7, reflect * 0.5f, -2);
                if (reflect == -1) GL.glScalef(1, -1, 1); //swap on Y axis
                DrawPins(mode);
            }
            GL.glPopMatrix();

            if (DontDrawBall == false)
            {
                GL.glPushMatrix(); // draw ball
                {
                    GL.glTranslatef(ballX, reflect * ballY, ballZ);
                    if (reflect == -1) GL.glScalef(1, -1, 1); //swap on Y axis
                    DrawBall(mode); 
                    
                }
                GL.glPopMatrix();
            }
            
        }

        void DrawRoadFigures(bool mode)
        {
            GL.glPushMatrix(); //draw lifter
            {
                drawLifter(mode);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); //draw static cylinders
            {
                drawCyl(mode);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); //draw machine
            {
                GL.glTranslatef(8, 5f, 0);
                GL.glRotatef(90, 0, 1, 0);
                GL.glCallList(ROBOT_LIST);
            }
            GL.glPopMatrix();

            GL.glPushMatrix(); // draw pins
            {
                GL.glTranslatef(7,0.5f, -2);
                DrawCones(mode);
            }
            GL.glPopMatrix();

            if (DontDrawWheel == false)
            {
                GL.glPushMatrix(); // draw ball
                {
                    GL.glTranslatef(ballX, ballY, ballZ);
                    DrawWheel(mode);
                }
                GL.glPopMatrix();
            }

        }



        public float lightX = -12f;
        public float lightY = 10f;
        public float lightZ = 0;
        private void DrawFloor(bool mode)
        {

            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            if (!water)
            {
                GL.glBegin(GL.GL_QUADS);
                {
                    if (!mode)
                        GL.glColor4f(1, 1, 1, 0.75f);
                    if (mode)
                        GL.glColor3f(1, 1, 1);

                    if (!mode) GL.glTexCoord2f(0.0f, 0.0f); //wood
                    else GL.glTexCoord2f(0.5f, 0);   //road
                    GL.glVertex3f(-15.0f, 0.0f, -5.0f); // Bottom-Left

                    if (!mode) GL.glTexCoord2f(0.5f, 0.0f); //wood
                    else GL.glTexCoord2f(1.0f, 0.0f); //road
                    GL.glVertex3f(-15.0f, 0.0f, 5.0f);  // Bottom-Right

                    if (!mode) GL.glTexCoord2f(0.5f, 1); //wood
                    else GL.glTexCoord2f(1.0f, 1.0f); //road
                    GL.glVertex3f(15.0f, 0.0f, 5.0f);   // Top-Right

                    if (!mode) GL.glTexCoord2f(0.0f, 1); //wood
                    else GL.glTexCoord2f(0.5f, 1.0f); //road
                    GL.glVertex3f(15.0f, 0.0f, -5.0f);  // Top-Left
                }
                GL.glEnd();
            }
            else
            {
                GL.glEnable(GL.GL_LIGHTING);
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[15]);            
                drawRipple();
                GL.glDisable(GL.GL_LIGHTING);
            }
            GL.glDisable(GL.GL_TEXTURE_2D);
     
        }




        private void DrawBall(bool mode)
        {

            if (!mode)
                GL.glColor3f(1f, 1f, 1f);
            else
                GL.glColor4f(0.2f, 0.2f, 0.2f, 0.85f);

            GL.glRotatef(ballSpin, 0.0f, 0.0f, 1.0f);

            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
            GLU.gluQuadricTexture(obj, 1);
 
            GL.glEnable(GL.GL_TEXTURE_GEN_S);
            GL.glTexGeni(GL.GL_S, GL.GL_TEXTURE_GEN_MODE, (int)GL.GL_OBJECT_LINEAR);
           
            GLU.gluSphere(obj, ballRadius, 20, 20);

            GL.glDisable(GL.GL_TEXTURE_GEN_S);
            GL.glDisable(GL.GL_TEXTURE_2D);

        }
        private void DrawWheel(bool mode)
        {
            if (!mode)
                GL.glColor3f(1f, 1f, 1f);
            else
                GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f);


            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[8]);
            GLU.gluQuadricTexture(obj, 1);

            GL.glEnable(GL.GL_TEXTURE_GEN_S);
            GL.glTexGeni(GL.GL_S, GL.GL_TEXTURE_GEN_MODE, (int)GL.GL_OBJECT_LINEAR);

            GL.glTranslatef(ballX + 9, ballY-0.5f, ballZ - 0.5f);
            GLU.gluCylinder(obj, 1f, 1f, 1f, 30, 30);
            GL.glRotatef(ballSpin, 0.0f, 0.0f, 1.0f);

            GL.glDisable(GL.GL_TEXTURE_GEN_S);
            GL.glDisable(GL.GL_TEXTURE_2D);
            

        }





        private void DrawPin(float x,float y, float z, bool mode)
        {
  
            GL.glEnable(GL.GL_COLOR_MATERIAL);

            GL.glColor4f(0.2f, 0.2f, 0.2f,0.85f); // if Mode

            if (!mode) GL.glColor3f(0.8f, 0.8f,0.8f ); 
            // BOTTOM HALF
            GL.glPushMatrix();//----------------  

            GL.glTranslatef(x, y, z);
            GL.glRotatef(90.0f,1,0,0);
            GLU.gluCylinder(obj, 0.5f,0.3f, 1f, 20, 20);

            GL.glPopMatrix();//-----------------

            //TOP HALF
            GL.glPushMatrix();//-------------------

            GL.glTranslatef(x,y+1.25f, z);
            GL.glRotatef(90.0f, 1, 0, 0);
            GLU.gluCylinder(obj, 0.2f, 0.5f, 1.25f, 20, 20);

            GL.glPopMatrix();//----------------

            //TOP BALL
            GL.glPushMatrix();//----------------

            GL.glTranslatef(x, y+1.5f, z);
            GL.glRotatef(90.0f, 1, 0, 0);
            GLU.gluSphere(obj, 0.35f, 20, 20);

            GL.glPopMatrix();//-----------------

            // RED RING
            GL.glPushMatrix();//----------------

            if (!mode) GL.glColor3f(1, 0, 0);
            GL.glTranslatef(x, y+1, z);
            GL.glRotatef(90.0f, 1, 0, 0);
            GLU.gluCylinder(obj, 0.3f, 0.3f, 0.1f, 20, 20);

            GL.glPopMatrix();//-----------------
            
            
        }


        private void DrawCone(float x, float y, float z, bool mode)
        {

            GL.glEnable(GL.GL_COLOR_MATERIAL);

            GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f); // if Mode

            if (!mode) GL.glColor3f(1f, 0.35f, 0f);
            // BOTTOM SQUARE
            GL.glPushMatrix();
            {

                GL.glBegin(GL.GL_QUADS);
                {
                    GL.glVertex3f(x-1f, y- 0.9f, z- 1f);
                    GL.glVertex3f(x- 1f, y- 0.9f, z+ 1f);
                    GL.glVertex3f(x+ 1f, y- 0.9f, z+ 1f);
                    GL.glVertex3f(x+ 1f, y-0.9f, z- 1f);
                }
                GL.glEnd();
            }
            GL.glPopMatrix();
            // CONE
            GL.glPushMatrix();
            {
                GL.glTranslatef(x, y+1.35f, z);
                GL.glRotatef(90.0f, 1, 0, 0);
                GLU.gluCylinder(obj, 0.2f, 0.6f, 2.35f, 20, 20);
            }
            GL.glPopMatrix();
            //LOWER STRIPE
            GL.glPushMatrix();
            {
                if (!mode) GL.glColor3f(1, 1, 1);
                GL.glTranslatef(x, y + 0.2f, z);
                GL.glRotatef(90.0f, 1, 0, 0);
                GLU.gluCylinder(obj, 0.3f, 0.5f, 0.5f, 20, 20);
            }
            GL.glPopMatrix();
            //UPPER STRIPE
            GL.glPushMatrix();
            {
                if (!mode) GL.glColor3f(1, 1, 1);
                GL.glTranslatef(x, y + 0.4f, z);
                GL.glRotatef(90.0f, 1, 0, 0);
                GLU.gluCylinder(obj, 0.3f, 0.45f, 0.2f, 20, 20);
            }
            GL.glPopMatrix();


        }





        public float pinPositionY = 0.5f;
        private void DrawPins(bool mode)
        {


            float[] pinPositionsZ = { -1f, 1f, 3f, 5f,   // Left right
                                         0f, 2f, 4f,
                                           1f, 3f,
                                             2f };

            float[] pinPositionsX = { 2f, 2f, 2f , 2f  // Up down
                                       ,1f, 1f, 1f,
                                          0f, 0f,
                                           -1f};
           
            for (int i = 0; i < pinPositionsX.Length; i++)
            {           
                DrawPin(pinPositionsX[i],pinPositionY, pinPositionsZ[i],mode); 
            }
          

        }

        private void DrawCones(bool mode)
        {


            float[] pinPositionsZ = { -1f, 1f, 3f, 5f,   // Left right
                                         0f, 2f, 4f,
                                           1f, 3f,
                                             2f };

            float[] pinPositionsX = { 2f, 2f, 2f , 2f  // Up down
                                       ,1f, 1f, 1f,
                                          0f, 0f,
                                           -1f};

            for (int i = 0; i < pinPositionsX.Length; i++)
            {
                DrawCone(pinPositionsX[i], pinPositionY, pinPositionsZ[i], mode);
            }


        }






        public void aimDraw(float z)
        {
            aimLine = z;
            Draw();
            
        }
      




        public void ThrowBall(float z)
        {
            aimLine = z;
            ballX += 1.5f;
           

            float Z = (ballX + 9) / (12 / aimLine);
         

            if (ballX >16) ballX= 16;

            ballZ = Z;
            Draw();
        }





        public void DrawSideLanes()
        {
        
            GL.glBegin(GL.GL_QUADS);
            {
                if(!Mode) GL.glColor3f(0.63f, 0.33f, 0.11f);
                else GL.glColor3f(0.12f, 0.15f, 0.18f);
                if(water) GL.glColor3f(0.15f, 0.65f, 0.82f);
                GL.glVertex3f(-15, 0, 5);
                GL.glVertex3f(-15, 0, 5 + 2 * ballRadius);
                GL.glVertex3f(15, 0, 5 + 2 * ballRadius);
                GL.glVertex3f(15, 0, 5);

                if (!Mode) GL.glColor3f(0.73f, 0.43f, 0.21f);
                else GL.glColor3f(0.22f, 0.25f, 0.28f);
                if (water) GL.glColor3f(0.25f, 0.75f, 0.92f);
                GL.glVertex3f(-15, 0, 5 + 2 * ballRadius);
                GL.glVertex3f(-15, 1, 5 + 2 * ballRadius);
                GL.glVertex3f(15, 1, 5 + 2 * ballRadius);
                GL.glVertex3f(15, 0, 5 + 2 * ballRadius);

                if (!Mode) GL.glColor3f(0.63f, 0.33f, 0.11f);
                else GL.glColor3f(0.12f, 0.15f, 0.18f);
                if (water) GL.glColor3f(0.15f, 0.65f, 0.82f);
                GL.glVertex3f(-15, 0, -5);
                GL.glVertex3f(-15, 0, -5 - 2 * ballRadius);
                GL.glVertex3f(15, 0, -5 - 2 * ballRadius);
                GL.glVertex3f(15, 0, -5);

                if (!Mode) GL.glColor3f(0.73f, 0.43f, 0.21f);
                else GL.glColor3f(0.22f, 0.25f, 0.28f);
                if (water) GL.glColor3f(0.25f, 0.75f, 0.92f);
                GL.glVertex3f(-15, 0, -5 - 2 * ballRadius);
                GL.glVertex3f(-15, 1, -5 - 2 * ballRadius);
                GL.glVertex3f(15, 1, -5 - 2 * ballRadius);
                GL.glVertex3f(15, 0, -5 - 2 * ballRadius);
            }
            GL.glEnd();
            

        }





        public float ARM_angle;
        public float SHOULDER_angle;
        public float ROBOT_angle;
        public float alpha;
        uint ROBOT_LIST, ARM_LIST, SHOULDER_LIST;
        float r;
        void PrepareLists()
        {

            float ARM_length, SHOULDER_length;
            ARM_length = 2;
            ARM_angle = 0;
            SHOULDER_length = 2.5f;
            SHOULDER_angle = 0;
            ROBOT_angle = 0;
            r = 0.2f;
            
            ROBOT_LIST = GL.glGenLists(3);
            ARM_LIST = ROBOT_LIST + 1;
            SHOULDER_LIST = ROBOT_LIST + 2;

            GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f); // if mode

            //----------------------------------------------------------------NEW LIST-----------------------------------------------------------
            GL.glPushMatrix();  // LOWER PART OF THE MACHINE
            {

                GL.glNewList(ARM_LIST, GL.GL_COMPILE);
                GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f);

                GL.glPushMatrix(); // first arm cylinder
                {
                    GL.glTranslated(-4f, -2.5, 0f);
                    GL.glRotatef(-90 - ARM_angle, 1, 0, 0);
                    GLU.gluCylinder(obj, r, r, SHOULDER_length, 20, 20);
                }
                GL.glPopMatrix();


                GL.glPushMatrix(); // second arm cylinder
                {
                    GL.glTranslated(4f, -2.5, 0);
                    GL.glRotatef(-90 - ARM_angle, 1, 0, 0);
                    GLU.gluCylinder(obj, r, r, SHOULDER_length, 20, 20);
                }
                GL.glPopMatrix();

                GL.glPushMatrix();
                {
                    GL.glTranslated(0f, 0, -1);
                    GL.glRotatef(90,1,0,0);

                    GL.glBegin(GL.GL_QUADS);
                    {
                        GL.glVertex3f(-5, 0, 2.5f);
                        GL.glVertex3f(5, 0, 2.5f);
                        GL.glVertex3f(5, 3, 2.5f);
                        GL.glVertex3f(-5, 3, 2.5f);
                    }
                    GL.glEnd();
                }
                GL.glPopMatrix();

                GL.glEndList();
            }
            GL.glPopMatrix();
            //----------------------------------------------------------------END LIST-----------------------------------------------------------

            //----------------------------------------------------------------NEW LIST-----------------------------------------------------------
            GL.glPushMatrix(); // UPPER PART OF THE MACHINE
            {
                GL.glNewList(SHOULDER_LIST, GL.GL_COMPILE);
                GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f); // if mode

                GL.glPushMatrix(); // first shoulder cylinder
                {

                    GL.glTranslated(-4, 0, 0);
                    GLU.gluSphere(obj, r * 1.5, 20, 20);

                    GL.glRotatef(90, 1, 0, 0);
                    GLU.gluCylinder(obj, r, r, SHOULDER_length, 20, 20);

                    GL.glRotatef(-90, 1, 0, 0); // rotate back
                    GL.glTranslated(0, -2.5, 0);// go to bottom of cylinder

                    GLU.gluSphere(obj, r * 1.5, 20, 20);
                }
                GL.glPopMatrix(); 

                GL.glPushMatrix(); // second shoulder cylinder
                {
                    GL.glTranslated(4, 0, 0);

                    GLU.gluSphere(obj, r * 1.5, 20, 20);

                    GL.glRotatef(90, 1, 0, 0);
                    GLU.gluCylinder(obj, r, r, SHOULDER_length, 20, 20);

                    GL.glRotatef(-90, 1, 0, 0); //rotate back
                    GL.glTranslated(0, -2.5, 0);//got to bottom of cylinder

                    GLU.gluSphere(obj, r * 1.5, 20, 20);
                }
                GL.glPopMatrix(); 

                GL.glEndList();
            }
            GL.glPopMatrix();
            //----------------------------------------------------------------END LIST-----------------------------------------------------------
            CreateRobotList();

        }







        public void CreateRobotList()
        {

            GL.glPushMatrix();
            {
                GL.glNewList(ROBOT_LIST, GL.GL_COMPILE);

                GL.glTranslatef(0, 0, SHOULDER_angle);
                GL.glCallList(SHOULDER_LIST);

                
                GL.glTranslatef(0, -2.5f, 0f);
                GL.glRotatef(180-ARM_angle, 1, 0, 0);
                GL.glCallList(ARM_LIST);

                GL.glEndList();
            }
            GL.glPopMatrix();
        }







        private void drawCyl(bool mode)
        {
            GL.glColor4f(0.2f, 0.2f, 0.2f,0.85f); // If mode

            GL.glPushMatrix(); // first static cylinder
            {
                if (!mode) GL.glColor3f(0.4f, 0.4f, 0.4f);
                GL.glRotatef(90,0,1,0);
                GL.glTranslated(-4f, 5, 5.5);
                GLU.gluCylinder(obj, r, r, 7f, 20, 20);
            }
            GL.glPopMatrix(); 
            
            GL.glPushMatrix(); // second static cylinder
            {
                if (!mode) GL.glColor3f(0.4f, 0.4f, 0.4f);
                GL.glRotatef(90, 0, 1, 0);
                GL.glTranslated(4, 5, 5.5);
                GLU.gluCylinder(obj, r, r, 7f, 20, 20);
            }
            GL.glPopMatrix(); 

            
        }







        public float lifterHigh = 3.5f;
        private void drawLifter(bool mode)
        {


            GL.glColor4f(0.1f, 0.1f, 0.1f, 0.75f); // If mode

            if (!mode) GL.glColor3f(0.2f, 0.2f, 0.2f);


            GL.glBegin(GL.GL_QUADS);

            
            GL.glNormal3f(-1,0,1);
            GL.glVertex3f(5, lifterHigh, 0f);
            GL.glVertex3f(10, lifterHigh, 5f);
            GL.glVertex3f(10, lifterHigh + 1.5f, 5f);
            GL.glVertex3f(5, lifterHigh + 1.5f, 0f);

            GL.glNormal3f(-1, 0, 1);
            GL.glVertex3f(5, lifterHigh, 0f);
            GL.glVertex3f(10, lifterHigh, -5f);
            GL.glVertex3f(10, lifterHigh + 1.5f, -5f);
            GL.glVertex3f(5, lifterHigh + 1.5f, 0f);

            GL.glNormal3f(1, 0, 1);
            GL.glVertex3f(10, lifterHigh + 1.5f, 5f);
            GL.glVertex3f(10, lifterHigh, 5f);
            GL.glVertex3f(10, lifterHigh, -5f);
            GL.glVertex3f(10, lifterHigh + 1.5f, -5f);

            GL.glEnd();


        }






        public float waterWave = 0.33f;
        float SurfaceFunction_f_of_xy(float x, float y, float d)
        {

           

            float dx = (float)(x-85 - ballX*5.8);
            float dy = (float)(y - ballZ*10);
            float balldis = (float)Math.Sqrt(dx * dx + dy * dy);


            float r = (float)Math.Max(balldis, 0.001f);
            //float r = (float)Math.Sqrt(x * x + y * y);
            float z = (float)Math.Sin(r - (2 * d)) / r;

            if (z > 0.8f) z = 0.8f;
            return z * waterWave;
          

        }
        void drawRipple()

        {
            int i, j;
            d += 0.1f;
            Vector av, bv, pv, qv, nv;
            float a = 180.0f;
            float b = 60.0f;
            GL.glColor4f(1.0f, 1.0f, 1.0f,0.55f); // try other combinations      
            GL.glBegin(GL.GL_QUADS);
            for (i = 0; i < a; i++)
                for (j = 0; j < b; j++)
                {
                    av.X = -15.0f + i * 30 / a;
                    av.Y = SurfaceFunction_f_of_xy(i , j - 30, d);
                    av.Z = -5.0f + j * 10 / b;

                    bv.X = -15.0f + (i + 1) * 30 / a;
                    bv.Y = SurfaceFunction_f_of_xy(i + 1 , j + 1 - 30, d);
                    bv.Z = -5.0f + (j + 1) * 10 / b;

                    pv = bv - av;  // find vector from the first to the second

                    bv.X = -15.0f + i * 30 / a;
                    bv.Y = SurfaceFunction_f_of_xy(i, j + 1 - 30, d);
                    bv.Z = -5.0f + (j + 1) * 10 / b;

                    qv = bv - av;                 // find vector from the first to the third

                    nv = pv.CrossProduct(qv); // calculate the normal 
                    nv = nv.Normalize(); // normalize it, give the normal a length of 1

                                     
                    //GL.glNormal3d(nv.X, nv.Y, nv.Z);
                    GL.glTexCoord2f(i / a, j / b);
                    GL.glVertex3f(-15.0f + i * 30 / a ,  SurfaceFunction_f_of_xy(i , j - 30, d) ,  -5.0f + j * 10 / b);

                    GL.glTexCoord2f(i / a, (j + 1) / b);
                    GL.glVertex3f(-15.0f + i * 30 / a,  SurfaceFunction_f_of_xy(i , j + 1 - 30, d) , -5f + (j + 1) * 10 / b);


                    GL.glTexCoord2f((i + 1) / a, (j + 1) / b);
                    GL.glVertex3f(-15.0f + (i + 1) * 30 / a, SurfaceFunction_f_of_xy(i , j + 1 - 30, d),  -5f + (j + 1) * 10 / b);


                    GL.glTexCoord2f((i + 1) / a, j / b);
                    GL.glVertex3f(-15.0f + (i + 1) * 30 / a, SurfaceFunction_f_of_xy(i , j - 30, d) , -5.0f + j * 10 / b);
                
                }
            GL.glEnd();


           
        }


        public float fireY = 2;
        public void EmitCircle(int numberOfEmitters, float circleRadius)
        {
            for (int i = 0; i < numberOfEmitters; i++)
            {
                // Calculate the angle to evenly distribute emitters around the circle
                float angle = (float)(i * 2 * Math.PI / numberOfEmitters);

                // Calculate the X and Y positions based on the angle and radius
                float x = ballX + circleRadius * (float)Math.Cos(angle);
                float z = ballZ + circleRadius * (float)Math.Sin(angle);             
                particleEmitter.Emit(new Vector(x, 0, z), 50,fireY);
                
            }
        }
        private void InitializeDeltaTime()
        {
            stopwatch.Start();
        }


        private float CalculateDeltaTime()
        {
            // Stop the stopwatch, get the elapsed time, and restart it
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            if (elapsed.TotalMilliseconds > 1f) stopwatch.Reset();
            stopwatch.Start();

            // Calculate deltaTime as the elapsed milliseconds converted to seconds
            float deltaTime = (float)elapsed.TotalMilliseconds / 1000.0f;

            return deltaTime;
        }





















        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;

        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        float[] cubeXform = new float[16];

        // Creates a shadow projection matrix out of the plane equation
        // coefficients and the position of the light. The return value is stored
        // in cubeXform[,]
        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }
        


        private void makingMath()
        {
            // not trivial
            double[] ModelVievMatrixBeforeSpecificTransforms = new double[16];
            double[] CurrentRotationTraslation = new double[16];

            GLU.gluLookAt(ScrollValue[0], ScrollValue[1], ScrollValue[2],
                       ScrollValue[3], ScrollValue[4], ScrollValue[5],
                       ScrollValue[6], ScrollValue[7], ScrollValue[8]);
            GL.glTranslatef(0.0f, 0.0f, -1.0f);



            //save current ModelView Matrix values
            //in ModelVievMatrixBeforeSpecificTransforms array
            //ModelView Matrix ========>>>>>> ModelVievMatrixBeforeSpecificTransforms
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, ModelVievMatrixBeforeSpecificTransforms);
            //ModelView Matrix was saved, so
            GL.glLoadIdentity(); // make it identity matrix

            //make transformation in accordance to KeyCode
            float delta;
            if (intOptionC != 0)
            {
                delta = 5.0f * Math.Abs(intOptionC) / intOptionC; // signed 5

                switch (Math.Abs(intOptionC))
                {
                    case 1:
                        GL.glRotatef(delta, 1, 0, 0);
                        break;
                    case 2:
                        GL.glRotatef(delta, 0, 1, 0);
                        break;
                    case 3:
                        GL.glRotatef(delta, 0, 0, 1);
                        break;
                    case 4:
                        GL.glTranslatef(delta / 20, 0, 0);
                        break;
                    case 5:
                        GL.glTranslatef(0, delta / 20, 0);
                        break;
                    case 6:
                        GL.glTranslatef(0, 0, delta / 20);
                        break;
                }
            }
            //as result - the ModelView Matrix now is pure representation
            //of KeyCode transform and only it !!!

            //save current ModelView Matrix values
            //in CurrentRotationTraslation array
            //ModelView Matrix =======>>>>>>> CurrentRotationTraslation
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, CurrentRotationTraslation);

            //The GL.glLoadMatrix function replaces the current matrix with
            //the one specified in its argument.
            //The current matrix is the
            //projection matrix, modelview matrix, or texture matrix,
            //determined by the current matrix mode (now is ModelView mode)
            GL.glLoadMatrixd(AccumulatedRotationsTraslations); //Global Matrix

            //The GL.glMultMatrix function multiplies the current matrix by
            //the one specified in its argument.
            //That is, if M is the current matrix and T is the matrix passed to
            //GL.glMultMatrix, then M is replaced with M • T
            GL.glMultMatrixd(CurrentRotationTraslation);

            //save the matrix product in AccumulatedRotationsTraslations
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            //replace ModelViev Matrix with stored ModelVievMatrixBeforeSpecificTransforms
            GL.glLoadMatrixd(ModelVievMatrixBeforeSpecificTransforms);
            //multiply it by KeyCode defined AccumulatedRotationsTraslations matrix
            GL.glMultMatrixd(AccumulatedRotationsTraslations);
        }

        protected virtual void InitializeGL()
		{
			m_uint_HWND = (uint)p.Handle.ToInt32();
			m_uint_DC   = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
			// result in a failure to subsequently create the RC.
			WGL.wglSwapBuffers(m_uint_DC);

			WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
			WGL.ZeroPixelDescriptor(ref pfd);
			pfd.nVersion        = 1; 
			pfd.dwFlags         = (WGL.PFD_DRAW_TO_WINDOW |  WGL.PFD_SUPPORT_OPENGL |  WGL.PFD_DOUBLEBUFFER); 
			pfd.iPixelType      = (byte)(WGL.PFD_TYPE_RGBA);
			pfd.cColorBits      = 32;
			pfd.cDepthBits      = 32;
			pfd.iLayerType      = (byte)(WGL.PFD_MAIN_PLANE);

            pfd.cStencilBits = 32;

            int pixelFormatIndex = 0;
			pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
			if(pixelFormatIndex == 0)
			{
				MessageBox.Show("Unable to retrieve pixel format");
				return;
			}

			if(WGL.SetPixelFormat(m_uint_DC,pixelFormatIndex,ref pfd) == 0)
			{
				MessageBox.Show("Unable to set pixel format");
				return;
			}
			//Create rendering context
			m_uint_RC = WGL.wglCreateContext(m_uint_DC);
			if(m_uint_RC == 0)
			{
				MessageBox.Show("Unable to get rendering context");
				return;
			}
			if(WGL.wglMakeCurrent(m_uint_DC,m_uint_RC) == 0)
			{
				MessageBox.Show("Unable to make rendering context current");
				return;
			}


            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        protected virtual void initRenderingGL()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;
            if (this.Width == 0 || this.Height == 0)
                return;
            GL.glClearColor(0.5f, 0.5f, 0.5f, 0.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);

            GL.glViewport(0, 0, this.Width, this.Height);
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();

            //nice 3D
            GLU.gluPerspective(45.0, 1.0, 0.4, 100.0);


            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

            //save the current MODELVIEW Matrix (now it is Identity)
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            //! TEXTURE 1a 
            GenerateTextures();
            //! TEXTURE 1b 
        }

        //! TEXTURE b
        public uint[] Textures = new uint[22];

        void GenerateTextures()
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glGenTextures(22, Textures);
            string[] imagesName ={ "lane.bmp", "front.bmp","back.bmp",
                                    "left.bmp","right.bmp","top.bmp","bottom.bmp","ball.bmp","wheel.bmp",
                                    "front2.bmp","back2.bmp","left2.bmp","right2.bmp","top2.bmp","bottom2.bmp","water.bmp",
                                   "front3.bmp","back3.bmp","left3.bmp","right3.bmp","top3.bmp","bottom3.bmp"};
            for (int i = 0; i < 22; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }


        void DrawTexturedCube()
        {
            GL.glColor3d(1, 1, 1);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glPushMatrix();
            {
                GL.glScaled(40, 40, -40);

                if (Mode) // Road
                {
                    GL.glRotated(210, 0, 1, 0);
                    GL.glTranslated(0, 0.2f, -0.5);
                }
                if(!Mode) // Bowling
                {
                    GL.glRotated(-98, 0, 1, 0);
                    GL.glTranslated(-0.2f, 0.2f, -0.3f);
                }
                if(water) // Ocean
                {
                    GL.glRotated(90,0,1,0);
                    GL.glTranslated(0, 0, 0);
                }


                // front
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[9]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[16]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glEnd();
                // back
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[10]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[17]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // left
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[11]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[18]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // right
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[12]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[19]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glEnd();
                // top
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[13]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[20]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // bottom
                if (Mode) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]); else GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[14]);
                if (water) GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[21]);
                GL.glBegin(GL.GL_QUADS);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glEnd();
            }
            GL.glPopMatrix();
            GL.glDisable(GL.GL_TEXTURE_2D);
        }
        //! TEXTURE CUBE e

    }




}




