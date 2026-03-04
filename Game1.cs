using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;



namespace cube_game
{
    public class CubePiece
    {
        private GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _primitiveCount;

        // 方块的当前世界矩阵（位置+旋转）
        public Matrix World { get; set; }

        // 原始位置（固定），用于旋转时参考
        public Vector3 OriginalPosition { get; private set; }

        // 当前的位置
        public Matrix OriginalMatrix { get; set; }

        // 更新原始位置
        public void UpdateOriginalPosition(Vector3 newPosition)
        {
            OriginalPosition = newPosition;
        }

        // 每个面的颜色（数组顺序：前、后、上、下、左、右）
        private Color[] _faceColors;

        public CubePiece(GraphicsDevice graphicsDevice, Vector3 position, Color[] faceColors)
        {
            _graphicsDevice = graphicsDevice;
            OriginalPosition = position;
            World = Matrix.CreateTranslation(position);
            OriginalMatrix = World;
            _faceColors = faceColors;

            BuildCube();
        }

        private void BuildCube()
        {
            float size = 0.96f; // 方块大小
                                // 定义立方体的8个顶点（中心在原点）
            Vector3[] corners = new Vector3[]
            {
            new Vector3(-size/2, -size/2, -size/2), // 0: 前左下
            new Vector3( size/2, -size/2, -size/2), // 1: 前右下
            new Vector3( size/2,  size/2, -size/2), // 2: 前右上
            new Vector3(-size/2,  size/2, -size/2), // 3: 前左上
            new Vector3(-size/2, -size/2,  size/2), // 4: 后左下
            new Vector3( size/2, -size/2,  size/2), // 5: 后右下
            new Vector3( size/2,  size/2,  size/2), // 6: 后右上
            new Vector3(-size/2,  size/2,  size/2)  // 7: 后左上
            };

            // 每个面对应的顶点索引（三角形带，共12个三角形，每个面2个三角形）
            int[][] faceIndices = new int[][]
            {
            new int[] {0,1,2, 0,2,3}, // 前面 (Z负)
            new int[] {4,6,5, 4,7,6}, // 后面 (Z正)
            new int[] {3,2,6, 3,6,7}, // 上面 (Y正)
            new int[] {0,5,1, 0,4,5}, // 下面 (Y负)
            new int[] {1,5,6, 1,6,2}, // 右面 (X正)
            new int[] {0,7,4, 0,3,7}  // 左面 (X负)
            };

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            List<ushort> indices = new List<ushort>();

            // 为每个面生成顶点（每个面4个顶点，因为颜色不同，不能共享顶点）
            for (int f = 0; f < 6; f++)
            {
                Color faceColor = _faceColors[f];
                int[] faceTriangles = faceIndices[f];
                // 每个面需要4个顶点（两个三角形共6个索引）
                for (int i = 0; i < faceTriangles.Length; i++)
                {
                    int cornerIndex = faceTriangles[i];
                    Vector3 pos = corners[cornerIndex];
                    vertices.Add(new VertexPositionColor(pos, faceColor));
                    indices.Add((ushort)(vertices.Count - 1));
                }
            }

            _vertexBuffer = new VertexBuffer(_graphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices.ToArray());

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices.ToArray());

            _primitiveCount = indices.Count / 3;
        }

        public void Draw(BasicEffect effect)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
        }
    }


    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private BasicEffect _effect;
        private Matrix _view;
        private Matrix _projection;
        private List<CubePiece> _cubes; // 存储27个方块
        private float _timer = 0; // 计时器
        private float _rotationProgress = 0; // 旋转进度（0-1）
        private float _rotationDuration = 02f; // 旋转持续时间（秒），越小速度越快 旋转90度的时间
        private bool _isRotating = false; // 是否正在旋转

        private char _currentFace = ' '; // 当前要旋转的面，' '表示等待输入
        
        // 创建旋转矩阵
        private Matrix CreateRotationMatrix(char face, float angle)
        {
            switch (face)
            {
                case 'u': // 最上层面绕y轴旋转（顺时针）
                    return Matrix.CreateRotationY(-angle);
                case 'd': // 最下层面绕y轴旋转（逆时针，与最上面相反）
                    return Matrix.CreateRotationY(angle);
                case 'l': // 最左层面绕x轴旋转
                    return Matrix.CreateRotationX(angle);
                case 'r': // 最右层面绕x轴旋转（与最左面相反）
                    return Matrix.CreateRotationX(-angle);
                case 'f': // 最前层面绕z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                case 'b': // 最后层面绕z轴旋转（与最前面相反）
                    return Matrix.CreateRotationZ(-angle);
                default:
                    return Matrix.Identity;
            }
        }
        
        // 检查立方体是否需要旋转
        private bool ShouldRotateCube(CubePiece cube, char face)
        {
            switch (face)
            {
                case 'u': // 最上层面绕y轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - 1) < 0.001f;
                case 'd': // 最下层面绕y轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - (-1)) < 0.001f;
                case 'l': // 最左层面绕x轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.X - (-1)) < 0.001f;
                case 'r': // 最右层面绕x轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.X - 1) < 0.001f;
                case 'f': // 最前层面绕z轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - 1) < 0.001f;
                case 'b': // 最后层面绕z轴旋转
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - (-1)) < 0.001f;
                default:
                    return false;
            }
        }
        
        // 更新立方体矩阵，应用旋转并四舍五入位置
        private void UpdateCubeMatrix(CubePiece cube, Matrix rotation)
        {
            // 使用OriginalMatrix和完整的旋转角度一次计算，减少误差
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
            // 使用完整的旋转角度（PiOver2）一次计算，减少浮点数误差
            Matrix finalRotation = CreateRotationMatrix(_currentFace, MathHelper.PiOver2);

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            // 设置相机，将视点拉远以看到整个大立方体
            _view = Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);

            _cubes = new List<CubePiece>();

            // 生成27个方块，位置从 -1 到 1，步长为1，形成3x3x3大立方体
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        // 为每个方块生成6个面的颜色，使用魔方标准颜色
                        Color[] faceColors = new Color[6];
                        // 魔方标准颜色：前红、后橙、上白、下黄、左绿、右蓝
                        faceColors[0] = (z == -1) ? Color.Red : Color.Black;     // 前面 (Z正)
                        faceColors[1] = (z == 1) ? Color.Orange : Color.Black;   // 后面 (Z负)
                        faceColors[2] = (y == 1) ? Color.White : Color.Black;    // 上面 (Y正)
                        faceColors[3] = (y == -1) ? Color.Yellow : Color.Black;  // 下面 (Y负)
                        faceColors[4] = (x == 1) ? Color.Blue : Color.Black;     // 右面 (X正) - 索引4
                        faceColors[5] = (x == -1) ? Color.Green : Color.Black;   // 左面 (X负) - 索引5

                        _cubes.Add(new CubePiece(GraphicsDevice, pos, faceColors));
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
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
                _rotationProgress = 0;
                _timer = 0;
            }
            else if (_isRotating)
            {
                // 正在旋转：根据_rotationDuration内完成PiOver2旋转
                _rotationProgress = Math.Min(1.0f, _timer / _rotationDuration);
                float currentRotation = _rotationProgress * MathHelper.PiOver2;

                // 创建旋转矩阵
                Matrix rotation = CreateRotationMatrix(_currentFace, currentRotation);

                // 更新每个小立方体的世界矩阵
                foreach (var cube in _cubes)
                {
                    bool shouldRotate = false;

                    // 根据_currentFace的值选择对应的面进行旋转
                    shouldRotate = ShouldRotateCube(cube, _currentFace);

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
                if (_rotationProgress >= 1.0f)
                {
                    CompleteRotation();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
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