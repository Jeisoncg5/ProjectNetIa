using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Program {
    static void Main() {
        var path = @"C:\Users\Jeiso\.nuget\packages\npgsql.entityframeworkcore.postgresql\10.0.2\lib\net10.0\Npgsql.EntityFrameworkCore.PostgreSQL.dll";
        var asm = Assembly.LoadFrom(path);
        foreach (var type in asm.GetExportedTypes().Where(t => t.Name.Contains("Vector") || t.FullName.Contains("Vector"))) {
            Console.WriteLine(type.FullName);
        }
    }
}
