using NUnit.Framework;
using Google.Protobuf.Editor;
using Google.Protobuf.Config;
using System.IO;
using UnityEngine;
using UnityEditor;

public class TestReferences
{
    public const string TestingFilePath = "Tests/Proto/test.proto";
    public const string RelativeTestingFilePath = "Assets/" + TestingFilePath;
    public static string AbsoluteTestingFilePath = Application.dataPath + "/" + TestingFilePath;

    public static string GlobalProtobufConfigPath = Application.dataPath + "/Tests/Global/ProtobufConfig.asset";
    public static string LocalProtobufConfigPath = Application.dataPath + "/Tests/Proto/ProtobufConfig.asset";

    public static string GlobalCSharpFilePath = Application.dataPath + "/Tests/Proto/Test.cs";
    public static string GlobalGoLangFilePath = Application.dataPath + "/Tests/Proto/test.pb.go";

    public const string CSharpTargetPath = "Tests/Generated/CSharp/";
    public static string CSharpFilePath = Application.dataPath + "/" + CSharpTargetPath + "Test.cs";

    public const string GoLangTargetPath = "Tests/Generated/Go/";
    public static string GoLangFilePath = Application.dataPath + "/" + GoLangTargetPath + "test.pb.go";
}

public class TestProtobufConfiguration
{
    [Test]
    public void PathConversion_AbsoluteToRelative()
    {
        // absolute to relative
        var relativePath = ProtobufCompiler.AbsoluteToRelative(TestReferences.AbsoluteTestingFilePath);
        Assert.AreEqual(TestReferences.RelativeTestingFilePath,relativePath);
    }

    [Test]
    public void PathConversion_RelativeToAbsolute()
    {
        var absolutePath = ProtobufCompiler.RelativeToAbsolute(TestReferences.TestingFilePath);
        Assert.AreEqual(TestReferences.AbsoluteTestingFilePath,absolutePath);
        absolutePath = ProtobufCompiler.RelativeToAbsolute(TestReferences.RelativeTestingFilePath);
        Assert.AreEqual(TestReferences.AbsoluteTestingFilePath,absolutePath);
    }

    [Test]
    public void CreateProtoFile()
    {
        string fullPath = ProtobufCompiler.RelativeToAbsolute(TestReferences.TestingFilePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        ProtobufCompiler.CreateProtobufFile(fullPath);
        Assert.True(File.Exists(fullPath));
    }

    [Test]
    public void GetGlobalProtoConfig()
    {
        var config = ProtobufCompiler.GetProtobufConfig();
        Assert.IsNotNull(config);
    }

    [Test]
    public void GetLocalProtoConfig()
    {
        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.LocalProtobufConfigPath);
        var referenceConfig = AssetDatabase.LoadAssetAtPath<ProtobufConfig>(relative);
        Assert.IsNotNull(referenceConfig);

        var config = ProtobufCompiler.GetProtobufConfig(TestReferences.AbsoluteTestingFilePath);
        Assert.AreSame(referenceConfig,config);
    }
    
    [Test]
    public void GlobalProtoConfigValuesAreSet()
    {
        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.GlobalProtobufConfigPath);
        var referenceConfig = AssetDatabase.LoadAssetAtPath<ProtobufConfig>(relative);
        Assert.IsNotNull(referenceConfig);
        Assert.AreEqual(true,referenceConfig.ProtocolCompilerEnabled);
        Assert.AreEqual(true,referenceConfig.CSharpCompilerEnabled);
        Assert.True(string.IsNullOrEmpty(referenceConfig.CSharpCustomPath));
        Assert.AreEqual(true,referenceConfig.GoLangCompilerEnabled);
        Assert.True(string.IsNullOrEmpty(referenceConfig.GoLangCustomPath));
    }

    [Test]
    public void LocalProtoConfigValuesAreSet()
    {
        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.LocalProtobufConfigPath);
        var referenceConfig = AssetDatabase.LoadAssetAtPath<ProtobufConfig>(relative);
        Assert.IsNotNull(referenceConfig);
        Assert.AreEqual(true,referenceConfig.ProtocolCompilerEnabled);
        Assert.AreEqual(true,referenceConfig.CSharpCompilerEnabled);
        Assert.AreEqual(TestReferences.CSharpTargetPath,referenceConfig.CSharpCustomPath);
        Assert.AreEqual(true,referenceConfig.GoLangCompilerEnabled);
        Assert.AreEqual(TestReferences.GoLangTargetPath,referenceConfig.GoLangCustomPath);
    }
}

public class TestProtobufCompiler
{
    [Test]
    public void CompilesCSharp_Global()
    {
        var target = TestReferences.GlobalCSharpFilePath;
        if(File.Exists(target))
        {
            File.Delete(target);
        }

        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.GlobalProtobufConfigPath);
        var config = AssetDatabase.LoadAssetAtPath<ProtobufConfig>(relative);
        Assert.IsNotNull(config);
        var result = ProtobufCompiler.Compile(TestReferences.AbsoluteTestingFilePath, config);
        Assert.IsTrue(result);
        Assert.True(File.Exists(target));

        if(File.Exists(TestReferences.GlobalCSharpFilePath))
        {
            File.Delete(TestReferences.GlobalCSharpFilePath);
        }
        if(File.Exists(TestReferences.GlobalGoLangFilePath))
        {
            File.Delete(TestReferences.GlobalGoLangFilePath);
        }
    }

    [Test]
    public void CompilesCSharp_CustomPath()
    {
        var target = TestReferences.CSharpFilePath;
        if(File.Exists(target))
        {
            File.Delete(target);
        }
        
        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.LocalProtobufConfigPath);
        var config = ProtobufCompiler.GetProtobufConfig(relative);
        Assert.IsNotNull(config);
        var result = ProtobufCompiler.Compile(TestReferences.AbsoluteTestingFilePath, config);
        Assert.IsTrue(result);
        Assert.True(File.Exists(target));

        if(File.Exists(TestReferences.CSharpFilePath))
        {
            File.Delete(TestReferences.CSharpFilePath);
        }
        if(File.Exists(TestReferences.GoLangFilePath))
        {
            File.Delete(TestReferences.GoLangFilePath);
        }
    }

    [Test]
    public void CompilesGo_Global()
    {
        var target = TestReferences.GlobalGoLangFilePath;
        if(File.Exists(target))
        {
            File.Delete(target);
        }

        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.GlobalProtobufConfigPath);
        var config = AssetDatabase.LoadAssetAtPath<ProtobufConfig>(relative);
        Assert.IsNotNull(config);
        var result = ProtobufCompiler.Compile(TestReferences.AbsoluteTestingFilePath, config);
        Assert.IsTrue(result);
        Assert.True(File.Exists(target));

        if(File.Exists(TestReferences.GlobalCSharpFilePath))
        {
            File.Delete(TestReferences.GlobalCSharpFilePath);
        }
        if(File.Exists(TestReferences.GlobalGoLangFilePath))
        {
            File.Delete(TestReferences.GlobalGoLangFilePath);
        }
    }

    [Test]
    public void CompilesGoLang_CustomPath()
    {
        var target = TestReferences.GoLangFilePath;
        if(File.Exists(target))
        {
            File.Delete(target);
        }
        
        var relative = ProtobufCompiler.AbsoluteToRelative(TestReferences.LocalProtobufConfigPath);
        var config = ProtobufCompiler.GetProtobufConfig(relative);
        Assert.IsNotNull(config);
        var result = ProtobufCompiler.Compile(TestReferences.AbsoluteTestingFilePath, config);
        Assert.IsTrue(result);
        Assert.True(File.Exists(target));

        if(File.Exists(TestReferences.CSharpFilePath))
        {
            File.Delete(TestReferences.CSharpFilePath);
        }
        if(File.Exists(TestReferences.GoLangFilePath))
        {
            File.Delete(TestReferences.GoLangFilePath);
        }
    }
}