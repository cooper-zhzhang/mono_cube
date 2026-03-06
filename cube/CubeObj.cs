using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using tool;

namespace cube_obj
{
    enum PlayType
    {
        Free,
        Solved
    }

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

        // 当前的位置（包含旋转后的位置，但平移部分保持整数）
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

            BuildPiece();
        }

        private void BuildPiece()
        {
            float size = 0.96f; // 方块大小
            // 定义立方体的8个顶点（中心在原点），右手坐标系：Z轴正方向指向屏幕外
            // 因此将原左手系的Z取反，使前面对应Z正
            Vector3[] corners = new Vector3[]
            {
                new Vector3(-size/2, -size/2,  size/2), // 0: 前左下
                new Vector3( size/2, -size/2,  size/2), // 1: 前右下
                new Vector3( size/2,  size/2,  size/2), // 2: 前右上
                new Vector3(-size/2,  size/2,  size/2), // 3: 前左上
                new Vector3(-size/2, -size/2, -size/2), // 4: 后左下
                new Vector3( size/2, -size/2, -size/2), // 5: 后右下
                new Vector3( size/2,  size/2, -size/2), // 6: 后右上
                new Vector3(-size/2,  size/2, -size/2)  // 7: 后左上
            };

            // 每个面对应的顶点索引（三角形带，共12个三角形，每个面2个三角形）
            // 注意：右手系中正面为逆时针，此处顶点顺序仍为顺时针（原左手系习惯），
            // 因此需要设置剔除模式为剔除顺时针（见Initialize），从而保留逆时针为正面
            int[][] faceIndices = new int[][]
            {
                new int[] {0,1,2, 0,2,3}, // 前面 (Z正)
                new int[] {4,6,5, 4,7,6}, // 后面 (Z负)
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

    public class Cube
    {


        private scene.BaseScene _baseScene;
        private PlayType _playType = PlayType.Free;
        private GraphicsDevice _graphicsDevice;
        private List<CubePiece> _cubes; // 存储27个方块
        private float _rotationDuration = 0.5f; // 旋转持续时间（秒），旋转90度的时间
        
        private float _speed = MathHelper.PiOver2; // 旋转速度 弧度/秒
        private bool _isRotating = false; // 是否正在旋转
        private string _currentFace = ""; // 当前要旋转的面
        private float __rotatingTimer = 0; // 旋转时间

        private string _cubeState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"; // 魔方状态字符串

        public Cube(GraphicsDevice graphicsDevice, string cubeState)
        {
            _graphicsDevice = graphicsDevice;
            _cubeState = cubeState;
        }

        private bool ShouldRotateCube(cube_obj.CubePiece cube, string face)
        {
            switch (face)
            {
                case "u": // 最上层面绕Y轴旋转
                case "u2":
                case "u'":
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - 1) < 0.001f;
                case "d": // 最下层面绕Y轴旋转
                case "d2":
                case "d'":
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - (-1)) < 0.001f;
                case "l": // 最左层面绕X轴旋转
                case "l2":
                case "l'":
                    return Math.Abs(cube.OriginalMatrix.Translation.X - (-1)) < 0.001f;
                case "r": // 最右层面绕X轴旋转
                case "r2":
                case "r'":
                    return Math.Abs(cube.OriginalMatrix.Translation.X - 1) < 0.001f;
                case "f": // 最前层面绕Z轴旋转（逻辑前面对应Z = 1）
                case "f2":
                case "f'":
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - 1) < 0.001f;
                case "b": // 最后层面绕Z轴旋转（逻辑后面对应Z = -1）
                case "b2":
                case "b'":
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - (-1)) < 0.001f;
                default:
                    return false;
            }
        }


        // 完成一次旋转后整体更新每个小立方体的矩阵
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

        public void createCubeByStage()
        {

            _cubes = new List<CubePiece>();

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
                            faceColors[4] = tool.ColorHelper.GetDefaultColor();
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
                            faceColors[0] = tool.ColorHelper.GetDefaultColor();
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
                            faceColors[3] = tool.ColorHelper.GetDefaultColor();
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
                            faceColors[5] = tool.ColorHelper.GetDefaultColor();
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
                            faceColors[1] = tool.ColorHelper.GetDefaultColor();
                        }

                        _cubes.Add(new CubePiece(_graphicsDevice, pos, faceColors));
                    }
                }
            }
        }

        // 按照指令进行旋转
        protected void RotationByFace(string face)
        {
            if(_currentFace == ""){
                _currentFace = face;
            }

            if (!_isRotating && _currentFace != "")
            {
                // 开始旋转
                _isRotating = true;
                __rotatingTimer = 0;
            }
            else if (_isRotating)
            {
                float angle = tool.RotationHelper.GetRotationAngle(_currentFace);
                // 正在旋转：根据_speed（弧度/秒）计算旋转角度
                float currentRotation = __rotatingTimer * _speed;
                float rotationProgress = Math.Min(1.0f, currentRotation / angle);
                // 创建旋转矩阵
                Matrix rotation = tool.RotationHelper.CreateRotationMatrix(_currentFace, currentRotation);

                // 更新每个小立方体的世界矩阵
                foreach (var cube in _cubes)
                {
                    cube.World  = ShouldRotateCube(cube, _currentFace)?cube.OriginalMatrix * rotation : cube.OriginalMatrix;
                }

                // 旋转完成
                if (rotationProgress >= 1.0f)
                {
                    CompleteRotation();
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            BasicEffect effect = _baseScene._effect;
            effect.View = _baseScene._view;
            effect.Projection = _baseScene._projection;

            // 遍历所有方块进行绘制
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                foreach (var cube in _cubes)
                {
                    effect.World = cube.World;
                    pass.Apply();
                    cube.Draw(effect);
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            // 计时器累加
            __rotatingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            string currentFace = "";
            // 检测键盘输入（只在等待输入状态时检测）
            if (!_isRotating && _currentFace == "")
            {
                var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.U))
                    currentFace = tool.RotationHelper.CMD_UP;
                else if (keyboardState.IsKeyDown(Keys.D))
                    currentFace = tool.RotationHelper.CMD_DOWN;
                else if (keyboardState.IsKeyDown(Keys.L))
                    currentFace = tool.RotationHelper.CMD_LEFT;
                else if (keyboardState.IsKeyDown(Keys.R))
                    currentFace = tool.RotationHelper.CMD_RIGHT;
                else if (keyboardState.IsKeyDown(Keys.F))
                    currentFace = tool.RotationHelper.CMD_FRONT;
                else if (keyboardState.IsKeyDown(Keys.B))
                    currentFace = tool.RotationHelper.CMD_BACK;
            }

            RotationByFace(currentFace);
        }

        private void CompleteRotation()
        {
            Matrix finalRotation = tool.RotationHelper.CreateRotationMatrix(_currentFace, MathHelper.PiOver2);
            for (int i = 0; i < _cubes.Count; i++)
            {
                if (ShouldRotateCube(_cubes[i], _currentFace))
                {
                    UpdateCubeMatrix(_cubes[i], finalRotation);
                }
            }

            _isRotating = false;
            __rotatingTimer = 0;
            _currentFace = ""; // 重置为等待输入状态
        }
    }
}