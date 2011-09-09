using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Research.Kinect.Nui;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;


namespace KinectMeasurmentAccuracyTest
{
    class Kinect
    {
        Runtime kinect;
        Texture2D depthImg, colorImg;
        Texture2D lastDepth, lastColor;
        private GraphicsDevice GraphicsDevice;
        private int[,] captureDistances;
        private int[] objDepths;
        int redLineY = 125;

        public Kinect(GraphicsDevice GraphicsDevice)
        {
            captureDistances = new int[240,320];
            objDepths = new int[320];
            this.GraphicsDevice = GraphicsDevice;
            kinect = new Runtime();
            kinect.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);
            kinect.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            kinect.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(DepthFrameReady);

            kinect.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(VideoFrameReady);
        }

        private void DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage p = e.ImageFrame.Image;
            Color[] DepthColor = new Color[p.Height * p.Width];

            int[] playerIndex = new int[p.Height * p.Width];

            float maxDist = 4000;
            float minDist = 850;
            float distOffset = maxDist - minDist;

            depthImg = new Texture2D(GraphicsDevice, p.Width, p.Height);
            

            int index = 0;
            for (int y = 0; y < p.Height; y++)
            {
                for (int x = 0; x < p.Width; x++, index += 2)
                {
                    

                    int n = (y * p.Width + x) * 2;
                    
                    playerIndex[y * p.Width + x] = (int)(p.Bits[n] & 7); ;
                    
                    int distance = (p.Bits[n + 0] >>3 | p.Bits[n + 1] << 5);
                    
                    byte intensity = (byte)(255 - (255 * Math.Max(distance - minDist, 0) / (distOffset)));

                    if (playerIndex[y * p.Width + x] > 0)//if its a player
                    {
                        DepthColor[y * p.Width + x] = new Color(intensity, intensity, intensity);
                    }
                    else
                    {
                        DepthColor[y * p.Width + x] = new Color(0, 0, 0);//otherwise, 
                    }
                        
                    captureDistances[y, x] = distance;
                }
            }
            depthImg.SetData(DepthColor);
            
        }

        void VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage p = e.ImageFrame.Image;

            Color[] color = new Color[p.Height * p.Width];
            colorImg = new Texture2D(GraphicsDevice, p.Width, p.Height);

            int index = 0;
            for (int y = 0; y < p.Height; y++)
            {
                for (int x = 0; x < p.Width; x++, index += 4)
                {
                    if (y != 250)
                    {
                        color[y * p.Width + x] = new Color(p.Bits[index + 2], p.Bits[index + 1], p.Bits[index + 0]);
                    }
                    else
                    {
                        color[y * p.Width + x] = new Color(255, 0, 0);
                    }
                }
            }

            colorImg.SetData(color);
        }

        internal Texture2D GetImage()
        {
            return colorImg;
        }

        internal Texture2D GetDepth()
        {
            return depthImg;
        }

        internal Texture2D GetLastImage()
        {
            return lastColor;
        }

        internal Texture2D GetLastDepth()
        {
            return lastDepth;
        }

        internal void CameraUp()
        {
            try
            {
                kinect.NuiCamera.ElevationAngle += 4;
            }
            catch (InvalidOperationException ex)
            {
            }
            catch (ArgumentOutOfRangeException outOfRangeException)
            {
            }
        }

        internal void CameraDown()
        {
            try
            {
                kinect.NuiCamera.ElevationAngle -= 4;
            }
            catch (InvalidOperationException ex)
            {
            }
            catch (ArgumentOutOfRangeException outOfRangeException)
            {
            }
        }

        internal void CameraReset()
        {
            kinect.NuiCamera.ElevationAngle = 0;
        }

        internal void LineUp()
        {
            redLineY -= 1;
        }

        internal void LineDown()
        {
            redLineY += 1;
        }

        public void Uninitalize()
        {
            kinect.Uninitialize();
        }

        /*public Vector3[,] Capture()
        {
            lastDepth = depthImg;
            lastColor = colorImg;
            //Vector3[,] redLinePoints = calculatePoints();
            return redLinePoints;
        }*/

        public Texture2D Capture()
        {
            lastDepth = depthImg;
            lastColor = colorImg;
            //Vector3[,] redLinePoints = calculatePoints();
            return depthImg;
        }


        public Vector3[,] calculatePoints()
        {
            Vector3[,] depths = new Vector3[240,320];

            int centralDist = captureDistances[120, 160];

            //get pixel resolution using the formula [resolution = 374/80096x^-0.953]
            double resolution = 374 / (double)(80096 * (Math.Pow(centralDist, -0.953)));
            /*for (int j = 0; j < captureDistances.GetLength(0); j++)
            {
                for (int i = 0; i < captureDistances.GetLength(1); i++)
                {
                    float xComp = (float)(i * resolution);
                    float yComp = (float)captureDistances[j, i];
                    float zComp = (float)(j*resolution);
                    depths[j,i] = new Vector3(xComp, yComp, zComp);
                }
            }*/
            
            return depths;
        }
    }
}
