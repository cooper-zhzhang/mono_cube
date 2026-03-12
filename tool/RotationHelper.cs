using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace tool
{
    public class RotationHelper
    {
        // 面标识符常量
        public const char FACE_UP = 'U';
        public const char FACE_DOWN = 'D';
        public const char FACE_LEFT = 'L';
        public const char FACE_RIGHT = 'R';
        public const char FACE_FRONT = 'F';
        public const char FACE_BACK = 'B';

        // 面索引常量（用于数组索引）
        public const int INDEX_FRONT = 0;  // 前(Z正)
        public const int INDEX_BACK = 1;   // 后(Z负)
        public const int INDEX_UP = 2;     // 上(Y正)
        public const int INDEX_DOWN = 3;   // 下(Y负)
        public const int INDEX_RIGHT = 4;  // 右(X正)
        public const int INDEX_LEFT = 5;   // 左(X负)
        public const int INDEX_MAX = 6;   // 最大索引值（用于循环遍历）

        // 旋转指令字符串常量
        public const string CMD_UP = "U";
        public const string CMD_UP_PRIME = "U'";
        public const string CMD_DOWN = "D";
        public const string CMD_DOWN_PRIME = "D'";
        public const string CMD_LEFT = "L";
        public const string CMD_LEFT_PRIME = "L'";
        public const string CMD_RIGHT = "R";
        public const string CMD_RIGHT_PRIME = "R'";
        public const string CMD_FRONT = "F";
        public const string CMD_FRONT_PRIME = "F'";
        public const string CMD_BACK = "B";
        public const string CMD_BACK_PRIME = "B'";
        public const string CMD_UP_2 = "U2";
        public const string CMD_DOWN_2 = "D2";
        public const string CMD_LEFT_2 = "L2";
        public const string CMD_RIGHT_2 = "R2";
        public const string CMD_FRONT_2 = "F2";
        public const string CMD_BACK_2 = "B2";

        // 正值表示顺时针，负值表示逆时针
        public static float GetRotationAngle(string cmd)
        {
            switch (cmd)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN: // 最下层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN_PRIME: // 最下层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_LEFT: // 最左层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_LEFT_PRIME: // 最左层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT: // 最右层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_FRONT: // 最前层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK: // 最后层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_2: // 最上层面绕Y轴旋转
                    return -MathHelper.Pi;
                case CMD_DOWN_2: // 最下层面绕Y轴旋转
                    return MathHelper.Pi;
                case CMD_LEFT_2: // 最左层面绕X轴旋转
                    return MathHelper.Pi;
                case CMD_RIGHT_2: // 最右层面绕X轴旋转
                    return -MathHelper.Pi;
                case CMD_FRONT_2: // 最前层面绕Z轴旋转
                    return -MathHelper.Pi;
                case CMD_BACK_2: // 最后层面绕Z轴旋转
                    return MathHelper.Pi;
                default:
                    return 0;
            }
        }

        public static Matrix CreateRotationMatrix(string face, float angle)
        {
            switch (face)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                case CMD_UP_2:
                    return Matrix.CreateRotationY(-angle);
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN: // 最下层面绕Y轴旋转
                case CMD_DOWN_2:
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN_PRIME: // 最下层面绕Y轴旋转
                    return Matrix.CreateRotationY(-angle);
                case CMD_LEFT: // 最左层面绕X轴旋转
                case CMD_LEFT_2:
                    return Matrix.CreateRotationX(angle);
                case CMD_LEFT_PRIME: // 最左层面绕X轴旋转
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT: // 最右层面绕X轴旋转
                case CMD_RIGHT_2:
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case CMD_FRONT: // 最前层面绕Z轴旋转
                case CMD_FRONT_2:
                    return Matrix.CreateRotationZ(-angle);
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK: // 最后层面绕Z轴旋转
                case CMD_BACK_2:
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                default:
                    return Matrix.Identity;
            }
        }


        // 六个面的顺时针映射 (新索引 : 旧索引)
        public static Dictionary<char, Dictionary<int, int>> clockwiseMaps = new Dictionary<char, Dictionary<int, int>>
        {
            [FACE_UP] = new Dictionary<int, int>
            {
                // U face (0-8)
                {0, 6}, {1, 3}, {2, 0},
                {3, 7}, {4, 4}, {5, 1},
                {6, 8}, {7, 5}, {8, 2},
                // edges: F.top ← R.top ← B.top ← L.top ← F.top
                {18, 9},  {19, 10}, {20, 11},
                {9, 45},  {10, 46}, {11, 47},
                {45, 36}, {46, 37}, {47, 38},
                {36, 18}, {37, 19}, {38, 20}
            },

            [FACE_RIGHT] = new Dictionary<int, int>
            {
                // R face (9-17)
                {9, 15}, {10, 12}, {11, 9},
                {12, 16}, {13, 13}, {14, 10},
                {15, 17}, {16, 14}, {17, 11}
            },

            [FACE_FRONT] = new Dictionary<int, int>
            {
                // F face (18-26)
                {18, 24}, {19, 21}, {20, 18},
                {21, 25}, {22, 22}, {23, 19},
                {24, 26}, {25, 23}, {26, 20}
            },

            [FACE_DOWN] = new Dictionary<int, int>
            {
                // D face (27-35)
                {27, 33}, {28, 30}, {29, 27},
                {30, 34}, {31, 31}, {32, 28},
                {33, 35}, {34, 32}, {35, 29},
                // edges: F.bottom → R.bottom → B.bottom → L.bottom → F.bottom
                {24, 42}, {25, 43}, {26, 44},
                {42, 51}, {43, 52}, {44, 53},
                {51, 15}, {52, 16}, {53, 17},
                {15, 24}, {16, 25}, {17, 26}
            },

            [FACE_LEFT] = new Dictionary<int, int>
            {
                // L face (36-44)
                {36, 42}, {37, 39}, {38, 36},
                {39, 43}, {40, 40}, {41, 37},
                {42, 44}, {43, 41}, {44, 38}
            },

            [FACE_BACK] = new Dictionary<int, int>
            {
                // B face (45-53)
                {45, 51}, {46, 48}, {47, 45},
                {48, 52}, {49, 49}, {50, 46},
                {51, 53}, {52, 50}, {53, 47}
            }
        };



        public static string RotationStage(string cubeState, char face, int times = 1)
        {
            Dictionary<int, int> rotationMap = clockwiseMaps[face];
            for (int i = 0; i < times; i++)
            {
                char[] new_state = cubeState.ToCharArray();
                foreach (var pair in rotationMap)
                {
                    new_state[pair.Key] = cubeState[pair.Value];
                }
                cubeState = new string(new_state);
            }

            return cubeState;
        }
    }
}