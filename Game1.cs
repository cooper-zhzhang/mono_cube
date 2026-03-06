using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace cube_game
{
    public class Game1 : Game
    {
        //scene.FreeCubeScene _freeCubeScene;
        private GraphicsDeviceManager _graphics;
        private BasicEffect _effect;
        private Matrix _view;
        private Matrix _projection;
        private List<cube_obj.CubePiece> _cubes; // 存储27个方块
        private float _timer = 0; // 计时器
        private float _rotationDuration = 0.5f; // 旋转持续时间（秒），旋转90度的时间
        private bool _isRotating = false; // 是否正在旋转
        private char _currentFace = ' '; // 当前要旋转的面，' '表示等待输入
        private string _cubeState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"; // 魔方状态字符串

        // 检查立方体是否需要旋转（基于原始逻辑位置，不受几何变换影响）
        private bool ShouldRotateCube(cube_obj.CubePiece cube, char face)
        {
            switch (face)
            {
                case 'u': // 最上层面绕Y轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - 1) < 0.001f;
                case 'd': // 最下层面绕Y轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - (-1)) < 0.001f;
                case 'l': // 最左层面绕X轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.X - (-1)) < 0.001f;
                case 'r': // 最右层面绕X轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.X - 1) < 0.001f;
                case 'f': // 最前层面绕Z轴旋转（逻辑前面对应Z = 1）
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - 1) < 0.001f;
                case 'b': // 最后层面绕Z轴旋转（逻辑后面对应Z = -1）
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - (-1)) < 0.001f;
                default:
                    return false;
            }
        }

        // 更新立方体矩阵，应用旋转并四舍五入位置
        private void UpdateCubeMatrix(cube_obj.CubePiece cube, Matrix rotation)
        {
            Matrix transformedMatrix = cube.OriginalMatrix * rotation;

            // 四舍五入到整数坐标，避免浮点数误差
            Vector3 roundedPosition = new Vector3(
                MathF.Round(transformedMatrix.Translation.X),
                MathF.Round(transformedMatrix.Translation.Y),
                MathF.Round(transformedMatrix.Translation.Z)
            );

            // 归一化四元数，确保旋转部分正确
            Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(transformedMatrix);
            rotationQuaternion.Normalize();

            // 创建新的矩阵，保持旋转但重置位置为整数
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(rotationQuaternion);
            Matrix translationMatrix = Matrix.CreateTranslation(roundedPosition);
            Matrix finalMatrix = rotationMatrix * translationMatrix;

            // 更新矩阵
            cube.OriginalMatrix = finalMatrix;
            cube.World = finalMatrix;
        }

        // 完成旋转，更新所有立方体矩阵并重置状态
        private void CompleteRotation()
        {
            Matrix finalRotation = tool.RotationHelper.CreateRotationMatrix(_currentFace, MathHelper.PiOver2);

            for (int i = 0; i < _cubes.Count; i++)
            {
                var cube = _cubes[i];
                if (ShouldRotateCube(cube, _currentFace))
                {
                    UpdateCubeMatrix(cube, finalRotation);
                }
            }

            _isRotating = false;
            _timer = 0;
            _currentFace = ' '; // 重置为等待输入状态
        }

        public Game1()
        {
            
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            _effect = new BasicEffect(GraphicsDevice);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // 设置剔除模式：剔除顺时针面，保留逆时针面作为正面（右手坐标系习惯）
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            base.Initialize();
        }

        protected void createCubeByStage()
        {
            _cubes = new List<cube_obj.CubePiece>();
            // 生成27个方块，位置从 -1 到 1，步长为1，形成3x3x3大立方体
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        // 为每个方块生成6个面的颜色，使用魔方状态字符串
                        Color[] faceColors = new Color[6];
                        
                        // 面索引：0=前(Z正), 1=后(Z负), 2=上(Y正), 3=下(Y负), 4=右(X正), 5=左(X负)
                        // 上面 (U): y=1，状态字符串位置0-8
                        if (y == 1)
                        {
                            int row = 1 - z; // z=-1→row=2, z=0→row=1, z=1→row=0
                            int col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                            int index = row * 3 + col;
                            faceColors[2] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[2] = Color.Black;
                        }
                        
                        // 右面 (R): x=1，状态字符串位置9-17
                        if (x == 1)
                        {
                            int row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                            int col = z + 1; // z=-1→col=0, z=0→col=1, z=1→col=2
                            int index = 9 + row * 3 + col;
                            faceColors[4] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[4] = Color.Black;
                        }
                        
                        // 前面 (F): 原为z==-1，现对应几何体前面（Z正），所以颜色应来自状态字符串前面部分
                        // 但逻辑前面（视觉上我们希望看到的面）对应原始状态字符串的前面（索引18-26）
                        // 而几何体前面是Z正，所以当z==1时才是几何体前面，应赋前面颜色
                        if (z == 1) // 几何体前面
                        {
                            int row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                            int col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                            int index = 18 + row * 3 + col; // 前面状态字符串
                            faceColors[0] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[0] = Color.Black;
                        }
                        
                        // 下面 (D): y=-1，状态字符串位置27-35
                        if (y == -1)
                        {
                            int row = z + 1; // z=-1→row=0, z=0→row=1, z=1→row=2
                            int col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                            int index = 27 + row * 3 + col;
                            faceColors[3] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[3] = Color.Black;
                        }
                        
                        // 左面 (L): x=-1，状态字符串位置36-44
                        if (x == -1)
                        {
                            int row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                            int col = 1 - z; // z=-1→col=2, z=0→col=1, z=1→col=0 (镜像)
                            int index = 36 + row * 3 + col;
                            faceColors[5] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[5] = Color.Black;
                        }
                        
                        // 后面 (B): 原为z==1，现对应几何体后面（Z负），所以颜色应来自状态字符串后面部分
                        if (z == -1) // 几何体后面
                        {
                            int row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                            int col = 1 - x; // x=-1→col=2, x=0→col=1, x=1→col=0 (镜像)
                            int index = 45 + row * 3 + col; // 后面状态字符串
                            faceColors[1] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[1] = Color.Black;
                        }

                        _cubes.Add(new cube_obj.CubePiece(GraphicsDevice, pos, faceColors));
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            _view = Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);

            createCubeByStage();
            // _freeCubeScene = new scene.FreeCubeScene(_graphics, _view, _projection);
            // _freeCubeScene .Initialize();
            // _freeCubeScene .LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            //_freeCubeScene.Update(gameTime);

            // 退出条件（按ESC或手柄返回键）
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // 计时器累加
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 检测键盘输入（只在等待输入状态时检测）
            if (!_isRotating && _currentFace == ' ')
            {
                var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.U))
                    _currentFace = 'u';
                else if (keyboardState.IsKeyDown(Keys.D))
                    _currentFace = 'd';
                else if (keyboardState.IsKeyDown(Keys.L))
                    _currentFace = 'l';
                else if (keyboardState.IsKeyDown(Keys.R))
                    _currentFace = 'r';
                else if (keyboardState.IsKeyDown(Keys.F))
                    _currentFace = 'f';
                else if (keyboardState.IsKeyDown(Keys.B))
                    _currentFace = 'b';
            }

            // 状态管理
            if (!_isRotating && _currentFace != ' ')
            {
                // 开始旋转
                _isRotating = true;
                _timer = 0;
            }
            else if (_isRotating)
            {
                // 正在旋转：根据_rotationDuration内完成PiOver2旋转
                float rotationProgress = Math.Min(1.0f, _timer / _rotationDuration);
                float currentRotation = rotationProgress * MathHelper.PiOver2;

                // 创建旋转矩阵
                Matrix rotation = tool.RotationHelper.CreateRotationMatrix(_currentFace, currentRotation);

                // 更新每个小立方体的世界矩阵
                foreach (var cube in _cubes)
                {
                    bool shouldRotate = ShouldRotateCube(cube, _currentFace);

                    if (shouldRotate)
                    {
                        // 对应面的立方体进行旋转
                        cube.World = cube.OriginalMatrix * rotation;
                    }
                    else
                    {
                        // 其他层：保持不动
                        cube.World = cube.OriginalMatrix;
                    }
                }

                // 旋转完成
                if (rotationProgress >= 1.0f)
                {
                    CompleteRotation();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //_freeCubeScene.Draw(gameTime);

            GraphicsDevice.Clear(Color.Gray);

            _effect.View = _view;
            _effect.Projection = _projection;

            // 遍历所有方块进行绘制
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                foreach (var cube in _cubes)
                {
                    _effect.World = cube.World;
                    pass.Apply();
                    cube.Draw(_effect);
                }
            }

            base.Draw(gameTime);
        }
    }
}