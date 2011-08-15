using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KinectMeasurmentAccuracyTest
{
    public class Camera
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        Vector3 cameraPosition, lookat;

        public Camera(float aspectRatio)
        {

            cameraPosition = Vector3.Zero;
            lookat = Vector3.Zero;
            view = Matrix.CreateLookAt(cameraPosition, lookat, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
        }

        public void Update()
        {
            Vector3 direction = cameraPosition - lookat;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraPosition.X += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraPosition.X -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraPosition.Z -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraPosition.Z += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                cameraPosition.Y -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                cameraPosition.Y += 5;
            }
            view = Matrix.CreateLookAt(cameraPosition, lookat, Vector3.Up);
            
        }

        internal void LookAt(Vector3 point)
        {
            lookat = point;
        }
    }
}
