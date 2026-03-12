using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Kociemba;


namespace cube_obj
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

        // 当前的位置（包含旋转后的位置，但平移部分保持整数）
        public Matrix OriginalMatrix { get; set; }

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

        public string CubePieceState()
        {
            // 面的原始顺序：前(0)、后(1)、上(2)、下(3)、右(4)、左(5)
            // 根据 OriginalMatrix 的旋转，确定每个原始面现在朝向哪个方向
            // 返回顺序：前、后、上、下、左、右

            char[] result = new char[6];

            // 从 OriginalMatrix 提取旋转信息
            // 在右手坐标系中，Z轴正方向指向屏幕外（前面）
            Vector3 originalFront = new Vector3(0, 0, 1);   // 原始前面方向
            Vector3 originalBack = new Vector3(0, 0, -1);   // 原始后面方向
            Vector3 originalUp = new Vector3(0, 1, 0);      // 原始上面方向
            Vector3 originalDown = new Vector3(0, -1, 0);   // 原始下面方向
            Vector3 originalRight = new Vector3(1, 0, 0);   // 原始右面方向
            Vector3 originalLeft = new Vector3(-1, 0, 0);   // 原始左面方向

            // 应用旋转矩阵，获取旋转后的方向
            Vector3 rotatedFront = Vector3.TransformNormal(originalFront, OriginalMatrix);
            Vector3 rotatedBack = Vector3.TransformNormal(originalBack, OriginalMatrix);
            Vector3 rotatedUp = Vector3.TransformNormal(originalUp, OriginalMatrix);
            Vector3 rotatedDown = Vector3.TransformNormal(originalDown, OriginalMatrix);
            Vector3 rotatedRight = Vector3.TransformNormal(originalRight, OriginalMatrix);
            Vector3 rotatedLeft = Vector3.TransformNormal(originalLeft, OriginalMatrix);

            // 四舍五入到整数，处理浮点数误差
            rotatedFront = new Vector3((float)Math.Round(rotatedFront.X), (float)Math.Round(rotatedFront.Y), (float)Math.Round(rotatedFront.Z));
            rotatedBack = new Vector3((float)Math.Round(rotatedBack.X), (float)Math.Round(rotatedBack.Y), (float)Math.Round(rotatedBack.Z));
            rotatedUp = new Vector3((float)Math.Round(rotatedUp.X), (float)Math.Round(rotatedUp.Y), (float)Math.Round(rotatedUp.Z));
            rotatedDown = new Vector3((float)Math.Round(rotatedDown.X), (float)Math.Round(rotatedDown.Y), (float)Math.Round(rotatedDown.Z));
            rotatedRight = new Vector3((float)Math.Round(rotatedRight.X), (float)Math.Round(rotatedRight.Y), (float)Math.Round(rotatedRight.Z));
            rotatedLeft = new Vector3((float)Math.Round(rotatedLeft.X), (float)Math.Round(rotatedLeft.Y), (float)Math.Round(rotatedLeft.Z));

            // 根据旋转后的方向，确定每个原始面的颜色应该放在结果数组的哪个位置
            // 结果数组顺序：前(0)、后(1)、上(2)、下(3)、右(4)、左(5)

            // 原始前面的颜色现在朝向 rotatedFront 方向
            if (rotatedFront.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝前
            else if (rotatedFront.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝后
            else if (rotatedFront.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝上
            else if (rotatedFront.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝下
            else if (rotatedFront.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝右
            else if (rotatedFront.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_FRONT]); // 朝左

            // 原始后面的颜色现在朝向 rotatedBack 方向
            if (rotatedBack.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);
            else if (rotatedBack.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);
            else if (rotatedBack.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);
            else if (rotatedBack.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);
            else if (rotatedBack.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);
            else if (rotatedBack.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_BACK]);

            // 原始上面的颜色现在朝向 rotatedUp 方向
            if (rotatedUp.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);
            else if (rotatedUp.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);
            else if (rotatedUp.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);
            else if (rotatedUp.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);
            else if (rotatedUp.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);
            else if (rotatedUp.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_UP]);

            // 原始下面的颜色现在朝向 rotatedDown 方向
            if (rotatedDown.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);
            else if (rotatedDown.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);
            else if (rotatedDown.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);
            else if (rotatedDown.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);
            else if (rotatedDown.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);
            else if (rotatedDown.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_DOWN]);

            // 原始右面的颜色现在朝向 rotatedRight 方向
            if (rotatedRight.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);
            else if (rotatedRight.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);
            else if (rotatedRight.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);
            else if (rotatedRight.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);
            else if (rotatedRight.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);
            else if (rotatedRight.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_RIGHT]);

            // 原始左面的颜色现在朝向 rotatedLeft 方向
            if (rotatedLeft.Z == 1) result[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);
            else if (rotatedLeft.Z == -1) result[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);
            else if (rotatedLeft.Y == 1) result[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);
            else if (rotatedLeft.Y == -1) result[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);
            else if (rotatedLeft.X == 1) result[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);
            else if (rotatedLeft.X == -1) result[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetCharFromColor(_faceColors[tool.RotationHelper.INDEX_LEFT]);

            return new string(result);
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
        private GraphicsDevice _graphicsDevice;
        private List<CubePiece> _cubePiecies; // 存储27个方块

        private float _speed = MathHelper.PiOver2; // 旋转速度 弧度/秒
        private bool _isRotating = false; // 是否正在旋转
        private string _currentCmd = ""; // 当前要旋转的命令
        private float _rotatingTimer = 0; // 旋转时间

        private string _cubeState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"; // 魔方状态字符串
        private string _sloveState;


        public bool CanInputCmd()
        {
            if (_isRotating)
            {
                return false;
            }

            return true;
        }

        public bool InputCmd(string cmd)
        {
            if (!CanInputCmd())
            {
                return false;
            }

            _currentCmd = cmd;
            return true;
        }

        public Cube(GraphicsDevice graphicsDevice, string cubeState, scene.BaseScene baseScene)
        {
            _baseScene = baseScene;
            _graphicsDevice = graphicsDevice;
            _cubeState = cubeState;
            Console.WriteLine("Cube {0}", _cubeState);
        }


        public string SloveCube()
        {
            Console.WriteLine("SloveCube {0}", _cubeState);
            string facelets = _cubeState;
            string info;
            string solution = SearchRunTime.solution(facelets, out info, maxDepth: 22, timeOut: 6000, useSeparator: false);

            // TODO: 做成异步 设置解决中状态
            _sloveState = solution;

            return solution;
        }

        private bool ShouldRotateCube(cube_obj.CubePiece cube, string cmd)
        {
            switch (cmd)
            {
                case tool.RotationHelper.CMD_UP: // 最上层面绕Y轴旋转
                case tool.RotationHelper.CMD_UP_2:
                case tool.RotationHelper.CMD_UP_PRIME:
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - 1) < 0.001f;
                case tool.RotationHelper.CMD_DOWN: // 最下层面绕Y轴旋转
                case tool.RotationHelper.CMD_DOWN_2:
                case tool.RotationHelper.CMD_DOWN_PRIME:
                    return Math.Abs(cube.OriginalMatrix.Translation.Y - (-1)) < 0.001f;
                case tool.RotationHelper.CMD_LEFT: // 最左层面绕X轴旋转
                case tool.RotationHelper.CMD_LEFT_2:
                case tool.RotationHelper.CMD_LEFT_PRIME:
                    return Math.Abs(cube.OriginalMatrix.Translation.X - (-1)) < 0.001f;
                case tool.RotationHelper.CMD_RIGHT: // 最右层面绕X轴旋转
                case tool.RotationHelper.CMD_RIGHT_2:
                case tool.RotationHelper.CMD_RIGHT_PRIME:
                    return Math.Abs(cube.OriginalMatrix.Translation.X - 1) < 0.001f;
                case tool.RotationHelper.CMD_FRONT: // 最前层面绕Z轴旋转（逻辑前面对应Z = 1）
                case tool.RotationHelper.CMD_FRONT_2:
                case tool.RotationHelper.CMD_FRONT_PRIME:
                    return Math.Abs(cube.OriginalMatrix.Translation.Z - 1) < 0.001f;
                case tool.RotationHelper.CMD_BACK: // 最后层面绕Z轴旋转（逻辑后面对应Z = -1）
                case tool.RotationHelper.CMD_BACK_2:
                case tool.RotationHelper.CMD_BACK_PRIME:
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

        public string CubeState()
        {
            // 获取当前魔方的状态，返回一个54字符的字符串
            ///U R F D L B
            // 按照以下顺序获取面：上(y=1)、右(x=1)、前(z=1)、下(y=-1)、左(x=-1)、后(z=-1)、
            // 每个面的颜色按从魔方外部面朝魔方面顺时针方向排列

            string result = "";

            // 上面 (U): y=1，状态字符串位置0-8
            for (int z = -1; z <= 1; z++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    var cube = FindCubeAtPosition(x, 1, z);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // cubeState 顺序：前、后、上、下、左、右
                        // 上面是索引2
                        result += cubeState[2];
                    }
                    else
                    {
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", x, 1, z);
                        result += 'U';
                    }
                }
            }

            // 右面 (R): x=1，状态字符串位置9-17
            for (int y = 1; y >= -1; y--)
            {
                for (int z = 1; z >= -1; z--)
                {
                    var cube = FindCubeAtPosition(1, y, z);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // 右面是索引4
                        result += cubeState[4];
                    }
                    else
                    {
                        result += 'R';
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", 1, y, z);
                    }
                }
            }

            // 前面 (F): z=1，状态字符串位置18-26
            for (int y = 1; y >= -1; y--)
            {
                for (int x = -1; x <= 1; x++)
                {
                    var cube = FindCubeAtPosition(x, y, 1);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // 前面是索引0
                        result += cubeState[0];
                    }
                    else
                    {
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", x, y, 1);
                        result += 'F';
                    }
                }
            }

            // 下面 (D): y=-1，状态字符串位置27-35
            for (int z = 1; z >= -1; z--)
            {
                for (int x = -1; x <= 1; x++)
                {
                    var cube = FindCubeAtPosition(x, -1, z);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // 下面是索引3
                        result += cubeState[3];
                    }
                    else
                    {
                        result += 'D';
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", x, -1, z);
                    }
                }
            }

            // 左面 (L): x=-1，状态字符串位置36-44
            for (int y = 1; y >= -1; y--)
            {
                for (int z = -1; z <= 1; z++)
                {
                    var cube = FindCubeAtPosition(-1, y, z);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // 左面是索引5
                        result += cubeState[5];
                    }
                    else
                    {
                        result += 'L';
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", -1, y, z);
                    }
                }
            }

            // 后面 (B): z=-1，状态字符串位置45-53
            for (int y = 1; y >= -1; y--)
            {
                for (int x = 1; x >= -1; x--)
                {
                    var cube = FindCubeAtPosition(x, y, -1);
                    if (cube != null)
                    {
                        string cubeState = cube.CubePieceState();
                        // 后面是索引1
                        result += cubeState[1];
                    }
                    else
                    {
                        result += 'B';
                        Console.WriteLine($"未找到位置 ({0}, {1}, {2}) 的立方体", x, y, -1);
                    }
                }
            }


            return result;
        }

        private CubePiece FindCubeAtPosition(float x, float y, float z)
        {
            foreach (var cube in _cubePiecies)
            {
                Vector3 pos = cube.OriginalMatrix.Translation;
                if (Math.Abs(pos.X - x) < 0.001f && Math.Abs(pos.Y - y) < 0.001f && Math.Abs(pos.Z - z) < 0.001f)
                {
                    return cube;
                }
            }
            return null;
        }

        private int GetFaceIndex(int x, int y, int z, char face)
        {
            int baseIndex = 0;
            int row = 0;
            int col = 0;

            switch (face)
            {
                case tool.RotationHelper.FACE_FRONT: // 前面 (F): z=1，状态字符串位置18-26
                    baseIndex = 18;
                    row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                    col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                    break;
                case tool.RotationHelper.FACE_BACK: // 后面 (B): z=-1，状态字符串位置45-53
                    baseIndex = 45;
                    row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                    col = 1 - x; // x=-1→col=2, x=0→col=1, x=1→col=0 (镜像)
                    break;
                case tool.RotationHelper.FACE_UP: // 上面 (U): y=1，状态字符串位置0-8
                    baseIndex = 0;
                    row = 1 + z; // z=-1→row=2, z=0→row=1, z=1→row=0
                    col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                    break;
                case tool.RotationHelper.FACE_DOWN: // 下面 (D): y=-1，状态字符串位置27-35
                    baseIndex = 27;
                    row = 1 - z; // z=-1→row=2, z=0→row=1, z=1→row=0
                    col = x + 1; // x=-1→col=0, x=0→col=1, x=1→col=2
                    break;
                case tool.RotationHelper.FACE_RIGHT: // 右面 (R): x=1，状态字符串位置9-17
                    baseIndex = 9;
                    row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                    col = 1 - z; // z=-1→col=0, z=0→col=1, z=1→col=2
                    break;
                case tool.RotationHelper.FACE_LEFT: // 左面 (L): x=-1，状态字符串位置36-44
                    baseIndex = 36;
                    row = 1 - y; // y=-1→row=2, y=0→row=1, y=1→row=0
                    col = 1 + z; // z=-1→col=0, z=0→col=1, z=1→col=2
                    break;
            }

            return baseIndex + row * 3 + col;
        }

        public void createCubeByStage()
        {
            _cubePiecies = new List<CubePiece>();
            // 生成27个方块，位置从 -1 到 1，步长为1，形成3x3x3大立方体
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        // 为每个方块生成6个面的颜色，使用魔方状态字符串
                        Color[] faceColors = new Color[tool.RotationHelper.INDEX_MAX];
                        // 面索引：FACE_FRONT=前(Z正), FACE_BACK=后(Z负), FACE_UP=上(Y正), FACE_DOWN=下(Y负), FACE_RIGHT=右(X正), FACE_LEFT=左(X负)
                        // 上面 (U): y=1，状态字符串位置0-8
                        if (y == 1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_UP);
                            faceColors[tool.RotationHelper.INDEX_UP] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_UP] = Color.Black;
                        }

                        // 右面 (R): x=1，状态字符串位置9-17
                        if (x == 1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_RIGHT);
                            faceColors[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_RIGHT] = tool.ColorHelper.GetDefaultColor();
                        }

                        // 前面 (F): z=1，状态字符串位置18-26
                        if (z == 1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_FRONT);
                            faceColors[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_FRONT] = tool.ColorHelper.GetDefaultColor();
                        }

                        // 下面 (D): y=-1，状态字符串位置27-35
                        if (y == -1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_DOWN);
                            faceColors[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_DOWN] = tool.ColorHelper.GetDefaultColor();
                        }

                        // 左面 (L): x=-1，状态字符串位置36-44
                        if (x == -1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_LEFT);
                            faceColors[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_LEFT] = tool.ColorHelper.GetDefaultColor();
                        }

                        // 后面 (B): z=-1，状态字符串位置45-53
                        if (z == -1)
                        {
                            int index = GetFaceIndex(x, y, z, tool.RotationHelper.FACE_BACK);
                            faceColors[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetColorFromChar(_cubeState[index]);
                        }
                        else
                        {
                            faceColors[tool.RotationHelper.INDEX_BACK] = tool.ColorHelper.GetDefaultColor();
                        }

                        _cubePiecies.Add(new CubePiece(_graphicsDevice, pos, faceColors));
                    }
                }
            }
        }



        protected void CompleteRotationStage()
        {
            switch (_currentCmd)
            {
                case tool.RotationHelper.CMD_UP:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_UP, 1);
                    break;
                case tool.RotationHelper.CMD_UP_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_UP, 2);
                    break;
                case tool.RotationHelper.CMD_UP_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_UP, 3);
                    break;
                case tool.RotationHelper.CMD_FRONT:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_FRONT, 1);
                    break;
                case tool.RotationHelper.CMD_FRONT_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_FRONT, 2);
                    break;
                case tool.RotationHelper.CMD_FRONT_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_FRONT, 3);
                    break;
                case tool.RotationHelper.CMD_BACK:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_BACK, 1);
                    break;
                case tool.RotationHelper.CMD_BACK_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_BACK, 2);
                    break;
                case tool.RotationHelper.CMD_BACK_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_BACK, 3);
                    break;
                case tool.RotationHelper.CMD_DOWN:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_DOWN, 1);
                    break;
                case tool.RotationHelper.CMD_DOWN_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_DOWN, 2);
                    break;
                case tool.RotationHelper.CMD_DOWN_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_DOWN, 3);
                    break;
                case tool.RotationHelper.CMD_RIGHT:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_RIGHT, 1);
                    break;
                case tool.RotationHelper.CMD_RIGHT_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_RIGHT, 2);
                    break;
                case tool.RotationHelper.CMD_RIGHT_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_RIGHT, 3);
                    break;
                case tool.RotationHelper.CMD_LEFT:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_LEFT, 1);
                    break;
                case tool.RotationHelper.CMD_LEFT_2:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_LEFT, 2);
                    break;
                case tool.RotationHelper.CMD_LEFT_PRIME:
                    _cubeState = tool.RotationHelper.RotationStage(_cubeState, tool.RotationHelper.FACE_LEFT, 3);
                    break;
                default:
                    return;
            }

            // 完成旋转后重绘
            createCubeByStage();
            _isRotating = false;
            _rotatingTimer = 0;
            _currentCmd = ""; // 重置为等待输入状态
        }

        // 直接运转stage, 不操作方块
        protected void RotationStageByCmd(string cmd)
        {
            if (_currentCmd == "")
            {
                _currentCmd = cmd;
            }

            if (!_isRotating && _currentCmd != "")
            {
                _isRotating = true;
                _rotatingTimer = 0;
            }
            else if (_isRotating)
            {
                float angle = tool.RotationHelper.GetRotationAngle(_currentCmd);
                // 正在旋转：根据_speed（弧度/秒）计算旋转角度
                float currentRotation = _rotatingTimer * _speed;
                float rotationProgress = Math.Min(1.0f, Math.Abs(currentRotation / angle));

                // 创建旋转矩阵
                Matrix rotation = tool.RotationHelper.CreateRotationMatrix(_currentCmd, currentRotation);

                // 更新每个小立方体的世界矩阵
                foreach (var cube in _cubePiecies)
                {
                    cube.World = ShouldRotateCube(cube, _currentCmd) ? cube.OriginalMatrix * rotation : cube.OriginalMatrix;
                }

                // 旋转完成
                if (rotationProgress >= 1.0f)
                {
                    CompleteRotationStage();
                }
            }
        }

        // 按照指令进行旋转
        protected void RotationByCmd(string cmd)
        {
            if (_currentCmd == "")
            {
                _currentCmd = cmd;
            }

            if (!_isRotating && _currentCmd != "")
            {
                _isRotating = true;
                _rotatingTimer = 0;
            }
            else if (_isRotating)
            {
                float angle = tool.RotationHelper.GetRotationAngle(_currentCmd);
                // 正在旋转：根据_speed（弧度/秒）计算旋转角度
                float currentRotation = _rotatingTimer * _speed;
                float rotationProgress = Math.Min(1.0f, Math.Abs(currentRotation / angle));

                // 创建旋转矩阵
                Matrix rotation = tool.RotationHelper.CreateRotationMatrix(_currentCmd, currentRotation);

                // 更新每个小立方体的世界矩阵
                foreach (var cube in _cubePiecies)
                {
                    cube.World = ShouldRotateCube(cube, _currentCmd) ? cube.OriginalMatrix * rotation : cube.OriginalMatrix;
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
                foreach (var cube in _cubePiecies)
                {
                    effect.World = cube.World;
                    pass.Apply();
                    cube.Draw(effect);
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            _rotatingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // RotationByCmd("");
            RotationStageByCmd("");
        }

        private void CompleteRotation()
        {
            Matrix finalRotation = tool.RotationHelper.CreateRotationMatrix(_currentCmd, Math.Abs(tool.RotationHelper.GetRotationAngle(_currentCmd)));
            for (int i = 0; i < _cubePiecies.Count; i++)
            {
                if (ShouldRotateCube(_cubePiecies[i], _currentCmd))
                {
                    UpdateCubeMatrix(_cubePiecies[i], finalRotation);
                }
            }

            _isRotating = false;
            _rotatingTimer = 0;
            _currentCmd = ""; // 重置为等待输入状态
        }
    }
}