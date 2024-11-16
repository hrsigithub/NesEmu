using System;

namespace Emulator
{
    public partial class CPU
    {
        // 主要なレジスタ
        public byte A;   // Accumulator (A)
        public byte X;   // Index Register (X)
        public byte Y;   // Index Register (Y)
        public ushort PC; // Program Counter (PC)
        public byte SP;  // Stack Pointer (SP)
        public byte P;   // Processor Status Register (P)

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
            IndexedY,
            IndexedIndirect,
            IndirectIndexed,
        }




    }
}
