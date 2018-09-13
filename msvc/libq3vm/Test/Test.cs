using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Q3VM;

namespace Test {
	class Test {
		const string LibName = "kernel32";
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(LibName, CallingConvention = CConv)]
		static extern bool SetDllDirectory(string Dir);

		static Test() {
			SetDllDirectory(IntPtr.Size == sizeof(long) ? "x64" : "x86");
		}

		static void Main(string[] Args) {
			string Asm;
			string Header;
			VM.GenSyscallAPI(new Type[] { typeof(Syscalls) }, out Asm, out Header);

			File.WriteAllText("qvm_src/syscalls.asm", Asm);
			File.WriteAllText("qvm_src/api_generated.h", Header);

			Q3VMSyscallAttribute.ResetCounter();
			VM V = new VM("out.qvm");
			V.RegisterSyscalls(typeof(Syscalls));

			V.Call(0);
			Console.ReadLine();
		}
	}

	public static class Syscalls {
		[Q3VMSyscall]
		public static void __print(string Str) {
			Console.Write(Str);
		}

		[Q3VMSyscall]
		public static void __printchar(byte Char) {
			Console.Write((char)Char);
		}

		[Q3VMSyscall]
		public static void __printnum(int Num) {
			Console.Write(Num.ToString());
		}

		[Q3VMSyscall]
		public static void __error(string Str) {
			ConsoleColor OldClr = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write(Str);
			Console.ForegroundColor = OldClr;
		}

		[Q3VMSyscall]
		public static void memset(IntPtr Ptr, byte Val, int Size) {
			for (int i = 0; i < Size; i++)
				Marshal.WriteByte(Ptr, i, Val);
		}

		[Q3VMSyscall]
		public static void memcpy(IntPtr Dst, IntPtr Src, int Size) {
			for (int i = 0; i < Size; i++)
				Marshal.WriteByte(Dst, i, Marshal.ReadByte(Src, i));
		}

		[Q3VMSyscall]
		public static IntPtr Test(IntPtr In) {
			Console.WriteLine("Test(0x{0:X})", In.ToInt64(), In.ToInt64());

			return In + 1;
		}
	}
}
