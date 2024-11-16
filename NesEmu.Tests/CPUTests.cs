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

    }
}
