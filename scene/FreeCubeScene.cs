using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace cube_game_scene
{
    public class FreeCubeScene : BaseScene
    {
        private cube_obj.Cube _cube;
        
        public override string Name => "FreeCubeScene";
        
        // 每个对象继承 obj 类，包含位置、旋转、缩放、颜色等属性。
        public FreeCubeScene() 
        : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up),  Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            _cube = new cube_obj.Cube(Graphics.GraphicsDevice, "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB", this);
            _cube.createCubeByStage();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            cube_game.Core.GraphicsDevice.Clear(Color.Gray);
            // Graphics.GraphicsDevice.Clear(Color.Gray);
            _cube.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            string currentCmd = "";
            if (_cube.CanInputCmd()){
                 var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.U))
                    currentCmd = tool.RotationHelper.CMD_UP;
                else if (keyboardState.IsKeyDown(Keys.D))
                    currentCmd = tool.RotationHelper.CMD_DOWN;
                else if (keyboardState.IsKeyDown(Keys.L))
                    currentCmd = tool.RotationHelper.CMD_LEFT;
                else if (keyboardState.IsKeyDown(Keys.R))
                    currentCmd = tool.RotationHelper.CMD_RIGHT;
                else if (keyboardState.IsKeyDown(Keys.F))
                    currentCmd = tool.RotationHelper.CMD_FRONT;
                else if (keyboardState.IsKeyDown(Keys.B))
                    currentCmd = tool.RotationHelper.CMD_BACK;
            }

            if (currentCmd != ""){
                _cube.InputCmd(currentCmd);
                Console.WriteLine(_cube.CubeState());
            }

            _cube.Update(gameTime);
            base.Update(gameTime);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            // Effect.VertexColorEnabled = true;
            // Effect.LightingEnabled = false;

            // View = Matrix.CreateLookAt(new Vector3(6, 4, 6), Vector3.Zero, Vector3.Up);
            // Projection = Matrix.CreatePerspectiveFieldOfView(
            //     MathHelper.PiOver4,
            //     Graphics.GraphicsDevice.Viewport.AspectRatio,
            //     0.1f,
            //     100f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cube?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}