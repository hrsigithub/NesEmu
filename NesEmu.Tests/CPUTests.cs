using Emulator; // メインプロジェクトの名前空間
using Xunit;

namespace NesEmu.Tests
{
    public class CPUTests
    {
        [Fact]
        public void test_lda_from_memory()
        {
            var cpu = new CPU();
            cpu.WriteMemory(0x10, 0x55);

            List<byte> program = new List<byte> { 0xA5, 0x10, 0x8D, 0x00 };
            cpu.LoadAndRun(program);

            Assert.NotEqual(0x55, cpu.A);
        }

        [Fact]
        public void LDA_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42; // 任意の値
            ushort address = 0x2000; // 任意のアドレス

            // メモリに値を設定
            cpu.WriteMemory(address, expectedValue);

            // Act
            cpu.LDA(address, CPU.AddressingMode.Absolute);

            // Assert
            Assert.Equal(expectedValue, cpu.A); // Aレジスタに期待通りの値がロードされているか確認
        }

        [Fact]
        public void LDA_ImmediateMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42; // 即値
            ushort address = 0; // Immediate モードではアドレスは使わないが仮に設定

            // メモリに値を設定 (Immediateモードではメモリに直接アクセスしないので省略可能)
            cpu.WriteMemory(address, expectedValue);

            // Act
            cpu.LDA(expectedValue, CPU.AddressingMode.Immediate);

            // Assert
            Assert.Equal(expectedValue, cpu.A); // Aレジスタに期待値が設定されているか確認
        }

        [Fact]
        public void LDA_ZeroPageMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            ushort address = 0x00FF; // ZeroPage のアドレスは 0x00~0xFF

            // メモリに値を設定
            cpu.WriteMemory(address, expectedValue);

            // Act
            cpu.LDA(address, CPU.AddressingMode.ZeroPage);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }

        [Fact]
        public void LDA_AbsoluteMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            ushort address = 0x2000; // 任意の絶対アドレス

            // メモリに値を設定
            cpu.WriteMemory(address, expectedValue);

            // Act
            cpu.LDA(address, CPU.AddressingMode.Absolute);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }

        [Fact]
        public void LDA_ZeroPageXMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            ushort baseAddress = 0x00F0; // ZeroPage
            cpu.X = 0x0F; // Xレジスタの値
            ushort effectiveAddress = (ushort)((baseAddress + cpu.X) & 0xFF); // ZeroPageアドレス範囲にマスク

            // メモリに値を設定
            cpu.WriteMemory(effectiveAddress, expectedValue);

            // Act
            cpu.LDA(baseAddress, CPU.AddressingMode.ZeroPageX);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }

        [Fact]
        public void LDA_AbsoluteXMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            ushort baseAddress = 0x2000;
            cpu.X = 0x10; // Xレジスタの値
            ushort effectiveAddress = (ushort)(baseAddress + cpu.X);

            // メモリに値を設定
            cpu.WriteMemory(effectiveAddress, expectedValue);

            // Act
            cpu.LDA(baseAddress, CPU.AddressingMode.AbsoluteX);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }

        [Fact]
        public void LDA_AbsoluteYMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            ushort baseAddress = 0x2000;
            cpu.Y = 0x10; // Yレジスタの値
            ushort effectiveAddress = (ushort)(baseAddress + cpu.Y);

            // メモリに値を設定
            cpu.WriteMemory(effectiveAddress, expectedValue);

            // Act
            cpu.LDA(baseAddress, CPU.AddressingMode.AbsoluteY);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }

        [Fact]
        public void LDA_IndexedIndirectMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            byte baseAddress = 0x80; // ZeroPage
            cpu.X = 0x10; // Xレジスタの値
            ushort effectiveAddress = 0x3000; // 間接アドレス

            // ZeroPage アドレスに間接アドレスを書き込む
            ushort indirectAddress = (ushort)((baseAddress + cpu.X) & 0xFF);
            cpu.WriteMemory(indirectAddress, (byte)(effectiveAddress & 0xFF)); // Low byte
            cpu.WriteMemory((ushort)((indirectAddress + 1) & 0xFF), (byte)(effectiveAddress >> 8)); // High byte

            // メモリに値を設定
            cpu.WriteMemory(effectiveAddress, expectedValue);

            // Act
            cpu.LDA(baseAddress, CPU.AddressingMode.IndexedIndirect);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }
        [Fact]
        public void LDA_IndirectIndexedMode_LoadsValueIntoAccumulator()
        {
            // Arrange
            var cpu = new CPU();
            byte expectedValue = 0x42;
            byte baseAddress = 0x80; // ZeroPage
            cpu.Y = 0x10; // Yレジスタの値
            //ushort indirectAddress = 0x3000; // 実際のデータが格納されているアドレス
            ushort baseIndirectAddress = 0x2000; // ZeroPageに格納されているアドレスのベース

            // ZeroPage に間接アドレスを書き込む
            cpu.WriteMemory(baseAddress, (byte)(baseIndirectAddress & 0xFF)); // Low byte
            cpu.WriteMemory((ushort)(baseAddress + 1), (byte)(baseIndirectAddress >> 8)); // High byte

            // 実効アドレスを計算
            ushort effectiveAddress = (ushort)(baseIndirectAddress + cpu.Y);

            // メモリに値を設定
            cpu.WriteMemory(effectiveAddress, expectedValue);

            // Act
            cpu.LDA(baseAddress, CPU.AddressingMode.IndirectIndexed);

            // Assert
            Assert.Equal(expectedValue, cpu.A);
        }
    }
}
