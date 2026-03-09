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

        // 每个对象继承 obj 类，包含位置、旋转、缩放、颜色等属性。
        public SloveCubeScene(GraphicsDeviceManager graphics, Matrix _view, Matrix _projection) : base(graphics, _view, _projection)
        {
            _cube = new cube_obj.Cube(_graphics.GraphicsDevice, "UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB", this);
            _cube.createCubeByStage();

            string slove = _cube.SloveCube();
            slove= "F'"; // TODO: test
            Console.WriteLine("slove {0}", slove);

            buildCMD();
        }

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
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            _view = Matrix.CreateLookAt(new Vector3(6, 4, 6), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                _graphics.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);
        }



        private void buildCMD()
        {
             _sloveCMD = new List<string>();
            string slove = _cube.SloveCube();

            int i = 0;
            while (true)
            {
                int nextI;
                string cmd = NextCMD(slove, i, out nextI);
                if (cmd == "")
                {
                    break;
                }
                _sloveCMD.Add(cmd);
                i = nextI;
            }
        }

        private string NextCMD(string sloveState, int index, out int newIndex)
        {
            string currentCmd = "";
            int i = index;
            for (; i < sloveState.Length - 1;)
            {
                char c = sloveState[i];

                char nextC = i + 1 < sloveState.Length ? sloveState[i + 1] : ' ';
                char nextNC = i + 2 < sloveState.Length ? sloveState[i + 2] : ' ';
                currentCmd = "" + c;
                i++;

                if (nextC == '\'' || nextNC == '2')
                {
                    currentCmd = currentCmd + nextC;
                    i++;
                }

                if (nextNC == '2' && nextC == '\'')
                {
                    currentCmd = currentCmd + nextNC;
                    i++;
                }
                break;
            }
            newIndex = i;

            return currentCmd;
        }

    }


}