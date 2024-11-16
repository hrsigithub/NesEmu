using System;

namespace Emulator
{
    public partial class CPU
    {
        public void LDA(ushort address, AddressingMode mode)
        {
            byte value;

            switch (mode)
            {
                case AddressingMode.Immediate:
                    value = (byte)(address & 0xFF); // 即値モード：addressは即値として使用
                    break;
                case AddressingMode.ZeroPage:
                    value = ReadMemory(address); // ゼロページモード：低位8ビットがゼロページアドレス
                    break;
                case AddressingMode.ZeroPageX: // ゼロページ + X
                    value = ReadMemory((ushort)((address + X) & 0xFF));
                    break;

                case AddressingMode.AbsoluteX: // 絶対アドレス + X
                    value = ReadMemory((ushort)(address + X));
                    break;

                case AddressingMode.AbsoluteY: // 絶対アドレス + Y
                    value = ReadMemory((ushort)(address + Y));
                    break;


                case AddressingMode.Absolute:
                    value = ReadMemory(address); // 絶対アドレスモード
                    break;
                case AddressingMode.IndexedX:
                    value = ReadMemory((ushort)(address + X)); // 絶対アドレス + X (インデックスモード X)
                    break;
                case AddressingMode.IndexedY:
                    value = ReadMemory((ushort)(address + Y)); // 絶対アドレス + Y (インデックスモード Y)
                    break;
                case AddressingMode.IndexedIndirect: // (Indirect,X)
                                                     // ZeroPage アドレスを X レジスタでインデックス
                    ushort indirectAddress = (ushort)((address + X) & 0xFF);
                    // 間接アドレスを取得
                    ushort effectiveAddress = (ushort)(ReadMemory(indirectAddress) | (ReadMemory((ushort)((indirectAddress + 1) & 0xFF)) << 8));
                    value = ReadMemory(effectiveAddress);
                    break;
                case AddressingMode.IndirectIndexed: // (Indirect),Y
                                                     // ZeroPage アドレスから間接アドレスを取得
                    ushort baseAddress = (ushort)(ReadMemory(address) | (ReadMemory((ushort)((address + 1) & 0xFF)) << 8));
                    // Y レジスタでインデックス
                    ushort effectiveAddressIndexed = (ushort)(baseAddress + Y);
                    value = ReadMemory(effectiveAddressIndexed);
                    break;



                default:
                    throw new ArgumentException("Unsupported addressing mode");
            }

            A = value; // Aレジスタに値をロード

            SetFlag(Flags.Z, A == 0);   // ゼロフラグの設定
            SetFlag(Flags.N, (A & 0x80) != 0); // 負フラグの設定
        }

        // STA命令の実装
        public void STA(ushort address, AddressingMode mode)
        {
            ushort effectiveAddress = address;

            switch (mode)
            {
                case AddressingMode.ZeroPage:
                    effectiveAddress = (ushort)(address & 0xFF); // ゼロページアドレス
                    break;
                case AddressingMode.Absolute:
                    // 絶対アドレスモードはそのまま使用
                    break;
                case AddressingMode.IndexedX:
                    effectiveAddress = (ushort)((address + X) & 0xFFFF); // Xインデックス付き
                    break;
                case AddressingMode.IndexedY:
                    effectiveAddress = (ushort)((address + Y) & 0xFFFF); // Yインデックス付き
                    break;
                default:
                    throw new ArgumentException("Unsupported addressing mode in STA");
            }

            // メモリに書き込む前に、アドレスが有効であるか確認
            if (effectiveAddress <= 0xFFFF)
            {
                WriteMemory(effectiveAddress, A); // Aレジスタの内容をメモリに書き込む
            }
            else
            {
                // アドレスが範囲外の場合のエラーハンドリング（必要に応じて追加）
                Console.WriteLine($"Error: Invalid address 0x{effectiveAddress:X}");
            }
        }

        public void TAX()
        {
            X = A;                       // アキュムレータの値をXレジスタにコピー
            SetFlag(Flags.Z, X == 0);   // ゼロフラグを設定
            SetFlag(Flags.N, (X & 0x80) != 0); // 負フラグを設定
        }

        public void BRK()
        {
            PC += 2; // BRK命令は2バイトなので、次の命令に進める

            // ステータスレジスタの内容とPCをスタックに保存
            PushStack((byte)((PC >> 8) & 0xFF)); // 上位バイト
            PushStack((byte)(PC & 0xFF));        // 下位バイト

            // フラグの設定。割り込みフラグ（I）をセットし、ブレークフラグ（B）もセット
            SetFlag(Flags.B, true); // Bフラグをセット
            SetFlag(Flags.I, true); // Iフラグをセット

            // ステータスレジスタに予約ビットを含めてスタックにプッシュ
            byte statusWithReservedBits = (byte)(P | 0x30); // 0b0011xxxx（予約ビットを含む）
            PushStack(statusWithReservedBits);

            // 割り込みベクタから新しいPCを取得
            byte low = ReadMemory(0xFFFE);
            byte high = ReadMemory(0xFFFF);
            PC = (ushort)((high << 8) | low);
        }

        public void INX()
        {
            X += 1; // Xレジスタの値をインクリメント

            // ゼロフラグの設定：Xレジスタが0であればZフラグをセット
            SetFlag(Flags.Z, X == 0);

            // 負フラグの設定：Xレジスタの最上位ビットが1ならばNフラグをセット
            SetFlag(Flags.N, (X & 0x80) != 0);
        }




    }
}
