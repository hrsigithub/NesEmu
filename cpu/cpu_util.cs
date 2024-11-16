using System;

namespace Emulator
{
    public partial class CPU
    {
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

        // フェッチ（メモリから命令を取得）
        private byte Fetch()
        {
            byte result = ReadMemory(PC);
            PC++;
            return result;
        }


        // メモリを読む
        public byte ReadMemory(ushort address)
        {
            return memory[address]; // メモリから値を読み込む
        }

        // メモリの特定位置から2バイト（16ビット）を読み込む
        public ushort ReadMemoryU16(ushort address)
        {
            byte lo = ReadMemory(address);           // 下位バイトを読み込む
            byte hi = ReadMemory((ushort)(address + 1)); // 上位バイトを読み込む
            return (ushort)((hi << 8) | lo);         // 上位バイトと下位バイトを結合して返す
        }

        // メモリに書き込む
        public void WriteMemory(ushort address, byte value)
        {
            memory[address] = value;
        }

        // メモリの特定位置に2バイト（16ビット）を書き込む
        public void WriteMemoryU16(ushort address, ushort value)
        {
            byte lo = (byte)(value & 0xFF);           // 下位バイトを抽出
            byte hi = (byte)((value >> 8) & 0xFF);    // 上位バイトを抽出
            WriteMemory(address, lo);                 // 下位バイトを書き込む
            WriteMemory((ushort)(address + 1), hi);   // 上位バイトを書き込む
        }



        // メモリのダンプ（完全に一致した方法で出力）
        public void PrintMemory(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(this.ReadMemory((ushort)i) + " ");
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

        // 引数にアドレスを追加して、異なる名前に変更
        public void PrintMemoryFromAddress(ushort address, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(this.ReadMemory((ushort)(address + i)) + " ");
            }
            Console.WriteLine();
        }




    }
}
