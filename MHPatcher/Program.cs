using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace MHPatcher
{
    internal class Program
    {
        enum Mode
        {
            x32,
            x64
        }

        static void Main(string[] args)
        {
            const string ExeHash32 = "AABFC231A0BA96229BCAC1C931EAEA777B7470EC";
            const string ExeHash64 = "6DC9BCDB145F98E5C2D7A1F7E25AEB75507A9D1A";

            if (args.Length < 1)
            {
                Console.WriteLine("Drag and drop a Marvel Heroes executable on this tool to patch it.");
                Console.ReadLine();
                return;
            }

            string filePath = args[0];

            Console.WriteLine($"Patching {filePath}...");
            if (File.Exists(filePath) == false)
            {
                Console.WriteLine($"{filePath} is not a valid file path");
                return;
            }

            byte[] data = File.ReadAllBytes(filePath);
            string hash = Convert.ToHexString(SHA1.HashData(data));

            Mode mode;
            if (hash == ExeHash32)
                mode = Mode.x32;
            else if (hash == ExeHash64)
                mode = Mode.x64;
            else
            {
                Console.WriteLine($"Invalid executable! Make sure you are trying to patch an executable of version 1.52.0.1700 (Steam).");
                Console.ReadLine();
                return;
            }

            string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string patchesFile = mode == Mode.x32 ? "Patches32.json" : "Patches64.json";
            Patch[] patches = JsonSerializer.Deserialize<Patch[]>(File.ReadAllText(Path.Combine(root, patchesFile)));

            foreach (var patch in patches)
                ApplyPatch(data, patch);

            string patchedExePath = Path.Combine(Path.GetDirectoryName(filePath), "MarvelHeroesOmegaPatched.exe");
            Console.WriteLine($"Saving patched executable to {patchedExePath}...");
            File.WriteAllBytes(patchedExePath, data);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void ApplyPatch(byte[] data, Patch patch)
        {
            if (patch.IsEnabled == false) return;

            Console.WriteLine($"Applying \"{patch.Name}\"...");
            int offset = Convert.ToInt32(patch.Offset, 16);
            byte[] patchData = Convert.FromHexString(patch.Data);

            if (offset < 0 || offset > data.Length)
            {
                Console.WriteLine($"Patch offset 0x{offset:X} is out of range (max: 0x{data.Length - 1:X})");
                return;
            }

            for (int i = 0; i < patchData.Length; i++)
                data[offset + i] = patchData[i];
        }
    }
}
