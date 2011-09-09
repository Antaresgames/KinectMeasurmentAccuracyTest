using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace KinectMeasurmentAccuracyTest
{
    public class Camera2
    {
        Game1 game;

        public Vector3 player_position {get; private set; }
        public Vector3 lookat { get; protected set; }
        float speed = 0.3f;
        protected Quaternion direction = Quaternion.Identity;
        float scale;
        double acceleratingFor=0;
        KeyboardState lastKeyboard;

        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

       
        
        public Camera2(Game1 game, Vector3 p, float s)
        {
            this.game = game;
            player_position = p;

            view = Matrix.CreateLookAt(player_position, lookat, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);
           
            
            scale = s;

           

        }

        public void Update(GameTime gameTime)
        {
           
            KeyboardState keyboard = Keyboard.GetState();
            float yaw=0, pitch=0, roll = 0;

            #region up/down
            if (keyboard.IsKeyDown(Keys.W))
            {
                pitch -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                pitch += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            #endregion

            #region turn
            if(keyboard.IsKeyDown(Keys.A)){
                yaw -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            
            if(keyboard.IsKeyDown(Keys.D)){
                yaw += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (keyboard.IsKeyDown(Keys.Q))
            {
                roll += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (keyboard.IsKeyDown(Keys.E))
            {
                roll -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            
            #endregion

            changeDirection(yaw, pitch, roll);

            #region accelerate/decelerate
            if (keyboard.IsKeyDown(Keys.Space) && acceleratingFor<6){//accelerate is trigger is pressed and spaceship < max speed.
                acceleratingFor += gameTime.ElapsedGameTime.TotalSeconds;
                accelerate(acceleratingFor);
            }else if(acceleratingFor>0) //decelerate if W no longer pressed
            {
                acceleratingFor -= gameTime.ElapsedGameTime.TotalSeconds;
                accelerate(acceleratingFor);
            }
            else if (keyboard.IsKeyDown(Keys.X) && acceleratingFor > -6)
            {//accelerate is trigger is pressed and spaceship < max speed.
                acceleratingFor -= gameTime.ElapsedGameTime.TotalSeconds;
                accelerate(acceleratingFor);
            }
            else if (acceleratingFor < 0) //decelerate if W no longer pressed
            {
                acceleratingFor += gameTime.ElapsedGameTime.TotalSeconds;
                accelerate(acceleratingFor);
            }

           
            #endregion

            
            lastKeyboard = keyboard;
            Vector3 offset = new Vector3(0, 0, 50);
            Vector3 desiredPosition = Vector3.Transform(offset, Matrix.CreateFromQuaternion(direction));
            desiredPosition += player_position;
            lookat = desiredPosition;

            view = Matrix.CreateLookAt(player_position, lookat, Vector3.Up);

        
        }


        internal void LookAt(Vector3 point)
        {
            lookat = point;
        }
        

        public Vector3 getPosition()
        {
            return player_position;
        }

        public Quaternion getDirection()
        {
            return direction;
        }

        public float getScale()
        {
            return scale;
        }

        public Vector3 getVelocity()
        {
            return player_position;
        }

        public void changeDirection(float yawIn, float pitchIn, float rollIn)
        {
           
            float yaw = (yawIn *speed) * MathHelper.ToRadians( -0.25f );

            float pitch = (pitchIn *speed) * MathHelper.ToRadians( -0.25f );

            float roll = (rollIn * speed) * MathHelper.ToRadians(-0.25f);

            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Right, pitch) * Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) * Quaternion.CreateFromAxisAngle(Vector3.Backward, roll);

            direction *= rot;
        }


        public void accelerate(double acceleratingFor)
        {
            Vector3 changePos = Vector3.Transform(new Vector3(0, 0, 1), direction);
            player_position += changePos * (float)acceleratingFor * speed;
            
        }

        

        
    }
}
