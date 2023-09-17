using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices; 

namespace myOpenGL
{
    public partial class Form1 : Form
    {
        cOGL cGL;

        public Form1()
        {





            InitializeComponent();
            cGL = new cOGL(panel1);
            Aim.Value = 45;
            Throw.Text = "Throw!";
            Cleanup.Text = "Cleanup";
            cGL.light0[0] = -14; cGL.light0[1] = 5; cGL.light0[2] = 0; cGL.light0[3] = 1;   // BALL
            cGL.light1[0] = 8f; cGL.light1[1] = 4; cGL.light1[2] = 0; cGL.light1[3] = 0;   // PINS 
            cGL.firelight[0] = cGL.ballX; cGL.firelight[1] = cGL.ballY; cGL.firelight[2] = cGL.ballZ; // FIRE LIGHT
            fireY.Maximum = 1000;  fireY.Minimum = 0;
            waterWave.Maximum = 500; waterWave.Minimum = 0;
            fireY.Enabled = false; waterWave.Enabled = false;
            //apply the bars values as cGL.ScrollValue[..] properties 
            //!!!
            hScrollBarScroll(hScrollBar1, null);
            hScrollBarScroll(hScrollBar2, null);
            hScrollBarScroll(hScrollBar3, null);
            hScrollBarScroll(hScrollBar4, null);
            hScrollBarScroll(hScrollBar5, null);
            hScrollBarScroll(hScrollBar6, null);
            hScrollBarScroll(hScrollBar7, null);
            hScrollBarScroll(hScrollBar8, null);
            hScrollBarScroll(hScrollBar9, null);
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            cGL.OnResize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void hScrollBarScroll(object sender, ScrollEventArgs e)
        {
            cGL.intOptionC = 0;
            HScrollBar hb = (HScrollBar)sender;
            int n = int.Parse(hb.Name.Substring(hb.Name.Length - 1));
            cGL.ScrollValue[n - 1] = (hb.Value - 100) / 10.0f;
            if (e != null)
                cGL.Draw();
        }

        public float[] oldPos = new float[7];

        private void numericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nUD = (NumericUpDown)sender;
            int i = int.Parse(nUD.Name.Substring(nUD.Name.Length - 1));
            int pos = (int)nUD.Value;
            switch (i)
            {
                case 1:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xShift += 0.25f;
                        cGL.intOptionC = 4;
                    }
                    else
                    {
                        cGL.xShift -= 0.25f;
                        cGL.intOptionC = -4;
                    }
                    break;
                case 2:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yShift += 0.25f;
                        cGL.intOptionC = 5;
                    }
                    else
                    {
                        cGL.yShift -= 0.25f;
                        cGL.intOptionC = -5;
                    }
                    break;
                case 3:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.zShift += 0.25f;
                        cGL.intOptionC = 6;
                    }
                    else
                    {
                        cGL.zShift -= 0.25f;
                        cGL.intOptionC = -6;
                    }
                    break;
                case 4:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xAngle += 5;
                        cGL.intOptionC = 1;
                    }
                    else
                    {
                        cGL.xAngle -= 5;
                        cGL.intOptionC = -1;
                    }
                    break;
                case 5:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yAngle += 5;
                        cGL.intOptionC = 2;
                    }
                    else
                    {
                        cGL.yAngle -= 5;
                        cGL.intOptionC = -2;
                    }
                    break;
                case 6:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.zAngle += 5;
                        cGL.intOptionC = 3;
                    }
                    else
                    {
                        cGL.zAngle -= 5;
                        cGL.intOptionC = -3;
                    }
                    break;


            }




            cGL.Draw();
            oldPos[i - 1] = pos;
            cGL.intOptionC = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            cGL.Draw();
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) water.Enabled = false;
            else water.Enabled = true;
            
            cGL.Mode = checkBox1.Checked;

        }
        private void Aim_Scroll(object sender, ScrollEventArgs e)
        {
            float z = (Aim.Value - Aim.Minimum) / (float)(Aim.Maximum - Aim.Minimum) * 10 - 4.5f;
            cGL.aimDraw(z);
        }

        private void Throw_Click(object sender, EventArgs e)
        {
            Aim.Enabled = false;
            Cleanup.Enabled = false;
            AnimationTimer.Start();

        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float z = (Aim.Value - Aim.Minimum) / (float)(Aim.Maximum - Aim.Minimum) * 10 - 4.5f;
            cGL.ThrowBall(z);
            int zStop = 5;
            if (checkBox1.Checked) zStop = 2; else zStop = 5;

            if (cGL.ballZ > zStop + cGL.ballRadius)
                cGL.ballZ = zStop + cGL.ballRadius;
            else if (cGL.ballZ < -zStop - cGL.ballRadius)
                cGL.ballZ = -zStop - cGL.ballRadius;
            cGL.ballSpin -= 50;

            if (cGL.ballX == 16)
            {
                Aim.Enabled = true;
                Cleanup.Enabled = true;
                AnimationTimer.Stop();
                cGL.ballX = -9.0f;
                cGL.ballY = 0.8f;
                cGL.ballZ = 0.0f;
            }

        }

        private void cameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = cameras.SelectedItem.ToString();

            switch (selectedItem)
            {
                case "Main":
                    cGL.cameras = 0;
                    break;

                case "Back":
                    cGL.cameras = 1;
                    break;

                case "Pins":
                    cGL.cameras = 2;
                    break;

                case "Side":
                    cGL.cameras = 3;
                    break;

                case "Far Side":
                    cGL.cameras = 4;
                    break;

                default:
                    cGL.cameras = 0;
                    break;
            }
        }

        private void Cleanup_Click(object sender, EventArgs e)
        {
            Cleanup.Enabled = false;
            Throw.Enabled = false;
            CleanupTimer.Start();
        }

        int flag = 0;
        private void CleanupTimer_Tick(object sender, EventArgs e)
        {
            //Go forward and rotate rectangle and lower lifter
            if (flag == 0)
            {
                if (cGL.SHOULDER_angle > -2.05)
                    cGL.SHOULDER_angle -= 0.5f;

                if (cGL.ARM_angle < 90 && cGL.SHOULDER_angle < -2.05)
                    cGL.ARM_angle += 10;

                if (cGL.lifterHigh > 0)
                    cGL.lifterHigh -= 0.5f;

                if (cGL.ARM_angle >= 90 && cGL.SHOULDER_angle <= -2.05)
                    flag = 1;
            }
            //lift pins and lifter
            if (flag == 1)
            {
                if (cGL.pinPositionY < 4)
                    cGL.pinPositionY += 0.5f;

                if (cGL.lifterHigh < 3.5f)
                    cGL.lifterHigh += 0.5f;


                if (cGL.pinPositionY >= 4)
                    flag = 2;
            }

            //go back
            if (flag == 2)
            {
                if (cGL.SHOULDER_angle < 4.5)
                    cGL.SHOULDER_angle += 0.5f;

                if (cGL.SHOULDER_angle >= 4.5)
                    flag = 3;
            }

            //go forward, rotate rectangle and lower pins and lifter

            if (flag == 3)
            {
                if (cGL.SHOULDER_angle > -2.05)
                    cGL.SHOULDER_angle -= 0.5f;

                if (cGL.ARM_angle > 0 && cGL.SHOULDER_angle < -2.05)
                    cGL.ARM_angle -= 10;

                if (cGL.ARM_angle > 0 && cGL.SHOULDER_angle < -2.05)
                {
                    if (cGL.pinPositionY > 0.5)
                        cGL.pinPositionY -= 0.5f;

                    if (cGL.lifterHigh > 0)
                        cGL.lifterHigh -= 0.5f;
                }

                if (cGL.ARM_angle <= 0 && cGL.SHOULDER_angle <= -2.05)
                    flag = 4;
            }

            //lower the lifter
            if (flag == 4)
            {
                if (cGL.lifterHigh < 3.5f)
                    cGL.lifterHigh += 0.5f;

                if (cGL.lifterHigh >= 3.5)
                    flag = 5;
            }

            cGL.CreateRobotList();
            //end
            if (flag == 5)
            {
                flag = 0;
                Cleanup.Enabled = true;
                Throw.Enabled = true;
                CleanupTimer.Stop();
            }

        }

        public float[] oldLightPos = new float[7];

        private void numericUpDownLightChanged(object sender, EventArgs e)
        {
            NumericUpDown nUD = (NumericUpDown)sender;
            int i = int.Parse(nUD.Name.Substring(nUD.Name.Length - 1));
            int pos = (int)nUD.Value;
            switch (i)
            {
                case 1:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light0[0] += 0.25f;
                    }
                    else
                    {
                        cGL.light0[0] -= 0.25f;
                    }
                    break;
                case 2:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light0[1] += 0.25f;
                    }
                    else
                    {
                        cGL.light0[1] -= 0.25f;
                    }
                    break;
                case 3:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light0[2] += 0.25f;
                    }
                    else
                    {
                        cGL.light0[2] -= 0.25f;
                    }
                    break;
                case 4:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light1[0] += 0.25f;
                    }
                    else
                    {
                        cGL.light1[0] -= 0.25f;
                    }
                    break;
                case 5:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light1[1] += 0.25f;
                    }
                    else
                    {
                        cGL.light1[1] -= 0.25f;
                    }
                    break;
                case 6:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.light1[2] += 0.25f;
                    }
                    else
                    {
                        cGL.light1[2] -= 0.25f;
                    }
                    break;


            }

            cGL.Draw();
            oldPos[i - 1] = pos;
            
        }

        private void water_CheckedChanged(object sender, EventArgs e)
        {
            if (water.Checked)
            {
                waterWave.Enabled = true;
                checkBox1.Enabled = false;
            }
            else
            {
                waterWave.Enabled = false;  
                checkBox1.Enabled = true;
            }
                cGL.water = water.Checked;
            cGL.Draw();
        }

        private void fire_CheckedChanged(object sender, EventArgs e)
        {
            if (fire.Checked)
            {
                fireY.Enabled = true;
                cGL.fire = true;
                firetimer.Start();
            }
            else
            {
                fireY.Enabled = false;
                cGL.fire = false;
                firetimer.Stop();
            }
        }
            private void firetimer_Tick(object sender, EventArgs e)
        {
            cGL.EmitCircle(20, 1f);
        }

        
        private void fireY_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.fireY = fireY.Value/100;
        }

        private void waterWave_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.waterWave= waterWave.Value/100;
        }
    }
        
 
}