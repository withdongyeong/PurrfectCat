using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostBuildProcess
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string buildDir = Path.GetDirectoryName(pathToBuiltProject);
        string sourceDir = Path.Combine(Directory.GetCurrentDirectory(), "AudioAnalyzer");
        string targetDir = Path.Combine(buildDir, "AudioAnalyzer");

        if (Directory.Exists(targetDir))
            Directory.Delete(targetDir, true);

        DirectoryCopy(sourceDir, targetDir, true);
        UnityEngine.Debug.Log("[PostBuild] AudioAnalyzer 전체 복사 완료");
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destDirName);

        foreach (FileInfo file in dir.GetFiles())
            file.CopyTo(Path.Combine(destDirName, file.Name), true);

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
                DirectoryCopy(subdir.FullName, Path.Combine(destDirName, subdir.Name), copySubDirs);
        }
    }
}