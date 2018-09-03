using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace qvmbuild {
	class Program {
		static string AsmDir = "qvm_asm";
		static string ApiDir = "qvm_src";
		static string OutFileName = "out.qvm";

		static List<string> AsmFiles = new List<string>();
		static List<string> CFiles = new List<string>();
		static List<string> IncludeDirs = new List<string>();

		static bool CleanAsmFiles;
		static bool UseAPI;

		static void Main(string[] Args) {
			if (Args.Length == 0) {
				Console.WriteLine("qvmbuild [args] -f file1.c file2.c ...");
				Console.WriteLine();
				Console.WriteLine("    -a           Delete assember files after compilation");
				Console.WriteLine("    -Oname       Output file");
				Console.WriteLine("    -f           (Optional) Consume following list of files as compiler input");
				Console.WriteLine("    -x           Compile without using the default API");
				return;
			}

			const string ProgDir = "E:\\Projects2018\\q3vm\\bin\\win32";
			AddToPath(ProgDir);

			CleanAsmFiles = true;
			UseAPI = true;

			for (int i = 0; i < Args.Length; i++) {
				string Arg = Args[i];

				if (Arg == "-a")
					CleanAsmFiles = true;
				else if (Arg == "x")
					UseAPI = false;
				else if (Arg == "-f") {
					CFiles.AddRange(Args.Skip(i + 1).ToArray());
					break;
				} else if (Arg.StartsWith("-O")) {
					OutFileName = Arg.Substring(2);
				} else {
					if (Arg.ToLower().EndsWith(".c") && File.Exists(Arg)) {
						CFiles.AddRange(Args.Skip(i).ToArray());
						break;
					}

					Console.WriteLine("Invalid argument '{0}'", Arg);
					Exit(1);
				}
			}

			if (UseAPI) {
				IncludeDirs.Add(ApiDir);
				CFiles.Insert(0, Path.Combine(ApiDir, "api.c"));
			}

			EnsureDirExists(AsmDir);
			Compile(CFiles.ToArray());

			if (UseAPI) {
				string[] APIAsmFiles = Directory.GetFiles(ApiDir, "*.asm", SearchOption.AllDirectories);

				foreach (var APIAsmFile in APIAsmFiles) {
					string Copy = Path.Combine(AsmDir, Path.GetFileName(APIAsmFile));
					File.Copy(APIAsmFile, Copy, true);
					AsmFiles.Add(Copy);
				}
			}

			Assemble(AsmFiles.ToArray());
			AtExit();

			//if (Debugger.IsAttached)
			Console.ReadLine();
		}

		static void AtExit() {
			if (CleanAsmFiles && AsmFiles != null)
				foreach (var Asm in AsmFiles)
					if (File.Exists(Asm))
						File.Delete(Asm);

			if (CleanAsmFiles)
				DeleteEmptyDir(AsmDir);
		}

		static void DeleteEmptyDir(string Dir) {
			if (Directory.Exists(Dir) && Directory.EnumerateFileSystemEntries(Dir).Count() == 0)
				Directory.Delete(Dir);
		}

		static void Exit(int Res) {
			if (Debugger.IsAttached)
				Debugger.Break();

			AtExit();
			Environment.Exit(Res);
		}

		static void EnsureDirExists(string Dir) {
			if (!Directory.Exists(Dir))
				Directory.CreateDirectory(Dir);
		}

		static string Quotify(string Src) {
			Src = Src.Trim();

			if (Src.Contains(" "))
				Src = string.Format("\"{0}\"", Src);

			return Src;
		}

		static void Compile(string[] SrcFiles) {
			for (int i = 0; i < SrcFiles.Length; i++) {
				string SrcFile = SrcFiles[i];
				string OutFile = Path.Combine(AsmDir, Path.GetFileNameWithoutExtension(SrcFile) + ".asm");

				string Includes = "";
				if (IncludeDirs.Count != 0)
					Includes = string.Join(" ", IncludeDirs.Select(In => "-I" + Quotify(In)).ToArray());

				Exec("lcc", string.Format("-o {0} {1} -S -Wf-target=bytecode -Wf-g {2}", OutFile, Includes, Quotify(SrcFile)));
				AsmFiles.Add(Path.GetFullPath(OutFile));
			}
		}

		static void Assemble(string[] SrcFiles) {
			for (int i = 0; i < SrcFiles.Length; i++)
				SrcFiles[i] = Quotify(SrcFiles[i]);

			Exec("q3asm", string.Format("-o {0} {1}", OutFileName, string.Join(" ", SrcFiles)));
		}

		static void Exec(string Prog, string CmdLine) {
			ConsoleColor Old = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("{0} {1}", Prog, CmdLine);
			Console.ForegroundColor = Old;

			Process P = new Process();
			P.StartInfo = new ProcessStartInfo(Prog, CmdLine) { UseShellExecute = false };
			P.Start();
			P.WaitForExit();
		}

		static void AddToPath(string Dir) {
			string Pth = Environment.GetEnvironmentVariable("path");

			if (!Pth.Contains(Dir))
				Environment.SetEnvironmentVariable("path", Pth + ";" + Dir);
		}
	}
}
