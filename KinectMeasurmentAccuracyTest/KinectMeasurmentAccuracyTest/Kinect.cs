using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private int[] redLineDist;
        private int[] objDepths;
        int redLineY = 125;

        public Kinect(GraphicsDevice GraphicsDevice)
        {
            redLineDist = new int[320];
            objDepths = new int[320];
            this.GraphicsDevice = GraphicsDevice;
            kinect = new Runtime();
            kinect.Initialize(RuntimeOptions.UseDepth | RuntimeOptions.UseColor);
            kinect.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            kinect.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(DepthFrameReady);

            kinect.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(VideoFrameReady);
        }

        private void DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage p = e.ImageFrame.Image;
            Color[] DepthColor = new Color[p.Height * p.Width];

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
                    int distance = (p.Bits[n + 0] | p.Bits[n + 1] << 8);
                    if (y != redLineY)
                    {
                        byte intensity = (byte)(255 - (255 * Math.Max(distance - minDist, 0) / (distOffset)));
                        DepthColor[y * p.Width + x] = new Color(intensity, intensity, intensity);
                    }
                    else
                    {
                        byte intensity = (byte)255;
                        redLineDist[x] = distance;
                        DepthColor[y * p.Width + x] = new Color(intensity, 0, 0);
                    }
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

        public Vector2[] Capture()
        {
            lastDepth = depthImg;
            lastColor = colorImg;
            Vector2[] redLinePoints = getObjectWidth();
            return redLinePoints;
        }


        public Vector2[] getObjectWidth()
        {
            Vector2[] depths = new Vector2[320];

            int centralDist = redLineDist[redLineDist.Length / 2];

            //get pixel resolution using the formula [resolution = 374/80096x^-0.953]
            double resolution = 374 / (80096 * (Math.Pow(centralDist, -0.953)));

            for(int i=0; i<redLineDist.Length;i++)
            {
                float xComp = (float)(i*resolution);
                float yComp= (float)redLineDist[i];
                depths[i] = new Vector2(xComp, yComp);
            }
            
            return depths;
        }
    }
}
