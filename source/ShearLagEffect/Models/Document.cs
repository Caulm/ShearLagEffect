using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace AppShearLagEffect.Models;

//项目文档
public class Document : DbContext
{
    public static Document? GetCurrent()
    {
        current ??= new();
        return current;
    }

    private static Document? current;

    //保存路径
    public string Path { get; }

    #region 表数据
    //项目信息
    public DbSet<Project> Projects { get; set; }
    //剪力滞折减
    public DbSet<ShearLagEffect> ShearLagEffects { get; set; }
    //局部稳定折减
    public DbSet<LocalStability> LocalStabilities { get; set; }
    #endregion

    private Document()
    {
        var appPath = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Resource1.AppName);
        if (Directory.Exists(appPath) && Directory.GetFiles(appPath).Length > 0)
        {
            try { Directory.Delete(appPath, true); } catch { }
        }
        Directory.CreateDirectory(appPath);
        Path = System.IO.Path.Join(appPath, Guid.NewGuid() + Resource1.FileExtension);
        Database.EnsureDeleted();
        Database.EnsureCreated();
        Initiate();
        SaveChanges();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path}");
        optionsBuilder.UseLazyLoadingProxies();
    }

    public void Close()
    {
        Database.EnsureDeleted();
        Dispose();
        current = null;
    }

    private void Initiate()
    {
        //项目信息
        if (!Projects.Any()) Add(new Project());
        //剪力滞折减
        if (!ShearLagEffects.Any())
        {
            Add(new ShearLagEffect(1) { Name = "工况1" });
        }
        //局部稳定折减
        if (!LocalStabilities.Any())
        {
            Add(new LocalStability(1) { Name = "工况1" });
        }
    }

}
