using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace scene
{
    public class SloveCubeScene : BaseScene
    {
        private cube_obj.Cube _cube;
        private string _sloveState;
        private List<string> _sloveCMD;
        private int _index = 0;

        public override string Name => "SloveCubeScene";

        // 每个对象继承 obj 类，包含位置、旋转、缩放、颜色等属性。
        public SloveCubeScene() : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up),  Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            _cube = new cube_obj.Cube(Graphics.GraphicsDevice, "URFFUURRLULLFRBBBDDUFDFRUFULLRRDURBLFFBULBRLDDDFLBDBDB", this);
            _cube.createCubeByStage();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Gray);
            _cube.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if(_cube.CanInputCmd() && !keyboardState.IsKeyDown(Keys.N))
            {
                return ;
            }

            string currentCmd = _index < _sloveCMD.Count ? _sloveCMD[_index] : "";
            if (currentCmd != "")
            {
                if (_cube.InputCmd(currentCmd))
                {
                    Console.WriteLine("currentCmd{0}", currentCmd);
                    _index++;
                }
                else
                {
                    Console.WriteLine("currentCmd failed}");
                }
            }

            _cube.Update(gameTime);
            base.Update(gameTime);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Effect.VertexColorEnabled = true;
            Effect.LightingEnabled = false;

            View = Matrix.CreateLookAt(new Vector3(6, 4, 6), Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Graphics.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cube?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void buildCMD()
        {
             _sloveCMD = new List<string>();
            string slove = _cube.SloveCube();
            // 使用空格将slove 字符串分割为多个命令
            _sloveCMD.AddRange(slove.Split(' '));
        }
    }
}