using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.Editor
{
	internal class ActEditorGlobalStuff
	{
		private const string OBSOLETE_PREFS_SIGN_AFTER_PROCESSING = "ACTsignAfterProcessing";
		private const string OBSOLETE_PREFS_INJECTION_SCAN_POSTPONED = "ACTDIDRecollect";
		private const string OBSOLETE_PREFS_AUTO_SIGN = "ACTautoSign";
		 
		internal const string PREFS_INJECTION_GLOBAL = "ACTDIDEnabledGlobal";
		internal const string PREFS_INJECTION = "ACTDIDEnabled";
		internal const string REPORT_EMAIL = "focus@codestage.ru";
		
		internal const string INJECTION_SERVICE_FOLDER = "InjectionDetectorData";
		internal const string INJECTION_DEFAULT_WHITELIST_FILE = "DefaultWhitelist.bytes";
		internal const string INJECTION_USER_WHITELIST_FILE = "UserWhitelist.bytes";
		internal const string INJECTION_DATA_FILE = "fndid.bytes";
		internal const string INJECTION_DATA_SEPARATOR = ":";

		internal const string ASSEMBLIES_PATH_RELATIVE = "Library/ScriptAssemblies";

		internal static readonly string ASSETS_PATH = Application.dataPath;
		internal static readonly string RESOURCES_PATH = ASSETS_PATH + "/Resources/";
		internal static readonly string ASSEMBLIES_PATH = ASSETS_PATH + "/../" + ASSEMBLIES_PATH_RELATIVE;

		internal static readonly string INJECTION_DATA_PATH = RESOURCES_PATH + INJECTION_DATA_FILE;

		internal static readonly string OBSOLETE_FINGERPRINTS_PATH = RESOURCES_PATH + "fn.txt";
		internal static readonly string OBSOLETE_INJECTION_DATA_PATH = RESOURCES_PATH + "fndid.txt";

		private static readonly string[] hexTable = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
		
		/*internal static readonly string[] UNITY_ASSEMBLIES_NAMES =
		{
			"Assembly-CSharp", "Assembly-CSharp-firstpass",
			"Assembly-UnityScript", "Assembly-UnityScript-firstpass",
			"Assembly-Boo", "Assembly-Boo-firstpass"
		};*/

		internal static void RemoveObsoleteFiles()
		{
			if (!File.Exists(OBSOLETE_FINGERPRINTS_PATH) && !File.Exists(OBSOLETE_INJECTION_DATA_PATH))
			{
				return;
			}

			RemoveReadOnlyAttribute(OBSOLETE_FINGERPRINTS_PATH);
			RemoveReadOnlyAttribute(OBSOLETE_FINGERPRINTS_PATH + ".meta");
			RemoveReadOnlyAttribute(OBSOLETE_INJECTION_DATA_PATH);
			RemoveReadOnlyAttribute(OBSOLETE_INJECTION_DATA_PATH + ".meta");

			FileUtil.DeleteFileOrDirectory(OBSOLETE_FINGERPRINTS_PATH);
			FileUtil.DeleteFileOrDirectory(OBSOLETE_FINGERPRINTS_PATH + ".meta");
			FileUtil.DeleteFileOrDirectory(OBSOLETE_INJECTION_DATA_PATH);
			FileUtil.DeleteFileOrDirectory(OBSOLETE_INJECTION_DATA_PATH + ".meta");

			RemoveDirectoryIfEmpty(RESOURCES_PATH);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}

		internal static void RemoveObsoletePrefs()
		{
			if (EditorPrefs.HasKey(OBSOLETE_PREFS_SIGN_AFTER_PROCESSING))
				EditorPrefs.DeleteKey(OBSOLETE_PREFS_SIGN_AFTER_PROCESSING);

			if (EditorPrefs.HasKey(OBSOLETE_PREFS_AUTO_SIGN))
				EditorPrefs.DeleteKey(OBSOLETE_PREFS_AUTO_SIGN);

			if (EditorPrefs.HasKey(OBSOLETE_PREFS_INJECTION_SCAN_POSTPONED))
				EditorPrefs.DeleteKey(OBSOLETE_PREFS_INJECTION_SCAN_POSTPONED);
		}

		internal static void CleanInjectionDetectorData()
		{
			if (!File.Exists(INJECTION_DATA_PATH))
			{
				return;
			}

			RemoveReadOnlyAttribute(INJECTION_DATA_PATH);
			RemoveReadOnlyAttribute(INJECTION_DATA_PATH + ".meta");

			FileUtil.DeleteFileOrDirectory(INJECTION_DATA_PATH);
			FileUtil.DeleteFileOrDirectory(INJECTION_DATA_PATH + ".meta");

			RemoveDirectoryIfEmpty(RESOURCES_PATH);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}

		internal static string ResolveInjectionDefaultWhitelistPath()
		{
			return ResolveInjectionServiceFolder() + "/" + INJECTION_DEFAULT_WHITELIST_FILE;
		}

		internal static string ResolveInjectionUserWhitelistPath()
		{
			return ResolveInjectionServiceFolder() + "/" + INJECTION_USER_WHITELIST_FILE;
		}
		
		internal static string ResolveInjectionServiceFolder()
		{
			string result = "";
			string[] targetFiles = Directory.GetDirectories(ASSETS_PATH, INJECTION_SERVICE_FOLDER, SearchOption.AllDirectories);
			if (targetFiles.Length == 0)
			{
				Debug.LogError("[ACT] Can't find " + INJECTION_SERVICE_FOLDER + " folder! Please report to " + REPORT_EMAIL);
			}
			else
			{
				result = targetFiles[0];
			}

			return result;
		}

		internal static string[] FindLibrariesAt(string dir)
		{
			string[] result = new string[0];

			if (Directory.Exists(dir))
			{
				result = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = result[i].Replace('\\', '/');
				}
			}

			return result;
		}

		private static string PublicKeyTokenToString(byte[] bytes)
		{
			string result = "";

			// AssemblyName.GetPublicKeyToken() returns 8 bytes
			for (int i = 0; i < 8; i++)
			{
				result += hexTable[bytes[i]];
			}

			return result;
		}

		private static void RemoveDirectoryIfEmpty(string directoryName)
		{
			if (Directory.Exists(directoryName) && IsDirectoryEmpty(directoryName))
			{
				FileUtil.DeleteFileOrDirectory(directoryName);
				if (File.Exists(Path.GetDirectoryName(directoryName) + ".meta"))
				{
					FileUtil.DeleteFileOrDirectory(Path.GetDirectoryName(directoryName) + ".meta");
				}
			}
		}

		private static bool IsDirectoryEmpty(string path)
		{
			string[] dirs = Directory.GetDirectories(path);
			string[] files = Directory.GetFiles(path);
			return dirs.Length == 0 && files.Length == 0;
		}

		internal static int GetAssemblyHash(AssemblyName ass)
		{
			string hashInfo = ass.Name;

			byte[] bytes = ass.GetPublicKeyToken();
			if (bytes != null && bytes.Length == 8)
			{
				hashInfo += PublicKeyTokenToString(bytes);
			}

			// Jenkins hash function (http://en.wikipedia.org/wiki/Jenkins_hash_function)
			int result = 0;
			int len = hashInfo.Length;

			for (int i = 0; i < len; ++i)
			{
				result += hashInfo[i];
				result += (result << 10);
				result ^= (result >> 6);
			}
			result += (result << 3);
			result ^= (result >> 11);
			result += (result << 15);

			return result;
		}

		internal static void RemoveReadOnlyAttribute(string path)
		{
			if (File.Exists(path))
			{
				FileAttributes attributes = File.GetAttributes(path);
				if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					attributes = attributes & ~FileAttributes.ReadOnly;
					File.SetAttributes(path, attributes);
				}
			}
		}

	}
}