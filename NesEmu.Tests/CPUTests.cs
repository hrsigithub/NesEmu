using Emulator; // メインプロジェクトの名前空間
using Xunit;

namespace NesEmu.Tests
{
    public class CPUTests
    {
        [Fact]
        public void Reset_ShouldInitializeRegisters()
        {
            // Arrange
            var cpu = new CPU();

            // Act
            cpu.Reset();

            // Assert
            Assert.Equal(0, cpu.A);
            Assert.Equal(0, cpu.X);
            Assert.Equal(0, cpu.Y);
            Assert.Equal(0x8000, cpu.PC);
            Assert.Equal(0xFD, cpu.SP);
            Assert.Equal(0x24, cpu.P);
        }
    }
}
