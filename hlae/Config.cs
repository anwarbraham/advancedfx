﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace AfxGui {

public class CfgLauncher
{
    public String CustomCmdLine;
    public Boolean ForceAlpha;
    public Boolean FullScreen;
    public String GamePath;
    public Boolean GfxForce;
    public UInt16 GfxWidth;
    public UInt16 GfxHeight;
    public Byte GfxBpp;
    public String Modification;
    public Boolean OptimizeDesktopRes;
    public Boolean OptimizeVisibilty;
    public Boolean RememberChanges;
    public Byte RenderMode;

    public CfgLauncher()
    {
        ThisDefault();
    }

    internal void Default()
    {
        ThisDefault();
    }

    internal void CopyFrom(CfgLauncher from)
    {
		this.GamePath = String.Copy(from.GamePath);
		this.Modification = String.Copy(from.Modification);
		this.CustomCmdLine = String.Copy(from.CustomCmdLine);
		this.GfxForce = from.GfxForce;
		this.GfxWidth = from.GfxWidth;
		this.GfxHeight = from.GfxHeight;
		this.GfxBpp = from.GfxBpp;
		this.RememberChanges = from.RememberChanges;
		this.ForceAlpha = from.ForceAlpha;
		this.OptimizeDesktopRes = from.OptimizeDesktopRes;
		this.OptimizeVisibilty = from.OptimizeVisibilty;
		this.FullScreen = from.FullScreen;
		this.RenderMode = from.RenderMode;
	}

    private void ThisDefault()
    {
        GamePath = "please select";
        Modification = "cstrike";
        CustomCmdLine = "+toggleconsole";
        GfxForce = true;
        GfxWidth = 800;
        GfxHeight = 600;
        GfxBpp = 32;
        RememberChanges = true;
        ForceAlpha = false;
        OptimizeDesktopRes = false;
        OptimizeVisibilty = true;
        FullScreen = false;
        RenderMode = 0;
    }
}

    public class CfgInjectDll
    {
        public String Path;
    }

public class CfgCustomLoader
{
	public String CmdLine;

        /// <summary>
        /// Do NOT USE. For backwards compat only.
        /// </summary>
        public String HookDllPath;

    public String ProgramPath;

        [XmlArrayItem("Dll")]
        public List<CfgInjectDll> InjectDlls;

    public CfgCustomLoader()
    {
        ThisDefault();
    }

    internal void Default()
    {
        ThisDefault();
    }

        internal void OnDeserialized()
        {
            if(null != this.HookDllPath)
            {
                CfgInjectDll cfgInjectDll = new CfgInjectDll();
                cfgInjectDll.Path = this.HookDllPath;

                this.InjectDlls.Add(cfgInjectDll);

                this.HookDllPath = null;
            }
        }

        internal static void OnSerializeOverrides(XmlAttributeOverrides overrides)
        {
            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlIgnore = true;

            overrides.Add(typeof(CfgCustomLoader), "HookDllPath", attrs);
        }

    private void ThisDefault()
	{
		HookDllPath = null;
		ProgramPath = "";
		CmdLine = "-steam -insecure +sv_lan 1 -window -console -game csgo";
        InjectDlls = new List<CfgInjectDll>();
    }
}


public class CfgDemoTools
{
    public String OutputFolder;

    public CfgDemoTools()
    {
        ThisDefault();
    }

    internal void Default()
    {
        ThisDefault();
    }

    private void ThisDefault()
    {
        OutputFolder = "";
    }
}

public class CfgLauncherCsgo
{
    public String CsgoExe;
    public Boolean MmcfgEnabled;
    public String Mmmcfg;
    public Boolean GfxEnabled;
    public UInt16 GfxWidth;
    public UInt16 GfxHeight;
    public Boolean GfxFull;
    public Boolean AvoidVac;
    public String CustomLaunchOptions;
    public Boolean RememberChanges;

    public CfgLauncherCsgo()
    {
        ThisDefault();
    }

    internal void Default()
    {
        ThisDefault();
    }

    private void ThisDefault()
    {
        CsgoExe = "please select";
        MmcfgEnabled = false;
        Mmmcfg = "";
        GfxEnabled = false;
        GfxWidth = 1280;
        GfxHeight = 720;
        GfxFull = false;
        AvoidVac = true;
        CustomLaunchOptions = "-console";
        RememberChanges = true;
    }
}


public class CfgSettings
{
    public CfgLauncherCsgo LauncherCsgo;
    public CfgLauncher Launcher;
    public CfgCustomLoader CustomLoader;
    public CfgDemoTools DemoTools;
    public SByte UpdateCheck;
    public Guid IgnoreUpdateGuid;

	public CfgSettings()
	{
        LauncherCsgo = new CfgLauncherCsgo();
		Launcher = new CfgLauncher();
		CustomLoader = new CfgCustomLoader();
		DemoTools = new CfgDemoTools();

        ThisDefault();
	}

        internal void OnDeserialized()
        {
            CustomLoader.OnDeserialized();
        }

        internal static void OnSerializeOverrides(XmlAttributeOverrides overrides)
        {
            CfgCustomLoader.OnSerializeOverrides(overrides);
        }

    internal void Default()
	{
        LauncherCsgo.Default();
		Launcher.Default();
		CustomLoader.Default();
		DemoTools.Default();

        ThisDefault();
    }

    private void ThisDefault()
    {
        UpdateCheck = 0;
    }

}

[XmlRootAttribute("HlaeConfig")]
public class Config
{
    //
    // Public members:

	public String Version;
	public CfgSettings Settings;

    public Config()
	{
		Settings = new CfgSettings();

        m_CfgPath = "hlaeconfig.xml";

        ThisDefault();
	}

    //
    // Internal members:

    internal static Config LoadOrCreate(String cfgPath)
    {
        Config config = Load(cfgPath);
        if(null == config)
        {
            config = new Config();
            config.CfgPath = cfgPath;
        }

        return config;
    }

    internal static Config Load(String cfgPath)
	{
        Config config = null;

		if( File.Exists( cfgPath ) )
		{
            FileStream fs = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
    			fs = new FileStream( cfgPath, FileMode.Open );

                config = serializer.Deserialize(fs) as Config;

                    config.OnDeserialized();
            }
            catch(Exception)
            {
                config = null;
            }

            if (null != fs)
                fs.Close();
        }

        if(null != config)
            config.CfgPath = cfgPath;

        return config;
	}

    internal bool BackUp()
	{
		return WriteToFile( m_CfgPath );
	}

    internal bool BackUp(String filePath)
	{
		return WriteToFile( filePath );
	}

        internal void OnDeserialized()
        {
            Settings.OnDeserialized();
        }

        internal static void OnSerializeOverrides(XmlAttributeOverrides overrides)
        {
            CfgSettings.OnSerializeOverrides(overrides);
        }

        internal void Default()
    {
        Settings.Default();
        ThisDefault();
    }

    //
    // Internal properties:

    internal String CfgPath
    {
        get
        {
            return m_CfgPath;
        }
        set
        {
            m_CfgPath = value;
        }
    }

    //
    // Private members:

	String m_CfgPath;

    private void ThisDefault()
    {
        Version = "unknown";
    }

	bool WriteToFile( String filePath )
	{
		bool bOk=false;

        TextWriter writer = null;

		try
		{
                XmlAttributeOverrides xOver = new XmlAttributeOverrides();

                OnSerializeOverrides(xOver);

                XmlSerializer serializer = new XmlSerializer( typeof(Config), xOver );

            writer = new StreamWriter(filePath);

			serializer.Serialize( writer, this );

			bOk = true;
		}
		catch(Exception)
		{
			bOk = false;
		}

        if(null != writer)
            writer.Close();

		return bOk;
	}
}

} // namespace AfxGui
