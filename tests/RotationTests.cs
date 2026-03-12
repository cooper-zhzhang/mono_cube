using System;
using System.Linq;

namespace cube_game.tests
{
    public class RotationTests
    {
        private const string InitialState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";
        private static readonly string[] Faces = { "U", "R", "F", "D", "L", "B" };

        public static void RunAllTests()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("    魔方旋转映射测试");
            Console.WriteLine("========================================\n");

            TestSingleFaceRotations();
            TestSimpleCancelCombinations();
            TestOppositeFaceCombinations();
            TestFormulaPeriods();
            TestAllFacesRotation();
            TestMultipleRotations();
            TestRandomScrambleAndRestore();
            TestSpecificScramble();
            TestAllRotationCounts();
            TestLongerFormulas();
            TestSequentialRotation();

            Console.WriteLine("\n========================================");
            Console.WriteLine("    所有测试完成！");
            Console.WriteLine("========================================");
        }

        private static void TestSingleFaceRotations()
        {
            Console.WriteLine("=== 测试每个面旋转4次 ===");
            foreach (string face in Faces)
            {
                string ret = tool.RotationHelper.RotationStage(InitialState, face[0], 4);
                bool isCorrect = ret == InitialState;
                Console.WriteLine($"Face {face}: {(isCorrect ? "正确 ✓" : "错误 ✗")}");
            }

            // 测试旋转一次后状态与原状态不相等
            Console.WriteLine("\n=== 测试旋转一次后状态与原状态不相等 ===");
            foreach (string face in Faces)
            {
                string ret = tool.RotationHelper.RotationStage(InitialState, face[0], 1);
                bool isDifferent = ret != InitialState;
                Console.WriteLine($"Face {face} 旋转一次后: {(isDifferent ? "不同 ✓" : "相同 ✗")}");
            }
            Console.WriteLine();
        }

        private static void TestSimpleCancelCombinations()
        {
            Console.WriteLine("=== 测试简单抵消组合 ===");
            Console.WriteLine($"U U': {TestCombo(InitialState, new[] { ('U', 1), ('U', 3) })}");
            Console.WriteLine($"R R': {TestCombo(InitialState, new[] { ('R', 1), ('R', 3) })}");
            Console.WriteLine($"F F': {TestCombo(InitialState, new[] { ('F', 1), ('F', 3) })}");
            Console.WriteLine($"D D': {TestCombo(InitialState, new[] { ('D', 1), ('D', 3) })}");
            Console.WriteLine($"L L': {TestCombo(InitialState, new[] { ('L', 1), ('L', 3) })}");
            Console.WriteLine($"B B': {TestCombo(InitialState, new[] { ('B', 1), ('B', 3) })}");
            Console.WriteLine();
        }

        private static void TestOppositeFaceCombinations()
        {
            Console.WriteLine("=== 测试顺序无关的组合 (对面旋转) ===");
            Console.WriteLine($"U D U' D': {TestCombo(InitialState, new[] { ('U', 1), ('D', 1), ('U', 3), ('D', 3) })}");
            Console.WriteLine($"R L R' L': {TestCombo(InitialState, new[] { ('R', 1), ('L', 1), ('R', 3), ('L', 3) })}");
            Console.WriteLine($"F B F' B': {TestCombo(InitialState, new[] { ('F', 1), ('B', 1), ('F', 3), ('B', 3) })}");
            Console.WriteLine();
        }

        private static void TestFormulaPeriods()
        {
            Console.WriteLine("=== 测试常见魔方公式的周期 ===");
            TestFormulaPeriod("R U R' U'", InitialState, new[] { ('R', 1), ('U', 1), ('R', 3), ('U', 3) });
            TestFormulaPeriod("F R F' R'", InitialState, new[] { ('F', 1), ('R', 1), ('F', 3), ('R', 3) });
            TestFormulaPeriod("R U2 R' U'", InitialState, new[] { ('R', 1), ('U', 2), ('R', 3), ('U', 3) });
            TestFormulaPeriod("U R U' L'", InitialState, new[] { ('U', 1), ('R', 1), ('U', 3), ('L', 3) });
            TestFormulaPeriod("Sune", InitialState, new[] { ('R', 1), ('U', 1), ('R', 3), ('U', 1), ('R', 1), ('U', 2), ('R', 3) });
            TestFormulaPeriod("Antisune", InitialState, new[] { ('R', 1), ('U', 2), ('R', 3), ('U', 3), ('R', 1), ('U', 3), ('R', 3) });
            Console.WriteLine();
        }

        private static void TestAllFacesRotation()
        {
            Console.WriteLine("=== 测试六面全旋转 ===");
            bool result = TestCombo(InitialState, new[] {
                ('U', 1), ('R', 1), ('F', 1), ('D', 1), ('L', 1), ('B', 1),
                ('B', 3), ('L', 3), ('D', 3), ('F', 3), ('R', 3), ('U', 3)
            });
            Console.WriteLine($"U R F D L B B' L' D' F' R' U': {result}");
            Console.WriteLine();
        }

        private static void TestMultipleRotations()
        {
            Console.WriteLine("=== 测试多次旋转 ===");
            Console.WriteLine($"U2 U2: {TestCombo(InitialState, new[] { ('U', 2), ('U', 2) })}");
            Console.WriteLine($"R2 R2: {TestCombo(InitialState, new[] { ('R', 2), ('R', 2) })}");
            Console.WriteLine($"U3 U: {TestCombo(InitialState, new[] { ('U', 3), ('U', 1) })}");
            Console.WriteLine();
        }

        private static void TestRandomScrambleAndRestore()
        {
            Console.WriteLine("=== 测试随机打乱和还原 ===");
            Random rand = new Random(42);
            for (int test = 1; test <= 5; test++)
            {
                string scrambled = InitialState;
                var moves = new System.Collections.Generic.List<(char face, int times)>();

                for (int i = 0; i < 20; i++)
                {
                    char face = Faces[rand.Next(Faces.Length)][0];
                    int times = rand.Next(1, 4);
                    moves.Add((face, times));
                    scrambled = tool.RotationHelper.RotationStage(scrambled, face, times);
                }

                string restored = scrambled;
                foreach (var (face, times) in Enumerable.Reverse(moves))
                {
                    restored = tool.RotationHelper.RotationStage(restored, face, 4 - times);
                }

                bool success = restored == InitialState;
                Console.WriteLine($"随机测试 {test}: {(success ? "成功 ✓" : "失败 ✗")}");
            }
            Console.WriteLine();
        }

        private static void TestSpecificScramble()
        {
            Console.WriteLine("=== 测试特定打乱状态 ===");
            string[] scrambleMoves = { "R", "U", "R", "U", "R", "U" };
            string scrambledState = InitialState;
            foreach (var move in scrambleMoves)
            {
                scrambledState = tool.RotationHelper.RotationStage(scrambledState, move[0], 1);
            }
            Console.WriteLine($"打乱后: {scrambledState}");

            string restoredState = scrambledState;
            foreach (var move in Enumerable.Reverse(scrambleMoves))
            {
                restoredState = tool.RotationHelper.RotationStage(restoredState, move[0], 3);
            }
            Console.WriteLine($"还原后: {restoredState}");
            Console.WriteLine($"还原成功: {restoredState == InitialState}");
            Console.WriteLine();
        }

        private static void TestAllRotationCounts()
        {
            Console.WriteLine("=== 测试所有面的所有旋转次数 ===");
            bool allCorrect = true;
            foreach (string face in Faces)
            {
                for (int times = 1; times <= 3; times++)
                {
                    string ret = tool.RotationHelper.RotationStage(InitialState, face[0], times);
                    string back = tool.RotationHelper.RotationStage(ret, face[0], 4 - times);
                    if (back != InitialState)
                    {
                        Console.WriteLine($"Face {face} x{times}: 错误 ✗");
                        allCorrect = false;
                    }
                }
            }
            if (allCorrect)
            {
                Console.WriteLine("所有面的所有旋转次数都正确 ✓");
            }
            Console.WriteLine();
        }

        private static void TestLongerFormulas()
        {
            Console.WriteLine("=== 测试更长的公式 ===");
            TestFormulaPeriod("T-Perm", InitialState, new[] {
                ('R', 1), ('U', 1), ('R', 3), ('U', 3), ('R', 3), ('F', 1),
                ('R', 2), ('U', 3), ('R', 3), ('U', 3), ('R', 1), ('U', 1),
                ('R', 3), ('F', 3)
            });

            TestFormulaPeriod("Y-Perm", InitialState, new[] {
                ('F', 1), ('R', 1), ('U', 3), ('R', 3), ('U', 3), ('R', 1),
                ('U', 1), ('R', 3), ('F', 3), ('R', 1), ('U', 1), ('R', 3),
                ('U', 3), ('R', 3), ('F', 1), ('R', 1), ('F', 3)
            });
            Console.WriteLine();
        }

        private static void TestSequentialRotation()
        {
            Console.WriteLine("=== 测试顺序旋转 (U D F B L R x10) ===");
            string ret = InitialState;
            for (int i = 0; i < 60; i++)
            {
                if (i % 6 == 0)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'U', 1);
                }
                else if (i % 6 == 1)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'D', 1);
                }
                else if (i % 6 == 2)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'F', 1);
                }
                else if (i % 6 == 3)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'B', 1);
                }
                else if (i % 6 == 4)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'L', 1);
                }
                else if (i % 6 == 5)
                {
                    ret = tool.RotationHelper.RotationStage(ret, 'R', 1);
                }
            }
            Console.WriteLine($"旋转60次后状态: {ret}");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string solution = Kociemba.Search.solution(ret, out string info, useSeparator: false);
            stopwatch.Stop();
            
            Console.WriteLine($"求解耗时: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"解法: {solution}");
            Console.WriteLine($"信息: {info}");
            Console.WriteLine();
        }

        private static bool TestCombo(string initial, (char face, int times)[] moves)
        {
            string state = initial;
            foreach (var (face, times) in moves)
            {
                state = tool.RotationHelper.RotationStage(state, face, times);
            }
            return state == initial;
        }

        private static void TestFormulaPeriod(string name, string initial, (char face, int times)[] moves)
        {
            string state = initial;
            for (int i = 1; i <= 252; i++)
            {
                foreach (var (face, times) in moves)
                {
                    state = tool.RotationHelper.RotationStage(state, face, times);
                }
                if (state == initial)
                {
                    Console.WriteLine($"{name}: 周期 = {i}");
                    return;
                }
            }
            Console.WriteLine($"{name}: 252次内未回到原位");
        }
    }
}
