using System;
using System.Collections.Generic;

namespace Emulator
{

    public partial class CPU
    {

        public CPU()
        {
            Reset();
        }

        // プログラムを読み込む
        // プログラムを指定されたアドレスにロードする関数
        public void Load(List<byte> program)
        {
            // List<byte> を配列に変換
            byte[] programArray = program.ToArray();

            // プログラムをメモリの0x8000番地にコピー
            Array.Copy(programArray, 0, memory, 0x8000, programArray.Length);

            WriteMemoryU16(0xFFFC, 0x8000);
        }

        // リセット
        public void Reset()
        {
            Array.Clear(memory, 0, memory.Length); // メモリをクリア

            A = 0;
            X = 0;
            Y = 0;
            SP = 0xFD;    // スタックポインタ初期値
            P = 0x24;     // ステータスレジスタ初期値

            PC = ReadMemoryU16(0xFFFC);
        }

        public void LoadAndRun(List<byte> program)
        {
            // プログラムをメモリにロード
            Load(program);

            // リセット処理を実行
            Reset();

            // プログラムを実行
            Run();
        }

        // 命令の実行を開始する関数
        public void Run()
        {
            while (true)
            {
                byte opcode = Fetch();
                switch (opcode)
                {
                    case 0xA9: // LDA Immediate
                        LDA(Fetch(), AddressingMode.Immediate);
                        break;
                    case 0xE8: // INX
                        INX();
                        break;
                    case 0x00: // BRK
                        BRK();
                        return; // BRK命令で停止
                    default:
                        throw new InvalidOperationException("Unknown opcode");
                }
            }
        }

    }
}
