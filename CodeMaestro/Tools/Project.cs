using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace CodeMaestro {
    internal class Project {
        public Sync CurrentSync;
        public string TryCompileProject() {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            // Получаем все C# файлы в проекте
            var csFiles = Directory.GetFiles(CurrentSync.ProjectPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in csFiles) {
                var code = File.ReadAllText(file);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                syntaxTrees.Add(syntaxTree);
            }

            // Опции компиляции
            var compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithAllowUnsafe(true)
                .WithNullableContextOptions(NullableContextOptions.Enable);

            // Сборка
            var compilation = CSharpCompilation.Create("ProjectAssembly")
                .WithOptions(compilationOptions)
                .AddReferences(GetMetadataReferences())
                .AddSyntaxTrees(syntaxTrees);

            // Компиляция в память
            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            // Обработка ошибок
            if (!emitResult.Success) {
                var errors = emitResult.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                    .Select(d => $"{d.Severity}: {d.GetMessage()} at {d.Location}")
                    .ToList();

                return "Compilation failed:\n" + string.Join("\n", errors);
            }

            return "Compilation successful.";
        }
        // Получение метаданных (стандартные .NET библиотеки)
        private IEnumerable<MetadataReference> GetMetadataReferences() {
            var assemblies = new[] {
                typeof(object).Assembly,  // mscorlib
                typeof(Console).Assembly, // System.Console
                Assembly.Load("System.Runtime"),
            };

            return assemblies.Select(a => MetadataReference.CreateFromFile(a.Location));
        }
    }
}