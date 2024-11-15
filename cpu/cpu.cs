using System;
using System.Collections.Generic;

namespace Emulator
{

    public partial class CPU
    {
        // メモリのサイズ（64KB）
        private byte[] memory = new byte[0x10000];

        // ステータスフラグ (Processor Status Register) のビット位置
        [Flags]
        public enum Flags : byte
        {
            C = 1 << 0,  // Carry Flag
            Z = 1 << 1,  // Zero Flag
            I = 1 << 2,  // Interrupt Disable
            D = 1 << 3,  // Decimal Mode (NESでは使われません)
            B = 1 << 4,  // Break Command
            U = 1 << 5,  // Unused (常に1)
            V = 1 << 6,  // Overflow Flag
            N = 1 << 7   // Negative Flag
        }

        // アドレッシングモード
        public enum AddressingMode
        {
            Immediate,
            ZeroPage,
            ZeroPageX,
            ZeroPageY,
            Absolute,
            AbsoluteX,
            AbsoluteY,
            IndexedX,
            IndexedY
        }

        // 主要なレジスタ
        public byte A;   // Accumulator (A)
        public byte X;   // Index Register (X)
        public byte Y;   // Index Register (Y)
        public ushort PC; // Program Counter (PC)
        public byte SP;  // Stack Pointer (SP)
        public byte P;   // Processor Status Register (P)

        public CPU()
        {
            Reset();
        }

        // プログラムを読み込む
        // プログラムを指定されたアドレスにロードする関数
        public void LoadProgram(List<byte> program)
        {
            program.CopyTo(memory, 0x8000); // メモリの0x8000番地からプログラムを配置
            PC = 0x8000; // プログラム開始位置をPCに設定
        }

        // リセット
        public void Reset()
        {
            A = 0;
            X = 0;
            Y = 0;
            PC = 0x8000;  // プログラム開始アドレス
            SP = 0xFD;    // スタックポインタ初期値
            P = 0x24;     // ステータスレジスタ初期値
            Array.Clear(memory, 0, memory.Length); // メモリをクリア
        }

        // 実行
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

        // ステータスフラグを設定
        public void SetFlag(Flags flag, bool value)
        {
            if (value)
                P |= (byte)flag;
            else
                P &= (byte)~flag;
        }

        // ステータスフラグを取得
        public bool GetFlag(Flags flag)
        {
            return (P & (byte)flag) != 0;
        }

        // メモリを読む
        public byte ReadMemory(ushort address)
        {
            return memory[address]; // メモリから値を読み込む
        }

        // メモリに書き込む
        public void WriteMemory(ushort address, byte value)
        {
            memory[address] = value;
        }

        // スタックに値をプッシュ
        private void PushStack(byte value)
        {
            WriteMemory((ushort)(0x0100 + SP), value);
            SP--;
        }

        // ステップ実行（1命令分の実行）
        public void Step()
        {
            byte opcode = Fetch();
            // 命令の実行はここで行います（例：LDA, STA, etc.）
        }

        // メモリのダンプ
        // メモリのダンプ（完全に一致した方法で出力）
        public void PrintMemory(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(this.ReadMemory((ushort)i) + " ");
            }
            Console.WriteLine();
        }

        // 引数にアドレスを追加して、異なる名前に変更
        public void PrintMemoryFromAddress(ushort address, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(this.ReadMemory((ushort)(address + i)) + " ");
            }
            Console.WriteLine();
        }

        public void PrintMemory(ushort address, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(this.ReadMemory((ushort)(address + i)) + " ");
            }
            Console.WriteLine();
        }


        // フェッチ（メモリから命令を取得）
        private byte Fetch()
        {
            byte result = ReadMemory(PC);
            PC++;
            return result;
        }
    }
}
