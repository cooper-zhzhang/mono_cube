using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace scene
{
    public class FreeCubeScene : BaseScene
    {

        private cube_obj.Cube _cube;
        
        // 每个对象继承 obj 类，包含位置、旋转、缩放、颜色等属性。
        public FreeCubeScene(GraphicsDeviceManager graphics, Matrix _view, Matrix _projection) : base(graphics, _view, _projection)
        {

            // UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB
            _cube = new cube_obj.Cube(_graphics.GraphicsDevice, "UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB", this);
            _cube.createCubeByStage();

        }

        // TODO:  把逻辑都写在这个里面

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Gray);
            _cube.Draw(gameTime);
            base.Draw(gameTime);
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
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            _view = Matrix.CreateLookAt(new Vector3(6, 4, 6), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                _graphics.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);
        }

    }
}