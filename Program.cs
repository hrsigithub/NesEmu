using Emulator;

var cpu = new CPU();
var program = new List<byte> { 0xA9, 0x01, 0xE8, 0x00 }; // LDA #$01, INX, BRK
cpu.Load(program);
cpu.Run();

