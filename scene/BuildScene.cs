using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using tool;

namespace cube_game_scene
{
    public class BuildScene : BaseScene
    {
        private string _cubeState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";
        private int _selectedFacelet = -1;
        private Color[] _faceletColors = new Color[54];
        private Vector2[] _faceletPositions = new Vector2[54];
        private int _faceletSize = 40;
        private int _margin = 10;
        
        // 当前选中的颜色
        private char _selectedColor = 'U';
        private Color[] _availableColors = new Color[] { ColorHelper.White, ColorHelper.Yellow, ColorHelper.Red, ColorHelper.Orange, ColorHelper.Green, ColorHelper.Blue};
        private char[] _availableColorChars = new char[] { 'U', 'D', 'F', 'B', 'L', 'R' };
        private string[] _colorNames = new string[] { "U(白)", "D(黄)", "F(红)", "B(橙)", "L(绿)", "R(蓝)" };
        
        // 颜色选择器位置
        private Vector2 _colorSelectorPosition;
        private int _colorSelectorSize = 30;
        private int _colorSelectorMargin = 10;
        
        // 输出按钮位置
        private Rectangle _outputButtonRect;
        private Color _outputButtonColor = Color.LightBlue;
        private Color _outputButtonHoverColor = Color.LightSkyBlue;
        private bool _isOutputButtonHovered = false;
        
        // 用于2D绘制的精灵批处理
        private SpriteBatch _spriteBatch;
        private Texture2D _pixelTexture;

        public override string Name => "BuildScene";

        public BuildScene() : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up),  Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            Debug.WriteLine("[BuildScene] 构造函数被调用");
        }

        public override void Initialize()
        {
            Debug.WriteLine("[BuildScene] Initialize 开始");
            base.Initialize();
            
            // 验证初始状态
            Debug.WriteLine($"[BuildScene] 初始魔方状态长度: {_cubeState.Length}");
            Debug.WriteLine($"[BuildScene] 初始魔方状态: {_cubeState}");
            
            UpdateFaceletColors();
            CalculateFaceletPositions();
            CalculateColorSelectorPosition();
            CalculateOutputButtonPosition();
            
            // 验证面块位置计算
            Debug.WriteLine($"[BuildScene] 面块位置计算完成，共 {_faceletPositions.Length} 个面块");
            Debug.WriteLine($"[BuildScene] U面(0-8)位置: {_faceletPositions[0]}, {_faceletPositions[4]}, {_faceletPositions[8]}");
            Debug.WriteLine($"[BuildScene] F面(18-26)位置: {_faceletPositions[18]}, {_faceletPositions[22]}, {_faceletPositions[26]}");
            
            Debug.WriteLine("[BuildScene] Initialize 完成");
        }

        public override void LoadContent()
        {
            Debug.WriteLine("[BuildScene] LoadContent 开始");
            base.LoadContent();
            
            // 创建精灵批处理和像素纹理
            _spriteBatch = new SpriteBatch(Graphics.GraphicsDevice);
            _pixelTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            Debug.WriteLine("[BuildScene] LoadContent 完成");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 处理鼠标移动 - 检查按钮悬停状态
            MouseState mouseState = cube_game.Core.Input.Mouse.CurrentState;
            Vector2 mousePos = new Vector2(cube_game.Core.Input.Mouse.X, cube_game.Core.Input.Mouse.Y);
            _isOutputButtonHovered = _outputButtonRect.Contains((int)mousePos.X, (int)mousePos.Y);

            if (cube_game.Core.Input.Mouse.IsButtonDown(cube_game_input.MouseButton.Left))
            {
                Debug.WriteLine($"[BuildScene] 鼠标点击位置: ({mousePos.X}, {mousePos.Y})");
                // 检查是否点击了输出按钮
                if (_outputButtonRect.Contains((int)mousePos.X, (int)mousePos.Y))
                {
                    string outputString = GetCubeStateString();
                    Console.WriteLine($"魔方状态字符串: {outputString}");
                    Debug.WriteLine($"[BuildScene] 输出魔方状态: {outputString}");

                    cube_game.Core.Instance.SetBlackboard("buildCubeState", _cubeState);
                    cube_game.Core.ChangeScene(new SloveCubeScene());
                    
                    return;
                }
                
                // 检查是否点击了颜色选择器
                for (int i = 0; i < 6; i++)
                {
                    Rectangle colorRect = new Rectangle(
                        (int)_colorSelectorPosition.X + i * (_colorSelectorSize + _colorSelectorMargin),
                        (int)_colorSelectorPosition.Y,
                        _colorSelectorSize,
                        _colorSelectorSize
                    );
                    if (colorRect.Contains((int)mousePos.X, (int)mousePos.Y))
                    {
                        char oldColor = _selectedColor;
                        _selectedColor = _availableColorChars[i];
                        Debug.WriteLine($"[BuildScene] 颜色选择器被点击: 从 {oldColor} 变为 {_selectedColor} ({_colorNames[i]})");
                        break;
                    }
                }
                
                // 检查是否点击了魔方方块
                for (int i = 0; i < 54; i++)
                {
                    Rectangle faceletRect = new Rectangle(
                        (int)_faceletPositions[i].X, 
                        (int)_faceletPositions[i].Y, 
                        _faceletSize, 
                        _faceletSize
                    );
                    if (faceletRect.Contains((int)mousePos.X, (int)mousePos.Y))
                    {
                        char oldColor = _cubeState[i];
                        _selectedFacelet = i;
                        // 将选中的颜色应用到方块
                        _cubeState = _cubeState.Substring(0, i) + _selectedColor + _cubeState.Substring(i + 1);
                        UpdateFaceletColors();
                        Debug.WriteLine($"[BuildScene] 方块 {i} 被点击: 从 {oldColor} 变为 {_selectedColor}");
                        Debug.WriteLine($"[BuildScene] 当前魔方状态: {_cubeState}");
                        break;
                    }
                }
            }

            // 处理键盘输入 - 选择颜色
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.U))
            {
                if (_selectedColor != 'U')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 U");
                    _selectedColor = 'U';
                }
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                if (_selectedColor != 'D')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 D");
                    _selectedColor = 'D';
                }
            }
            else if (keyboardState.IsKeyDown(Keys.F))
            {
                if (_selectedColor != 'F')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 F");
                    _selectedColor = 'F';
                }
            }
            else if (keyboardState.IsKeyDown(Keys.B))
            {
                if (_selectedColor != 'B')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 B");
                    _selectedColor = 'B';
                }
            }
            else if (keyboardState.IsKeyDown(Keys.L))
            {
                if (_selectedColor != 'L')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 L");
                    _selectedColor = 'L';
                }
            }
            else if (keyboardState.IsKeyDown(Keys.R))
            {
                if (_selectedColor != 'R')
                {
                    Debug.WriteLine($"[BuildScene] 键盘选择颜色: 从 {_selectedColor} 变为 R");
                    _selectedColor = 'R';
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Debug.WriteLine("[BuildScene] Draw 开始");
            
            // 绘制背景
            Graphics.GraphicsDevice.Clear(Color.Gray);
            Debug.WriteLine("[BuildScene] 背景已清除为灰色");

            // 检查资源是否已加载
            if (_spriteBatch == null)
            {
                Debug.WriteLine("[BuildScene] 错误: _spriteBatch 为 null");
                return;
            }
            if (_pixelTexture == null)
            {
                Debug.WriteLine("[BuildScene] 错误: _pixelTexture 为 null");
                return;
            }
            
            Debug.WriteLine($"[BuildScene] 绘制 {54} 个面块");
            Debug.WriteLine($"[BuildScene] 颜色选择器位置: {_colorSelectorPosition}");
            Debug.WriteLine($"[BuildScene] 第一个面块位置: {_faceletPositions[0]}, 颜色: {_faceletColors[0]}");

            try
            {
                // 使用 SpriteBatch 进行 2D 绘制
                _spriteBatch.Begin();
                Debug.WriteLine("[BuildScene] SpriteBatch.Begin() 成功");

                // 绘制颜色选择器
                for (int i = 0; i < 6; i++)
                {
                    Rectangle colorRect = new Rectangle(
                        (int)_colorSelectorPosition.X + i * (_colorSelectorSize + _colorSelectorMargin),
                        (int)_colorSelectorPosition.Y,
                        _colorSelectorSize,
                        _colorSelectorSize
                    );
                    
                    // 绘制颜色块
                    _spriteBatch.Draw(_pixelTexture, colorRect, _availableColors[i]);
                    
                    // 绘制选中标记
                    if (_availableColorChars[i] == _selectedColor)
                    {
                        DrawRectangleBorder(colorRect, Color.Red, 3);
                    }
                    else
                    {
                        DrawRectangleBorder(colorRect, Color.Black, 1);
                    }
                }
                Debug.WriteLine("[BuildScene] 颜色选择器绘制完成");

                // 绘制输出按钮
                Color buttonColor = _isOutputButtonHovered ? _outputButtonHoverColor : _outputButtonColor;
                _spriteBatch.Draw(_pixelTexture, _outputButtonRect, buttonColor);
                DrawRectangleBorder(_outputButtonRect, Color.Black, 2);
                Debug.WriteLine("[BuildScene] 输出按钮绘制完成");

                // 绘制魔方展开图
                for (int i = 0; i < 54; i++)
                {
                    // 绘制面块
                    Rectangle faceletRect = new Rectangle(
                        (int)_faceletPositions[i].X, 
                        (int)_faceletPositions[i].Y, 
                        _faceletSize, 
                        _faceletSize
                    );

                    // 绘制面块颜色
                    _spriteBatch.Draw(_pixelTexture, faceletRect, _faceletColors[i]);

                    // 绘制面块边框
                    if (i == _selectedFacelet)
                    {
                        // 选中的面块使用红色边框
                        DrawRectangleBorder(faceletRect, Color.Red, 3);
                    }
                    else
                    {
                        // 未选中的面块使用黑色边框
                        DrawRectangleBorder(faceletRect, Color.Black, 1);
                    }
                }
                Debug.WriteLine("[BuildScene] 魔方展开图绘制完成");
                
                _spriteBatch.End();
                Debug.WriteLine("[BuildScene] SpriteBatch.End() 成功");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BuildScene] 绘制时发生异常: {ex.Message}");
                Debug.WriteLine($"[BuildScene] 异常堆栈: {ex.StackTrace}");
            }
            
            Debug.WriteLine("[BuildScene] Draw 完成");
        }

        private void UpdateFaceletColors()
        {
            Debug.WriteLine("[BuildScene] UpdateFaceletColors 开始");
            for (int i = 0; i < 54; i++)
            {
                Color oldColor = _faceletColors[i];
                _faceletColors[i] = ColorHelper.GetColorFromChar(_cubeState[i]);
                if (oldColor != _faceletColors[i])
                {
                    Debug.WriteLine($"[BuildScene] 面块 {i} 颜色更新: 从 {oldColor} 变为 {_faceletColors[i]}");
                }
            }
            Debug.WriteLine("[BuildScene] UpdateFaceletColors 完成");
        }

        private void CalculateFaceletPositions()
        {
            Debug.WriteLine("[BuildScene] CalculateFaceletPositions 开始");
            int screenWidth = Graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = Graphics.GraphicsDevice.Viewport.Height;
            Debug.WriteLine($"[BuildScene] 屏幕尺寸: {screenWidth}x{screenHeight}");

            // 计算中心位置
            int centerX = screenWidth / 2;
            int centerY = screenHeight / 2 - 20;  // 向上移动20像素
            Debug.WriteLine($"[BuildScene] 中心位置: ({centerX}, {centerY})");

            // 计算一个面的总宽度/高度（3个面块 + 2个间距）
            int faceWidth = 3 * _faceletSize + 2 * _margin;
            int faceHeight = 3 * _faceletSize + 2 * _margin;

            // 定义面的位置 - 使用标准魔方展开图布局
            //       [U]
            // [L]  [F]  [R]  [B]
            //       [D]

            // U面 (索引 0-8) - 在F面上方
            Debug.WriteLine("[BuildScene] 计算 U面 位置 (索引 0-8)");
            int uStartX = centerX - faceWidth / 2;
            int uStartY = centerY - faceHeight - faceHeight / 2 - _margin;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        uStartX + col * (_faceletSize + _margin),
                        uStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] U面位置: 左上={_faceletPositions[0]}, 中心={_faceletPositions[4]}, 右下={_faceletPositions[8]}");

            // L面 (索引 36-44) - 在F面左侧
            Debug.WriteLine("[BuildScene] 计算 L面 位置 (索引 36-44)");
            int lStartX = centerX - 2 * faceWidth - _margin * 2 + 70;  // 向右移动10像素
            int lStartY = centerY - faceHeight / 2;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = 36 + row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        lStartX + col * (_faceletSize + _margin),
                        lStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] L面位置: 左上={_faceletPositions[36]}, 中心={_faceletPositions[40]}, 右下={_faceletPositions[44]}");

            // F面 (索引 18-26) - 中心位置
            Debug.WriteLine("[BuildScene] 计算 F面 位置 (索引 18-26)");
            int fStartX = centerX - faceWidth / 2;
            int fStartY = centerY - faceHeight / 2;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = 18 + row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        fStartX + col * (_faceletSize + _margin),
                        fStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] F面位置: 左上={_faceletPositions[18]}, 中心={_faceletPositions[22]}, 右下={_faceletPositions[26]}");

            // R面 (索引 9-17) - 在F面右侧
            Debug.WriteLine("[BuildScene] 计算 R面 位置 (索引 9-17)");
            int rStartX = centerX + faceWidth / 2 + _margin * 2;
            int rStartY = centerY - faceHeight / 2;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = 9 + row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        rStartX + col * (_faceletSize + _margin),
                        rStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] R面位置: 左上={_faceletPositions[9]}, 中心={_faceletPositions[13]}, 右下={_faceletPositions[17]}");

            // B面 (索引 45-53) - 在R面右侧
            Debug.WriteLine("[BuildScene] 计算 B面 位置 (索引 45-53)");
            int bStartX = centerX + faceWidth / 2 + faceWidth + _margin * 4;
            int bStartY = centerY - faceHeight / 2;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = 45 + row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        bStartX + col * (_faceletSize + _margin),
                        bStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] B面位置: 左上={_faceletPositions[45]}, 中心={_faceletPositions[49]}, 右下={_faceletPositions[53]}");

            // D面 (索引 27-35) - 在F面下方
            Debug.WriteLine("[BuildScene] 计算 D面 位置 (索引 27-35)");
            int dStartX = centerX - faceWidth / 2;
            int dStartY = centerY + faceHeight / 2 + _margin * 2;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int index = 27 + row * 3 + col;
                    _faceletPositions[index] = new Vector2(
                        dStartX + col * (_faceletSize + _margin),
                        dStartY + row * (_faceletSize + _margin)
                    );
                }
            }
            Debug.WriteLine($"[BuildScene] D面位置: 左上={_faceletPositions[27]}, 中心={_faceletPositions[31]}, 右下={_faceletPositions[35]}");
            
            Debug.WriteLine("[BuildScene] CalculateFaceletPositions 完成");
        }

        private void CalculateColorSelectorPosition()
        {
            Debug.WriteLine("[BuildScene] CalculateColorSelectorPosition 开始");
            int screenWidth = Graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = Graphics.GraphicsDevice.Viewport.Height;
            
            // 计算 D 面的底部位置（D面在F面下方，索引27-35）
            int faceWidth = 3 * _faceletSize + 2 * _margin;
            int faceHeight = 3 * _faceletSize + 2 * _margin;
            int centerX = screenWidth / 2;
            int centerY = screenHeight / 2;
            int dStartY = centerY + faceHeight / 2 + _margin * 2;
            int dBottomY = dStartY + faceHeight;
            
            // 将颜色选择器放在 D 面下方，留出足够的间距
            int totalWidth = 6 * _colorSelectorSize + 5 * _colorSelectorMargin;
            _colorSelectorPosition = new Vector2(
                (screenWidth - totalWidth) / 2,
                dBottomY + 50 - 30  // 在D面下方50像素处，再向上移动30像素
            );
            Debug.WriteLine($"[BuildScene] D面底部位置: {dBottomY}");
            Debug.WriteLine($"[BuildScene] 颜色选择器位置: {_colorSelectorPosition}, 总宽度: {totalWidth}");
            Debug.WriteLine("[BuildScene] CalculateColorSelectorPosition 完成");
        }

        private void CalculateOutputButtonPosition()
        {
            Debug.WriteLine("[BuildScene] CalculateOutputButtonPosition 开始");
            
            // 将输出按钮放在颜色选择器的右边
            int buttonWidth = 120;
            int buttonHeight = 40;
            int colorSelectorRightX = (int)_colorSelectorPosition.X + 6 * _colorSelectorSize + 5 * _colorSelectorMargin;
            
            _outputButtonRect = new Rectangle(
                colorSelectorRightX + 20,
                (int)_colorSelectorPosition.Y,
                buttonWidth,
                buttonHeight
            );
            Debug.WriteLine($"[BuildScene] 输出按钮位置: {_outputButtonRect}");
            Debug.WriteLine("[BuildScene] CalculateOutputButtonPosition 完成");
        }

        private void DrawRectangleBorder(Rectangle rect, Color color, int thickness)
        {
            // 绘制上边框
            _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Top, rect.Width, thickness), color);
            // 绘制下边框
            _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Bottom - thickness, rect.Width, thickness), color);
            // 绘制左边框
            _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Top, thickness, rect.Height), color);
            // 绘制右边框
            _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Right - thickness, rect.Top, thickness, rect.Height), color);
        }

        private string GetCubeStateString()
        {
            return _cubeState;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _spriteBatch?.Dispose();
                _pixelTexture?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}